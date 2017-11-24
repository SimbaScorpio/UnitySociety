using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using TMPro;

namespace DesignSociety
{
	// 控制行走、动作和关键词气泡点击
	public class NetworkPlayerAction : NetworkBehaviour
	{
		private NetworkActionDealer ad;
		private CameraPerspectiveEditor cameraEditor;

		private Ray ray;
		private RaycastHit hit;

		void Start ()
		{
			CheckLocal ();
			ad = GetComponent<NetworkActionDealer> ();
			cameraEditor = Camera.main.GetComponent<CameraPerspectiveEditor> ();
		}

		void CheckLocal ()
		{
			if (!isLocalPlayer)
				enabled = false;
		}

		public void WalkTo (Vector3 position)
		{
			Landmark lm = new Landmark ();
			lm.Set (position);
			ad.ApplyWalkAction (lm, false, null);
		}

		public void ApplyAction (string name)
		{
			ad.ApplyAction (name, null);
		}

		void Update ()
		{
			CheckLocal ();
			PresseEvent ();
		}

		void PresseEvent ()
		{
			if (EventSystem.current == null || EventSystem.current.IsPointerOverGameObject ())
				return;
			if (Input.GetMouseButtonDown (0)) {
				ray = cameraEditor.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out hit, float.MaxValue)) {
					CheckKeywordPressed (hit.collider.gameObject);
					CheckServerAIPressed (hit.collider.gameObject);
				}
			}
		}

		public void CheckKeywordPressed (GameObject collider)
		{
			if (collider.tag == "KeywordBubble") {
				// show keyword in menu
				TextMeshPro text = collider.GetComponentInChildren<TextMeshPro> ();
				UIClientMessageKeywordBlob.GetInstance ().PushBlob (text.text);
			}
		}

		public void CheckServerAIPressed (GameObject collider)
		{
			if (collider.tag == "Player") {
				Person person = collider.GetComponent<Person> ();
				if (person != null) {
					UIClientMessageAvatarBlob.GetInstance ().PushBlob (collider);
				}
			}
		}
	}
}