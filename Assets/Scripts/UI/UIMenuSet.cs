using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuSet : MonoBehaviour
{
	public void OnButtonPlayClicked ()
	{
		Time.timeScale = 1;
	}

	public void OnButtonStopClicked ()
	{
		Time.timeScale = 0;
	}

	public void OnButtonCloseClicked ()
	{
		Application.Quit ();
	}
}
