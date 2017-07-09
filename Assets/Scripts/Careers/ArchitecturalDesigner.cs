using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchitecturalDesigner : Person
{
	void Start ()
	{
		this.C_ID = CareerID.ARCHITECT;

		RandomDestination ();
	}


	private void RandomDestination ()
	{
		Vector3 pos = NavMeshManager.GetInstance ().GetRandomPoint ();
		this.SetDestination (pos);
	}


	public override void OnActionCompleted (Action ac)
	{
		switch (ac.ID) {
		case ActionID.SETDESTINATION:
			this.GetComponent<Animator> ().SetTrigger ("PlayVR");
			FacilityInfo.Status = Status.USING;
			break;
		}
	}


	public void AnimPlayVRFinished ()
	{
		this.Chat ("VR played!", 2);
		FacilityInfo.Status = Status.NULL;
		RandomDestination ();
	}


	public Facility script;
	public Status status;

	void FixedUpdate ()
	{
		script = FacilityInfo.FacilityScript;
		status = FacilityInfo.Status;
	}
}
