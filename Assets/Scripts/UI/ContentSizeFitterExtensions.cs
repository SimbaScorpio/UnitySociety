using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class ContentSizeFitterExtensions
{
	public static void CallBack (this ContentSizeFitter contentSizeFitter, System.Action<Vector2> change)
	{
		ContentSizeFitterEvent test = contentSizeFitter.GetComponent<ContentSizeFitterEvent> () ??
		                              contentSizeFitter.gameObject.AddComponent<ContentSizeFitterEvent> ();
		test.change = change;
	}

	[ExecuteInEditMode]
	public class ContentSizeFitterEvent : UIBehaviour
	{
		public System.Action<Vector2> change = null;

		protected override void OnRectTransformDimensionsChange ()
		{
			base.OnRectTransformDimensionsChange ();
			if (change != null) {
				RectTransform rectTransform = transform as RectTransform;
				change (rectTransform.sizeDelta);
			}
		}
	}
}
