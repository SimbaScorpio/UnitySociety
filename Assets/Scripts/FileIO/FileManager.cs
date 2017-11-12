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
		public bool loadAtStart = false;
		private UIFileDialog uiFileDialog;

		private int loadCount = 0;
		private Dictionary<string, bool> storylinefiles = new Dictionary<string, bool> ();
		private Dictionary<string, bool> landmarkfiles = new Dictionary<string, bool> ();

		private static FileManager instance;

		public static FileManager GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			if (loadAtStart)
				StartLoading ();
		}

		public void StartLoading ()
		{
			if (++loadCount > 0)
				Log.info ("----第【" + loadCount + "】次更新数据----");
			if (uiFileDialog == null)
				uiFileDialog = FindObjectOfType<UIFileDialog> ();
			LoadLandmarkData ();
		}


		#region 故事线读取

		public void LoadGameData ()
		{
			LoadKeywordData ();
			if (uiFileDialog == null || uiFileDialog.storylinePath == "") {
				LoadGameDataPath (Global.StorylineJsonURL, true);
			} else {
				LoadGameDataPath (uiFileDialog.storylinePath, false);
			}
		}

		void LoadGameDataPath (string directory, bool flag)
		{
			StorylineManager.GetInstance ().ClearStorylinePart ();
			storylinefiles.Clear ();
			if (flag) {
				directory = FineTuneDirectoryPath (directory);
				string[] paths = System.IO.Directory.GetFiles (directory);
				foreach (string path in paths) {
					storylinefiles [GetFileName (path)] = false;
				}
				foreach (string path in paths) {
					StartCoroutine (LoadGameDataCoroutine (GetFileURL (path), GetFileName (path)));
				}
			} else {
				storylinefiles [GetFileName (directory)] = false;
				StartCoroutine (LoadGameDataCoroutine (GetFileURL (directory), GetFileName (directory)));
			}
		}

		IEnumerator LoadGameDataCoroutine (string jsonurl, string filename)
		{
			WWW www = new WWW (jsonurl);
			yield return www;
			if (!string.IsNullOrEmpty (www.error)) {
				Log.error ("无法打开故事线文件【" + filename + "】");
			} else {
				string json = www.text;
				try {
					Log.info ("=> " + Log.green ("正在解析故事线【" + filename + "】..."));

					StorylinePart part = StorylineManager.GetInstance ().gameObject.AddComponent<StorylinePart> ();
					part.storyline = JsonUtility.FromJson<Storyline> (json);
					part.fileName = filename;
					StorylineManager.GetInstance ().AddStorylinePart (part);

					Log.info (Log.green ("故事线【" + filename + "】解析完成！"));
				} catch (Exception e) {
					Log.error ("解析故事线【" + filename + "】出现错误");
					Log.error (e.ToString ());
				}
			}
			storylinefiles [filename] = true;
			foreach (string file in storylinefiles.Keys) {
				if (storylinefiles [file] == false)
					yield break;
			}
			Log.info ("---");
			StorylineManager.GetInstance ().Restart ();
		}

		#endregion


		#region 坐标集读取

		public void LoadLandmarkData ()
		{
			if (uiFileDialog == null || uiFileDialog.landmarkPath == "") {
				LoadLandmarkDataPath (Global.LandmarkJsonURL, true);
			} else {
				LoadLandmarkDataPath (uiFileDialog.landmarkPath, false);
			}
		}

		void LoadLandmarkDataPath (string directory, bool flag)
		{
			LandmarkCollection.GetInstance ().ClearLandmarkPart ();
			landmarkfiles.Clear ();
			if (flag) {
				directory = FineTuneDirectoryPath (directory);
				string[] paths = System.IO.Directory.GetFiles (directory);
				foreach (string path in paths) {
					landmarkfiles [GetFileName (path)] = false;
				}
				foreach (string path in paths) {
					StartCoroutine (LoadLandmarkDataCoroutine (GetFileURL (path), GetFileName (path)));
				}
			} else {
				landmarkfiles [GetFileName (directory)] = false;
				StartCoroutine (LoadLandmarkDataCoroutine (GetFileURL (directory), GetFileName (directory)));
			}
		}

		IEnumerator LoadLandmarkDataCoroutine (string jsonurl, string filename)
		{
			WWW www = new WWW (jsonurl);
			yield return www;
			if (!string.IsNullOrEmpty (www.error)) {
				Log.error ("无法打开坐标集文件【" + filename + "】");
			} else {
				string json = www.text;
				try {
					Log.info ("=> " + Log.green ("正在解析坐标集【" + filename + "】..."));

					LandmarkList part = JsonUtility.FromJson<LandmarkList> (json);
					LandmarkCollection.GetInstance ().AddLandmarkPart (part);

					Log.info (Log.green ("坐标集【" + filename + "】解析完成！"));
				} catch (Exception e) {
					Log.error ("解析坐标集【" + filename + "】出现错误");
					Log.error (e.ToString ());
				}
			}
			landmarkfiles [filename] = true;
			foreach (string file in landmarkfiles.Keys) {
				if (landmarkfiles [file] == false)
					yield break;
			}
			Log.info ("---");
			LandmarkCollection.GetInstance ().Initialize ();
			LoadGameData ();
		}

		#endregion


		#region 关键词读取

		public void LoadKeywordData ()
		{
			StartCoroutine (LoadKeywordDataCoroutine (Global.KeywordJsonURL));
		}

		IEnumerator LoadKeywordDataCoroutine (string jsonurl)
		{
			print (jsonurl);
			WWW www = new WWW (jsonurl);
			yield return www;
			if (!string.IsNullOrEmpty (www.error)) {
				Log.error ("无法打开关键词文件");
			} else {
				string json = www.text;
				try {
					Log.info ("=> " + Log.green ("正在解析关键词..."));

					KeywordCollection.GetInstance ().keywordList = JsonUtility.FromJson<KeywordList> (json);

					Log.info ("=> " + Log.green ("关键词解析完成！"));
				} catch (Exception e) {
					Log.error ("解析关键词出现错误");
					Log.error (e.ToString ());
				}
			}
		}

		#endregion


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

		string GetFileName (string path)
		{
			int index = path.LastIndexOf (Path.DirectorySeparatorChar);
			return (index < 0) ? path : path.Substring (index + 1, path.Length - index - 1);
		}

		string FineTuneDirectoryPath (string path)
		{
			if (path.Contains ("file:")) {
				for (int i = 5; i < path.Length; ++i) {
					if (path [i] != Path.DirectorySeparatorChar)
						return Path.DirectorySeparatorChar + path.Substring (i, path.Length - i - 1);
				}
			}
			return path;
		}
	}
}