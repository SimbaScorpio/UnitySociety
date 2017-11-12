using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

namespace DesignSociety
{
	public class NetworkKeywordBubble : MonoBehaviour
	{
		private CameraPerspectiveEditor cameraEditor;

		void Start ()
		{
			cameraEditor = Camera.main.GetComponent<CameraPerspectiveEditor> ();
		}

		void Update ()
		{
			if (Input.GetMouseButtonDown (0)) {
				Ray ray = cameraEditor.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, float.MaxValue)) {
					CheckKeywordPressed (hit.collider.gameObject);
				}
			}
		}

		public bool CheckKeywordPressed (GameObject collider)
		{
			if (collider == gameObject) {
				TextMeshPro tp = gameObject.GetComponentInChildren<TextMeshPro> ();
				if (tp != null) {
					Keyword k = KeywordCollection.GetInstance ().Get (tp.text);
					UIKeywordPanel.GetInstance ().OpenPanel (k);
					return true;
				}
			}
			return false;
		}
	}
}