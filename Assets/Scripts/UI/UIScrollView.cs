using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class UIScrollView : MonoBehaviour
	{
		public GameObject itemPref;
		public RectTransform content;
		public ScrollRect scrollRect;
		// 刚好能显示的单元数量（实际会多实例1个保证滑动无空白）
		public int displayCount;
		public bool debug;

		[HideInInspector]
		public delegate void UpdateListIndex (List<GameObject> items, int minIndex, int maxIndex);

		public event UpdateListIndex OnListIndexUpdated;

		private List<GameObject> items = new List<GameObject> ();
		private float itemWidth;
		private float itemHeight;
		private int totalCount;

		private int minIndex;
		private int maxIndex;
	

		// 初始化单元长宽，创建预设，绑定滑动条事件
		public void Initialize ()
		{
			itemWidth = (itemPref.transform as RectTransform).sizeDelta.x;
			itemHeight = (itemPref.transform as RectTransform).sizeDelta.y;
			FitContentSize (0);
			for (int i = 0; i < displayCount + 1; ++i) {
				GameObject obj = i < items.Count ? items [i] : (GameObject)Instantiate (itemPref);
				obj.transform.SetParent (content);
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = new Vector3 (0, -i * itemHeight, 0);
				if (i >= items.Count)
					items.Add (obj);
			}
			scrollRect.onValueChanged.AddListener (delegate {
				OnScrollValueChanged ();
			});
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
			if (Mathf.Abs (scrollRect.verticalNormalizedPosition) < 0.01f) {
				content.sizeDelta = new Vector2 (itemWidth, num * itemHeight);
				scrollRect.verticalNormalizedPosition = 0f;
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
				items [i].transform.localPosition = new Vector3 (0, -(minIndex + i) * itemHeight, 0);
			}
		}

		void UpdateInfo ()
		{
			minIndex = totalCount <= displayCount + 1 ? 0 :
				Mathf.FloorToInt ((1 - scrollRect.verticalNormalizedPosition) * (content.sizeDelta.y - displayCount * itemHeight) / itemHeight);
			minIndex = minIndex < 0 ? 0 : minIndex;
			maxIndex = totalCount <= displayCount ? totalCount - 1 : 
				minIndex + displayCount < totalCount - 1 ? minIndex + displayCount : totalCount - 1;
			if (OnListIndexUpdated != null)
				OnListIndexUpdated (items, minIndex, maxIndex);
			if (debug)
				Debug.Log ("min:" + minIndex + " max:" + maxIndex);
		}

		// 滑动视图至特定元素序号（尽量置顶）
		public void ScrollViewTo (int index)
		{
			float ratio = 1 - (float)(index) * itemHeight / (float)(content.sizeDelta.y - displayCount * itemHeight);
			ratio = ratio < 0 ? 0 : ratio;
			ratio = ratio > 1 ? 1 : ratio;
			scrollRect.verticalNormalizedPosition = ratio;
		}

		// 逐步向上滑动窗口
		void RollingUpwards ()
		{
			
		}

		// 逐步向下滑动窗口
		void RollingDownwards ()
		{
			
		}
	}
}
