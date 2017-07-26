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
	public Action ApplyWalkToAction (GameObject obj, Vector3 destinationPosition, Quaternion destinationRotation, ActionCompleted callback)
	{
		ActionWalkTo ac = obj.AddComponent<ActionWalkTo> ();
		ac.Setting (obj, destinationPosition, true, destinationRotation, callback);
		return ac;
	}

	public Action ApplyWalkToAction (GameObject obj, Vector3 destinationPosition, ActionCompleted callback)
	{
		ActionWalkTo ac = obj.AddComponent<ActionWalkTo> ();
		ac.Setting (obj, destinationPosition, false, Quaternion.identity, callback);
		return ac;
	}


	// ChatBubble
	public Action ApplyChatAction (GameObject obj, string content, float duration, ActionCompleted callback)
	{
		obj = obj.transform.Find ("hip_ctrl").transform.Find ("Bubble").gameObject;
		if (!obj)
			return null;
		ActionChat ac = obj.GetComponent<ActionChat> ();
		if (ac == null)
			ac = obj.AddComponent<ActionChat> ();
		ac.Setting (obj, content, duration, callback);
		return ac;
	}


	// Animation
	public Action ApplySitDownAction (GameObject obj, ActionCompleted callback)
	{
		ActionSitDown ac = obj.AddComponent<ActionSitDown> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyStandUpAction (GameObject obj, ActionCompleted callback)
	{
		ActionStandUp ac = obj.AddComponent<ActionStandUp> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplySimpleClickAction (GameObject obj, ActionCompleted callback)
	{
		ActionSimpleClick ac = obj.AddComponent<ActionSimpleClick> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplySimpleTypeAction (GameObject obj, ActionCompleted callback)
	{
		ActionSimpleType ac = obj.AddComponent<ActionSimpleType> ();
		ac.Setting (obj, callback);
		return ac;
	}

	public Action ApplyScratchHeadAction (GameObject obj, ActionCompleted callback)
	{
		ActionScratchHead ac = obj.AddComponent<ActionScratchHead> ();
		ac.Setting (obj, callback);
		return ac;
	}
}


public interface ActionCompleted
{
	void OnActionCompleted (Action action);
}


public class Action : MonoBehaviour
{
	public ActionID ID;

	public void Free ()
	{
		Destroy (this);	// 卸载脚本
	}
}


public class ActionSingle : Action
{
	// 独立动作
}


public class ActionMultiple : Action
{
	// 复合动作
}


public class ActionThread : Action
{
	// 同存动作
}


public enum ActionID
{
	IDLE,
	WALKTO,
	CHAT,
	SITDOWN,
	STANDUP,
	SIMPLECLICK,
	SIMPLETYPE,
	SCRATCHHEAD
}