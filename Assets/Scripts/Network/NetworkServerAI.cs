using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Pathfinding;

namespace DesignSociety
{
	public class NetworkServerAI : NetworkBehaviour
	{
		[SyncVar]
		public CharacterData character;

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
				NavmeshCut navmeshcut = GetComponent<NavmeshCut> ();
				if (navmeshcut != null)
					navmeshcut.enabled = false;
				MyRichAI myRichAI = GetComponent<MyRichAI> ();
				if (myRichAI != null)
					myRichAI.enabled = false;
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