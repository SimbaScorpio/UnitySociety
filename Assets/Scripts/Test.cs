using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
	private Animator anim;
	private NavMeshAgent agent;
	private Vector2 smoothDeltaPosition = Vector2.zero;
	private Vector2 velocity = Vector2.zero;


	void Start()
	{
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		agent.updatePosition = false;
	}


	void Update()
	{
		Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

		float dx = Vector3.Dot(transform.right, worldDeltaPosition);
		float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
		Vector2 deltaPosition = new Vector2(dx, dy);

		float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
		smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

		if (Time.deltaTime > 1e-5)
			velocity = smoothDeltaPosition / Time.deltaTime;

		bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

		anim.SetBool("IsWalking", shouldMove);

		//agent.nextPosition = transform.position + worldDeltaPosition;
		//transform.position = agent.nextPosition - worldDeltaPosition;
		if (worldDeltaPosition.magnitude > agent.radius)
			agent.nextPosition = transform.position + 0.9f * worldDeltaPosition;
			
		RaycastHit hit;
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				agent.destination = hit.point;
			}
		}
	}


	void OnAnimatorMove()
	{
		Vector3 position = anim.rootPosition;
		position.y = agent.nextPosition.y;
		transform.position = position;
	}
}
