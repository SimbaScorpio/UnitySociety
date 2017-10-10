using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class StorylineManager : MonoBehaviour
	{
		public Storyline storyline;
		public Dictionary<string, GameObject> nameToCharacterObj;
		public Dictionary<string, CharacterData> nameToCharacter;
		public Dictionary<string, CompositeMovementData> nameToCompositeMovement;
		public Dictionary<string, InitCharacterData> nameToJob;
		public Dictionary<string, string> nameToJobCandidateName;
		public Dictionary<string, SceneData> nameToScene;
		public Dictionary<string, SceneState> nameToSceneState;
		public Dictionary<string, HashSet<string>> nameToSceneCandidateNames;


		//public GameObject PlayerPrefab;

		private float time = 0.0f;
		private bool isTicking = false;

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
		}

		public void Initialize ()
		{
			Log.info (Log.blue ("初始化角色..."));
			InitializeCharacters ();
			Log.info (Log.blue ("初始化组合动作..."));
			InitializeCompositeMovements ();
			Log.info (Log.blue ("初始化岗位..."));
			InitializeJobs ();
			Log.info (Log.blue ("进行岗位分配..."));
			RandomlyArrangeJobCandidates ();
			Log.info (Log.blue ("初始化故事节点..."));
			InitializeScenes ();
			Log.info (Log.blue ("开始！"));
			Tick ();
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
			int chaNum = storyline.characterdata.Length;
			for (int i = 0; i < chaNum; ++i) {
				CharacterData cha = storyline.characterdata [i];
				if (string.IsNullOrEmpty (cha.name)) {
					Log.error ("Initialize character [" + i + "] failed: empty name");
					continue;
				}
				Landmark initialLocation = LandmarkCollection.GetInstance ().Get (cha.initial_position);
				//GameObject player = Instantiate (PlayerPrefab, initialLocation.position, initialLocation.rotation) as GameObject;
				//Material clothing = MaterialCollection.GetInstance ().Get (cha.clothing);
				//player.transform.Find ("mesh").GetComponent<Renderer> ().material = clothing;
				//player.name = cha.name;
				GameObject player = NetworkServerAISpawner.GetInstance ().Spawn (cha, initialLocation.position, initialLocation.rotation);

				if (nameToCharacter.ContainsKey (cha.name)) {
					Log.warn ("Initialize character [" + i + "] warning: overlapped name");
				}
				nameToCharacter [cha.name] = cha;
				nameToCharacterObj [cha.name] = player;

				// init action string
				ChangeHeader (cha.spare_time_main_position, ref cha.spare_time_main_action);
				for (int j = 0; j < cha.spare_time_aid_sit.Length; ++j) {
					ChangeHeader ("sit", ref cha.spare_time_aid_sit [j]);
				}
				for (int j = 0; j < cha.spare_time_aid_stand.Length; ++j) {
					ChangeHeader ("stand", ref cha.spare_time_aid_stand [j]);
				}
				for (int j = 0; j < cha.spare_time_aid_other.Length; ++j) {
					ChangeHeader ("else", ref cha.spare_time_aid_other [j]);
				}
			}
		}

		void InitializeCompositeMovements ()
		{
			int comNum = storyline.compositemovementdata.Length;
			for (int i = 0; i < comNum; ++i) {
				CompositeMovementData com = storyline.compositemovementdata [i];
				if (string.IsNullOrEmpty (com.name)) {
					Log.error ("Initialize composite movement [" + i + "] failed: empty name");
					continue;
				}
				if (nameToCompositeMovement.ContainsKey (com.name)) {
					Log.warn ("Initialize composite movement [" + i + "] warning: overlapped name");
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

		void ChangeHeader (string header, ref string main)
		{
			if (main == "sit" || main == "stand")
				return;
			main = header + "_" + main;
		}

		void InitializeJobs ()
		{
			int jobNum = storyline.initcharacterdata.Length;
			for (int i = 0; i < jobNum; ++i) {
				InitCharacterData job = storyline.initcharacterdata [i];
				if (string.IsNullOrEmpty (job.name)) {
					Log.error ("Initialize job [" + i + "] failed: empty name");
					continue;
				}
				if (nameToJob.ContainsKey (job.name)) {
					Log.warn ("Initialize job [" + i + "] warning: overlapped name");
				}
				nameToJob [job.name] = job;
			}
		}

		public void RandomlyArrangeJobCandidates ()
		{
			List<string> names = new List<string> ();
			foreach (string name in nameToJob.Keys) {
				names.Add (name);
			}
			nameToJobCandidateName.Clear ();
			if (!RecursivelyArrangeJobCandidate (names, 0)) {
				Log.error ("Select job candidates failed: impossible combination");
			}
		}

		bool RecursivelyArrangeJobCandidate (List<string> jobNames, int i)
		{
			if (i >= jobNames.Count)
				return true;
		
			InitCharacterData job = nameToJob [jobNames [i]];
			if (job.candidates.Length == 0) {
				Log.warn ("Select job candidate [" + jobNames [i] + "] warning: no candidates");
				return RecursivelyArrangeJobCandidate (jobNames, i + 1);
			}

			List<string> possibleChaNames = new List<string> ();
			foreach (string chaName in job.candidates) {
				if (!nameToCharacter.ContainsKey (chaName)) {
					Log.warn ("Character [" + chaName + "] in job [" + jobNames [i] + "] is illegal: cannot find");
					continue;
				}
				if (!nameToJobCandidateName.ContainsValue (chaName))
					possibleChaNames.Add (chaName);
			}
			if (possibleChaNames.Count == 0) {
				nameToJobCandidateName.Remove (jobNames [i]);
				return false;
			}

			int index = Random.Range (0, possibleChaNames.Count);
			nameToJobCandidateName [jobNames [i]] = possibleChaNames [index];
			while (!RecursivelyArrangeJobCandidate (jobNames, i + 1)) {
				possibleChaNames.RemoveAt (index);
				if (possibleChaNames.Count == 0) {
					nameToJobCandidateName.Remove (jobNames [i]);
					return false;
				}
				index = Random.Range (0, possibleChaNames.Count);
				nameToJobCandidateName [jobNames [i]] = possibleChaNames [index];
			}
			return true;
		}

		void InitializeScenes ()
		{
			int spotNum = storyline.scenedata.Length;
			for (int i = 0; i < spotNum; ++i) {
				SceneData spot = storyline.scenedata [i];
				if (string.IsNullOrEmpty (spot.spot_name)) {
					Log.error ("Initialize spot [" + i + "] failed: empty name");
					continue;
				}
				if (string.IsNullOrEmpty (spot.principal)) {
					Log.error ("Spot [" + spot.spot_name + "] misses principal: skipped");
					continue;
				}
				if (!nameToJob.ContainsKey (spot.principal)) {
					Log.error ("Spot [" + spot.spot_name + "] has undefined principal [" + spot.principal + "] : skipped");
					continue;
				}
				if (nameToScene.ContainsKey (spot.spot_name)) {
					Log.warn ("Initialize spot [" + i + "] warning: overlapped name");
				}
				nameToScene [spot.spot_name] = spot;
				nameToSceneState [spot.spot_name] = SceneState.READY;
				nameToSceneCandidateNames [spot.spot_name] = new HashSet<string> ();
			}
		}

		void StartStorylineSpot (string spotName)
		{
			SceneData spot = nameToScene [spotName];
			string name = nameToJobCandidateName [spot.principal];
			GameObject obj = nameToCharacterObj [name];
			Person person = obj.GetComponent<Person> ();
			bool success = person.AddPrincipalActivities (spot.principal_activity, spotName);
			if (success) {
				nameToSceneState [spotName] = SceneState.STARTED;
				Log.info ("开始故事节点 " + Log.pink ("【" + spotName + "】"));
				JoinStorylineSpot (name, spotName);
			}
		}

		public void JoinStorylineSpot (string candidate, string spotName)
		{
			nameToSceneCandidateNames [spotName].Add (candidate);
			Log.info (Log.yellow ("【" + candidate + "】") + " 加入故事节点 " + Log.pink ("【" + spotName + "】"));
		}

		public void QuitStorylineSpot (string candidate, string spotName)
		{
			HashSet<string> set = nameToSceneCandidateNames [spotName];
			if (set.Remove (candidate)) {
				Log.info (Log.yellow ("【" + candidate + "】") + " 退出故事节点 " + Log.pink ("【" + spotName + "】"));
				if (set.Count == 0) {
					nameToSceneState [spotName] = SceneState.ENDED;
					Log.info ("结束故事节点 " + Log.pink ("【" + spotName + "】"));
				}
			}
		}

		void KillStorylineSpot (string spotName)
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
			Log.info ("杀死故事节点 " + Log.pink ("【" + spotName + "】"));
		}


		public void Tick ()
		{
			time = 0.0f;
			isTicking = true;
		}

		void Update ()
		{
			if (!isTicking)
				return;
			time += Time.deltaTime;
			foreach (string spotName in nameToScene.Keys) {
				SceneData spot = nameToScene [spotName];
				if (time >= spot.start_time && time < spot.end_time) {
					if (nameToSceneState [spotName] == SceneState.READY) {
						StartStorylineSpot (spotName);
					}
				} else if (time >= spot.end_time) {
					if (nameToSceneState [spotName] == SceneState.STARTED) {
						KillStorylineSpot (spotName);
					}
				}
			}
		}
	}
}