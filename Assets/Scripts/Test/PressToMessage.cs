using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignSociety;

public class PressToMessage : MonoBehaviour
{
	public string message;
	public int maxCharacter;

	private NetworkBubbleDealer bd;

	void GenerateMessage ()
	{
		message = "";
		int num = Random.Range (1, maxCharacter);
		for (int i = 0; i < num; ++i)
			message += "烫";
	}

	void Start ()
	{
		bd = GetComponent<NetworkBubbleDealer> ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Z)) {
			GenerateMessage ();
			bd.ApplyChatBubble (message, 3, Random.Range (0, 5), null);
		} else if (Input.GetKeyDown (KeyCode.X)) {
			GenerateMessage ();
			bd.ApplyErrorBubble (message, 3);
		} else if (Input.GetKeyDown (KeyCode.V)) {
			bd.ApplyIconBubble ("办公&日常_事项", 5, null);
		} else if (Input.GetKeyDown (KeyCode.C)) {
			bd.ApplyScreenBubble ("办公&日常_事项", 5, Random.Range (0, 5), null);
		}
	}
}
