using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : Object
{
	private static ActionManager instance;

	public static ActionManager GetInstance ()
	{
		if (instance == null) {
			instance = new ActionManager ();
		}
		return instance;
	}

	public void Reset (GameObject obj)
	{
		Action[] actions = obj.GetComponents<Action> ();
		foreach (Action ac in actions) {
			ac.Free ();
		}
	}


	// Movement
	public Action ApplyWalkToAction (GameObject obj, Vector3 destinationPosition, bool finalRotate, Quaternion destinationRotation, IActionCompleted callback)
	{
		ActionWalkTo ac = obj.GetComponent<ActionWalkTo> ();
		if (ac == null)
			ac = obj.AddComponent<ActionWalkTo> ();
		ac.Setting (obj, destinationPosition, finalRotate, destinationRotation, callback);
		return ac;
	}


	// ChatBubble
	public Action ApplyChatAction (GameObject obj, string content, float duration, IActionCompleted callback)
	{
		GameObject bubble = obj.transform.Find ("hip_ctrl").transform.Find ("Bubble").gameObject;
		if (bubble == null)
			return null;
		ActionChatBubble ac = bubble.GetComponent<ActionChatBubble> ();
		if (ac == null)
			ac = bubble.AddComponent<ActionChatBubble> ();
		ac.Setting (obj, bubble, content, duration, callback);
		return ac;
	}


	// Animation
	public Action ApplySitDownAction (GameObject obj, IActionCompleted callback)
	{
		obj.GetComponent<Person> ().isStanding = false;
		ActionSitDown ac = obj.AddComponent<ActionSitDown> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyStandUpAction (GameObject obj, IActionCompleted callback)
	{
		obj.GetComponent<Person> ().isStanding = true;
		ActionStandUp ac = obj.AddComponent<ActionStandUp> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyClickAction (GameObject obj, IActionCompleted callback)
	{
		ActionClick ac = obj.AddComponent<ActionClick> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyTypeAction (GameObject obj, IActionCompleted callback)
	{
		ActionType ac = obj.AddComponent<ActionType> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyScratchHeadAction (GameObject obj, IActionCompleted callback)
	{
		ActionScratchHead ac = obj.AddComponent<ActionScratchHead> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyHandOnChinAction (GameObject obj, IActionCompleted callback)
	{
		ActionHandOnChin ac = obj.AddComponent<ActionHandOnChin> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyUseCameraAction (GameObject obj, IActionCompleted callback)
	{
		ActionUseCamera ac = obj.AddComponent<ActionUseCamera> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplySpeechAction (GameObject obj, IActionCompleted callback)
	{
		ActionSpeech ac = obj.AddComponent<ActionSpeech> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyPickUpTelephoneAction (GameObject obj, IActionCompleted callback)
	{
		ActionPickUpTelephone ac = obj.AddComponent<ActionPickUpTelephone> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyPutDownTelephoneAction (GameObject obj, IActionCompleted callback)
	{
		ActionPutDownTelephone ac = obj.AddComponent<ActionPutDownTelephone> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyDialTelephoneAction (GameObject obj, IActionCompleted callback)
	{
		ActionDialTelephone ac = obj.AddComponent<ActionDialTelephone> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyUseTelephoneAction (GameObject obj, IActionCompleted callback)
	{
		ActionUseTelephone ac = obj.AddComponent<ActionUseTelephone> ();
		ac.Setting (obj, callback);
		return ac;
	}



















	public bool IsSitAction (string actionName)
	{
		if (actionName == "起立" || actionName == "敲击键盘" || actionName == "移动鼠标" || actionName == "坐姿挠头")
			return true;
		return false;
	}

	public bool IsStandAction (string actionName)
	{
		if (actionName == "坐下")
			return true;
		return false;
	}


	public Action ApplyAction (string actionName, GameObject obj, IActionCompleted monitor)
	{
		switch (actionName) {
		case "坐下":
			obj.GetComponent<Person> ().isStanding = false;
			return ApplySitDownAction (obj, monitor);
		case "起立":
			obj.GetComponent<Person> ().isStanding = true;
			return ApplyStandUpAction (obj, monitor);
		case "敲击键盘":
			return ApplyTypeAction (obj, monitor);
		case "移动鼠标":
			return ApplyClickAction (obj, monitor);
		case "坐姿挠头":
			return ApplyScratchHeadAction (obj, monitor);
		default:
			return null;
		}
	}
}
