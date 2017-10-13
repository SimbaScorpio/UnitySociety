using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class CompositeMovementData
	{
		public string name;
		public string mainrole_position;
		public string mainrole_main;
		public string[] mainrole_aid = new string[0];
		public string wait_mainrole_main;
		public string[] wait_mainrole_aid = new string[0];
		public string[] start_mainrole_main = new string[0];
		public string[] end_mainrole_main = new string[0];

		public string otherrole_position;
		public string otherroles_main;
		public string[] otherroles_aid = new string[0];
		public string wait_otherroles_main;
		public string[] wait_otherroles_aid = new string[0];
		public string[] start_otherroles_main = new string[0];
		public string[] end_otherroles_main = new string[0];
	}
}