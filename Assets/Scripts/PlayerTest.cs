using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
	private Animator anim;

	// Use this for initialization
	void Awake()
	{
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update()
	{
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
	}
}
