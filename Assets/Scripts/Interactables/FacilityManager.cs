using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityManager : Object
{
	private static FacilityManager instance;

	public static FacilityManager GetInstance ()
	{
		if (instance == null) {
			instance = new FacilityManager ();
		}
		return instance;
	}


	public FacilityElevator GetElevator ()
	{
		return GameObject.Find ("Elevator").GetComponent<FacilityElevator> ();
	}
}


public class Facility: MonoBehaviour
{
	public FacilityID ID;

	public virtual void OnChildTriggerEnter (Collider collider)
	{
	}

	public virtual void OnChildTriggerExit (Collider collider)
	{
	}
}


public class FacilityInfo : Object
{
	public Facility FacilityScript;
	public Status Status;
	public int[] Int = new int[10];
	public Vector3[] Vec = new Vector3[10];
	public GameObject[] Obj = new GameObject[10];
}


public enum FacilityID
{
	ELEVATOR
}


public enum Status
{
	NULL,
	ENTERINGAREA,
	ENTEREDAREA,
	QUEUING,
	LEAVINGAREA,
	USING
}
