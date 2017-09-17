using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoveTest : MonoBehaviour
{
	public float Radius = 10f;
	public float Speed = 2f;
	private Vector3 originPosition;
	private Vector3 targetPosition;

	// Use this for initialization
	void Start ()
	{
		originPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Vector3.Distance (transform.position, targetPosition) < 0.1f) {
			targetPosition = RandomPosition ();
		} else {
			transform.position = Vector3.MoveTowards (transform.position, targetPosition, Speed * Time.deltaTime);
		}
	}

	Vector3 RandomPosition ()
	{
		return originPosition + Random.insideUnitSphere * Radius;
	}
}
