using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public interface IActionCompleted
	{
		void OnActionCompleted (Action action);
	}


	public class Action : MonoBehaviour
	{
		//public ActionID id;

		public void Free ()
		{
			DestroyImmediate (this);	// 卸载脚本
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
			
		};
	}
}