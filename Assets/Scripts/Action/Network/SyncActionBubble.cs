using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class SyncActionBubble : ActionThread
	{
		public string currentContent;
		public float currentDuration;
		public int currentType;
		private IActionCompleted currentMonitor;

		public string recentContent;
		public float recentDuration;
		public int recentType;
		private IActionCompleted recentMonitor;
	
		protected List<Transform> bubbles;
		private List<Vector3> offsets;
		private List<UISceneBubbleEventReceiver> receivers = new List<UISceneBubbleEventReceiver> ();

		private Transform hook;
		private bool faceCamera;

		public float duration = -1.0f;
		private bool tryEnd;

		private string showStateName = "ChatBubbleShow";
		private string hideStateName = "ChatBubbleHide";

		public void Setting (List<Transform> bubbles, List<Vector3> offsets, Transform hook, bool faceCamera,
		                     string content, float duration, int type, IActionCompleted callback)
		{
			// 考虑是否需要顶替气泡
			if (string.IsNullOrEmpty (currentContent)) {
				Init (bubbles, offsets, hook, faceCamera);
				currentContent = content;
				currentDuration = duration;
				currentType = type;
				currentMonitor = callback;
				Begin ();
			} else {
				recentContent = content;
				recentDuration = duration;
				recentType = type;
				recentMonitor = callback;
				tryEnd = false;
			}
		}

		void Init (List<Transform> bubbles, List<Vector3> offsets, Transform hook, bool faceCamera)
		{
			this.bubbles = bubbles;
			this.offsets = offsets;
			this.hook = hook;
			this.faceCamera = faceCamera;
			receivers.Clear ();
			for (int i = 0; i < bubbles.Count; ++i) {
				if (bubbles [i] != null) {
					UISceneBubbleEventReceiver receiver = bubbles [i].GetComponent<UISceneBubbleEventReceiver> ();
					receiver.OnBubbleStartedHandler += OnBubbleStarted;
					receiver.OnBubbleFinishedHandler += OnBubbleFinished;
					receivers.Add (receiver);
					bubbles [i].Find ("root").localScale = Vector3.zero;
					bubbles [i].gameObject.SetActive (false);
				}
			}
		}

		void Begin ()
		{
			if (currentType < 0 || currentType >= bubbles.Count || bubbles [currentType] == null) {
				OnBubbleFinished ();
				return;
			}
			StopAllCoroutines ();
			Transform bubble = bubbles [currentType];
			bubble.gameObject.SetActive (true);
			bubble.GetComponent<Animator> ().Play (showStateName);
			OnSetContent (bubble, currentContent);
		}

		protected virtual void OnSetContent (Transform bubble, string content)
		{
		}

		void End ()
		{
			Transform bubble = bubbles [currentType];
			bubble.GetComponent<Animator> ().Play (hideStateName);
			StopAllCoroutines ();
		}

		void OnBubbleStarted ()
		{
			StartCoroutine (BubbleCountdown ());
		}

		void OnBubbleFinished ()
		{
			if (!(currentType < 0 || currentType >= bubbles.Count || bubbles [currentType] == null)) {
				bubbles [currentType].gameObject.SetActive (false);
			}
			if (currentMonitor != null)
				currentMonitor.OnActionCompleted (this);
			if (!string.IsNullOrEmpty (recentContent)) {
				currentContent = recentContent;
				currentDuration = recentDuration;
				currentType = recentType;
				currentMonitor = recentMonitor;
				ClearRecent ();
				Begin ();
			} else {
				for (int i = 0; i < bubbles.Count; ++i) {
					if (bubbles [i] != null) {
						receivers [i].OnBubbleStartedHandler -= OnBubbleStarted;
						receivers [i].OnBubbleFinishedHandler -= OnBubbleFinished;
					}
				}
				Destroy (this);
			}
		}

		void ClearRecent ()
		{
			recentContent = null;
			recentDuration = 0;
			recentType = 0;
			recentMonitor = null;
		}

		protected virtual void Update ()
		{
			if (bubbles [currentType] != null) {
				if (faceCamera)
					bubbles [currentType].rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
				if (hook != null)
					bubbles [currentType].position = hook.position + offsets [currentType];
			}
		}

		IEnumerator BubbleCountdown ()
		{
			duration = currentDuration;
			while (duration > 0 && string.IsNullOrEmpty (recentContent) && !tryEnd) {
				duration -= Time.deltaTime;
				yield return null;
			}
			End ();
		}

		// 留给Dealer的，用来关闭该类气泡
		public virtual void Terminate ()
		{
			tryEnd = true;
			ClearRecent ();
		}
	}
}