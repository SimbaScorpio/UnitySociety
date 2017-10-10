using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkServerAISpawner : NetworkBehaviour
	{
		public GameObject AIPrefab;

		public static NetworkServerAISpawner instance;

		public static NetworkServerAISpawner GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		public GameObject Spawn (CharacterData cha, Vector3 position, Quaternion rotation)
		{
			GameObject ai = Instantiate (AIPrefab, position, rotation) as GameObject;
			ai.GetComponent<NetworkServerAI> ().character = cha;
			NetworkServer.Spawn (ai);
			return ai;
		}
	}
}