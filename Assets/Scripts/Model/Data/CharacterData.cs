using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class CharacterData
	{
		public string name;
		public string initial_position;
		public string body_type;
		public string clothing;
		public string spare_time_main_position;
		public string spare_time_main_action;
		public string[] spare_time_aid_sit;
		public string[] spare_time_aid_stand;
		public string[] spare_time_aid_other;
	}
}