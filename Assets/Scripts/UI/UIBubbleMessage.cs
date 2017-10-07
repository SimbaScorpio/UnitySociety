using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class UIBubbleMessage : MonoBehaviour
	{
		public float lifeTime = 10f;
		public float showSpeed = 5f;
		public float fadeSpeed = 2f;
		public float panSpeed = 200f;
		public float panGap = 2f;
		public float elementEmptyHeight = 20f;
		public List<GameObject> bubblePrefabs;

		private List<GameObject> displayList = new List<GameObject> ();

		private List<Info> waitingList = new List<Info> ();
		private bool canPop = true;

		private class Info
		{
			public int type;
			public string title;
			public string content;

			public Info (int type, string title, string content)
			{
				this.type = type;
				this.title = title;
				this.content = content;
			}
		}

		private static UIBubbleMessage instance;

		public static UIBubbleMessage GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		void Update ()
		{
			if (canPop && waitingList.Count > 0) {
				canPop = false;
				PopMessage (waitingList [0]);
			}
		}

		public void PushMessage (int type, string title, string content)
		{
			Info info = new Info (type, title, content);
			waitingList.Add (info);
		}

		void PopMessage (Info info)
		{
			GameObject obj = CreateBubbleOfType (info.type);
			Text[] texts = obj.transform.GetComponentsInChildren<Text> ();

			ContentSizeFitter fitter = texts [1].GetComponent<ContentSizeFitter> ();
			fitter.CallBack (delegate(Vector2 size) {
				fitter.CallBack (null);
				float elementHeight = elementEmptyHeight + size.y;
				RectTransform tr = obj.transform as RectTransform;
				tr.sizeDelta = new Vector2 (tr.sizeDelta.x, elementHeight);
				StartCoroutine (StartPopping (elementHeight + panGap, obj));
			});
			texts [0].text = info.title;
			texts [1].text = info.content;
		}


		IEnumerator StartPopping (float distance, GameObject obj)
		{
			print ("StartPopping");
			if (displayList.Count == 0) {
				OnShowNewItem (obj);
			} else {
				Dictionary<GameObject, Vector3> target = new Dictionary<GameObject, Vector3> ();
				foreach (GameObject key in displayList) {
					Vector3 targetPos = key.transform.localPosition;
					targetPos.y -= distance;
					target [key] = targetPos;
				}
				bool finish = false;
				while (!finish) {
					finish = true;
					foreach (GameObject key in displayList) {
						Vector3 nextPos = key.transform.localPosition;
						nextPos.y -= panSpeed * Time.deltaTime;
						if (nextPos.y - target [key].y > 0.1f) {
							key.transform.localPosition = nextPos;
							finish = false;
						} else {
							key.transform.localPosition = target [key];
						}
						CheckOutOfBorder (key);
					}
					yield return null;
				}
				OnShowNewItem (obj);
			}
		}


		void OnShowNewItem (GameObject obj)
		{
			Info latestInfo = waitingList [0];
			waitingList.RemoveAt (0);
			displayList.Add (obj);
			StartCoroutine (StartShowing (obj));
		}


		IEnumerator StartShowing (GameObject obj)
		{
			while (GetAlpha (obj, true) < 1) {
				SetAlpha (obj, showSpeed * Time.deltaTime, false);
				yield return null;
			}
			StartCoroutine (FadeCountDown (obj));
			canPop = true;
		}

		IEnumerator StartFading (GameObject obj)
		{
			while (GetAlpha (obj, false) > 0) {
				SetAlpha (obj, -fadeSpeed * Time.deltaTime, false);
				yield return null;
			}
			displayList.Remove (obj);
			Destroy (obj);
		}

		IEnumerator FadeCountDown (GameObject obj)
		{
			float countdown = lifeTime;
			while (countdown > 0 && GetAlpha (obj, true) >= 1) {
				countdown -= Time.deltaTime;
				yield return null;
			}
			StartCoroutine (StartFading (obj));
		}


		bool CheckOutOfBorder (GameObject obj)
		{
			RectTransform tr1 = transform as RectTransform;
			RectTransform tr2 = obj.transform as RectTransform;
			if (tr2.localPosition.y - tr2.sizeDelta.y < -tr1.sizeDelta.y) {
				if (GetAlpha (obj, true) >= 1) {
					StartCoroutine (StartFading (obj));
					return true;
				}
			}
			return false;
		}

		GameObject CreateBubbleOfType (int type)
		{
			GameObject newOne = Instantiate (bubblePrefabs [type]) as GameObject;
			newOne.transform.SetParent (this.gameObject.transform);
			newOne.transform.localScale = Vector3.one;
			newOne.transform.localPosition = Vector3.zero;
			SetAlpha (newOne, 0, true);
			newOne.SetActive (true);
			return newOne;
		}


		void SetAlpha (GameObject obj, float num, bool isSet)
		{
			Image[] images = obj.GetComponentsInChildren<Image> ();
			for (int i = 0; i < images.Length; ++i) {
				Color c = images [i].color;
				images [i].color = isSet ? new Color (c.r, c.g, c.b, num) : new Color (c.r, c.g, c.b, c.a + num);
			}
			Text[] texts = obj.GetComponentsInChildren<Text> ();
			for (int i = 0; i < texts.Length; ++i) {
				Color c = texts [i].color;
				texts [i].color = isSet ? new Color (c.r, c.g, c.b, num) : new Color (c.r, c.g, c.b, c.a + num);
			}
		}

		float GetAlpha (GameObject obj, bool isMin)
		{
			if (obj == null)
				return -1;
			float min = float.MaxValue;
			float max = float.MinValue;
			Image[] images = obj.GetComponentsInChildren<Image> ();
			for (int i = 0; i < images.Length; ++i) {
				float value = images [i].color.a;
				min = value > min ? min : value;
				max = value < max ? max : value;
			}
			Text[] texts = obj.GetComponentsInChildren<Text> ();
			for (int i = 0; i < texts.Length; ++i) {
				float value = texts [i].color.a;
				min = value > min ? min : value;
				max = value < max ? max : value;
			}
			return isMin ? min : max;
		}
	}
}