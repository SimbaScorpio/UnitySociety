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
		}
	}

	public static void warn (string content)
	{
		if (display && displayWarn) {
			Debug.LogWarning (content);
		}
	}

	public static void info (string content)
	{
		if (display && displayInfo) {
			Debug.Log (content);
		}
	}
}
