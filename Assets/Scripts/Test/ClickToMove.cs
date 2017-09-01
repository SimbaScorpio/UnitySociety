using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickToMove : MonoBehaviour
{
	public Transform target;

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000)) {
				if (!GetComponent<ActionSingle> ()) {
					target.position = hit.point;
					//AIMove ac = gameObject.AddComponent<AIMove> ();
					//ac.Setting (gameObject, target, null);
				}
			}
		}
	}
}
