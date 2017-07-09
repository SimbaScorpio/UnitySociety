using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : Object
{
	private static ActionManager instance;

	public static ActionManager GetInstance()
	{
		if (instance == null)
		{
			instance = new ActionManager();
		}
		return instance;
	}

	public void Reset(GameObject obj)
	{
		Action[] actions = obj.GetComponents<Action>();
		foreach (Action ac in actions)
		{
			ac.Free();
		}
	}

	public Action ApplyMoveToAction(GameObject obj, Vector3 position, ActionID type, ActionCompleted callback)
	{
		if (type == ActionID.MOVETO)
		{
			ActionMoveTo[] movetos = obj.GetComponents<ActionMoveTo>();
			foreach (ActionMoveTo moveto in movetos)
				Destroy(moveto);
		}

		ActionLookAt[] lookats = obj.GetComponents<ActionLookAt>();
		foreach (ActionLookAt lookat in lookats)
			lookat.Finish();
		
		ActionMoveTo ac = obj.AddComponent<ActionMoveTo>();
		ac.setting(obj, position, type, callback);
		return ac;
	}


	public Action ApplyElevatorAction(GameObject obj, FacilityElevator elevator, int waitingFloor, int targetFloor, ActionCompleted callback)
	{
		ActionTakeElevator ac = obj.AddComponent<ActionTakeElevator>();
		ac.setting(obj, elevator, waitingFloor, targetFloor, callback);
		return ac;
	}

	public Action ApplySetDestinationAction(GameObject obj, Vector3 position, ActionCompleted callback)
	{
		ActionSetDestination ac = obj.AddComponent<ActionSetDestination>();
		ac.setting(obj, position, callback);
		return ac;
	}

	public Action ApplyChatAction(GameObject obj, string content, float duration, ActionCompleted callback)
	{
		ActionChat ac = obj.AddComponent<ActionChat>();
		ac.setting(obj, content, duration, callback);
		return ac;
	}

	public Action ApplyLookAtAction(GameObject obj, Vector3 lookAtTargetPosition, ActionCompleted callback)
	{
		ActionLookAt[] lookats = obj.GetComponents<ActionLookAt>();
		foreach (ActionLookAt lookat in lookats)
			lookat.Finish();
		ActionLookAt ac = obj.AddComponent<ActionLookAt>();
		ac.setting(obj, lookAtTargetPosition, callback);
		return ac;
	}
}


public interface ActionCompleted
{
	void OnActionCompleted(Action action);
}


public class Action : MonoBehaviour
{
	public ActionID ID;

	public void Free()
	{
		Destroy(this);	// 卸载脚本
	}
}


public class ActionAuto : Action
{
}


public class ActionMan:Action
{
}


public enum ActionID
{
	MOVETO,
	TAKEELEVATOR,
	SETDESTINATION,
	CHAT,
	AVOID,
	LOOKAT
}