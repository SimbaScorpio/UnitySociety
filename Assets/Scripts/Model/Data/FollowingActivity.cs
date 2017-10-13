using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class FollowingActivity
	{
		public float duration;
		public string description;
		public string position;
		public string action;
		public Self self;
		public ThirdPerson[] other_people;
	}
}