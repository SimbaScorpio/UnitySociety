using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailTest : MonoBehaviour
{
	public List<GameObject> receivers = new List<GameObject> ();

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.M)) {
			print (1);
			MailManager.GetInstance ().SendMail (this.gameObject, receivers, 2.0f);
		}
	}
}
