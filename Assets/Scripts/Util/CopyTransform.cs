using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class CopyTransform : MonoBehaviour
	{
		public Transform target;

		void Update ()
		{
			if (target != null) {
				transform.position = target.position;
				transform.rotation = target.rotation;
				transform.localScale = target.lossyScale;
			}
		}
	}
}