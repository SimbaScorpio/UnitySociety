using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class CharacterData
	{
		public string name;
		public string chinesename;
		public string job;
		public string initial_position;
		public string body_type;
		public string clothing;
		public int hair;
		public int glasses;
		public int bag;
		public string spare_time_main_position;
		public string spare_time_main_action;
		public string[] spare_time_aid_sit;
		public string[] spare_time_aid_stand;
		public string[] spare_time_aid_other;
	}
}