using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpeech : ActionTrigger
{
	private GameObject paper;

	public override void Setting (GameObject obj, string stateName, IActionCompleted monitor)
	{
		base.Setting (obj, stateName, monitor);
		paper = obj.transform.Find ("Polygon").gameObject;
		paper.SetActive (true);
	}

	public override void Finish ()
	{
		paper.SetActive (false);
		base.Finish ();
	}
}
