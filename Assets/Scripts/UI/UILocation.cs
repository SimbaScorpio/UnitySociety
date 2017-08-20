using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UILocation : MonoBehaviour
{
	public RectTransform content;
	public Button buttonPref;
	public GameObject mark;
	public BasicCameraController cameraCtrl;

	private List<Button> messages;
	private Dictionary<string, float> lastClickTime;

	void Awake ()
	{
		messages = new List<Button> ();
		lastClickTime = new Dictionary<string, float> ();
	}

	void Start ()
	{
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("Location").OrderBy (g => g.transform.GetSiblingIndex ()).ToArray ();
		foreach (GameObject obj in objs) {
			Button btn = Instantiate (buttonPref) as Button;
			btn.transform.SetParent (content);
			btn.transform.localScale = Vector3.one;
			RectTransform rectTransform = btn.transform as RectTransform;
			btn.transform.localPosition = new Vector3 (2, -messages.Count * rectTransform.sizeDelta.y, 0);
			btn.GetComponentInChildren<Text> ().text = obj.name;
			btn.onClick.AddListener (delegate() {
				mark.transform.position = obj.transform.position;
				if (Time.time - lastClickTime [btn.name] < 0.3f)
					TravelToLocation (obj);
				lastClickTime [btn.name] = Time.time;
			});
			messages.Add (btn);
			lastClickTime [btn.name] = 0.0f;
		}
	}

	void TravelToLocation (GameObject obj)
	{
		cameraCtrl.projection = BasicCameraController.CameraProjection.perspective;
		cameraCtrl.SetProjectionScript ();
		cameraCtrl.isDesired = true;
		cameraCtrl.desiredPosition = obj.transform.position - cameraCtrl.transform.forward * 12;
	}
}
