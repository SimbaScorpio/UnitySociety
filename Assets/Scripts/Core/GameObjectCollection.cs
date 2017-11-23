using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class GameObjectCollection : MonoBehaviour
	{
		Dictionary<string, GameObject[]> tagToGameObject = new Dictionary<string, GameObject[]> ();


		private static GameObjectCollection instance;

		public static GameObjectCollection GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		public GameObject[] Get (string tag)
		{
			if (tagToGameObject.ContainsKey (tag))
				return tagToGameObject [tag];
			tagToGameObject [tag] = GameObject.FindGameObjectsWithTag (tag);
			return tagToGameObject [tag];
		}

		public void Clear (string tag)
		{
			tagToGameObject.Remove (tag);
		}
			
	}
}