using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class MaterialCollection : MonoBehaviour
	{
		private Dictionary<string, Material> materials = new Dictionary<string, Material> ();
		private NetworkManagerHUD hud;

		#region instance

		private static MaterialCollection instance;

		public static MaterialCollection GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		#endregion

		void Start ()
		{
			hud = FindObjectOfType<NetworkManagerHUD> ();
			hud.showGUI = false;
			LoadClothesData ();
			LoadScreenData ();
			LoadIconData ();
		}

		public Texture2D GetTexture (string name, string path)
		{
			if (clothesTextures.ContainsKey (name)) {
				return clothesTextures [name];
			} else if (iconTextures.ContainsKey (name)) {
				return iconTextures [name];
			} else if (screenTextures.ContainsKey (name)) {
				return screenTextures [name];
			} else {
				return null;
			}
		}

		public Material GetMaterial (string name)
		{
			if (materials.ContainsKey (name)) {
				return materials [name];
			} else {
				return LoadMaterial (name);
			}
		}

		Material LoadMaterial (string name)
		{
			Texture2D texture = GetTexture (name, Global.TexturePath);
			if (texture != null) {
				Material mat = CreateMaterialWithTexture (texture);
				materials.Add (name, mat);
				return mat;
			}
			return null;
		}

		Material CreateMaterialWithTexture (Texture2D texture)
		{
			Material mat = new Material (Shader.Find ("Standard"));
			mat.SetTexture ("_EmissionMap", texture);
			mat.SetColor ("_EmissionColor", Color.white);
			mat.EnableKeyword ("_EMISSION");
			return mat;
		}


		#region 资源预加载

		public Dictionary<string, Texture2D> clothesTextures = new Dictionary<string, Texture2D> ();
		public Dictionary<string, Texture2D> iconTextures = new Dictionary<string, Texture2D> ();
		public Dictionary<string, Texture2D> screenTextures = new Dictionary<string, Texture2D> ();

		public bool loadClothes;
		public bool loadIcon;
		public bool loadScreen;

		private string[] paths;
		private string filename;
		private List<string> filenames = new List<string> ();

		public void OnClothesLoaded ()
		{
			loadClothes = true;
			if (loadScreen && loadIcon)
				hud.showGUI = true;
		}

		public void OnIconLoaded ()
		{
			loadIcon = true;
			if (loadClothes && loadScreen)
				hud.showGUI = true;
		}

		public void OnScreenLoaded ()
		{
			loadScreen = true;
			if (loadClothes && loadIcon)
				hud.showGUI = true;
		}

		#region 加载衣物贴图

		public void LoadClothesData ()
		{
			// 避免重复加载
			if (clothesTextures.Count != 0)
				return;

			// 获取文件夹下所有图片路径
			string directory = FineTuneDirectoryPath (Global.TexturePath);
			try {
				paths = System.IO.Directory.GetFiles (directory);
			} catch (Exception e) {
				Log.error (e.Message);
				OnClothesLoaded ();
				return;
			}
			if (paths.Length == 0)
				OnClothesLoaded ();

			filenames.Clear ();
			for (int i = 0; i < paths.Length; ++i) {
				filename = GetFileNameWithoutDot (GetFileName (paths [i]));
				filenames.Add (filename);
				clothesTextures [filename] = null;
			}
			for (int i = 0; i < paths.Length; ++i) {
				StartCoroutine (LoadClothesDataCoroutine (GetFileURL (paths [i]), filenames [i]));
			}
		}

		IEnumerator LoadClothesDataCoroutine (string url, string filename)
		{
			WWW www = new WWW (url);
			yield return www;
			if (!string.IsNullOrEmpty (www.error)) {
				clothesTextures.Remove (filename);
			} else {
				try {
					Texture2D texture = new Texture2D (2, 2);
					www.LoadImageIntoTexture (texture);
					clothesTextures [filename] = texture;
				} catch (Exception e) {
					Log.error (e.ToString ());
					clothesTextures.Remove (filename);
				}
			}
			foreach (string file in clothesTextures.Keys) {
				if (clothesTextures [file] == null)
					yield break;
			}
			OnClothesLoaded ();
		}

		#endregion

		#region 加载icon贴图

		public void LoadIconData ()
		{
			if (iconTextures.Count != 0)
				return;
			
			string directory = FineTuneDirectoryPath (Global.IconPath);
			try {
				paths = System.IO.Directory.GetFiles (directory);
			} catch (Exception e) {
				Log.error (e.Message);
				OnIconLoaded ();
				return;
			}
			if (paths.Length == 0)
				OnIconLoaded ();

			filenames.Clear ();
			for (int i = 0; i < paths.Length; ++i) {
				filename = GetFileNameWithoutDot (GetFileName (paths [i]));
				filenames.Add (filename);
				iconTextures [filename] = null;
			}
			for (int i = 0; i < paths.Length; ++i) {
				StartCoroutine (LoadIconDataCoroutine (GetFileURL (paths [i]), filenames [i]));
			}
		}

		IEnumerator LoadIconDataCoroutine (string url, string filename)
		{
			WWW www = new WWW (url);
			yield return www;
			if (!string.IsNullOrEmpty (www.error)) {
				iconTextures.Remove (filename);
			} else {
				try {
					Texture2D texture = new Texture2D (2, 2);
					www.LoadImageIntoTexture (texture);
					iconTextures [filename] = texture;
				} catch (Exception e) {
					Log.error (e.ToString ());
					iconTextures.Remove (filename);
				}
			}
			foreach (string file in iconTextures.Keys) {
				if (iconTextures [file] == null)
					yield break;
			}
			OnIconLoaded ();
		}

		#endregion

		#region 加载屏幕贴图

		public void LoadScreenData ()
		{
			if (screenTextures.Count != 0)
				return;
			
			string directory = FineTuneDirectoryPath (Global.DashboardPath);
			try {
				paths = System.IO.Directory.GetFiles (directory);
			} catch (Exception e) {
				Log.error (e.Message);
				OnScreenLoaded ();
				return;
			}
			if (paths.Length == 0)
				OnScreenLoaded ();

			filenames.Clear ();
			for (int i = 0; i < paths.Length; ++i) {
				filename = GetFileNameWithoutDot (GetFileName (paths [i]));
				filenames.Add (filename);
				screenTextures [filename] = null;
			}
			for (int i = 0; i < paths.Length; ++i) {
				StartCoroutine (LoadScreenDataCoroutine (GetFileURL (paths [i]), filenames [i]));
			}
		}

		IEnumerator LoadScreenDataCoroutine (string url, string filename)
		{
			WWW www = new WWW (url);
			yield return www;
			if (!string.IsNullOrEmpty (www.error)) {
				screenTextures.Remove (filename);
			} else {
				try {
					Texture2D texture = new Texture2D (2, 2);
					www.LoadImageIntoTexture (texture);
					screenTextures [filename] = texture;
				} catch (Exception e) {
					Log.error (e.ToString ());
					screenTextures.Remove (filename);
				}
			}
			foreach (string file in screenTextures.Keys) {
				if (screenTextures [file] == null)
					yield break;
			}
			OnScreenLoaded ();
		}

		#endregion

		#endregion

		# region 文件名处理

		string GetFileURL (string path)
		{
			return (new System.Uri (path)).AbsoluteUri;
		}

		string GetFileName (string path)
		{
			int index = path.LastIndexOf (Path.DirectorySeparatorChar);
			return (index < 0) ? path : path.Substring (index + 1, path.Length - index - 1);
		}

		string GetFileNameWithoutDot (string filename)
		{
			int index = filename.LastIndexOf ('.');
			return (index < 0) ? filename : filename.Substring (0, index);
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

		#endregion
	}
}