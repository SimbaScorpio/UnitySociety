using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class MaterialCollection : MonoBehaviour
	{
		private Dictionary<string, Material> materials;
		private Dictionary<string, Texture2D> textures;

		private List<string> errorTextures;

		private static MaterialCollection instance;

		public static MaterialCollection GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
			materials = new Dictionary<string, Material> ();
			textures = new Dictionary<string, Texture2D> ();
			errorTextures = new List<string> ();
		}

		public Material GetMaterial (string name)
		{
			if (string.IsNullOrEmpty (name) || errorTextures.Contains (name)) {
				return null;
			}
			if (materials.ContainsKey (name)) {
				return materials [name];
			} else {
				return LoadMaterial (name);
			}
		}

		public Texture2D GetTexture (string name, string path)
		{
			if (string.IsNullOrEmpty (name) || errorTextures.Contains (name)) {
				return null;
			}
			if (textures.ContainsKey (name)) {
				return textures [name];
			} else {
				return LoadTexture (name, path);
			}
		}

		Texture2D LoadTexture (string name, string path)
		{
			string url = path + name + ".png";
			url = GetFileURL (url);
			//Log.info ("贴图路径：" + url);

			WWW www = new WWW (url);
			while (!www.isDone) {
				// blocked
			}
			if (!string.IsNullOrEmpty (www.error)) {
				Log.error (www.error);
				errorTextures.Add (name);
				return null;
			} else {
				Texture2D texture = new Texture2D (2, 2);
				www.LoadImageIntoTexture (texture);
				textures.Add (name, texture);
				return texture;
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

		string GetFileURL (string path)
		{
			return (new System.Uri (path)).AbsoluteUri;
		}
	}
}