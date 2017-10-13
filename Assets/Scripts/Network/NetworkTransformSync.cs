using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DesignSociety
{
	[NetworkSettings (channel = 1, sendInterval = 0.03f)]
	public class NetworkTransformSync : NetworkBehaviour
	{
		public bool isServerControl;
		public bool sendPosition;
		public bool sendRotation;
		public float lerpPosRate = 20f;
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
			if (isServerControl && isServer) {
				if (sendPosition)
					syncPosition = transform.position;
				if (sendRotation)
					syncRotation = transform.rotation;
			} else if (isLocalPlayer) {
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