using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGetPaper : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;
	private bool inMyState = false;
	private string stateName = "接纸";
	private GameObject questionnaire;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger (stateName);
		questionnaire = obj.transform.Find ("questionnaire").gameObject;
		questionnaire.SetActive (true);
	}

	void Update ()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName (stateName)) {
			inMyState = true;
		} else if (inMyState) {
			Finish ();
		}
	}

	public void Finish ()
	{
		questionnaire.SetActive (false);
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
