using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityAreaTrigger : MonoBehaviour
{
	private Facility parent;


	void Start()
	{
		parent = transform.parent.GetComponent<Facility>();
	}


	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Player")
		{
			parent.OnChildTriggerEnter(collider);
		}
	}


	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Player")
		{
			parent.OnChildTriggerExit(collider);
		}
	}
}
