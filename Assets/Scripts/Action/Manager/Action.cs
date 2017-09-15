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
			"站着不动",
			"使用固定电话",
			"发言",
			"使用相机",
			"站立听",
			"站立鼓掌",
			"站立说话",
			"边说边指远方",
			"边说边指近桌",
			"边说边对桌子指指点点",
			"站立指ppt",
			"传纸",
			"接纸",
			//"",
			"坐着不动",
			"敲击键盘",
			"点击鼠标",
			"挠头思考",
			"托腮思考",
			"坐向后仰",
			"坐抱头后仰",
			"哈哈大笑",
			"VR眼镜",
			"听写记录",
			"拿手机扫码",
			"使用桌面电话",
			"写字画画",
			"丢纸"
		};

		public static bool IsValidAction (string name)
		{
			for (int i = 0; i < validName.Length; ++i) {
				if (validName [i] == name)
					return true;
			}
			return false;
		}

		public static bool IsStandAction (string name)
		{
			int index = 0;
			for (index = 0; index < validName.Length; ++index) {
				if (validName [index] == "")
					break;
			}
			for (int i = 0; i < 13; ++i) {
				if (validName [i] == name)
					return true;
			}
			return false;
		}

		public static bool IsSitAction (string name)
		{
			int index = 0;
			for (index = 0; index < validName.Length; ++index) {
				if (validName [index] == "")
					break;
			}
			for (int i = 13; i < validName.Length; ++i) {
				if (validName [i] == name)
					return true;
			}
			return false;
		}
	}
}