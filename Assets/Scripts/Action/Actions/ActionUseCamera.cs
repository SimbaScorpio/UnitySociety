using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionUseCamera : ActionTrigger
{
	private GameObject cameraObj;

	public override void Setting (GameObject obj, string stateName, IActionCompleted monitor)
	{
		base.Setting (obj, stateName, monitor);
		cameraObj = obj.transform.Find ("hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/camera2").gameObject;
		cameraObj.SetActive (true);
	}

	public override void Finish ()
	{
		cameraObj.SetActive (false);
		base.Finish ();
	}
}
