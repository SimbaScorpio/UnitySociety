using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScratchHead : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.ID = ActionID.SCRATCHHEAD;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("ScratchHead");
	}

	public void OnScratchHeadFinished ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
