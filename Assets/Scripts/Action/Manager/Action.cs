using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionCompleted
{
	void OnActionCompleted (Action action);
}


public class Action : MonoBehaviour
{
	public ActionID id;

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


public enum ActionID
{
	IDLE,
	WALKTO,
	CHATBUBBLE,
	SITDOWN,
	STANDUP,
	CLICK,
	TYPE,
	SCRATCHHEAD,
	HANDONCHIN,
	USECAMERA,
	PICKUPTELEPHONE,
	PUTDOWNTELEPHONE,
	DIALTELEPHONE,
	USETELEPHONE,
	SPEECH
}
