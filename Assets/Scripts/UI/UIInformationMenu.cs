using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class UIInformationMenu : MonoBehaviour
	{
		public float lerpRate = 20f;
		public bool isLerping;

		#region instance

		private static UIInformationMenu instance;

		public static UIInformationMenu GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		#endregion

		public void Show ()
		{
			StopAllCoroutines ();
			StartCoroutine (Showing ());
		}

		public void Hide ()
		{
			StopAllCoroutines ();
			StartCoroutine (Hiding ());
		}

		IEnumerator Showing ()
		{
			isLerping = true;
			while (transform.localPosition.x < 0) {
				Vector3 pos = transform.localPosition;
				pos.x += Time.deltaTime * lerpRate * 10;
				pos.x = pos.x > 0 ? 0 : pos.x;
				transform.localPosition = pos;
				yield return null;
			}
			isLerping = false;
		}

		IEnumerator Hiding ()
		{
			isLerping = true;
			RectTransform tr = transform as RectTransform;
			float width = tr.rect.width;
			while (tr.localPosition.x > -width) {
				Vector3 pos = tr.localPosition;
				pos.x -= Time.deltaTime * lerpRate * 10;
				pos.x = pos.x < -width ? -width : pos.x;
				tr.localPosition = pos;
				yield return null;
			}
			isLerping = false;
		}
	}
}