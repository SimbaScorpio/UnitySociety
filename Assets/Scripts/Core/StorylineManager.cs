using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	/***
	 * StorylineManager 集合所有单独的故事线，用以构建查询字典
	 */
	public class StorylineManager : MonoBehaviour
	{
		public Dictionary<string, GameObject> nameToCharacterObj;
		public Dictionary<string, CharacterData> nameToCharacter;
		public Dictionary<string, CompositeMovementData> nameToCompositeMovement;
		public Dictionary<string, InitCharacterData> nameToJob;
		public Dictionary<string, string> nameToJobCandidateName;
		public Dictionary<string, SceneData> nameToScene;
		public Dictionary<string, SceneState> nameToSceneState;
		public Dictionary<string, HashSet<string>> nameToSceneCandidateNames;
		public Dictionary<string, StorylinePart> nameToStorylinePart;
	
		public List<StorylinePart> storylineParts = new List<StorylinePart> ();

		private static StorylineManager instance;

		public static StorylineManager GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		void InitializeVariables ()
		{
			nameToCharacterObj = new Dictionary<string, GameObject> ();
			nameToCharacter = new Dictionary<string, CharacterData> ();
			nameToCompositeMovement = new Dictionary<string, CompositeMovementData> ();
			nameToJob = new Dictionary<string, InitCharacterData> ();
			nameToJobCandidateName = new Dictionary<string, string> ();
			nameToScene = new Dictionary<string, SceneData> ();
			nameToSceneState = new Dictionary<string, SceneState> ();
			nameToSceneCandidateNames = new Dictionary<string, HashSet<string>> ();
			nameToStorylinePart = new Dictionary<string, StorylinePart> ();
		}

		public void AddStorylinePart (StorylinePart part)
		{
			if (!storylineParts.Contains (part)) {
				storylineParts.Add (part);
			}
		}

		public void ClearStorylinePart ()
		{
			for (int i = 0; i < storylineParts.Count; ++i)
				Destroy (storylineParts [i]);
			storylineParts.Clear ();
		}

		public void Initialize ()
		{
			Log.info (Log.blue ("初始化角色..."));
			InitializeCharacters ();
			Log.info (Log.blue ("初始化组合动作..."));
			InitializeCompositeMovements ();
			Log.info (Log.blue ("初始化岗位..."));
			InitializeJobs ();
			Log.info (Log.blue ("初始化场景..."));
			InitializeScenes ();
			Log.info (Log.blue ("开始！"));
			for (int i = 0; i < storylineParts.Count; ++i) {
				storylineParts [i].Restart ();
			}
		}

		public void Restart ()
		{
			GameObject[] objs = GameObject.FindGameObjectsWithTag ("Player");
			for (int i = 0; i < objs.Length; ++i) {
				Destroy (objs [i]);
			}
			InitializeVariables ();
			Initialize ();
		}

		void InitializeCharacters ()
		{
			foreach (StorylinePart part in storylineParts) {
				if (part.storyline.characterdata == null)
					continue;
				Log.info ("初始化文件【" + part.fileName + "】角色...");
				foreach (CharacterData cha in part.storyline.characterdata) {
					if (string.IsNullOrEmpty (cha.name)) {
						Log.error ("初始化角色1个失败: 空命名");
						continue;
					}
					if (nameToCharacter.ContainsKey (cha.name)) {
						Log.warn ("初始化角色【" + cha.name + "】警告: 重复的命名");
					}
					nameToCharacter [cha.name] = cha;

					// init action string
					ChangeHeader (cha.spare_time_main_position, ref cha.spare_time_main_action);
					for (int j = 0; j < cha.spare_time_aid_sit.Length; ++j) {
						ChangeHeader ("sit", ref cha.spare_time_aid_sit [j]);
					}
					for (int j = 0; j < cha.spare_time_aid_stand.Length; ++j) {
						ChangeHeader ("stand", ref cha.spare_time_aid_stand [j]);
					}
					for (int j = 0; j < cha.spare_time_aid_other.Length; ++j) {
						ChangeHeader ("other", ref cha.spare_time_aid_other [j]);
					}

				}
			}
			foreach (string name in nameToCharacter.Keys) {
				CharacterData cha = nameToCharacter [name];
				Landmark initialLocation = LandmarkCollection.GetInstance ().Get (cha.initial_position);
				GameObject player = NetworkServerAISpawner.GetInstance ().Spawn (cha, initialLocation.position, initialLocation.rotation);
				nameToCharacterObj [cha.name] = player;
			}
		}

		void InitializeCompositeMovements ()
		{
			foreach (StorylinePart part in storylineParts) {
				if (part.storyline.compositemovementdata == null)
					continue;
				Log.info ("初始化文件【" + part.fileName + "】组合动作...");
				foreach (CompositeMovementData com in part.storyline.compositemovementdata) {
					if (string.IsNullOrEmpty (com.name)) {
						Log.error ("初始化组合动作1个失败: 空命名");
						continue;
					}
					if (nameToCompositeMovement.ContainsKey (com.name)) {
						Log.warn ("初始化组合动作【" + com.name + "】警告：重复的命名");
					}
					nameToCompositeMovement [com.name] = com;

					// init composite string
					string main = com.mainrole_position;
					ChangeHeader (main, ref com.mainrole_main);
					ChangeHeader (main, ref com.wait_mainrole_main);
					for (int j = 0; j < com.mainrole_aid.Length; ++j) {
						ChangeHeader (main, ref com.mainrole_aid [j]);
					}
					for (int j = 0; j < com.wait_mainrole_aid.Length; ++j) {
						ChangeHeader (main, ref com.wait_mainrole_aid [j]);
					}
					for (int j = 0; j < com.start_mainrole_main.Length; ++j) {
						ChangeHeader (main, ref com.start_mainrole_main [j]);
					}
					for (int j = 0; j < com.end_mainrole_main.Length; ++j) {
						ChangeHeader (main, ref com.end_mainrole_main [j]);
					}

					string other = com.otherrole_position;
					ChangeHeader (other, ref com.otherroles_main);
					ChangeHeader (other, ref com.wait_otherroles_main);
					for (int j = 0; j < com.otherroles_aid.Length; ++j) {
						ChangeHeader (other, ref com.otherroles_aid [j]);
					}
					for (int j = 0; j < com.wait_otherroles_aid.Length; ++j) {
						ChangeHeader (other, ref com.wait_otherroles_aid [j]);
					}
					for (int j = 0; j < com.start_otherroles_main.Length; ++j) {
						ChangeHeader (other, ref com.start_otherroles_main [j]);
					}
					for (int j = 0; j < com.end_otherroles_main.Length; ++j) {
						ChangeHeader (other, ref com.end_otherroles_main [j]);
					}
				}
			}
		}

		void ChangeHeader (string header, ref string main)
		{
			if (main == "sit" || main == "stand" || string.IsNullOrEmpty (main))
				return;
			main = header + "_" + main;
		}

		void InitializeJobs ()
		{
			foreach (StorylinePart part in storylineParts) {
				if (part.storyline.initcharacterdata == null)
					continue;
				Log.info ("初始化文件【" + part.fileName + "】岗位...");
				foreach (InitCharacterData job in part.storyline.initcharacterdata) {
					if (string.IsNullOrEmpty (job.name)) {
						Log.error ("初始化岗位1个失败: 空命名");
						continue;
					}
					if (nameToJob.ContainsKey (job.name)) {
						Log.warn ("初始化岗位【" + job.name + "】警告：重复的命名");
					}
					nameToJob [job.name] = job;
				}
			}
		}

		void InitializeScenes ()
		{
			foreach (StorylinePart part in storylineParts) {
				if (part.storyline.scenedata == null)
					continue;
				Log.info ("初始化文件【" + part.fileName + "】场景...");
				foreach (SceneData spot in part.storyline.scenedata) {
					if (string.IsNullOrEmpty (spot.spot_name)) {
						Log.error ("初始化场景1个失败: 空命名");
						continue;
					}
					if (string.IsNullOrEmpty (spot.principal)) {
						Log.error ("初始化场景【" + spot.spot_name + "】失败：缺少主要岗位");
						continue;
					}
					if (!nameToJob.ContainsKey (spot.principal)) {
						Log.error ("初始化场景【" + spot.spot_name + "】失败：未定义的主要岗位【" + spot.principal + "】");
						continue;
					}
					if (nameToScene.ContainsKey (spot.spot_name)) {
						Log.warn ("初始化场景【" + spot.spot_name + "】警告：重复的命名");
					}
					nameToScene [spot.spot_name] = spot;
					nameToSceneState [spot.spot_name] = SceneState.READY;
					nameToSceneCandidateNames [spot.spot_name] = new HashSet<string> ();
					nameToStorylinePart [spot.spot_name] = part;
				}
			}
		}

		public void StartStorylineSpot (string spotName)
		{
			SceneData spot = nameToScene [spotName];
			string name = nameToJobCandidateName [spot.principal];
			GameObject obj = nameToCharacterObj [name];
			Person person = obj.GetComponent<Person> ();
			bool success = person.AddPrincipalActivities (spot.principal_activity, spotName);
			if (success) {
				nameToSceneState [spotName] = SceneState.STARTED;
				Log.info (GetPartTime (spotName) + Log.pink ("【" + spotName + "】") + "场景开始");
				JoinStorylineSpot (name, spotName);
			}
		}

		public void JoinStorylineSpot (string candidate, string spotName)
		{
			nameToSceneCandidateNames [spotName].Add (candidate);
			Log.info (GetPartTime (spotName) + Log.yellow ("【" + candidate + "】") + " 加入场景 " + Log.pink ("【" + spotName + "】"));
		}

		public void QuitStorylineSpot (string candidate, string spotName)
		{
			HashSet<string> set = nameToSceneCandidateNames [spotName];
			if (set.Remove (candidate)) {
				Log.info (GetPartTime (spotName) + Log.yellow ("【" + candidate + "】") + " 退出场景 " + Log.pink ("【" + spotName + "】"));
				if (set.Count == 0) {
					nameToSceneState [spotName] = SceneState.ENDED;
					Log.info (GetPartTime (spotName) + Log.pink ("【" + spotName + "】") + "场景结束");
					nameToStorylinePart [spotName].CheckLoopStoryline ();
				}
			}
		}

		public void KillStorylineSpot (string spotName)
		{
			HashSet<string> set = nameToSceneCandidateNames [spotName];
			foreach (string name in set) {
				GameObject obj = nameToCharacterObj [name];
				Person person = obj.GetComponent<Person> ();
				person.spotName = null;
				person.Stop ();
			}
			set.Clear ();
			nameToSceneState [spotName] = SceneState.KILLED;
			Log.info (GetPartTime (spotName) + Log.pink ("【" + spotName + "】") + "场景杀死");
			nameToStorylinePart [spotName].CheckLoopStoryline ();
		}

		public string GetPartTime (string spotName)
		{
			if (!nameToStorylinePart.ContainsKey (spotName))
				return "";
			int time = (int)nameToStorylinePart [spotName].time;
			return "【" + time + "】，";
		}
	}
}