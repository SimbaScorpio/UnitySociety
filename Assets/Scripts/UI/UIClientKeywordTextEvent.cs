using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DesignSociety
{
	public class UIClientKeywordTextEvent : MonoBehaviour
	{
		private TextMeshProUGUI pro;
		private int maxPages = 3;

		void Start ()
		{
			pro = GetComponentInChildren<TextMeshProUGUI> ();
		}

		public void OnPageLeft ()
		{
			if (pro.pageToDisplay <= 1) {
				pro.pageToDisplay = 1;
				return;
			}
			pro.pageToDisplay -= 1;
		}

		public void OnPageRight ()
		{
			if (pro.pageToDisplay >= maxPages) {
				pro.pageToDisplay = maxPages;
				return;
			}
			pro.pageToDisplay += 1;
		}
	}
}