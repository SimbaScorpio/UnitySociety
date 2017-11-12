using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;

namespace DesignSociety
{
	public class UIFileDialog : MonoBehaviour
	{
		public InputField storylineInputField;
		public Button storylineButton;

		public InputField landmarkInputField;
		public Button landmarkButton;

		public string storylinePath;
		public string landmarkPath;

		void Start ()
		{
			storylineButton.onClick.AddListener (delegate {
				OnStorylinePathButtonClicked ();
			});
			storylineInputField.onEndEdit.AddListener (delegate {
				OnStorylinePathInputFieldEditEnded ();
			});
			landmarkButton.onClick.AddListener (delegate {
				OnLandmarkPathButtonClicked ();
			});
			landmarkInputField.onEndEdit.AddListener (delegate {
				OnLandmarkPathInputFieldEditEnded ();
			});
		}

		public void OnStorylinePathButtonClicked ()
		{
			ExtensionFilter[] filters = new ExtensionFilter[] {
				new ExtensionFilter ("Json", "json")
			};
			string[] paths = StandaloneFileBrowser.OpenFilePanel ("Open File", "", filters, false);
			//string[] paths = StandaloneFileBrowser.OpenFolderPanel ("Select Folder", "", false);
			storylinePath = paths.Length > 0 ? paths [0] : storylinePath;
			storylineInputField.text = storylinePath;
		}

		public void OnStorylinePathInputFieldEditEnded ()
		{
			storylinePath = storylineInputField.text;
		}

		public void OnLandmarkPathButtonClicked ()
		{
			ExtensionFilter[] filters = new ExtensionFilter[] {
				new ExtensionFilter ("Json", "json")
			};
			string[] paths = StandaloneFileBrowser.OpenFilePanel ("Open File", "", filters, false);
			//string[] paths = StandaloneFileBrowser.OpenFolderPanel ("Select Folder", "", false);
			landmarkPath = paths.Length > 0 ? paths [0] : landmarkPath;
			landmarkInputField.text = landmarkPath;
		}

		public void OnLandmarkPathInputFieldEditEnded ()
		{
			landmarkPath = landmarkInputField.text;
		}

	}
}