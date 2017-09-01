using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILandmarkManager : MonoBehaviour
{
	public GameObject itemPref;
	public RectTransform content;
	[HideInInspector]
	public UILandmarkItem selectedItem;
	private float clickGap = 0.0f;
	private float doubleClickThreshold = 0.4f;
	private bool waitClick = false;

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
		lmlist = new List<UILandmarkItem> ();
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
		} else {
			if (clickGap < doubleClickThreshold) {
				waitClick = false;
				selectedItem.OnButtonDoubleClicked ();
				print ("!");
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

	void Update ()
	{
		clickGap += Time.deltaTime;
		if (waitClick) {
			if (clickGap > doubleClickThreshold) {
				selectedItem.OnButtonLagClicked ();
				waitClick = false;
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
