using System;
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
		public InputField actionField;
		public RawImage avatar;

		private AvatarInfo av;
		private string hookName;
		private float rayDistance = 1000f;

		private int maxRecord = 5;

		void Update ()
		{
			if (Input.GetMouseButtonDown (0)) {
				if (EventSystem.current && EventSystem.current.IsPointerOverGameObject ())
					return;
				// ray could shoot from different types of camera
				CameraPerspectiveEditor cameraEditor = Camera.main.GetComponent<CameraPerspectiveEditor> ();
				Ray ray = (cameraEditor != null && cameraEditor.isActiveAndEnabled) ?
						cameraEditor.ScreenPointToRay (Input.mousePosition) :
						Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, rayDistance)) {
					if (hit.collider.tag == "Player" && hit.collider.GetComponent<Person> () != null) {
						if (!string.IsNullOrEmpty (hookName)) {
							AvatarManager.GetInstance ().RemoveAvatarSnap (hookName);
						}
						avatar.texture = AvatarManager.GetInstance ().GetAvatarSnap (hit.collider.name);
						hookName = hit.collider.name;
					}
				}
			}
			if (!string.IsNullOrEmpty (hookName)) {
				av = AvatarManager.GetInstance ().GetAvatarInfo (hookName);
				if (av != null) {
					nameField.text = av.name;
					careerField.text = av.job;
					sceneField.text = av.scene;
					actionField.text = GenerateActionRecord (av.recentActions);
				}
			}
		}

		string GenerateActionRecord (List<string> record)
		{
			string recordStr = "";
			if (record != null) {
				int min = record.Count < maxRecord ? record.Count : maxRecord;
				for (int i = 0; i < min; ++i) {
					recordStr += record [i] + Environment.NewLine;
				}
			}
			return recordStr;
		}
	}
}