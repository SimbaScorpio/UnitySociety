using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class UIAppearanceMenu : MonoBehaviour
	{
		public float lerpRate = 20f;
		public bool isLerping;

		#region instance

		private static UIAppearanceMenu instance;

		public static UIAppearanceMenu GetInstance ()
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
			StartCoroutine (LerpShowing ());
		}

		public void Hide ()
		{
			StopAllCoroutines ();
			StartCoroutine (LerpHiding ());
		}

		#region show/hide

		IEnumerator LerpShowing ()
		{
			isLerping = true;
			RectTransform tr = transform as RectTransform;
			while (tr.localPosition.y < -0.001f) {
				Vector3 pos = tr.localPosition;
				pos.y = Mathf.Lerp (pos.y, 0, Time.deltaTime * lerpRate);
				tr.localPosition = pos;
				yield return null;
			}
			isLerping = false;
		}

		IEnumerator LerpHiding ()
		{
			isLerping = true;
			RectTransform tr = transform as RectTransform;
			float height = tr.rect.height;
			while (tr.localPosition.y + height > 0.001f) {
				Vector3 pos = tr.localPosition;
				pos.y = Mathf.Lerp (pos.y, -height, Time.deltaTime * lerpRate);
				tr.localPosition = pos;
				yield return null;
			}
			isLerping = false;
		}

		#endregion


		public void OnBodyChangeButtonClicked ()
		{
			NetworkPlayerResources.GetInstance ().RandomlyChangeBodyType ();
		}

		public void OnClothesChangeButtonClicked ()
		{
			NetworkPlayerResources.GetInstance ().RandomlyChangeClothes ();
		}
	}
}