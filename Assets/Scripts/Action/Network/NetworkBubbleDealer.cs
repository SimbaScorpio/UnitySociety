using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkBubbleDealer : NetworkBehaviour
	{
		public Transform root;
		public bool faceCamera = true;
		public List<Transform> chatBubbles;
		public List<Transform> errorBubbles;
		public List<Transform> iconBubbles;
		public List<Transform> screenBubbles;
		public List<Transform> keywordBubbles;

		private List<Vector3> chatOffsets = new List<Vector3> ();
		private List<Vector3> errorOffsets = new List<Vector3> ();
		private List<Vector3> iconOffsets = new List<Vector3> ();
		private List<Vector3> screenOffsets = new List<Vector3> ();
		private List<Vector3> keywordOffsets = new List<Vector3> ();

		void Start ()
		{
			if (root != null) {
				CalOffset (chatBubbles, chatOffsets);
				CalOffset (errorBubbles, errorOffsets);
				CalOffset (iconBubbles, iconOffsets);
				CalOffset (screenBubbles, screenOffsets);
				CalOffset (keywordBubbles, keywordOffsets);
			}
		}

		void CalOffset (List<Transform> bubble, List<Vector3> offset)
		{
			offset.Clear ();
			foreach (Transform tr in bubble) {
				if (tr != null) {
					offset.Add (tr.position - root.position);
				}
			}
		}

		void DealWithOverlap (SyncActionBubble ac)
		{
			SyncActionScreenBubble[] sb = GetComponents<SyncActionScreenBubble> ();
			List<SyncActionScreenBubble> list = new List<SyncActionScreenBubble> (sb);
			SyncActionBubble[] ab = GetComponents<SyncActionBubble> ();
			for (int i = 0; i < ab.Length; ++i) {
				if (ab [i] != ac && !list.Contains (ab [i] as SyncActionScreenBubble)) {
					ab [i].Terminate ();
				}
			}
		}

		public void ApplyChatBubble (string content, float duration, int type, IActionCompleted callback)
		{
			SyncActionChatBubble ac = GetComponent<SyncActionChatBubble> ();
			if (ac == null)
				ac = gameObject.AddComponent<SyncActionChatBubble> ();
			ac.Setting (chatBubbles, chatOffsets, root, faceCamera, content, duration, type, callback);
			DealWithOverlap (ac);
		}

		public void ApplyErrorBubble (string content, float duration)
		{
			SyncActionErrorBubble ac = GetComponent<SyncActionErrorBubble> ();
			if (ac == null)
				ac = gameObject.AddComponent<SyncActionErrorBubble> ();
			ac.Setting (errorBubbles, errorOffsets, root, faceCamera, content, duration, 0, null);
			DealWithOverlap (ac);
		}

		public void ApplyIconBubble (string icon, float duration, IActionCompleted callback)
		{
			SyncActionIconBubble ac = GetComponent<SyncActionIconBubble> ();
			if (ac == null)
				ac = gameObject.AddComponent<SyncActionIconBubble> ();
			ac.Setting (iconBubbles, iconOffsets, root, faceCamera, icon, duration, 0, callback);
			DealWithOverlap (ac);
		}

		public void ApplyScreenBubble (string screen, float duration, int type, IActionCompleted callback)
		{
			SyncActionScreenBubble ac = GetComponent<SyncActionScreenBubble> ();
			if (ac == null)
				ac = gameObject.AddComponent<SyncActionScreenBubble> ();
			ac.Setting (screenBubbles, screenOffsets, null, false, screen, duration, type, callback);
		}

		public void ApplyKeywordBubble (string content, float duration)
		{
			RpcApplyKeywordBubble (content, duration);
		}

		[ClientRpc]
		void RpcApplyKeywordBubble (string content, float duration)
		{
			SyncActionKeywordBubble ac = GetComponent<SyncActionKeywordBubble> ();
			if (ac == null)
				ac = gameObject.AddComponent<SyncActionKeywordBubble> ();
			ac.Setting (keywordBubbles, keywordOffsets, root, faceCamera, content, duration, 0, null);
			DealWithOverlap (ac);
		}
	}
}