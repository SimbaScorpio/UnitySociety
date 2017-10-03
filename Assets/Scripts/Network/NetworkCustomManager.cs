using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkCustomManager : NetworkManager
{
	public override void OnStartHost ()
	{
		base.OnStartHost ();
		print ("OnStartHost");
	}

	public override void OnStopHost ()
	{
		base.OnStopHost ();
		print ("OnStopHost");
	}

	public override void OnClientConnect (NetworkConnection conn)
	{
		base.OnClientConnect (conn);
		print ("OnClientConnect");
	}

	public override void OnClientDisconnect (NetworkConnection conn)
	{
		base.OnClientDisconnect (conn);
		print ("OnClientDisconnect");
	}

	public override void OnStartClient (NetworkClient client)
	{
		base.OnStartClient (client);
		print ("OnStartClient");
	}

	public override void OnStopClient ()
	{
		base.OnStopClient ();
		print ("OnStopClient");
	}
}
