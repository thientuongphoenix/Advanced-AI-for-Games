using UnityEngine;

public class RobberBehavior : MonoBehaviour
{
    BehaviorTree tree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.tree = new BehaviorTree(); // root Node
        Node steal = new Node("Steal Something");
        Node goToDiamond = new Node("Go To Diamond");
        Node goToVan = new Node("Go To Van");

        steal.AddChild(goToDiamond);
        steal.AddChild(goToVan);
        this.tree.AddChild(steal); //root Node -> steal something -> ( 1. Go to diamond; 2. Go to van)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
