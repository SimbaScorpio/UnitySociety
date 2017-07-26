using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryLine
{
	public Character[] characters;
	public Job[] jobs;
	public CompositeMovement[] composite_movements;
	public StoryLineSpot[] storyline_spots;
}
