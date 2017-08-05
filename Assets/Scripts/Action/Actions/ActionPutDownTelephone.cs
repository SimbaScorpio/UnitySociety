using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPutDownTelephone : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;
	private bool inMyState = false;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.id = ActionID.PUTDOWNTELEPHONE;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("放下电话");
	}

	void Update ()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("放下电话")) {
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
