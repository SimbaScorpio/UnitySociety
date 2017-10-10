using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class SceneData
	{
		public float start_time;
		public float end_time;
		public string spot_name;
		public string principal;
		public PrincipalActivity[] principal_activity;
	}
}