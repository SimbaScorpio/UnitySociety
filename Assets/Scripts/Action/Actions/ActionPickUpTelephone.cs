using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPickUpTelephone : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;
	private bool inMyState = false;

	public void Setting (GameObject obj, IActionCompleted monitor)
	{
		this.id = ActionID.PICKUPTELEPHONE;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("拿起电话");
	}

	public void OnPhoneAppear ()
	{
		obj.transform.Find ("hip_ctrl/root/spline/right_chest/left_arm/left_elbow/left_hand/mic_2").gameObject.SetActive (true);
	}

	void Update ()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("拿起电话")) {
			inMyState = true;
		} else if (inMyState) {
			Finish ();
		}
	}

	public void Finish ()
	{
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
