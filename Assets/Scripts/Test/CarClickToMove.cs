using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using DesignSociety;

public class CarClickToMove : MonoBehaviour
{
	private MyRichAI ai;

	void Start ()
	{
		ai = GetComponent<MyRichAI> ();
		ai.OnTargetReached += OnTargetReached;
	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			CameraPerspectiveEditor editor = Camera.main.GetComponent<CameraPerspectiveEditor> ();
			Ray ray;
			if (editor != null && editor.isActiveAndEnabled)
				ray = editor.ScreenPointToRay (Input.mousePosition);
			else
				ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000)) {
				ActionSingle single = GetComponent<ActionSingle> ();
				if (single != null) {
					single.Free ();
				} 
				Landmark mark = new Landmark ();
				mark.m_data [0] = hit.point.x;
				mark.m_data [1] = hit.point.y;
				mark.m_data [2] = hit.point.z;
				ai.target = mark;
				ai.enabled = true;
			}
		}
	}

	void OnTargetReached (object sender, EventArgs e)
	{
		print ("reach");
		ai.enabled = false;
	}
}
