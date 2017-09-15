using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionPutDownStuff : ActionTrigger
	{
		public void OnStuffPutDown ()
		{
			List<Stuff> stuffs = obj.GetComponent<ActionDealer> ().holdingStuffs;
			if (stuffs.Count > 0) {
				Stuff stuff = stuffs [stuffs.Count - 1];
				stuffs.RemoveAt (stuffs.Count - 1);
				stuff.SetParent (null);
			}
		}
	}
}