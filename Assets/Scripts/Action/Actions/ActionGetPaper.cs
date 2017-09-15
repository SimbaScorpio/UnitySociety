using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionGetPaper : ActionTrigger
	{
		private GameObject questionnaire;

		public override void Setting (GameObject obj, string stateName, IActionCompleted monitor)
		{
			base.Setting (obj, stateName, monitor);
			questionnaire = obj.transform.Find ("questionnaire").gameObject;
			questionnaire.SetActive (true);
		}

		public override void Finish ()
		{
			questionnaire.SetActive (false);
			base.Finish ();
		}
	}
}