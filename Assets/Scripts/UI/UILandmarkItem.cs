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
			button.gameObject.SetActive (true);
			inputField.gameObject.SetActive (false);
		}

		public void OnButtonClicked ()
		{
			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
				UILandmarkManager.GetInstance ().SelectItem (this, true, false);
			else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.LeftControl))
				UILandmarkManager.GetInstance ().SelectItem (this, false, true);
			else
				UILandmarkManager.GetInstance ().SelectItem (this, false, false);
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
			UILandmarkManager.GetInstance ().ShiftCameraToMark ();
		}

		public void OnDropDownValueChanged ()
		{
			landmark.label = dropdown.captionText.text;
		}


		public void UpdateName (string name, bool skip)
		{
			if (name.Length == 0)
				return;
			landmark.name = UILandmarkManager.GetInstance ().TryToGetValidName (name, skip);
			button.GetComponentInChildren<Text> ().text = landmark.name;
		}

		public void UpdateTag (string tag)
		{
			for (int i = 0; i < dropdown.options.Count; ++i) {
				if (dropdown.options [i].text == tag) {
					dropdown.value = i;
					landmark.label = tag;
					return;
				}
			}
		}

		public void ColorPressed ()
		{
			button.colors = ColorBlock (btnColors.pressedColor);
		}

		public void ColorNormal ()
		{
			button.colors = btnColors;
		}

		public ColorBlock ColorBlock (Color color)
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