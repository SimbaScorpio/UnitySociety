using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{
	public const string characterFileName = "storyline.json";
	private string url;

	void Start ()
	{
		LoadGameData ();
	}

	void LoadGameData ()
	{
		url = "file://" + Application.dataPath + "/Data/" + characterFileName;
		StartCoroutine (LoadData ());
	}

	IEnumerator LoadData ()
	{
		WWW www = new WWW (url);
		yield return www;
		if (!string.IsNullOrEmpty (www.error))
			Debug.Log (www.error);
		else {
			string json = www.text;
			print (json);
			StoryLine storyline = JsonUtility.FromJson<StoryLine> (json);
			print (storyline.storyline_spots [0].principal_activities [0].other_people [0].following_activities [0].self.bubble_content);
		}
	}
}
