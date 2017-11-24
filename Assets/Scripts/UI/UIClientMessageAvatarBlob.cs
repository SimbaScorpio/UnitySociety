using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class UIClientMessageAvatarBlob : MonoBehaviour
	{
		public GameObject prefab;
		private List<GameObject> factory = new List<GameObject> ();

		#region instance

		private static UIClientMessageAvatarBlob instance;

		public static UIClientMessageAvatarBlob GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		#endregion

		public void PushBlob (GameObject serverai)
		{
			if (serverai == null)
				return;
			GameObject obj = Get ();
			CharacterData cdata = serverai.GetComponent<NetworkServerAI> ().character;
			RawImage avatar = obj.transform.Find ("PersonInfo/Avatar").GetComponent<RawImage> ();
			avatar.texture = AvatarSnapManager.GetInstance ().GetAvatar (cdata);
			Text[] texts = obj.GetComponentsInChildren<Text> ();
			texts [0].text = "这个人是";
			texts [1].text = cdata.name;
			texts [2].text = cdata.job;
			UIClientMessageMenu.GetInstance ().PushMessage (obj, 1);
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
			Text[] texts = obj.GetComponentsInChildren<Text> ();
			AvatarSnapManager.GetInstance ().RemoveAvatar (texts [1].text);
			factory.Add (obj);
		}
	}
}