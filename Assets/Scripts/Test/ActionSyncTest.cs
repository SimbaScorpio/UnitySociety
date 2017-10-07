using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DesignSociety;

public class ActionSyncTest : NetworkBehaviour, IActionCompleted
{
	Animator anim;

	void Start ()
	{
		anim = GetComponent<Animator> ();
		for (int i = 0; i < anim.parameterCount; ++i)
			GetComponent<NetworkAnimator> ().SetParameterAutoSend (i, true);
		if (isLocalPlayer)
			GetComponentInChildren<MeshRenderer> ().material.color = Color.red;
	}

	void Update ()
	{
		if (!isLocalPlayer)
			return;
		if (Input.GetKeyDown (KeyCode.A)) {
			SyncActionCubeMove ac = gameObject.AddComponent<SyncActionCubeMove> ();
			ac.Setting (gameObject, "CubeMove2", this);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			SyncActionCubeMove ac = gameObject.AddComponent<SyncActionCubeMove> ();
			ac.Setting (gameObject, "CubeMove3", this);
		}
	}

	public void OnActionCompleted (Action ac)
	{
		print ("Finish");
	}

	[Command]
	public void CmdSyncAction (string name)
	{
		RpcSyncAction (name);
	}

	[ClientRpc]
	void RpcSyncAction (string name)
	{
		if (!isLocalPlayer)
			anim.Play (name, 0, 0f);
	}
}
