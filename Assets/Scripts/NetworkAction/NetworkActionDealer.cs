using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkActionDealer : NetworkBehaviour, IActionCompleted
	{
		Animator anim;
		NetworkAnimator netAnim;
		//private string lastStateName = "walk_blend_tree";


		#region 随机动作判断

		private float nextTimeToConsider = 1.0f;
		private Coroutine handler;
		private bool aidActive;

		public void StartCountingAid ()
		{
			if (handler != null)
				return;
			aidActive = false;
			handler = StartCoroutine (HandleAidActivity ());
		}

		public void StopCountingAid ()
		{
			if (handler != null) {
				StopCoroutine (handler);
				handler = null;
			}
			aidActive = false;
		}

		public bool IsAidActive ()
		{
			return aidActive;
		}

		IEnumerator HandleAidActivity ()
		{
			while (!aidActive) {
				float p = StorylineManager.GetInstance ().storyline.aid_possibility;
				int aidPossiblity = Random.Range (0, (int)(1 / p));
				if (aidPossiblity == 0) {
					aidActive = true;
					handler = null;
				} else
					yield return new WaitForSeconds (nextTimeToConsider);
			}
		}

		#endregion

		void Start ()
		{
			anim = GetComponent<Animator> ();
			netAnim = GetComponent<NetworkAnimator> ();
			if (isServer) {
				for (int i = 0; i < anim.parameterCount; ++i)
					netAnim.SetParameterAutoSend (i, true);
			}
		}



		public Action ApplyWalkAction (Landmark landmark, IActionCompleted callback)
		{
			return SyncActionWalk (landmark, callback);
		}

		public Action ApplyAction (string stateName, IActionCompleted callback)
		{
			return SyncAction (stateName, callback);
		}



		#region network

		[ClientRpc]
		void RpcSyncAction (string name)
		{
			if (!isLocalPlayer)
				anim.Play (name, 0, 0f);
		}

		#endregion



		public void OnActionCompleted (Action ac)
		{
			print ("Finish");
		}



		Action SyncActionWalk (Landmark landmark, IActionCompleted callback)
		{
			SyncActionWalk ac = gameObject.AddComponent<SyncActionWalk> ();
			ac.Setting (landmark, callback);
			RpcSyncAction ("walk_blend_tree");
			return ac;
		}

		Action SyncAction (string stateName, IActionCompleted callback)
		{
			NetworkActionPlay ac = gameObject.AddComponent<NetworkActionPlay> ();
			ac.Setting (stateName, callback);
			RpcSyncAction (stateName);
			return ac;
		}

	}
}