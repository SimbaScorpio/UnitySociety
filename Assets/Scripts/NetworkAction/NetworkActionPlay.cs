using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class NetworkActionPlay : ActionSingle
	{
		public string stateName;
		protected IActionCompleted monitor;
		protected Animator anim;
		private bool hasFinished = false;

		public virtual void Setting (string name, IActionCompleted callback)
		{
			this.stateName = name;
			this.monitor = callback;
			anim = GetComponent<Animator> ();
			anim.Play (name, 0, 0f);
		}

		void Update ()
		{
			if (!string.IsNullOrEmpty (stateName)) {
				if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1 || !anim.GetCurrentAnimatorStateInfo (0).IsName (stateName)) {
					Finish ();
				}
			}
		}

		public virtual void Finish ()
		{
			if (monitor != null) {
				monitor.OnActionCompleted (this);
			}
			if (!hasFinished) {
				hasFinished = true;
				Free ();
			}
		}

		void OnDestroy ()
		{
			if (!hasFinished) {
				hasFinished = true;
				Finish ();
			}
		}
	}
}