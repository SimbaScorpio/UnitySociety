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

		void Start ()
		{
			CheckCanvasVisibility ();
		}

		void CheckCanvasVisibility ()
		{
			if (isServer) {
				Instantiate (server);
			} else {
				Instantiate (client);
			}
		}
	}
}
