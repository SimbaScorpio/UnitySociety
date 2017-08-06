using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionUseCamera : ActionSingle
{
	public GameObject obj;
	private Animator animator;
	private IActionCompleted monitor;
	private bool inMyState = false;
	private GameObject cameraObj;

	public void Setting (GameObject obj, GameObject cameraObj, IActionCompleted monitor)
	{
		this.id = ActionID.USECAMERA;
		this.obj = obj;
		this.monitor = monitor;
		this.animator = obj.GetComponent<Animator> ();
		animator.SetTrigger ("使用相机");
		this.cameraObj = cameraObj;
		this.cameraObj.SetActive (true);
	}

	void Update ()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("使用相机")) {
			inMyState = true;
		} else if (inMyState) {
			Finish ();
		}
	}

	public void Finish ()
	{
		cameraObj.SetActive (false);
		if (monitor != null) {
			monitor.OnActionCompleted (this);
		}
		Free ();
	}
}
