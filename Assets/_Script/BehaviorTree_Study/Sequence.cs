using UnityEngine;

public class Sequence : Node
{
    public Sequence(string n)
    {
        name = n;
    }

    public override Status Process()
    {
        Status childstatus = children[currentChild].Process();
        if(childstatus == Status.RUNNING) return Status.RUNNING;
        if(childstatus == Status.FAILURE) return childstatus; //Đệ quy trở lại Status hiện tại, check tiếp

        currentChild++;
        if(currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.SUCCESS; //Nếu không có lá nào trả về FAILURE thì đồng nghĩa Node Sequence là SUCCESS
        }

        return Status.RUNNING;
    }
}
