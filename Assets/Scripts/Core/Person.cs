using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
	public Person parent;
	public bool isPrincipal;
	public bool isBeingControlled;
	public bool isStanding = true;

	private CompositeMovement compositeMovement;
	private float compositeTiming;

	private List<PrincipalActivity> principalActivities;
	private List<FollowingActivity> followingActivities;
	private PrincipalActivity currentPrincipalActivity;
	private FollowingActivity currentFollowingActivity;
	private List<SecondPerson> secondChildren;
	private List<ThirdPerson> thirdChildren;

	private StorylineManager storylineManager;
	private ActionManager actionManager;
	private Character character;

	private float spareTimeAidPossibility = 0.1f;
	private string spareTimeAidTemp;

	private List<Person> waitToBeSecondChild;
	private List<Person> waitToBeThirdChild;

	void Start ()
	{
		secondChildren = new List<SecondPerson> ();
		thirdChildren = new List<ThirdPerson> ();
		waitToBeSecondChild = new List<Person> ();
		waitToBeThirdChild = new List<Person> ();
		principalActivities = new List<PrincipalActivity> ();
		followingActivities = new List<FollowingActivity> ();
		storylineManager = StorylineManager.GetInstance ();
		actionManager = ActionManager.GetInstance ();
		character = storylineManager.nameToCharacter [this.name];
	}

	public bool AddPrincipalActivities (PrincipalActivity[] activities)
	{
		if (parent != null || isPrincipal || isBeingControlled)
			return false;
		Stop ();
		isPrincipal = true;
		foreach (PrincipalActivity activity in activities)
			principalActivities.Add (activity);
		return true;
	}

	public void Stop ()
	{
		StopAllCoroutines ();
		compositeMovement = null;
		principalActivities.Clear ();
		followingActivities.Clear ();
		currentPrincipalActivity = null;
		currentFollowingActivity = null;
		foreach (SecondPerson person in secondChildren) {
			string candidate = storylineManager.nameToJobCandidate [person.name];
			GameObject obj = storylineManager.nameToCharacterObj [candidate];
			Person script = obj.GetComponent<Person> ();
			if (script.parent == this)
				script.Stop ();
		}
		secondChildren.Clear ();
		waitToBeSecondChild.Clear ();
		foreach (ThirdPerson person in thirdChildren) {
			string candidate = storylineManager.nameToJobCandidate [person.name];
			GameObject obj = storylineManager.nameToCharacterObj [candidate];
			Person script = obj.GetComponent<Person> ();
			if (script.parent == this)
				script.Stop ();
		}
		thirdChildren.Clear ();
		waitToBeThirdChild.Clear ();
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
			Log.error ("Unexpected state of [" + this.name + "]: principal being controlled");
		} else if (isPrincipal && parent != null) {
			Stop ();
			Log.error ("Unexpected state of [" + this.name + "]: principal with parent");
		} else if (parent == null && isBeingControlled) {
			Stop ();
			Log.error ("Unexpected state of [" + this.name + "]: being controlled without parent");
		} else if (isPrincipal) {
			if (currentPrincipalActivity != null) {
				DealOwnPrincipalActivity ();
			} else if (currentPrincipalActivity == null && principalActivities.Count > 0) {
				AssignNewPrincipalActivity ();
			} else {
				Stop ();
				Log.info ("Spot for [" + this.name + "] complete naturely");
			}
		} else if (isBeingControlled) {
			return;
		} else if (parent != null) {
			if (currentFollowingActivity != null) {
				DealOwnFollowingActivity ();
			} else if (currentFollowingActivity == null && followingActivities.Count > 0) {
				AssignNewFollowingActivity ();
			} else {
				Stop ();
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
					int aidPossiblity = Random.Range (0, (int)(1 / spareTimeAidPossibility));
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


	void AssignNewPrincipalActivity ()
	{
		PrincipalActivity activity = principalActivities [0];
		if (storylineManager.nameToCompositeMovement.ContainsKey (activity.composite_movement_name)) {
			foreach (SecondPerson other in activity.other_people) {
				if (storylineManager.nameToJob.ContainsKey (other.name)) {
					int index = 0;
					for (index = 0; index < secondChildren.Count; ++index) {
						if (secondChildren [index].name == other.name)
							break;
					}
					if (index < secondChildren.Count) {
						Log.warn ("Principal activity [" + activity.description + "] job [" + other.name + "] overlapped: skipped");
					} else {
						secondChildren.Add (other);
						string realName = storylineManager.nameToJobCandidate [other.name];
						GameObject realObj = storylineManager.nameToCharacterObj [realName];
						Person realScript = realObj.GetComponent<Person> ();
						waitToBeSecondChild.Add (realScript);
					}
				} else {
					Log.warn ("Principal activity [" + activity.description + "] has undefined job [" + other.name + "]: skipped");
				}
			}
			currentPrincipalActivity = activity;
			StartCoroutine (WaitToBeMySecondChild ());
		} else {
			Log.warn ("Principal activity [" + activity.description + "] has undefined composite movement [" + activity.composite_movement_name + "]: skipped");
		}
		principalActivities.RemoveAt (0);
	}

	IEnumerator WaitToBeMySecondChild ()
	{
		while (waitToBeSecondChild.Count > 0) {
			for (int i = 0; i < waitToBeSecondChild.Count; ++i) {
				Person person = waitToBeSecondChild [i];
				if (!person.isPrincipal && !person.isBeingControlled &&
				    (person.parent == null || person.parent == this)) {
					person.Stop ();
					person.parent = this;
					person.isBeingControlled = true;
					waitToBeSecondChild.RemoveAt (i--);
				}
			}
			yield return new WaitForEndOfFrame ();
		}
	}


	void AssignNewFollowingActivity ()
	{
		FollowingActivity activity = followingActivities [0];
		if (storylineManager.nameToCompositeMovement.ContainsKey (activity.composite_movement_name)) {
			foreach (ThirdPerson other in activity.other_people) {
				if (storylineManager.nameToJob.ContainsKey (other.name)) {
					int index = 0;
					for (index = 0; index < thirdChildren.Count; ++index) {
						if (thirdChildren [index].name == other.name)
							break;
					}
					if (index < thirdChildren.Count) {
						Log.warn ("Following activity [" + activity.description + "] job [" + other.name + "] overlapped: skipped");
					} else {
						thirdChildren.Add (other);
						string realName = storylineManager.nameToJobCandidate [other.name];
						GameObject realObj = storylineManager.nameToCharacterObj [realName];
						Person realScript = realObj.GetComponent<Person> ();
						waitToBeThirdChild.Add (realScript);
					}
				} else {
					Log.warn ("Following activity [" + activity.description + "] has undefined job [" + other.name + "]: skipped");
				}
			}
			currentFollowingActivity = followingActivities [0];
			StartCoroutine (WaitToBeMyThirdChild ());
		} else {
			Log.warn ("Following activity [" + activity.description + "] has undefined composite movement [" + activity.composite_movement_name + "]: skipped");
		}
		followingActivities.RemoveAt (0);
	}

	IEnumerator WaitToBeMyThirdChild ()
	{
		while (waitToBeThirdChild.Count > 0) {
			for (int i = 0; i < waitToBeThirdChild.Count; ++i) {
				Person person = waitToBeThirdChild [i];
				if (!person.isPrincipal && !person.isBeingControlled &&
				    (person.parent == null || person.parent == this)) {
					person.Stop ();
					person.parent = this;
					person.isBeingControlled = true;
					waitToBeThirdChild.RemoveAt (i--);
				}
			}
			yield return new WaitForEndOfFrame ();
		}
	}



	void DealOwnPrincipalActivity ()
	{
		 
	}

	void DealOwnFollowingActivity ()
	{

	}

	void Update ()
	{
		WhatNext ();
	}
}
