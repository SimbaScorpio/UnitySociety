using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionGetStuffFrom : ActionTrigger
	{
		public Stuff bindedStuff;
		public bool bindedStuffReleased;
		private string boneRoot = "hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/";

		void GainStuff ()
		{
			bindedStuff.SetParent (obj.transform.Find (boneRoot));
			obj.GetComponent<ActionDealer> ().holdingStuffs.Add (bindedStuff);
		}

		public void OnStuffGotFrom ()
		{
			if (bindedStuff != null && bindedStuffReleased) {
				GainStuff ();
			} else {
				StartCoroutine (WaitForPartner ());
			}
		}

		IEnumerator WaitForPartner ()
		{
			while (bindedStuff == null || !bindedStuffReleased)
				yield return null;
			GainStuff ();
		}

		public override void Finish ()
		{
			StopAllCoroutines ();
			base.Finish ();
		}
	}
}