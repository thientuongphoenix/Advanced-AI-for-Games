using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class Selector : Node
{
    public Selector() : base()
    {

    }

    public Selector(List<Node> children) : base(children)
    {

    }
    public override NodeState Evaluate()
    {
        foreach (var node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.FAILURE: continue;
                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }
}
