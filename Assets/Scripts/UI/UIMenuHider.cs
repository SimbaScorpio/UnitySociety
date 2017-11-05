using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class UIMenuHider : MonoBehaviour
	{
		private Canvas canvasScript;

		void Start ()
		{
			canvasScript = GetComponent<Canvas> ();
		}

		void Update ()
		{
			if (Input.GetKeyDown (KeyCode.Escape)) {
				if (canvasScript != null) {
					canvasScript.enabled = !canvasScript.enabled;
				}
			}
		}
	}
}
