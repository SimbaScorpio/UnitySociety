using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
	public static string JsonURL = "file://" + Application.dataPath + "/Data/json/storyline.json";
	public static string TexturePath = "file://" + Application.dataPath + "/Data/texture/";
}

public enum SpotState
{
	READY,
	STARTED,
	ENDED,
	KILLED
}

public enum ComState
{
	ARRIVING,
	ARRIVINGSTOP,
	PREPARING,
	PREPARINGSTOP,
	STARTING,
	STARTINGSTOP,
	ENDING,
	ENDINGSTOP,
	LEAVING
}