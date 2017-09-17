using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class Person : MonoBehaviour
	{
		public Person parent;
		public bool isPrincipal;
		public bool isBeingControlled;
		public string spotName;

		private CompositeMovement compositeMovement;
		private float compositeTiming;
		private ComState state;

		private List<PrincipalActivity> principalActivities;
		private List<FollowingActivity> followingActivities;
		private PrincipalActivity currentPrincipalActivity;
		private FollowingActivity currentFollowingActivity;
		private List<SecondPerson> secondChildren;
		private List<ThirdPerson> thirdChildren;
		private List<Person> waitToBeChildren;
		private List<Person> children;

		private StorylineManager storylineManager;
		private ActionDealer actionDealer;

		private float staticAidPossibility;
		private float distanceError = 0.5f;
		private float randomDiff = 1.0f;
		[HideInInspector]
		public int actionListIndex;


		void Start ()
		{
			secondChildren = new List<SecondPerson> ();
			thirdChildren = new List<ThirdPerson> ();
			waitToBeChildren = new List<Person> ();
			children = new List<Person> ();
			principalActivities = new List<PrincipalActivity> ();
			followingActivities = new List<FollowingActivity> ();
			storylineManager = StorylineManager.GetInstance ();
			actionDealer = GetComponent<ActionDealer> ();
			staticAidPossibility = storylineManager.storyline.aid_possibility;
		}

		public bool AddPrincipalActivities (PrincipalActivity[] activities, string name)
		{
			if (parent != null || isPrincipal || isBeingControlled)
				return false;
			Stop ();
			isPrincipal = true;
			spotName = name;
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
				script.spotName = spotName;
				if (script.parent == this)
					script.Stop ();
			}
			secondChildren.Clear ();
			foreach (ThirdPerson person in thirdChildren) {
				string candidate = storylineManager.nameToJobCandidate [person.name];
				GameObject obj = storylineManager.nameToCharacterObj [candidate];
				Person script = obj.GetComponent<Person> ();
				script.spotName = spotName;
				if (script.parent == this)
					script.Stop ();
			}
			thirdChildren.Clear ();
			waitToBeChildren.Clear ();
			children.Clear ();

			parent = null;
			isPrincipal = false;
			isBeingControlled = false;
			if (!string.IsNullOrEmpty (spotName)) {
				storylineManager.QuitStorylineSpot (name, spotName);
				spotName = null;
			}
			state = ComState.LEAVING;

			ActionWalkTo walk = GetComponent<ActionWalkTo> ();
			if (walk != null) {
				walk.Finish ();
			}
		}

		void WhatNext ()
		{
			ActionSingle ac = GetComponent<ActionSingle> ();
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
			Character character = storylineManager.nameToCharacter [this.name];
			Landmark initialLandmark = LandmarkCollection.GetInstance ().Get (character.initial_position);
			if (Vector3.Distance (transform.position, initialLandmark.position) > distanceError) {
				if (actionDealer.TryNotSitting (null)) {
					ActionManager.GetInstance ().ApplyWalkToAction (gameObject, initialLandmark, null);
				}
			} else {
				DealMainActionWithAid (actionDealer, character.spare_time_main, character.spare_time_aid, false, "?", "?", "?");
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
							Log.warn ("Spot [" + spotName + "]'s principal activity [" + activity.description + "] job [" + other.name + "] overlapped: skipped");
						} else {
							string realName = storylineManager.nameToJobCandidate [other.name];
							GameObject realObj = storylineManager.nameToCharacterObj [realName];
							Person realScript = realObj.GetComponent<Person> ();
							secondChildren.Add (other);
							waitToBeChildren.Add (realScript);
							children.Add (realScript);
						}
					} else {
						Log.warn ("Spot [" + spotName + "]'s principal activity [" + activity.description + "] has undefined job [" + other.name + "]: skipped");
					}
				}
				currentPrincipalActivity = activity;
				compositeMovement = storylineManager.nameToCompositeMovement [activity.composite_movement_name];
				state = ComState.ARRIVING;
				StartCoroutine (WaitToBeMySecondChild ());
			} else {
				Log.warn ("Spot [" + spotName + "]'s principal activity [" + activity.description + "] has undefined composite movement [" + activity.composite_movement_name + "]: skipped");
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
						person.spotName = spotName;
						storylineManager.JoinStorylineSpot (person.name, spotName);
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
							Log.warn ("Spot [" + spotName + "]'s following activity [" + activity.description + "] job [" + other.name + "] overlapped: skipped");
						} else {
							string realName = storylineManager.nameToJobCandidate [other.name];
							GameObject realObj = storylineManager.nameToCharacterObj [realName];
							Person realScript = realObj.GetComponent<Person> ();
							thirdChildren.Add (other);
							waitToBeChildren.Add (realScript);
							children.Add (realScript);
						}
					} else {
						Log.warn ("Spot [" + spotName + "]'s following activity [" + activity.description + "] has undefined job [" + other.name + "]: skipped");
					}
				}
				currentFollowingActivity = followingActivities [0];
				compositeMovement = storylineManager.nameToCompositeMovement [activity.composite_movement_name];
				state = ComState.ARRIVING;
				StartCoroutine (WaitToBeMyThirdChild ());
			} else {
				Log.warn ("Spot [" + spotName + "]'s following activity [" + activity.description + "] has undefined composite movement [" + activity.composite_movement_name + "]: skipped");
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
						person.spotName = spotName;
						storylineManager.JoinStorylineSpot (person.name, spotName);
						waitToBeChildren.RemoveAt (i--);
					}
				}
				yield return new WaitForEndOfFrame ();
			}
		}



		void DealOwnActivity ()
		{
			Self self = isPrincipal ? currentPrincipalActivity.self : currentFollowingActivity.self;

			bool hasAction;
			switch (state) {

			case ComState.ARRIVING:
				if (IsAtProperLocation (self, gameObject, true)) {
					if (CheckEveryOneInPosition ())
						state = ComState.ARRIVINGSTOP;
					else {
						DealMainActionWithAid (actionDealer, compositeMovement.wait_mainrole_main, compositeMovement.wait_mainrole_aid, true, name, "执行等待动作", "执行等待动作(辅助)");
					}
				}
				break;

			case ComState.ARRIVINGSTOP:
				if (CheckEveryOneEndAction ()) {
					state = ComState.PREPARING;
					actionListIndex = -1;
					foreach (Person person in children) {
						person.actionListIndex = -1;
						ActionManager.GetInstance ().ApplyIdleAction (person.gameObject, "", Random.Range (0.0f, randomDiff), null);
					}
				}
				break;

			case ComState.PREPARING:
			// 如果有开始动作，并且开始动作未执行完，继续执行
				hasAction = DealListAction (actionDealer, compositeMovement.start_mainrole_main, ref actionListIndex, true, name, "执行开始动作");
			// 如果没有开始动作，或已经执行完，判断他人是否完成，否则继续
				if (!hasAction && compositeMovement.start_otherroles_main != null) {
					foreach (Person person in children) {
						if (person.actionListIndex < compositeMovement.start_otherroles_main.Length) {
							hasAction = true;
							break;
						}
					}
				}
			// 全部完成，进入下一状态
				if (!hasAction)
					state = ComState.PREPARINGSTOP;
				break;

			case ComState.PREPARINGSTOP:
				if (CheckEveryOneEndAction ()) {
					state = ComState.STARTING;
					DisplayBubble (gameObject, self);
					for (int i = 0; i < children.Count; ++i) {
						if (isPrincipal)
							DisplayBubble (children [i].gameObject, secondChildren [i]);
						else
							DisplayBubble (children [i].gameObject, thirdChildren [i]);
					}
				}
				break;

			case ComState.STARTING:
				DealMainActionWithAid (actionDealer, compositeMovement.mainrole_main, compositeMovement.mainrole_aid, true, name, "执行主要动作", "执行主要动作(辅助)");
				break;

			case ComState.STARTINGSTOP:
				if (CheckEveryOneEndAction ()) {
					state = ComState.ENDING;
					actionListIndex = -1;
					foreach (Person person in children) {
						person.actionListIndex = -1;
						ActionManager.GetInstance ().ApplyIdleAction (person.gameObject, "", Random.Range (0.0f, randomDiff), null);
					}
				}
				break;

			case ComState.ENDING:
			// 如果有结束动作，并且结束动作未执行完，继续执行
				hasAction = DealListAction (actionDealer, compositeMovement.end_mainrole_main, ref actionListIndex, true, name, "执行结束动作");
			// 如果没有结束动作，或已经执行完，判断他人是否完成，否则继续
				if (!hasAction && compositeMovement.end_otherroles_main != null) {
					foreach (Person person in children) {
						if (person.actionListIndex < compositeMovement.end_otherroles_main.Length) {
							hasAction = true;
							break;
						}
					}
				}
			// 全部完成，进入下一状态
				if (!hasAction)
					state = ComState.ENDINGSTOP;
				break;

			case ComState.ENDINGSTOP:
				if (CheckEveryOneEndAction ())
					state = ComState.LEAVING;
				break;

			case ComState.LEAVING:
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
				if (person.GetComponent<ActionSingle> () != null)
					continue;
			
				switch (state) {
				case ComState.ARRIVING:
					if (IsAtProperLocation (others [i], person.gameObject, true)) {
						DealMainActionWithAid (person.actionDealer, compositeMovement.wait_otherroles_main, compositeMovement.wait_otherroles_aid, true, person.name, "执行等待动作", "执行等待动作(辅助)");
					}
					break;
				case ComState.ARRIVINGSTOP:
					break;
				case ComState.PREPARING:
					DealListAction (person.actionDealer, compositeMovement.start_otherroles_main, ref person.actionListIndex, true, person.name, "执行开始动作");
					break;
				case ComState.PREPARINGSTOP:
					break;
				case ComState.STARTING:
					DealMainActionWithAid (person.actionDealer, compositeMovement.otherroles_main, compositeMovement.otherroles_aid, true, person.name, "执行主要动作", "执行主要动作(辅助)");
					break;
				case ComState.ENDING:
					DealListAction (person.actionDealer, compositeMovement.end_otherroles_main, ref person.actionListIndex, true, person.name, "执行结束动作");
					break;
				case ComState.ENDINGSTOP:
					break;
				case ComState.LEAVING:
					break;
				}
			}
		}


		void DealMainActionWithAid (ActionDealer actionDealer, string main, string[] aid, bool showInfo, string personName, string mainInfo, string aidInfo)
		{
			if (aid == null || aid.Length == 0) {
				actionDealer.ApproachAction (main, null);

				if (showInfo && !string.IsNullOrEmpty (main))
					Log.info (Log.yellow ("【" + personName + "】 ") + mainInfo + Log.blue (" 【" + main + "】"));
			} else {
				int aidPossibility = Random.Range (0, (int)(1 / staticAidPossibility));
				if (aidPossibility == 0) {
					int index = Random.Range (0, aid.Length);
					actionDealer.ApproachAction (aid [index], null);

					if (showInfo && !string.IsNullOrEmpty (aid [index]))
						Log.info (Log.yellow ("【" + personName + "】 ") + aidInfo + Log.blue (" 【" + aid [index] + "】"));
				} else {
					actionDealer.ApproachAction (main, null);

					if (showInfo && !string.IsNullOrEmpty (main)) {
						Log.info (Log.yellow ("【" + personName + "】 ") + mainInfo + Log.blue (" 【" + main + "】"));
					}
				}
			}
		}

		bool DealListAction (ActionDealer actionDealer, string[] list, ref int index, bool info, string personName, string mainInfo)
		{
			if (list != null) {
				index++;
				if (index < list.Length) {
					string actionName = list [index];
					actionDealer.ApproachAction (actionName, null);
					if (info && !string.IsNullOrEmpty (actionName))
						Log.info (Log.yellow ("【" + personName + "】 ") + mainInfo + Log.blue (" 【" + actionName + "】"));
					return true;
				}
			}
			return false;
		}

		bool IsAtProperLocation (Self self, GameObject gameObject, bool doAction)
		{
			Landmark destination;
			switch (self.location_to_type) {
			case 0:	// stand by
				return true;
			case 1:	// labeled location
				if (string.IsNullOrEmpty (self.location_to)) {
					Character cha = storylineManager.nameToCharacter [gameObject.name];
					destination = LandmarkCollection.GetInstance ().Get (cha.initial_position);
				} else {
					destination = LandmarkCollection.GetInstance ().Get (self.location_to);
				}
				if (Vector3.Distance (gameObject.transform.position, destination.position) <= distanceError)
					return true;
				else if (doAction) {
					if (gameObject.GetComponent<ActionDealer> ().TryNotSitting (null))
						ActionManager.GetInstance ().ApplyWalkToAction (gameObject, destination, null);
				}
				return false;
			case 2:	// closest location
				if (self.location_to == null) {
					Log.warn ("Empty object location when needed");
					return true;
				}
				destination = LandmarkCollection.GetInstance ().GetNearestObject (gameObject.transform.position, self.location_to);
				if (Vector3.Distance (gameObject.transform.position, destination.position) <= distanceError)
					return true;
				else if (doAction) {
					if (gameObject.GetComponent<ActionDealer> ().TryNotSitting (null))
						ActionManager.GetInstance ().ApplyWalkToAction (gameObject, destination, null);
				}
				return false;
			default:
				Log.warn ("Undefined location-to type [" + self.location_to_type + "]");
				return true;
			}
		}


		bool CheckEveryOneInPosition ()
		{
			if (isPrincipal) {
				if (!IsAtProperLocation (currentPrincipalActivity.self, gameObject, false))
					return false;
				for (int i = 0; i < secondChildren.Count; ++i)
					if (children [i].parent != this || !IsAtProperLocation (secondChildren [i], children [i].gameObject, false))
						return false;
				return true;
			} else {
				if (!IsAtProperLocation (currentFollowingActivity.self, gameObject, false))
					return false;
				for (int i = 0; i < thirdChildren.Count; ++i)
					if (children [i].parent != this || !IsAtProperLocation (thirdChildren [i], children [i].gameObject, false))
						return false;
				return true;
			}
		}


		bool CheckEveryOneEndAction ()
		{
			if (GetComponent<ActionSingle> () != null)
				return false;
			for (int i = 0; i < children.Count; ++i)
				if (children [i].parent != this || children [i].GetComponent<ActionSingle> () != null)
					return false;
			return true;
		}


		void DisplayBubble (GameObject obj, Self self)
		{
			switch (self.bubble_type) {
			case 1:
				ActionManager.GetInstance ().ApplyChatAction (obj, self.bubble_content, 2, null);
				break;
			case 2:
				break;
			}
		}


		void Update ()
		{
			WhatNext ();

			if (currentPrincipalActivity != null || currentFollowingActivity != null)
				DealChildActivity ();

			if (state == ComState.STARTING) {
				compositeTiming += Time.deltaTime;
				if (currentPrincipalActivity != null) {
					if (compositeTiming >= currentPrincipalActivity.duration)
						state = ComState.STARTINGSTOP;
				} else if (currentFollowingActivity != null) {
					if (compositeTiming >= currentFollowingActivity.duration)
						state = ComState.STARTINGSTOP;
				}
			} else {
				compositeTiming = 0.0f;
			}
		}
	}
}