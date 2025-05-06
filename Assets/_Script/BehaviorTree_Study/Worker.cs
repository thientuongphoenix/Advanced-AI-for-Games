using UnityEngine;

public class Worker : BTAgent
{
    public GameObject office;
    GameObject patron;
    
    public override void Start()
    {
        base.Start();
        Leaf goToPatron = new Leaf("Go To Patron", GoToPatron);
        Leaf goToOffice = new Leaf("Go To Office", GoToOffice);

        Selector beWorker = new Selector("Be A Worker");
        beWorker.AddChild(goToPatron);
        beWorker.AddChild(goToOffice);

        tree.AddChild(beWorker);
    }

    public Node.Status GoToPatron()
    {
        if(Blackboard.Instance.patrons.Count == 0) return Node.Status.FAILURE;
        this.patron = Blackboard.Instance.patrons.Pop();
        Node.Status s = GotoLocation(patron.transform.position);
        if(s == Node.Status.SUCCESS)
        {
            patron.GetComponent<PatronBehavior>().ticket = true;
            patron = null;
        }
        return s;
    }

    public Node.Status GoToOffice()
    {
        Node.Status s = GotoLocation(office.transform.position);
        return s;
    }
}
