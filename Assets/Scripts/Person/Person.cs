using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Person : MonoBehaviour, ActionCompleted
{
	public virtual void OnActionCompleted (Action ac)
	{
	}
}