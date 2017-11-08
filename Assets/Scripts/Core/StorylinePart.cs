using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	/***
	 * StorylinePart 是单独的故事线，管理该故事线的场景开始和结束
	 */
	public class StorylinePart : MonoBehaviour
	{
		public string fileName;
		public Storyline storyline;

		private StorylineManager sm;

		public float time = 0.0f;
		private bool isTicking = false;

		void Awake ()
		{
			sm = StorylineManager.GetInstance ();
		}

		public void Restart ()
		{
			StartCoroutine (WaitJobArrangementToStart ());
		}

		void Update ()
		{
			// isTicking 实际上是用来让Manager调用的，保证数据统筹完成才开始
			if (!isTicking || storyline.scenedata.Length == 0)
				return;
			time += Time.deltaTime;

			// 通过自身的spotName，获取Manager的SceneData，,因为假设不知道Manager如何处理不同故事线间重叠的场景命名
			foreach (SceneData scene in storyline.scenedata) {
				string spotName = scene.spot_name;
				if (!sm.nameToScene.ContainsKey (spotName))
					continue;	// 该场景不合格
				SceneData spot = sm.nameToScene [spotName];
				if (spot != scene)
					continue;	// 该场景被覆盖
				if (time >= spot.start_time && time < spot.end_time) {
					if (sm.nameToSceneState [spotName] == SceneState.READY) {
						sm.StartStorylineSpot (spotName);
					}
				} else if (time >= spot.end_time) {
					if (sm.nameToSceneState [spotName] == SceneState.STARTED) {
						sm.KillStorylineSpot (spotName);
					}
				}
			}
		}

		public void CheckLoopStoryline ()
		{
			List<string> spotNames = new List<string> ();
			foreach (SceneData scene in storyline.scenedata) {
				string spotName = scene.spot_name;
				if (!sm.nameToScene.ContainsKey (spotName))
					continue;	// 该场景不合格
				SceneData spot = sm.nameToScene [spotName];
				if (spot != scene)
					continue;	// 该场景被覆盖
				spotNames.Add (spotName);
				SceneState state = sm.nameToSceneState [spotName];
				if (state == SceneState.READY || state == SceneState.STARTED)
					return;
			}
			foreach (string spotName in spotNames) {
				sm.nameToSceneState [spotName] = SceneState.READY;
			}
			Restart ();
		}

		IEnumerator WaitJobArrangementToStart ()
		{
			WaitForSeconds wait = new WaitForSeconds (2);
			while (!RandomlyArrangeJobCandidates ()) {
				yield return wait;
			}
			time = 0.0f;
			isTicking = true;
			Log.info (Log.green ("【" + fileName + "故事线】") + "置零开始");
		}


		public bool RandomlyArrangeJobCandidates ()
		{
			Log.info ("【" + fileName + "】重新分配岗位...");

			List<string> names = new List<string> ();
			foreach (InitCharacterData initdata in storyline.initcharacterdata) {
				string jobName = initdata.name;
				if (!sm.nameToJob.ContainsKey (jobName))
					continue;	// 该岗位不合格
				InitCharacterData job = sm.nameToJob [jobName];
				if (job != initdata)
					continue; 	// 该岗位被覆盖
				names.Add (jobName);
				sm.nameToJobCandidateName.Remove (jobName);
			}

			if (!RecursivelyArrangeJobCandidate (names, 0)) {
				Log.error ("【" + fileName + "】岗位分配失败：不可能的组合");
				return false;
			}
			Log.info ("【" + fileName + "】岗位分配成功！");
			return true;
		}

		bool RecursivelyArrangeJobCandidate (List<string> jobNames, int i)
		{
			if (i >= jobNames.Count)
				return true;

			InitCharacterData job = sm.nameToJob [jobNames [i]];
			if (job.candidates.Length == 0) {
				Log.warn ("【" + fileName + "】岗位【" + jobNames [i] + "】警告: 没有参与者");
				return RecursivelyArrangeJobCandidate (jobNames, i + 1);
			}

			List<string> possibleChaNames = new List<string> ();
			foreach (string chaName in job.candidates) {
				if (!sm.nameToCharacter.ContainsKey (chaName)) {
					Log.warn ("【" + fileName + "】岗位【" + jobNames [i] + "】角色【" + chaName + "】警告：未定义");
					continue;
				}
				if (!sm.nameToJobCandidateName.ContainsValue (chaName))
					possibleChaNames.Add (chaName);
			}
			if (possibleChaNames.Count == 0) {
				sm.nameToJobCandidateName.Remove (jobNames [i]);
				return false;
			}

			int index = Random.Range (0, possibleChaNames.Count);
			sm.nameToJobCandidateName [jobNames [i]] = possibleChaNames [index];
			while (!RecursivelyArrangeJobCandidate (jobNames, i + 1)) {
				possibleChaNames.RemoveAt (index);
				if (possibleChaNames.Count == 0) {
					sm.nameToJobCandidateName.Remove (jobNames [i]);
					return false;
				}
				index = Random.Range (0, possibleChaNames.Count);
				sm.nameToJobCandidateName [jobNames [i]] = possibleChaNames [index];
			}
			return true;
		}
	}
}