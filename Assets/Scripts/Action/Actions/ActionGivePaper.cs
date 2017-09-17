using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionGivePaper : ActionTrigger
	{
		private GameObject pen;
		private GameObject questionnaire;

		public override void Setting (GameObject obj, string stateName, IActionCompleted monitor)
		{
			base.Setting (obj, stateName, monitor);
			pen = obj.transform.Find ("pen").gameObject;
			pen.SetActive (true);
			questionnaire = obj.transform.Find ("questionnaire").gameObject;
			questionnaire.SetActive (true);
		}

		public override void Finish ()
		{
			pen.SetActive (false);
			questionnaire.SetActive (false);
			base.Finish ();
		}
	}
}