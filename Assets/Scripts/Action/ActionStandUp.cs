using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionStandUp : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.ID = ActionID.STANDUP;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("StandUp");
	}

	public void OnStandUpFinished ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
