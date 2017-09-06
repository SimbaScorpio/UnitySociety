using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTest : MonoBehaviour
{
	public GameObject stuff;
	public GameObject parent;
	public Transform tr;

	public void OnStuffPicked ()
	{
		stuff.transform.SetParent (parent.transform);
		stuff.transform.position = tr.position;
		stuff.transform.rotation = tr.rotation;
		stuff.GetComponent<Rigidbody> ().isKinematic = true;
	}

	public void OnStuffPut ()
	{
		stuff.transform.SetParent (null);
		stuff.GetComponent<Rigidbody> ().isKinematic = false;
	}

	public void OnStuffDisappear ()
	{
		stuff.SetActive (false);
	}

	public void OnStuffAppear ()
	{
		stuff.SetActive (true);
	}
}
