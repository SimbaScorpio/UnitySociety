using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DesignSociety
{
	/* 这是用于Unity自带AI系统的导航行走动作，可能因为新的AI系统被替代。
 * */
	public class ActionWalkTo2 : ActionSingle
	{
		public GameObject obj;
		public Landmark destination;
		private IActionCompleted monitor;

		private Animator animator;
		private NavMeshAgent agent;
		//private NavMeshObstacle obstacle;

		private const float NAVMESHSAMPLEDISTANCE = 4f;
		private const float STOPDISTANCEPROPOTION = 0.05f;
		private const float TURNSPEEDTHRESHOLD = 0.1f;
		private const float SPEEDDAMPTIME = 0.5f;
		private const float SLOWINGSPEED = 0.05f;
		private const float TURNSMOOTHING = 15f;
		private const float ANIMATORSPEEDPROPOTION = 1.4f;

		private float speed;

		private readonly int hashSpeedPara = Animator.StringToHash ("Speed");

		public void Setting (GameObject obj, Landmark destination, IActionCompleted monitor)
		{
			if (this.obj == null) {
				//this.id = ActionID.WALKTO;
				this.obj = obj;
				this.destination = destination;
				this.monitor = monitor;
				animator = obj.GetComponent<Animator> ();
				agent = obj.GetComponent<NavMeshAgent> ();
				//obstacle = obj.GetComponent<NavMeshObstacle> ();
				Begin ();
			} else {
				this.destination = destination;
				this.monitor = monitor;
				Begin ();
			}
		}

		void Begin ()
		{
			//obstacle.enabled = false;
			//agent.enabled = true;
//		NavMeshHit hit;
//		if (NavMesh.SamplePosition (destination.position, out hit, NAVMESHSAMPLEDISTANCE, NavMesh.AllAreas)) {
//			destination.px = hit.position.x;
//			destination.py = hit.position.y;
//			destination.pz = hit.position.z;
//		}
			agent.updateRotation = false;
			agent.ResetPath ();
			agent.SetDestination (destination.position);
			agent.isStopped = false;
		}

		void Update ()
		{
			if (!agent.enabled || agent.pathPending)
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
			float ySpeed = Mathf.Abs (agent.velocity.y);
			ySpeed = ySpeed > 0 ? 1 + ySpeed : 1;
			animator.speed = speed * ANIMATORSPEEDPROPOTION * ySpeed;

			if (transform.position == destination.position) {
				Finish ();
			}
		}

		void Stopping (out float speed)
		{
			agent.isStopped = true;
			transform.position = destination.position;
			speed = 0f;
		}

		void Slowing (out float speed, float distanceToDestination)
		{
			agent.isStopped = true;
			float propotionDistance = 1f - distanceToDestination / agent.stoppingDistance;
			Quaternion targetRotation = destination.rotation;
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, propotionDistance);
			transform.position = Vector3.MoveTowards (transform.position, destination.position, SLOWINGSPEED * Time.deltaTime);
			speed = Mathf.Lerp (SLOWINGSPEED, 0f, propotionDistance);
		}

		void Moving ()
		{
			Quaternion targetRotation = Quaternion.LookRotation (agent.desiredVelocity);
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, TURNSMOOTHING * Time.deltaTime);
		}

		public void Finish ()
		{
			animator.speed = 1;
			animator.SetFloat (hashSpeedPara, 0);
			agent.ResetPath ();
			//agent.enabled = false;
			//obstacle.enabled = true;
			if (monitor != null) {
				monitor.OnActionCompleted (this);
			}
			Free ();
		}
	}
}