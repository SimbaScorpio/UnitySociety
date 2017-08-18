using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCameraController : MonoBehaviour
{
	public float xSpeed = 100.0f;
	public float ySpeed = 100.0f;
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
	private float currentDistance;
	private float desiredDistance;
	private Quaternion currentRotation;
	private Quaternion desiredRotation;
	private Quaternion rotation;

	void Start ()
	{
		Init ();
	}

	void OnEnable ()
	{
		Init ();
	}

	public void Init ()
	{
		xDeg = Vector3.Angle (Vector3.right, transform.right);
		yDeg = Vector3.Angle (Vector3.up, transform.up);
	}

	void Update ()
	{
		if (Input.GetMouseButton (1)) {
			xDeg += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
			yDeg -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;
			yDeg = ClampAngle (yDeg, yMinLimit, yMaxLimit);
		} else if (Input.GetMouseButton (2)) {
			desiredX = -Input.GetAxis ("Mouse X") * panRate * Time.deltaTime;
			desiredY = -Input.GetAxis ("Mouse Y") * panRate * Time.deltaTime;
		} else {
			desiredX = desiredY = 0;
		}

		// set camera rotation 
		desiredRotation = Quaternion.Euler (yDeg, xDeg, 0);
		currentRotation = transform.rotation;
		rotation = Quaternion.Lerp (currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
		transform.rotation = rotation;

		// set camera translation
		currentX = Mathf.Lerp (currentX, desiredX, Time.deltaTime * zoomDampening);
		currentY = Mathf.Lerp (currentY, desiredY, Time.deltaTime * zoomDampening);
		transform.position += transform.TransformDirection (Vector3.right) * currentX;
		transform.position += transform.TransformDirection (Vector3.up) * currentY;

		// set camera zooming
		desiredDistance = Input.GetAxis ("Mouse ScrollWheel") * zoomRate * Time.deltaTime * 10;
		currentDistance = Mathf.Lerp (currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
		transform.position += rotation * Vector3.forward * currentDistance;
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
