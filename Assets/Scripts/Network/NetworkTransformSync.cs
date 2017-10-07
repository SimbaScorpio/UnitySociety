using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkTransformSync : NetworkBehaviour
	{
		public bool sendPosition;
		public bool sendRotation;
		public float lerpPosRate = 10f;
		public float lerpRotRate = 10f;

		[SyncVar]
		private Vector3 syncPosition;
		[SyncVar]
		private Quaternion syncRotation;

		void Start ()
		{
			syncPosition = transform.position;
			syncRotation = transform.rotation;
		}

		void FixedUpdate ()
		{
			if (isLocalPlayer) {
				if (sendPosition)
					CmdSendPosition (transform.position);
				if (sendRotation)
					CmdSendRotation (transform.rotation);
			} else {
				if (sendPosition)
					transform.position = Vector3.Lerp (transform.position, syncPosition, lerpPosRate * Time.deltaTime);
				if (sendRotation)
					transform.rotation = Quaternion.Lerp (transform.rotation, syncRotation, lerpRotRate * Time.deltaTime);
			}
		}


		[Command]
		void CmdSendPosition (Vector3 position)
		{
			syncPosition = position;
		}

		[Command]
		void CmdSendRotation (Quaternion rotation)
		{
			syncRotation = rotation;
		}
	}
}