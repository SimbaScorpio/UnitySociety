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
		public string position;
		public string action;
		public Self self;
		public int num_other_people;
		public SecondPerson[] other_people;
	}
}