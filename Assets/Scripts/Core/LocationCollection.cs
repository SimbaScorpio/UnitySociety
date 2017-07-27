using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationCollection : ScriptableObject
{
	private static Dictionary<string, Transform> locations;
	private static string defaultName = "location_1";

	public static Transform Get (string name)
	{
		if (locations == null) {
			locations = new Dictionary<string, Transform> ();
			GameObject obj = GameObject.Find (defaultName);
			if (obj == null) {
				Log.error ("Find default location object [" + defaultName + "] failed: cannot find");
			} else {
				locations.Add (defaultName, obj.transform);
			}
		}

		if (string.IsNullOrEmpty (name)) {
			return locations [defaultName];
		}
			
		if (locations.ContainsKey (name)) {
			return locations [name];
		} else {
			GameObject obj = GameObject.Find (name);
			if (obj == null) {
				Log.error ("Find location object [" + name + "] failed: cannot find");
				return locations [defaultName];
			} else {
				locations.Add (name, obj.transform);
				return obj.transform;
			}
		}
	}
}
