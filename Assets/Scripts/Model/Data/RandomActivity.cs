using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class RandomActivity
	{
		public string randomscene;
		public string[] randomperson;
		public string[] randomlocation;
		public RandomAction[] randomaction;
	}
}