using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FacilityElevator : Facility
{
	// 电梯移动需要的时间
	public float TimeReach = 1.0f;

	// 电梯动画管理器
	private Animator anim;

	// 等待人员
	struct WaitingPlayer
	{
		public GameObject obj;
		public int waitingFloor;
		public int targetFloor;

		public WaitingPlayer (GameObject obj, int waitingFloor, int targetFloor)
		{
			this.obj = obj;
			this.waitingFloor = waitingFloor;
			this.targetFloor = targetFloor;
		}
	};

	// 电梯内乘员
	private List<WaitingPlayer> insiders;
	// 等待电梯人员
	private List<WaitingPlayer> waitingList;
	// 出入电梯人员
	private List<WaitingPlayer> crossList;

	// 电梯各楼层内部的中心锚点
	private Transform[] floorMarks;
	// 电梯各楼层门口的中心锚点
	private Transform[] doorMarks;

	private int currentFloor = 1;
	private float counting = 0;

	private bool hasTrigger = true;
	private bool isAllClosed = true;
	private bool isGoingOut = false;


	void Awake ()
	{
		anim = this.GetComponent<Animator> ();
		insiders = new List<WaitingPlayer> ();
		waitingList = new List<WaitingPlayer> ();
		crossList = new List<WaitingPlayer> ();

		// 获得各楼层的电梯中心(注意序号从1开始)
		Transform floorMarkP = this.transform.Find ("FloorMarks");
		floorMarks = floorMarkP.GetComponentsInChildren<Transform> ();
		// 获得各楼层的门口中心(注意序号从1开始)
		Transform doorMarkP = this.transform.Find ("DoorMarks");
		doorMarks = doorMarkP.GetComponentsInChildren<Transform> ();
	}


	void Start ()
	{
		this.ID = FacilityID.ELEVATOR;
	}


	void Update ()
	{
		counting -= Time.deltaTime;
		if (counting <= 0) {	// 电梯有目标
			if (!hasTrigger) {
				anim.SetTrigger ("Floor" + currentFloor + "Open");
				isAllClosed = false;
				hasTrigger = true;
			}
			// 电梯待客
			else if (insiders.Count == 0 && isAllClosed) {
				this.ReadyToPickUp ();
			}
		}
		this.TurnToTheElevator ();
	}


	// 随时准备接客
	void ReadyToPickUp ()
	{
		// 监视等待队列，有人则接FIFO
		if (waitingList.Count > 0) {
			currentFloor = waitingList [0].waitingFloor;
			counting = TimeReach;
			hasTrigger = false;
		}
	}


	// 转向电梯
	void TurnToTheElevator ()
	{
		foreach (WaitingPlayer player in waitingList) {
			ActionMoveTo moveto = player.obj.GetComponent<ActionMoveTo> ();
			ActionLookAt lookat = player.obj.GetComponent<ActionLookAt> ();
			if (!moveto && !lookat) {
				Vector3 lookAtTargetPosition = (floorMarks [player.waitingFloor].position + doorMarks [player.waitingFloor].position) / 2;
				float angle = player.obj.transform.rotation.eulerAngles.y;
				float angle2 = angle / 180 * Mathf.PI;
				Vector3 curDir = new Vector3 (Mathf.Sin (angle2), 0, Mathf.Cos (angle2)).normalized;
				Vector3 futDir = lookAtTargetPosition - player.obj.transform.position;
				curDir.y = futDir.y = 0;
				if (Vector3.Angle (curDir, futDir) > 1f)
					ActionManager.GetInstance ().ApplyLookAtAction (player.obj, lookAtTargetPosition, null);
			}
		}
	}


	// 加入等待队列（会被监听到）
	public void AddWaiting (GameObject obj, int waitingFloor, int targetFloor)
	{
		foreach (WaitingPlayer p in waitingList) {
			if (p.obj == obj)
				return;
		}
		waitingList.Add (new WaitingPlayer (obj, waitingFloor, targetFloor));
		//print("[" + obj.name + "] added to waiting list.");
	}


	// 电梯门打开后的回调函数
	public void AnimFloorOpened (int floor)
	{
		// 重置出入计数器
		crossList.Clear ();

		// 电梯空，接一班人
		if (insiders.Count == 0) {	// 找出准备上电梯的人
			isGoingOut = false;
			for (int i = 0; i < waitingList.Count; ++i) {
				WaitingPlayer player = waitingList [i];
				if (player.waitingFloor == currentFloor && crossList.Count < 4) {
					crossList.Add (player);
					print ("[" + player.obj.name + "] added to cross list.");
				}
			}
			// 让他们进门，并且不再是等待状态
			for (int i = 0; i < crossList.Count; ++i) {
				this.EnterDoor (crossList [i], currentFloor);
				waitingList.Remove (crossList [i]);
				//print("[" + crossList[i].obj.name + "] remove from waiting list.");
			}
		} else {	// 电梯内有人要出去
			isGoingOut = true;
			for (int i = 0; i < insiders.Count; ++i) {
				WaitingPlayer player = insiders [i];
				if (player.targetFloor == currentFloor) {
					crossList.Add (player);
					//print("[" + player.obj.name + "] added to cross list.");
				}
			}
			for (int i = 0; i < crossList.Count; ++i) {
				this.LeaveDoor (crossList [i], currentFloor);
				//print("[" + crossList[i].obj.name + "] leave from elevator.");
			}
		}
	}


	// 电梯门关闭后的回调函数
	public void AnimFloorClosed (int floor)
	{
		// 电梯内有乘客就运行
		if (insiders.Count != 0) {
			// 选择一个目标楼层
			int targetFloor = insiders [0].targetFloor;
			// 转移乘客至目标楼层
			Vector3 target = floorMarks [targetFloor].position;
			Vector3 current = floorMarks [currentFloor].position;
			float dy = target.y - current.y;
			for (int i = 0; i < insiders.Count; ++i) {
				WaitingPlayer player = insiders [i];
				Vector3 pos = player.obj.transform.position + new Vector3 (0, dy, 0);
				NavMeshAgent agent = player.obj.GetComponent<NavMeshAgent> ();

				agent.enabled = false;
				player.obj.transform.LookAt (doorMarks [currentFloor].position);
				player.obj.transform.position = pos;
				agent.enabled = true;
			}
			// 准备开门
			currentFloor = targetFloor;
			counting = TimeReach;
			hasTrigger = false;
		}
		isAllClosed = true;
	}


	// 单位跨越电梯门的回调函数
	void OnTriggerExit (Collider collider)
	{
		if (collider.tag == "Player") {
			for (int i = 0; i < crossList.Count; ++i) {
				if (crossList [i].obj == collider.gameObject) {
					if (isGoingOut) {
						crossList [i].obj.GetComponent<ActionTakeElevator> ().Finish ();
					}
					crossList.RemoveAt (i--);
					if (crossList.Count == 0) {
						anim.SetTrigger ("Floor" + currentFloor + "Close");
					}
				}
			}
		}
	}


	public override void OnChildTriggerEnter (Collider collider)
	{
		if (collider.tag == "Player") {
			Person person = collider.GetComponent<Person> ();
			if (person.FacilityInfo.FacilityScript == this) {
				if (person.FacilityInfo.Status == Status.ENTERINGAREA)
					person.FacilityInfo.Status = Status.ENTEREDAREA;
			}
		}
	}


	public override void OnChildTriggerExit (Collider collider)
	{
		//if (collider.tag == "Player") {
		//Person person = collider.GetComponent<Person> ();
		//if (person.FacilityInfo.Status == Status.LEAVINGAREA)
		//person.FacilityInfo.FacilityScript = null;
		//}
	}

	 
	// 让单位进入电梯内部
	void EnterDoor (WaitingPlayer player, int floor)
	{
		insiders.Add (player);
		Person person = player.obj.GetComponent<Person> ();
		person.FacilityInfo.Status = Status.USING;
		ActionManager.GetInstance ().ApplyMoveToAction (player.obj, floorMarks [floor].position, ActionID.MOVETO, null);
	}


	// 让单位离开电梯内部
	void LeaveDoor (WaitingPlayer player, int floor)
	{
		insiders.Remove (player);
		Person person = player.obj.GetComponent<Person> ();
		person.FacilityInfo.Status = Status.LEAVINGAREA;
		ActionManager.GetInstance ().ApplyMoveToAction (player.obj, doorMarks [floor].position, ActionID.MOVETO, null);
	}
}
