using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignSociety
{
	[System.Serializable]
	public class Landmark
	{
		public string m_name;
		public string m_label;
		public float[] m_data;
		public string[] m_bubble_content;
		public int m_bubble_direction;

		public Vector3 position { 
			get {
				return new Vector3 (m_data [0], m_data [1], m_data [2]);
			}
		}

		public Quaternion rotation {
			get {
				return Quaternion.Euler (new Vector3 (0, m_data [3], 0));
			}
		}

		public Landmark Copy ()
		{
			Landmark temp = new Landmark ();
			temp.m_label = m_label;
			temp.m_name = m_name;
			temp.m_data = new float[m_data.Length];
			for (int i = 0; i < m_data.Length; ++i) {
				temp.m_data [i] = m_data [i];
			}
			return temp;
		}

		public Landmark ()
		{
			m_name = m_label = "";
			m_data = new float[4]{ 0, 0, 0, 0 };
		}

		public void Set (Vector3 position)
		{
			m_data [0] = position.x;
			m_data [1] = position.y;
			m_data [2] = position.z;
		}
	}
}