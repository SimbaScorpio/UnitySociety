using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OtherPerson
{
	public string name;
	public int location_to_type;
	public string location_to;
	public int bubble_type;
	public float bubble_duration;
	public string bubble_content;
	public string screen;
	public int num_following_activities;
	public FollowingActivity[] following_activities;
}
