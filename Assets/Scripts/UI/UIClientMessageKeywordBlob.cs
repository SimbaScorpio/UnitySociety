using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DesignSociety
{
	public class UIClientMessageKeywordBlob : MonoBehaviour
	{
		public GameObject prefab;
		private List<GameObject> factory = new List<GameObject> ();

		#region instance

		private static UIClientMessageKeywordBlob instance;

		public static UIClientMessageKeywordBlob GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		#endregion

		public void PushBlob (string name)
		{
			if (string.IsNullOrEmpty (name))
				return;
			Keyword keyword = KeywordCollection.GetInstance ().Get (name);
			if (keyword == null)
				return;
			GameObject obj = Get ();
			Text[] texts = obj.GetComponentsInChildren<Text> ();
			texts [0].text = keyword.keyword;
			TextMeshProUGUI pro = obj.GetComponentInChildren<TextMeshProUGUI> ();
			pro.text = keyword.description;
			UIClientMessageMenu.GetInstance ().PushMessage (obj, 2);
		}

		GameObject Get ()
		{
			GameObject obj;
			if (factory.Count > 0) {
				obj = factory [0];
				factory.RemoveAt (0);
			} else {
				obj = Instantiate (prefab) as GameObject;
			}
			return obj;
		}

		public void Remove (GameObject obj)
		{
			factory.Add (obj);
		}
	}
}