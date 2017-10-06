using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class CameraFollower : MonoBehaviour
	{
		public GameObject Character;
		public float SmoothTime = 0.3f;

		public float RotY = 180f;
		public float distance = 100f;
		public float DiffX = 0.433f;
		public float DiffY = 0.25f;

		public bool IsShaking = false;
		public float ShakeAmount = 1f;
		public float ShakeTime = 0.5f;

		float mShakeTime;

		Vector3 velocity = Vector3.zero;

		// Use this for initialization
		void Start ()
		{
			transform.rotation = Quaternion.Euler (new Vector3 (0, RotY, 0));
		}

		// Update is called once per frame
		void Update ()
		{
			if (Character != null) {
				SmoothFollow ();
			}
			if (IsShaking && mShakeTime > 0) {
				mShakeTime -= Time.deltaTime;
				float dx = Random.insideUnitSphere.x * ShakeAmount;
				float dz = Random.insideUnitSphere.z * ShakeAmount;
				Vector3 dv = new Vector3 (dx, 0, dz);
				transform.position = transform.position + dv;
			} else {
				IsShaking = false;
				mShakeTime = ShakeTime;
			}
		}

		void SmoothFollow ()
		{
			Vector3 target = Character.transform.position;
			float distanceZ = Character.transform.position.z - transform.position.z;
			float diffX = distanceZ * DiffX;
			float diffY = distanceZ * DiffY;
			float x = target.x + diffX;
			float y = target.y - diffY;
			float z = distance + target.z;
			Vector3 dest = new Vector3 (x, y, z);
			transform.position = Vector3.SmoothDamp (transform.position, dest, ref velocity, SmoothTime);
		}

		public void Shake ()
		{
			IsShaking = true;
			mShakeTime = ShakeTime;
		}
	}
}