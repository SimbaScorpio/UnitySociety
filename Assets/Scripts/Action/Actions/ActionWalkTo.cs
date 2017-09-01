using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;
using Pathfinding.RVO;

/* 使用A*PathFinding扩展插件的寻路行走动作
 * */
public class ActionWalk : AIPath
{
	public GameObject obj;
	public IActionCompleted monitor;

	private bool finalRotate = false;
	private float animationSpeed = 0.7f;
	private Animator animator;
	private NavmeshCut navCut;

	private readonly int hashSpeedPara = Animator.StringToHash ("Speed");

	private Vector3 velocity;
	private Vector3 direction;
	private float speed;
	private float angle;


	public void Setting (GameObject obj, Transform target, IActionCompleted monitor)
	{
		this.obj = obj;
		this.target = target;
		this.monitor = monitor;
		animator = GetComponent<Animator> ();
		navCut = GetComponent<NavmeshCut> ();
		navCut.enabled = false;
	}

	void Update ()
	{
		velocity = Vector3.zero;
		if (finalRotate) {
			transform.rotation = Quaternion.Lerp (transform.rotation, target.rotation, Time.deltaTime * 20);
			angle = Quaternion.Angle (target.rotation, tr.rotation);
			speed = angle / 180.0f;
			animator.SetFloat (hashSpeedPara, speed);
			animator.speed = speed * animationSpeed;
			if (angle < 0.1f)
				Finish ();
		} else {
			if (canMove) {  // begins
				direction = CalculateVelocity (transform.position);
				RotateTowards (targetDirection);
				if (rvoController != null) {
					rvoController.Move (direction);
					velocity = rvoController.velocity;
				}
			}
			speed = velocity.magnitude;
			animator.SetFloat (hashSpeedPara, speed);
			//Modify animation speed to match velocity
			animator.speed = speed * animationSpeed;
		}
	}

	public override void OnTargetReached ()
	{
		// Rotate to the target direction before finish
		finalRotate = true;
	}

	public void Finish ()
	{
		navCut.enabled = true;
		animator.speed = 1;
		animator.SetFloat (hashSpeedPara, 0);
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
