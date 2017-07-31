﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FollowingActivity
{
	public int fa_id;
	public float duration;
	public string description;
	public string composition_movement_name;
	public int contact_type;
	public int num_other_people;
	public Self self;
	public ThirdPerson[] other_people;
}
