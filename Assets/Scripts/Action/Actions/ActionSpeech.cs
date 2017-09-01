using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpeech : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;
	private bool inMyState = false;
	private GameObject paper;

	public void Setting (GameObject obj, GameObject paper, IActionCompleted monitor)
	{
		//this.id = ActionID.SPEECH;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("发言");
		this.paper = paper;
		this.paper.SetActive (true);
	}

	void Update ()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("发言")) {
			inMyState = true;
		} else if (inMyState) {
			Finish ();
		}
	}

	public void Finish ()
	{
		paper.SetActive (false);
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
