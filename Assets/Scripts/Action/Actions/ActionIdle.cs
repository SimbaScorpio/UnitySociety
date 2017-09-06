using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIdle : ActionSingle
{
	public GameObject obj;
	public string stateName;
	private IActionCompleted monitor;
	private float count;

	public void Setting (GameObject obj, string stateName, float count, IActionCompleted monitor)
	{
		this.obj = obj;
		this.stateName = stateName;
		this.count = count;
		this.monitor = monitor;
	}

	void Update ()
	{
		if (count > 0)
			count -= Time.deltaTime;
		else
			Finish ();
	}

	public void Finish ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
