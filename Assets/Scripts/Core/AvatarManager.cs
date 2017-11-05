using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class AvatarInfo
	{
		public string name;
		public string job;
		public string scene;
		public List<string> recentActions;
	}

	public class AvatarManager : MonoBehaviour
	{
		private static AvatarManager instance;

		public static AvatarManager GetInstance ()
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
			// 姓名
			av.name = cd.name;
			// 场景
			foreach (string scene in sm.nameToSceneCandidateNames.Keys) {
				if (sm.nameToSceneCandidateNames [scene].Contains (cd.name))
					av.scene = scene;
			}
			// 职能
			foreach (string job in sm.nameToJobCandidateName.Keys) {
				if (sm.nameToJobCandidateName [job] == cd.name)
					av.job = job;
			}
			// 动作
			GameObject obj = sm.nameToCharacterObj [characterName];
			NetworkActionDealer ad = obj.GetComponent<NetworkActionDealer> ();
			if (ad != null)
				av.recentActions = ad.recentActions;
			return av;
		}

		public RenderTexture GetAvatarSnap (string characterName)
		{
			StorylineManager sm = StorylineManager.GetInstance ();
			if (!sm.nameToCharacter.ContainsKey (characterName))
				return null;
			CharacterData cd = sm.nameToCharacter [characterName];
			return AvatarSnapManager.GetInstance ().GetAvatar (cd);
		}

		public void RemoveAvatarSnap (string characterName)
		{
			AvatarSnapManager.GetInstance ().RemoveAvatar (characterName);
		}
	}
}