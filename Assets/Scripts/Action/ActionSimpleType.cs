using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSimpleType : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.ID = ActionID.SIMPLETYPE;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("SimpleType");
	}

	public void OnSimpleTypeFinished ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
