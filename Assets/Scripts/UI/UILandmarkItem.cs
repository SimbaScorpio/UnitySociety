using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILandmarkItem : MonoBehaviour
{
	public Button button;
	public InputField inputField;
	public Dropdown dropdown;
	[HideInInspector]
	public ColorBlock btnColors;
	[HideInInspector]
	public Landmark landmark;

	void Start ()
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
		UpdateName (inputField.text);
		button.gameObject.SetActive (true);
		inputField.gameObject.SetActive (false);
	}

	public void OnButtonClicked ()
	{
		UILandmarkManager.GetInstance ().SelectItem (this);
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
		// shift camera to mark
	}

	public void OnDropDownValueChanged ()
	{
		landmark.label = dropdown.captionText.text;
	}


	public void UpdateName (string name)
	{
		// check validation
		if (name.Length == 0 || name == landmark.name)
			return;
		// check existance
		landmark.name = UILandmarkManager.GetInstance ().TryToGetValidName (name);
		SetName (landmark.name);
	}


	public void SetName (string name)
	{
		button.GetComponentInChildren<Text> ().text = name;
	}

	public void SetTag (string tag)
	{
		for (int i = 0; i < dropdown.options.Count; ++i) {
			string name = dropdown.options [i].text;
			if (string.Equals (name, tag)) {
				dropdown.value = i;
				return;
			}
		}
	}
}