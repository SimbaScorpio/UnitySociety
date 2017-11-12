using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkPlayerAction : NetworkBehaviour
	{
		private NetworkActionDealer ad;

		private bool calledStop;

		void Start ()
		{
			ad = GetComponent<NetworkActionDealer> ();
		}

		public void WalkTo (Vector3 position)
		{
			if (!isLocalPlayer)
				return;
			Landmark lm = new Landmark ();
			lm.Set (position);
			ad.ApplyWalkAction (lm, false, null);
		}

		public void ApplyAction (string name, IActionCompleted callback)
		{
			if (!isLocalPlayer)
				return;
			ad.ApplyAction (name, null);
		}


		// 查看是否空闲，flag-true试图停止手头作业
		//		public bool IsSpare (bool flag)
		//		{
		//			ActionSingle ac = GetComponent<ActionSingle> ();
		//			if (ac == null) {
		//				calledStop = false;
		//				return true;
		//			}
		//			if (!calledStop && flag) {
		//				calledStop = true;
		//				ad.CallingStop ();
		//			}
		//			return false;
		//		}
	}
}