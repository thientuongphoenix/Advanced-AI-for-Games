using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PatrolBehavior : BTAgent
{
    public GameObject[] art;
    public GameObject frontdoor;
    public GameObject home;

    [Range(0, 1000)]
    public int boredom = 0;

    public override void Start()
    {
        base.Start();
        RSelector selectObject = new RSelector("Select Art To View");
        for(int i = 0; i < art.Length; i++)
        {
            Leaf gta = new Leaf("Go To " + art[i].name, i, GoToArt);
            selectObject.AddChild(gta);
        }

        Leaf goToFrontDoor = new Leaf("Go To Frontdoor", GoToFrontDoor);
        Leaf goToHome = new Leaf("Go To Home", GoToHome);
        Leaf isBored = new Leaf("Is Bored?", IsBored);

        Sequence viewArt = new Sequence("View Art");
        viewArt.AddChild(isBored);
        viewArt.AddChild(goToFrontDoor);

        BehaviorTree whileBored = new BehaviorTree();
        whileBored.AddChild(isBored);

        Loop lookAtPainting = new Loop("Look", whileBored);
        lookAtPainting.AddChild(selectObject);

        viewArt.AddChild(lookAtPainting);

        viewArt.AddChild(goToHome);

        Selector bePatrol = new Selector("Be A Patrol");
        bePatrol.AddChild(viewArt);

        tree.AddChild(bePatrol);

        StartCoroutine("IncreaseBoredom");
    }

    IEnumerator IncreaseBoredom()
    {
        while(true)
        {
            this.boredom = Mathf.Clamp(this.boredom + 20, 0, 1000);
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }

    public Node.Status GoToArt(int i)
    {
        if(!this.art[i].activeSelf) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(this.art[i].transform.position);
        if(s == Node.Status.SUCCESS)
        {
            this.boredom = Mathf.Clamp(this.boredom - 150, 0, 1000);
        }
        return s;
    }

    public Node.Status GoToFrontDoor()
    {
        Node.Status s = base.GoToDoor(frontdoor);
        
        return s;
    }

    public Node.Status GoToHome()
    {
        Node.Status s = GotoLocation(this.home.transform.position);
        
        return s;
    }

    public Node.Status IsBored()
    {
        if(this.boredom < 100)
        {
            return Node.Status.FAILURE;
        }
        else
        {
            return Node.Status.SUCCESS;
        }
    }

    public Node.Status IsOpen()
    {
        if(Blackboard.Instance.timeOfDay < 9 || Blackboard.Instance.timeOfDay > 17)
        {
            return Node.Status.FAILURE;
        }
        else
        {
            return Node.Status.SUCCESS;
        }
    }
}
