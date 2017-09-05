using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPickUpTelephone : ActionTrigger
{
	public void OnPhoneAppear ()
	{
		obj.transform.Find ("hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/mic_2").gameObject.SetActive (true);
	}
}
