using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkPlayerIdleCheck : NetworkBehaviour
	{
		public float maxTimeToIdle = 10f;
		public GameObject idleMark;
		public float idleCount = 0f;
		public bool isIdleForALongTime = false;

		private NetworkActionDealer ad;

		private GameObject bodymesh;

		void Start ()
		{
			CheckLocal ();
			ad = GetComponent<NetworkActionDealer> ();
			bodymesh = transform.Find ("mesh").gameObject;
			idleCount = maxTimeToIdle;
		}

		void CheckLocal ()
		{
			if (!isLocalPlayer)
				enabled = false;
		}

		void Update ()
		{
			CheckLocal ();
			if (UIKeywordPanel.GetInstance ().isActive)
				return;
			if (isIdleForALongTime) {
				if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject ()) {
					idleCount = 0;
					NotIdle ();
				}
			} else {
				if (!ad.IsPlaying () && !Input.anyKeyDown && bodymesh.activeInHierarchy) {
					idleCount += Time.deltaTime;
					if (idleCount > maxTimeToIdle)
						Idle ();
				} else {
					idleCount = 0;
				}
			}
		}

		void Idle ()
		{
			isIdleForALongTime = true;
			if (idleMark != null) {
				idleMark.SetActive (true);
			}
			UIInformationMenu.GetInstance ().ShowIdlePanel ();
		}

		void NotIdle ()
		{
			isIdleForALongTime = false;
			if (idleMark != null) {
				idleMark.SetActive (false);
			}
			UIInformationMenu.GetInstance ().ShowActivePanel ();
		}
	}
}