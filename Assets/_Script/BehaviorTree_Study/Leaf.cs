using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;

    public delegate Status TickM(int index); //M là multiple, dùng cho cái list tranh
    public TickM ProcessMethodM;

    public int index;

    public Leaf() { }

    public Leaf(string n, Tick pm)
    {
        this.name = n;
        this.ProcessMethod = pm;
    }

    public Leaf(string n, int i, TickM pm)
    {
        this.name = n;
        this.ProcessMethodM = pm;
        this.index = i;
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
        else if(ProcessMethodM != null) return ProcessMethodM(index);
        return Status.FAILURE;
    }
}
