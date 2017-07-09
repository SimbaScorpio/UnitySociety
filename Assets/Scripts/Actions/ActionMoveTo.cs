using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionMoveTo : ActionAuto, ActionCompleted
{
	public GameObject obj;
	public Vector3 position;

	private ActionCompleted monitor = null;

	private Person person;
	private NavMeshAgent agent;
	//private NavMeshObstacle obstacle;
	private Animator animator;

	//private List<GameObject> colliders = new List<GameObject>();

	private float blockLimitTime = 1.0f;
	private float blockCounting = 0.0f;

	private float avoidLimitTime = 1.0f;
	private float avoidCounting = 0.0f;

	// 防坑变量，若Avoid和MoveTo的Update函数同时运行，Avoid和MoveTo同时判断出了终点结束，而MoveTo可能在Finish后收到Avoid的回调函数，从而导致行走动画持续播放。
	// 因此，MoveTo不能在Avoid脚本存在时执行Update，该变量就是用来控制的。
	private bool isBegin = false;


	public void setting(GameObject obj, Vector3 position, ActionID type, ActionCompleted callback)
	{
		this.obj = obj;
		this.position = position;
		this.ID = type;
		this.monitor = callback;

		person = obj.GetComponent<Person>();
		agent = obj.GetComponent<NavMeshAgent>();
		//obstacle = obj.GetComponent<NavMeshObstacle>();
		animator = obj.GetComponent<Animator>();

		this.Begin(position);
	}


	void Begin(Vector3 position)
	{
		isBegin = true;
		agent.SetDestination(position);
		animator.SetBool("IsWalking", true);
		if (this.ID == ActionID.MOVETO)
		{
			agent.updateRotation = true;
			animator.SetFloat("WalkSpeed", 1);
		}
		if (this.ID == ActionID.AVOID)
		{
			agent.updateRotation = false;
			animator.SetFloat("WalkSpeed", -1);
		}
	}


	void Finish()
	{
		//obstacle.enabled = false;
		//agent.enabled = true;
		agent.ResetPath();
		agent.updateRotation = true;
		animator.SetBool("IsWalking", false);
		if (monitor != null)
		{
			monitor.OnActionCompleted(this);
		}
		Destroy(this);
	}


	void Update()
	{
		if (agent.enabled && isBegin)
		{
			if (this.ID == ActionID.AVOID)	// 作为回避移动多1个时间停止条件
			{
				avoidCounting += Time.deltaTime;
				if (avoidCounting >= avoidLimitTime)
				{
					this.Finish();
				}
				this.RotateTowards(position);
			}
			if (!agent.pathPending)
			{
				if (agent.remainingDistance <= agent.stoppingDistance)
				{
					if (!agent.hasPath || agent.velocity.sqrMagnitude == 0)
					{
						this.Finish();
					}
				}
			}
			this.BlazeATrail();
		}
	}


	void RotateTowards(Vector3 position)
	{
		float angle = obj.transform.rotation.eulerAngles.y;
		float angle2 = angle / 180 * Mathf.PI;
		Vector3 curDir = new Vector3(Mathf.Sin(angle2), 0, Mathf.Cos(angle2)).normalized;
		Vector3 futDir = obj.transform.position - position;

		curDir.y = futDir.y = 0;

		curDir = Vector3.RotateTowards(curDir, futDir, 3 * Time.deltaTime, float.PositiveInfinity);
		obj.transform.LookAt(obj.transform.position + curDir);
	}


	void BlazeATrail()
	{
		Status status = person.FacilityInfo.Status;
		if (status != Status.QUEUING)	// 排队时不需要对方让道
		{
			Ray ray = new Ray(obj.transform.position, agent.velocity);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 2))
			{
				if (hit.collider.tag == "Player")
				{
					// 如果对方已经在避让，就不为难他了
					ActionMoveTo[] movetos = hit.collider.GetComponents<ActionMoveTo>();
					foreach (ActionMoveTo ac in movetos)
					{
						if (ac.ID == ActionID.AVOID)
						{
							return;
						}
					}
		
					Person person2 = hit.collider.GetComponent<Person>();
					Status status2 = person2.FacilityInfo.Status;
					if (status > status2)	// 优先级比对方大
					{
						Vector3 avoidPos = this.CalculateAvoidance(hit.collider.gameObject);
						ActionMoveTo ac = hit.collider.GetComponent<ActionMoveTo>();
						if (ac)
						{
							ac.isBegin = false;
							ActionManager.GetInstance().ApplyMoveToAction(hit.collider.gameObject, avoidPos, ActionID.AVOID, ac);
						}
						else
							ActionManager.GetInstance().ApplyMoveToAction(hit.collider.gameObject, avoidPos, ActionID.AVOID, null);
					}
				}
			}
		}
	}


	Vector3 CalculateAvoidance(GameObject other)
	{
		Vector3 otherPos = other.transform.position;
		Vector3 direction = otherPos - obj.transform.position;
		Vector3 avoidPos = otherPos + direction.normalized;
		return avoidPos;
	}


	public void OnActionCompleted(Action ac)
	{
		if (ac.ID == ActionID.AVOID)
		{
			this.Begin(position);
		}
	}


	void OnTriggerStay(Collider collider)
	{
		if (collider.tag == "Player")
		{
			Facility facility = person.FacilityInfo.FacilityScript;
			if (facility != null)
			{
				if (person.FacilityInfo.Status == Status.ENTERINGAREA || person.FacilityInfo.Status == Status.ENTEREDAREA)
				{
					Person person2 = collider.GetComponent<Person>();
					Facility facility2 = person2.FacilityInfo.FacilityScript;
					if (facility2 != null)
					{
						if (person2.FacilityInfo.Status == Status.QUEUING || person2.FacilityInfo.Status == Status.USING)
						{
							if (facility.gameObject == facility2.gameObject)
							{
								this.Finish();
							}
						}
					}
				}
			}
			this.AvoidBlocking();
		}
	}


	void AvoidBlocking()
	{
		if (agent.velocity.magnitude < 0.5f)
		{
			blockCounting += Time.deltaTime;
			if (blockCounting >= blockLimitTime)
			{
				if (person.FacilityInfo.Status == Status.ENTEREDAREA)
					this.Finish();
				else
					agent.nextPosition = transform.position + agent.velocity.normalized * agent.radius * 2;
			}
		}
		else
		{
			blockCounting = 0.0f;
		}
	}



	/*
	void OnTriggerStay(Collider collider)
	{
		GameObject other = collider.gameObject;
		if (other.tag == "Player")
		{
			if (person.FacilityInfo.Status != Status.USING && person.FacilityInfo.Status != Status.LEAVINGAREA)
			{
				Person otherP = other.GetComponent<Person>();
				Facility myFacility = person.FacilityInfo.FacilityScript;
				Facility cdFacility = otherP.FacilityInfo.FacilityScript;
				if (myFacility != null && cdFacility != null && myFacility.gameObject == cdFacility.gameObject)
				{
					if (otherP.FacilityInfo.Status == Status.QUEUING)
					{
						this.Finish();
					}
					else
					{
						if (!colliders.Contains(other))
						{
							int id1 = obj.GetInstanceID();
							int id2 = other.GetInstanceID();
							if (id1 < id2)
							{
								colliders.Add(other);
								animator.SetBool("IsWalking", false);
								agent.enabled = false;
								obstacle.enabled = true;
							}
						}
					}
				}
			}
			else
			{
				obstacle.enabled = false;
				agent.enabled = true;
				this.Begin(position);
			}
			if (agent.velocity.magnitude < 0.1f)
			{
				thresholdCounting += Time.deltaTime;
				if (thresholdCounting >= blockThreshold)
				{
					if (person.FacilityInfo.Status == Status.ENTEREDAREA)
					{
						this.Finish();
					}
					else
					{
						agent.nextPosition = transform.position + agent.velocity.normalized * agent.radius * 2;
					}
				}
			}
			else
			{
				thresholdCounting = 0.0f;
			}
		}
	}


	void OnTriggerExit(Collider collider)
	{
		GameObject other = collider.gameObject;
		if (other.tag == "Player")
		{
			if (colliders.Contains(other))
			{
				colliders.Remove(other);
				if (colliders.Count == 0)
				{
					obstacle.enabled = false;
					agent.enabled = true;
					this.Begin(position);
				}
			}
		}
	}
	*/
}
