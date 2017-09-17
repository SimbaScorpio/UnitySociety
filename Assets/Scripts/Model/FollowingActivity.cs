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
		public string composite_movement_name;
		public Self self;
		public ThirdPerson[] other_people;
	}
}