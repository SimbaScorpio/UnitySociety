using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class SecondPerson : Self
	{
		public string name;
		public int has_following_actions;
		public FollowingActivity[] following_actions;
	}
}