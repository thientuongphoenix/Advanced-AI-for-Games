using UnityEngine;
using UnityEngine.AI;

public class RobberBehavior : MonoBehaviour
{
    BehaviorTree tree;
    public GameObject diamond;
    public GameObject van;
    NavMeshAgent agent;

    public enum ActionState { IDLE, WORKING};
    ActionState state = ActionState.IDLE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();

        this.tree = new BehaviorTree(); // root Node
        Node steal = new Node("Steal Something");  
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);

        steal.AddChild(goToDiamond);
        steal.AddChild(goToVan);
        this.tree.AddChild(steal);
        //root Node (tree) -> steal something -> ( 1. Go to diamond; 2. Go to van)

        tree.PrintTree();

        tree.Process();
    }

    public Node.Status GoToDiamond()
    {
        return this.GotoLocation(this.diamond.transform.position);
        //this.agent.SetDestination(this.diamond.transform.position);
        //return Node.Status.SUCCESS;
    }
    
    public Node.Status GoToVan()
    {
        return this.GotoLocation(this.van.transform.position);
        //this.agent.SetDestination(this.van.transform.position);
        //return Node.Status.SUCCESS;
    }

    /// <summary>
    /// Điều khiển NavMeshAgent di chuyển đến vị trí chỉ định.
    /// Trả về trạng thái:
    /// - SUCCESS: Khi đã tới gần đích.
    /// - FAILURE: Khi điểm đích không còn hợp lệ.
    /// - RUNNING: Khi đang di chuyển.
    /// </summary>
    /// <param name="destination">Vị trí mục tiêu cần đến.</param>
    /// <returns>Trạng thái hành động (SUCCESS, FAILURE, RUNNING).</returns>
    Node.Status GotoLocation(Vector3 destination)
    {
        // Tính khoảng cách hiện tại tới điểm đến
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);

        // Nếu đang ở trạng thái chờ (IDLE), bắt đầu di chuyển
        if (this.state == ActionState.IDLE)
        {
            this.agent.SetDestination(destination); // Đặt điểm đến cho NavMeshAgent
            this.state = ActionState.WORKING;        // Chuyển trạng thái sang đang làm việc
        }
        // Nếu điểm đến đã thay đổi quá xa so với kế hoạch ban đầu
        else if (Vector3.Distance(this.agent.pathEndPosition, destination) >= 2)
        {
            this.state = ActionState.IDLE;           // Reset trạng thái
            return Node.Status.FAILURE;              // Thất bại
        }
        // Nếu đã gần đến điểm đích
        else if (distanceToTarget < 2)
        {
            this.state = ActionState.IDLE;           // Reset trạng thái
            return Node.Status.SUCCESS;              // Thành công
        }

        // Nếu chưa tới đích và không thất bại → tiếp tục di chuyển
        return Node.Status.RUNNING;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
