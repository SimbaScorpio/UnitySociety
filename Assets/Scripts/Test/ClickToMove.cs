using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using DesignSociety;

public class ClickToMove : NetworkBehaviour
{
	public bool isLocal = true;

	void Update ()
	{
		if (isLocal && (!isServer && !isLocalPlayer))
			return;
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
				GetComponent<NetworkActionDealer> ().ApplyWalkAction (mark, false, null);
			}
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			ActionSingle single = GetComponent<ActionSingle> ();
			if (single != null) {
				single.Free ();
			}
			GetComponent<NetworkActionDealer> ().ApplyAction ("stand_camera", null);
		}
	}
}
