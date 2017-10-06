using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[RequireComponent (typeof(Rigidbody))]
	public class Stuff : MonoBehaviour
	{
		public StuffType stuffType;
		public bool isOwned;
		public Vector3 offsetPosition;
		public Vector3 offsetRotation;

		private float LerpPositionRate = 20f;
		private float LerpRotationRate = 5f;
		private float LerpPositionError = 0.001f;
		private float LerpRotationError = 0.01f;
		private Rigidbody rbody;

		void Start ()
		{
			gameObject.tag = stuffType.ToString ();
			rbody = GetComponent<Rigidbody> ();
		}

		public void SetParent (Transform root)
		{
			if (transform.parent == root)
				return;
			StopAllCoroutines ();

			if (root == null) {
				isOwned = false;
				transform.SetParent (null);
				UnLockPhysics ();
			} else {
				isOwned = true;
				transform.SetParent (root);
				LockPhysics ();
				//transform.localPosition = offsetPosition;
				StartCoroutine (LerpToPosition ());
				//transform.localRotation = offsetRotation;
				StartCoroutine (LerpToRotation ());
			}
		}

		IEnumerator LerpToPosition ()
		{
			while (Vector3.Distance (transform.localPosition, offsetPosition) > LerpPositionError) {
				transform.localPosition = Vector3.Lerp (transform.localPosition, offsetPosition, Time.deltaTime * LerpPositionRate);
				yield return null;
			}
		}

		IEnumerator LerpToRotation ()
		{
			Quaternion targetRotation = Quaternion.Euler (offsetRotation);
			while (Vector3.Distance (transform.localRotation.eulerAngles, offsetRotation) > LerpRotationError) {
				transform.localRotation = Quaternion.Lerp (transform.localRotation, targetRotation, Time.deltaTime * LerpRotationRate);
				yield return null;
			}
		}

		void LockPhysics ()
		{
			rbody.isKinematic = true;
			rbody.detectCollisions = false;
		}

		void UnLockPhysics ()
		{
			rbody.isKinematic = false;
			rbody.detectCollisions = true;
		}
	}
}