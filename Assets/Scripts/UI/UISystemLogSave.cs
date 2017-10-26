using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class UISystemLogSave : MonoBehaviour
	{
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