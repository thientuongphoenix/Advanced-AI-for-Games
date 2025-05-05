using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    public Leaf() { }

    public Leaf(string n, Tick pm)
    {
        this.name = n;
        this.ProcessMethod = pm;
    }

    public Leaf(string n, Tick pm, int order)
    {
        this.name = n;
        this.ProcessMethod = pm;
        this.sortOrder = order;
    }

    public override Status Process()
    {
        if(ProcessMethod != null) return ProcessMethod();
        return Status.FAILURE;
    }
}
