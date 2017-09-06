﻿using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour
{
	public bool loadStoryline;
	public bool loadLandmark;
	private int loadCount = 0;

	private static FileManager instance;

	public static FileManager GetInstance ()
	{
		return instance;
	}

	void Awake ()
	{
		instance = this;
		StartLoading ();
	}

	public void StartLoading ()
	{
		if (loadCount++ > 0)
			Log.info ("----------------------------------------------------------------------------------------------------------------------");
		if (loadLandmark)
			LoadLandmarkData ();
		if (loadStoryline)
			LoadGameData ();
	}

	public void LoadGameData ()
	{
		StartCoroutine (LoadGameDataCoroutine (Global.StorylineJsonURL));
	}

	IEnumerator LoadGameDataCoroutine (string url)
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
				StorylineManager.GetInstance ().Restart ();
			} catch (Exception e) {
				Log.error ("解析数据集出现错误");
				Log.error (e.ToString ());
			}
		}
	}

	public void LoadLandmarkData ()
	{
		StartCoroutine (LoadLandmarkDataCoroutine (Global.LandmarkJsonRURL));
	}

	IEnumerator LoadLandmarkDataCoroutine (string url)
	{
		WWW www = new WWW (url);
		yield return www;
		if (!string.IsNullOrEmpty (www.error))
			Log.error (www.error);
		else {
			string json = www.text;
			try {
				Log.info (Log.green ("正在解析坐标集..."));
				LandmarkCollection.GetInstance ().list = JsonUtility.FromJson<LandmarkList> (json);
				Log.info (Log.green ("坐标集解析完成！"));
				LandmarkCollection.GetInstance ().Initialize ();
			} catch (Exception e) {
				Log.error ("解析坐标集出现错误");
				Log.error (e.ToString ());
			}
		}
	}

	public bool SaveLandmarkData (LandmarkList lmlist)
	{
		try {
			string json = JsonUtility.ToJson (lmlist, true);
			FileStream fs = new FileStream (Global.LandmarkJsonWURL, FileMode.Create);
			byte[] bytes = Encoding.UTF8.GetBytes (json);
			fs.Write (bytes, 0, bytes.Length);
			fs.Flush ();
			fs.Close ();
			fs.Dispose ();
			return true;
		} catch (Exception e) {
			Log.error (e.ToString ());
			return false;
		}
	}
}
