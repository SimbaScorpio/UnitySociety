using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignSociety;

public class SyncActionCubeMove : ActionSingle
{
	IActionCompleted monitor;
	Animator anim;
	string stateName;

	public void Setting (GameObject obj, string name, IActionCompleted callback)
	{
		this.stateName = name;
		this.monitor = callback;
		anim = GetComponent<Animator> ();
		anim.Play (name, 0, 0f);
		obj.GetComponent<ActionSyncTest> ().CmdSyncAction (name);
	}

	void Update ()
	{
		if (!string.IsNullOrEmpty (stateName)) {
			if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1 || !anim.GetCurrentAnimatorStateInfo (0).IsName (stateName)) {
				Finish ();
			}
		}
	}

	public void Finish ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Destroy (this);
	}
}
