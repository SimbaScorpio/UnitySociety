using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSimpleClick : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private ActionCompleted monitor;

	private GameObject mouse;

	public void Setting (GameObject obj, ActionCompleted monitor)
	{
		this.ID = ActionID.SIMPLECLICK;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();

		mouse = obj.transform.Find ("mouse").gameObject;
		mouse.SetActive (true);
		animator.SetTrigger ("SimpleClick");
	}

	public void OnSimpleClickFinished ()
	{
		mouse.SetActive (false);
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
