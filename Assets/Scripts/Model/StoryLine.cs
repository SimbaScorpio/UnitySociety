﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Storyline
{
	public Character[] characters;
	public Job[] jobs;
	public CompositeMovement[] composite_movements;
	public StorylineSpot[] storyline_spots;
}