using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Landmark
{
	public string name;
	public string label;
	public float px, py, pz, ry;

	public Vector3 position { 
		get {
			return new Vector3 (px, py, pz);
		}
	}

	public Quaternion rotation {
		get {
			return Quaternion.Euler (new Vector3 (0, ry, 0));
		}
	}

	public Landmark Copy ()
	{
		Landmark temp = new Landmark ();
		temp.label = this.label;
		temp.name = this.name;
		temp.px = this.px;
		temp.py = this.py;
		temp.pz = this.pz;
		temp.ry = this.ry;
		return temp;
	}
}
