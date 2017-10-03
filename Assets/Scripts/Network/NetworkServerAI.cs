using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkServerAI : NetworkBehaviour
	{
		[SyncVar]
		public Character character;

		void Start ()
		{
			CheckServer ();
			InitialCharater ();
		}

		void CheckServer ()
		{
			if (!isServer) {
				Person person = GetComponent<Person> ();
				if (person != null)
					person.enabled = false;
				ActionDealer actionDealer = GetComponent<ActionDealer> ();
				if (actionDealer != null)
					actionDealer.enabled = false;
			}
		}

		void InitialCharater ()
		{
			Material clothMat = MaterialCollection.GetInstance ().Get (character.clothing);
			transform.Find ("mesh").GetComponent<Renderer> ().material = clothMat;
			this.name = character.name;
		}
	}
}