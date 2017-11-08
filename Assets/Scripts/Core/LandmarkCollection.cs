using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace DesignSociety
{
	public class LandmarkCollection : MonoBehaviour
	{
		// Read-only
		public List<LandmarkList> landmarkPartList = new List<LandmarkList> ();
		public List<Landmark> landmarkList = new List<Landmark> ();

		// won't change since first read
		private Dictionary<string, Landmark> landmarks;

		private List<Landmark> labeledLandmarks;

		private static LandmarkCollection instance;

		public static LandmarkCollection GetInstance ()
		{
			return instance;
		}

		void Awake ()
		{
			instance = this;
			landmarks = new Dictionary<string, Landmark> ();
			labeledLandmarks = new List<Landmark> ();
		}

		public void AddLandmarkPart (LandmarkList part)
		{
			if (!landmarkPartList.Contains (part))
				landmarkPartList.Add (part);
		}

		public void ClearLandmarkPart ()
		{
			landmarkPartList.Clear ();
		}

		public void Initialize ()
		{
			landmarks.Clear ();
			landmarkList.Clear ();
			foreach (LandmarkList part in landmarkPartList) {
				foreach (Landmark lm in part.landmarkList) {
					if (landmarks.ContainsKey (lm.m_name)) {
						Log.warn ("初始化坐标 【" + lm.m_name + "】 警告: 重复的命名");
					}
					landmarks [lm.m_name] = lm;
					landmarkList.Add (lm);
				}
			}
			if (UILandmarksManager.GetInstance ())
				UILandmarksManager.GetInstance ().Initialize (landmarkList.ToArray ());
		}


		public Landmark Get (string name)
		{
			if (string.IsNullOrEmpty (name)) {
				Log.error ("尝试获取一个空地点，请给出地点名称");
				return null;
			}
			if (landmarks.ContainsKey (name)) {
				return landmarks [name];
			}
			Log.error ("尝试获取一个不存在的地点 【" + name + "】，请确保该地点命名正确");
			return null;
		}


		Landmark ValidLandmark (Landmark mark)
		{
			NNInfo info = AstarPath.active.GetNearest (mark.position);
			Vector3 pos = info.clampedPosition;
			Landmark ans = new Landmark ();
			ans.m_name = mark.m_name;
			ans.m_label = mark.m_label;
			ans.m_data [0] = pos.x;
			ans.m_data [1] = pos.y;
			ans.m_data [2] = pos.z;
			ans.m_data [3] = mark.m_data [3];
			return ans;
		}


		public Landmark GetNearestObject (Vector3 position, string objectName)
		{
			if (string.IsNullOrEmpty (objectName)) {
				Log.error ("Try to get a null object's location ");
				return null;
			}
			labeledLandmarks.Clear ();
			foreach (string name in landmarks.Keys) {
				if (landmarks [name].m_label == objectName)
					labeledLandmarks.Add (landmarks [name]);
			}
			if (labeledLandmarks.Count == 0) {
				Log.error ("There is no such object location label [" + objectName + "]");
				return null;
			}
			Landmark closest = labeledLandmarks [0];
			float minDistance = float.MaxValue;
			for (int i = 0; i < labeledLandmarks.Count; ++i) {
				float distance = Vector3.Distance (position, labeledLandmarks [i].position);
				if (distance < minDistance) {
					minDistance = distance;
					closest = labeledLandmarks [i];
				}
			}
			return closest;
		}
	}
}