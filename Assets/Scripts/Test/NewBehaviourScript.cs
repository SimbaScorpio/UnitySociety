using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
	List<int> list = new List<int> ();

	void Start ()
	{
		for (int i = 0; i < 10; ++i) {
			list.Add (1);
		}
		StartCoroutine (Wait ());
	}

	IEnumerator Wait ()
	{
		while (list.Count > 0) {
			print (list.Count);
			list.RemoveAt (0);
			yield return new WaitForSeconds (1);
		}
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.A)) {
			list.Add (1);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			list.RemoveAt (0);
		}
		if (Input.GetKeyDown (KeyCode.Q)) {
			StopAllCoroutines ();
		}
	}
}
