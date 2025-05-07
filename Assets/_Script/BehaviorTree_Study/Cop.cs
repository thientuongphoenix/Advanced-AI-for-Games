using UnityEngine;

public class Cop : BTAgent
{
    public GameObject[] patrolPoints;

    public override void Start()
    {
        base.Start();
        
        Sequence selectPatrolPoint = new Sequence("Select Patrol Point");
        for(int i = 0; i < patrolPoints.Length; i++)
        {
            Leaf pp = new Leaf("Go to " + patrolPoints[i].name, i, GoToPoint);
            selectPatrolPoint.AddChild(pp);
        }

        tree.AddChild(selectPatrolPoint);
    }

    public Node.Status GoToPoint(int i)
    {
        Node.Status s = GotoLocation(patrolPoints[i].transform.position);
        return s;
    }
}
