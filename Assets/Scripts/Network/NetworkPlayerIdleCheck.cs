using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkPlayerIdleCheck : NetworkBehaviour
	{
		public float maxTimeToIdle = 10f;
		public GameObject idleMark;
		public float idleCount = 0f;
		public bool isIdleForALongTime = true;

		void Update ()
		{
			if (!isLocalPlayer)
				return;
			ActionSingle ac = GetComponent<ActionSingle> ();
			if (ac == null) {
				idleCount += Time.deltaTime;
				if (idleCount > maxTimeToIdle)
					Idle ();
			} else {
				idleCount = 0;
				NotIdle ();
			}
		}

		void Idle ()
		{
			isIdleForALongTime = true;
			if (idleMark != null) {
				idleMark.SetActive (true);
			}
		}

		void NotIdle ()
		{
			isIdleForALongTime = false;
			if (idleMark != null) {
				idleMark.SetActive (false);
			}
		}
	}
}