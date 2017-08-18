using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemLog : MonoBehaviour
{
	public ScrollRect scrollRect;
	public RectTransform content;
	public Text textPref;

	private List<string> messages;
	private float maxWidth = 0;

	private static UISystemLog instance;

	public static UISystemLog GetInstance ()
	{
		return instance;
	}

	void Awake ()
	{
		instance = this;
		messages = new List<string> ();
	}

	public void AddMessage (string message)
	{
		if (string.IsNullOrEmpty (message))
			return;
		
		Text text = Instantiate (textPref) as Text;
		text.rectTransform.SetParent (content);
		text.rectTransform.localScale = Vector3.one;
		text.rectTransform.localPosition = new Vector3 (2, -messages.Count * text.rectTransform.sizeDelta.y, 0);
		text.text = message;
		messages.Add (message);

		ContentSizeFitter fitter = text.GetComponent<ContentSizeFitter> ();
		fitter.CallBack (delegate(Vector2 size) {
			if (size.x > maxWidth)
				maxWidth = size.x;
			content.sizeDelta = new Vector2 (maxWidth + 2, messages.Count * text.rectTransform.sizeDelta.y);
			if (scrollRect.verticalScrollbar.value < 0.01f)
				scrollRect.verticalNormalizedPosition = 0f;
		});
	}

	//	float CalculateTextWidth (string message)
	//	{
	//		Font font = textPref.font;
	//		font.RequestCharactersInTexture (message, textPref.fontSize, textPref.fontStyle);
	//		CharacterInfo characterInfo;
	//		float width = 0.0f;
	//		foreach (char character in message) {
	//			font.GetCharacterInfo (character, out characterInfo, textPref.fontSize);
	//			width += characterInfo.advance;
	//		}
	//		return width;
	//	}
}
