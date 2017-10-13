using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionInfo
	{
		public string stateName;
		public string[] itemPaths;
		// tag for obj in scene
		public string tag;
		// name for instantiate item
		public string prefabName;
		// type for stuff in scene
		public StuffType stuffType;

		public ActionInfo (string stateName, string[] itemPaths, string tag, string prefabName, StuffType type)
		{
			this.stateName = stateName;
			this.itemPaths = itemPaths;
			this.tag = tag;
			this.prefabName = prefabName;
			this.stuffType = type;
		}
	}

	/// <summary>
	/// Network action play.
	/// 1) items are totally owned by player
	/// 2) items should grab from scene, but act like player's own, finally release to scene
	/// 3) items are created by player and released into scene
	/// 4) stuff always remain in scene
	/// </summary>
	/// 
	public class NetworkActionPlay : ActionSingle
	{
		public ActionInfo info;
		protected IActionCompleted monitor;
		protected Animator anim;
		private bool hasFinished = false;

		#region basic

		public virtual void Setting (ActionInfo info, IActionCompleted callback)
		{
			this.info = info;
			this.monitor = callback;
			anim = GetComponent<Animator> ();
			anim.Play (info.stateName, 0, 0f);
		}

		void Update ()
		{
			if (!string.IsNullOrEmpty (info.stateName)) {
				if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1 || !anim.GetCurrentAnimatorStateInfo (0).IsName (info.stateName)) {
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

		public void SafeFinish ()
		{
			monitor = null;
			this.Finish ();
		}

		void OnDestroy ()
		{
			if (!hasFinished) {
				hasFinished = true;
				Finish ();
			}
		}

		#endregion




		#region callbacks

		public void OnItemsShown ()
		{
			
		}

		public void OnItemsHidden ()
		{
			
		}

		public void OnItemGained ()
		{
			// search item in scene and rename it under certain path
		}

		public void OnItemReleased ()
		{
			
		}

		public void OnStuffPickedUp ()
		{
			
		}

		public void OnStuffPutDown ()
		{
			
		}

		public void OnStuffPutAway ()
		{
			
		}

		public void OnStuffGivenTo ()
		{
			
		}

		public void OnStuffTakenFrom ()
		{
			
		}

		#endregion
	}
}