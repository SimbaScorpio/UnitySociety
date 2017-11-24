using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignSociety;

public class ClientMessageTest : MonoBehaviour
{
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.M)) {
			UIClientMessageKeywordBlob.GetInstance ().PushBlob ("交互设计");
		}
		if (Input.GetKeyDown (KeyCode.Z)) {
			UIInformationMenu.GetInstance ().Show ();
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			UIInformationMenu.GetInstance ().Hide ();
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
