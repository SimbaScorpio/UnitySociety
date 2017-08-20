using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICameraSelectButtonSet : MonoBehaviour
{
	public BasicCameraController controller;
	public BasicCameraController.CameraProjection[] projection = 
		new BasicCameraController.CameraProjection[3];

	public void OnButtonClicked (int id)
	{
		controller.projection = projection [id];
		controller.SetProjectionScript ();
	}

	public void OnResetClicked ()
	{
		controller.ResetTransform ();
	}
}
