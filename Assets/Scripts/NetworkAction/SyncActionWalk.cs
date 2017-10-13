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
		private float animationSpeed = 1.1f;

		private NavmeshCut navCut;
		private MyRichAI ai;

		private float lastH, speedV = 0f, speedH = 0f;

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

			ActionInfo info = new ActionInfo (stateName, null, null, null, StuffType.BigStuff);
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

			lastH = transform.position.y;

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

			if (navCut != null) {
				navCut.enabled = true;
				FindObjectOfType<TileHandlerHelper> ().ForceUpdate ();
			}

			anim.speed = 1;
			anim.SetFloat (hashSpeedParaH, 0);
			anim.SetFloat (hashSpeedParaV, 0);

			base.Finish ();
		}
	}
}