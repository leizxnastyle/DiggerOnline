using System;
using UnityEngine;

public class VoiceChatUnityServer : MonoBehaviour
{
	private void Start()
	{
		Network.InitializeServer(this.MaxConnections, this.Port, false);
		UnityEngine.Object.Destroy(base.GetComponent<VoiceChatUnityClient>());
	}

	private void OnPlayerDisconnected(NetworkPlayer player)
	{
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	public int Port = 15000;

	public int MaxConnections = 8;
}
