using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class TrafficManager : MonoBehaviour
	{
		public int max = 10;
		public float gap = 5;
		public List<GameObject> carPref;
		public GameObject spawnParent;
		public GameObject cornerParent;

		private List<Vector3> spawns = new List<Vector3> ();
		private List<Vector3> corners = new List<Vector3> ();

		private List<GameObject> cars = new List<GameObject> ();

		private bool isTicking;
		private float count = 0;

		private static TrafficManager instance;

		public static TrafficManager GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		void Start ()
		{
			GetChildPoints (spawns, spawnParent);
			GetChildPoints (corners, cornerParent);
			isTicking = true;
		}

		void GetChildPoints (List<Vector3> list, GameObject parent)
		{
			Transform[] trs = parent.GetComponentsInChildren<Transform> ();
			for (int i = 1; i < trs.Length; ++i) {
				list.Add (trs [i].position);
			}
		}

		public void RemoveCar (GameObject obj, Vector3 end)
		{
			cars.Remove (obj);
			Destroy (obj);
		}

		public void AddCar (Vector3 spawn)
		{
			int index = Random.Range (0, carPref.Count);
			GameObject car = Instantiate (carPref [index], spawn, Quaternion.identity) as GameObject;
			car.GetComponent<CarMovement> ().StartSearching (corners, spawn);
			cars.Add (car);
		}

		void Update ()
		{
			if (isTicking) {
				count += Time.deltaTime;
				if (count > gap && cars.Count < max) {
					AddCar (spawns [Random.Range (0, spawns.Count)]);
					count = 0;
				}
			}
		}
	}
}