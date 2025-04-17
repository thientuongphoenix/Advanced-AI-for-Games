using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : Node
{
    public BehaviorTree()
    {
        name = "Tree";
    }

    public BehaviorTree(string n)
    {
        name = n;
    }

    public void PrintTree()
    {
        Stack<(Node node, int depth)> stack = new Stack<(Node node, int depth)>();
        stack.Push((this, 0));

        while(stack.Count > 0)
        {
            var (current, depth) = stack.Pop();
            string indent = new string(' ', depth * 2); //Thụt lề
            Debug.Log(indent + current.name);

            // Duyệt từ cuối để khi pop ra theo thứ tự ban đầu
            for (int i = current.children.Count - 1; i >= 0; i--)
            {
                stack.Push((current.children[i], depth + 1));
            }
        }
    }
}
