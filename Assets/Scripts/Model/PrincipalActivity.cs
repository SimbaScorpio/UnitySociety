using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrincipalActivity
{
	public int pa_id;
	public float duration;
	public string description;
	public string composite_movement_name;
	public int num_other_people;
	public Self self;
	public SecondPerson[] other_people;
}
