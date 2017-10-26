using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class AvatarInfo
	{
		public string name;
		public string job;
		public string scene;
	}

	public class ServerAIInfoManager : MonoBehaviour
	{
		private static ServerAIInfoManager instance;

		public static ServerAIInfoManager GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		public AvatarInfo GetAvatarInfo (string characterName)
		{
			AvatarInfo av = new AvatarInfo ();
			StorylineManager sm = StorylineManager.GetInstance ();
			if (!sm.nameToCharacter.ContainsKey (characterName))
				return av;
			CharacterData cd = sm.nameToCharacter [characterName];
			av.name = cd.name;
			// search scene
			foreach (string scene in sm.nameToSceneCandidateNames.Keys) {
				if (sm.nameToSceneCandidateNames [scene].Contains (cd.name)) {
					av.scene = scene;
				}
			}
			// search job
			foreach (string job in sm.nameToJobCandidateName.Keys) {
				if (sm.nameToJobCandidateName [job] == cd.name) {
					av.job = job;
				}
			}
			return av;
		}
	}
}