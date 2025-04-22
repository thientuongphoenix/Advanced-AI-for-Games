using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : Node
{
    public BehaviorTree()
    {
        // name này là name trong node
        // gọi qua đây để khai báo tên của cây, vd như NPC Behavior hoặc Boss chẳng hạn
        this.name = "Tree";
    }

    public BehaviorTree(string n)
    {
        // Lấy chính tên cây để đặt tên cho node đầu tiên, đây chính là root node
        this.name = n;
    }

    struct NodeLevel
    {
        public int level;
        public Node node;
    }

    public void PrintTree()
    {
        string treePrintout = "";
        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
        Node currentNode = this;
        nodeStack.Push(new NodeLevel { level = 0, node = currentNode });

        while (nodeStack.Count != 0)
        {
            NodeLevel nextNode = nodeStack.Pop();
            treePrintout += new string('-', nextNode.level) + nextNode.node.name + "\n";
            for(int i = nextNode.node.children.Count - 1; i >= 0; i--)
            {
                nodeStack.Push( new NodeLevel { level = nextNode.level + 1, node = nextNode.node.children[i] });
            }
        }

        Debug.Log(treePrintout);
        //--------------------------------------------------------------------------
        //Stack<(Node node, int depth)> stack = new Stack<(Node node, int depth)>();
        //stack.Push((this, 0));

        //while(stack.Count > 0)
        //{
        //    var (current, depth) = stack.Pop();
        //    string indent = new string(' ', depth * 2); //Thụt lề
        //    Debug.Log(indent + current.name);

        //    // Duyệt từ cuối để khi pop ra theo thứ tự ban đầu
        //    for (int i = current.children.Count - 1; i >= 0; i--)
        //    {
        //        stack.Push((current.children[i], depth + 1));
        //    }
        //}
    }
}
