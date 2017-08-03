using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour, IActionCompleted
{
	public Person parent;
	public bool isPrincipal;
	public bool isBeingControlled;
	public bool isStanding = true;

	private CompositeMovement compositeMovement;
	private float compositeTiming;
	private int state;

	private List<PrincipalActivity> principalActivities;
	private List<FollowingActivity> followingActivities;
	private PrincipalActivity currentPrincipalActivity;
	private FollowingActivity currentFollowingActivity;
	private List<SecondPerson> secondChildren;
	private List<ThirdPerson> thirdChildren;
	private List<Person> waitToBeChildren;
	private List<Person> children;

	private StorylineManager storylineManager;
	private ActionManager actionManager;
	private Character character;

	private float staticAidPossibility = 0.1f;
	private string spareTimeAidTemp;
	private float distanceError = 1f;


	void Start ()
	{
		secondChildren = new List<SecondPerson> ();
		thirdChildren = new List<ThirdPerson> ();
		waitToBeChildren = new List<Person> ();
		children = new List<Person> ();
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

	public void AddFollowingActivities (FollowingActivity[] activities)
	{
		foreach (FollowingActivity activity in activities)
			followingActivities.Add (activity);
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
		foreach (ThirdPerson person in thirdChildren) {
			string candidate = storylineManager.nameToJobCandidate [person.name];
			GameObject obj = storylineManager.nameToCharacterObj [candidate];
			Person script = obj.GetComponent<Person> ();
			if (script.parent == this)
				script.Stop ();
		}
		thirdChildren.Clear ();
		waitToBeChildren.Clear ();
		children.Clear ();
		parent = null;
		isPrincipal = false;
		isBeingControlled = false;
		state = 0;

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
				DealOwnActivity ();
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
				DealOwnActivity ();
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
		if (Vector3.Distance (transform.position, initialTranform.position) > distanceError) {
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
					int aidPossiblity = Random.Range (0, (int)(1 / staticAidPossibility));
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
						waitToBeChildren.Add (realScript);
						children.Add (realScript);
					}
				} else {
					Log.warn ("Principal activity [" + activity.description + "] has undefined job [" + other.name + "]: skipped");
				}
			}
			currentPrincipalActivity = activity;
			compositeMovement = storylineManager.nameToCompositeMovement [activity.composite_movement_name];
			state = 0;
			StartCoroutine (WaitToBeMySecondChild ());
		} else {
			Log.warn ("Principal activity [" + activity.description + "] has undefined composite movement [" + activity.composite_movement_name + "]: skipped");
		}
		principalActivities.RemoveAt (0);
	}

	IEnumerator WaitToBeMySecondChild ()
	{
		while (waitToBeChildren.Count > 0) {
			for (int i = 0; i < waitToBeChildren.Count; ++i) {
				Person person = waitToBeChildren [i];
				if (!person.isPrincipal && !person.isBeingControlled &&
				    (person.parent == null || person.parent == this)) {
					person.Stop ();
					person.parent = this;
					person.isBeingControlled = true;
					waitToBeChildren.RemoveAt (i--);
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
						waitToBeChildren.Add (realScript);
						children.Add (realScript);
					}
				} else {
					Log.warn ("Following activity [" + activity.description + "] has undefined job [" + other.name + "]: skipped");
				}
			}
			currentFollowingActivity = followingActivities [0];
			compositeMovement = storylineManager.nameToCompositeMovement [activity.composite_movement_name];
			state = 0;
			StartCoroutine (WaitToBeMyThirdChild ());
		} else {
			Log.warn ("Following activity [" + activity.description + "] has undefined composite movement [" + activity.composite_movement_name + "]: skipped");
		}
		followingActivities.RemoveAt (0);
	}

	IEnumerator WaitToBeMyThirdChild ()
	{
		while (waitToBeChildren.Count > 0) {
			for (int i = 0; i < waitToBeChildren.Count; ++i) {
				Person person = waitToBeChildren [i];
				if (!person.isPrincipal && !person.isBeingControlled &&
				    (person.parent == null || person.parent == this)) {
					person.Stop ();
					person.parent = this;
					person.isBeingControlled = true;
					waitToBeChildren.RemoveAt (i--);
				}
			}
			yield return new WaitForEndOfFrame ();
		}
	}



	void DealOwnActivity ()
	{
		Self self = isPrincipal ? currentPrincipalActivity.self : currentFollowingActivity.self;
		switch (state) {
		case 0:
			if (IsAtProperLocation (self, gameObject, true)) {
				// do nothing and wait children
				CheckEveryOneInPosition ();
			}
			break;
		case 1:
			if (isStanding && actionManager.IsSitAction (compositeMovement.mainrole_main)) {
				actionManager.ApplySitDownAction (gameObject, null);
			} else if (!isStanding && actionManager.IsStandAction (compositeMovement.mainrole_main)) {
				actionManager.ApplyStandUpAction (gameObject, null);
			} else {
				if (compositeMovement.mainrole_aid.Length == 0) {
					actionManager.ApplyAction (compositeMovement.mainrole_main, gameObject, null);
				} else {
					int aidPossiblity = Random.Range (0, (int)(1 / staticAidPossibility));
					if (aidPossiblity == 0) {
						int index = Random.Range (0, compositeMovement.mainrole_aid.Length);
						actionManager.ApplyAction (compositeMovement.mainrole_aid [index], gameObject, null);
					} else {
						actionManager.ApplyAction (compositeMovement.mainrole_main, gameObject, null);
					}
				}
			}
			break;
		case 2:
			if (isPrincipal) {
				currentPrincipalActivity = null;
				for (int i = 0; i < secondChildren.Count; ++i) {
					children [i].AddFollowingActivities (secondChildren [i].following_activities);
					children [i].isBeingControlled = false;
				}
				secondChildren.Clear ();
			} else {
				currentFollowingActivity = null;
				for (int i = 0; i < thirdChildren.Count; ++i) {
					children [i].Stop ();
				}
				thirdChildren.Clear ();
			}
			children.Clear ();
			break;
		}
	}


	void DealChildActivity ()
	{  
		List<Self> others = new List<Self> ();
		if (isPrincipal) {
			foreach (SecondPerson sp in secondChildren)
				others.Add (sp as Self);
		} else {
			foreach (ThirdPerson tp in thirdChildren)
				others.Add (tp as Self);
		}
		for (int i = 0; i < others.Count; ++i) {
			Person person = children [i];
			if (person.parent != this)
				continue;
			if (person.GetComponent<Action> () != null)
				continue;
			switch (state) {
			case 0:
				if (IsAtProperLocation (others [i], person.gameObject, true)) {
					// do nothing and wait other children
					CheckEveryOneInPosition ();
				}
				break;
			case 1:
				if (isStanding && actionManager.IsSitAction (compositeMovement.otherroles_main)) {
					actionManager.ApplySitDownAction (person.gameObject, null);
				} else if (!isStanding && actionManager.IsStandAction (compositeMovement.otherroles_main)) {
					actionManager.ApplyStandUpAction (person.gameObject, null);
				} else {
					if (compositeMovement.otherroles_aid.Length == 0) {
						actionManager.ApplyAction (compositeMovement.otherroles_main, person.gameObject, null);
					} else {
						int aidPossiblity = Random.Range (0, (int)(1 / staticAidPossibility));
						if (aidPossiblity == 0) {
							int index = Random.Range (0, compositeMovement.otherroles_aid.Length);
							actionManager.ApplyAction (compositeMovement.otherroles_aid [index], person.gameObject, null);
						} else {
							actionManager.ApplyAction (compositeMovement.otherroles_main, person.gameObject, null);
						}
					}
				}
				break;
			case 2:
				break;
			}
		}
	}


	bool IsAtProperLocation (Self self, GameObject gameObject, bool doAction)
	{
		Transform destination;
		switch (self.location_to_type) {
		case 0:	// stand by
			return true;
		case 1:	// labeled location
			if (self.location_to == null) {
				Log.warn ("Empty labeled location when needed");
				return true;
			}
			destination = LocationCollection.Get (self.location_to);
			if (Vector3.Distance (transform.position, destination.position) <= distanceError)
				return true;
			if (doAction) {
				if (!isStanding)
					actionManager.ApplyStandUpAction (gameObject, null);
				else
					actionManager.ApplyWalkToAction (gameObject, destination.position, true, destination.rotation, this);
			}
			return false;
		case 2:	// closest location
			if (self.location_to == null) {
				Log.warn ("Empty object location when needed");
				return true;
			}
			destination = LocationCollection.GetNearestObject (transform.position, self.location_to);
			if (Vector3.Distance (transform.position, destination.position) <= distanceError)
				return true;
			if (doAction) {
				if (!isStanding)
					actionManager.ApplyStandUpAction (gameObject, null);
				else
					actionManager.ApplyWalkToAction (gameObject, destination.position, true, destination.rotation, this);
			}
			return false;
		default:
			Log.warn ("Undefined location-to type [" + self.location_to_type + "]");
			return true;
		}
	}


	void CheckEveryOneInPosition ()
	{
		if (isPrincipal) {
			for (int i = 0; i < secondChildren.Count; ++i)
				if (children [i].parent != this || !IsAtProperLocation (secondChildren [i], null, false))
					return;
			state = 1;
		} else {
			for (int i = 0; i < thirdChildren.Count; ++i)
				if (children [i].parent != this || !IsAtProperLocation (thirdChildren [i], null, false))
					return;
			state = 1;
		}
	}


	public void OnActionCompleted (Action ac)
	{
		CheckEveryOneInPosition ();
	}


	void Update ()
	{
		WhatNext ();
		if (currentPrincipalActivity != null || currentFollowingActivity != null)
			DealChildActivity ();

		if (state == 1) {
			compositeTiming += Time.deltaTime;
			if (currentPrincipalActivity != null) {
				if (compositeTiming >= currentPrincipalActivity.duration)
					state = 2;
			} else if (currentFollowingActivity != null) {
				if (compositeTiming >= currentFollowingActivity.duration)
					state = 2;
			}
		}
	}
}
