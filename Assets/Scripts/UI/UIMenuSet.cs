using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	public void OnButtonReplayClicked ()
	{
		StorylineManager.GetInstance ().Restart ();
	}

	public void OnButtonCloseClicked ()
	{
		Application.Quit ();
	}
}
