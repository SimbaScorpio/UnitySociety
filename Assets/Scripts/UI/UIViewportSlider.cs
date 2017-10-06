using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class UIViewportSlider : MonoBehaviour
	{
		public float deltaSize;

		private float startSize;

		void Start ()
		{
			startSize = Camera.main.orthographicSize;
		}

		public void OnSliderValueChanged (float value)
		{
			Camera.main.orthographicSize = startSize + deltaSize * (0.5f - value);
		}
	}
}
