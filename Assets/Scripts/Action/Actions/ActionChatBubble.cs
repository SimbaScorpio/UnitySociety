using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionChatBubble : ActionThread
{
	private GameObject obj;
	private GameObject bubble;
	public string content = "";
	public float duration = -1.0f;
	private IActionCompleted monitor = null;

	public Queue<string> contents = new Queue<string> ();
	private Queue<float> durations = new Queue<float> ();
	private Queue<IActionCompleted> monitors = new Queue<IActionCompleted> ();

	private Animator animator;
	private bool startCounting = false;
	private float deltaHeight = 0.0f;
	private Transform hip;

	public void Setting (GameObject obj, GameObject bubble, string content, float duration, IActionCompleted callback)
	{
		if (this.obj == null) {
			this.id = ActionID.CHATBUBBLE;
			this.obj = obj;
			this.bubble = bubble;
			this.content = content;
			this.duration = duration;
			this.monitor = callback;
			animator = bubble.GetComponent<Animator> ();
			hip = obj.transform.Find ("hip_ctrl");
			Begin ();
		} else {
			contents.Enqueue (content);
			durations.Enqueue (duration);
			monitors.Enqueue (callback);
			animator.SetTrigger ("Fade");
		}
	}

	void Begin ()
	{
		Text text = bubble.GetComponentInChildren<Text> ();
		text.text = content;
		ContentSizeFitter fitter = text.GetComponent<ContentSizeFitter> ();
		fitter.CallBack (delegate(Vector2 size) {
			Image image = bubble.GetComponentInChildren<Image> ();
			float height = size.y + 10;
			height = height > 25 ? height : 25;
			image.rectTransform.sizeDelta = new Vector2 (size.x * 1.3f, size.y + 10);
			float worldHeight = height * image.rectTransform.localScale.y;
			deltaHeight = worldHeight / 2 + 1.0f;
		});
		bubble.SetActive (true);
		animator.SetTrigger ("Pop");
	}

	public void OnChatStarted ()
	{
		startCounting = true;
	}

	public void OnChatFinished ()
	{
		startCounting = false;
		StartCoroutine (wait ());
	}

	IEnumerator wait ()
	{
		yield return new WaitForEndOfFrame ();
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

	void Update ()
	{
		if (duration < 0)
			animator.SetTrigger ("Fade");
		if (startCounting)
			duration -= Time.deltaTime;
		//bubble.transform.LookAt(Camera.main.transform.position);
		//bubble.transform.Rotate(new Vector3(0, 180, 0));
		bubble.transform.localPosition = new Vector3 (0, hip.localPosition.y + deltaHeight, 0);
		bubble.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
	}
}
