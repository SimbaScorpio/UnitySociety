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

		public GameObject IdlePanel;
		public GameObject ActivePanel;

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
			//StartCoroutine (Showing ());
			StartCoroutine (LerpShowing ());
		}

		public void Hide ()
		{
			StopAllCoroutines ();
			//StartCoroutine (Hiding ());
			StartCoroutine (LerpHiding ());
		}

		#region show/hide

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

		IEnumerator LerpShowing ()
		{
			isLerping = true;
			while (transform.position.x < -0.001f) {
				Vector3 pos = transform.localPosition;
				pos.x = Mathf.Lerp (pos.x, 0, Time.deltaTime * lerpRate);
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

		IEnumerator LerpHiding ()
		{
			isLerping = true;
			RectTransform tr = transform as RectTransform;
			float width = tr.rect.width;
			while (tr.localPosition.x + width > 0.001f) {
				Vector3 pos = tr.localPosition;
				pos.x = Mathf.Lerp (pos.x, -width, Time.deltaTime * lerpRate);
				tr.localPosition = pos;
				yield return null;
			}
			isLerping = false;
		}

		#endregion

		public void ShowIdlePanel ()
		{
			if (ActivePanel != null)
				ActivePanel.SetActive (false);
			if (IdlePanel != null)
				IdlePanel.SetActive (true);
		}

		public void ShowActivePanel ()
		{
			if (ActivePanel != null)
				ActivePanel.SetActive (true);
			if (IdlePanel != null)
				IdlePanel.SetActive (false);
		}
	}
}