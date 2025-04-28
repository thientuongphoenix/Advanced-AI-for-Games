using UnityEngine;
using UnityEngine.AI;

public class RobberBehavior : MonoBehaviour
{
    BehaviorTree tree;
    public GameObject diamond;
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;
    NavMeshAgent agent;

    public enum ActionState { IDLE, WORKING};
    ActionState state = ActionState.IDLE;

    Node.Status treeStatus = Node.Status.RUNNING;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.agent = GetComponent<NavMeshAgent>();

        this.tree = new BehaviorTree(); // root Node
        Sequence steal = new Sequence("Steal Something");  
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Selector opendoor = new Selector("Open Door");

        opendoor.AddChild(goToFrontDoor);
        opendoor.AddChild(goToBackDoor);

        steal.AddChild(opendoor); // Chọn cửa để vào
        steal.AddChild(goToDiamond);
        //steal.AddChild(goToBackDoor);
        steal.AddChild(goToVan);
        this.tree.AddChild(steal);
        //root Node (tree) -> steal something -> ( 1. Go to diamond; 2. Go to van)

        tree.PrintTree();
    }

    public Node.Status GoToDiamond()
    {
        Node.Status s = GotoLocation(this.diamond.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (diamond != null)
            {
                diamond.transform.parent = this.gameObject.transform; // Lôi viên kim cương theo luôn
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else return s;
    }

    public Node.Status GoToBackDoor()
    {
        return this.GoToDoor(backdoor);
        //this.agent.SetDestination(this.diamond.transform.position);
        //return Node.Status.SUCCESS;
    }
    public Node.Status GoToFrontDoor()
    {
        return this.GoToDoor(frontdoor);
        //this.agent.SetDestination(this.diamond.transform.position);
        //return Node.Status.SUCCESS;
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status s = GotoLocation(door.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked)
            {
                door.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else return s; //Thông thường mà chưa tới nơi, nó sẽ trả ra RUNNING
    }

    public Node.Status GoToVan()
    {
        return this.GotoLocation(this.van.transform.position);
        //this.agent.SetDestination(this.van.transform.position);
        //return Node.Status.SUCCESS;
    }

    /// <summary>
    /// Điều khiển NavMeshAgent di chuyển đến vị trí chỉ định.
    /// Trả về trạng thái:
    /// - SUCCESS: Khi đã tới gần đích.
    /// - FAILURE: Khi điểm đích không còn hợp lệ.
    /// - RUNNING: Khi đang di chuyển.
    /// </summary>
    /// <param name="destination">Vị trí mục tiêu cần đến.</param>
    /// <returns>Trạng thái hành động (SUCCESS, FAILURE, RUNNING).</returns>
    Node.Status GotoLocation(Vector3 destination)
    {
        // Tính khoảng cách hiện tại tới điểm đến
        float distanceToTarget = Vector3.Distance(destination, this.transform.position);

        // Nếu đang ở trạng thái chờ (IDLE), bắt đầu di chuyển
        if (this.state == ActionState.IDLE)
        {
            this.agent.SetDestination(destination); // Đặt điểm đến cho NavMeshAgent
            this.state = ActionState.WORKING;        // Chuyển trạng thái sang đang làm việc
        }
        // Nếu điểm đến đã thay đổi quá xa so với kế hoạch ban đầu
        else if (Vector3.Distance(this.agent.pathEndPosition, destination) >= 2)
        {
            this.state = ActionState.IDLE;           // Reset trạng thái
            return Node.Status.FAILURE;              // Thất bại
        }
        // Nếu đã gần đến điểm đích
        else if (distanceToTarget < 2)
        {
            this.state = ActionState.IDLE;           // Reset trạng thái
            return Node.Status.SUCCESS;              // Thành công
        }

        // Nếu chưa tới đích và không thất bại → tiếp tục di chuyển
        return Node.Status.RUNNING;
    }

    // Update is called once per frame
    void Update()
    {
        if(treeStatus == Node.Status.RUNNING) treeStatus = tree.Process();
    }
}
