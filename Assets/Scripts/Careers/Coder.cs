using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coder : MonoBehaviour, ActionCompleted
{
	public Transform chair;

	private int lastindex;

	void Start ()
	{
		//ActionManager.GetInstance ().ApplyChatAction (this.gameObject, "TEst", 2f, null);
		ActionManager.GetInstance ().ApplyWalkToAction (this.gameObject, chair.position, chair.rotation, this);
	}

	public void OnActionCompleted (Action ac)
	{
		if (ac.ID == ActionID.WALKTO) {
			ActionManager.GetInstance ().ApplySitDownAction (this.gameObject, this);
		} else {
			RandomSitDownAnimation ();
		}
	}

	void RandomSitDownAnimation ()
	{
		int index = Random.Range (0, 3);
		while (index == lastindex)
			index = Random.Range (0, 3);
		lastindex = index;
		if (index == 0) {
			ActionManager.GetInstance ().ApplySimpleTypeAction (this.gameObject, this);
		} else if (index == 1) {
			ActionManager.GetInstance ().ApplySimpleClickAction (this.gameObject, this);
		} else if (index == 2) {
			ActionManager.GetInstance ().ApplyScratchHeadAction (this.gameObject, this);
		}
	}
}
