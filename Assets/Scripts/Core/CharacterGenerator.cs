using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	public class CharacterGenerator : MonoBehaviour
	{
		private string hairPath = "Prefabs/Hair/";
		private string bodyPath = "Prefabs/Body/";

		private static CharacterGenerator instance;

		public static CharacterGenerator GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
		}

		public void Generate (GameObject model, CharacterData data)
		{
			// 发型
			string hairName = GetHairName (data.hair);
			Transform hair = model.transform.Find ("hair");
			Transform[] children = hair.GetComponentsInChildren<Transform> ();
			for (int i = 1; i < children.Length; ++i)
				Destroy (children [i].gameObject);
			Instantiate (Resources.Load (hairPath + hairName), hair);

			// 体型
			string bodyName = GetBodyName (data.body_type);
			GameObject obj = (GameObject)Resources.Load (bodyPath + bodyName);
			Mesh mesh = obj.GetComponent<SkinnedMeshRenderer> ().sharedMesh;
			model.GetComponentInChildren<SkinnedMeshRenderer> ().sharedMesh = mesh;

			// 服装贴图
			Material clothMat = MaterialCollection.GetInstance ().GetMaterial (data.clothing);
			model.transform.Find ("mesh").GetComponent<Renderer> ().material = clothMat;
		}

		string GetHairName (int style)
		{
			if (style == 1) {
				return "hair_man_0";
			} else if (style == 2) {
				return "hair_woman_0";
			} else {
				return "hair_man_0";
			}
		}

		string GetBodyName (string type)
		{
			if (type == "男瘦") {
				return "body_man_0";
			} else if (type == "男胖") {
				return "body_man_1";
			} else if (type == "女裙") {
				return "body_woman_0";
			} else if (type == "女裤") {
				return "body_woman_1";
			} else {
				return "body_man_0";
			}
		}
	}
}