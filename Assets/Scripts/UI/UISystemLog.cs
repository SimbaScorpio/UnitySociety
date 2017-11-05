using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class UISystemLog : MonoBehaviour
	{
		private UIScrollView scrollview;
		private float maxWidth = 0;

		[HideInInspector]
		public List<string> messages = new List<string> ();

		private static UISystemLog instance;

		public static UISystemLog GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
			scrollview = GetComponent<UIScrollView> ();
			scrollview.OnListIndexUpdated += OnListIndexUpdatedUpdated;
			scrollview.Initialize ();
		}

		public void AddMessage (string message)
		{
			if (string.IsNullOrEmpty (message))
				return;
			messages.Add (message);
			scrollview.SetTotalCount (messages.Count);
		}

		void OnListIndexUpdatedUpdated (List<GameObject> items, int min, int max)
		{
			for (int i = min; i <= max; ++i) {
				Text text = items [i - min].GetComponent<Text> ();
				text.text = messages [i];
				ContentSizeFitter fitter = text.GetComponent<ContentSizeFitter> ();
				fitter.CallBack (delegate(Vector2 size) {
					if (size.x > maxWidth)
						maxWidth = size.x;
				});
			}
			scrollview.content.sizeDelta = new Vector2 (maxWidth + 2, scrollview.content.sizeDelta.y);
		}

		public void ClearLog ()
		{
			UISystemLog.GetInstance ().messages.Clear ();
			scrollview.SetTotalCount (0);
		}

		public void SaveLog ()
		{
			string data = "";
			foreach (string log in UISystemLog.GetInstance().messages) {
				data += log + Environment.NewLine;
			}
			if (FileManager.GetInstance ().SaveLogData (data)) {
				Log.info (Log.green ("日志保存成功 ヽ(✿ﾟ▽ﾟ)ノ"));
			} else {
				Log.error ("日志保存失败 w(ﾟДﾟ)w");
			}
		}
	}
}