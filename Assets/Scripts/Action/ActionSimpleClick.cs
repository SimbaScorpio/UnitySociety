using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSimpleClick : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;

	private GameObject mouse;

	public void Setting (GameObject obj, IActionCompleted monitor)
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
