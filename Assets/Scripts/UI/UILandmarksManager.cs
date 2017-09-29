using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DesignSociety
{
	public class UILandmarksManager : MonoBehaviour
	{
		// raycast button (need to change color)
		public Button raycastButton;
		// mark
		public GameObject markObj;
		// search
		public InputField searchField;
		public Text searchInfo;
		public GameObject deletePanel;
		// input field of position and rotation
		public InputField[] inputFields;


		private UIScrollView scrollview;

		private List<Landmark> data = new List<Landmark> ();
		private List<Landmark> selectedLandmarks = new List<Landmark> ();
		private List<GameObject> objItems;
		private Landmark hookLandmark;

		private const string defaultName = "新建坐标";
		private float doubleClickThreshold = 0.4f;
		private float clickGap = 0.0f;
		private bool waitClick = false;

		private ColorBlock raycastBtnColors;
		private bool isRaycasting = false;
		private float rayDistance = 1000;

		private BasicCameraController basicCameraScript;
		private const float distanceFromMark = 12;

		private List<Landmark> searchResults = new List<Landmark> ();
		private Landmark lastSearchLandmark;
		private string lastSearchName;

		private static UILandmarksManager instance;

		public static UILandmarksManager GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
			scrollview = GetComponent<UIScrollView> ();
			scrollview.OnListIndexUpdated += OnListIndexUpdated;
			scrollview.OnBeginDragHandler += OnBeginDragHandler;
			scrollview.OnEndDragHandler += OnEndDragHandler;
			scrollview.Initialize ();

			basicCameraScript = Camera.main.GetComponent<BasicCameraController> ();
			raycastBtnColors = raycastButton.colors;
			markObj.SetActive (false);
			foreach (InputField inputField in inputFields) {
				inputField.onEndEdit.AddListener (delegate {
					OnInputFieldEndEdit ();
				});
			}
			JumpOperation ();

			for (int i = 0; i < 33; ++i) {
				AddNewItems ();
			}
		}

		public void Initialize (Landmark[] list)
		{
			data.Clear ();
			selectedLandmarks.Clear ();
			hookLandmark = null;
			JumpOperation ();

			for (int i = 0; i < list.Length; ++i) {
				data.Add (list [i]);
			}
			scrollview.SetTotalCount (data.Count);
		}


		// ensure nonredundant name by adding (num), if [skip] is true, won't check selected items
		public string TryToGetValidName (string name, bool skip)
		{
			name = ExtractSeriesString (name);
			int index = 1;
			string temp = name;
			while (IsNameExist (temp, skip))
				temp = name + "(" + (index++).ToString () + ")";
			return temp;
		}

		// extract "(1)" formed string
		string ExtractSeriesString (string name)
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
			return name;
		}

		// check name existence, if [skip] is true, won't check selected items
		bool IsNameExist (string name, bool skip)
		{
			for (int i = 0; i < data.Count; ++i) {
				if (skip && selectedLandmarks.Contains (data [i]))
					continue;
				if (data [i].m_name == name)
					return true;
			}
			return false;
		}


		void Update ()
		{
			DetectingLagClick ();
			RaycastingLandmark ();
		}

		void DetectingLagClick ()
		{
			clickGap += Time.deltaTime;
			if (waitClick && clickGap > doubleClickThreshold) {
				UILandmarkItem hookItem = FindLandmarkItem (hookLandmark);
				if (hookItem != null)
					hookItem.OnButtonLagClicked ();
				waitClick = false;
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
					if (hookLandmark == null || selectedLandmarks.Count != 1) {
						JumpOperation ();
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
						hookLandmark.m_data [0] = hit.point.x;
						hookLandmark.m_data [1] = hit.point.y;
						hookLandmark.m_data [2] = hit.point.z;
						DisplayLandmarkData (hookLandmark);
					}
				}
			}
		}


		#region 事件监听

		void OnListIndexUpdated (List<GameObject> items, int min, int max)
		{
			for (int i = min; i <= max; ++i) {
				UILandmarkItem item = items [i - min].GetComponent<UILandmarkItem> ();
				item.CloseInputField ();
				item.landmark = data [i];
				item.SetName (item.landmark.m_name);
				item.UpdateTag (item.landmark.m_label);
				if (selectedLandmarks.Contains (item.landmark)) {
					item.ColorPressed ();
				} else {
					item.ColorNormal ();
				}
			}
			objItems = items;
		}

		private List<int> dragIndexes = new List<int> ();
		List<Landmark> dragLandmarks = new List<Landmark> ();

		void OnBeginDragHandler (int index)
		{
			dragIndexes.Clear ();
			dragLandmarks.Clear ();
			// prevent drag event until condition match
			scrollview.SetDragable (false);
			// record indexes of selected items
			for (int i = 0; i < selectedLandmarks.Count; ++i) {
				int j = data.IndexOf (selectedLandmarks [i]);
				dragIndexes.Add (j);
				// if pointer is above one of the selected items, enable drag event
				if (index == j)
					scrollview.SetDragable (true);
			}
			// sort indexes from smaller to larger
			dragIndexes.Sort ();
		}

		void OnEndDragHandler (int index)
		{
			if (dragIndexes [0] == index)
				return;
			// use to record how many selected item indexes are above the target index
			int lose = 0;
			// since drag indexes are locally recorded, items should be stored before removing any one of them
			for (int i = 0; i < dragIndexes.Count; ++i) {
				dragLandmarks.Add (data [dragIndexes [i]]);
				if (index > dragIndexes [i])
					lose += 1;
			}
			// now we can remove the selected items, and store every item's delta index towards first element
			for (int i = 0; i < dragIndexes.Count; ++i) {
				data.RemoveAt (dragIndexes [i] - i);
				if (i != 0)
					dragIndexes [i] = dragIndexes [i] - dragIndexes [0];
			}
			// since all selected items have been removed, first index changes according to lose value
			int startIndex = index - lose;
			data.Insert (startIndex, dragLandmarks [0]);
			// after inserting the first item, the rest of them could be inserted according to delta index
			for (int i = 1; i < dragIndexes.Count; ++i) {
				int j = startIndex + dragIndexes [i];
				j = j > data.Count ? data.Count : j;
				data.Insert (j, dragLandmarks [i]);
			}
			scrollview.SetTotalCount (data.Count);
		}

		#endregion


		#region 坐标显示栏

		void OnInputFieldEndEdit ()
		{
			for (int i = 0; i < inputFields.Length; ++i) {
				float num = hookLandmark.m_data [i];
				string str = inputFields [i].text;
				inputFields [i].text = num.ToString ("f2");
				if (float.TryParse (str, out num)) {
					hookLandmark.m_data [i] = num;
					inputFields [i].text = num.ToString ("f2");
				}
			}
			SetLandmarkModelTo (hookLandmark);
		}

		void DisplayLandmarkData (Landmark landmark)
		{
			if (landmark == null) {
				for (int i = 0; i < inputFields.Length; ++i) {
					inputFields [i].text = "";
					inputFields [i].interactable = false;
				}
				markObj.SetActive (false);
				return;
			} 
			for (int i = 0; i < inputFields.Length; ++i) {
				float num = landmark.m_data [i];
				inputFields [i].text = num.ToString ("f2");
				inputFields [i].interactable = true;
			}
			SetLandmarkModelTo (landmark);
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

		#endregion


		#region 搜索栏

		public void OnSearchBtnClicked (bool upSearch)
		{
			string name = searchField.text;
			// 空输入，隐藏数量统计
			if (string.IsNullOrEmpty (name)) {
				lastSearchName = "";
				searchResults.Clear ();
				DisplaySearchInfo (0, -1);
				return;
			}
			// 新输入，更新参数
			if (name != lastSearchName) {
				lastSearchName = name;
				lastSearchLandmark = null;
			}
			// 寻找全部符合的结果
			FindAllSearchResults (name);
			// 无结果显示
			if (searchResults.Count == 0) {
				DisplaySearchInfo (0, 0);
				return;
			}
			// 必定有结果，但搜索历史可能已不存在
			int searchListIndex = searchResults.IndexOf (lastSearchLandmark);
			// 如果搜索历史不存在，回到首位记录
			if (searchListIndex < 0) {
				lastSearchLandmark = searchResults [0];
				DisplaySearchInfo (1, searchResults.Count);
			} else if (upSearch) {
				searchListIndex = (searchResults.Count + searchListIndex - 1) % searchResults.Count;
				lastSearchLandmark = searchResults [searchListIndex];
				DisplaySearchInfo (searchListIndex + 1, searchResults.Count);
			} else if (!upSearch) {
				searchListIndex = (searchListIndex + 1) % searchResults.Count;
				lastSearchLandmark = searchResults [searchListIndex];
				DisplaySearchInfo (searchListIndex + 1, searchResults.Count);
			}
			int dataListIndex = data.IndexOf (lastSearchLandmark);
			SelectItem (lastSearchLandmark, false, false, false);
			scrollview.ScrollViewTo (dataListIndex);
		}

		void FindAllSearchResults (string name)
		{
			searchResults.Clear ();
			for (int i = 0; i < data.Count; ++i) {
				if (data [i].m_name.Contains (name)) {
					searchResults.Add (data [i]);
				}
			}
		}

		void DisplaySearchInfo (int index, int total)
		{
			if (index > total) {
				searchInfo.text = "";
			} else {
				searchInfo.text = index + " / " + total;
			}
		}

		void JumpOperation ()
		{
			waitClick = false;
			clickGap = 0.0f;
			if (isRaycasting)
				SwitchRaycastMode ();
			DisplayLandmarkData (null);
		}

		#endregion


		#region 主视窗栏

		public void SelectItem (Landmark landmark, bool shift, bool ctrl, bool enableEvent)
		{
			if (!shift && !ctrl) {
				// 如果先前即为单选，且选项与当前相同，那么判断双击事件
				if (selectedLandmarks.Count == 1 && hookLandmark == landmark) {
					if (!enableEvent)
						return;
					if (clickGap < doubleClickThreshold) {
						waitClick = false;
						UILandmarkItem item = FindLandmarkItem (landmark);
						if (item != null)
							item.OnButtonDoubleClicked ();
					} else if (!waitClick) {
						waitClick = true;
						clickGap = 0.0f;
					}
				} else {
					ClearUpSelectedLandmarks ();
					selectedLandmarks.Add (landmark);
					UILandmarkItem item = FindLandmarkItem (landmark);
					if (item != null)
						item.ColorPressed ();
					hookLandmark = landmark;
					JumpOperation ();
					DisplayLandmarkData (landmark);
				}
			} else if (shift && !ctrl) {
				if (hookLandmark == null || hookLandmark == landmark) {
					SelectItem (landmark, false, false, enableEvent);
				} else {
					ClearUpSelectedLandmarks ();
					int index1 = data.IndexOf (hookLandmark);
					int index2 = data.IndexOf (landmark);
					int minI = index1 < index2 ? index1 : index2;
					int maxI = index1 < index2 ? index2 : index1;
					for (int i = minI; i <= maxI; ++i) {
						selectedLandmarks.Add (data [i]);
						UILandmarkItem item = FindLandmarkItem (data [i]);
						if (item != null)
							item.ColorPressed ();
					}
					JumpOperation ();
				}
			} else if (!shift && ctrl) {
				int index = selectedLandmarks.IndexOf (landmark);
				if (index >= 0) {
					UILandmarkItem item = FindLandmarkItem (landmark);
					if (item != null)
						item.ColorNormal ();
					selectedLandmarks.RemoveAt (index);
				} else {
					UILandmarkItem item = FindLandmarkItem (landmark);
					if (item != null)
						item.ColorPressed ();
					selectedLandmarks.Add (landmark);
				}
				JumpOperation ();
				if (selectedLandmarks.Count == 1) {
					hookLandmark = landmark;
					DisplayLandmarkData (landmark);
				}
			}
		}

		void ClearUpSelectedLandmarks ()
		{
			for (int i = 0; i < selectedLandmarks.Count; ++i) {
				UILandmarkItem item = FindLandmarkItem (selectedLandmarks [i]);
				if (item != null)
					item.ColorNormal ();
			}
			selectedLandmarks.Clear ();
		}

		UILandmarkItem FindLandmarkItem (Landmark landmark)
		{
			for (int i = 0; i < objItems.Count; ++i) {
				if (objItems [i].activeInHierarchy) {
					UILandmarkItem item = objItems [i].GetComponent<UILandmarkItem> ();
					if (item != null && item.landmark == landmark)
						return item;
				}
			}
			return null;
		}

		#endregion


		#region 菜单按钮

		public void AddNewItems ()
		{
			if (selectedLandmarks.Count == 0) {
				Landmark temp = AddNewItemAt (data.Count, null);
				SelectItem (temp, false, false, false);
			} else {
				List<Landmark> list = new List<Landmark> ();
				for (int i = 0; i < selectedLandmarks.Count; ++i) {
					list.Add (selectedLandmarks [i]);
				}
				ClearUpSelectedLandmarks ();
				hookLandmark = null;
				for (int i = 0; i < list.Count; ++i) {
					int index = data.IndexOf (list [i]) + 1;
					Landmark temp = AddNewItemAt (index, list [i]);
					SelectItem (temp, false, true, false);
				}
				// 特殊的，添加单独的最后一项，保持窗口置底
				if (selectedLandmarks.Count == 1 && hookLandmark == data [data.Count - 1]) {
					scrollview.ScrollViewTo (data.Count - 1);
				}
			}
		}

		Landmark AddNewItemAt (int index, Landmark landmark)
		{
			Landmark temp = landmark == null ? new Landmark () : landmark.Copy ();
			temp.m_name = TryToGetValidName (defaultName, false);
			data.Insert (index, temp);
			scrollview.SetTotalCount (data.Count);
			return temp;
		}

		public void SwitchRaycastMode ()
		{
			if (isRaycasting) {
				isRaycasting = false;
				raycastButton.colors = raycastBtnColors;
			} else {
				if (hookLandmark == null || selectedLandmarks.Count != 1)
					return;
				isRaycasting = true;
				raycastButton.colors = UILandmarkItem.ColorBlock (raycastBtnColors.pressedColor);
			}
		}

		public void Save ()
		{
			LandmarkList list = new LandmarkList ();
			list.landmarkList = new Landmark[data.Count];
			for (int i = 0; i < data.Count; ++i) {
				list.landmarkList [i] = data [i].Copy ();
			}
			if (FileManager.GetInstance ().SaveLandmarkData (list)) {
				Log.info (Log.green ("坐标保存成功 ヽ(✿ﾟ▽ﾟ)ノ"));
			} else {
				Log.error ("坐标保存失败 w(ﾟДﾟ)w");
			}
		}

		public void DeleteItems ()
		{
			for (int i = 0; i < selectedLandmarks.Count; ++i) {
				data.Remove (selectedLandmarks [i]);
			}
			selectedLandmarks.Clear ();
			hookLandmark = null;
			scrollview.SetTotalCount (data.Count);
			JumpOperation ();
			HideDeletePanel ();
		}

		public void ShowDeletePanel ()
		{
			if (selectedLandmarks.Count != 0) {
				Text text = deletePanel.transform.GetComponentInChildren<Text> ();
				text.text = "确定删除 " + selectedLandmarks.Count + " 项？";
				deletePanel.SetActive (true);
			}
		}

		public void HideDeletePanel ()
		{
			deletePanel.SetActive (false);
		}

		#endregion
	}
}
