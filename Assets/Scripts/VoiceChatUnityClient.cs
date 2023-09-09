using System;
using UnityEngine;

public class VoiceChatUnityClient : MonoBehaviour
{
	private void Start()
	{
		Network.Connect(this.Address, this.Port);
	}

	private void OnConnectedToServer()
	{
		this.proxy = VoiceChatUtils.CreateProxy();
	}

	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		UnityEngine.Object.Destroy(this.proxy.gameObject);
	}

	private VoiceChatNetworkProxy proxy;

	public int Port = 15000;

	public string Address = "127.0.0.1";
}
