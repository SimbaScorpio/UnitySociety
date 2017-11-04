using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DesignSociety
{
	public class UIServerAIInfomation : MonoBehaviour
	{
		public InputField nameField;
		public InputField careerField;
		public InputField sceneField;

		private float rayDistance = 1000f;

		void Update ()
		{
			if (Input.GetMouseButtonDown (0)) {
				if (EventSystem.current.IsPointerOverGameObject ())
					return;
				// ray could shoot from different types of camera
				CameraPerspectiveEditor cameraEditor = Camera.main.GetComponent<CameraPerspectiveEditor> ();
				Ray ray = (cameraEditor != null && cameraEditor.isActiveAndEnabled) ?
						cameraEditor.ScreenPointToRay (Input.mousePosition) :
						Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, rayDistance)) {
					if (hit.collider.tag == "Player" && hit.collider.GetComponent<Person> () != null) {
						AvatarInfo av = ServerAIInfoManager.GetInstance ().GetAvatarInfo (hit.collider.name);
						nameField.text = av.name;
						careerField.text = av.job;
						sceneField.text = av.scene;
					}
				}
			}
		}
	}
}