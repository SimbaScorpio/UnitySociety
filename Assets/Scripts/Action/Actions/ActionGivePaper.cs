using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGivePaper : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;
	private bool inMyState = false;
	private string stateName = "传纸";
	private GameObject pen;
	private GameObject questionnaire;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger (stateName);
		pen = obj.transform.Find ("pen").gameObject;
		pen.SetActive (true);
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
		pen.SetActive (false);
		questionnaire.SetActive (false);
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
