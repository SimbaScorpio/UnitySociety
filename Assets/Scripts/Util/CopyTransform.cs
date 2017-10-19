using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class CopyTransform : MonoBehaviour
	{
		public Transform target;
		public float LerpRate = 20f;

		void Update ()
		{
			if (target != null) {
				transform.position = target.position;
				//transform.position = Vector3.Lerp (transform.position, target.position, Time.deltaTime * LerpRate);
				transform.rotation = target.rotation;
				//transform.rotation = Quaternion.Lerp (transform.rotation, target.rotation, Time.deltaTime * LerpRate);
				transform.localScale = target.lossyScale;
				//transform.localScale = Vector3.Lerp (transform.localScale, target.lossyScale, Time.deltaTime * LerpRate);
			}
		}
	}
}