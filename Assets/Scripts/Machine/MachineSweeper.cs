using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace DesignSociety
{
	public class MachineSweeper : MonoBehaviour
	{
		public bool sweep = true;
		public List<Transform> anchors;

		private MyRichAI ai;
		private int index = 0;

		void Start ()
		{
			ai = GetComponent<MyRichAI> ();
			if (sweep)
				Sweep ();
		}

		void Update ()
		{
			if (sweep) {
				ai.enabled = true;
				if (Vector3.Distance (transform.position, ai.target.position) < 0.1f) {
					Sweep ();
				}
			} else {
				ai.enabled = false;
			}
		}

		void Sweep ()
		{
			if (anchors.Count > 0) {
				Vector3 target = anchors [(index++) % anchors.Count].position;
				NNInfo info = AstarData.active.GetNearest (target);
				target = info.clampedPosition;
				Landmark lm = new Landmark ();
				lm.m_data [0] = target.x;
				lm.m_data [1] = target.y;
				lm.m_data [2] = target.z;
				ai.target = lm;
			}
		}
	}
}