using UnityEngine;
using UnityEngine.AI;

public class RobberBehavior : BTAgent
{
    //BehaviorTree tree; //Cái này có rồi, nhờ kế thừa BTAgent
    public GameObject diamond;
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;
    public GameObject painting;

    GameObject pickup;
    //NavMeshAgent agent;

    //public enum ActionState { IDLE, WORKING};
    //ActionState state = ActionState.IDLE;

    //Node.Status treeStatus = Node.Status.RUNNING;

    [Range(0, 1000)]
    public int money = 800;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start() //Thay vì override thì không, ở đây sử dụng new để hiding (che khuất hàm cùng tên trong class cha)
    {
        //this.agent = GetComponent<NavMeshAgent>();

        //this.tree = new BehaviorTree(); // root Node
        base.Start();
        Sequence steal = new Sequence("Steal Something");  
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond);
        Leaf goToPainting = new Leaf("Go To Diamond", GoToPainting);
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);
        Leaf goToBackDoor = new Leaf("Go To BackDoor", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To FrontDoor", GoToFrontDoor);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Selector opendoor = new Selector("Open Door");
        Selector selectObject = new Selector("Select Object To Steal");

        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney);

        opendoor.AddChild(goToFrontDoor);
        opendoor.AddChild(goToBackDoor);
        steal.AddChild(invertMoney);
        steal.AddChild(opendoor); // Chọn cửa để vào

        selectObject.AddChild(goToDiamond);
        selectObject.AddChild(goToPainting);
        steal.AddChild(selectObject); //Chọn vật để trộm

        //steal.AddChild(goToBackDoor);
        steal.AddChild(goToVan); // trở về xe
        this.tree.AddChild(steal); // Thêm tất cả vào node gốc
        //root Node (tree) -> steal something -> ( 1. Go to diamond; 2. Go to van)

        this.tree.PrintTree();
    }

    public Node.Status HasMoney()
    {
        if (this.money < 500) return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToDiamond()
    {
        if(!this.diamond.activeSelf) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(this.diamond.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            //if (this.diamond != null)
            //{
            //    this.diamond.transform.parent = this.gameObject.transform; // Lôi viên kim cương theo luôn
            //    return Node.Status.SUCCESS;
            //}
            //return Node.Status.FAILURE;

            this.diamond.transform.parent = this.gameObject.transform;
            this.pickup = this.diamond;
        }
        //else return s;
        return s;
    }

    public Node.Status GoToPainting()
    {
        if (!this.painting.activeSelf) return Node.Status.FAILURE;
        Node.Status s = GotoLocation(this.painting.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            this.painting.transform.parent = this.gameObject.transform;
            this.pickup = this.painting;
        }
        return s;
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
                //door.SetActive(false);
                door.GetComponent<NavMeshObstacle>().enabled = false;
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else return s; //Thông thường mà chưa tới nơi, nó sẽ trả ra RUNNING
    }

    public Node.Status GoToVan()
    {
        //return this.GotoLocation(this.van.transform.position);
        //this.agent.SetDestination(this.van.transform.position);
        //return Node.Status.SUCCESS;
        Node.Status s = GotoLocation(this.van.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            money += 300;
            pickup.SetActive(false);
        }
        return s;
    }


    //Đoạn code bên dưới đã chuyển qua cho Class cha giữ rồi nhá
    ///// <summary>
    ///// Điều khiển NavMeshAgent di chuyển đến vị trí chỉ định.
    ///// Trả về trạng thái:
    ///// - SUCCESS: Khi đã tới gần đích.
    ///// - FAILURE: Khi điểm đích không còn hợp lệ.
    ///// - RUNNING: Khi đang di chuyển.
    ///// </summary>
    ///// <param name="destination">Vị trí mục tiêu cần đến.</param>
    ///// <returns>Trạng thái hành động (SUCCESS, FAILURE, RUNNING).</returns>
    //new Node.Status GotoLocation(Vector3 destination)
    //{
    //    // Tính khoảng cách hiện tại tới điểm đến
    //    float distanceToTarget = Vector3.Distance(destination, this.transform.position);

    //    // Nếu đang ở trạng thái chờ (IDLE), bắt đầu di chuyển
    //    if (this.state == ActionState.IDLE)
    //    {
    //        this.agent.SetDestination(destination); // Đặt điểm đến cho NavMeshAgent
    //        this.state = ActionState.WORKING;        // Chuyển trạng thái sang đang làm việc
    //    }
    //    // Nếu điểm đến đã thay đổi quá xa so với kế hoạch ban đầu
    //    else if (Vector3.Distance(this.agent.pathEndPosition, destination) >= 2)
    //    {
    //        this.state = ActionState.IDLE;           // Reset trạng thái
    //        return Node.Status.FAILURE;              // Thất bại
    //    }
    //    // Nếu đã gần đến điểm đích
    //    else if (distanceToTarget < 2)
    //    {
    //        this.state = ActionState.IDLE;           // Reset trạng thái
    //        return Node.Status.SUCCESS;              // Thành công
    //    }

    //    // Nếu chưa tới đích và không thất bại → tiếp tục di chuyển
    //    return Node.Status.RUNNING;
    //}
}
