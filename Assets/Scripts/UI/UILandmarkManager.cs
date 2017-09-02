using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILandmarkManager : MonoBehaviour
{
	// list item used to instantiate
	public GameObject itemPref;
	// item content (dynamically scale with list size)
	public RectTransform content;
	// raycast button (need to change color)
	public Button raycastButton;
	// input field of position and rotation
	public InputField[] inputFields;
	// mark
	public GameObject markObj;

	// select item zone
	private UILandmarkItem selectedItem;
	private float clickGap = 0.0f;
	private float doubleClickThreshold = 0.4f;
	private bool waitClick = false;
	// raycast zone
	private ColorBlock raycastBtnColors;
	private bool isRaycasting = false;
	private float rayDistance = 1000;

	// local data zone
	private List<UILandmarkItem> lmlist;
	private const string defaultName = "新建坐标";

	private static UILandmarkManager instance;

	public static UILandmarkManager GetInstance ()
	{
		return instance;
	}

	void Awake ()
	{
		instance = this;
		raycastBtnColors = raycastButton.colors;
		lmlist = new List<UILandmarkItem> ();
		foreach (InputField inputField in inputFields) {
			inputField.onEndEdit.AddListener (delegate {
				OnInputFieldEndEdit ();
			});
		}
		markObj.SetActive (false);
	}

	public void Initialize (Landmark[] list)
	{
		foreach (Landmark landmark in list) {
			GameObject obj = Instantiate (itemPref) as GameObject;
			UILandmarkItem script = obj.GetComponent<UILandmarkItem> ();
			script.landmark = landmark.Copy ();
			script.SetName (landmark.name);
			script.SetTag (landmark.label);

			obj.transform.SetParent (content);
			obj.transform.localScale = Vector3.one;
			RectTransform rectTransform = obj.transform as RectTransform;
			obj.transform.localPosition = new Vector3 (0, -lmlist.Count * rectTransform.sizeDelta.y, 0);

			lmlist.Add (script);
		}
		FitContentSize ();
	}

	void FitContentSize ()
	{
		RectTransform rectTransform = itemPref.transform as RectTransform;
		content.sizeDelta = new Vector2 (rectTransform.sizeDelta.x, lmlist.Count * rectTransform.sizeDelta.y);
	}

	public bool IsNameExist (string name)
	{
		foreach (UILandmarkItem item in lmlist) {
			if (item.landmark.name == name)
				return true;
		}
		return false;
	}

	public string TryToGetValidName (string name)
	{
		int index = 1;
		string temp = name;
		while (IsNameExist (temp))
			temp = name + "(" + (index++).ToString () + ")";
		return temp;
	}

	// 保存
	public void Save ()
	{
		LandmarkList list = new LandmarkList ();
		list.landmarkList = new Landmark[lmlist.Count];
		for (int i = 0; i < lmlist.Count; ++i) {
			list.landmarkList [i] = lmlist [i].landmark.Copy ();
		}
		if (FileManager.GetInstance ().SaveLandmarkData (list)) {
			Log.info (Log.green ("坐标保存成功 0v0"));
		} else {
			Log.error ("坐标保存失败 T^T");
		}
	}

	// 射线
	public void SwitchRaycastMode ()
	{
		if (isRaycasting) {
			isRaycasting = false;
			raycastButton.colors = raycastBtnColors;
		} else {
			if (selectedItem == null)
				return;
			isRaycasting = true;
			raycastButton.colors = ColorBlock (raycastBtnColors.pressedColor);
		}
	}

	// 添加
	public void AddNewItem ()
	{
		GameObject obj = Instantiate (itemPref) as GameObject;
		UILandmarkItem script = obj.GetComponent<UILandmarkItem> ();

		int index = 0;
		if (selectedItem == null) {
			script.landmark = new Landmark ();
			script.landmark.name = TryToGetValidName (defaultName);
		} else {
			index = lmlist.IndexOf (selectedItem) + 1;
			script.landmark = selectedItem.landmark.Copy ();
			script.landmark.name = TryToGetValidName (script.landmark.name);
		}
		script.SetName (script.landmark.name);
		script.SetTag (script.landmark.label);

		obj.transform.SetParent (content);
		obj.transform.localScale = Vector3.one;
		float height = (obj.transform as RectTransform).sizeDelta.y;
		obj.transform.localPosition = new Vector3 (0, -index * height, 0);

		for (int i = index; i < lmlist.Count; ++i) {
			lmlist [i].gameObject.transform.localPosition = new Vector3 (0, -(i + 1) * height, 0);
		}
	
		lmlist.Insert (index, script);
		FitContentSize ();
	}

	// 上移
	public void MoveUp ()
	{
		if (selectedItem == null)
			return;
		int index = lmlist.IndexOf (selectedItem);
		Switch (index, index - 1);
	}

	// 下移
	public void MoveDown ()
	{
		if (selectedItem == null)
			return;
		int index = lmlist.IndexOf (selectedItem);
		Switch (index, index + 1);
	}

	// 删除
	public void Remove ()
	{
		if (selectedItem == null)
			return;
		lmlist.Remove (selectedItem);
		Destroy (selectedItem.gameObject);
		selectedItem = null;

		for (int i = 0; i < lmlist.Count; ++i) {
			GameObject obj = lmlist [i].gameObject;
			float height = (obj.transform as RectTransform).sizeDelta.y;
			obj.transform.localPosition = new Vector3 (0, -i * height, 0);
		}
		FitContentSize ();

		if (isRaycasting)
			SwitchRaycastMode ();
		markObj.SetActive (false);
	}


	void Switch (int a, int b)
	{
		if (a != b && a >= 0 && a < lmlist.Count && b >= 0 && b < lmlist.Count) {
			// switch ui position
			GameObject obj1 = lmlist [a].gameObject;
			GameObject obj2 = lmlist [b].gameObject;
			float height = (obj1.transform as RectTransform).sizeDelta.y;
			obj1.transform.localPosition = new Vector3 (0, -b * height, 0);
			obj2.transform.localPosition = new Vector3 (0, -a * height, 0);

			// switch list index
			UILandmarkItem temp = lmlist [a];
			lmlist [a] = lmlist [b];
			lmlist [b] = temp;
		}
	}


	public void SelectItem (UILandmarkItem item)
	{
		if (selectedItem != item) {
			if (selectedItem != null)
				selectedItem.button.colors = selectedItem.btnColors;
			selectedItem = item;
			selectedItem.button.colors = ColorBlock (selectedItem.btnColors.pressedColor);
			waitClick = false;
			clickGap = 0.0f;
			if (isRaycasting)
				SwitchRaycastMode ();
			DisplayItemData (selectedItem);
		} else {
			if (clickGap < doubleClickThreshold) {
				waitClick = false;
				selectedItem.OnButtonDoubleClicked ();
			} else if (!waitClick) {
				waitClick = true;
				clickGap = 0.0f;
			} 
		}
	}

	ColorBlock ColorBlock (Color color)
	{
		ColorBlock block = new ColorBlock ();
		block.highlightedColor = color;
		block.normalColor = color;
		block.pressedColor = color;
		block.colorMultiplier = 1;
		return block;
	}

	void DisplayItemData (UILandmarkItem item)
	{
		for (int i = 0; i < inputFields.Length; ++i) {
			float num = item.landmark.data [i];
			inputFields [i].text = num.ToString ("f2");
			if (item == selectedItem) {
				inputFields [i].enabled = true;
			} else {
				inputFields [i].enabled = false;
			}
		}
		SetLandmarkModel (selectedItem.landmark);
	}

	public void OnInputFieldEndEdit ()
	{
		float num = 0.0f;
		for (int i = 0; i < inputFields.Length; ++i) {
			string str = inputFields [i].text;
			if (float.TryParse (str, out num)) {
				inputFields [i].text = num.ToString ("f2");
				selectedItem.landmark.data [i] = num;
			}
		}
		SetLandmarkModel (selectedItem.landmark);
	}


	void SetLandmarkModel (Landmark mark)
	{
		markObj.SetActive (true);
		markObj.transform.position = new Vector3 (mark.data [0], mark.data [1], mark.data [2]);
		Vector3 direction = new Vector3 (0, mark.data [3], 0);
		markObj.transform.rotation = Quaternion.Euler (direction);
	}


	void Update ()
	{
		// detect lag click
		clickGap += Time.deltaTime;
		if (waitClick) {
			if (clickGap > doubleClickThreshold) {
				selectedItem.OnButtonLagClicked ();
				waitClick = false;
			}
		}

		// raycast mode
		if (isRaycasting) {
			if (Input.GetMouseButtonDown (0)) {
				if (selectedItem == null) {
					SwitchRaycastMode ();
					return;
				}
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out hit, rayDistance)) {
					selectedItem.landmark.data [0] = hit.point.x;
					selectedItem.landmark.data [1] = hit.point.y;
					selectedItem.landmark.data [2] = hit.point.z;
					DisplayItemData (selectedItem);
				}
			}
		}
	}

	/* 中文排序暂不考虑
	public void Sort ()
	{
		lmlist.Sort ((p1, p2) => p1.landmark.name.CompareTo (p2.landmark.name));
		for (int i = 0; i < lmlist.Count; ++i) {
			GameObject obj = lmlist [i].gameObject;
			RectTransform rectTransform = obj.transform as RectTransform;
			obj.transform.localPosition = new Vector3 (0, -i * rectTransform.sizeDelta.y, 0);
		}
	}
	*/
}
