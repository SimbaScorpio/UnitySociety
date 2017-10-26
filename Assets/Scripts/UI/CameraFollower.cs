using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class CameraFollower : MonoBehaviour
	{
		public GameObject target;
		public float smoothTime = 0.3f;
		public Vector2 lensShift = Vector2.zero;
		public Vector2 offset = new Vector2 (0.5f, 0.5f);
		public Vector2 clampX;
		public Vector2 clampY;

		private Vector3 velocity = Vector3.zero;
		private float initialSize;

		void Awake ()
		{
			initialSize = Camera.main.orthographicSize;
			if (clampX.x > clampX.y)
				Switch (ref clampX.x, ref clampX.y);
			if (clampY.x > clampY.y)
				Switch (ref clampY.x, ref clampY.y);
		}

		void Update ()
		{
			if (target != null) {
				SmoothFollow ();
			}
		}

		public Vector3 CalCameraPosition (Vector3 target, float offsetX, float offsetY)
		{
			float distanceZ = target.z - transform.position.z;
			float diffX = distanceZ * lensShift.x;
			float diffY = distanceZ * lensShift.y;
			float x = target.x + diffX + (offsetX * 2 - 1f) * Camera.main.orthographicSize * Screen.width / Screen.height;
			float y = target.y - diffY - (offsetY * 2 - 1f) * Camera.main.orthographicSize;
			Vector3 dest = new Vector3 (x, y, transform.position.z);
			dest = ClampPosition (dest);
			return dest;
		}

		void SmoothFollow ()
		{
			transform.position = Vector3.SmoothDamp (transform.position, CalCameraPosition (target.transform.position, offset.x, offset.y), ref velocity, smoothTime);
			transform.position = ClampPosition (transform.position);
		}

		public Vector3 CalObjectPosition (float z, float offsetX, float offsetY)
		{
			if (z >= transform.position.z)
				return Vector3.zero;
			float distanceZ = z - transform.position.z;
			float diffX = distanceZ * lensShift.x;
			float diffY = distanceZ * lensShift.y;
			float x = transform.position.x - diffX - (offsetX * 2 - 1f) * Camera.main.orthographicSize * Screen.width / Screen.height;
			float y = transform.position.y + diffY + (offsetY * 2 - 1f) * Camera.main.orthographicSize;
			Vector3 dest = new Vector3 (x, y, z);
			return dest;
		}

		public Vector3 ClampPosition (Vector3 pos)
		{
			Vector2 vx = GetValidXRange ();
			Vector2 vy = GetValidYRange ();
			pos.x = Mathf.Clamp (pos.x, vx.x, vx.y);
			pos.y = Mathf.Clamp (pos.y, vy.x, vy.y);
			return pos;
		}

		public Vector2 GetValidXRange ()
		{
			float currentSize = Camera.main.orthographicSize;
			float delta = initialSize - currentSize;
			float xmin = clampX.x - delta * Screen.width / Screen.height;
			float xmax = clampX.y + delta * Screen.width / Screen.height;
			return new Vector2 (xmin, xmax);
		}

		public Vector2 GetValidYRange ()
		{
			float currentSize = Camera.main.orthographicSize;
			float delta = initialSize - currentSize;
			float ymin = clampY.x - delta;
			float ymax = clampY.y + delta;
			return new Vector2 (ymin, ymax);
		}

		void Switch (ref float x, ref float y)
		{
			float t = x;
			x = y;
			y = t;
		}
	}
}