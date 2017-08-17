﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{
	public const string characterFileName = "storyline.json";
	private string url;

	void Start ()
	{
		LoadGameData ();
	}

	void LoadGameData ()
	{
		url = "file://" + Application.dataPath + "/Data/" + characterFileName;
		StartCoroutine (LoadData ());
	}

	IEnumerator LoadData ()
	{
		WWW www = new WWW (url);
		yield return www;
		if (!string.IsNullOrEmpty (www.error))
			Debug.Log (www.error);
		else {
			string json = www.text;
			StorylineManager.GetInstance ().storyline = JsonUtility.FromJson<Storyline> (json);
			StorylineManager.GetInstance ().Initialize ();
		}
	}
}