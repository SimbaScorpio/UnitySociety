using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSitDown : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private ActionCompleted monitor;

	public void Setting (GameObject obj, ActionCompleted monitor)
	{
		this.ID = ActionID.SITDOWN;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("SitDown");
	}

	public void OnSitDownFinished ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
