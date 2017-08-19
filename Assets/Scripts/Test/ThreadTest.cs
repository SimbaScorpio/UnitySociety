using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadTest : MonoBehaviour
{
	string index = "1";

	void Start ()
	{
		StartCoroutine (Test (index));
		index += "2";
	}

	IEnumerator Test (string i)
	{
		yield return new WaitForSeconds (2);
		i += "aaa";
		print (i);
		print (index);
	}
}
