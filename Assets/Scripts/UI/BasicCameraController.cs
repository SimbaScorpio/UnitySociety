using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DesignSociety
{
	public class BasicCameraController : MonoBehaviour
	{
		public enum CameraProjection
		{
			gentleman,
			perspective,
			orthographic
		}

		public CameraProjection projection;
		public Vector2 rotateSpeed = new Vector2 (50f, 50f);
		public Vector2 rotateYLimit = new Vector2 (-80, 80);

		public Vector2 rayDistanceLimit = new Vector2 (10f, 200f);
		public float rayOrthographicScale = 50f;
		public float zoomRate = 10f;
		public float panRate = 1f;
		public float dampening = 20.0f;

		public Vector2 deltaFromGentlemanToNormal = new Vector2 (20, 13);

		private Vector2 degree;
		private bool canRotate = false;

		private Vector2 desiredPos;
		private Vector2 currentPos;

		[HideInInspector]
		public bool isDesired;
		[HideInInspector]
		public Vector3 desiredPosition;

		private float currentDistance;
		private float desiredDistance;
		private float rayDistance;

		private Quaternion currentRotation;
		private Quaternion desiredRotation;

		private Camera cameraScript;
		private CameraPerspectiveEditor cameraEditor;

		private Vector2 _degree;
		private Vector3 _positionG;
		private Vector3 _positionP;
		private float _size;

		void Start ()
		{
			cameraScript = GetComponent <Camera> ();
			cameraEditor = GetComponent<CameraPerspectiveEditor> ();
			_degree.x = degree.x = Vector3.Angle (Vector3.right, transform.right);
			_degree.y = degree.y = Vector3.Angle (Vector3.up, transform.up);
			_positionG = transform.position;
			_positionP = AdjustGentleman (2);
			_size = cameraScript.orthographicSize;
			SetProjectionScript ();
		}

		public void ResetTransform ()
		{
			degree.x = _degree.x;
			degree.y = _degree.y;
			isDesired = true;
			desiredPosition = (projection == CameraProjection.gentleman) ? _positionG : _positionP;
			cameraScript.orthographicSize = _size;
		}

		public void SetProjectionScript ()
		{
			if (cameraEditor != null) {
				if (projection == CameraProjection.gentleman && !cameraEditor.isActiveAndEnabled) {
					transform.position = AdjustGentleman (1);
				} else if (projection != CameraProjection.gentleman && cameraEditor.isActiveAndEnabled) {
					transform.position = AdjustGentleman (2);
				}
			}
			switch (projection) {
			case CameraProjection.perspective:
				cameraScript.orthographic = false;
				if (cameraEditor)
					cameraEditor.enabled = false;
				canRotate = true;
				break;
			case CameraProjection.orthographic:
				cameraScript.orthographic = true;
				if (cameraEditor)
					cameraEditor.enabled = false;
				canRotate = false;
				break;
			case CameraProjection.gentleman:
				cameraScript.orthographic = true;
				if (cameraEditor)
					cameraEditor.enabled = true;
				canRotate = false;
				break;
			}
		}

		Vector3 AdjustGentleman (int flag)
		{
//			Vector3 wx = transform.TransformDirection (Vector3.right) * deltaX;
//			Vector3 wy = transform.TransformDirection (Vector3.up) * deltaY;
			Vector3 wx = transform.TransformDirection (Vector3.right) * deltaFromGentlemanToNormal.x;
			Vector3 wy = transform.TransformDirection (Vector3.up) * deltaFromGentlemanToNormal.y;
			if (flag == 1) {
				return transform.position + wx + wy;
			} else if (flag == 2) {
				return transform.position - wx - wy;
			} else
				return transform.position;
		}

		void Update ()
		{
			if (Time.timeScale > 0) {
				UpdateRaycast ();
				if (canRotate && Input.GetMouseButton (1) || Input.GetKey (KeyCode.LeftAlt)) {
					degree.x += Input.GetAxis ("Mouse X") * rotateSpeed.x * 0.02f;
					degree.y -= Input.GetAxis ("Mouse Y") * rotateSpeed.y * 0.02f;
					degree.y = ClampAngle (degree.y, rotateYLimit.x, rotateYLimit.y);
					isDesired = false;
				} else if (Input.GetMouseButton (2) || Input.GetKey (KeyCode.Space)) {
					desiredPos.x = -Input.GetAxis ("Mouse X") * panRate * rayDistance * Time.deltaTime;
					desiredPos.y = -Input.GetAxis ("Mouse Y") * panRate * rayDistance * Time.deltaTime;
					isDesired = false;
				} else {
					desiredPos.x = desiredPos.y = 0;
				}

				// set camera rotation 
				desiredRotation = Quaternion.Euler (degree.y, degree.x, 0);
				currentRotation = transform.rotation;
				transform.rotation = Quaternion.Lerp (currentRotation, desiredRotation, Time.deltaTime * dampening);

				// set camera translation
				if (isDesired) {
					currentPos.x = currentPos.y = desiredPos.x = desiredPos.y = 0;
					float distance = Vector3.Distance (transform.position, desiredPosition);
					//transform.position = Vector3.MoveTowards (transform.position, desiredPosition, Time.deltaTime * distance / 0.3f);
					transform.position = desiredPosition;
					if (distance < 0.1f)
						isDesired = false;
				} else {
					currentPos.x = Mathf.Lerp (currentPos.x, desiredPos.x, Time.deltaTime * dampening);
					currentPos.y = Mathf.Lerp (currentPos.y, desiredPos.y, Time.deltaTime * dampening);
					transform.position += transform.TransformDirection (Vector3.right) * currentPos.x;
					transform.position += transform.TransformDirection (Vector3.up) * currentPos.y;
					desiredPosition = transform.position;
				}


				// set camera zooming
				if (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject ()) {	// mouse on ui dont zoom
					if (projection == CameraProjection.perspective) {
						desiredDistance = Input.GetAxis ("Mouse ScrollWheel") * zoomRate * rayDistance * Time.deltaTime * 10;
						currentDistance = Mathf.Lerp (currentDistance, desiredDistance, Time.deltaTime * dampening);
						transform.position += transform.rotation * Vector3.forward * currentDistance;
					} else {
						desiredDistance = Input.GetAxis ("Mouse ScrollWheel") * zoomRate * rayDistance * Time.deltaTime * 2;
						currentDistance = Mathf.Lerp (currentDistance, desiredDistance, Time.deltaTime * dampening);
						cameraScript.orthographicSize -= currentDistance;
						if (cameraScript.orthographicSize <= 0.1f)
							cameraScript.orthographicSize = 0.1f;
					}
				}
				if (Mathf.Abs (desiredDistance) != 0)
					isDesired = false;
			}
		}

		private static float ClampAngle (float angle, float min, float max)
		{
			if (angle < -360)
				angle += 360;
			if (angle > 360)
				angle -= 360;
			return Mathf.Clamp (angle, min, max);
		}



		void UpdateRaycast ()
		{
			Ray ray;
			if (cameraEditor == null || !cameraEditor.enabled) {
				ray = cameraScript.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
			} else {
				ray = cameraEditor.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
			}
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, rayDistanceLimit.y)) {
				rayDistance = hit.distance;
			} else {
				rayDistance = rayDistanceLimit.y;
			}

			if (projection == CameraProjection.gentleman || projection == CameraProjection.orthographic) {
				rayDistance = rayDistance * cameraScript.orthographicSize / rayOrthographicScale;
			}
			rayDistance = Mathf.Clamp (rayDistance, rayDistanceLimit.x, rayDistanceLimit.y);
		}
	}
}