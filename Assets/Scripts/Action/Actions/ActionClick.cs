using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionClick : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;
	private bool inMyState = false;
	private GameObject mouse;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.id = ActionID.CLICK;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("点击鼠标");
		mouse = obj.transform.Find ("mouse").gameObject;
		mouse.SetActive (true);
	}

	void Update ()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("点击鼠标")) {
			inMyState = true;
		} else if (inMyState) {
			Finish ();
		}
	}

	public void Finish ()
	{
		mouse.SetActive (false);
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
