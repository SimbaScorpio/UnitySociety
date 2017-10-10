using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace DesignSociety
{
	public class NetworkPlayerDrag : NetworkBehaviour
	{
		public float dragThreshold = 10f;
		public float dropHeight = 1f;
		public float playerHeight = 1.75f;

		private Vector3 lastPressPos;
		private float rayDistance = 300f;
		private bool isCheckBegin;
		private bool isDragBegin;

		private CameraPerspectiveEditor cameraEditor;

		void Start ()
		{
			if (!isLocalPlayer)
				return;
			Camera.main.GetComponent<CameraFollower> ().Character = this.gameObject;
			cameraEditor = Camera.main.GetComponent<CameraPerspectiveEditor> ();
		}

		void Update ()
		{
			CheckDrag ();
		}

		void CheckDrag ()
		{
			if (!isLocalPlayer)
				return;
			
			if (EventSystem.current.IsPointerOverGameObject ())
				return;
			
			if (Input.GetMouseButtonDown (0)) {
				lastPressPos = Input.mousePosition;
				if (IsPressingPlayer ()) {
					isCheckBegin = true;
				}
			} else if (Input.GetMouseButtonUp (0)) {
				isCheckBegin = false;
				if (isDragBegin) {
					isDragBegin = false;
					OnDragEnd ();
					//CmdOnDragEnd ();
				}
			} else if (Input.GetMouseButton (0)) {
				if (isCheckBegin) {
					float distance = Vector3.Distance (Input.mousePosition, lastPressPos);
					if (distance > dragThreshold) {
						isDragBegin = true;
						isCheckBegin = false;
						OnDragBegin ();
						//CmdOnDragBegin ();
					}
				} else if (isDragBegin) {
					OnDragging ();
				}
			}
		}

		bool IsPressingPlayer ()
		{
			Ray ray = cameraEditor.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, float.MaxValue)) {
				if (hit.collider.gameObject == this.gameObject) {
					return true;
				}
			}
			return false;
		}

		void OnDragBegin ()
		{
			
		}

		void OnDragEnd ()
		{
			
		}


		void OnDragging ()
		{
			Vector3 a1 = cameraEditor.WorldToScreenPoint (Vector3.zero);
			Vector3 a2 = cameraEditor.WorldToScreenPoint (Vector3.zero + Vector3.up);
			float worldNormYToScreenY = a2.y - a1.y;

			Vector3 landingPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y - dropHeight * worldNormYToScreenY, Input.mousePosition.z);
			Ray ray = cameraEditor.ScreenPointToRay (landingPos);

			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, rayDistance)) {
				if (hit.collider.tag != "Player") {
					//GameObject.Find ("Fuck").transform.position = hit.point;
					Vector3 targetPos = new Vector3 (hit.point.x, hit.point.y + dropHeight - playerHeight / 2, hit.point.z);
					transform.position = targetPos;
				}
			}
		}


		#region Network

		//		// 客户端通知服务器对象，服务器通知其他客户端，注意ServerOnly的服务器本身不接受Rpc指令
		//		[Command]
		//		void CmdOnDragBegin ()
		//		{
		//			RpcOnDragBegin ();
		//			OnDragBegin ();
		//		}
		//
		//		// 客户端通知服务器对象，服务器通知其他客户端，注意ServerOnly的服务器本身不接受Rpc指令
		//		[Command]
		//		void CmdOnDragEnd ()
		//		{
		//			RpcOnDragEnd ();
		//			OnDragEnd ();
		//		}
		//
		//		[ClientRpc]
		//		void RpcOnDragBegin ()
		//		{
		//			if (!isLocalPlayer) {
		//				OnDragBegin ();
		//			}
		//		}
		//
		//		[ClientRpc]
		//		void RpcOnDragEnd ()
		//		{
		//			if (!isLocalPlayer) {
		//				OnDragEnd ();
		//			}
		//		}

		#endregion
	}
}