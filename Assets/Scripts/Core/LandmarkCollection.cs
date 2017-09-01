using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkCollection : MonoBehaviour
{
	// Read-only
	public LandmarkList list;

	// won't change since first read
	private Dictionary<string, Landmark> landmarks;

	private List<Landmark> labeledLandmarks;

	private static LandmarkCollection instance;

	public static LandmarkCollection GetInstance ()
	{
		return instance;
	}

	void Awake ()
	{
		instance = this;
		landmarks = new Dictionary<string, Landmark> ();
		labeledLandmarks = new List<Landmark> ();
	}

	public void Initialize ()
	{
		foreach (Landmark lm in list.landmarkList) {
			if (landmarks.ContainsKey (lm.name)) {
				Log.warn ("Initialize landmark [" + lm.name + "] warning: overlapped name");
			}
			landmarks [lm.name] = lm;
		}
		if (UILandmarkManager.GetInstance ())
			UILandmarkManager.GetInstance ().Initialize (list.landmarkList);
	}


	public Landmark Get (string name)
	{
		if (string.IsNullOrEmpty (name)) {
			Log.error ("Try to get a null location");
			return null;
		}
		if (landmarks.ContainsKey (name)) {
			return landmarks [name];
		}
		Log.error ("Try to get a nonexist location");
		return null;
	}


	public Landmark GetNearestObject (Vector3 position, string objectName)
	{
		if (string.IsNullOrEmpty (objectName)) {
			Log.error ("Try to get a null object's location ");
			return null;
		}
		labeledLandmarks.Clear ();
		foreach (string name in landmarks.Keys) {
			if (landmarks [name].label == objectName)
				labeledLandmarks.Add (landmarks [name]);
		}
		if (labeledLandmarks.Count == 0) {
			Log.error ("There is no such object location label [" + objectName + "]");
			return null;
		}
		Landmark closest = labeledLandmarks [0];
		float minDistance = float.MaxValue;
		for (int i = 0; i < labeledLandmarks.Count; ++i) {
			float distance = Vector3.Distance (position, labeledLandmarks [i].position);
			if (distance < minDistance) {
				minDistance = distance;
				closest = labeledLandmarks [i];
			}
		}
		return closest;
	}
}
