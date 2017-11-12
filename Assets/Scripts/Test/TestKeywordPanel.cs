using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignSociety;

public class TestKeywordPanel : MonoBehaviour
{
	string Generate ()
	{
		string s = "";
		int max = Random.Range (0, 1000);
		for (int i = 0; i < max; ++i) {
			s += "统计图表";
		}
		return s;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			Keyword keyword = new Keyword ();
			keyword.keyword = "统计图表";
			keyword.description = Generate ();
			UIKeywordPanel.GetInstance ().OpenPanel (keyword);
		}
	}
}
