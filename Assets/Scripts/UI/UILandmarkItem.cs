using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class UILandmarkItem : MonoBehaviour
	{
		public Button button;
		public InputField inputField;
		public Dropdown dropdown;
		[HideInInspector]
		public ColorBlock btnColors;
		[HideInInspector]
		public Landmark landmark;

		void Awake ()
		{
			button.onClick.AddListener (delegate {
				OnButtonClicked ();
			});
			inputField.onEndEdit.AddListener (delegate {
				OnInputFieldEndEdit ();
			});
			dropdown.onValueChanged.AddListener (delegate {
				OnDropDownValueChanged ();
			});
			btnColors = button.colors;
		}

		public void OnInputFieldEndEdit ()
		{
			UpdateName (inputField.text, true);
			CloseInputField ();
		}

		public void OnButtonClicked ()
		{
			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
				UILandmarksManager.GetInstance ().SelectItem (landmark, true, false, false);
			else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.LeftControl))
				UILandmarksManager.GetInstance ().SelectItem (landmark, false, true, false);
			else
				UILandmarksManager.GetInstance ().SelectItem (landmark, false, false, true);
		}

		public void OnButtonLagClicked ()
		{
			button.gameObject.SetActive (false);
			inputField.gameObject.SetActive (true);
			inputField.ActivateInputField ();
			inputField.text = button.GetComponentInChildren<Text> ().text;
		}

		public void OnButtonDoubleClicked ()
		{
			UILandmarksManager.GetInstance ().ShiftCameraToMark ();
		}

		public void OnDropDownValueChanged ()
		{
			landmark.m_label = dropdown.captionText.text;
		}


		public void UpdateName (string name, bool skip)
		{
			if (name.Length == 0)
				return;
			landmark.m_name = UILandmarksManager.GetInstance ().TryToGetValidName (name, skip);
			SetName (name);
		}

		public void SetName (string name)
		{
			button.GetComponentInChildren<Text> ().text = landmark.m_name;
		}

		public void UpdateTag (string tag)
		{
			for (int i = 0; i < dropdown.options.Count; ++i) {
				if (dropdown.options [i].text == tag) {
					dropdown.value = i;
					landmark.m_label = tag;
					return;
				}
			}
		}

		public void CloseInputField ()
		{
			button.gameObject.SetActive (true);
			inputField.gameObject.SetActive (false);
		}

		public void ColorPressed ()
		{
			button.colors = ColorBlock (btnColors.pressedColor);
		}

		public void ColorNormal ()
		{
			button.colors = btnColors;
		}

		public static ColorBlock ColorBlock (Color color)
		{
			ColorBlock block = new ColorBlock ();
			block.highlightedColor = color;
			block.normalColor = color;
			block.pressedColor = color;
			block.colorMultiplier = 1;
			return block;
		}
	}
}