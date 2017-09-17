using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionTakeOutStuff : ActionTrigger
	{
		public void OnStuffTakenOut ()
		{
			List<Stuff> stuffs = obj.GetComponent<ActionDealer> ().holdingStuffs;
			if (stuffs.Count > 0)
				stuffs [stuffs.Count - 1].gameObject.SetActive (true);
		}
	}
}