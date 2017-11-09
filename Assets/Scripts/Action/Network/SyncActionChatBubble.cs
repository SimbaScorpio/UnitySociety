using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DesignSociety
{
	public class SyncActionChatBubble : SyncActionBubble
	{
		protected override void OnSetContent (Transform bubble, string content)
		{
			bubble.GetComponentInChildren<TextMeshPro> ().SetText (content);
		}
	}

	public class SyncActionErrorBubble : SyncActionBubble
	{
		protected override void OnSetContent (Transform bubble, string content)
		{
			bubble.GetComponentInChildren<TextMeshPro> ().SetText (content);
		}
	}

	public class SyncActionIconBubble : SyncActionBubble
	{
		protected override void OnSetContent (Transform bubble, string content)
		{
			Texture2D texture = MaterialCollection.GetInstance ().GetTexture (content, Global.IconPath);
			if (texture != null) {
				Rect size = new Rect (0, 0, texture.width, texture.height);
				Sprite sprite = Sprite.Create (texture, size, new Vector2 (0.5f, 0f));
				bubble.GetComponentInChildren<SpriteRenderer> ().sprite = sprite;
			}
		}
	}

	public class SyncActionScreenBubble : SyncActionBubble
	{
		protected override void OnSetContent (Transform bubble, string content)
		{
			Texture2D texture = MaterialCollection.GetInstance ().GetTexture (content, Global.DashboardPath);
			if (texture != null) {
				Rect size = new Rect (0, 0, texture.width, texture.height);
				Sprite sprite = Sprite.Create (texture, size, new Vector2 (0.5f, 0.5f));
				bubble.GetComponentInChildren<SpriteRenderer> ().sprite = sprite;
			}
		}
	}

	public class SyncActionKeywordBubble : SyncActionBubble
	{
		protected override void OnSetContent (Transform bubble, string content)
		{
			bubble.GetComponentInChildren<TextMeshPro> ().SetText (content);

			int num = content.Length;
			bubble.GetComponentsInChildren<SpriteRenderer> () [0].size = new Vector2 (num * 0.5f + 1f, 1.44f);
			bubble.GetComponentsInChildren<SpriteRenderer> () [1].transform.localPosition = new Vector3 ((num * 0.5f + 1f) * 0.8f - 0.5f, 2f, 0);
		}

		protected override void Update ()
		{
			base.Update ();
		}
	}
}