using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene : MonoBehaviour
{
	public GameObject Player;

	public int Number = 1;

	void Start()
	{
		for (int i = 0; i < Number; ++i)
		{
			Vector3 pos = NavMeshManager.GetInstance().GetRandomPoint();
			GameObject obj = Instantiate(Player, pos, Quaternion.identity) as GameObject;
			obj.name = "player" + i;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}

	public void Reset()
	{
		
	}
}
