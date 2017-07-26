using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraShift : MonoBehaviour
{
	public float shiftDistance = 50;
	public int index = 1;
	public int maxNum = 3;
	public float speed = 0.5f;
	private Vector3 origin;
	private Vector3 destination;

	void Start ()
	{
		origin = Camera.main.transform.position;
		destination = origin;
	}

	public void OnShiftLeftButtonClicked ()
	{
		if (index <= 1)
			return;
		index--;
		destination = origin - new Vector3 ((index - 1) * shiftDistance, 0, 0);
	}

	public void OnShiftRightButtonClicked ()
	{
		if (index >= 3)
			return;
		index++;
		destination = origin - new Vector3 ((index - 1) * shiftDistance, 0, 0);
	}

	void Update ()
	{
		if (destination != Camera.main.transform.position) {
			Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, destination, speed);
		}
	}
}
