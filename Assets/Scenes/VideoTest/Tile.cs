using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

	public GameObject tile;
	public int number;

	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < number; ++i) {
			Instantiate (tile);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
