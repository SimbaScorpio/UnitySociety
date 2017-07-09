using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour, ActionCompleted
{
	public CareerID C_ID;
	public Action Action;

	public FacilityInfo FacilityInfo;

	void Awake ()
	{
		FacilityInfo = new FacilityInfo ();
	}

	public void MoveTo (Vector3 position)
	{
		ActionManager.GetInstance ().ApplyMoveToAction (this.gameObject, position, ActionID.MOVETO, this);
	}

	public void SetDestination (Vector3 position)
	{
		ActionManager.GetInstance ().ApplySetDestinationAction (this.gameObject, position, this);
	}

	public void Chat (string content, float duration)
	{
		ActionManager.GetInstance ().ApplyChatAction (this.gameObject, content, duration, this);
	}

	public virtual void OnActionCompleted (Action ac)
	{
	}
}


public enum CareerID
{
	ARCHITECT
}