﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
	// 气泡预设
	public GameObject BubblePref;
	// 气泡悬空高度比例
	public float BubbleAboveRatio = 1.2f;

	/*
	// 气泡宽度
	public float BubbleWidth = 100.0f;
	// 气泡高度
	public float BubbleHeight = 70.0f;
	// 气泡贴图
	private Texture2D bubbleTexture;
	// 气泡样式
	private GUIStyle bubbleStyle;
	*/

	// 记录正在交谈的人
	private List<GameObject> speakingPeople;
	// 记录每个人的气泡内容
	private List<string> speakingContent;
	// 记录每个人的气泡时间
	private List<float> speakingTime;
	// 记录每个人的气泡
	private List<GameObject> speakingBubbles;
	// 气泡工厂（回收利用）
	private List<GameObject> factoryBubbles;

	// 气泡画布
	private GameObject canvas;

	// 该类为单例对象
	private static ChatManager instance;


	void Awake()
	{
		instance = this;

		speakingPeople = new List<GameObject>();
		speakingContent = new List<string>();
		speakingTime = new List<float>();
		speakingBubbles = new List<GameObject>();
		factoryBubbles = new List<GameObject>();

		canvas = GameObject.Find("Canvas");
	}


	public static ChatManager Instance
	{
		get
		{	// 单例模式
			return instance;
		}
	}


	// 气泡计时
	void Update()
	{
		int count = speakingPeople.Count;
		for (int i = 0; i < count; ++i)
		{
			if (speakingTime[i] < 0.0f)
			{
				this.Silence(speakingPeople[i--]);
				count -= 1;
			}
			else
			{
				speakingTime[i] -= Time.deltaTime;

				// 提前播放气泡消退动画
				if (speakingTime[i] < 0.2f)
				{
					GameObject bubble = speakingBubbles[i];
					bubble.GetComponent<Animator>().SetBool("IsPoping", false);
				}
			}
		}
	}


	// 气泡绘制
	void OnGUI()
	{
		int count = speakingPeople.Count;
		for (int i = 0; i < count; ++i)
		{
			GameObject person = speakingPeople[i];
			string words = speakingContent[i];

			// 气泡在3D坐标中的位置
			Vector3 size = this.GetBoundsSize(person);
			Vector3 worldPosition = person.transform.position + new Vector3(0, size.y * BubbleAboveRatio, 0);

			// 3D坐标映射为屏幕坐标
			Vector2 position = Camera.main.WorldToScreenPoint(worldPosition);
			position = new Vector2(position.x, position.y);

			// 方法一：气泡预设组件跟随人物坐标
			GameObject bubble = speakingBubbles[i];
			bubble.transform.position = position;

			// 方法二： 动态绘制气泡
			/*
			 position = new Vector2(position.x, Screen.height - position.y);
			Rect rect = new Rect(
				            position.x - BubbleWidth / 2,
				            position.y - BubbleHeight / 2,
				            BubbleWidth,
				            BubbleHeight
			            );
			this.GUIDrawBubble(rect, words, Color.white);
			*/

		}
	}


	// 获取人物体积
	Vector3 GetBoundsSize(GameObject obj)
	{
		/*
		MeshFilter meshFilter = obj.GetComponentInChildren<MeshFilter>();
		if (!meshFilter)
		{
			return Vector3.zero;
		}
		Vector3 origin = meshFilter.mesh.bounds.size;
		*/

		/*
		SkinnedMeshRenderer renderer = obj.GetComponentInChildren<SkinnedMeshRenderer>();
		if (!renderer)
		{
			return Vector3.zero;
		}
		Vector3 origin = renderer.bounds.size;
		*/

		Collider collider = obj.GetComponent<Collider>();
		if (!collider)
		{
			return Vector3.zero;
		}
		Vector3 origin = collider.bounds.size;

		Vector3 size = new Vector3(origin.x, origin.y, origin.z);
		return size;
	} 


	/*
	// 绘制气泡（纯色背景）
	void GUIDrawBubble(Rect position, string words, Color color)
	{
		if (bubbleTexture == null)
		{
			bubbleTexture = new Texture2D(1, 1);
		}
		if (bubbleStyle == null)
		{
			bubbleStyle = new GUIStyle();
		}
		bubbleTexture.SetPixel(0, 0, color);
		bubbleTexture.Apply();
		bubbleStyle.normal.background = bubbleTexture;
		bubbleStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Box(position, words, bubbleStyle);
	}
	*/


	// 气泡工厂
	GameObject createBubble()
	{
		if (factoryBubbles.Count > 0)
		{
			GameObject bubble = factoryBubbles[0];
			factoryBubbles.RemoveAt(0);
			return bubble;
		}
		else
		{
			GameObject bubble = (GameObject)Instantiate(BubblePref, canvas.transform);
			return bubble;
		}
	}


	// 开启对话气泡
	public void Speak(GameObject person, string words, float time)
	{
		int index = speakingPeople.IndexOf(person);
		// 加上气泡弹出动画的时间
		time += 0.2f;

		// 如果已经在对话，替换气泡
		if (index >= 0)
		{
			print(person + "already speaking!");
			speakingContent[index] = words;
			speakingTime[index] = time;
			GameObject bubble = speakingBubbles[index];
			Text textUI = bubble.GetComponentInChildren<Text>();
			textUI.text = words;
		}
		// 如果刚开始对话，添加气泡
		else
		{
			speakingPeople.Add(person);
			speakingContent.Add(words);
			speakingTime.Add(time);

			// 方法一：利用UI预设组件的Text组件
			GameObject bubble = createBubble();
			bubble.GetComponent<Animator>().SetBool("IsPoping", true);
			Text textUI = bubble.GetComponentInChildren<Text>();
			textUI.text = words;
			speakingBubbles.Add(bubble);
		}
	}


	// 关闭对话气泡（取消各队列的跟踪对象）
	void Silence(GameObject person)
	{
		int index = speakingPeople.IndexOf(person);
		if (index >= 0)
		{
			// 回收气泡
			GameObject bubble = speakingBubbles[index];
			factoryBubbles.Add(bubble);
			// 取消队列记录
			speakingPeople.RemoveAt(index);
			speakingContent.RemoveAt(index);
			speakingTime.RemoveAt(index);
			speakingBubbles.RemoveAt(index);
		}
	}
}
