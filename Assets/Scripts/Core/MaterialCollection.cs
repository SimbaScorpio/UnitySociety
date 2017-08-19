using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCollection : MonoBehaviour
{
	private Dictionary<string, Material> materials;

	private static MaterialCollection instance;

	public static MaterialCollection GetInstance ()
	{
		return instance;
	}

	void Awake ()
	{
		instance = this;
		materials = new Dictionary<string, Material> ();
	}

	public Material Get (string name)
	{
		if (string.IsNullOrEmpty (name)) {
			return null;
		}
		if (materials.ContainsKey (name)) {
			return materials [name];
		} else {
			return LoadData (name);
		}
	}

	Material LoadData (string name)
	{
		string url = Global.TexturePath + name;
		WWW www = new WWW (url);
		while (!www.isDone) {
			// blocked
		}
		if (!string.IsNullOrEmpty (www.error)) {
			Log.error (www.error);
			return null;
		} else {
			Texture2D texture = new Texture2D (2, 2);
			www.LoadImageIntoTexture (texture);
			Material mat = CreateMaterialWithTexture (texture);
			materials.Add (name, mat);
			return mat;
		}
	}

	Material CreateMaterialWithTexture (Texture2D texture)
	{
		Material mat = new Material (Shader.Find ("Standard"));
		mat.SetTexture ("_EmissionMap", texture);
		mat.SetColor ("_EmissionColor", Color.white);
		mat.EnableKeyword ("_EMISSION");
		return mat;
	}
}
