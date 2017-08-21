using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIdle : ActionSingle
{
	public GameObject obj;
	private IActionCompleted monitor;
	private float count = 0.5f;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.id = ActionID.IDLE;
		this.obj = obj;
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
