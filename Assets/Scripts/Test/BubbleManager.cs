﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleManager : MonoBehaviour
{
	// 气泡预设
	public GameObject CanvasPref;
	// 气泡悬空高度比例
	public float BubbleAboveRatio = 1.2f;

	private List<GameObject> followers = new List<GameObject>();
	private List<GameObject> canvases = new List<GameObject>();
	private List<string> contents = new List<string>();
	private List<float> durations = new List<float>();

	private List<GameObject> factoryCanvas = new List<GameObject>();

	private static BubbleManager instance;

	public static BubbleManager GetInstance()
	{
		return instance;
	}


	void Awake()
	{
		instance = this;
	}


	void Update()
	{
		for (int i = 0; i < followers.Count; ++i)
		{
			if (durations[i] < 0)
			{
				this.Silence(followers[i--]);
			}
			else
			{
				durations[i] -= Time.deltaTime;
				GameObject canvas = canvases[i];
				if (durations[i] <= 0.2f)
				{
					canvas.GetComponent<Animator>().SetBool("IsPoping", false);
				}
				canvas.transform.LookAt(Camera.main.transform.position);
				canvas.transform.Rotate(new Vector3(0, 180, 0));
			}
		}
	}


	// 获取人物体积
	Vector3 GetBoundsSize(GameObject obj)
	{
		Collider collider = obj.GetComponent<Collider>();
		if (!collider)
			return Vector3.zero;
		Vector3 origin = collider.bounds.size;
		Vector3 size = new Vector3(origin.x, origin.y, origin.z);
		return size;
	} 


	// 获取气泡
	GameObject GiveMeACanvas()
	{
		if (factoryCanvas.Count > 0)
		{
			GameObject canvas = factoryCanvas[0];
			factoryCanvas.RemoveAt(0);
			return canvas;
		}
		else
		{
			GameObject canvas = Instantiate(CanvasPref) as GameObject;
			return canvas;
		}
	}


	public void Speak(GameObject follower, string content, float duration)
	{
		GameObject canvas = this.GiveMeACanvas();

		followers.Add(follower);
		canvases.Add(canvas);
		contents.Add(content);
		durations.Add(duration + 0.2f);

		canvas.transform.SetParent(follower.transform);
		Vector3 size = this.GetBoundsSize(follower);
		Vector3 localPosition = new Vector3(0, size.y * BubbleAboveRatio, 0);
		canvas.transform.localPosition = localPosition;

		canvas.GetComponentInChildren<Text>().text = content;
		canvas.GetComponent<Animator>().SetBool("IsPoping", true);

	}


	void Silence(GameObject follower)
	{
		int index = followers.IndexOf(follower);
		if (index >= 0 && index < followers.Count)
		{
			GameObject canvas = canvases[index];
			factoryCanvas.Add(canvas);
			followers.RemoveAt(index);
			canvases.RemoveAt(index);
			contents.RemoveAt(index);
			durations.RemoveAt(index);
		}
	}
}
