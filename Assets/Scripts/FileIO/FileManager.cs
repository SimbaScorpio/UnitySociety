using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour
{
	void Start ()
	{
		LoadGameData ();
	}

	void LoadGameData ()
	{
		StartCoroutine (LoadData (Global.JsonURL));
	}

	IEnumerator LoadData (string url)
	{
		WWW www = new WWW (url);
		yield return www;
		if (!string.IsNullOrEmpty (www.error))
			Log.error (www.error);
		else {
			string json = www.text;
			try {
				Log.info (Log.green ("正在解析数据集..."));
				StorylineManager.GetInstance ().storyline = JsonUtility.FromJson<Storyline> (json);
				Log.info (Log.green ("数据集解析完成！"));
				StorylineManager.GetInstance ().Initialize ();
			} catch (Exception e) {
				Log.error ("解析数据集出现错误");
				Log.error (e.ToString ());
			}
		}
	}
}
