using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace DesignSociety
{
	[RequireComponent (typeof(MyRichAI))]
	public class CarMovement : MonoBehaviour
	{
		public TrafficRoute tr;
		public float d = 3f;
		public CarMovement waitForCar;

		private float initAISpeed;
		private int currentIndex;
		private Landmark lm = new Landmark ();

		private List<Transform> wheels = new List<Transform> ();

		private MyRichAI ai;

		void Start ()
		{
			ai = GetComponent<MyRichAI> ();
			ai.enabled = true;
			ai.OnTargetReached += OnTargetReached;
			initAISpeed = ai.maxSpeed;
			FindWheels ();
			FindStartIndex ();
			GoToNextCorner ();
		}

		void FindWheels ()
		{
			for (int i = 0; i < 8; ++i) {
				Transform temp = transform.Find ("tire_" + i);
				if (temp != null)
					wheels.Add (temp);
			}
		}

		void FindStartIndex ()
		{
			float min = float.MaxValue;
			float distance;
			for (int i = 0; i < tr.corners.Count; ++i) {
				distance = Vector3.Distance (tr.corners [i], transform.position);
				if (distance < min) {
					min = distance;
					currentIndex = i;
				}
			}
		}

		void OnTargetReached (object sender, EventArgs e)
		{
			GoToNextCorner ();
		}

		void GoToNextCorner ()
		{
			currentIndex = (currentIndex + 1) % tr.corners.Count;
			lm.Set (tr.corners [currentIndex]);
			ai.target = lm;
			if (!ai.repeatedlySearchPaths)
				ai.UpdatePath ();
		}

		void Update ()
		{
			DetectObstacle ();
			AnimateWheel ();
		}

		private Vector3 angle = Vector3.zero;

		void AnimateWheel ()
		{
			angle.x = Time.deltaTime * ai.Velocity.magnitude * 180f;
			for (int i = 0; i < wheels.Count; ++i) {
				wheels [i].Rotate (angle);
			}
		}

		#region 探测

		void DetectObstacle ()
		{
			Vector3 start = transform.position;
			start.y += 1f;

			Ray ray1 = new Ray (start, transform.TransformDirection (Vector3.forward));
			Debug.DrawLine (ray1.origin, ray1.origin + ray1.direction * d, Color.blue);
			Ray ray2 = new Ray (start, transform.TransformDirection ((Vector3.forward + Vector3.right / 5f)));
			Debug.DrawLine (ray2.origin, ray2.origin + ray2.direction * d, Color.blue);
			Ray ray3 = new Ray (start, transform.TransformDirection ((Vector3.forward + Vector3.left / 5f)));
			Debug.DrawLine (ray3.origin, ray3.origin + ray3.direction * d, Color.blue);

			if (!Raycast (ray1) && !Raycast (ray2) && !Raycast (ray3)) {
				ai.maxSpeed = initAISpeed;
				waitForCar = null;
			}
		}

		bool Raycast (Ray ray)
		{
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, d)) {
				if (hit.collider.tag == "Car") {
					return DealWithCarDetect (hit.collider);
				} else if (hit.collider.tag == "Player") {
					return DealWithPlayerDetect (hit.collider);
				}
			}
			return false;
		}

		bool DealWithPlayerDetect (Collider collider)
		{
			ai.maxSpeed = 0;
			waitForCar = null;
			return true;
		}

		bool DealWithCarDetect (Collider collider)
		{
			CarMovement sw = collider.GetComponent<CarMovement> ();
			if (sw == null)
				return false;
			Vector3 v1 = sw.ai.Velocity;
			Vector3 v2 = ai.Velocity;
			if (Vector3.Dot (v1, v2) < 0)
				return false;

			CarMovement temp = sw;
			while (temp.waitForCar != null) {
				temp = temp.waitForCar;
				if (temp == this)
					return false;
			}
			waitForCar = sw;
			ai.maxSpeed = 0;
			return true;
		}

		#endregion
	}
}