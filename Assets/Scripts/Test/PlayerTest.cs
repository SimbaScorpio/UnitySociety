using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTest : MonoBehaviour
{
	public Text text;

	//private Animator anim;

	// Use this for initialization
	void Awake ()
	{
		//anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray.origin, ray.direction, out hit)) {
				print (hit.point);
				ActionManager.GetInstance ().ApplyWalkToAction (this.gameObject, hit.point, null);
				//ActionManager.GetInstance ().ApplyMoveToAction (this.gameObject, hit.point, ActionID.MOVETO, null);
			}
		}

		/*
		if (Input.GetKey (KeyCode.W)) {
			anim.SetBool ("IsWalking", true);
		} else {
			anim.SetBool ("IsWalking", false);
		}
		if (Input.GetKey (KeyCode.Q)) {
			anim.SetBool ("IsStairing", true);
		} else {
			anim.SetBool ("IsStairing", false);
		}
		if (Input.GetKey (KeyCode.S)) {
			anim.SetBool ("IsSitting", true);
		} else {
			anim.SetBool ("IsSitting", false);
		}
	*/

	}

	/*
	public void OnIdle ()
	{
		print ("Idle");
		text.text = "Idle";
	}

	public void OnWalk ()
	{
		print ("Walk");
		if (anim.GetBool ("IsWalking"))
			text.text = "Walk";
	}

	public void OnUpStairs ()
	{
		print ("UpStairs");
		if (anim.GetBool ("IsStairing"))
			text.text = "Up Stairs";
	}

	public void OnSitDown ()
	{
		print ("SitDown");
		text.text = "Sit Down";
	}

	public void OnStandUp ()
	{
		print ("StandUp");
		text.text = "Stand Up";
	}

	public void OnTypeAndClick ()
	{
		print ("TypeAndClick");
		text.text = "Type And Click";
	}

	public void OnScratchHead ()
	{
		print ("ScratchHead");
		text.text = "Scratch Head";
	}

	public void OnWorry ()
	{
		print ("Worry");
		text.text = "Worry";
	}
	*/
}
