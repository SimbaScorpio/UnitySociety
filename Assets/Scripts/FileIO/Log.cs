using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
	public static bool display = true;
	public static bool displayError = true;
	public static bool displayWarn = true;
	public static bool displayInfo = true;

	public static void error (string content)
	{
		if (display && displayError) {
			Debug.LogError (content);
			UISystemLog.GetInstance ().AddMessage (red (content));
		}
	}

	public static void warn (string content)
	{
		if (display && displayWarn) {
			Debug.LogWarning (content);
			UISystemLog.GetInstance ().AddMessage (yellow (content));
		}
	}

	public static void info (string content)
	{
		if (display && displayInfo) {
			Debug.Log (content);
			UISystemLog.GetInstance ().AddMessage (content);
		}
	}

	public static string red (string content)
	{
		return "<color=red>" + content + "</color>";
	}

	public static string green (string content)
	{
		return "<color=#CAFF70>" + content + "</color>";
	}

	public static string blue (string content)
	{
		return "<color=#BFEFFF>" + content + "</color>";
	}

	public static string yellow (string content)
	{
		return "<color=yellow>" + content + "</color>";
	}

	public static string pink (string content)
	{
		return "<color=#FFA07A>" + content + "</color>";
	}
}
