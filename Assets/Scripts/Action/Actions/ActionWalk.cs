using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace DesignSociety
{
	public class ActionWalkTo : ActionSingle
	{
		public GameObject obj;
		public Landmark landmark;
		public IActionCompleted monitor;

		private bool finalRotate = false;
		private float animationSpeed = 1f;

		private NavmeshCut navCut;
		private Animator anim;
		private MyRichAI ai;

		private readonly int hashSpeedParaH = Animator.StringToHash ("SpeedH");
		private readonly int hashSpeedParaV = Animator.StringToHash ("SpeedV");

		public void Setting (GameObject obj, Landmark landmark, IActionCompleted monitor)
		{
			this.obj = obj;
			this.landmark = landmark;
			this.monitor = monitor;

			// animator
			anim = obj.GetComponent<Animator> ();

			// navcut (disable and force mesh update)
			navCut = GetComponent<NavmeshCut> ();
			if (navCut != null) {
				navCut.enabled = false;
				FindObjectOfType<TileHandlerHelper> ().ForceUpdate ();
			}

			// richAI (enable and add target reached listener)
			ai = obj.GetComponent<MyRichAI> ();
			ai.target = landmark;
			ai.enabled = true;
			ai.OnTargetReached += OnTargetReached;
		}

		void OnTargetReached (object sender, EventArgs e)
		{
			finalRotate = true;
		}

		void Update ()
		{
			float speed = ai.Velocity.magnitude;
			anim.SetFloat (hashSpeedParaH, speed);
			anim.speed = speed * animationSpeed;

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
			obj.transform.rotation = Quaternion.Euler (ran);
			return Mathf.Abs (ran.y - tan.y) < 1f;
		}

		public void Finish ()
		{
			ai.enabled = false;
			ai.OnTargetReached -= OnTargetReached;

			if (navCut != null) {
				navCut.enabled = true;
				FindObjectOfType<TileHandlerHelper> ().ForceUpdate ();
			}

			anim.speed = 1;
			anim.SetFloat (hashSpeedParaH, 0);

			if (monitor != null)
				monitor.OnActionCompleted (this);
			Free ();
		}
	}
}