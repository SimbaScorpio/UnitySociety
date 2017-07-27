using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coder : MonoBehaviour, IActionCompleted
{
	public Transform chair;

	void Start ()
	{
		ActionManager.GetInstance ().ApplyWalkToAction (this.gameObject, chair.position, chair.rotation, this);
	}

	public void OnActionCompleted (Action ac)
	{
		if (ac.ID == ActionID.WALKTO) {
			ActionManager.GetInstance ().ApplySitDownAction (this.gameObject, this);
		}
	}

	void RandomSitDownAnimation ()
	{
		int index = Random.Range (0, 3);
		if (index == 0) {
			ActionManager.GetInstance ().ApplySimpleTypeAction (this.gameObject, this);
			//ActionManager.GetInstance ().ApplyChatAction (this.gameObject, ac.ID.ToString (), 2f, null);
		} else if (index == 1) {
			ActionManager.GetInstance ().ApplyScratchHeadAction (this.gameObject, this);
			//ActionManager.GetInstance ().ApplyChatAction (this.gameObject, ac.ID.ToString (), 2f, null);
		} else if (index == 2) {
			ActionManager.GetInstance ().ApplySimpleClickAction (this.gameObject, this);
			//ActionManager.GetInstance ().ApplyChatAction (this.gameObject, ac.ID.ToString (), 2f, null);
		}
	}

		
	public void OnSitDownIdleFinished ()
	{
		RandomSitDownAnimation ();
	}
}
