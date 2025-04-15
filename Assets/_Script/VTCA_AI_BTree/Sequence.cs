using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence() : base()
    {

    }

    public Sequence(List<Node> children) : base(children)
    {

    }
    public override NodeState Evaluate()
    {
        bool isAnyChildRunning = false;
        foreach (var node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.FAILURE: // neu node con tra ve trang thai that bai
                    state = NodeState.FAILURE; // gan trang thai cua node la that bai
                    return state;
                case NodeState.SUCCESS: // neu node con tra ve trang thai thanh cong
                    continue;
                case NodeState.RUNNING: // neu node con dang chay
                    isAnyChildRunning = true;
                    break;
            }
        }
        state = isAnyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
}
