using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DesignSociety;

public class ClickToMove : MonoBehaviour
{
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
				if (!GetComponent<ActionWalkTo> ()) {
					Landmark mark = new Landmark ();
					mark.data [0] = hit.point.x;
					mark.data [1] = hit.point.y;
					mark.data [2] = hit.point.z;
					ActionWalkTo ac = gameObject.AddComponent<ActionWalkTo> ();
					ac.Setting (gameObject, mark, null);
				}
			}
		}
	}
}
