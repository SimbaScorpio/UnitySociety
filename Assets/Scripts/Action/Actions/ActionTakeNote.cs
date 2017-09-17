using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionTakeNote : ActionTrigger
	{
		private GameObject pen;
		private GameObject book;

		public override void Setting (GameObject obj, string stateName, IActionCompleted monitor)
		{
			base.Setting (obj, stateName, monitor);
			pen = obj.transform.Find ("hip_ctrl/root/spline/right_chest/left_hand_Goal/pen").gameObject;
			pen.SetActive (true);
			book = obj.transform.Find ("hip_ctrl/root/spline/right_chest/right_hand_Goal/book").gameObject;
			book.SetActive (true);
		}

		public override void Finish ()
		{
			pen.SetActive (false);
			book.SetActive (false);
			base.Finish ();
		}
	}
}