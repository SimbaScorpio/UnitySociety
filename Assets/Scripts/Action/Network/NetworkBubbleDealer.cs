using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkBubbleDealer : NetworkBehaviour, IActionCompleted
	{
		public bool isPlaying;
		public Transform root;
		public bool faceCamera = true;

		private Action lastAction;

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
			isPlaying = false;
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
			Quaternion temp = transform.rotation;
			transform.rotation = Quaternion.Euler (Vector3.zero);
			for (int i = 0; i < bubble.Count; ++i) {
				offset.Add (bubble [i].transform.position - root.position);
			}
			transform.rotation = temp;
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

		public void OnActionCompleted (Action action)
		{
			if (action == lastAction)
				isPlaying = false;
		}


		#region 错误

		public void ApplyErrorBubble (string content, float duration)
		{
			isPlaying = true;
			SyncActionErrorBubble ac = GetComponent<SyncActionErrorBubble> ();
			if (ac == null)
				ac = gameObject.AddComponent<SyncActionErrorBubble> ();
			ac.Setting (errorBubbles, errorOffsets, root, faceCamera, content, duration, 0, this);
			DealWithOverlap (ac);
			lastAction = ac;
		}

		#endregion

		#region 对话

		void ApplyChatBubble (string content, float duration, int type)
		{
			isPlaying = true;
			SyncActionChatBubble ac = GetComponent<SyncActionChatBubble> ();
			if (ac == null)
				ac = gameObject.AddComponent<SyncActionChatBubble> ();
			ac.Setting (chatBubbles, chatOffsets, root, faceCamera, content, duration, type, this);
			DealWithOverlap (ac);
			lastAction = ac;
		}

		public void NetworkChatBubble (string content, float duration, int type)
		{
			if (isServer) {
				ApplyChatBubble (content, duration, type);
				RpcApplyChatBubble (content, duration, type);
			} else
				CmdApplyChatBubble (content, duration, type);
		}

		[Command]
		void CmdApplyChatBubble (string content, float duration, int type)
		{
			ApplyChatBubble (content, duration, type);
			RpcApplyChatBubble (content, duration, type);
		}

		[ClientRpc]
		void RpcApplyChatBubble (string content, float duration, int type)
		{
			ApplyChatBubble (content, duration, type);
		}

		#endregion

		#region 标签

		void ApplyIconBubble (string content, float duration)
		{
			isPlaying = true;
			SyncActionIconBubble ac = GetComponent<SyncActionIconBubble> ();
			if (ac == null)
				ac = gameObject.AddComponent<SyncActionIconBubble> ();
			ac.Setting (iconBubbles, iconOffsets, root, faceCamera, content, duration, 0, this);
			DealWithOverlap (ac);
			lastAction = ac;
		}

		public void NetworkIconBubble (string content, float duration)
		{
			if (isServer) {
				ApplyIconBubble (content, duration);
				RpcApplyIconBubble (content, duration);
			} else
				CmdApplyIconBubble (content, duration);
		}

		[Command]
		public void CmdApplyIconBubble (string content, float duration)
		{
			ApplyIconBubble (content, duration);
			RpcApplyIconBubble (content, duration);
		}

		[ClientRpc]
		void RpcApplyIconBubble (string content, float duration)
		{
			ApplyIconBubble (content, duration);
		}

		#endregion

		#region 屏幕

		void ApplyScreenBubble (string screen, float duration, int type)
		{
			isPlaying = true;
			SyncActionScreenBubble ac = GetComponent<SyncActionScreenBubble> ();
			if (ac == null)
				ac = gameObject.AddComponent<SyncActionScreenBubble> ();
			ac.Setting (screenBubbles, screenOffsets, null, false, screen, duration, type, this);
			lastAction = ac;
		}

		public void NetworkScreenBubble (string screen, float duration, int type)
		{
			if (isServer) {
				ApplyScreenBubble (screen, duration, type);
				RpcApplyScreenBubble (screen, duration, type);
			} else
				CmdApplyScreenBubble (screen, duration, type);
		}

		[Command]
		public void CmdApplyScreenBubble (string screen, float duration, int type)
		{
			ApplyScreenBubble (screen, duration, type);
			RpcApplyScreenBubble (screen, duration, type);
		}

		[ClientRpc]
		void RpcApplyScreenBubble (string screen, float duration, int type)
		{
			ApplyScreenBubble (screen, duration, type);
		}

		#endregion

		#region 关键词

		void ApplyKeywordBubble (string content, float duration)
		{
			isPlaying = true;
			SyncActionKeywordBubble ac = GetComponent<SyncActionKeywordBubble> ();
			if (ac == null)
				ac = gameObject.AddComponent<SyncActionKeywordBubble> ();
			ac.Setting (keywordBubbles, keywordOffsets, root, faceCamera, content, duration, 0, this);
			DealWithOverlap (ac);
			lastAction = ac;
		}

		public void NetworkKeywordBubble (string content, float duration)
		{
			if (isServer) {
				ApplyKeywordBubble (content, duration);
				RpcApplyKeywordBubble (content, duration);
			} else
				CmdApplyKeywordBubble (content, duration);
		}

		[Command]
		public void CmdApplyKeywordBubble (string content, float duration)
		{
			ApplyKeywordBubble (content, duration);
			RpcApplyKeywordBubble (content, duration);
		}

		[ClientRpc]
		void RpcApplyKeywordBubble (string content, float duration)
		{
			ApplyKeywordBubble (content, duration);
		}

		#endregion
	}
}