using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class NetworkPlayer : MonoBehaviour
	{
		void Start ()
		{
			FileManager.GetInstance ().LoadKeywordData ();
		}
	}
}