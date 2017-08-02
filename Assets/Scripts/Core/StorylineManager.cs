using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorylineManager : MonoBehaviour
{
	public Storyline storyline;
	public Dictionary<string, GameObject> nameToCharacterObj;
	public Dictionary<string, Character> nameToCharacter;
	public Dictionary<string, CompositeMovement> nameToCompositeMovement;
	public Dictionary<string, Job> nameToJob;
	public Dictionary<string, string> nameToJobCandidate;

	public GameObject PlayerPrefab;

	private float time;
	private bool isTicking;

	private bool[] spotHasStarted;

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
		RandomlyArrangeJobCandidates ();
		InitializeSelfParameters ();
		Tick ();
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
			player.name = cha.name;

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

	public void RandomlyArrangeJobCandidates ()
	{
		List<string> names = new List<string> ();
		foreach (string name in nameToJob.Keys) {
			names.Add (name);
		}
		nameToJobCandidate.Clear ();
		if (!RecursivelyArrangeJobCandidate (names, 0)) {
			Log.error ("Select job candidates failed: impossible combination");
		}
	}

	bool RecursivelyArrangeJobCandidate (List<string> jobNames, int i)
	{
		if (i >= jobNames.Count)
			return true;
		
		Job job = nameToJob [jobNames [i]];
		if (job.candidates.Length == 0) {
			Log.warn ("Select job candidate [" + jobNames [i] + "] warning: no candidates");
			return RecursivelyArrangeJobCandidate (jobNames, i + 1);
		}

		List<string> possibleChaNames = new List<string> ();
		foreach (string chaName in job.candidates) {
			if (!nameToCharacter.ContainsKey (chaName)) {
				Log.warn ("Character [" + chaName + "] in job [" + jobNames [i] + "] is illegal: cannot find");
				continue;
			}
			if (!nameToJobCandidate.ContainsValue (chaName))
				possibleChaNames.Add (chaName);
		}
		if (possibleChaNames.Count == 0) {
			nameToJobCandidate.Remove (jobNames [i]);
			return false;
		}

		int index = Random.Range (0, possibleChaNames.Count);
		nameToJobCandidate [jobNames [i]] = possibleChaNames [index];
		while (!RecursivelyArrangeJobCandidate (jobNames, i + 1)) {
			possibleChaNames.RemoveAt (index);
			if (possibleChaNames.Count == 0) {
				nameToJobCandidate.Remove (jobNames [i]);
				return false;
			}
			index = Random.Range (0, possibleChaNames.Count);
			nameToJobCandidate [jobNames [i]] = possibleChaNames [index];
		}
		return true;
	}

	void InitializeSelfParameters ()
	{
		time = 0.0f;
		isTicking = false;
		spotHasStarted = new bool[storyline.storyline_spots.Length];
		for (int i = 0; i < storyline.storyline_spots.Length; ++i)
			spotHasStarted [i] = false;
	}

	bool StartStorylineSpot (StorylineSpot spot)
	{
		string name = nameToJobCandidate [spot.principal];
		GameObject obj = nameToCharacterObj [name];
		Person person = obj.GetComponent<Person> ();
		return person.AddPrincipalActivities (spot.principal_activities);
	}

	void EndStorylineSpot (StorylineSpot spot)
	{
		string name = nameToJobCandidate [spot.principal];
		GameObject obj = nameToCharacterObj [name];
		Person person = obj.GetComponent<Person> ();
		person.Stop ();
	}


	public void Tick ()
	{
		time = 0.0f;
		isTicking = true;
	}

	void Update ()
	{
		if (!isTicking)
			return;
		time += Time.deltaTime;
		for (int i = 0; i < storyline.storyline_spots.Length; ++i) {
			StorylineSpot spot = storyline.storyline_spots [i];
			if (string.IsNullOrEmpty (spot.principal)) {
				spotHasStarted [i] = true;
				Log.warn ("Spot [" + spot.spot_name + "] misses principal: skipped");
			} else if (!nameToJob.ContainsKey (spot.principal)) {
				spotHasStarted [i] = true;
				Log.warn ("Spot [" + spot.spot_name + "] has undefined principal [" + spot.principal + "] : skipped");
			} else if (spot.principal_activities.Length == 0) {
				spotHasStarted [i] = true;
				Log.warn ("Spot [" + spot.spot_name + "] misses principal activities: skipped");
			} else {
				if (time >= spot.start_time && time < spot.end_time) {
					if (!spotHasStarted [i]) {
						if (StartStorylineSpot (spot)) {
							spotHasStarted [i] = true;
						} else {
							Log.warn ("Try to start spot [" + spot.spot_name + "] failed: principal busy");
						}
					}
				} else if (time >= spot.end_time) {
					spotHasStarted [i] = true;
					EndStorylineSpot (spot);
				}
			}
		}
	}
}
