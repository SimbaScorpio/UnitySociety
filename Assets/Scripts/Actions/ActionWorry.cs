using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionWorry : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private ActionCompleted monitor;

	public void Setting (GameObject obj, ActionCompleted monitor)
	{
		this.ID = ActionID.WORRY;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("Worry");
	}

	public void OnWorryFinished ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
