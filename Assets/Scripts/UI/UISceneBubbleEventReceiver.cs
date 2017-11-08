using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class UISceneBubbleEventReceiver : MonoBehaviour
	{
		public delegate void OnBubbleStartedEvent ();

		public event OnBubbleStartedEvent OnBubbleStartedHandler;

		public delegate void OnBubbleFinishedEvent ();

		public event OnBubbleFinishedEvent OnBubbleFinishedHandler;

		public void OnBubbleStarted ()
		{
			if (OnBubbleStartedHandler != null)
				OnBubbleStartedHandler ();
		}

		public void OnBubbleFinished ()
		{
			if (OnBubbleFinishedHandler != null)
				OnBubbleFinishedHandler ();
		}
	}
}
