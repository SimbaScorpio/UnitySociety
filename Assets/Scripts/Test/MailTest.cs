using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailTest : MonoBehaviour
{
	public List<GameObject> receivers = new List<GameObject> ();
	string i = "好";

	void Start ()
	{
		
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.M)) {
			ActionManager.GetInstance ().ApplyChatAction (gameObject, "你好" + i, 5, null);
			i += "好";
		}
	}
}
