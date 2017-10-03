using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class ActionRPCManager : NetworkBehaviour
	{
		public Action ApplyWalkToAction (Landmark destination, IActionCompleted callback)
		{
			ActionWalkTo ac = GetComponent<ActionWalkTo> ();
			if (ac == null)
				ac = this.gameObject.AddComponent<ActionWalkTo> ();
			ac.Setting (this.gameObject, destination, callback);
			RpcApplyWalkToAction (destination);
			return ac;
		}

		[ClientRpc]
		void RpcApplyWalkToAction (Landmark destination)
		{
			if (!isServer) {
				print ("client walk!");
				ActionWalkTo ac = GetComponent<ActionWalkTo> ();
				if (ac == null)
					ac = this.gameObject.AddComponent<ActionWalkTo> ();
				ac.Setting (this.gameObject, destination, null);
			}
		}
	}
}