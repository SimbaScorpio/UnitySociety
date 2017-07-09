using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTakeElevator : ActionAuto
{
	public GameObject obj;
	public FacilityElevator elevator;
	public int waitingFloor;
	public int targetFloor;


	private ActionCompleted monitor = null;

	void Start()
	{
		this.ID = ActionID.TAKEELEVATOR;
	}

	public void setting(GameObject obj, FacilityElevator elevator, int waitingFloor, int targetFloor, ActionCompleted callback)
	{
		this.obj = obj;
		this.elevator = elevator;
		this.waitingFloor = waitingFloor;
		this.targetFloor = targetFloor;
		this.monitor = callback;

		this.Begin();
	}


	void Begin()
	{
		Person person = obj.GetComponent<Person>();
		person.FacilityInfo.FacilityScript = elevator;
		person.FacilityInfo.Status = Status.QUEUING;

		elevator.AddWaiting(obj, waitingFloor, targetFloor);
	}


	public void Finish()
	{
		if (monitor != null)
		{
			monitor.OnActionCompleted(this);
		}
		Destroy(this);
	}
}
