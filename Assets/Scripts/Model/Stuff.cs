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
		private Rigidbody rbody;


		private Transform parent;

		void Start ()
		{
			gameObject.tag = stuffType.ToString ();
			rbody = GetComponent<Rigidbody> ();
		}

		public void SetParent (Transform root)
		{
			if (parent == root)
				return;
			StopAllCoroutines ();
			transform.SetParent (root);
			parent = root;

			if (root == null) {
				isOwned = false;
				UnLockPhysics ();
			} else {
				isOwned = true;
				LockPhysics ();
				StartCoroutine (LerpToPosition ());
				StartCoroutine (LerpToRotation ());
			}
		}

		//		void Update ()
		//		{
		//			if (parent != null) {
		//				Vector3 targetPosition = parent.position + offsetPosition;
		//				transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * LerpPositionRate);
		//				Quaternion targetRotation = Quaternion.Euler (parent.rotation.eulerAngles + offsetRotation);
		//				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * LerpRotationRate);
		//			}
		//		}

		IEnumerator LerpToPosition ()
		{
			Vector3 targetPosition = offsetPosition;
			while (Vector3.Distance (transform.localPosition, targetPosition) > 0.01f) {
				targetPosition = offsetPosition;
				transform.localPosition = Vector3.Lerp (transform.localPosition, targetPosition, Time.deltaTime * LerpPositionRate);
				yield return null;
			}
		}

		IEnumerator LerpToRotation ()
		{
			Quaternion targetRotation = Quaternion.Euler (offsetRotation);
			while (Vector3.Distance (transform.localRotation.eulerAngles, offsetRotation) > 0.01f) {
				targetRotation = Quaternion.Euler (offsetRotation);
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