using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerTest : MonoBehaviour
{
	//private Animator anim;
	//private NavMeshAgent agent;

	// Use this for initialization
	void Awake ()
	{
		//anim = GetComponent<Animator> ();
		//agent = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray.origin, ray.direction, out hit)) {
				print (hit.point);
				ActionManager.GetInstance ().ApplyMoveToAction (this.gameObject, hit.point, ActionID.MOVETO, null);
			}
		}
		/*
		if (Input.GetKey(KeyCode.W))
		{
			anim.SetBool("IsWalking", true);
		}
		else
		{
			anim.SetBool("IsWalking", false);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			anim.SetTrigger("PlayVR");
		}
		if (Input.GetKey(KeyCode.S))
		{
			anim.SetBool("IsSiting", true);
		}
		else
		{
			anim.SetBool("IsSiting", false);
		}
		*/
	}
}
