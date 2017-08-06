using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coder : MonoBehaviour, IActionCompleted
{
	public Transform chair;

	void Start ()
	{
		ActionManager.GetInstance ().ApplyWalkToAction (this.gameObject, chair.position, true, chair.rotation, this);
	}

	public void OnActionCompleted (Action ac)
	{
		ActionManager.GetInstance ().ApplyChatAction (this.gameObject, ac.id.ToString (), 2, null);
		if (ac.id == ActionID.WALKTO) {
			ActionManager.GetInstance ().ApplySitDownAction (this.gameObject, this);
		} else {
			RandomSitDownAnimation ();
		}
	}

	void RandomSitDownAnimation ()
	{
		int index = Random.Range (0, 4);
		if (index == 0) {
			ActionManager.GetInstance ().ApplyTypeAction (this.gameObject, this);
		} else if (index == 1) {
			ActionManager.GetInstance ().ApplyScratchHeadAction (this.gameObject, this);
		} else if (index == 2) {
			ActionManager.GetInstance ().ApplyClickAction (this.gameObject, this);
		} else if (index == 3) {
			ActionManager.GetInstance ().ApplyHandOnChinAction (this.gameObject, this);
		}
	}

	void Update ()
	{
		Action ac = GetComponent<Action> ();
		if (ac == null)
			Log.warn ("nope");
	}
}
