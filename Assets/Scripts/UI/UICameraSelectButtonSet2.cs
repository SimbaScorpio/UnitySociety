using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class UICameraSelectButtonSet2 : MonoBehaviour
	{
		public CameraGentlemanMovement gentleman;
		public CameraPerspectiveMovement perspective;
		public CameraOrthographicMovement orthographic;

		private int index;
		private bool displayMode;

		public void OnGentlemanClicked ()
		{
			if (gentleman == null)
				return;
			TurnOnCamera (0);
		}

		public void OnPerspectiveClicked ()
		{
			if (perspective == null)
				return;
			TurnOnCamera (1);
		}

		public void OnOrthographicClicked ()
		{
			if (orthographic == null)
				return;
			TurnOnCamera (2);
		}

		void TurnOffAllCamera ()
		{
			if (gentleman != null) {
				gentleman.gameObject.SetActive (false);
				gentleman.tag = "Untagged";
			}
			if (perspective != null) {
				perspective.gameObject.SetActive (false);
				perspective.tag = "Untagged";
			}
			if (orthographic != null) {
				orthographic.gameObject.SetActive (false);
				orthographic.tag = "Untagged";
			}
		}

		void TurnOnCamera (int index)
		{
			if (index < 0 || index > 2)
				return;
			TurnOffAllCamera ();
			this.index = index;
			if (index == 0 && gentleman != null) {
				gentleman.gameObject.SetActive (true);
				gentleman.tag = "MainCamera";
			}
			if (index == 1 && perspective != null) {
				perspective.gameObject.SetActive (true);
				perspective.tag = "MainCamera";
			}
			if (index == 2 && orthographic != null) {
				orthographic.gameObject.SetActive (true);
				orthographic.tag = "MainCamera";
			}
		}

		public void OnDisplayModeClicked ()
		{
			if (!displayMode) {
				TurnOnCamera (0);
				gentleman.DisplayAspect (true);
				displayMode = true;
			} else {
				gentleman.DisplayAspect (false);
				displayMode = false;
			}
		}

		public void OnResetClicked ()
		{
			if (index == 0 && gentleman != null) {
				gentleman.ResetTransform ();
			}
			if (index == 1 && perspective != null) {
				perspective.ResetTransform ();
			}
			if (index == 2 && orthographic != null) {
				orthographic.ResetTransform ();
			}
		}
	}
}
