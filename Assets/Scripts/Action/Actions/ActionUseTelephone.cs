﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionUseTelephone : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;
	private bool inMyState = false;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.id = ActionID.USETELEPHONE;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("使用电话");
	}

	void Update ()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("使用电话")) {
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
