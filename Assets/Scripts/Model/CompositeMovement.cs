using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class CompositeMovement
	{
		public string name;
		public string mainrole_main;
		public string[] mainrole_aid;
		public string wait_mainrole_main;
		public string[] wait_mainrole_aid;
		public string[] start_mainrole_main;
		public string[] end_mainrole_main;

		public string otherroles_main;
		public string[] otherroles_aid;
		public string wait_otherroles_main;
		public string[] wait_otherroles_aid;
		public string[] start_otherroles_main;
		public string[] end_otherroles_main;
	}
}