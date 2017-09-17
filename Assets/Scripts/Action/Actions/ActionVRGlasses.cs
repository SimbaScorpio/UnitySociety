using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionVRGlasses : ActionTrigger
	{
		private GameObject VRGlasses;
		private GameObject VRHandle;

		public override void Setting (GameObject obj, string stateName, IActionCompleted monitor)
		{
			base.Setting (obj, stateName, monitor);
			VRGlasses = obj.transform.Find ("VR").gameObject;
			VRGlasses.SetActive (true);
			VRHandle = obj.transform.Find ("hip_ctrl/root/spline/right_chest/left_hand_Goal/handle").gameObject;
			VRHandle.SetActive (true);
		}

		public override void Finish ()
		{
			VRGlasses.SetActive (false);
			VRHandle.SetActive (false);
			base.Finish ();
		}
	}
}