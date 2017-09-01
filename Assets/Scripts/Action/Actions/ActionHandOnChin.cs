using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandOnChin : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;
	private bool inMyState = false;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		//this.id = ActionID.HANDONCHIN;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("托腮思考");
	}

	void Update ()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("托腮思考")) {
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
