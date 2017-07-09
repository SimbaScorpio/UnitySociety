using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionSetDestination : ActionAuto, ActionCompleted
{
	public GameObject obj;
	public Vector3 position;

	private ActionCompleted monitor = null;

	private Person person;

	private FacilityElevator facilityElevator;

	private bool ready = false;

	void Start()
	{
		this.ID = ActionID.SETDESTINATION;
	}


	public void setting(GameObject obj, Vector3 position, ActionCompleted callback)
	{
		this.obj = obj;
		this.position = position;
		this.monitor = callback;

		person = obj.GetComponent<Person>();

		this.Begin(position);
	}


	void Begin(Vector3 position)
	{
		facilityElevator = FacilityManager.GetInstance().GetElevator();

		if (obj.transform.position.y < 5 && position.y < 5 || obj.transform.position.y > 5 && position.y > 5)
		{
			ready = true;
			ActionManager.GetInstance().ApplyMoveToAction(obj, position, ActionID.MOVETO, this);
		}
		else if (obj.transform.position.y < 5 && position.y > 5)
		{
			ready = false;
			person.FacilityInfo.Int[0] = 1;
			person.FacilityInfo.Int[1] = 2;
			person.FacilityInfo.FacilityScript = facilityElevator;
			person.FacilityInfo.Status = Status.ENTERINGAREA;
			Vector3 doorMark = GameObject.Find("DoorMark1").transform.position;
			ActionManager.GetInstance().ApplyMoveToAction(obj, doorMark, ActionID.MOVETO, this);
		}
		else if (obj.transform.position.y > 5 && position.y < 5)
		{
			ready = false;
			person.FacilityInfo.Int[0] = 2;
			person.FacilityInfo.Int[1] = 1;
			person.FacilityInfo.FacilityScript = facilityElevator;
			person.FacilityInfo.Status = Status.ENTERINGAREA;
			Vector3 doorMark = GameObject.Find("DoorMark2").transform.position;
			ActionManager.GetInstance().ApplyMoveToAction(obj, doorMark, ActionID.MOVETO, this);
		}
	}


	void Finish()
	{
		if (monitor != null)
		{
			monitor.OnActionCompleted(this);
		}
		Destroy(this);
	}


	public void OnActionCompleted(Action ac)
	{
		switch (ac.ID)
		{
			case ActionID.MOVETO:
				if (!ready)
					ActionManager.GetInstance().ApplyElevatorAction(obj, facilityElevator, person.FacilityInfo.Int[0], person.FacilityInfo.Int[1], this);
				else
					Finish();
				break;
			case ActionID.TAKEELEVATOR:
				ready = true;
				ActionManager.GetInstance().ApplyMoveToAction(obj, position, ActionID.MOVETO, this);
				break;
		}
	}
}
