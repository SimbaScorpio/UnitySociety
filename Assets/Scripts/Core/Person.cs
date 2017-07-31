using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour, IActionCompleted
{
	public string parent;
	public bool isPrincipal;
	public bool isBeingControlled;
	public List<string> children;

	private CompositeMovement compositeMovement;
	private float compositeTiming;

	private List<PrincipalActivity> principalActivities;
	private List<FollowingActivity> followingActivities;

	public bool isStanding = true;
	private StorylineManager storylineManager;
	private ActionManager actionManager;
	private Character character;
	private PrincipalActivity currentPrincipalActivity;
	private FollowingActivity currentFollowingActivity;

	private float spareTimeAidPossibility = 0.5f;
	private string spareTimeAidTemp;

	void Start ()
	{
		children = new List<string> ();
		principalActivities = new List<PrincipalActivity> ();
		followingActivities = new List<FollowingActivity> ();
		storylineManager = StorylineManager.GetInstance ();
		actionManager = ActionManager.GetInstance ();
		character = storylineManager.nameToCharacter [this.name];
	}

	public bool AddPrincipalActivities (PrincipalActivity[] activities)
	{
		if (!string.IsNullOrEmpty (parent) || isPrincipal || activities.Length == 0)
			return false;
		parent = null;
		isPrincipal = true;
		isBeingControlled = false;
		compositeMovement = null;
		principalActivities.Clear ();
		followingActivities.Clear ();
		foreach (PrincipalActivity activity in activities)
			principalActivities.Add (activity);
		return true;
	}

	public void Stop ()
	{
		compositeMovement = null;
		principalActivities.Clear ();
		followingActivities.Clear ();
		foreach (string name in children) {
			GameObject obj = storylineManager.nameToCharacterObj [name];
			Person person = obj.GetComponent<Person> ();
			person.Stop ();
		}
		children.Clear ();
		parent = null;
		isPrincipal = false;
		isBeingControlled = false;

		ActionWalkTo walk = GetComponent<ActionWalkTo> ();
		if (walk != null) {
			walk.Finish ();
		}
	}

	void WhatNext ()
	{
		Action ac = GetComponent<Action> ();
		if (ac != null) {
			return;
		} else if (isPrincipal && isBeingControlled) {
			Stop ();
			Log.warn ("Unexpected state of [" + this.name + "]: principal being controlled");
		} else if (isPrincipal && !string.IsNullOrEmpty (parent)) {
			Stop ();
			Log.warn ("Unexpected state of [" + this.name + "]: principal with parent");
		} else if (string.IsNullOrEmpty (parent) && isBeingControlled) {
			Stop ();
			Log.warn ("Unexpected state of [" + this.name + "]: being controlled without parent");
		} else if (isPrincipal) {
			if (currentPrincipalActivity != null) {
				DealOwnPrincipalActivity ();
			} else if (currentPrincipalActivity == null && principalActivities.Count > 0) {
				currentPrincipalActivity = principalActivities [0];
				principalActivities.RemoveAt (0);
			} else {
				DealSpareActivity ();
			}
		} else if (isBeingControlled) {
			return;
		} else if (!string.IsNullOrEmpty (parent)) {
			if (currentFollowingActivity != null) {
				DealOwnFollowingActivity ();
			} else if (currentFollowingActivity == null && followingActivities.Count > 0) {
				currentFollowingActivity = followingActivities [0];
				followingActivities.RemoveAt (0);
			} else {
				DealSpareActivity ();
			}
		} else {
			DealSpareActivity ();
		}
	}

	void DealSpareActivity ()
	{
		Transform initialTranform = LocationCollection.Get (character.initial_position);
		if (Vector3.Distance (transform.position, initialTranform.position) > 1f) {
			if (!isStanding) {
				actionManager.ApplyStandUpAction (gameObject, null);
			} else {
				actionManager.ApplyWalkToAction (gameObject, initialTranform.position, true, initialTranform.rotation, null);
			}
		} else {
			if (!string.IsNullOrEmpty (spareTimeAidTemp)) {
				actionManager.ApplyAction (spareTimeAidTemp, gameObject, null);
				spareTimeAidTemp = null;
			} else if (isStanding && actionManager.IsSitAction (character.spare_time_main)) {
				actionManager.ApplySitDownAction (gameObject, null);
			} else if (!isStanding && actionManager.IsStandAction (character.spare_time_main)) {
				actionManager.ApplyStandUpAction (gameObject, null);
			} else {
				if (character.spare_time_aid.Length == 0) {
					actionManager.ApplyAction (character.spare_time_main, gameObject, null);
				} else {
					int aidPossiblity = Random.Range (0, (int)(1 / spareTimeAidPossibility)); // 1/5
					if (aidPossiblity == 0) {
						int index = Random.Range (0, character.spare_time_aid.Length);
						spareTimeAidTemp = character.spare_time_aid [index];
						if (isStanding && actionManager.IsSitAction (spareTimeAidTemp)) {
							actionManager.ApplySitDownAction (gameObject, null);
						} else if (!isStanding && actionManager.IsStandAction (spareTimeAidTemp)) {
							actionManager.ApplyStandUpAction (gameObject, null);
						} else {
							actionManager.ApplyAction (spareTimeAidTemp, gameObject, null);
							spareTimeAidTemp = null;
						}
					} else {
						actionManager.ApplyAction (character.spare_time_main, gameObject, null);
					}
				}
			}
		}
	}

	void DealOwnPrincipalActivity ()
	{
		
	}

	void DealOwnFollowingActivity ()
	{

	}

	void DealWithOwnActionCallback (Action ac)
	{
		print ("recieve " + name);
	}

	void DealWithChildActionCallback (Action ac)
	{
		
	}

	public void OnActionCompleted (Action ac)
	{
		if (!string.IsNullOrEmpty (parent) && isBeingControlled)
			return;
		if (ac.gameObject == gameObject) {
			DealWithOwnActionCallback (ac);
		} else {
			DealWithChildActionCallback (ac);
		}
	}

	void Update ()
	{
		WhatNext ();
	}
}
