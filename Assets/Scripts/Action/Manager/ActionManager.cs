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
	public Action ApplyWalkToAction (GameObject obj, Landmark destinationPosition, IActionCompleted callback)
	{
		ActionWalkTo ac = obj.GetComponent<ActionWalkTo> ();
		if (ac == null)
			ac = obj.AddComponent<ActionWalkTo> ();
		ac.Setting (obj, destinationPosition, callback);
		return ac;
	}


	// ChatBubble
	public Action ApplyChatAction (GameObject obj, string content, float duration, IActionCompleted callback)
	{
		GameObject bubble = obj.transform.Find ("Bubble").gameObject;
		if (bubble == null)
			return null;
		ActionChatBubble ac = bubble.GetComponent<ActionChatBubble> ();
		if (ac == null)
			ac = bubble.AddComponent<ActionChatBubble> ();
		ac.Setting (obj, bubble, content, duration, callback);
		return ac;
	}


	// Animation
	public Action ApplyClickAction (GameObject obj, string stateName, IActionCompleted callback)
	{
		ActionClick ac = obj.AddComponent<ActionClick> ();
		ac.Setting (obj, stateName, callback);
		return ac;
	}

	public Action ApplySpeechAction (GameObject obj, string stateName, IActionCompleted callback)
	{
		ActionSpeech ac = obj.AddComponent<ActionSpeech> ();
		ac.Setting (obj, stateName, callback);
		return ac;
	}

	public Action ApplyUseCameraAction (GameObject obj, string stateName, IActionCompleted callback)
	{
		ActionUseCamera ac = obj.AddComponent<ActionUseCamera> ();
		ac.Setting (obj, stateName, callback);
		return ac;
	}

	public Action ApplyPickUpTelephoneAction (GameObject obj, string stateName, IActionCompleted callback)
	{
		ActionPickUpTelephone ac = obj.AddComponent<ActionPickUpTelephone> ();
		ac.Setting (obj, stateName, callback);
		return ac;
	}

	public Action ApplyPutDownTelephoneAction (GameObject obj, string stateName, IActionCompleted callback)
	{
		ActionPutDownTelephone ac = obj.AddComponent<ActionPutDownTelephone> ();
		ac.Setting (obj, stateName, callback);
		return ac;
	}

	public Action ApplyIdleAction (GameObject obj, string stateName, IActionCompleted callback)
	{
		ActionIdle ac = obj.AddComponent<ActionIdle> ();
		ac.Setting (obj, stateName, callback);
		return ac;
	}

	public Action ApplyGivePaperAction (GameObject obj, string stateName, IActionCompleted callback)
	{
		ActionGivePaper ac = obj.AddComponent<ActionGivePaper> ();
		ac.Setting (obj, stateName, callback);
		return ac;
	}

	public Action ApplyGetPaperAction (GameObject obj, string stateName, IActionCompleted callback)
	{
		ActionGetPaper ac = obj.AddComponent<ActionGetPaper> ();
		ac.Setting (obj, stateName, callback);
		return ac;
	}

	public Action ApplyTriggerAction (GameObject obj, string stateName, IActionCompleted callback)
	{
		ActionTrigger ac = obj.AddComponent<ActionTrigger> ();
		ac.Setting (obj, stateName, callback);
		return ac;
	}
}
