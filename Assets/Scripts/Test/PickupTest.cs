using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignSociety;

public class PickupTest : MonoBehaviour
{
	//	public GameObject stuff;
	//	public GameObject parent;
	//	public Transform tr;

	//	public void OnStuffPicked ()
	//	{
	//		stuff.transform.SetParent (parent.transform);
	//		stuff.transform.position = tr.position;
	//		stuff.transform.rotation = tr.rotation;
	//		stuff.GetComponent<Rigidbody> ().isKinematic = true;
	//	}
	//
	//	public void OnStuffPut ()
	//	{
	//		stuff.transform.SetParent (null);
	//		stuff.GetComponent<Rigidbody> ().isKinematic = false;
	//	}
	//
	//	public void OnStuffDisappear ()
	//	{
	//		stuff.SetActive (false);
	//	}
	//
	//	public void OnStuffAppear ()
	//	{
	//		stuff.SetActive (true);
	//	}

	void Update ()
	{
		if (gameObject.GetComponent<ActionSingle> ()) {
			return;
		}
		// 从桌面拿放
		if (Input.GetKeyDown (KeyCode.Z)) {
			ActionPickUpStuff ac = gameObject.AddComponent<ActionPickUpStuff> ();
			ac.Setting (gameObject, "单手拿起一个小东西", null);
			ac.SetStuffType (StuffType.SmallStuff);
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "单手放下一个小东西", null);
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			ActionPickUpStuff ac = gameObject.AddComponent<ActionPickUpStuff> ();
			ac.Setting (gameObject, "双手拿起一个中型东西", null);
			ac.SetStuffType (StuffType.MiddleStuff);
		}
		if (Input.GetKeyDown (KeyCode.V)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "双手放下一个中型东西", null);
		}
		if (Input.GetKeyDown (KeyCode.B)) {
			ActionPickUpStuff ac = gameObject.AddComponent<ActionPickUpStuff> ();
			ac.Setting (gameObject, "双手抬起一个大东西", null);
			ac.SetStuffType (StuffType.BigStuff);
		}
		if (Input.GetKeyDown (KeyCode.N)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "双手放下一个大东西", null);
		}

		// 从包包拿放
		if (Input.GetKeyDown (KeyCode.A)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "单手把小东西从包中拿出", null);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "单手把小东西放入包内", null);
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "双手把中型东西从包中拿出", null);
		}
		if (Input.GetKeyDown (KeyCode.F)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "双手把中型东西放入包内", null);
		}

		// 从对方拿放
		if (Input.GetKeyDown (KeyCode.Q)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "单手把小东西从另一个人手中接过", null);
		}
		if (Input.GetKeyDown (KeyCode.W)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "单手把小东西递给另一个人", null);
		}
		if (Input.GetKeyDown (KeyCode.E)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "双手把中型东西从另一个人手中接过", null);
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			ActionPutDownStuff ac = gameObject.AddComponent<ActionPutDownStuff> ();
			ac.Setting (gameObject, "双手把中型东西递给另一个人", null);
		}
	}
}
