using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
	NavMeshAgent agent;
	public GameObject a;
	public GameObject b;
	float error = 2f;

	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();
		agent.destination = a.transform.position;
	}

	void Update ()
	{
		if (Vector3.Distance (transform.position, a.transform.position) < error) {
			agent.destination = b.transform.position;
		} else if (Vector3.Distance (transform.position, b.transform.position) < error) {
			agent.destination = a.transform.position;
		}
	}
}
