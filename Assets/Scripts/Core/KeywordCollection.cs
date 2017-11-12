using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class KeywordCollection : MonoBehaviour
	{
		public KeywordList keywordList;

		private Dictionary<string, Keyword> nameToKeyword = new Dictionary<string, Keyword> ();

		private static KeywordCollection instance;

		public static KeywordCollection GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		public Keyword Get (string name)
		{
			if (nameToKeyword.ContainsKey (name))
				return nameToKeyword [name];
			Keyword temp;
			for (int i = 0; i < keywordList.keyword_info.Length; ++i) {
				temp = keywordList.keyword_info [i];
				if (temp.keyword == name) {
					nameToKeyword [name] = temp;
					return temp;
				}
			}
			return null;
		}
	}
}