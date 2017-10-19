using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionInfo
	{
		public string stateName;
		public string[] itemPaths;
		// name for instantiate item
		public string prefabName;
		// type for stuff in scene
		public StuffType stuffType;

		public ActionInfo (string stateName, string[] itemPaths, string prefabName, StuffType type)
		{
			this.stateName = stateName;
			this.itemPaths = itemPaths;
			this.prefabName = prefabName;
			this.stuffType = type;
		}
	}

	/// <summary>
	/// Network action play.
	/// 1) items are totally owned by player
	/// 2) items should grab from scene, but act like player's own, finally release to scene
	/// 3) items are created by player and released into scene
	/// 4) stuff always remain in scene
	/// </summary>
	/// 
	public class NetworkActionPlay : ActionSingle
	{
		public ActionInfo info;
		protected IActionCompleted monitor;
		protected Animator anim;
		private bool hasFinished = false;

		private NetworkActionDealer dealer;

		public bool findingPartner{ get; set; }

		public bool waitingPartner{ get; set; }

		#region basic

		public virtual void Setting (ActionInfo info, IActionCompleted callback)
		{
			this.info = info;
			this.monitor = callback;
			dealer = GetComponent<NetworkActionDealer> ();
			anim = GetComponent<Animator> ();
			anim.Play (info.stateName, 0, 0f);
		}

		void Update ()
		{
			if (!string.IsNullOrEmpty (info.stateName)) {
				if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1 || !anim.GetCurrentAnimatorStateInfo (0).IsName (info.stateName)) {
					Finish ();
				}
			}
		}

		public virtual void Finish ()
		{
			if (monitor != null) {
				monitor.OnActionCompleted (this);
			}
			if (!hasFinished) {
				hasFinished = true;
				Free ();
			}
		}

		public void SafeFinish ()
		{
			OnItemHidden ();
			OnItemReleased ();
			OnItemReset ();
			OnStuffPutDown ();
			monitor = null;
			this.Finish ();
		}

		void OnDestroy ()
		{
			if (!hasFinished) {
				hasFinished = true;
				Finish ();
			}
		}

		#endregion




		#region callbacks

		// 普通的显示和消失
		public void OnItemShown ()
		{
			for (int i = 0; i < info.itemPaths.Length; ++i) {
				transform.Find (info.itemPaths [i]).gameObject.SetActive (true);
			}
		}

		public void OnItemHidden ()
		{
			for (int i = 0; i < info.itemPaths.Length; ++i) {
				transform.Find (info.itemPaths [i]).gameObject.SetActive (false);
			}
		}

		// 创建物品至场景
		public void OnItemCreated ()
		{
			Transform parent = transform.Find (info.itemPaths [0]);
			dealer.createdItem = Instantiate (Resources.Load ("Prefabs/" + info.prefabName)) as GameObject;
			dealer.createdItem.GetComponent<CopyTransform> ().target = parent;
		}

		public void OnItemReleased ()
		{
			if (dealer.createdItem != null) {
				dealer.createdItem.GetComponent<CopyTransform> ().target = null;
				dealer.createdItem = null;
			}
		}

		// 从场景中拿取物品
		public void OnItemGained ()
		{
			// search item in scene
			dealer.gainedItems.Clear ();
			for (int i = 0; i < info.itemPaths.Length; ++i) {
				string[] root_name = SplitRootAndName (info.itemPaths [i]);
				Transform tr = transform.Find (root_name [0]);
				string itemName = GetRealName (root_name [1]);
				GameObject[] objs = GameObject.FindGameObjectsWithTag (itemName);

				// find closest obj relative to parent position
				GameObject closest = null;
				float min = float.MaxValue;
				foreach (GameObject obj in objs) {
					if (obj.GetComponent<CopyTransform> ().target == null) {
						float distance = Vector3.Distance (obj.transform.position, tr.position);
						if (distance < min) {
							min = distance;
							closest = obj;
						}
					}
				}
				if (closest != null) {
					closest.GetComponent<CopyTransform> ().target = transform.Find (info.itemPaths [i]);
					dealer.gainedItems.Add (closest);
				}
			}
		}

		public void OnItemReset ()
		{
			for (int i = 0; i < dealer.gainedItems.Count; ++i) {
				if (dealer.gainedItems [i] != null) {
					dealer.gainedItems [i].GetComponent<CopyTransform> ().target = null;
				}
			}
			dealer.gainedItems.Clear ();
		}


		// 完全场景物品系列
		private string boneRoot = "hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/";

		public void OnStuffPickedUp ()
		{
			Stuff stuff = SearchForStuffType (info.stuffType);
			if (stuff != null) {
				dealer.stuffs.Add (stuff);
				stuff.SetParent (transform.Find (boneRoot));
			}
		}

		public void OnStuffPutDown ()
		{
			if (dealer.stuffs.Count > 0) {
				dealer.stuffs [0].SetParent (null);
				dealer.stuffs.RemoveAt (0);
			}
		}

		public void OnStuffPutAway ()
		{
			if (dealer.stuffs.Count > 0)
				dealer.stuffs [0].gameObject.SetActive (false);
		}

		public void OnStuffTakenOut ()
		{
			if (dealer.stuffs.Count > 0)
				dealer.stuffs [0].gameObject.SetActive (true);
		}

		public void OnStuffGivenTo ()
		{
			StartCoroutine (SearchForPartner ());
		}

		public void OnStuffTakenFrom ()
		{
			StartCoroutine (WaitForPartner ());
		}

		/** Search the nearest reasonable stuff in front of the player */
		Stuff SearchForStuffType (StuffType stuffType)
		{
			Vector3 forwardDir = transform.TransformDirection (transform.forward);
			Vector3 objectDir = Vector3.zero;

			float reasonableDistance = 2f;
			float distance = 0f;
			float nearestDistance = float.MaxValue;
			Stuff nearestStuff = null;
			Stuff tempStuff = null;

			GameObject[] objs = GameObject.FindGameObjectsWithTag (stuffType.ToString ());
			foreach (GameObject stuffobj in objs) {
				distance = Vector3.Distance (transform.position, stuffobj.transform.position);
				if (distance <= reasonableDistance && distance < nearestDistance) {
					objectDir = transform.TransformDirection (stuffobj.transform.position - transform.position);
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

		IEnumerator SearchForPartner ()
		{
			// stop animation till partner show up
			anim.speed = 0;
			dealer.RpcSetActionSpeed (0);
			findingPartner = true;

			GameObject[] objs = GameObject.FindGameObjectsWithTag ("Player");
			while (findingPartner) {
				foreach (GameObject player in objs) {
					// player in certain range
					if (player != this.gameObject && Vector3.Distance (player.transform.position, transform.position) < 2f) {
						// player in the front
						Vector3 direction = transform.TransformDirection (player.transform.position - transform.position);
						Vector3 forward = transform.TransformDirection (transform.forward);
						if (Vector3.Dot (direction, forward) > 0) {
							// player is waiting
							NetworkActionPlay np = player.GetComponent<NetworkActionPlay> ();
							if (np != null && np.waitingPartner) {
								player.GetComponent<NetworkActionDealer> ().stuffs.Add (dealer.stuffs [0]);
								dealer.stuffs [0].SetParent (player.transform.Find (boneRoot));
								dealer.stuffs.RemoveAt (0);

								anim.speed = 1;
								dealer.RpcSetActionSpeed (1);

								findingPartner = false;
								np.waitingPartner = false;
								break;
							}
						}
					}
				}
				yield return null;
			}
		}

		IEnumerator WaitForPartner ()
		{
			// stop animation till partner show up
			anim.speed = 0;
			dealer.RpcSetActionSpeed (0);
			waitingPartner = true;

			while (waitingPartner) {
				yield return null;
			}

			anim.speed = 1;
			dealer.RpcSetActionSpeed (1);
		}


		#endregion

		string[] SplitRootAndName (string path)
		{
			int index = path.LastIndexOf ("/");
			if (index >= 0) {
				return new string[] { path.Substring (0, index), path.Substring (index + 1, path.Length - index - 1) };
			} else {
				return new string[] { "", path };
			}
		}

		string GetRealName (string name)
		{
			int cut = name.Length;
			for (int i = name.Length - 1; i >= 0; --i) {
				if (name [i] == '_' || (name [i] >= '0' && name [i] <= '9')) {
					--cut;
				} else {
					break;
				}
			}
			return name.Substring (0, cut);
		}
	}
}