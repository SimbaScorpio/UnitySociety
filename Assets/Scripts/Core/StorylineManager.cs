using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorylineManager : MonoBehaviour
{
	public StoryLine storyline;
	public Dictionary<string, GameObject> nameToCharacterObj;
	public Dictionary<string, Character> nameToCharacter;
	public Dictionary<string, CompositeMovement> nameToCompositeMovement;
	public Dictionary<string, Job> nameToJob;
	public Dictionary<string, string> nameToJobCandidate;

	public GameObject PlayerPrefab;

	private static StorylineManager instance;

	public static StorylineManager GetInstance ()
	{
		return instance;
	}

	void Awake ()
	{
		instance = this;
		nameToCharacterObj = new Dictionary<string, GameObject> ();
		nameToCharacter = new Dictionary<string, Character> ();
		nameToCompositeMovement = new Dictionary<string, CompositeMovement> ();
		nameToJob = new Dictionary<string, Job> ();
		nameToJobCandidate = new Dictionary<string, string> ();
	}

	public void Initialize ()
	{
		InitializeCharacters ();
		InitializeCompositeMovements ();
		InitializeJobs ();
	}

	void InitializeCharacters ()
	{
		int chaNum = storyline.characters.Length;
		for (int i = 0; i < chaNum; ++i) {
			Character cha = storyline.characters [i];
			if (string.IsNullOrEmpty (cha.name)) {
				Log.error ("Initialize character [" + i + "] failed: empty name");
				continue;
			}
			Transform initialLocation = LocationCollection.Get (cha.initial_position);
			GameObject player = Instantiate (PlayerPrefab, initialLocation.position, initialLocation.rotation) as GameObject;
			Material clothing = MaterialCollection.Get (cha.clothing);
			player.transform.Find ("mesh").GetComponent<Renderer> ().material = clothing;

			if (nameToCharacter.ContainsKey (cha.name)) {
				Log.warn ("Initialize character [" + i + "] warning: overlapped name");
			}
			nameToCharacter [cha.name] = cha;
			nameToCharacterObj [cha.name] = player;
		}
	}

	void InitializeCompositeMovements ()
	{
		int comNum = storyline.composite_movements.Length;
		for (int i = 0; i < comNum; ++i) {
			CompositeMovement com = storyline.composite_movements [i];
			if (string.IsNullOrEmpty (com.name)) {
				Log.error ("Initialize composite movement [" + i + "] failed: empty name");
				continue;
			}
			if (nameToCompositeMovement.ContainsKey (com.name)) {
				Log.warn ("Initialize composite movement [" + i + "] warning: overlapped name");
			}
			nameToCompositeMovement [com.name] = com;
		}
	}

	void InitializeJobs ()
	{
		int jobNum = storyline.jobs.Length;
		for (int i = 0; i < jobNum; ++i) {
			Job job = storyline.jobs [i];
			if (string.IsNullOrEmpty (job.name)) {
				Log.error ("Initialize job [" + i + "] failed: empty name");
				continue;
			}
			if (nameToJob.ContainsKey (job.name)) {
				Log.warn ("Initialize job [" + i + "] warning: overlapped name");
			}
			nameToJob [job.name] = job;
		}
	}

	public void RandomlyArrangeJobCandidate ()
	{
		
	}
}
