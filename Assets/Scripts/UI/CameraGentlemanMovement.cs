using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignSociety
{
	[RequireComponent (typeof(CameraPerspectiveEditor))]
	[RequireComponent (typeof(CameraFollower))]
	public class CameraGentlemanMovement : MonoBehaviour
	{
		public Vector2 rayDistanceLimit = new Vector2 (10f, 200f);
		public float rayOrthographicScale = 50f;
		public float zoomRate = 10f;
		public float panRate = 1f;
		public float dampening = 20.0f;

		public Vector2 clampXWithDisplay;
		public Vector2 clampYWithDisplay;

		public Vector2 clampXWithEditor;
		public Vector2 clampYWithEditor;

		private Vector2 desiredPos;
		private Vector2 currentPos;

		private bool isDesired;
		private Vector3 desiredPosition;

		private float currentDistance;
		private float desiredDistance;
		private float rayDistance;

		private Camera cameraScript;
		private CameraPerspectiveEditor cameraEditor;
		private CameraFollower cameraFollower;

		private Vector3 _position;
		private float _size;

		public void SetDesirePosition (Vector3 pos)
		{
			isDesired = true;
			desiredPosition = pos;
		}

		public void ResetTransform ()
		{
			cameraScript.orthographicSize = _size;
			SetDesirePosition (_position);
		}

		public void DisplayAspect (bool flag)
		{
			if (flag == true) {
				//Screen.SetResolution (1920, 1080, false);
				//cameraScript.aspect = 16f / 9f;
				cameraFollower.clampX = clampXWithDisplay;
				cameraFollower.clampY = clampYWithDisplay;
				ClampPosition ();
			} else {
				//Screen.SetResolution (3300, 800, false);
				//cameraScript.aspect = 3300f / 800f;
				cameraFollower.clampX = clampXWithEditor;
				cameraFollower.clampY = clampYWithEditor;
				ClampPosition ();
			}
		}

		void ClampPosition ()
		{
			transform.position = cameraFollower.ClampPosition (transform.position);
			desiredPosition = transform.position;
		}

		void Start ()
		{
			cameraScript = GetComponent <Camera> ();
			cameraEditor = GetComponent<CameraPerspectiveEditor> ();
			cameraFollower = GetComponent<CameraFollower> ();
			cameraScript.orthographic = true;
			_size = cameraScript.orthographicSize;
			_position = transform.position;
			DisplayAspect (false);
		}

		void Update ()
		{
			if (Time.timeScale <= 0)
				return;
			UpdateRaycast ();

			if (Input.GetMouseButton (2)) {
				desiredPos.x = -Input.GetAxis ("Mouse X") * panRate * rayDistance * Time.deltaTime;
				desiredPos.y = -Input.GetAxis ("Mouse Y") * panRate * rayDistance * Time.deltaTime;
				isDesired = false;
			} else {
				desiredPos.x = desiredPos.y = 0;
			}

			// 平移
			if (isDesired) {
				currentPos.x = currentPos.y = desiredPos.x = desiredPos.y = 0;
				float distance = Vector3.Distance (transform.position, desiredPosition);
				transform.position = Vector3.MoveTowards (transform.position, desiredPosition, Time.deltaTime * distance / 0.3f);
				//transform.position = desiredPosition;
				if (distance < 0.1f)
					isDesired = false;
			} else {
				currentPos.x = Mathf.Lerp (currentPos.x, desiredPos.x, Time.deltaTime * dampening);
				currentPos.y = Mathf.Lerp (currentPos.y, desiredPos.y, Time.deltaTime * dampening);
				transform.position += transform.TransformDirection (Vector3.right) * currentPos.x;
				transform.position += transform.TransformDirection (Vector3.up) * currentPos.y;
				ClampPosition ();
			}

			// 缩放
			if (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject ()) {
				desiredDistance = Input.GetAxis ("Mouse ScrollWheel") * zoomRate * rayDistance * Time.deltaTime * 2;
				currentDistance = Mathf.Lerp (currentDistance, desiredDistance, Time.deltaTime * dampening);
				cameraScript.orthographicSize -= currentDistance;
				cameraScript.orthographicSize = cameraScript.orthographicSize > _size ? _size : cameraScript.orthographicSize;
				if (cameraScript.orthographicSize <= 0.1f)
					cameraScript.orthographicSize = 0.1f;
				ClampPosition ();
			}
			if (Mathf.Abs (desiredDistance) != 0)
				isDesired = false;
		}

		void UpdateRaycast ()
		{
			Ray ray = cameraEditor.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, rayDistanceLimit.y)) {
				rayDistance = hit.distance;
			} else {
				rayDistance = rayDistanceLimit.y;
			}
			rayDistance = rayDistance * cameraScript.orthographicSize / rayOrthographicScale;
			rayDistance = Mathf.Clamp (rayDistance, rayDistanceLimit.x, rayDistanceLimit.y);
		}
	}
}