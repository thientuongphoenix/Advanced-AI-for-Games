using System.Collections.Generic;
using UnityEngine;

// Node status 
public enum NodeState
{
    SUCCESS, // trang thai thanh cong
    FAILURE, // trang thai that bai
    RUNNING // trang thai dang chay
}

public abstract class Node
{
    protected NodeState state; // trang thai cua node
    public Node parent; // node cha
    protected List<Node> children = new List<Node>(); // danh sach cac node con

    public Node()
    {

    }

    public Node(List<Node> children)
    {
        foreach (var child in children)
        {
            Attach(child);
        }
    }

    public void Attach(Node child)
    {
        child.parent = this; // gan node cha cho node con
        children.Add(child); // them node con vao danh sach
    }

    public abstract NodeState Evaluate(); // phuong thucc danh gia trang thai cua node

    public NodeState GetState() => state;
    public List<Node> GetChildren() => children;
}