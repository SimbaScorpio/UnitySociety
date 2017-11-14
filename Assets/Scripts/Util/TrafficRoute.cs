using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class TrafficRoute : MonoBehaviour
	{
		public List<Vector3> corners = new List<Vector3> ();

		void Awake ()
		{
			Transform[] trs = GetComponentsInChildren<Transform> ();
			for (int i = 1; i < trs.Length; ++i) {
				corners.Add (trs [i].position);
			}
		}
	}
}