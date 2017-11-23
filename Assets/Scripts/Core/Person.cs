using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace DesignSociety
{
	public class Person : MonoBehaviour
	{
		public Person parent;
		public bool isPrincipal;
		public bool isBeingControlled;
		public string spotName;

		// 随机走路速度
		public Vector2 walkSpeedRange = new Vector2 (0.8f, 1.2f);

		private CompositeMovementData compositeMovement;
		private float compositeTiming;
		public ComState state;

		public List<PrincipalActivity> principalActivities;
		public List<FollowingActivity> followingActivities;
		public PrincipalActivity currentPrincipalActivity;
		public FollowingActivity currentFollowingActivity;
		private List<SecondPerson> secondChildren;
		private List<ThirdPerson> thirdChildren;
		private List<Person> waitToBeChildren;
		private List<Person> children;

		private StorylineManager storylineManager;
		private NetworkActionDealer actionDealer;
		private NetworkBubbleDealer bubbleDealer;

		private float distanceError = 0.5f;
		private float rotationError = 0.2f;
		private float randomDiff = 1.0f;
		[HideInInspector]
		public int actionListIndex;
		public int bubbleListIndex;

		[HideInInspector]
		public bool hasArrived;


		// temp variables (reduce GC)
		private CharacterData tempChaData;
		private Landmark tempLandmark;
		private string[] tempStrings;
		private CompositeMovementData tempComMovement = new CompositeMovementData ();
		private Self tempSelf;
		private bool tempHasAction;
		private Person tempPerson;
		private List<Self> tempOthers = new List<Self> ();
		private Landmark tempDestination;
		private CharacterData tempChaData2;


		void Start ()
		{
			secondChildren = new List<SecondPerson> ();
			thirdChildren = new List<ThirdPerson> ();
			waitToBeChildren = new List<Person> ();
			children = new List<Person> ();
			principalActivities = new List<PrincipalActivity> ();
			followingActivities = new List<FollowingActivity> ();
			storylineManager = StorylineManager.GetInstance ();
			actionDealer = GetComponent<NetworkActionDealer> ();
			bubbleDealer = GetComponent<NetworkBubbleDealer> ();

			GetComponent<MyRichAI> ().maxSpeed = Random.Range (walkSpeedRange.x, walkSpeedRange.y);
		}

		public bool AddPrincipalActivities (PrincipalActivity[] activities, string name)
		{
			if (parent != null || isPrincipal || isBeingControlled)
				return false;
			Stop ();
			isPrincipal = true;
			spotName = name;
			for (int i = 0; i < activities.Length; ++i) {
				principalActivities.Add (activities [i]);
			}
			return true;
		}

		public void AddFollowingActivities (FollowingActivity[] activities)
		{
			for (int i = 0; i < activities.Length; ++i)
				followingActivities.Add (activities [i]);
		}

		public void Stop ()
		{
			StopAllCoroutines ();
			compositeMovement = null;
			principalActivities.Clear ();
			followingActivities.Clear ();
			currentPrincipalActivity = null;
			currentFollowingActivity = null;

			SecondPerson person1;
			for (int i = 0; i < secondChildren.Count; ++i) {
				person1 = secondChildren [i];
				string candidate = storylineManager.nameToJobCandidateName [person1.name];
				GameObject obj = storylineManager.nameToCharacterObj [candidate];
				Person script = obj.GetComponent<Person> ();
				script.spotName = spotName;
				if (script.parent == this)
					script.Stop ();
			}
			secondChildren.Clear ();

			ThirdPerson person2;
			for (int i = 0; i < thirdChildren.Count; ++i) {
				person2 = thirdChildren [i];
				string candidate = storylineManager.nameToJobCandidateName [person2.name];
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

			hasArrived = false;

			// 注意！由于动作间有过度，行走动作的当前动作可能是起身等动作，因此不能直接通过GetComponent<Walk>这种方式判断，应借由Dealer来结束
			actionDealer.CallingStop ();
		}

		void WhatNext ()
		{
			if (actionDealer.IsPlaying ())
				return;
			if (isPrincipal && isBeingControlled) {
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
			tempChaData = storylineManager.nameToCharacter [this.name];
			tempLandmark = LandmarkCollection.GetInstance ().Get (tempChaData.initial_position);
			if (Vector3.Distance (transform.position, tempLandmark.position) > distanceError
			    || Mathf.Abs (gameObject.transform.rotation.eulerAngles.y - tempLandmark.rotation.eulerAngles.y) % 360 > rotationError) {
				actionDealer.ApplyWalkAction (tempLandmark, true, null);
			} else {
				int num = Random.Range (0, 3);
				if (num == 0)
					tempStrings = tempChaData.spare_time_aid_sit;
				else if (num == 1)
					tempStrings = tempChaData.spare_time_aid_stand;
				else
					tempStrings = tempChaData.spare_time_aid_other;
				
				DealMainActionWithAid (actionDealer, tempChaData.spare_time_main_action, tempStrings, false, "?", "?", "?");
			}
		}


		void AssignNewPrincipalActivity ()
		{
			PrincipalActivity activity = principalActivities [0];
			if (isValidCompositeAction (activity.position, activity.action)) {
				SecondPerson other;
				for (int i = 0; i < activity.other_people.Length; ++i) {
					other = activity.other_people [i];
					if (storylineManager.nameToJob.ContainsKey (other.name)) {
						int index = 0;
						for (index = 0; index < secondChildren.Count; ++index) {
							if (secondChildren [index].name == other.name)
								break;
						}
						if (index < secondChildren.Count) {
							Log.warn ("Spot [" + spotName + "]'s principal activity [" + activity.description + "] job [" + other.name + "] overlapped: skipped");
						} else {
							string realName = storylineManager.nameToJobCandidateName [other.name];
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
				compositeMovement = GetCompositeMovementData (activity.position, activity.action);
				state = ComState.ARRIVING;
				StartCoroutine (WaitToBeMySecondChild ());
			} else {
				Log.warn ("Spot [" + spotName + "]'s principal activity [" + activity.description + "] has undefined composite movement [" + activity.action + "]: skipped");
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
			if (isValidCompositeAction (activity.position, activity.action)) {
				ThirdPerson other;
				for (int i = 0; i < activity.other_people.Length; ++i) {
					other = activity.other_people [i];
					if (storylineManager.nameToJob.ContainsKey (other.name)) {
						int index = 0;
						for (index = 0; index < thirdChildren.Count; ++index) {
							if (thirdChildren [index].name == other.name)
								break;
						}
						if (index < thirdChildren.Count) {
							Log.warn ("Spot [" + spotName + "]'s following activity [" + activity.description + "] job [" + other.name + "] overlapped: skipped");
						} else {
							string realName = storylineManager.nameToJobCandidateName [other.name];
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
				compositeMovement = GetCompositeMovementData (activity.position, activity.action);
				state = ComState.ARRIVING;
				StartCoroutine (WaitToBeMyThirdChild ());
			} else {
				Log.warn ("Spot [" + spotName + "]'s following activity [" + activity.description + "] has undefined composite movement [" + activity.action + "]: skipped");
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

		bool isValidCompositeAction (string position, string action)
		{
			if (position == "composite") {
				return storylineManager.nameToCompositeMovement.ContainsKey (action);
			} else if (action == "sit" || action == "stand") {
				return true;
			} else if (position == "sit" || position == "stand" || position == "other") {
				return true;
			}
			return false;
		}

		CompositeMovementData GetCompositeMovementData (string position, string action)
		{
			if (position == "composite") {
				return storylineManager.nameToCompositeMovement [action];
			} else {
				if (action == "sit" || action == "stand")
					tempComMovement.mainrole_main = action;
				else
					tempComMovement.mainrole_main = position + "_" + action;
				return tempComMovement;
			}
		}

	

		void DealOwnActivity ()
		{
			tempSelf = isPrincipal ? currentPrincipalActivity.self : currentFollowingActivity.self;

			switch (state) {
			case ComState.ARRIVING:
				if (IsAtProperLocation (tempSelf, this, true)) {
					if (CheckEveryOneInPosition ()) {
						state = ComState.ARRIVINGSTOP;
					} else {
						DealMainActionWithAid (actionDealer, compositeMovement.wait_mainrole_main, compositeMovement.wait_mainrole_aid, true, name, "执行等待动作", "执行等待动作(辅助)");
					}
				} else {
					actionDealer.StopCountingAid ();
				}
				break;

			case ComState.ARRIVINGSTOP:
				actionDealer.StopCountingAid ();
				if (CheckEveryOneEndAction ()) {
					state = ComState.PREPARING;
					actionListIndex = -1;
					for (int i = 0; i < children.Count; ++i) {
						tempPerson = children [i];
						tempPerson.actionDealer.StopCountingAid ();
						tempPerson.actionListIndex = -1;
					}
					LogTimeToStartOrEnd ("正式开始");
				}
				break;

			case ComState.PREPARING:
			// 如果有开始动作，并且开始动作未执行完，继续执行
				tempHasAction = DealListAction (actionDealer, compositeMovement.start_mainrole_main, ref actionListIndex, true, name, "执行开始动作");
			// 如果没有开始动作，或已经执行完，判断他人是否完成，否则继续
				if (!tempHasAction && compositeMovement.start_otherroles_main != null) {
					for (int i = 0; i < children.Count; ++i) {
						tempPerson = children [i];
						if (tempPerson.actionListIndex < compositeMovement.start_otherroles_main.Length) {
							tempHasAction = true;
							break;
						}
					}
				}
			// 全部完成，进入下一状态
				if (!tempHasAction)
					state = ComState.PREPARINGSTOP;
				break;

			case ComState.PREPARINGSTOP:
				if (CheckEveryOneEndAction ()) {
					state = ComState.STARTING;
					bubbleListIndex = 0;
					for (int i = 0; i < children.Count; ++i) {
						children [i].bubbleListIndex = 0;
					}
				}
				break;

			case ComState.STARTING:
				DealMainActionWithAid (actionDealer, compositeMovement.mainrole_main, compositeMovement.mainrole_aid, true, name, "执行主要动作", "执行主要动作(辅助)");
				break;

			case ComState.STARTINGSTOP:
				actionDealer.StopCountingAid ();
				if (CheckEveryOneEndAction ()) {
					state = ComState.ENDING;
					actionListIndex = -1;
					for (int i = 0; i < children.Count; ++i) {
						tempPerson = children [i];
						tempPerson.actionDealer.StopCountingAid ();
						tempPerson.actionListIndex = -1;
						ActionManager.GetInstance ().ApplyIdleAction (tempPerson.gameObject, "", Random.Range (0.0f, randomDiff), null);
					}
					LogTimeToStartOrEnd ("正式结束");
				}
				break;

			case ComState.ENDING:
				// 如果有结束动作，并且结束动作未执行完，继续执行
				tempHasAction = DealListAction (actionDealer, compositeMovement.end_mainrole_main, ref actionListIndex, true, name, "执行结束动作");
				// 如果没有结束动作，或已经执行完，判断他人是否完成，否则继续
				if (!tempHasAction && compositeMovement.end_otherroles_main != null) {
					for (int i = 0; i < children.Count; ++i) {
						tempPerson = children [i];
						if (tempPerson.actionListIndex < compositeMovement.end_otherroles_main.Length) {
							tempHasAction = true;
							break;
						}
					}
				}
				// 全部完成，进入下一状态
				if (!tempHasAction)
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
						children [i].AddFollowingActivities (secondChildren [i].following_actions);
						children [i].isBeingControlled = false;
						children [i].hasArrived = false;
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
				hasArrived = false;
				break;
			}
		}


		void DealChildActivity ()
		{  
			tempOthers.Clear ();
			if (isPrincipal) {
				for (int i = 0; i < secondChildren.Count; ++i)
					tempOthers.Add (secondChildren [i] as Self);
			} else {
				for (int i = 0; i < thirdChildren.Count; ++i)
					tempOthers.Add (thirdChildren [i] as Self);
			}

			for (int i = 0; i < tempOthers.Count; ++i) {
				tempPerson = children [i];
				if (tempPerson.parent != this)
					continue;
				if (tempPerson.actionDealer.IsPlaying ())
					continue;
			
				switch (state) {
				case ComState.ARRIVING:
					if (IsAtProperLocation (tempOthers [i], tempPerson, true)) {
						DealMainActionWithAid (tempPerson.actionDealer, compositeMovement.wait_otherroles_main, compositeMovement.wait_otherroles_aid, true, tempPerson.name, "执行等待动作", "执行等待动作(辅助)");
					} else {
						tempPerson.actionDealer.StopCountingAid ();
					}
					break;
				case ComState.ARRIVINGSTOP:
					break;
				case ComState.PREPARING:
					DealListAction (tempPerson.actionDealer, compositeMovement.start_otherroles_main, ref tempPerson.actionListIndex, true, tempPerson.name, "执行开始动作");
					break;
				case ComState.PREPARINGSTOP:
					break;
				case ComState.STARTING:
					DealMainActionWithAid (tempPerson.actionDealer, compositeMovement.otherroles_main, compositeMovement.otherroles_aid, true, tempPerson.name, "执行主要动作", "执行主要动作(辅助)");
					break;
				case ComState.STARTINGSTOP:
					break;
				case ComState.ENDING:
					DealListAction (tempPerson.actionDealer, compositeMovement.end_otherroles_main, ref tempPerson.actionListIndex, true, tempPerson.name, "执行结束动作");
					break;
				case ComState.ENDINGSTOP:
					break;
				case ComState.LEAVING:
					break;
				}
			}
		}



		void DealMainActionWithAid (NetworkActionDealer actionDealer, string main, string[] aid, bool showInfo, string personName, string mainInfo, string aidInfo)
		{
			if (aid == null || aid.Length == 0) {
				actionDealer.ApplyAction (main, null);

				//if (showInfo && !string.IsNullOrEmpty (main))
				//	Log.info (Log.yellow ("【" + personName + "】 ") + mainInfo + Log.blue (" 【" + main + "】"));

			} else {
				if (actionDealer.IsAidActive ()) {
					int index = Random.Range (0, aid.Length);
					actionDealer.ApplyAction (aid [index], null);

					//if (showInfo && !string.IsNullOrEmpty (aid [index]))
					//	Log.info (Log.yellow ("【" + personName + "】 ") + aidInfo + Log.blue (" 【" + aid [index] + "】"));

				} else {
					actionDealer.ApplyAction (main, null);

					//if (showInfo && !string.IsNullOrEmpty (main))
					//	Log.info (Log.yellow ("【" + personName + "】 ") + mainInfo + Log.blue (" 【" + main + "】"));

				}
				actionDealer.StartCountingAid ();
			}
		}

		bool DealListAction (NetworkActionDealer actionDealer, string[] list, ref int index, bool info, string personName, string mainInfo)
		{
			if (list != null) {
				index++;
				if (index < list.Length) {
					string actionName = list [index];
					actionDealer.ApplyAction (actionName, null);

					//if (info && !string.IsNullOrEmpty (actionName))
					//	Log.info (Log.yellow ("【" + personName + "】 ") + mainInfo + Log.blue (" 【" + actionName + "】"));
					
					return true;
				}
			}
			return false;
		}


		bool IsAtProperLocation (Self self, Person person, bool doAction)
		{
			tempChaData2 = storylineManager.nameToCharacter [person.gameObject.name];
			switch (self.location_to_type) {
			case 0:	// stand by
				return true;
			case 1:	// labeled location
				if (string.IsNullOrEmpty (self.location_to))
					return true;
				tempDestination = LandmarkCollection.GetInstance ().Get (self.location_to);
				if (tempDestination == null || Vector3.Distance (person.transform.position, tempDestination.position) <= distanceError && Mathf.Abs (person.transform.rotation.eulerAngles.y - tempDestination.rotation.eulerAngles.y) % 360 < rotationError) {
					if (doAction) {
						person.transform.position = tempDestination.position;
						person.transform.rotation = tempDestination.rotation;
					}
					if (!person.hasArrived) {
						person.hasArrived = true;
						LogTimeToActionPlace (person);
					}
					return true;
				} else if (doAction) {
					person.actionDealer.ApplyWalkAction (tempDestination, true, null);
				}
				return false;
			case 2:	// initial location
				tempDestination = LandmarkCollection.GetInstance ().Get (tempChaData2.initial_position);
				if (tempDestination == null || Vector3.Distance (person.transform.position, tempDestination.position) <= distanceError && Mathf.Abs (person.transform.rotation.eulerAngles.y - tempDestination.rotation.eulerAngles.y) % 360 < rotationError) {
					if (doAction) {
						person.transform.position = tempDestination.position;
						person.transform.rotation = tempDestination.rotation;
					}
					if (!person.hasArrived) {
						person.hasArrived = true;
						LogTimeToActionPlace (person);
					}
					return true;
				} else if (doAction) {
					person.actionDealer.ApplyWalkAction (tempDestination, true, null);
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
				if (!IsAtProperLocation (currentPrincipalActivity.self, this, false))
					return false;
				for (int i = 0; i < secondChildren.Count; ++i)
					if (children [i].parent != this || !IsAtProperLocation (secondChildren [i], children [i], false))
						return false;
				return true;
			} else {
				if (!IsAtProperLocation (currentFollowingActivity.self, this, false))
					return false;
				for (int i = 0; i < thirdChildren.Count; ++i)
					if (children [i].parent != this || !IsAtProperLocation (thirdChildren [i], children [i], false))
						return false;
				return true;
			}
		}


		bool CheckEveryOneEndAction ()
		{
			if (actionDealer.IsPlaying ())
				return false;
			for (int i = 0; i < children.Count; ++i)
				if (children [i].parent != this || children [i].actionDealer.IsPlaying ())
					return false;
			return true;
		}


		// 0-无气泡 1-对话气泡 2-关键词气泡 3-icon气泡 4-screen
		void DisplayBubble (Person person, Self self, ref int index)
		{
			if (person != null && self != null)
				DisplayBubble (person, self.bubble_content, self.bubble_direction, ref index);
		}

		void DisplayBubble (Person person, string[] bubble_content, int bubble_direction, ref int index)
		{
			if (person == null || bubble_content == null || bubble_content.Length == 0)
				return;
			if (index < 0 || index >= bubble_content.Length)
				index = Mathf.Clamp (index, 0, bubble_content.Length - 1);
			// 解析格式: num_name
			string str = bubble_content [index];
			index = (index + 1) % bubble_content.Length;
			int i = str.IndexOf ('_');
			if (i <= 0)
				return;
			string num = str.Substring (0, i);
			string content = str.Substring (i + 1);
			int bubble_type = int.Parse (num);
			// 分类
			switch (bubble_type) {
			case 1:
				person.bubbleDealer.NetworkChatBubble (content, 8f, bubble_direction);
				break;
			case 2:
				person.bubbleDealer.NetworkKeywordBubble (content, 8f);
				break;
			case 3:
				person.bubbleDealer.NetworkIconBubble (content, 8f);
				break;
			case 4:
				person.bubbleDealer.NetworkScreenBubble (content, 8f, bubble_direction);
				break;
			}
		}

		void DealBubbleActivity ()
		{
			tempSelf = isPrincipal ? currentPrincipalActivity.self : currentFollowingActivity.self;
			if (!actionDealer.IsBubbling ())
				DisplayBubble (this, tempSelf, ref bubbleListIndex);
			for (int i = 0; i < children.Count; ++i) {
				if (!children [i].actionDealer.IsBubbling ()) {
					if (isPrincipal) {
						DisplayBubble (children [i], secondChildren [i], ref children [i].bubbleListIndex);
					} else {
						DisplayBubble (children [i], thirdChildren [i], ref children [i].bubbleListIndex);
					}
				}
			}
		}


		void LogTimeToActionPlace (Person person)
		{
			if (isPrincipal) {
				if (currentPrincipalActivity.action != null)
					Log.info (storylineManager.GetPartTime (person.spotName) + Log.yellow ("【" + person.name + "】") + "到达" + Log.blue ("【" + currentPrincipalActivity.action + "】") + "需要的地点");
				else
					Log.info (storylineManager.GetPartTime (person.spotName) + Log.yellow ("【" + person.name + "】") + "到达" + Log.blue ("【" + currentPrincipalActivity.position + "】") + "需要的地点");
			} else {
				if (currentFollowingActivity.action != null)
					Log.info (storylineManager.GetPartTime (person.spotName) + Log.yellow ("【" + person.name + "】") + "到达" + Log.blue ("【" + currentFollowingActivity.action + "】") + "需要的地点");
				else
					Log.info (storylineManager.GetPartTime (person.spotName) + Log.yellow ("【" + person.name + "】") + "到达" + Log.blue ("【" + currentFollowingActivity.position + "】") + "需要的地点");
			}
		}


		void LogTimeToStartOrEnd (string info)
		{
			if (isPrincipal) {
				if (currentPrincipalActivity.action != null)
					Log.info (storylineManager.GetPartTime (spotName) + Log.pink ("【" + spotName + "】") + Log.blue ("【" + currentPrincipalActivity.action + "】") + info);
				else
					Log.info (storylineManager.GetPartTime (spotName) + Log.pink ("【" + spotName + "】") + Log.blue ("【" + currentPrincipalActivity.position + "】") + info);
			} else {
				if (currentFollowingActivity.action != null)
					Log.info (storylineManager.GetPartTime (spotName) + Log.pink ("【" + spotName + "】") + Log.blue ("【" + currentFollowingActivity.action + "】") + info);
				else
					Log.info (storylineManager.GetPartTime (spotName) + Log.pink ("【" + spotName + "】") + Log.blue ("【" + currentFollowingActivity.position + "】") + info);
			}
		}


		void Update ()
		{
			if (isRandomPerson) {
				DealWithRandomActivity ();
				return;
			}

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
				DealBubbleActivity ();
			} else {
				compositeTiming = 0.0f;
			}
		}



		#region random

		// 添加的需求：人物随机行动
		public bool isRandomPerson;
		public string randomScene;
		public RandomAction randomAction;
		public Landmark randomLandmark;

		private float randomMaxTime;
		private float randomCountTime;
		private bool randomTick;

		public void SetAsRandomPerson (string randomScene)
		{
			isRandomPerson = true;
			this.randomScene = randomScene;
		}

		void DealWithRandomActivity ()
		{
			if (randomTick) {
				randomCountTime += Time.deltaTime;
				if (!actionDealer.IsBubbling () && randomLandmark != null) {
					DisplayBubble (this, randomLandmark.m_bubble_content, randomLandmark.m_bubble_direction, ref bubbleListIndex);
				}
			}
			if (actionDealer.IsPlaying ())
				return;
			if (randomCountTime >= randomMaxTime) {
				actionDealer.StopCountingAid ();
				randomCountTime = 0;
				randomTick = false;
				randomAction = storylineManager.AskForNewLocation (randomScene, this, ref randomLandmark, ref randomMaxTime);
				actionDealer.ApplyWalkAction (randomLandmark, true, null);
			} else if (randomTick) {
				if (randomAction != null)
					DealMainActionWithAid (actionDealer, randomAction.main, randomAction.aid, false, "", "", "");
			} else if (!randomTick) {
				if (Vector3.Distance (transform.position, randomLandmark.position) < distanceError)
					randomTick = true;
			}
			return;
		}

		#endregion
	}
}