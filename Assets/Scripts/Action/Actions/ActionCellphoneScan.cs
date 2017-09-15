using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionCellphoneScan : ActionTrigger
	{
		private GameObject cellphone;

		public override void Setting (GameObject obj, string stateName, IActionCompleted monitor)
		{
			base.Setting (obj, stateName, monitor);
			cellphone = obj.transform.Find ("hip_ctrl/root/spline/right_chest/left_hand_Goal/phone").gameObject;
			cellphone.SetActive (true);
		}

		public override void Finish ()
		{
			cellphone.SetActive (false);
			base.Finish ();
		}
	}
}