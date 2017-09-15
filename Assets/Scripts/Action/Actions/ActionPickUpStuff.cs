using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionPickUpStuff : ActionTrigger
	{
		private StuffType stuffType;
		private Stuff stuff;
		private float reasonableDistance = 5f;
		private string boneRoot = "hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/";

		public void SetStuffType (StuffType stuffType)
		{
			this.stuffType = stuffType;
		}

		/** Search the nearest reasonable stuff in front of the player */
		Stuff SearchForStuffType (StuffType stuffType)
		{
			Vector3 forwardDir = obj.transform.TransformDirection (obj.transform.forward);
			Vector3 objectDir = Vector3.zero;

			float distance = 0f;
			float nearestDistance = float.MaxValue;
			Stuff nearestStuff = null;
			Stuff tempStuff = null;

			GameObject[] objs = GameObject.FindGameObjectsWithTag (stuffType.ToString ());
			foreach (GameObject stuffobj in objs) {
				distance = Vector3.Distance (obj.transform.position, stuffobj.transform.position);
				if (distance <= reasonableDistance && distance < nearestDistance) {
					objectDir = obj.transform.TransformDirection (stuffobj.transform.position - obj.transform.position);
					if (Vector3.Dot (forwardDir, objectDir) > 0) {
						tempStuff = stuffobj.GetComponent<Stuff> ();
						if (tempStuff != null && !tempStuff.isOwned) {
							nearestStuff = tempStuff;
							nearestDistance = distance;
						}
					}
				}
			}
			return nearestStuff;
		}

		public void OnStuffPickedUp ()
		{
			stuff = SearchForStuffType (stuffType);
			if (stuff != null) {
				obj.GetComponent<ActionDealer> ().holdingStuffs.Add (stuff);
				stuff.SetParent (obj.transform.Find (boneRoot));
			}
		}
	}
}