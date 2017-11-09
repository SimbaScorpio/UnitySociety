using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace DesignSociety
{
	[RequireComponent (typeof(MyRichAI))]
	public class CarMovement : MonoBehaviour
	{
		public MyRichAI ai;
		private float error = 5f;

		private List<Vector3> corners = new List<Vector3> ();
		private Vector3 node;
		private bool isTicking;
		private float initAISpeed;

		public void StartSearching (List<Vector3> nodes, Vector3 end)
		{
			if (nodes == null || nodes.Count == 0)
				return;
			for (int i = 0; i < nodes.Count; ++i) {
				corners.Add (nodes [i]);
			}
			node = end;
			ai = GetComponent<MyRichAI> ();
			initAISpeed = ai.maxSpeed;
			RecursiveSearch ();
			ai.enabled = true;
			isTicking = true;
		}

		void RecursiveSearch ()
		{
			// 如果存在节点，去随机的，反之去结束点
			if (corners.Count == 0) {
				ai.target = V (node);
			} else {
				int index = Random.Range (0, corners.Count);
				ai.target = V (corners [index]);
				corners.RemoveAt (index);
			}
			ai.UpdatePath ();
		}

		Landmark V (Vector3 vec)
		{
			Landmark m = new Landmark ();
			m.m_data [0] = vec.x;
			m.m_data [1] = vec.y;
			m.m_data [2] = vec.z;
			return m;
		}

		void Update ()
		{
			if (isTicking) {
				DetectFrontCar (3);
				for (int i = 0; i < corners.Count; ++i) {
					if (Vector3.Distance (transform.position, corners [i]) < error) {
						corners.RemoveAt (i--);
					}
				}
				if (Vector3.Distance (transform.position, ai.target.position) < error) {
					if (Vector3.Distance (ai.target.position, node) < error) {
						isTicking = false;
						ai.enabled = false;
						TrafficManager.GetInstance ().RemoveCar (gameObject, node);
					} else {
						RecursiveSearch ();
					}
				}
			}
		}

		void DetectFrontCar (float d)
		{
			Vector3 start = transform.position;
			start.y += 1f;

			Ray ray1 = new Ray (start, transform.TransformDirection (Vector3.forward));
			Debug.DrawLine (ray1.origin, ray1.origin + ray1.direction * d, Color.blue);
			Ray ray2 = new Ray (start, transform.TransformDirection ((Vector3.forward + Vector3.right / 2f)));
			Debug.DrawLine (ray2.origin, ray2.origin + ray2.direction * d, Color.blue);
			Ray ray3 = new Ray (start, transform.TransformDirection ((Vector3.forward + Vector3.left / 2f)));
			Debug.DrawLine (ray3.origin, ray3.origin + ray3.direction * d, Color.blue);

			RaycastHit hit;
			bool flag = false;
			if (!flag && Physics.Raycast (ray1, out hit, d)) {
				if (hit.collider.tag == "Car") {
					flag = DealWithDetect (hit.collider);
				}
			} else if (!flag && Physics.Raycast (ray2, out hit, d)) {
				if (hit.collider.tag == "Car") {
					flag = DealWithDetect (hit.collider);
				}
			} else if (!flag && Physics.Raycast (ray3, out hit, d)) {
				if (hit.collider.tag == "Car") {
					flag = DealWithDetect (hit.collider);
				}
			}
			if (!flag) {
				ai.maxSpeed = initAISpeed;
			}
		}

		bool DealWithDetect (Collider collider)
		{
			CarMovement sw = collider.GetComponent<CarMovement> ();
			if (sw != null) {
				Vector3 v13 = this.ai.Velocity;
				Vector2 v12 = new Vector2 (v13.x, v13.z);
				Vector3 v23 = sw.ai.Velocity;
				Vector2 v22 = new Vector2 (v23.x, v23.z);
				if (Vector2.Dot (v12, v22) > 0) {
					ai.maxSpeed = initAISpeed * 0.5f;
				} else {
					ai.maxSpeed = initAISpeed * 1.2f;
				}
				return true;
			}
			return false;
		}
	}
}