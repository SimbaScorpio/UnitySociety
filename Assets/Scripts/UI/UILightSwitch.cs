using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class UILightSwitch : MonoBehaviour
	{
		public void SwitchLight ()
		{
			Light lightObj = FindObjectOfType<Light> ();
			if (lightObj != null) {
				lightObj.enabled = !lightObj.enabled;
			}
		}
	}
}