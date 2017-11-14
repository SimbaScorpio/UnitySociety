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
		[HideInInspector]
		public List<string> infomessages = new List<string> ();
		[HideInInspector]
		public List<string> errormessages = new List<string> ();

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

		public void AddMessage (string message, bool flag)
		{
			if (string.IsNullOrEmpty (message))
				return;
			messages.Add (message);
			if (flag)
				infomessages.Add (message);
			else
				errormessages.Add (message);
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

		public void SaveLog (bool flag)
		{
			string data = "";
			if (flag) {
				foreach (string log in UISystemLog.GetInstance().infomessages) {
					data += log + Environment.NewLine;
				}
				if (FileManager.GetInstance ().SaveInfoLogData (data)) {
					Log.info (Log.green ("时间日志保存成功 ヽ(✿ﾟ▽ﾟ)ノ"));
				} else {
					Log.error ("时间日志保存失败 w(ﾟДﾟ)w");
				}
			} else {
				foreach (string log in UISystemLog.GetInstance().errormessages) {
					data += log + Environment.NewLine;
				}
				if (FileManager.GetInstance ().SaveErrorAndWarningLogData (data)) {
					Log.info (Log.green ("错误日志保存成功 ヽ(✿ﾟ▽ﾟ)ノ"));
				} else {
					Log.error ("错误日志保存失败 w(ﾟДﾟ)w");
				}
			}
		}
	}
}