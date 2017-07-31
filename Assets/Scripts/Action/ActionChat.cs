using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionChat : ActionThread
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

	public void Setting (GameObject obj, GameObject bubble, string content, float duration, IActionCompleted callback)
	{
		if (this.obj == null) {
			this.id = ActionID.CHAT;
			this.obj = obj;
			this.bubble = bubble;
			this.content = content;
			this.duration = duration;
			this.monitor = callback;
			animator = bubble.GetComponent<Animator> ();
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
		bubble.SetActive (true);
		bubble.GetComponentInChildren<Text> ().text = content;
		animator.SetTrigger ("Pop");
		startCounting = false;
	}


	public void OnChatStarted ()
	{
		startCounting = true;
	}


	public void OnChatFinished ()
	{
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
			animator.SetBool ("IsPoping", false);
		if (startCounting)
			duration -= Time.deltaTime;
		//bubble.transform.LookAt(Camera.main.transform.position);
		//bubble.transform.Rotate(new Vector3(0, 180, 0));
		bubble.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
	}
}
