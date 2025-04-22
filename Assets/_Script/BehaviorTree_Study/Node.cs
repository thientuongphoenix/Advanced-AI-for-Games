using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum Status { SUCCESS, RUNNING, FAILURE };
    public Status status;
    public List<Node> children = new List<Node>();
    public int currentChild = 0;
    public string name;

    public Node() { }

    /// <summary>
    /// Dùng để đặt tên cho Node mới
    /// </summary>
    /// <param name="n">Tên đặt cho Node</param>
    public Node(string n)
    {
        this.name = n;
    }

    public virtual Status Process()
    {
        return Status.SUCCESS;
    }

    /// <summary>
    /// Dùng để thêm Node con vào
    /// </summary>
    /// <param name="n">Node con muốn thêm vào</param>
    public void AddChild(Node n)
    {
        children.Add(n);
    }
}
