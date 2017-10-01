using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DesignSociety
{
	[RequireComponent (typeof(UIScrollViewData))]
	public class UIScrollView : ScrollRect, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		UIScrollViewData sd;

		public delegate void UpdateListIndex (List<GameObject> items, int minIndex, int maxIndex);

		public delegate void BeginDragHandler (int index);

		public delegate void EndDragHandler (int index);

		public event UpdateListIndex OnListIndexUpdated;
		public event BeginDragHandler OnBeginDragHandler;
		public event EndDragHandler OnEndDragHandler;

		private List<GameObject> items = new List<GameObject> ();
		private float itemWidth;
		private float itemHeight;
		private int displayCount;
		private int totalCount;

		private int minIndex;
		private int maxIndex;

		private bool dragable;
		private int dragBeginIndex = -1;
		private int dragEndIndex = -1;
		private bool truelyDragging;
		private float rollingSpeed;

		private GameObject indicator;


		// 初始化单元长宽，创建预设，绑定滑动条事件
		public void Initialize ()
		{
			sd = GetComponent<UIScrollViewData> ();
			itemWidth = (sd.itemPref.transform as RectTransform).sizeDelta.x;
			itemHeight = (sd.itemPref.transform as RectTransform).sizeDelta.y;
			displayCount = Mathf.CeilToInt (viewport.rect.height / itemHeight);
			Instantiate ();
			this.onValueChanged.AddListener (delegate {
				OnScrollValueChanged ();
			});
			dragable = sd.dragable;
		}

		void Instantiate ()
		{
			for (int i = 0; i < displayCount + 1; ++i) {
				GameObject obj = i < items.Count ? items [i] : (GameObject)Instantiate (sd.itemPref);
				obj.transform.SetParent (content);
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = new Vector3 (obj.transform.localPosition.x, -i * itemHeight, 0);
				obj.SetActive (false);
				if (i >= items.Count)
					items.Add (obj);
			}
			if (indicator == null && sd.splitIndicatorPref != null) {
				indicator = (GameObject)Instantiate (sd.splitIndicatorPref);
				indicator.transform.SetParent (content);
				indicator.transform.localScale = Vector3.one;
				indicator.SetActive (false);
			}
		}

		// 设置数据总数量，可能会改变Content大小，以及部分单元的显示与否
		public void SetTotalCount (int num)
		{
			totalCount = num;
			int min = displayCount + 1 < totalCount ? displayCount + 1 : totalCount;
			for (int i = 0; i < min; ++i) {
				items [i].SetActive (true);
			}
			for (int i = min; i < displayCount + 1; ++i) {
				items [i].SetActive (false);
			}
			FitContentSize (totalCount);
			UpdateInfo ();
		}

		// 改变Content的大小，关联滑动条的显示和长度
		void FitContentSize (int num)
		{
			// 滑动条处于底端时，Content窗口缩小端默认是下端，会出现Content不覆盖Viewport的现象，导致导航错误，此时需要强制导航至底，可考虑更新元素坐标
			if (Mathf.Abs (verticalNormalizedPosition) < 0.01f) {
				content.sizeDelta = new Vector2 (itemWidth, num * itemHeight);
				verticalNormalizedPosition = 0f;
				OnScrollValueChanged ();
			} else {
				content.sizeDelta = new Vector2 (itemWidth, num * itemHeight);
			}
		}

		// 滑动条事件，计算显示区域的序号，循环排列区域外单元的坐标
		void OnScrollValueChanged ()
		{
			UpdateInfo ();
			for (int i = 0; i < items.Count; ++i) {
				items [i].transform.localPosition = new Vector3 (items [i].transform.localPosition.x, -(minIndex + i) * itemHeight, 0);
			}
		}

		void UpdateInfo ()
		{
			minIndex = totalCount <= displayCount + 1 ? 0 :
				Mathf.FloorToInt ((1 - verticalNormalizedPosition) * (content.sizeDelta.y - viewport.rect.height) / itemHeight);
			minIndex = minIndex < 0 ? 0 : minIndex;
			maxIndex = totalCount <= displayCount ? totalCount - 1 : 
				minIndex + displayCount < totalCount - 1 ? minIndex + displayCount : totalCount - 1;
			if (OnListIndexUpdated != null)
				OnListIndexUpdated (items, minIndex, maxIndex);
			if (sd.debug)
				Debug.Log ("min: " + minIndex + " max: " + maxIndex);
		}

		// 滑动视图至特定元素序号（尽量置顶）
		public void ScrollViewTo (int index)
		{
			float ratio = 1 - (float)(index) * itemHeight / (float)(content.sizeDelta.y - viewport.rect.height);
			ratio = ratio < 0 ? 0 : ratio;
			ratio = ratio > 1 ? 1 : ratio;
			verticalNormalizedPosition = ratio;
		}

		void AutoRolling (float speedUp)
		{
			if (speedUp == 0)
				return;
			float delta = sd.autoRollingSpeed * Time.deltaTime / (content.sizeDelta.y - viewport.rect.height);
			float value = verticalNormalizedPosition + delta * speedUp;
			value = value < 0 ? 0 : value;
			value = value > 1 ? 1 : value;
			verticalNormalizedPosition = value;
		}

		public override void OnBeginDrag (PointerEventData eventData)
		{
			if (dragable) {
				dragBeginIndex = GetIndexFromPosition (eventData.position);
				if (OnBeginDragHandler != null) {
					OnBeginDragHandler (dragBeginIndex);
				}
				if (sd.debug) {
					print ("OnBeginDrag: " + dragBeginIndex);
				}
			}
		}

		public override void OnDrag (PointerEventData eventData)
		{
			if (dragable) {
				dragEndIndex = GetIndexFromPosition (eventData.position);
				if (!truelyDragging && dragEndIndex != dragBeginIndex)
					truelyDragging = true;
				
				if (dragEndIndex <= minIndex + 2) {
					rollingSpeed = 1;
				} else if (dragEndIndex >= maxIndex - 2) {
					rollingSpeed = -1;
				} else {
					rollingSpeed = 0;
				}

				if (truelyDragging) {
					RectTransform tr = (RectTransform)indicator.transform;
					tr.localPosition = new Vector3 (0, -dragEndIndex * itemHeight, 0);
					indicator.SetActive (true);
				}
			} else {
				StopDragging ();
			}
		}

		public override void OnEndDrag (PointerEventData eventData)
		{
			if (dragable) {
				if (!truelyDragging)
					return;
				dragEndIndex = GetIndexFromPosition (eventData.position);
				if (OnEndDragHandler != null) {
					OnEndDragHandler (dragEndIndex);
				}
				if (sd.debug) {
					print ("OnEndDrag: " + dragEndIndex);
				}
			}
			StopDragging ();
			this.dragable = sd.dragable;
		}

		public void SetDragable (bool dragable)
		{
			if (sd.dragable)
				this.dragable = dragable;
		}

		void StopDragging ()
		{
			truelyDragging = false;
			rollingSpeed = 0;
			if (indicator != null)
				indicator.SetActive (false);
		}

		int GetIndexFromPosition (Vector2 screenPosition)
		{
			Vector3 position = new Vector3 (screenPosition.x, screenPosition.y, 0);
			position = content.transform.InverseTransformPoint (position);
			int index = Mathf.FloorToInt (-position.y / itemHeight);
			index = index < 0 ? 0 : index;
			index = index > totalCount - 1 ? totalCount - 1 : index;
			return index;
		}

		void Update ()
		{
			if (sd != null && sd.dragable) {
				AutoRolling (rollingSpeed);
			}
		}
	}
}
