using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesignSociety
{
	public class AvatarSnapManager : MonoBehaviour
	{
		// 摄影棚预设
		public GameObject prefab;
		// 初始第一个元素的X偏移
		public float offsetStart = -500f;
		// 元素间的间隔
		public float offset = -5f;

		public string RenderTexturePath = "Materials/Avatar";
		public string PlayerPath = "Avatar";

		// 关联人物名称和正在使用的摄影棚
		private Dictionary<string, GameObject> nameToAvatar = new Dictionary<string,GameObject> ();
		// 工厂列表（重复利用）
		private List<GameObject> factory = new List<GameObject> ();
		// 已实例化总数
		private int totalCount = 0;


		private static AvatarSnapManager instance;

		public static AvatarSnapManager GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		RenderTexture CreateAvatar (CharacterData cd)
		{
			GameObject snap = (GameObject)Instantiate (prefab);
			snap.transform.position = new Vector3 (offsetStart + totalCount * offset, 0, 0);
			nameToAvatar [cd.name] = snap;

			// 设置人物
			GameObject man = snap.transform.Find (PlayerPath).gameObject;
			CharacterGenerator.GetInstance ().Generate (man, cd);
			man.GetComponent<Animator> ().Play (StandActionName.stand_nod.ToString (), 0, 0f);

			// 设置投影
			Camera cs = snap.GetComponentInChildren<Camera> ();
			RenderTexture rt = (RenderTexture)Instantiate (Resources.Load (RenderTexturePath));
			cs.targetTexture = rt;

			totalCount += 1;

			return rt;
		}

		public RenderTexture GetAvatar (CharacterData cd)
		{
			// 如果已经在显示，那么只需返回当前投影
			if (nameToAvatar.ContainsKey (cd.name)) {
				return nameToAvatar [cd.name].GetComponentInChildren<Camera> ().targetTexture;
			}
			// 如果工厂不足，则创建新元素，否则利用工厂元素
			if (factory.Count == 0) {
				return CreateAvatar (cd);
			} else {
				GameObject snap = factory [0];
				factory.RemoveAt (0);
				snap.SetActive (true);

				nameToAvatar [cd.name] = snap;

				GameObject man = snap.transform.Find (PlayerPath).gameObject;
				CharacterGenerator.GetInstance ().Generate (man, cd);
				man.GetComponent<Animator> ().Play (StandActionName.stand_nod.ToString (), 0, 0f);

				Camera cs = snap.GetComponentInChildren<Camera> ();
				return cs.targetTexture;
			}
		}

		public void RemoveAvatar (string name)
		{
			if (nameToAvatar.ContainsKey (name)) {
				GameObject snap = nameToAvatar [name];
				snap.SetActive (false);
				nameToAvatar.Remove (name);
				factory.Add (snap);
			}
		}
	}
}