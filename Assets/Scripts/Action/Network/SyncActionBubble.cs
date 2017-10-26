using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class SyncActionBubble : ActionThread
	{
		protected IActionCompleted monitor;

		private Queue<string> contents = new Queue<string> ();
		private Queue<float> durations = new Queue<float> ();
		private Queue<IActionCompleted> monitors = new Queue<IActionCompleted> ();

		private GameObject bubble;
		private Text text;
		private UISceneBubbleEventReceiver handler;

		public string content = "";
		public float duration = -1.0f;

		private Animator animator;
		private bool startCounting = false;
		private float deltaHeight = 0.0f;
		private Transform hip;

		void Awake ()
		{
			this.bubble = transform.Find ("Bubble").gameObject;
			this.hip = transform.Find ("hip_ctrl");
			this.animator = bubble.GetComponent<Animator> ();
			this.text = bubble.GetComponentInChildren<Text> ();
			this.handler = bubble.GetComponent<UISceneBubbleEventReceiver> ();
			handler.OnBubbleStartedHandler += OnBubbleStarted;
			handler.OnBubbleFinishedHandler += OnBubbleFinished;
		}

		public virtual void Setting (string content, float duration, IActionCompleted callback)
		{
			if (string.IsNullOrEmpty (this.content)) {
				this.content = content;
				this.duration = duration;
				this.monitor = callback;
				Begin ();
			} else {
				contents.Enqueue (content);
				durations.Enqueue (duration);
				monitors.Enqueue (callback);
				animator.Play ("Fade", 0, 0);
			}
		}

		void Begin ()
		{
			text.text = content;
			ContentSizeFitter fitter = text.GetComponent<ContentSizeFitter> ();
			fitter.CallBack (delegate(Vector2 size) {
				// bg image size
				Image image = bubble.GetComponentInChildren<Image> ();
				float height = size.y + 10;
				height = height > 25 ? height : 25;
				image.rectTransform.sizeDelta = new Vector2 (size.x * 1.3f, size.y + 10);

				// bubble height
				float worldHeight = height * image.rectTransform.localScale.y;
				deltaHeight = worldHeight / 2 + 1.0f;
			});
			bubble.SetActive (true);
			animator.Play ("Pop", 0, 0);
		}

		void OnBubbleStarted ()
		{
			startCounting = true;
		}

		void OnBubbleFinished ()
		{
			if (monitor != null) {
				monitor.OnActionCompleted (this);
			}
			if (contents.Count > 0) {
				content = contents.Dequeue ();
				duration = durations.Dequeue ();
				monitor = monitors.Dequeue ();
				Begin ();
			} else {
				bubble.SetActive (false);
				Free ();
			}
		}

		void OnDestroy ()
		{
			handler.OnBubbleStartedHandler -= OnBubbleStarted;
			handler.OnBubbleFinishedHandler -= OnBubbleFinished;
		}

		void Update ()
		{
			if (startCounting) {
				if (duration < 0) {
					startCounting = false;
					animator.Play ("Fade", 0, 0);
				} else
					duration -= Time.deltaTime;
			}
			if (bubble != null) {
				bubble.transform.localPosition = new Vector3 (0, hip.localPosition.y + deltaHeight, 0);
				bubble.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
			}
		}
	}
}