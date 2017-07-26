using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryLineSpot
{
	public int id;
	public float start_time;
	public float end_time;
	public string spot_name;
	public string principal;
	public PrincipalActivity[] principal_activities;
}
