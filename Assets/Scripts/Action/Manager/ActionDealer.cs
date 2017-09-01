using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDealer : MonoBehaviour, IActionCompleted
{
	public bool isStanding;
	public bool isUsingTelephone;
	private string tryingToDoActionName;
	private IActionCompleted monitor;

	private string[] standActions = {
		"站着不动", "站姿使用电话", "站姿拨打电话", "发言", "使用相机", "站立听", "站立鼓掌", "站立说话", "边说边指远方", "边说边指近桌", "边说边对桌子指指点点", "站立指ppt", "传纸", "接纸"
	};

	private string[] sitActions = {
		"坐着不动", "坐姿使用电话", "坐姿拨打电话", "敲击键盘", "点击鼠标", "挠头思考", "托腮思考", "坐向后仰", "坐抱头后仰"
	};

	private string[] telephoneActions = {
		"站姿使用电话", "站姿拨打电话", "坐姿使用电话", "坐姿拨打电话" 
	};

	void Start ()
	{
		isStanding = true;
		isUsingTelephone = false; 
	}

	public bool TryNotStanding (IActionCompleted callback)
	{
		if (TryNotUsingTelephone (callback)) {
			if (isStanding) {
				ApplyAction ("坐下", callback);
				return false;
			}
			return true;
		}
		return false;
	}

	public bool TryNotSitting (IActionCompleted callback)
	{
		if (TryNotUsingTelephone (callback)) {
			if (!isStanding) {
				ApplyAction ("起立", callback);
				return false;
			}
			return true;
		}
		return false;
	}

	public bool TryNotUsingTelephone (IActionCompleted callback)
	{
		if (isUsingTelephone) {
			ApplyAction ("放下电话", callback);
			return false;
		}
		return true;
	}


	public bool IsStandAction (string name)
	{
		foreach (string ac in standActions) {
			if (ac == name)
				return true;
		}
		return false;
	}

	public bool IsSitAction (string name)
	{
		foreach (string ac in sitActions) {
			if (ac == name)
				return true;
		}
		return false;
	}

	public bool IsTelephoneAction (string name)
	{
		foreach (string ac in telephoneActions) {
			if (ac == name)
				return true;
		}
		return false;
	}




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
		if (IsStandAction (name)) {
			if (isStanding) {
				if (IsTelephoneAction (name)) {
					if (isUsingTelephone) {
						tryingToDoActionName = null;
						ApplyAction (name, monitor);
					} else {
						ApplyAction ("拿起电话", this);
					}
				} else {
					tryingToDoActionName = null;
					ApplyAction (name, monitor);
				}
			} else {
				if (isUsingTelephone) {
					ApplyAction ("放下电话", this);
				} else {
					ApplyAction ("起立", this);
				}
			}
		} else if (IsSitAction (name)) {
			if (!isStanding) {
				if (IsTelephoneAction (name)) {
					if (isUsingTelephone) {
						tryingToDoActionName = null;
						ApplyAction (name, monitor);
					} else {
						ApplyAction ("放下电话", this);
					}
				} else {
					tryingToDoActionName = null;
					ApplyAction (name, monitor);
				}
			} else {
				if (isUsingTelephone) {
					ApplyAction ("放下电话", this);
				} else {
					ApplyAction ("坐下", this);
				}
			}
		}
	}




	void ApplyAction (string name, IActionCompleted callback)
	{
		switch (name) {
		case "坐下":
			if (isStanding) {
				isStanding = false;
				ActionManager.GetInstance ().ApplySitDownAction (gameObject, callback);
			} else {
				if (callback != null)
					callback.OnActionCompleted (null);
			}
			break;
		case "起立":
			if (!isStanding) {
				isStanding = true;
				ActionManager.GetInstance ().ApplyStandUpAction (gameObject, callback);
			} else {
				if (callback != null)
					callback.OnActionCompleted (null);
			}
			break;
		case "放下电话":
			if (isUsingTelephone) {
				isUsingTelephone = false;
				ActionManager.GetInstance ().ApplyPutDownTelephoneAction (gameObject, callback);
			} else {
				if (callback != null)
					callback.OnActionCompleted (null);
			}
			break;
		case "拿起电话":
			if (!isUsingTelephone) {
				isUsingTelephone = true;
				ActionManager.GetInstance ().ApplyPickUpTelephoneAction (gameObject, callback);
			} else {
				if (callback != null)
					callback.OnActionCompleted (null);
			}
			break;
		case "坐着不动":
			ActionManager.GetInstance ().ApplyIdleAction (gameObject, callback);
			break;
		case "站着不动":
			ActionManager.GetInstance ().ApplyIdleAction (gameObject, callback);
			break;
		case "敲击键盘":
			ActionManager.GetInstance ().ApplyTypeAction (gameObject, callback);
			break;
		case "点击鼠标":
			ActionManager.GetInstance ().ApplyClickAction (gameObject, callback);
			break;
		case "挠头思考":
			ActionManager.GetInstance ().ApplyScratchHeadAction (gameObject, callback);
			break;
		case "托腮思考":
			ActionManager.GetInstance ().ApplyHandOnChinAction (gameObject, callback);
			break;
		case "发言":
			ActionManager.GetInstance ().ApplySpeechAction (gameObject, callback);
			break;
		case "使用相机":
			ActionManager.GetInstance ().ApplyUseCameraAction (gameObject, callback);
			break;
		case "站姿拨打电话":
			ActionManager.GetInstance ().ApplyDialTelephoneAction (gameObject, callback);
			break;
		case "坐姿拨打电话":
			ActionManager.GetInstance ().ApplyDialTelephoneAction (gameObject, callback);
			break;
		case "站姿使用电话":
			ActionManager.GetInstance ().ApplyUseTelephoneAction (gameObject, callback);
			break;
		case "坐姿使用电话":
			ActionManager.GetInstance ().ApplyUseTelephoneAction (gameObject, callback);
			break;
		case "站立听":
			ActionManager.GetInstance ().ApplyStandListenAction (gameObject, callback);
			break;
		case "站立鼓掌":
			ActionManager.GetInstance ().ApplyStandClapAction (gameObject, callback);
			break;
		case "站立说话":
			ActionManager.GetInstance ().ApplyStandSpeakAction (gameObject, callback);
			break;
		case "边说边指远方":
			ActionManager.GetInstance ().ApplySpeakAndPointFarAction (gameObject, callback);
			break;
		case "边说边指近桌":
			ActionManager.GetInstance ().ApplySpeakAndPointNearDeskAction (gameObject, callback);
			break;
		case "边说边对桌子指指点点":
			ActionManager.GetInstance ().ApplySpeakAndPointDeskAction (gameObject, callback);
			break;
		case "站立指ppt":
			ActionManager.GetInstance ().ApplyStandPointPPTAction (gameObject, callback);
			break;
		case "传纸":
			ActionManager.GetInstance ().ApplyGivePaperAction (gameObject, callback);
			break;
		case "接纸":
			ActionManager.GetInstance ().ApplyGetPaperAction (gameObject, callback);
			break;
		case "坐向后仰":
			ActionManager.GetInstance ().ApplySitBackAction (gameObject, callback);
			break;
		case "坐抱头后仰":
			ActionManager.GetInstance ().ApplySitBackWithHandAction (gameObject, callback);
			break;
		}
	}


}
