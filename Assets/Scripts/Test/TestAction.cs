using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignSociety;

public class TestAction : MonoBehaviour, IActionCompleted
{

	NetworkActionPlay player;
	// Use this for initialization
	void Start ()
	{
		player = GetComponent<NetworkActionPlay> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.M)) {
			player.Setting (new ActionInfo ("stand_nod", null, null, StuffType.BigStuff), this);
		}
	}

	public void OnActionCompleted (Action ac)
	{
		print ("finish");
		player.Setting (new ActionInfo ("stand_left30", null, null, StuffType.BigStuff), this);
	}
}
