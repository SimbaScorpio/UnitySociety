using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
using Pathfinding.RVO;

/* 使用A*PathFinding扩展插件的寻路行走动作
 * */
public class ActionWalkTo : AIPath
{
	public GameObject obj;
	public Landmark landmark;
	public IActionCompleted monitor;

	private bool finalRotate = false;
	private float animationSpeed = 1.1f;
	private Animator animator;
	private NavmeshCut navCut;

	private readonly int hashSpeedPara = Animator.StringToHash ("Speed");

	private Vector3 velocity;
	private Vector3 direction;
	private float animSpeed;
	private float angle;


	public void Setting (GameObject obj, Landmark target, IActionCompleted monitor)
	{
		this.obj = obj;
		this.landmark = target.Copy ();
		this.target = target.position;
		this.monitor = monitor;
		animator = GetComponent<Animator> ();
		navCut = GetComponent<NavmeshCut> ();
		navCut.enabled = false;
	}

	public override void Update ()
	{
		velocity = Vector3.zero;
		if (finalRotate) {
			FinalRotate ();
		} else {
			direction = CalculateVelocity (transform.position);
			RotateTowards (targetDirection);
			if (rvoController != null) {
				rvoController.Move (direction);
				velocity = rvoController.velocity;
			}
			animSpeed = velocity.magnitude;
			animator.SetFloat (hashSpeedPara, animSpeed);
			//Modify animation speed to match velocity
			animator.speed = animSpeed * animationSpeed;
		}
	}

	void FinalRotate ()
	{
		transform.rotation = Quaternion.Lerp (transform.rotation, landmark.rotation, Time.deltaTime * 20);
		angle = Quaternion.Angle (landmark.rotation, tr.rotation);
		animSpeed = angle / 180.0f;
		animator.SetFloat (hashSpeedPara, animSpeed);
		animator.speed = animSpeed * animationSpeed;
		if (angle < 0.1f)
			Finish ();
	}

	public override void OnTargetReached ()
	{
		// rotate to the target direction before finish
		finalRotate = true;
	}

	public void Finish ()
	{
		// make sure to stop
		rvoController.Move (Vector3.zero);
		// cut mesh when stopped
		navCut.enabled = true;
		// set default speed
		animator.speed = 1;
		animator.SetFloat (hashSpeedPara, 0);

		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
