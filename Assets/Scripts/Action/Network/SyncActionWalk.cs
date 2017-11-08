using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace DesignSociety
{
	public class SyncActionWalk : NetworkActionPlay
	{
		public Landmark landmark;

		private bool finalRotate = false;
		private float animationSpeed = 1.5f;

		private NavmeshCut navCut;
		[HideInInspector]
		public MyRichAI ai;

		private float lastH, speedV = 0f, speedH = 0f;
		private float initAISpeed;

		private readonly int hashSpeedParaH = Animator.StringToHash ("SpeedH");
		private readonly int hashSpeedParaV = Animator.StringToHash ("SpeedV");

		public void Setting (string stateName, Landmark landmark, IActionCompleted monitor)
		{
			this.landmark = landmark;
			lastH = transform.position.y;

			// navcut (disable and force mesh update)
			navCut = GetComponent<NavmeshCut> ();
			if (navCut != null) {
				navCut.enabled = false;
				FindObjectOfType<TileHandlerHelper> ().ForceUpdate ();
			}

			// richAI (enable and add target reached listener)
			ai = GetComponent<MyRichAI> ();
			ai.target = landmark;	// make sure target set before enabled
			ai.enabled = true;
			ai.OnTargetReached += OnTargetReached;

			initAISpeed = ai.maxSpeed;

			ActionInfo info = new ActionInfo (stateName, null, null, StuffType.BigStuff);
			Setting (info, monitor);
		}

		void OnTargetReached (object sender, EventArgs e)
		{
			finalRotate = true;
		}

		void Update ()
		{
			speedH = ai.Velocity.magnitude;
			speedV = Mathf.Lerp (speedV, (transform.position.y - lastH) / Time.deltaTime, Time.deltaTime * 5);
			anim.SetFloat (hashSpeedParaH, speedH);
			anim.SetFloat (hashSpeedParaV, speedV);
			anim.speed = speedH * animationSpeed;
			GetComponent<NetworkActionDealer> ().SyncActionSpeed (anim.speed);

			lastH = transform.position.y;

			DetectFrontPerson ();
			if (finalRotate && FinalRotate ())
				Finish ();
		}

		bool FinalRotate ()
		{
			//return true;
			Vector3 tan = landmark.rotation.eulerAngles;
			Vector3 ran = transform.rotation.eulerAngles;
			float deltaTime = Mathf.Min (Time.smoothDeltaTime * 2, Time.deltaTime);
			ran.y = Mathf.MoveTowardsAngle (ran.y, tan.y, ai.rotationSpeed * deltaTime);
			transform.rotation = Quaternion.Euler (ran);
			return Mathf.Abs (ran.y - tan.y) < 1f;
		}

		public override void Finish ()
		{
			ai.OnTargetReached -= OnTargetReached;
			ai.enabled = false;
			ai.maxSpeed = initAISpeed;

			if (navCut != null) {
				navCut.enabled = true;
				FindObjectOfType<TileHandlerHelper> ().ForceUpdate ();
			}

			anim.speed = 1;
			GetComponent<NetworkActionDealer> ().SyncActionSpeed (anim.speed);
			anim.SetFloat (hashSpeedParaH, 0);
			anim.SetFloat (hashSpeedParaV, 0);

			base.Finish ();
		}

		void DetectFrontPerson ()
		{
			Vector3 start = transform.position;
			start.y += 1f;

			Ray ray1 = new Ray (start, transform.TransformDirection (Vector3.forward));
			Debug.DrawLine (ray1.origin, ray1.origin + ray1.direction, Color.blue);
			Ray ray2 = new Ray (start, transform.TransformDirection ((Vector3.forward + Vector3.right) / 2f));
			Debug.DrawLine (ray2.origin, ray2.origin + ray2.direction, Color.blue);
			Ray ray3 = new Ray (start, transform.TransformDirection ((Vector3.forward + Vector3.left) / 2f));
			Debug.DrawLine (ray3.origin, ray3.origin + ray3.direction, Color.blue);

			RaycastHit hit;
			bool flag = false;
			if (!flag && Physics.Raycast (ray1, out hit, 1f)) {
				if (hit.collider.tag == "Player") {
					flag = DealWithDetect (hit.collider);
				}
			} else if (!flag && Physics.Raycast (ray2, out hit, 1f)) {
				if (hit.collider.tag == "Player") {
					flag = DealWithDetect (hit.collider);
				}
			} else if (!flag && Physics.Raycast (ray3, out hit, 1f)) {
				if (hit.collider.tag == "Player") {
					flag = DealWithDetect (hit.collider);
				}
			}
			if (!flag) {
				ai.maxSpeed = initAISpeed;
			}
		}

		bool DealWithDetect (Collider collider)
		{
			SyncActionWalk sw = collider.GetComponent<SyncActionWalk> ();
			if (sw != null) {
				Vector3 v13 = this.ai.Velocity;
				Vector2 v12 = new Vector2 (v13.x, v13.z);
				Vector3 v23 = sw.ai.Velocity;
				Vector2 v22 = new Vector2 (v23.x, v23.z);
				if (Vector2.Dot (v12, v22) > 0) {
					ai.maxSpeed = initAISpeed - 0.5f;
				} else {
					ai.maxSpeed = initAISpeed + 0.5f;
				}
				return true;
			}
			return false;
		}
	}
}