using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionGiveStuffTo : ActionTrigger
	{
		public bool hasPartner;
		public bool stuffReleased = false;
		public Stuff stuff;
		public ActionGetStuffFrom ac;

		private float distance = 2f;

		public override void Setting (GameObject obj, string stateName, IActionCompleted monitor)
		{
			base.Setting (obj, stateName, monitor);
			List<Stuff> stuffs = obj.GetComponent<ActionDealer> ().holdingStuffs;
			if (stuffs.Count > 0) {
				stuff = stuffs [stuffs.Count - 1];
				StartCoroutine (SearchPartner ());
			}
		}

		IEnumerator SearchPartner ()
		{
			GameObject[] objs = GameObject.FindGameObjectsWithTag ("Player");
			while (!hasPartner) {
				foreach (GameObject player in objs) {
					if (Vector3.Distance (player.transform.position, obj.transform.position) < distance) {
						ac = player.GetComponent<ActionGetStuffFrom> ();
						if (ac != null) {
							Vector3 direction = player.transform.position - obj.transform.position;
							Vector3 forward = obj.transform.TransformDirection (obj.transform.forward);
							if (Vector3.Dot (direction, forward) > 0) {
								FoundProperPartner ();
								break;
							}
						}
					}
				}
				yield return null;
			}
		}

		void FoundProperPartner ()
		{
			if (stuffReleased && !stuff.isOwned) {
				stuff.isOwned = true;
				ac.bindedStuff = stuff;
				ac.bindedStuffReleased = true;
			} else if (!stuffReleased && stuff.isOwned) {
				ac.bindedStuff = stuff;
				ac.bindedStuffReleased = false;
			}
			hasPartner = true;
		}

		public void OnStuffGivenTo ()
		{
			if (stuff == null)
				return;
			List<Stuff> stuffs = obj.GetComponent<ActionDealer> ().holdingStuffs;
			stuffs.RemoveAt (stuffs.Count - 1);
			stuff.SetParent (null);
			stuffReleased = true;

			// function called after partner found
			if (hasPartner) {
				stuff.isOwned = true;
				if (ac != null) {
					ac.bindedStuff = stuff;
					ac.bindedStuffReleased = true;
				}
			}
		}

		public override void Finish ()
		{
			StopAllCoroutines ();
			base.Finish ();
		}
	}
}
