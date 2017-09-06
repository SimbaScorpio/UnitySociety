using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTrigger : ActionSingle
{
	public GameObject obj;
	public string stateName;

	private Animator animator;
	private IActionCompleted monitor;

	private bool inMyState = false;

	public virtual void Setting (GameObject obj, string stateName, IActionCompleted monitor)
	{
		this.obj = obj;
		this.stateName = stateName;
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

	public virtual void Finish ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
