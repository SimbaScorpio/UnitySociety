using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionLookAt : ActionSingle
{
	public GameObject obj;
	public Vector3 lookAtTargetPosition;
	private ActionCompleted monitor;

	private Person person;
	private Animator animator;

	public float TURNINGSPEED = 3.0f;


	void Start ()
	{
		this.ID = ActionID.LOOKAT;
		Check ();
	}


	public void Setting (GameObject obj, Vector3 lookAtTargetPosition, ActionCompleted monitor)
	{
		this.obj = obj;
		this.lookAtTargetPosition = lookAtTargetPosition;
		this.monitor = monitor;

		person = obj.GetComponent<Person> ();
		animator = obj.GetComponent<Animator> ();

		person.Action = this;
	}


	void Update ()
	{
		animator.SetBool ("IsWalking", true);
		float angle = obj.transform.rotation.eulerAngles.y;
		float anglePI = angle / 180 * Mathf.PI;
		Vector3 curDir = new Vector3 (Mathf.Sin (anglePI), 0, Mathf.Cos (anglePI)).normalized;
		Vector3 futDir = lookAtTargetPosition - obj.transform.position;

		curDir.y = futDir.y = 0;

		curDir = Vector3.RotateTowards (curDir, futDir, TURNINGSPEED * Time.deltaTime, float.PositiveInfinity);
		obj.transform.LookAt (obj.transform.position + curDir);

		if (Vector3.Angle (curDir, futDir) < 0.1f)
			Finish ();
	}


	public void Finish ()
	{
		person.Action = null;
		animator.SetBool ("IsWalking", false);
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Destroy (this);
	}
}
