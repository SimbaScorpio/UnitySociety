﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private float xDeg = 0.0f;
	private float yDeg = 0.0f;

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
	private Vector3 _position;
	private float _size;

	void Start ()
	{
		cameraScript = GetComponent <Camera> ();
		cameraEditor = GetComponent<CameraPerspectiveEditor> ();
		_xDeg = xDeg = Vector3.Angle (Vector3.right, transform.right);
		_yDeg = yDeg = Vector3.Angle (Vector3.up, transform.up);
		_position = transform.position;
		_size = cameraScript.orthographicSize;
		SetProjectionScript ();
	}

	public void ResetTransform ()
	{
		xDeg = _xDeg;
		yDeg = _yDeg;
		isDesired = true;
		desiredPosition = _position;
		cameraScript.orthographicSize = _size;
	}

	public void SetProjectionScript ()
	{
		switch (projection) {
		case CameraProjection.perspective:
			cameraScript.orthographic = false;
			if (cameraEditor)
				cameraEditor.enabled = false;
			break;
		case CameraProjection.orthographic:
			cameraScript.orthographic = true;
			if (cameraEditor)
				cameraEditor.enabled = false;
			break;
		case CameraProjection.gentleman:
			cameraScript.orthographic = true;
			if (cameraEditor)
				cameraEditor.enabled = true;
			break;
		}
	}

	void Update ()
	{
		if (Time.timeScale > 0) {
			if (Input.GetMouseButton (1) || Input.GetKey (KeyCode.LeftAlt)) {
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
				transform.position = Vector3.MoveTowards (transform.position, desiredPosition, Time.deltaTime * distance / 0.3f);
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
