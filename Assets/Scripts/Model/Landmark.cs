using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Landmark
{
	public string name;
	public string label;
	public float[] data;

	public Vector3 position { 
		get {
			return new Vector3 (data [0], data [1], data [2]);
		}
	}

	public Quaternion rotation {
		get {
			return Quaternion.Euler (new Vector3 (0, data [3], 0));
		}
	}

	public Landmark Copy ()
	{
		Landmark temp = new Landmark ();
		temp.label = label;
		temp.name = name;
		temp.data = new float[data.Length];
		for (int i = 0; i < data.Length; ++i) {
			temp.data [i] = data [i];
		}
		return temp;
	}

	public Landmark ()
	{
		name = label = "";
		data = new float[4]{ 0, 0, 0, 0 };
	}
}
