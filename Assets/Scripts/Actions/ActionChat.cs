using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionChat : ActionAuto
{
	public GameObject obj;
	public string content;
	public float duration;

	private ActionCompleted monitor = null;

	private GameObject canvas;
	private Animator animator;


	void Start()
	{
		this.ID = ActionID.CHAT;
	}


	public void setting(GameObject obj, string content, float duration, ActionCompleted callback)
	{
		this.obj = obj;
		this.content = content;
		this.duration = duration + 0.2f;
		this.monitor = callback;

		canvas = obj.transform.Find("Bubble").gameObject;
		animator = canvas.GetComponent<Animator>();

		this.Begin();
	}


	void Begin()
	{
		canvas.SetActive(true);
		canvas.GetComponentInChildren<Text>().text = content;
		animator.SetBool("IsPoping", true);
	}


	void Finish()
	{
		if (monitor != null)
		{
			monitor.OnActionCompleted(this);
		}
		Destroy(this);
	}


	void Update()
	{
		if (duration < 0)
		{
			this.Finish();
		}
		else
		{
			duration -= Time.deltaTime;
			if (duration <= 0.2f)
			{
				animator.SetBool("IsPoping", false);
			}
			//canvas.transform.LookAt(Camera.main.transform.position);
			//canvas.transform.Rotate(new Vector3(0, 180, 0));
			canvas.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
		}
	}
}
