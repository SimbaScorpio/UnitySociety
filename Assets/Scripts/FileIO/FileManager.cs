using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
