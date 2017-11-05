using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignSociety
{
	public class CameraOrthographicMovement : MonoBehaviour
	{
		public Vector2 rotateSpeed = new Vector2 (50f, 50f);
		public Vector2 rotateYLimit = new Vector2 (-80, 80);

		public Vector2 rayDistanceLimit = new Vector2 (10f, 200f);
		public float rayOrthographicScale = 50f;
		public float zoomRate = 10f;
		public float panRate = 1f;
		public float dampening = 20.0f;

		private Vector2 degree;

		private Vector2 desiredPos;
		private Vector2 currentPos;

		private bool isDesired;
		private Vector3 desiredPosition;

		private float currentDistance;
		private float desiredDistance;
		private float rayDistance;

		private Quaternion currentRotation;
		private Quaternion desiredRotation;

		private Camera cameraScript;

		private Vector2 _degree;
		private Vector3 _position;
		private float _size;

		float ClampAngle (float angle, float min, float max)
		{
			if (angle < -360)
				angle += 360;
			if (angle > 360)
				angle -= 360;
			return Mathf.Clamp (angle, min, max);
		}

		public void SetDesirePosition (Vector3 pos)
		{
			isDesired = true;
			desiredPosition = pos;
		}

		public void ResetTransform ()
		{
			degree.x = _degree.x;
			degree.y = _degree.y;
			cameraScript.orthographicSize = _size;
			SetDesirePosition (_position);
		}

		void Start ()
		{
			cameraScript = GetComponent <Camera> ();
			cameraScript.orthographic = true;
			_size = cameraScript.orthographicSize;
			_degree.x = degree.x = Vector3.Angle (Vector3.right, transform.right);
			_degree.y = degree.y = Vector3.Angle (Vector3.up, transform.up);
			_position = transform.position;
		}

		void Update ()
		{
			if (Time.timeScale <= 0)
				return;
			UpdateRaycast ();

			if (Input.GetMouseButton (1)) {
				degree.x += Input.GetAxis ("Mouse X") * rotateSpeed.x * 0.02f;
				degree.y -= Input.GetAxis ("Mouse Y") * rotateSpeed.y * 0.02f;
				degree.y = ClampAngle (degree.y, rotateYLimit.x, rotateYLimit.y);
				isDesired = false;
			} else if (Input.GetMouseButton (2)) {
				desiredPos.x = -Input.GetAxis ("Mouse X") * panRate * rayDistance * Time.deltaTime;
				desiredPos.y = -Input.GetAxis ("Mouse Y") * panRate * rayDistance * Time.deltaTime;
				isDesired = false;
			} else {
				desiredPos.x = desiredPos.y = 0;
			}

			// 旋转
			desiredRotation = Quaternion.Euler (degree.y, degree.x, 0);
			currentRotation = transform.rotation;
			transform.rotation = Quaternion.Lerp (currentRotation, desiredRotation, Time.deltaTime * dampening);

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
				desiredPosition = transform.position;
			}

			// 缩放
			if (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject ()) {
				desiredDistance = Input.GetAxis ("Mouse ScrollWheel") * zoomRate * rayDistance * Time.deltaTime * 2;
				currentDistance = Mathf.Lerp (currentDistance, desiredDistance, Time.deltaTime * dampening);
				cameraScript.orthographicSize -= currentDistance;
				if (cameraScript.orthographicSize <= 0.1f)
					cameraScript.orthographicSize = 0.1f;
			}
			if (Mathf.Abs (desiredDistance) != 0)
				isDesired = false;
		}

		void UpdateRaycast ()
		{
			Ray ray = cameraScript.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
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