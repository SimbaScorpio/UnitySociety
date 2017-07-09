using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionLookAt : ActionAuto
{
	public GameObject obj;
	public Vector3 lookAtTargetPosition;
	private ActionCompleted monitor;

	private Animator animator;
	private Vector3 lookAtPosition;

	void Start()
	{
		this.ID = ActionID.LOOKAT;
	}


	public void setting(GameObject obj, Vector3 lookAtTargetPosition, ActionCompleted monitor)
	{
		this.obj = obj;
		this.lookAtTargetPosition = lookAtTargetPosition;
		this.monitor = monitor;

		animator = obj.GetComponent<Animator>();
		//float angle = obj.transform.rotation.eulerAngles.y;
		//Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
		//lookAtPosition = obj.transform.position + dir.normalized;
		//Debug.DrawRay(obj.transform.position, dir, Color.green);
	}


	void Update()
	{
		/*
		Vector3 curDir = lookAtPosition - obj.transform.position;
		Vector3 futDir = lookAtTargetPosition - obj.transform.position;

		curDir.y = futDir.y = 0;

		//Debug.DrawRay(obj.transform.position, futDir, Color.red);
		//Debug.DrawRay(obj.transform.position, curDir, Color.blue);

		curDir = Vector3.RotateTowards(curDir, futDir, Time.deltaTime, float.PositiveInfinity);
		lookAtPosition = obj.transform.position + curDir;

		float angle = Vector3.Angle(curDir, futDir);
		agent.transform.rotation = Quaternion.Euler(new Vector3(0, -angle, 0));
		*/

		animator.SetBool("IsWalking", true);
		float angle = obj.transform.rotation.eulerAngles.y;
		float angle2 = angle / 180 * Mathf.PI;
		Vector3 curDir = new Vector3(Mathf.Sin(angle2), 0, Mathf.Cos(angle2)).normalized;
		Vector3 futDir = lookAtTargetPosition - obj.transform.position;

		curDir.y = futDir.y = 0;

		Debug.DrawRay(obj.transform.position, futDir, Color.red);
		Debug.DrawRay(obj.transform.position, curDir, Color.blue);

		curDir = Vector3.RotateTowards(curDir, futDir, 3 * Time.deltaTime, float.PositiveInfinity);
		obj.transform.LookAt(obj.transform.position + curDir);

		if (Vector3.Angle(curDir, futDir) < 0.1f)
			Finish();
	}


	public void Finish()
	{
		animator.SetBool("IsWalking", false);
		if (monitor != null)
		{
			monitor.OnActionCompleted(this);
		}
		Destroy(this);
	}
}
