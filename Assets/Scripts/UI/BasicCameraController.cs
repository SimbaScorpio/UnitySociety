using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasicCameraController : MonoBehaviour
{
	public enum CameraProjection
	{
		gentleman,
		perspective,
		orthographic
	}

	public CameraProjection projection;
	public float xSpeed = 50.0f;
	public float ySpeed = 50.0f;
	public int yMinLimit = -80;
	public int yMaxLimit = 80;
	public int zoomRate = 60;
	public float panRate = 20f;
	public float zoomDampening = 20.0f;

	public float deltaX = 20;
	public float deltaY = 13;

	private float xDeg = 0.0f;
	private float yDeg = 0.0f;
	private bool canRotate = false;

	private float desiredX;
	private float desiredY;
	private float currentX;
	private float currentY;
	public bool isDesired;
	public Vector3 desiredPosition;

	private float currentDistance;
	private float desiredDistance;

	private Quaternion currentRotation;
	private Quaternion desiredRotation;

	private Camera cameraScript;
	private CameraPerspectiveEditor cameraEditor;

	private float _xDeg = 0.0f;
	private float _yDeg = 0.0f;
	private Vector3 _positionG;
	private Vector3 _positionP;
	private float _size;

	void Start ()
	{
		cameraScript = GetComponent <Camera> ();
		cameraEditor = GetComponent<CameraPerspectiveEditor> ();
		_xDeg = xDeg = Vector3.Angle (Vector3.right, transform.right);
		_yDeg = yDeg = Vector3.Angle (Vector3.up, transform.up);
		_positionG = transform.position;
		_positionP = AdjustGentleman (2);
		_size = cameraScript.orthographicSize;
		SetProjectionScript ();
	}

	public void ResetTransform ()
	{
		xDeg = _xDeg;
		yDeg = _yDeg;
		isDesired = true;
		desiredPosition = (projection == CameraProjection.gentleman) ? _positionG : _positionP;
		cameraScript.orthographicSize = _size;
	}

	public void SetProjectionScript ()
	{
		if (projection == CameraProjection.gentleman && !cameraEditor.isActiveAndEnabled) {
			transform.position = AdjustGentleman (1);
		} else if (projection != CameraProjection.gentleman && cameraEditor.isActiveAndEnabled) {
			transform.position = AdjustGentleman (2);
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
		Vector3 wx = transform.TransformDirection (Vector3.right) * deltaX;
		Vector3 wy = transform.TransformDirection (Vector3.up) * deltaY;
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
			if (canRotate && Input.GetMouseButton (1) || Input.GetKey (KeyCode.LeftAlt)) {
				xDeg += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
				yDeg -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;
				yDeg = ClampAngle (yDeg, yMinLimit, yMaxLimit);
				isDesired = false;
			} else if (Input.GetMouseButton (2) || Input.GetKey (KeyCode.Space)) {
				desiredX = -Input.GetAxis ("Mouse X") * panRate * Time.deltaTime;
				desiredY = -Input.GetAxis ("Mouse Y") * panRate * Time.deltaTime;
				isDesired = false;
			} else {
				desiredX = desiredY = 0;
			}

			// set camera rotation 
			desiredRotation = Quaternion.Euler (yDeg, xDeg, 0);
			currentRotation = transform.rotation;
			transform.rotation = Quaternion.Lerp (currentRotation, desiredRotation, Time.deltaTime * zoomDampening);

			// set camera translation
			if (isDesired) {
				currentX = currentY = desiredX = desiredY = 0;
				float distance = Vector3.Distance (transform.position, desiredPosition);
				//transform.position = Vector3.MoveTowards (transform.position, desiredPosition, Time.deltaTime * distance / 0.3f);
				transform.position = desiredPosition;
				if (distance < 0.1f)
					isDesired = false;
			} else {
				currentX = Mathf.Lerp (currentX, desiredX, Time.deltaTime * zoomDampening);
				currentY = Mathf.Lerp (currentY, desiredY, Time.deltaTime * zoomDampening);
				transform.position += transform.TransformDirection (Vector3.right) * currentX;
				transform.position += transform.TransformDirection (Vector3.up) * currentY;
				desiredPosition = transform.position;
			}

			// set camera zooming
			if (!EventSystem.current.IsPointerOverGameObject ()) {	// mouse on ui dont zoom
				if (projection == CameraProjection.perspective) {
					desiredDistance = Input.GetAxis ("Mouse ScrollWheel") * zoomRate * Time.deltaTime * 10;
					currentDistance = Mathf.Lerp (currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
					transform.position += transform.rotation * Vector3.forward * currentDistance;
				} else {
					desiredDistance = Input.GetAxis ("Mouse ScrollWheel") * zoomRate * Time.deltaTime * 2;
					currentDistance = Mathf.Lerp (currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
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
}
