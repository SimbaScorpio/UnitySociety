using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DesignSociety;

public class UIActionTest : MonoBehaviour, IActionCompleted
{
	public ActionDealer dealer;
	public Dropdown dp;
	private bool valid = true;

	void Start ()
	{
		List<string> options = new List<string> (ActionName.validName);
		dp.AddOptions (options);
		dp.onValueChanged.AddListener (delegate {
			OnValueChanged ();
		});
	}

	public void OnValueChanged ()
	{
		if (!valid)
			return;
		valid = false;
		string name = dp.captionText.text;
		dealer.ApproachAction (name, this);
	}

	public void OnActionCompleted (Action ac)
	{
		valid = true;
	}
}
