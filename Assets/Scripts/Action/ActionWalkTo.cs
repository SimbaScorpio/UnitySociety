using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionWalkTo : ActionSingle
{
	public GameObject obj;
	public Vector3 destinationPosition;
	public bool finalRotate = false;
	public Quaternion destinationRotation;
	private IActionCompleted monitor;

	private Animator animator;
	private NavMeshAgent agent;

	private const float NAVMESHSAMPLEDISTANCE = 4f;
	private const float STOPDISTANCEPROPOTION = 0.1f;
	private const float TURNSPEEDTHRESHOLD = 0.1f;
	private const float SPEEDDAMPTIME = 0.5f;
	private const float SLOWINGSPEED = 0.2f;
	private const float TURNSMOOTHING = 15f;
	private const float ANIMATORSPEEDPROPOTION = 1.5f;

	private float speed;

	private readonly int hashSpeedPara = Animator.StringToHash ("Speed");

	public void Setting (GameObject obj, Vector3 destinationPosition, bool finalRotate, Quaternion destinationRotation, IActionCompleted monitor)
	{
		this.ID = ActionID.WALKTO;
		this.obj = obj;
		this.finalRotate = finalRotate;
		this.destinationPosition = destinationPosition;
		this.destinationRotation = destinationRotation;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		this.agent = obj.GetComponent<NavMeshAgent> ();

		Begin ();
	}

	void Begin ()
	{
		NavMeshHit hit;
		if (NavMesh.SamplePosition (destinationPosition, out hit, NAVMESHSAMPLEDISTANCE, NavMesh.AllAreas)) {
			destinationPosition = hit.position;
		}
		agent.updateRotation = false;
		agent.SetDestination (destinationPosition);
		agent.isStopped = false;
	}

	void Update ()
	{
		if (agent.pathPending)
			return;
		speed = agent.desiredVelocity.magnitude;

		if (agent.remainingDistance <= agent.stoppingDistance * STOPDISTANCEPROPOTION)
			Stopping (out speed);
		else if (agent.remainingDistance <= agent.stoppingDistance)
			Slowing (out speed, agent.remainingDistance);
		else if (agent.remainingDistance > TURNSPEEDTHRESHOLD)
			Moving ();

		if (speed > 0.1f)
			animator.SetFloat (hashSpeedPara, speed, SPEEDDAMPTIME, Time.deltaTime);
		else
			animator.SetFloat (hashSpeedPara, speed);
		animator.speed = speed * ANIMATORSPEEDPROPOTION;

		if (transform.position == destinationPosition) {
			animator.speed = 1;
			agent.ResetPath ();
			Finish ();
		}
	}

	void Stopping (out float speed)
	{
		agent.isStopped = true;
		transform.position = destinationPosition;
		speed = 0f;
	}

	void Slowing (out float speed, float distanceToDestination)
	{
		agent.isStopped = true;
		float propotionDistance = 1f - distanceToDestination / agent.stoppingDistance;
		Quaternion targetRotation = finalRotate ? destinationRotation : transform.rotation;
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, propotionDistance);
		transform.position = Vector3.MoveTowards (transform.position, destinationPosition, SLOWINGSPEED * Time.deltaTime);
		speed = Mathf.Lerp (SLOWINGSPEED, 0f, propotionDistance);
	}

	void Moving ()
	{
		Quaternion targetRotation = Quaternion.LookRotation (agent.desiredVelocity);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, TURNSMOOTHING * Time.deltaTime);
	}

	void Finish ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
