using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSitBack : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;
	private bool inMyState = false;
	private string stateName = "坐向后仰";

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger (stateName);
	}

	void Update ()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName (stateName)) {
			inMyState = true;
		} else if (inMyState) {
			Finish ();
		}
	}

	public void Finish ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
