using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionCompleted
{
	void OnActionCompleted (Action action);
}


public class Action : MonoBehaviour
{
	//public ActionID id;

	public void Free ()
	{
		Destroy (this);	// 卸载脚本
	}
}


public class ActionSingle : Action
{
	// 独立动作
}


public class ActionThread : Action
{
	// 同存动作
}


public class ActionName
{
	public static string[] validName = {
		"1"
	};

	public static bool IsValidAction (string name)
	{
		return true;
	}

	public static bool IsStandAction (string name)
	{
		return true;
	}

	public static bool IsSitAction (string name)
	{
		return true;
	}

	public static bool IsTelephoneAction (string name)
	{
		return true;
	}
}

