using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
	NavMeshAgent agent;
	NavMeshObstacle obstacle;
	bool flag = false;

	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();
		obstacle = GetComponent<NavMeshObstacle> ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.A)) {
			if (!flag)
				ImObstacle ();
			else
				ImAgent ();
		}
	}

	void ImAgent ()
	{
		obstacle.enabled = false;
		agent.enabled = true;
		flag = false;
	}

	void ImObstacle ()
	{
		agent.enabled = false;
		obstacle.enabled = true;
		flag = true;
	}
}
