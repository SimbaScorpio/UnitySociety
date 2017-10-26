using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;

namespace DesignSociety
{
	public class FileManager : MonoBehaviour
	{
		private UIFileDialog uiFileDialog;
		private int loadCount = 0;

		private static FileManager instance;

		public static FileManager GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
			uiFileDialog = FindObjectOfType<UIFileDialog> ();
		}

		public void StartLoading ()
		{
			if (++loadCount > 0)
				Log.info ("----第【" + loadCount + "】次更新数据----");
			LoadLandmarkData ();
			LoadGameData ();
		}

		public void LoadGameData ()
		{
			if (uiFileDialog == null || uiFileDialog.storylinePath == "") {
				//StartCoroutine (LoadGameDataCoroutine (Global.StorylineJsonURL));
			} else {
				StartCoroutine (LoadGameDataCoroutine (GetFileURL (uiFileDialog.storylinePath)));
			}
		}

		IEnumerator LoadGameDataCoroutine (string url)
		{
			WWW www = new WWW (url);
			yield return www;
			if (!string.IsNullOrEmpty (www.error)) {
				Log.error ("无法打开故事线文件");
			} else {
				string json = www.text;
				try {
					Log.info (Log.green ("正在解析故事线..."));
					StorylineManager.GetInstance ().storyline = JsonUtility.FromJson<Storyline> (json);
					Log.info (Log.green ("故事线解析完成！"));
					StorylineManager.GetInstance ().Restart ();
				} catch (Exception e) {
					Log.error ("解析故事线出现错误");
					Log.error (e.ToString ());
				}
			}
		}

		public void LoadLandmarkData ()
		{
			if (uiFileDialog == null || uiFileDialog.landmarkPath == "") {
				//StartCoroutine (LoadLandmarkDataCoroutine (Global.LandmarkJsonRURL));
			} else {
				StartCoroutine (LoadLandmarkDataCoroutine (GetFileURL (uiFileDialog.landmarkPath)));
			}
		}

		IEnumerator LoadLandmarkDataCoroutine (string url)
		{
			WWW www = new WWW (url);
			yield return www;
			if (!string.IsNullOrEmpty (www.error)) {
				Log.error ("无法打开坐标集文件");
			} else {
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
				byte[] bytes = Encoding.UTF8.GetBytes (json);

				ExtensionFilter[] filters = new ExtensionFilter[] {
					new ExtensionFilter ("Json", "json")
				};
				string path = StandaloneFileBrowser.SaveFilePanel ("Save File", "", "", filters);
				if (string.IsNullOrEmpty (path))
					return false;
				FileStream fs = new FileStream (path, FileMode.Create);
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

		public bool SaveLogData (string data)
		{
			try {
				byte[] bytes = Encoding.UTF8.GetBytes (data);

				ExtensionFilter[] filters = new ExtensionFilter[] {
					new ExtensionFilter ("Text", "txt")
				};
				string path = StandaloneFileBrowser.SaveFilePanel ("Save File", "", "", filters);
				if (string.IsNullOrEmpty (path))
					return false;
				FileStream fs = new FileStream (path, FileMode.Create);
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

		string GetFileURL (string path)
		{
			return (new System.Uri (path)).AbsoluteUri;
		}
	}
}