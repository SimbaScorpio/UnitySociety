using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionClick : ActionTrigger
{
	private GameObject mouse;

	public override void Setting (GameObject obj, string stateName, IActionCompleted monitor)
	{
		base.Setting (obj, stateName, monitor);
		mouse = obj.transform.Find ("mouse").gameObject;
		mouse.SetActive (true);
	}

	public override void Finish ()
	{
		mouse.SetActive (false);
		base.Finish ();
	}
}
