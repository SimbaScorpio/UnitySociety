using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCollection : ScriptableObject
{
	private static Dictionary<string, Material> materials;
	private static string defaultName = "player_1";

	public static Material Get (string name)
	{
		if (materials == null) {
			materials = new Dictionary<string, Material> ();
			Material mat = Resources.Load ("Materials/" + defaultName) as Material;
			if (mat == null) {
				Log.error ("Load default material [" + defaultName + "] failed: cannot find");
			} else {
				materials.Add (defaultName, mat);
			}
		}

		if (string.IsNullOrEmpty (name)) {
			return materials [defaultName];
		}

		if (materials.ContainsKey (name)) {
			return materials [name];
		} else {
			Material mat = Resources.Load ("Materials/" + name) as Material;
			if (mat == null) {
				Log.error ("Load material [" + name + "] failed: cannot find");
				return materials [defaultName];
			} else {
				materials.Add (name, mat);
				return mat;
			}
		}
	}
}
