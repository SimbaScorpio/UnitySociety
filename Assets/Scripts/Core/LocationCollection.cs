using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationCollection : ScriptableObject
{
	private static Dictionary<string, Transform> locations;
	private static string defaultName = "location_1";
	private static string objectLocationSuffix = "地点";

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
			Log.error ("Try to get a null location");
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


	public static Transform GetNearestObject (Vector3 position, string objectName)
	{
		if (string.IsNullOrEmpty (objectName)) {
			Log.error ("Try to get a null object's location ");
			return locations [defaultName];
		}
		objectName += objectLocationSuffix;
		GameObject[] locationObjs = GameObject.FindGameObjectsWithTag (objectName);
		if (locationObjs.Length == 0) {
			Log.error ("There is no such object location tag [" + objectName + "]");
			return locations [defaultName];
		}
		Transform closest = locationObjs [0].transform;
		float minDistance = float.MaxValue;
		for (int i = 0; i < locationObjs.Length; ++i) {
			float distance = Vector3.Distance (position, locationObjs [i].transform.position);
			if (distance < minDistance) {
				minDistance = distance;
				closest = locationObjs [i].transform;
			}
		}
		return closest;
	}
}
