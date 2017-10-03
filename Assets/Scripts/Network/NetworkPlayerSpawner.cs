using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkPlayerSpawner : NetworkBehaviour
	{
		public GameObject AIPrefab;

		public static NetworkPlayerSpawner instance;

		public static NetworkPlayerSpawner GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		public GameObject Spawn (Character cha, Vector3 position, Quaternion rotation)
		{
			GameObject ai = Instantiate (AIPrefab, position, rotation) as GameObject;
			ai.GetComponent<NetworkServerAI> ().character = cha;
			NetworkServer.Spawn (ai);
			return ai;
		}

		public void DestroyAllAIs ()
		{
			
		}
	}
}