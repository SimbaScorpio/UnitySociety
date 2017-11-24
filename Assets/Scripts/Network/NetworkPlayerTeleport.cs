using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Pathfinding;

namespace DesignSociety
{
	public class NetworkPlayerTeleport : NetworkBehaviour
	{
		// smallest camera size
		public float smallSize = 10f;
		// largest camera size
		public float largeSize = 20.2f;
		// time interval between mouse down and up for click event
		public float clickPressThreshold = 0.3f;
		// smallest time interval between mouse down and up for drag event
		public float dragPressThreshold = 2f;
		// lerp rate for zooming
		public float lerpRate = 20f;
		// minimum drag distance to start dropping
		public float dragDistanceThreshold = 50f;

		public float dropHeight = 1.5f;
		public float playerHeight = 1.75f;
		public float gravity = -9.82f;

		public GameObject ghostPref;
		public GameObject ghost;
		public float ghostZ = 200f;

		private float velocity;

		private float pressCount = 0f;
		private bool canDragDrop;
		private Vector2 startMousePosition;
		private Vector2 lastMousePosition;
		private Vector3 mouseLandingPos;
		private Vector3 scrollMapStartTarget;
		private Vector2 ghostFloatOffset;

		private bool cameraValidX, cameraValidY;

		// {0: 正常, 1: 大地图拖拽, 2: 小地图缩放中, 3: 小地图拖拽, 4: 大地图滚动, 5: 小地图滚动}
		private int state = 0;

		private CameraPerspectiveEditor cameraEditor;
		private CameraFollower cameraFollower;
		private Animator ghostAnim;

		private NetworkPlayerAction playerAction;
		private NetworkActionDealer ad;

		void Start ()
		{
			if (isLocalPlayer) {
				playerAction = GetComponent<NetworkPlayerAction> ();
				ad = GetComponent<NetworkActionDealer> ();

				cameraEditor = Camera.main.GetComponent<CameraPerspectiveEditor> ();
				cameraFollower = Camera.main.GetComponent<CameraFollower> ();
				cameraFollower.target = this.gameObject;
				Camera.main.orthographicSize = smallSize;

				ghost = Instantiate (ghostPref) as GameObject;
				ghost.SetActive (false);
				ghostAnim = ghost.GetComponent<Animator> ();
			}
		}

		void Update ()
		{
			if (!isLocalPlayer)
				return;
			if (EventSystem.current.IsPointerOverGameObject ())
				return;
			if (Input.GetMouseButtonDown (0)) {
				StartCoroutine (CountPressTime ());
				CheckPressDownSpecial ();
			}
			if (Input.GetMouseButtonUp (0)) {
				StopAllCoroutines ();
				CheckPressUpSpecial ();
			}
			if (Input.GetMouseButton (0)) {
				if (state == 1 || state == 3) { 	// drag and drop
					DealWithDrag ();
				} else if (state == 4 || state == 5) { 	// scroll and click drop
					DealWithScroll ();
				}
			}
		}

		// press count (trigger drag event)
		IEnumerator CountPressTime ()
		{
			pressCount = 0;
			canDragDrop = false;
			startMousePosition = Input.mousePosition;
			lastMousePosition = Input.mousePosition;
			bool alwaysPressing = true;
			while (true) {
				pressCount += Time.deltaTime;
				yield return null;
				if (state == 0) {
					if (alwaysPressing && (!IsPressing (this.gameObject) || ad.IsPlaying ())) {
						alwaysPressing = false;
					}
					if (alwaysPressing && pressCount > dragPressThreshold) {
						OnTeleportBegin ();
						pressCount = 0;
						state = 1;
						break;
					}
				}
			}
		}

		// hide menu and scale to world view
		void OnTeleportBegin ()
		{
			SwitchPlayerToGhost ();
			UIInformationMenu.GetInstance ().Hide ();
			UIAppearanceMenu.GetInstance ().Hide ();
			StartCoroutine (LerpView (true));
			ghostAnim.Play ("drag_float", 0, 0);
			playerAction.ApplyAction ("drag_float");
		}

		// scale to normal view, waiting drop command
		void OnTeleportReadyEnd ()
		{
			StartCoroutine (LerpView (false));
		}

		// drop
		void OnTeleportEnd ()
		{
			SwitchGhostToPlayer ();
			UIInformationMenu.GetInstance ().Show ();
			UIAppearanceMenu.GetInstance ().Show ();
			playerAction.ApplyAction ("walk_blend_tree");
		}

		// scroll view {true: large, false: normal}
		IEnumerator LerpView (bool flag)
		{
			float size = flag ? largeSize : smallSize;
			while (Mathf.Abs (Camera.main.orthographicSize - size) > 0.001f) {
				Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, size, Time.deltaTime * lerpRate);

				// ghost scale with camera size
				float ratio = Camera.main.orthographicSize / smallSize;
				ghost.transform.localScale = new Vector3 (ratio, ratio, ratio);

				// camera focus under mouse
				Vector2 o = MouseOnScreenOffset ();
				Vector3 target = RaycastPoint ();
				Camera.main.transform.position = cameraFollower.CalCameraPosition (target, o.x, o.y);
				if (state == 2)
					ghost.transform.position = CalGhostWithMousePos ();
				if (state == 5)
					ghost.transform.position = CalGhostWithScreenPos (ghostFloatOffset.x, ghostFloatOffset.y);

				yield return null;
			}

			if (state == 2)
				state = 3;
		}

		bool IsPressing (GameObject obj)
		{
			Ray ray = cameraEditor.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, float.MaxValue)) {
				if (hit.collider.gameObject == obj) {
					return true;
				}
			}
			return false;
		}

		Vector3 RaycastPoint ()
		{
			Ray ray = cameraEditor.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, float.MaxValue)) {
				return hit.point;
			}
			return Vector3.zero;
		}

		bool RaycastObstacle ()
		{
			Ray ray = cameraEditor.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, float.MaxValue)) {
				if (hit.collider.tag == "Player" || hit.collider.tag == "KeywordBubble" || hit.collider.tag == "Car")
					return true;
			}
			return false;
		}

		# region 方式一：拖拽人物

		void DealWithDrag ()
		{
			bool panning = CheckMapPan ();
			
			if (!canDragDrop) {
				float distance = Vector2.Distance (Input.mousePosition, startMousePosition);
				if (distance > dragDistanceThreshold)
					canDragDrop = true;
			} else if (state == 1 && !panning && CheckFocus ()) {
				state = 2;
				OnTeleportReadyEnd ();
				return;
			}

			CalRaycastLandingPos ();
			ghost.transform.position = CalGhostWithMousePos ();
		}

		void CalRaycastLandingPos ()
		{
			Vector3 a1 = cameraEditor.WorldToScreenPoint (Vector3.zero);
			Vector3 a2 = cameraEditor.WorldToScreenPoint (Vector3.zero + Vector3.up);
			float worldNormYToScreenY = a2.y - a1.y;

			Vector3 landingScreenPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y - dropHeight * worldNormYToScreenY, Input.mousePosition.z);
			Ray ray = cameraEditor.ScreenPointToRay (landingScreenPos);

			//Ray ray = cameraEditor.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 1000)) {
				if (hit.collider.tag != "Player") {
					mouseLandingPos = hit.point;
					//GameObject.Find ("CubeShadow").transform.position = hit.point;
				}
			}
		}

		Vector3 CalGhostWithMousePos ()
		{
			Vector2 o = MouseOnScreenOffset ();
			return CalGhostWithScreenPos (o.x, o.y);
		}

		Vector3 CalGhostWithScreenPos (float offsetX, float offsetY)
		{
			Vector3 ghostFeetPos = cameraFollower.CalObjectPosition (ghostZ, offsetX, offsetY);
			ghostFeetPos.y -= playerHeight / 2 * ghost.transform.localScale.y;
			return ghostFeetPos;
		}

		bool CheckFocus ()
		{
			float distance = Vector2.Distance (Input.mousePosition, lastMousePosition);
			if (distance < 1f) {
				pressCount += Time.deltaTime;
			} else {
				pressCount = 0;
			}
			lastMousePosition = Input.mousePosition;
			return pressCount > dragPressThreshold;
		}

		bool CheckMapPan ()
		{
			Vector2 o = MouseOnScreenOffset ();
			if (o.x < 0.2f)
				return ScrollPanCamera (true, 1f);
			if (o.x > 0.8f)
				return ScrollPanCamera (true, -1f);
			if (o.y < 0.2f)
				return ScrollPanCamera (false, -1f);
			if (o.y > 0.8f)
				return ScrollPanCamera (false, 1f);
			return false;
		}

		bool ScrollPanCamera (bool flag, float ratio)
		{
			ratio = ratio * Camera.main.orthographicSize / largeSize;
			Vector3 pos = Camera.main.transform.position;
			if (flag) {
				Vector2 vx = cameraFollower.GetValidXRange ();
				if (pos.x < vx.x && ratio <= 0)
					return false;
				if (pos.x > vx.y && ratio >= 0)
					return false;
				float temp = pos.x + ratio;
				if (pos.x < vx.x && temp > vx.x)
					pos.x = temp > vx.y ? vx.y : temp;
				else if (pos.x > vx.y && temp < vx.y)
					pos.x = temp < vx.x ? vx.x : temp;
				else
					pos.x = temp;
			} else {
				Vector2 vy = cameraFollower.GetValidYRange ();
				if (pos.y <= vy.x && ratio <= 0)
					return false;
				if (pos.y >= vy.y && ratio >= 0)
					return false;
				float temp = pos.y + ratio;
				if (pos.y <= vy.x && temp >= vy.x) {
					pos.y = temp > vy.y ? vy.y : temp;
				} else if (pos.y >= vy.y && temp <= vy.y) {
					pos.y = temp < vy.x ? vy.x : temp;
				} else {
					pos.y = temp;
				}
			}
			Camera.main.transform.position = pos;
			return true;
		}

		#endregion

		#region 方式二：滚动地图

		void DealWithScroll ()
		{
			Vector2 o = MouseOnScreenOffset ();
			Vector3 temp = cameraFollower.CalCameraPosition (scrollMapStartTarget, o.x, o.y);
			Camera.main.transform.position = cameraFollower.ClampPosition (temp);
			ghost.transform.position = CalGhostWithScreenPos (ghostFloatOffset.x, ghostFloatOffset.y);
		}

		#endregion



		void CheckPressDownSpecial ()
		{
			if (state == 4 || state == 5) {
				scrollMapStartTarget = RaycastPoint ();
			}
		}

		void CheckPressUpSpecial ()
		{
			if ((state == 0 || state == 4 || state == 5) && pressCount < clickPressThreshold)
				OnClick ();
			else {
				if (state == 1) {
					state = 4;
					ghostFloatOffset = MouseOnScreenOffset ();
				} else if (state == 2 || state == 3) {
					OnTeleportEnd ();
					StartCoroutine (Falling ());
					state = 0;
				}
			}
		}

		void OnClick ()
		{
			if (state == 0) {
				if (ad.IsWalking () && IsPressing (this.gameObject)) {
					ad.CallingStop ();
				} else {
					if (!RaycastObstacle ()) {
						if (ad.IsWalking () || !ad.IsPlaying ()) {
							mouseLandingPos = RaycastPoint ();
							playerAction.WalkTo (mouseLandingPos);
						} else {
							ad.CallingStop ();
						}
					}
				}
			} else if (state == 4) {
				state = 5;
				StartCoroutine (LerpView (false));
			} else if (state == 5) {
				state = 0;
				mouseLandingPos = RaycastPoint ();
				OnTeleportEnd ();
				StartCoroutine (Falling ());
			}
		}

		void SwitchPlayerToGhost ()
		{
			// set ghost position
			ghost.transform.position = CalGhostWithMousePos ();

			// hide player
			SetPlayerVisibility (false);
			// show ghost
			ghost.SetActive (true);
			cameraFollower.target = null;
		}

		void SwitchGhostToPlayer ()
		{
			// set player target position
			Vector3 landingPos = GetAvailableLandPos (mouseLandingPos);
			// set player drop position
			Vector3 dropingPos = new Vector3 (landingPos.x, landingPos.y + dropHeight - playerHeight / 2, landingPos.z);
			transform.position = dropingPos;
			transform.rotation = Quaternion.identity;
			mouseLandingPos = landingPos;

			// show player
			SetPlayerVisibility (true);
			// hide ghost
			ghost.SetActive (false);
			cameraFollower.target = this.gameObject;
		}

		private Collider m_collider;
		private NavmeshCut m_navCut;

		void SetPlayerVisibility (bool visible)
		{
			if (m_collider == null)
				m_collider = GetComponent<Collider> ();
			m_collider.enabled = visible;
			if (m_navCut == null)
				m_navCut = GetComponent<NavmeshCut> ();
			if (m_navCut != null)
				m_navCut.enabled = visible;
			transform.Find ("mesh").gameObject.SetActive (visible);
			transform.Find ("ears").gameObject.SetActive (visible);
			transform.Find ("hair").gameObject.SetActive (visible);
		}

		IEnumerator Falling ()
		{
			Vector3 position = transform.position;
			velocity = 0;
			while (position.y > mouseLandingPos.y) {
				velocity += Time.deltaTime * gravity;
				position.y += velocity * Time.deltaTime;
				transform.position = position;
				yield return null;
			}
			transform.position = mouseLandingPos;
		}

		Vector3 GetAvailableLandPos (Vector3 pos)
		{
			NNInfo info = AstarPath.active.GetNearest (pos);
			return info.clampedPosition;
		}

		Vector2 MouseOnScreenOffset ()
		{
			float offsetX = Input.mousePosition.x / Screen.width;
			float offsetY = Input.mousePosition.y / Screen.height;
			return new Vector2 (offsetX, offsetY);
		}
	}
}
