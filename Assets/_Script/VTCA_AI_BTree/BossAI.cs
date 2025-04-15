using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BossAI : MonoBehaviour
{
    public Transform playerTransform;
    public NavMeshAgent agent;

    public float maxHealth = 100f;
    public float currentHealth;
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float detectionRange = 10f;
    public float fleeHealthThreshold = 30f;
    public float buffCooldown = 30f;
    public float lastBuffTime = 0f;
    public float moveSpeed = 2;

    public GameObject robotPrefab;
    public int maxRobotCount = 3;
    public List<GameObject> summonedRobots = new List<GameObject>();

    public Transform[] patrolPoints;
    public Transform currentPatrolTarget;

    private Node rootNode;

    enum BossAIState { Idle, Evaluating, RunningAction }

    private BossAIState currentState;
    Node currentRunningNode;


    void Start()
    {
        currentHealth = maxHealth;

        rootNode = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckFleeHealthNode(this, fleeHealthThreshold),
                new FleeNode(this)
            }),
            new Sequence(new List<Node>
            {
                new CheckSummonConditionNode(this),
                new SummonNode(this)
            }),
            new Sequence(new List<Node>
            {
                new CheckBuffConditionNode(this),
                new SelfBuffNode(this)
            }),
            new Sequence(new List<Node>
            {
                new CheckAttackConditionNode(this),
                new AttackNode(this)
            }),
            new Sequence(new List<Node>
            {
                new CheckPatrolConditionNode(this),
                new PatrolNode(this)
            }),
        });
    }

    void Update()
    {
        // LUÔN Evaluate cây hành vi mỗi frame để đảm bảo điều kiện tuần tra có thể được đánh giá lại
        // rootNode.Evaluate();

        switch (currentState)
        {
            case BossAIState.Idle:
                if (ShouldEvaluate()) // Có tình huống đột xuất
                {
                    NodeState result = rootNode.Evaluate();
                    if (result == NodeState.RUNNING)
                    {
                        currentRunningNode = FindRunningLeafNode(rootNode);
                        currentState = BossAIState.RunningAction;
                    }
                }
                break;

            case BossAIState.RunningAction:
                if (currentRunningNode != null)
                {
                    NodeState actionState = currentRunningNode.Evaluate();
                    if (actionState != NodeState.RUNNING)
                    {
                        currentRunningNode = null;
                        currentState = BossAIState.Idle;
                    }
                }
                break;
        }
    }

    public void SummonRobot()
    {
        Vector3 position = transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        GameObject robot = Instantiate(robotPrefab, position, Quaternion.identity);
        summonedRobots.Add(robot);
    }

    bool ShouldEvaluate()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        bool isPlayerClose = dist <= detectionRange || dist <= attackRange;
        bool isLowHealth = currentHealth <= fleeHealthThreshold;
        bool canBuff = Time.time - lastBuffTime >= buffCooldown;

        // Nếu không có hành động nào đang chạy → kiểm tra trigger để hành động tiếp theo
        return isPlayerClose || isLowHealth || canBuff
               || !agent.hasPath || agent.remainingDistance < 0.5f;
    }

    // tìm node đang running
    Node FindRunningLeafNode(Node node)
    {
        if (node.GetState() == NodeState.RUNNING)
        {
            return node;
        }

        foreach (var child in node.GetChildren())
        {
            Node runningNode = FindRunningLeafNode(child);
            if (runningNode != null)
            {
                return runningNode;
            }
        }

        return null;
    }
}


// Kiểm tra máu để quyết định có chạy trốn hay không
public class CheckFleeHealthNode : Node
{
    private BossAI bossAI;
    private float fleeThreshold;

    public CheckFleeHealthNode(BossAI bossAI, float threshold)
    {
        this.bossAI = bossAI;
        fleeThreshold = threshold;
    }

    public override NodeState Evaluate()
    {
        Debug.Log($"Check Flee Health Running: {bossAI.currentHealth} / {fleeThreshold}");
        if (bossAI.currentHealth <= fleeThreshold)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}

// Chạy trốn, càng xa player càng tốt, ngược hướng chuyển động của Player
public class FleeNode : Node
{
    private BossAI bossAI;

    public FleeNode(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Fleeing from Player!");
        // Tính toán hướng chạy trốn, dùng navmesh agent để di chuyển
        Vector3 fleeDirection = (bossAI.transform.position -
                                 bossAI.playerTransform.position).normalized;
        Vector3 fleePosition = bossAI.transform.position + fleeDirection * 5f; // chạy xa 5m
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, 5f, NavMesh.AllAreas))
        {
            bossAI.agent.SetDestination(hit.position); // di chuyển đến vị trí chạy trốn

            // success khi đến vị trí
            if (Vector3.Distance(bossAI.transform.position, hit.position) < 0.1f)
            {
                Debug.Log("Monster reached flee point!");
                state = NodeState.SUCCESS;
                return state;
            }
        }

        Debug.Log("Boss đang chạy trốn!");
        state = NodeState.RUNNING;
        return state;
    }
}

// Kiểm tra điều kiện triệu hồi đệ tử
public class CheckSummonConditionNode : Node
{
    private BossAI bossAI;

    public CheckSummonConditionNode(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Check Summon Condition Running");
        var distanceToPlayer = Vector3.Distance(
            bossAI.transform.position,
            bossAI.playerTransform.position
        );

        var canSummon = bossAI.summonedRobots.Count < bossAI.maxRobotCount;

        if (distanceToPlayer <= bossAI.detectionRange && canSummon)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}

// Triệu hồi đệ tử
public class SummonNode : Node
{
    private BossAI bossAI;

    public SummonNode(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Summon Node Running");
        if (bossAI.summonedRobots.Count >= bossAI.maxRobotCount)
        {
            state = NodeState.FAILURE;
            return state;
        }

        bossAI.SummonRobot(); // triệu hồi đệ tử
        Debug.Log("Monster summoned a minion!");
        state = NodeState.SUCCESS;
        return state;
    }
}

// Kiểm tra điều kiện buff
public class CheckBuffConditionNode : Node
{
    private BossAI bossAI;

    public CheckBuffConditionNode(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Check Buff Condition Running");
        bool canBuff = Time.time - bossAI.lastBuffTime >= bossAI.buffCooldown;

        if (canBuff)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}

// Buff cho Boss
public class SelfBuffNode : Node
{
    private BossAI bossAI;

    public SelfBuffNode(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Self Buff Node Running");
        bossAI.attackDamage *= 1.5f; // tăng sát thương 
        bossAI.moveSpeed *= 1.3f; // tăng tốc độ di chuyển
        bossAI.lastBuffTime = Time.time;

        Debug.Log("Monster buffed itself!");
        state = NodeState.SUCCESS;
        return state;
    }
}

// Kiểm tra điều kiện tấn công
public class CheckAttackConditionNode : Node
{
    private BossAI bossAI;

    public CheckAttackConditionNode(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Check Attack Condition Running");
        float distance = Vector3.Distance(
            bossAI.transform.position,
            bossAI.playerTransform.position
        );

        if (distance <= bossAI.attackRange)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}

// Tấn công Player
public class AttackNode : Node
{
    private BossAI bossAI;

    public AttackNode(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Attack Node Running");
        float distance = Vector3.Distance(
            bossAI.transform.position,
            bossAI.playerTransform.position
        );

        if (distance <= bossAI.attackRange)
        {
            // Thực hiện tấn công
            Debug.Log($"Monster attacks for {bossAI.attackDamage} damage!");
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}

// ✅ CheckPatrolConditionNode: chỉ tuần nếu không có player gần
public class CheckPatrolConditionNode : Node
{
    private BossAI bossAI;

    public CheckPatrolConditionNode(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Check Patrol Condition Running");
        float distanceToPlayer = Vector3.Distance(
            bossAI.transform.position,
            bossAI.playerTransform.position
        );

        if (distanceToPlayer > bossAI.detectionRange &&
            bossAI.currentHealth > bossAI.fleeHealthThreshold)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}

// ✅ PatrolNode xử lý tuần tra liên tục mượt mà
public class PatrolNode : Node
{
    private BossAI bossAI;

    public PatrolNode(BossAI bossAI)
    {
        this.bossAI = bossAI;
    }

    public override NodeState Evaluate()
    {
        if (bossAI.currentPatrolTarget == null ||
            bossAI.agent.remainingDistance <= bossAI.agent.stoppingDistance + 0.1f)
        {
            Transform nextPoint;
            do
            {
                nextPoint = bossAI.patrolPoints[Random.Range(0, bossAI.patrolPoints.Length)];
            } while (nextPoint == bossAI.currentPatrolTarget && bossAI.patrolPoints.Length > 1);

            bossAI.currentPatrolTarget = nextPoint;
            bossAI.agent.SetDestination(nextPoint.position);
            Debug.Log($"Set new destination to {nextPoint.name}");
        }

        state = NodeState.RUNNING;
        Debug.Log("Patrol Node Running");
        return state;
    }



}