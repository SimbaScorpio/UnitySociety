using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using DesignSociety;

public class RandomWalk : MonoBehaviour
{
	public float size = 2;

	AstarPath astar;

	void Start ()
	{
		astar = FindObjectOfType<AstarPath> ();
	}

	Landmark GetRandomPlace ()
	{
		float x, z;
		Vector3 pos, near;
		NNInfo info;
		do {
			x = Random.Range (-size / 2, size / 2);
			z = Random.Range (-size / 2, size / 2);
			pos = new Vector3 (x, 0, z);
			info = astar.graphs [0].GetNearest (pos);
			near = info.clampedPosition;
		} while (Vector3.Distance (near, pos) > 0.1f);

		Landmark mark = new Landmark ();
		mark.m_data [0] = x;
		mark.m_data [2] = z;
		return mark;
	}

	void Walk ()
	{
		Landmark mark = GetRandomPlace ();
		ActionWalkTo ac = gameObject.AddComponent <ActionWalkTo> ();
		ac.Setting (gameObject, mark, null);
	}

	void Update ()
	{
		if (GetComponent<ActionWalkTo> () == null) {
			Walk ();
		} 
	}

	IEnumerator Wait ()
	{
		yield return new WaitForSeconds (2);
		Walk ();
	}
}
