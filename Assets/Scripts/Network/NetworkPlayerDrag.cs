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
		public float dropHeight = 1.5f;
		public float playerHeight = 1.75f;
		public float gravity = -9.82f;

		private Vector3 lastPressPos;
		private float rayDistance = 300f;
		private bool isCheckBegin;
		private bool isDragBegin;

		private float velocity;

		private CameraPerspectiveEditor cameraEditor;

		private Animator anim;
		private NetworkAnimator netAnim;

		void Start ()
		{
			anim = GetComponent<Animator> ();
			if (isLocalPlayer) {
				Camera.main.GetComponent<CameraFollower> ().Character = this.gameObject;
				cameraEditor = Camera.main.GetComponent<CameraPerspectiveEditor> ();
				netAnim = GetComponent<NetworkAnimator> ();
				if (isServer) {
					for (int i = 0; i < anim.parameterCount; ++i)
						netAnim.SetParameterAutoSend (i, true);
				}
			}
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
					CmdOnDragEnd ();

				}
			} else if (Input.GetMouseButton (0)) {
				if (isCheckBegin) {
					float distance = Vector3.Distance (Input.mousePosition, lastPressPos);
					if (distance > dragThreshold) {
						isDragBegin = true;
						isCheckBegin = false;
						OnDragBegin ();
						CmdOnDragBegin ();

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
			velocity = 0;
			anim.Play ("drag_float", 0, 0);
		}

		void OnDragEnd ()
		{
			StartCoroutine (Falling ());
			anim.Play ("drag_drop", 0, 0);
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


		IEnumerator Falling ()
		{
			float lasty;
			Vector3 position;
			do {
				velocity += Time.deltaTime * gravity;
				position = transform.position;
				lasty = position.y;
				position.y += velocity * Time.deltaTime;
				transform.position = position;
				yield return null;
			} while(!RaycastPosition (ref position, lasty));
		}

		bool RaycastPosition (ref Vector3 position, float lasty)
		{
			RaycastHit hit;
			float up = Mathf.Max (playerHeight / 2, lasty - position.y + playerHeight / 2);
			// make sure ray shoot from above ground
			if (Physics.Raycast (position + Vector3.up * up, Vector3.down, out hit, up, -1)) {
				if (Mathf.Abs (hit.distance - up) < 0.1f || hit.distance < up) {
					position = hit.point;
					return true;
				}
			}
			return false;
		}


		void OnNetDragBegin ()
		{
			anim.Play ("drag_float", 0, 0);
		}

		void OnNetDragEnd ()
		{
			anim.Play ("drag_drop", 0, 0);
		}

		#region Network

		// 客户端通知服务器对象，服务器通知其他客户端，注意ServerOnly的服务器本身不接受Rpc指令
		[Command]
		void CmdOnDragBegin ()
		{
			RpcOnDragBegin ();
			OnNetDragBegin ();
		}
		
		// 客户端通知服务器对象，服务器通知其他客户端，注意ServerOnly的服务器本身不接受Rpc指令
		[Command]
		void CmdOnDragEnd ()
		{
			RpcOnDragEnd ();
			OnNetDragEnd ();
		}

		[ClientRpc]
		void RpcOnDragBegin ()
		{
			if (!isLocalPlayer) {
				OnNetDragBegin ();
			}
		}

		[ClientRpc]
		void RpcOnDragEnd ()
		{
			if (!isLocalPlayer) {
				OnNetDragEnd ();
			}
		}

		#endregion
	}
}