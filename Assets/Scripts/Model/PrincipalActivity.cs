using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class PrincipalActivity
	{
		public float duration;
		public string description;
		public string composite_movement_name;
		public Self self;
		public SecondPerson[] other_people;
	}
}