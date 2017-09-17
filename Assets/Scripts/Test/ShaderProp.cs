//using UnityEngine;
//using System;
//using UnityEditor;
//using System.Collections.Generic;
//
//[ExecuteInEditMode]
//public class ShaderProp : MonoBehaviour
//{
//	void OnEnable ()
//	{
//		Shader s = GetComponent<MeshRenderer> ().sharedMaterial.shader;
//		List<String> props = new List<String> ();
//		for (int i = 0; i < ShaderUtil.GetPropertyCount (s); ++i)
//			props.Add (ShaderUtil.GetPropertyName (s, i));
//		Debug.Log (String.Join ("\n", props.ToArray ()));
//	}
//}
