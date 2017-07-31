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
		//int index = Random.Range (0, 3);
		int index = 0;
		if (index == 0) {
			ActionManager.GetInstance ().ApplySimpleTypeAction (this.gameObject, this);
		} else if (index == 1) {
			ActionManager.GetInstance ().ApplyScratchHeadAction (this.gameObject, this);
		} else if (index == 2) {
			ActionManager.GetInstance ().ApplySimpleClickAction (this.gameObject, this);
		}
	}
}
