using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class UIClientMessageMenu : MonoBehaviour
	{
		public float panSpeed = 300f;
		public float showSpeed = 5f;
		public int maxWaitNumber = 5;
		public Transform root;

		private Dictionary<GameObject, int> objToType = new Dictionary<GameObject, int> ();
		private List<GameObject> displayList = new List<GameObject> ();
		private List<GameObject> waitingList = new List<GameObject> ();
		private bool canPop = true;

		#region instance

		private static UIClientMessageMenu instance;

		public static UIClientMessageMenu GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		#endregion

		public void PushMessage (GameObject blob, int type)
		{
			objToType [blob] = type;
			blob.SetActive (false);
			blob.transform.SetParent (root.transform);
			blob.transform.localScale = Vector3.zero;
			float height = (blob.transform as RectTransform).sizeDelta.y;
			blob.transform.localPosition = new Vector3 (0, -height / 2f, 0);

			if (waitingList.Count > maxWaitNumber && maxWaitNumber > 2) {
				Remove (waitingList [1]);
				waitingList.RemoveAt (1);
			}
			waitingList.Add (blob);
		}

		void Update ()
		{
			if (canPop && waitingList.Count > 0 && !UIInformationMenu.GetInstance ().isLerping) {
				canPop = false;
				StartCoroutine (PopingMessage (waitingList [0]));
			}
		}

		// 移开其他气泡，腾出位置给即将弹出的气泡
		IEnumerator PopingMessage (GameObject blob)
		{
			if (displayList.Count == 0) {
				OnShowNewItem (blob);
			} else {
				Dictionary<GameObject, Vector3> target = new Dictionary<GameObject, Vector3> ();
				float distance = (blob.transform as RectTransform).sizeDelta.y;
				GameObject display;
				for (int i = 0; i < displayList.Count; ++i) {
					display = displayList [i];
					Vector3 targetPos = display.transform.localPosition;
					targetPos.y -= distance;
					target [display] = targetPos;
				}
				bool finish = false;
				while (!finish) {
					finish = true;
					for (int i = displayList.Count - 1; i >= 0; --i) {
						display = displayList [i];
						Vector3 nextPos = display.transform.localPosition;
						nextPos.y -= panSpeed * Time.deltaTime;
						if (nextPos.y - target [display].y > 0.1f) {
							display.transform.localPosition = nextPos;
							finish = false;
						} else {
							display.transform.localPosition = target [display];
						}
						CheckOutOfBorder (display);
					}
					yield return null;
				}
				OnShowNewItem (blob);
			}
		}

		// 弹出气泡
		void OnShowNewItem (GameObject blob)
		{
			waitingList.RemoveAt (0);
			displayList.Add (blob);
			blob.SetActive (true);
			StartCoroutine (StartShowing (blob));
		}

		// 气泡弹出中
		IEnumerator StartShowing (GameObject blob)
		{
			Vector3 scale = blob.transform.localScale;
			scale.z = 1;
			while (scale.x < 1 && scale.y < 1) {
				scale.x += Time.deltaTime * showSpeed;
				scale.y += Time.deltaTime * showSpeed;
				blob.transform.localScale = scale;
				yield return null;
			}
			blob.transform.localScale = Vector3.one;
			canPop = true;
		}

		// 气泡超出显示区域则回收
		void CheckOutOfBorder (GameObject blob)
		{
			RectTransform tr1 = transform as RectTransform;
			RectTransform tr2 = blob.transform as RectTransform;
			float height = (blob.transform as RectTransform).sizeDelta.y;
			if (tr2.localPosition.y + height / 2f < -tr1.sizeDelta.y) {
				displayList.Remove (blob);
				Remove (blob);
			}
		}

		void Remove (GameObject blob)
		{
			blob.SetActive (false);
			int type = objToType [blob];
			// avatar
			if (type == 1) {
				UIClientMessageAvatarBlob.GetInstance ().Remove (blob);
			}
		}
	}
}