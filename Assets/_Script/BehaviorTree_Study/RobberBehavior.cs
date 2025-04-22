using UnityEngine;
using UnityEngine.AI;

public class RobberBehavior : MonoBehaviour
{
    BehaviorTree tree;
    public GameObject diamond;
    public GameObject van;
    NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();

        this.tree = new BehaviorTree(); // root Node
        Node steal = new Node("Steal Something");  
        Node goToDiamond = new Node("Go To Diamond");
        Node goToVan = new Node("Go To Van");

        steal.AddChild(goToDiamond);
        steal.AddChild(goToVan);
        this.tree.AddChild(steal);
        //root Node (tree) -> steal something -> ( 1. Go to diamond; 2. Go to van)

        tree.PrintTree();

        agent.SetDestination(diamond.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
