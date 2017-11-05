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
		private string lastStateName = "walk_blend_tree";
		private string newStateName = "";
		private int checkState = 0;
		private IActionCompleted newCallback;

		// 记录执行最近的动作
		[HideInInspector]
		public List<string> recentActions = new List<string> ();
		private int maxRecord = 5;

		// 如果接下来的动作和进行的动作具有相同的root名称，跳过开始和结束动作的过渡判断
		private string oldRoot = "";
		private string newRoot = "";

		private Landmark landmark;
		private bool callingStop;
		[HideInInspector]
		public GameObject createdItem;
		[HideInInspector]
		public List<GameObject> gainedItems = new List<GameObject> ();
		[HideInInspector]
		public List<Stuff> stuffs = new List<Stuff> ();

		#region 随机动作判断

		private float nextTimeToConsider = 0.1f;
		private float possibility = 0.01f;
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
				int aidPossiblity = Random.Range (0, (int)(1 / possibility));
				if (aidPossiblity == 0) {
					aidActive = true;
					handler = null;
				} else {
					yield return new WaitForSeconds (nextTimeToConsider);
				}
			}
		}

		#endregion

		void Start ()
		{
			anim = GetComponent<Animator> ();
			netAnim = GetComponent<NetworkAnimator> ();
			if (netAnim != null && (isServer || isLocalPlayer)) {
				for (int i = 0; i < anim.parameterCount; ++i)
					netAnim.SetParameterAutoSend (i, true);
			}
		}



		public void ApplyWalkAction (Landmark landmark, IActionCompleted callback)
		{
			this.landmark = landmark;
			this.callingStop = false;
			if (lastStateName == "walk_book_blend_tree" || lastStateName == "stand_book_hold" || lastStateName == "stand_book_pickup_table" || lastStateName == "stand_book_pickup_bag") {
				ApplyAction ("walk_book_blend_tree", callback);
			} else if (lastStateName == "walk_small_blend_tree" || lastStateName == "stand_small_hold" || lastStateName == "stand_small_pickup_table" || lastStateName == "stand_small_pickup_bag") {
				ApplyAction ("walk_small_blend_tree", callback);
			} else if (lastStateName == "walk_middle_blend_tree" || lastStateName == "stand_middle_hold" || lastStateName == "stand_middle_pickup_table" || lastStateName == "stand_middle_pickup_bag") {
				ApplyAction ("walk_middle_blend_tree", callback);
			} else {
				ApplyAction ("walk_blend_tree", callback);
			}
		}

		public void ApplyAction (string stateName, IActionCompleted callback)
		{
			if (string.IsNullOrEmpty (stateName))
				return;
			ActionType newType = ActionName.IsValid (stateName);
			if (newType == ActionType.error) {
				Log.error ("未知的动作【" + stateName + "】");
				// bubble
				SyncActionBubble ac = gameObject.GetComponent<SyncActionBubble> ();
				if (ac == null)
					ac = gameObject.AddComponent<SyncActionBubble> ();
				ac.Setting ("未知的动作【" + stateName + "】", 5f, null);
				ActionIdle idle = gameObject.AddComponent<ActionIdle> ();
				idle.Setting (gameObject, "", 5f, null);
				return;
			}
			newStateName = stateName;
			newCallback = callback;
		
			oldRoot = ActionName.FindBorder (lastStateName);
			newRoot = ActionName.FindBorder (newStateName);
			if (!string.IsNullOrEmpty (oldRoot) && !string.IsNullOrEmpty (newRoot) && oldRoot == newRoot) {
				ActualApply ();
			} else {
				CheckOldBorder ();
			}
		}

		void CheckOldBorder ()
		{
			checkState = 0;
			if (!string.IsNullOrEmpty (oldRoot)) {
				// need to find items here
				string[] paths = ActionName.FindItems (lastStateName);
				ActionInfo info = new ActionInfo (oldRoot + "_end", paths, null, StuffType.BigStuff);
				SyncAction (info, this);
			} else {
				CheckSitStand ();
			}
		}

		void CheckSitStand ()
		{
			checkState = 1;
			ActionType newType = ActionName.IsValid (newStateName);
			ActionType oldType = ActionName.IsValid (lastStateName);
			if (newType == ActionType.sit && oldType == ActionType.stand) {
				ActionInfo info = new ActionInfo ("sit_down", null, null, StuffType.BigStuff);
				SyncAction (info, this);
			} else if (newType == ActionType.stand && oldType == ActionType.sit) {
				ActionInfo info = new ActionInfo ("stand_up", null, null, StuffType.BigStuff);
				SyncAction (info, this);
			} else {
				CheckNewBorder ();
			}
		}

		void CheckNewBorder ()
		{
			checkState = 2;
			if (!string.IsNullOrEmpty (newRoot)) {
				// need to find items here
				string[] paths = ActionName.FindItems (newStateName);
				ActionInfo info = new ActionInfo (newRoot + "_begin", paths, null, StuffType.BigStuff);
				SyncAction (info, this);
			} else {
				ActualApply ();
			}
		}

		void ActualApply ()
		{
			lastStateName = newStateName;
			RecordRecentAction (lastStateName);
			if (landmark != null) {
				if (!callingStop) {
					SyncActionWalk (newStateName, landmark, newCallback);
					landmark = null;
				}
			} else {
				// need to find items here
				string[] paths = ActionName.FindItems (newStateName);
				string prefabName = ActionName.FindPrefabs (newStateName);
				StuffType stuffType = ActionName.FindStuffType (newStateName);
				ActionInfo info = new ActionInfo (newStateName, paths, prefabName, stuffType);
				SyncAction (info, newCallback);
			}
		}

		void RecordRecentAction (string stateName)
		{
			if (recentActions.Count < maxRecord) {
				recentActions.Add (stateName);
			}
			for (int i = recentActions.Count - 1; i > 0; --i) {
				recentActions [i] = recentActions [i - 1];
			}
			recentActions [0] = stateName;
		}

		public void OnActionCompleted (Action ac)
		{
			if (checkState == 0) {
				CheckSitStand ();
			} else if (checkState == 1) {
				CheckNewBorder ();
			} else if (checkState == 2) {
				ActualApply ();
			}
		}


		public void CallingStop ()
		{
			if (landmark != null) {
				callingStop = true;
			} else {
				SyncActionWalk ac = GetComponent<SyncActionWalk> ();
				if (ac != null)
					ac.Finish ();
			}
		}


		Action SyncActionWalk (string stateName, Landmark landmark, IActionCompleted callback)
		{
			SyncActionWalk ac = gameObject.AddComponent<SyncActionWalk> ();
			ac.Setting (stateName, landmark, callback);
			if (isServer)
				RpcSyncAction (stateName);
			else
				CmdSyncAction (stateName);
			return ac;
		}

		Action SyncAction (ActionInfo info, IActionCompleted callback)
		{
			NetworkActionPlay ac = gameObject.AddComponent<NetworkActionPlay> ();
			ac.Setting (info, callback);
			if (isServer)
				RpcSyncAction (info.stateName);
			else
				CmdSyncAction (info.stateName);
			return ac;
		}


		#region network

		[Command]
		void CmdSyncAction (string name)
		{
			anim.Play (name, 0, 0f);
			RpcSyncAction (name);
		}

		[ClientRpc]
		public void RpcSyncAction (string name)
		{
			if (!isLocalPlayer)
				anim.Play (name, 0, 0f);
		}

		[ClientRpc]
		public void RpcSetActionSpeed (float speed)
		{
			if (!isLocalPlayer)
				anim.speed = speed;
		}

		[Command]
		void CmdSyncItem (string[] path, bool isShown)
		{
			RpcSyncItem (path, isShown);
		}

		[ClientRpc]
		public void RpcSyncItem (string[] path, bool isShown)
		{
			if (!isLocalPlayer) {
				for (int i = 0; i < path.Length; ++i) {
					GameObject obj = gameObject.transform.Find (path [i]).gameObject;
					obj.SetActive (isShown);
				}
			}
		}

		#endregion


	}
}