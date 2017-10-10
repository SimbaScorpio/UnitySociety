using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkSpawner : NetworkBehaviour
	{
		public GameObject server;
		public GameObject client;
		public List<GameObject> lobby = new List<GameObject> ();

		void Start ()
		{
			CheckCanvasVisibility ();
			DisableLobbyItems ();
		}

		void CheckCanvasVisibility ()
		{
			if (isServer) {
				Instantiate (server);
			} else {
				Instantiate (client);
			}
		}

		void DisableLobbyItems ()
		{
			for (int i = 0; i < lobby.Count; ++i) {
				lobby [i].SetActive (false);
			}
		}
	}
}
