using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DesignSociety
{
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
		private UILandmarkItem hookItem;
		private List<UILandmarkItem> selectedItems = new List<UILandmarkItem> ();
		private float clickGap = 0.0f;
		private float doubleClickThreshold = 0.4f;
		private bool waitClick = false;
		// raycast zone
		private ColorBlock raycastBtnColors;
		private bool isRaycasting = false;
		private float rayDistance = 1000;
		// camera zone
		private BasicCameraController basicCameraScript;
		private const float distanceFromMark = 12;

		// local data zone
		private List<UILandmarkItem> lmlist = new List<UILandmarkItem> ();
		private const string defaultName = "新建坐标";
		private float itemHeight;

		private static UILandmarkManager instance;

		public static UILandmarkManager GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
			basicCameraScript = Camera.main.GetComponent<BasicCameraController> ();
			raycastBtnColors = raycastButton.colors;
			foreach (InputField inputField in inputFields) {
				inputField.onEndEdit.AddListener (delegate {
					OnInputFieldEndEdit ();
				});
			}
			markObj.SetActive (false);
			itemHeight = (itemPref.transform as RectTransform).sizeDelta.y;
		}

		public void Initialize (Landmark[] list)
		{
			// clear
			for (int i = 0; i < lmlist.Count; ++i) {
				Destroy (lmlist [i].gameObject);
			}
			hookItem = null;
			lmlist.Clear ();
			selectedItems.Clear ();
			markObj.SetActive (false);
			if (isRaycasting)
				SwitchRaycastMode ();
			DisplayItemData (null);
			// load
			for (int i = 0; i < list.Length; ++i) {
				UILandmarkItem item = AddNewItem (list [i].m_name, list [i].m_label, lmlist.Count);
				item.transform.localPosition = new Vector3 (0, -(lmlist.Count - 1) * itemHeight, 0);
			}
			FitContentSize ();
		}

		void FitContentSize ()
		{
			RectTransform rectTransform = itemPref.transform as RectTransform;
			content.sizeDelta = new Vector2 (rectTransform.sizeDelta.x, lmlist.Count * rectTransform.sizeDelta.y);
		}

		// check name existence, if [skip] is true, won't check selected items
		public bool IsNameExist (string name, bool skip)
		{
			foreach (UILandmarkItem item in lmlist) {
				if (skip && selectedItems.Contains (item))
					continue;
				if (item.landmark.m_name == name)
					return true;
			}
			return false;
		}

		// ensure nonredundant name by adding (num), if [skip] is true, won't check selected items
		public string TryToGetValidName (string name, bool skip)
		{
			if (name [name.Length - 1] == ')') {
				int i = name.LastIndexOf ("(");
				if (i >= 0) {
					string str = name.Substring (i + 1, name.Length - i - 2);
					int num;
					if (int.TryParse (str, out num)) {
						name = name.Substring (0, i);
					}
				}
			}
			int index = 1;
			string temp = name;
			while (IsNameExist (temp, skip))
				temp = name + "(" + (index++).ToString () + ")";
			return temp;
		}


		public void SelectItem (UILandmarkItem item, bool shift, bool ctrl)
		{
			if (!shift && !ctrl) {
				// 如果先前即为单选，且选项与当前相同，那么判断双击事件
				if (selectedItems.Count == 1 && hookItem == item) {
					if (clickGap < doubleClickThreshold) {
						waitClick = false;
						item.OnButtonDoubleClicked ();
					} else if (!waitClick) {
						waitClick = true;
						clickGap = 0.0f;
					}
				} else {
					for (int i = 0; i < selectedItems.Count; ++i) {
						selectedItems [i].ColorNormal ();
						selectedItems.RemoveAt (i--);
					}
					item.ColorPressed ();
					selectedItems.Add (item);
					hookItem = item;
					waitClick = false;
					if (isRaycasting)
						SwitchRaycastMode ();
					DisplayItemData (item);
				}
			} else if (shift && !ctrl) {
				if (hookItem == null || hookItem == item) {
					SelectItem (item, false, false);
				} else {
					for (int i = 0; i < selectedItems.Count; ++i) {
						selectedItems [i].ColorNormal ();
						selectedItems.RemoveAt (i--);
					}
					int index1 = lmlist.IndexOf (hookItem);
					int index2 = lmlist.IndexOf (item);
					int minI = index1 < index2 ? index1 : index2;
					int maxI = index1 < index2 ? index2 : index1;
					for (int i = minI; i <= maxI; ++i) {
						UILandmarkItem temp = lmlist [i];
						selectedItems.Add (temp);
						temp.ColorPressed ();
					}
					waitClick = false;
					if (isRaycasting)
						SwitchRaycastMode ();
					DisplayItemData (null);
				}
			} else if (!shift && ctrl) {
				int index = selectedItems.IndexOf (item);
				if (index >= 0) {
					item.ColorNormal ();
					selectedItems.RemoveAt (index);
				} else {
					item.ColorPressed ();
					selectedItems.Add (item);
				}
				waitClick = false;
				if (isRaycasting)
					SwitchRaycastMode ();
				DisplayItemData (null);
			}
		}

		void DisplayItemData (UILandmarkItem item)
		{
			if (item == null) {
				for (int i = 0; i < inputFields.Length; ++i) {
					inputFields [i].text = "";
					inputFields [i].interactable = false;
				}
				return;
			} 
			for (int i = 0; i < inputFields.Length; ++i) {
				float num = item.landmark.m_data [i];
				inputFields [i].text = num.ToString ("f2");
				inputFields [i].interactable = true;
			}
			SetLandmarkModelTo (item.landmark);
		}

		public void OnInputFieldEndEdit ()
		{
			for (int i = 0; i < inputFields.Length; ++i) {
				float num = hookItem.landmark.m_data [i];
				string str = inputFields [i].text;
				inputFields [i].text = num.ToString ("f2");
				if (float.TryParse (str, out num)) {
					hookItem.landmark.m_data [i] = num;
					inputFields [i].text = num.ToString ("f2");
				}
			}
			SetLandmarkModelTo (hookItem.landmark);
		}

		void SetLandmarkModelTo (Landmark mark)
		{
			markObj.SetActive (true);
			markObj.transform.position = new Vector3 (mark.m_data [0], mark.m_data [1], mark.m_data [2]);
			Vector3 direction = new Vector3 (0, mark.m_data [3], 0);
			markObj.transform.rotation = Quaternion.Euler (direction);
		}

		public void ShiftCameraToMark ()
		{
			if (basicCameraScript != null) {
				basicCameraScript.projection = BasicCameraController.CameraProjection.perspective;
				basicCameraScript.SetProjectionScript ();
				basicCameraScript.isDesired = true;
				basicCameraScript.desiredPosition = markObj.transform.position - basicCameraScript.transform.forward * distanceFromMark;
			}
		}


		void Update ()
		{
			DetectingLagClick ();
			RaycastingLandmark ();
		}

		void DetectingLagClick ()
		{
			clickGap += Time.deltaTime;
			if (waitClick) {
				if (clickGap > doubleClickThreshold) {
					hookItem.OnButtonLagClicked ();
					waitClick = false;
				}
			}
		}

		void RaycastingLandmark ()
		{
			if (isRaycasting) {
				// ui block raycast
				if (EventSystem.current.IsPointerOverGameObject ())
					return;
				if (Input.GetMouseButtonDown (0)) {
					// check validation
					if (hookItem == null || selectedItems.Count != 1) {
						SwitchRaycastMode ();
						return;
					}
					// ray could shoot from different types of camera
					Ray ray;
					RaycastHit hit;
					CameraPerspectiveEditor cameraEditor = Camera.main.GetComponent<CameraPerspectiveEditor> ();
					if (cameraEditor != null && cameraEditor.isActiveAndEnabled)
						ray = cameraEditor.ScreenPointToRay (Input.mousePosition);
					else
						ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					if (Physics.Raycast (ray, out hit, rayDistance)) {
						hookItem.landmark.m_data [0] = hit.point.x;
						hookItem.landmark.m_data [1] = hit.point.y;
						hookItem.landmark.m_data [2] = hit.point.z;
						DisplayItemData (hookItem);
					}
				}
			}
		}

		void MoveItemsToPosition ()
		{
			for (int i = 0; i < lmlist.Count; ++i) {
				lmlist [i].transform.localPosition = new Vector3 (0, -i * itemHeight, 0);
			}
		}


		#region 功能按钮

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
				if (hookItem == null || selectedItems.Count != 1)
					return;
				isRaycasting = true;
				raycastButton.colors = UILandmarkItem.ColorBlock (raycastBtnColors.pressedColor);
			}
		}

		// 添加
		public void AddNewItems ()
		{
			if (selectedItems.Count == 0) {
				int index = lmlist.Count;
				UILandmarkItem item = AddNewItem (defaultName, null, index);
				SelectItem (item, false, false);
			} else {
				for (int i = 0; i < selectedItems.Count; ++i) {
					int index = lmlist.IndexOf (selectedItems [i]) + 1;
					UILandmarkItem item = AddNewItem (selectedItems [i].landmark.m_name, selectedItems [i].landmark.m_label, index);
					if (selectedItems.Count == 1)
						SelectItem (item, false, false);
				}
			}
			MoveItemsToPosition ();
			FitContentSize ();
		}

		UILandmarkItem AddNewItem (string name, string tag, int index)
		{
			GameObject obj = Instantiate (itemPref) as GameObject;
			UILandmarkItem script = obj.GetComponent<UILandmarkItem> ();
			script.landmark = new Landmark ();
			script.UpdateName (name, false);
			script.UpdateTag (tag);
			obj.transform.SetParent (content);
			obj.transform.localScale = Vector3.one;
			lmlist.Insert (index, script);
			return script;
		}

		// 上移
		public void MoveUp ()
		{
			
		}

		// 下移
		public void MoveDown ()
		{
			
		}

		// 删除
		public void Remove ()
		{
			for (int i = 0; i < selectedItems.Count; ++i) {
				Destroy (selectedItems [i].gameObject);
				lmlist.Remove (selectedItems [i]);
				selectedItems.RemoveAt (i--);
			}
			MoveItemsToPosition ();
			FitContentSize ();
			if (isRaycasting)
				SwitchRaycastMode ();
			markObj.SetActive (false);
		}

		#endregion
	}
}