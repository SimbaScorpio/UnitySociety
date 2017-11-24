using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class UIKeywordPanel : MonoBehaviour
	{
		public Keyword keyword;
		public bool isActive = false;

		public GameObject panel;
		public Text headerTitle;
		public RectTransform scrollviewContent;
		public Text contentTitle;
		public Text content;
		public Image contentImage;
		public Text contentImageDes;
		//public float ImageGap = 60f;
		//public float ImageHeight = 450f;

		private ScrollRect scrollrect;

		private static UIKeywordPanel instance;

		public static UIKeywordPanel GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
			if (scrollviewContent != null)
				scrollrect = scrollviewContent.GetComponentInParent<ScrollRect> ();
		}

		public void OnCloseButtonClicked ()
		{
			if (panel != null) {
				panel.SetActive (false);
				isActive = false;
			}
		}

		public void OnOpenButtonClicked ()
		{
			if (panel != null) {
				panel.SetActive (true);
				isActive = true;
			}
			if (scrollrect != null)
				scrollrect.verticalNormalizedPosition = 1;
		}

		public void OpenPanel (Keyword keyword)
		{
			if (keyword == null)
				return;
			this.keyword = keyword;

			if (headerTitle != null)
				headerTitle.text = keyword.keyword;
			if (contentTitle != null)
				contentTitle.text = keyword.keyword;
			if (content != null) {
				content.text = keyword.description;
				OnOpenButtonClicked ();
				ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter> ();
				fitter.CallBack (delegate(Vector2 size) {
					fitter.CallBack (null);
					DynamicSizePosition (size);
				});
			}
		}

		void DynamicSizePosition (Vector2 size)
		{
			float height = -content.transform.localPosition.y + size.y;
			//if (contentImage != null) {
			//	Vector3 t = contentImage.transform.localPosition;
			//	contentImage.transform.localPosition = new Vector3 (t.x, -height - ImageGap, t.z);
			//}
			if (scrollviewContent != null) {
				Vector2 s = scrollviewContent.sizeDelta;
				//scrollviewContent.sizeDelta = new Vector2 (s.x, height + ImageHeight);
				scrollviewContent.sizeDelta = new Vector2 (s.x, height);
			}
		}
	}
}