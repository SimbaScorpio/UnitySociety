using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public static class Global
	{
		public static string StorylineJsonURL = "file://" + Application.dataPath + "/Data/json/storyline.json";
		public static string LandmarkJsonRURL = "file://" + Application.dataPath + "/Data/json/landmark.json";
		public static string LandmarkJsonWURL = Application.dataPath + "/Data/json/landmark.json";
		public static string TexturePath = "file://" + Application.dataPath + "/Data/texture/";
	}

	public enum SceneState
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

	public enum StuffType
	{
		SmallStuff,
		MiddleStuff,
		BigStuff,
		BookStuff
	}
}