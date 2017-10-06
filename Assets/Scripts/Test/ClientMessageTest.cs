using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignSociety;

public class ClientMessageTest : MonoBehaviour
{
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			UIBubbleMessage.GetInstance ().PushMessage (Random.Range (0, 2), RandomText ("1"), RandomText ("2"));
		}
	}

	string RandomText (string letter)
	{
		int num = Random.Range (1, 100);
		string s = "";
		while (num-- > 0) {
			s += letter;
		}
		return s;
	}
}
