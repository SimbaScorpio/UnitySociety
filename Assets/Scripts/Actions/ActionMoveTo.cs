using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionMoveTo : ActionSingle, ActionCompleted
{
	public GameObject obj;
	public Vector3 targetPosition;
	private ActionCompleted monitor = null;

	private Person person;
	private NavMeshAgent agent;
	private Animator animator;

	private float blockLimitTime = 1.0f;
	private float blockCounting = 0.0f;

	private float avoidLimitTime = 1.0f;
	private float avoidCounting = 0.0f;

	public float WALKINGSPEED = 1.0f;
	public float TURNINGSPEED = 3.0f;
	public float RAYHITLENGTH = 2.0f;

	// 防坑变量，若Avoid和MoveTo的Update函数同时运行，Avoid和MoveTo同时判断出了终点结束，而MoveTo可能在Finish后收到Avoid的回调函数，从而导致行走动画持续播放。
	private bool isBegin = false;


	public void Setting (GameObject obj, Vector3 targetPosition, ActionID type, ActionCompleted callback)
	{
		this.obj = obj;
		this.targetPosition = targetPosition;
		this.ID = type;
		this.monitor = callback;

		person = obj.GetComponent<Person> ();
		agent = obj.GetComponent<NavMeshAgent> ();
		animator = obj.GetComponent<Animator> ();

		Check ();
		Begin (targetPosition);
	}


	void Begin (Vector3 targetPosition)
	{
		//person.Action = this;
		isBegin = true;

		agent.SetDestination (targetPosition);
		animator.SetBool ("IsWalking", true);

		if (this.ID == ActionID.MOVETO) {
			agent.updateRotation = true;
			animator.SetFloat ("WalkSpeed", WALKINGSPEED);
		}
		if (this.ID == ActionID.AVOID) {
			agent.updateRotation = false;
			animator.SetFloat ("WalkSpeed", -WALKINGSPEED);
		}
	}


	void Finish ()
	{
		person.Action = null;
		agent.ResetPath ();
		agent.updateRotation = true;
		animator.SetBool ("IsWalking", false);
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Destroy (this);
	}


	void Update ()
	{
		if (agent.enabled && isBegin) {
			if (this.ID == ActionID.AVOID) {	// 作为回避移动多1个时间停止条件
				avoidCounting += Time.deltaTime;
				if (avoidCounting >= avoidLimitTime) {
					Finish ();
				}
				RotateTowards (targetPosition);
			}
			if (!agent.pathPending) {
				if (agent.remainingDistance <= agent.stoppingDistance) {
					if (!agent.hasPath || agent.velocity.sqrMagnitude == 0) {
						Finish ();
					}
				}
			}
			BlazeATrail ();
		}
	}


	void RotateTowards (Vector3 position)
	{
		float angle = obj.transform.rotation.eulerAngles.y;
		float anglePI = angle / 180 * Mathf.PI;
		Vector3 curDir = new Vector3 (Mathf.Sin (anglePI), 0, Mathf.Cos (anglePI)).normalized;
		Vector3 futDir = obj.transform.position - position;	// 注意这里是背对目标方向

		curDir.y = futDir.y = 0;

		curDir = Vector3.RotateTowards (curDir, futDir, TURNINGSPEED * Time.deltaTime, float.PositiveInfinity);
		obj.transform.LookAt (obj.transform.position + curDir);
	}


	void BlazeATrail ()
	{
		Status status = person.FacilityInfo.Status;
		if (person.Action && person.Action.ID != ActionID.AVOID && status != Status.QUEUING) {
			Ray ray = new Ray (obj.transform.position, agent.velocity);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, RAYHITLENGTH)) {
				if (hit.collider.tag == "Player") {
					Person person2 = hit.collider.GetComponent<Person> ();
					if (!person2.Action || person2.Action && person2.Action.ID != ActionID.AVOID) {
						Status status2 = person2.FacilityInfo.Status;
						if (status > status2) {	// 优先级比对方大
							Vector3 avoidPos = CalculateAvoidance (hit.collider.gameObject);
							ActionMoveTo ac = hit.collider.GetComponent<ActionMoveTo> ();
							if (ac) {
								ac.isBegin = false;
								ActionManager.GetInstance ().ApplyMoveToAction (hit.collider.gameObject, avoidPos, ActionID.AVOID, ac);
							} else
								ActionManager.GetInstance ().ApplyMoveToAction (hit.collider.gameObject, avoidPos, ActionID.AVOID, null);
						}
					}
				}
			}
		}
	}


	Vector3 CalculateAvoidance (GameObject other)
	{
		Vector3 otherPos = other.transform.position;
		Vector3 direction = otherPos - obj.transform.position;
		Vector3 avoidPos = otherPos + direction.normalized;
		return avoidPos;
	}


	public void OnActionCompleted (Action ac)
	{
		if (ac.ID == ActionID.AVOID) {
			Begin (targetPosition);
		}
	}


	void OnTriggerStay (Collider collider)
	{
		if (collider.tag == "Player") {
			Facility facility = person.FacilityInfo.FacilityScript;
			if (facility != null) {
				if (person.FacilityInfo.Status == Status.ENTERINGAREA || person.FacilityInfo.Status == Status.ENTEREDAREA) {
					Person person2 = collider.GetComponent<Person> ();
					Facility facility2 = person2.FacilityInfo.FacilityScript;
					if (facility2 != null) {
						if (person2.FacilityInfo.Status == Status.QUEUING || person2.FacilityInfo.Status == Status.USING) {
							if (facility.gameObject == facility2.gameObject) {
								Finish ();
							}
						}
					}
				}
			}
			AvoidBlocking ();
		}
	}


	void AvoidBlocking ()
	{
		if (agent.velocity.magnitude < 0.2f) {
			blockCounting += Time.deltaTime;
			if (blockCounting >= blockLimitTime) {
				if (person.FacilityInfo.Status == Status.ENTEREDAREA)
					Finish ();
				else
					agent.nextPosition = transform.position + agent.velocity.normalized * agent.radius * 2;
			}
		} else {
			blockCounting = 0.0f;
		}
	}
}
