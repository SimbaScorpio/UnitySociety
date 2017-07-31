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
		this.id = ActionID.STANDUP;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("StandUp");
	}

	public void OnStandUpFinished ()
	{
		StartCoroutine (wait ());
	}

	IEnumerator wait ()
	{
		yield return new WaitForEndOfFrame ();
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
