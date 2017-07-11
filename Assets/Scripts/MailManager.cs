using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailManager : MonoBehaviour
{
	public GameObject MailPref;
	public float Speed = 1;

	/*
	private class Bunch
	{
		public GameObject initiator;
		public List<GameObject> receivers;
		public Bunch(GameObject initiator) {
			this.initiator = initiator;
			receivers = new List<GameObject>();
		}
	}
	*/

	private class Mail
	{
		public GameObject canvas;
		public GameObject follower;
		public float duration;

		public Mail (GameObject canvas, GameObject follower, float duration)
		{
			this.canvas = canvas;
			this.follower = follower;
			this.duration = duration;
		}
	}

	private List<Mail> mails = new List<Mail> ();
	private List<GameObject> factory = new List<GameObject> ();

	private static MailManager instance;

	public static MailManager GetInstance ()
	{
		return instance;
	}

	void Awake ()
	{
		instance = this;
	}


	void Update ()
	{
		for (int i = 0; i < mails.Count; ++i) {
			Mail mail = mails [i];
			if (!AssertLegal (mail)) {
				DeleteMail (mail);
			} else {
				if (mail.duration <= 0) {
					DeleteMail (mail);
				} else {
					Vector3 currentPosition = mail.canvas.transform.position;
					Vector3 targetPosition = mail.follower.transform.position + Vector3.up;
					if (Vector3.Distance (currentPosition, targetPosition) <= 0.1f) {
						mail.duration -= Time.deltaTime;
					}
					Vector3 newPosition = Vector3.MoveTowards (currentPosition, targetPosition, Speed * Time.deltaTime);
					mail.canvas.transform.position = newPosition;
				}
			}
		}
	}


	public void SendMail (GameObject initiator, List<GameObject> receivers, float duration)
	{
		if (initiator != null) {
			for (int i = 0; i < receivers.Count; ++i) {
				if (receivers [i] != null) {
					Mail mail = GetMail (receivers [i], duration);
					mail.canvas.transform.position = initiator.transform.position + Vector3.up;
					mails.Add (mail);
				}
			}
		}
	}


	Mail GetMail (GameObject follower, float duration)
	{
		if (factory.Count > 0) {
			GameObject mailobj = factory [0];
			mailobj.SetActive (true);
			factory.RemoveAt (0);
			Mail mail = new Mail (mailobj, follower, duration);
			return mail;
		} else {
			GameObject mailobj = Instantiate (MailPref) as GameObject;
			Mail mail = new Mail (mailobj, follower, duration);
			return mail;
		}
	}


	void DeleteMail (Mail mail)
	{
		if (mail == null)
			return;
		if (mail.canvas != null) {
			mail.canvas.transform.position = new Vector3 (10000, 0, 0);	// 画面外
			mail.canvas.SetActive (false);
			factory.Add (mail.canvas);
		}
		int index = mails.IndexOf (mail);
		if (index >= 0 && index < mails.Count) {
			mails.RemoveAt (index);
		}
	}


	bool AssertLegal (Mail mail)
	{
		return (mail != null && mail.canvas && mail.follower);
	}
}
