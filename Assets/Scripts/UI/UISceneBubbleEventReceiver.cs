﻿using System.Collections;
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
			print ("event started");
			if (OnBubbleStartedHandler != null)
				OnBubbleStartedHandler ();
		}

		public void OnBubbleFinished ()
		{
			print ("event finished");
			if (OnBubbleFinishedHandler != null)
				OnBubbleFinishedHandler ();
		}
	}
}