using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class ActionDealer : MonoBehaviour, IActionCompleted
	{
		public bool isStanding;
		//public bool isUsingTelephone;
		public List<Stuff> holdingStuffs = new List<Stuff> ();

		private string tryingToDoActionName;
		private IActionCompleted monitor;

		private float nextTimeToConsider = 1.0f;
		private Coroutine handler;
		private bool aidActive;

		void Start ()
		{
			isStanding = true;
			//isUsingTelephone = false; 
		}


		#region 随机动作判断

		public void StartCountingAid ()
		{
			if (handler != null)
				return;
			aidActive = false;
			handler = StartCoroutine (HandleAidActivity ());
		}

		public void StopCountingAid ()
		{
			if (handler != null) {
				StopCoroutine (handler);
				handler = null;
			}
			aidActive = false;
		}

		public bool IsAidActive ()
		{
			return aidActive;
		}

		IEnumerator HandleAidActivity ()
		{
			while (!aidActive) {
				float p = StorylineManager.GetInstance ().storyline.aid_possibility;
				int aidPossiblity = Random.Range (0, (int)(1 / p));
				if (aidPossiblity == 0) {
					aidActive = true;
					handler = null;
				} else
					yield return new WaitForSeconds (nextTimeToConsider);
			}
		}

		#endregion


		public bool TryNotStanding (IActionCompleted callback)
		{
			if (isStanding) {
				ApplyAction ("坐下", callback);
				return false;
			}
			return true;
		}

		public bool TryNotSitting (IActionCompleted callback)
		{
			if (!isStanding) {
				ApplyAction ("起立", callback);
				return false;
			}
			return true;
		}

		//	public bool TryNotUsingTelephone (IActionCompleted callback)
		//	{
		//		if (isUsingTelephone) {
		//			ApplyAction ("放下电话", callback);
		//			return false;
		//		}
		//		return true;
		//	}


		public void OnActionCompleted (Action ac)
		{
			if (!string.IsNullOrEmpty (tryingToDoActionName)) {
				ApproachAction (tryingToDoActionName, monitor);
			}
		}

		public void ApproachAction (string name, IActionCompleted callback)
		{
			if (string.IsNullOrEmpty (name))
				return;
			tryingToDoActionName = name;
			monitor = callback;
//			if (ActionName.IsStandAction (name)) {
//				if (isStanding) {
//					tryingToDoActionName = null;
//					ApplyAction (name, monitor);
//				} else {
//					ApplyAction ("起立", this);
//				}
//			} else if (ActionName.IsSitAction (name)) {
//				if (!isStanding) {
//					tryingToDoActionName = null;
//					ApplyAction (name, monitor);
//				} else {
//					ApplyAction ("坐下", this);
//				}
//			}
		}


		void ApplyAction (string name, IActionCompleted callback)
		{
			switch (name) {
			case "坐下":
				if (isStanding) {
					isStanding = false;
					ActionManager.GetInstance ().ApplyTriggerAction (gameObject, name, callback);
				} else {
					if (callback != null)
						callback.OnActionCompleted (null);
				}
				break;
			case "起立":
				if (!isStanding) {
					isStanding = true;
					ActionManager.GetInstance ().ApplyTriggerAction (gameObject, name, callback);
				} else {
					if (callback != null)
						callback.OnActionCompleted (null);
				}
				break;
			case "坐着不动":
				ActionManager.GetInstance ().ApplyIdleAction (gameObject, name, Random.Range (0.1f, 0.5f), callback);
				break;
			case "站着不动":
				ActionManager.GetInstance ().ApplyIdleAction (gameObject, name, Random.Range (0.1f, 0.5f), callback);
				break;
			case "点击鼠标":
				ActionManager.GetInstance ().ApplyClickAction (gameObject, name, callback);
				break;
			case "发言":
				ActionManager.GetInstance ().ApplySpeechAction (gameObject, name, callback);
				break;
			case "使用相机":
				ActionManager.GetInstance ().ApplyUseCameraAction (gameObject, name, callback);
				break;
			case "传纸":
				ActionManager.GetInstance ().ApplyGivePaperAction (gameObject, name, callback);
				break;
			case "接纸":
				ActionManager.GetInstance ().ApplyGetPaperAction (gameObject, name, callback);
				break;
			case "拿手机扫码":
				ActionManager.GetInstance ().ApplyCellphoneScanAction (gameObject, name, callback);
				break;
			case "听写记录":
				ActionManager.GetInstance ().ApplyTakeNoteAction (gameObject, name, callback);
				break;
			case "VR眼镜":
				ActionManager.GetInstance ().ApplyVRGlassesAction (gameObject, name, callback);
				break;
			default:
				ActionManager.GetInstance ().ApplyTriggerAction (gameObject, name, callback);
				break;
			}
		}
	}
}