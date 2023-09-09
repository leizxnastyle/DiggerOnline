using System;
using System.Text;
using UnityEngine;

public class SupportLogging : MonoBehaviour
{
	public void Start()
	{
		if (this.LogTrafficStats)
		{
			base.InvokeRepeating("LogStats", 10f, 10f);
		}
	}

	public void OnApplicationQuit()
	{
		base.CancelInvoke();
	}

	public void LogStats()
	{
		if (this.LogTrafficStats)
		{
			UnityEngine.Debug.Log("SupportLogger " + PhotonNetwork.NetworkStatisticsToString());
		}
	}

	private void LogBasics()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("SupportLogger Info: PUN {0}: ", "1.64.2");
		stringBuilder.AppendFormat("AppID: {0}*** GameVersion: {1} ", PhotonNetwork.networkingPeer.mAppId.Substring(0, 8), PhotonNetwork.networkingPeer.mAppVersionPun);
		stringBuilder.AppendFormat("Server: {0}. Region: {1} ", PhotonNetwork.ServerAddress, PhotonNetwork.networkingPeer.CloudRegion);
		stringBuilder.AppendFormat("HostType: {0} ", PhotonNetwork.PhotonServerSettings.HostType);
		UnityEngine.Debug.Log(stringBuilder.ToString());
	}

	public void OnConnectedToPhoton()
	{
		UnityEngine.Debug.Log("SupportLogger OnConnectedToPhoton().");
		this.LogBasics();
		if (this.LogTrafficStats)
		{
			PhotonNetwork.NetworkStatisticsEnabled = true;
		}
	}

	public void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		UnityEngine.Debug.Log("SupportLogger OnFailedToConnectToPhoton(" + cause + ").");
		this.LogBasics();
	}

	public void OnJoinedLobby()
	{
		UnityEngine.Debug.Log("SupportLogger OnJoinedLobby(" + PhotonNetwork.lobby + ").");
	}

	public void OnJoinedRoom()
	{
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"SupportLogger OnJoinedRoom(",
			PhotonNetwork.room,
			"). ",
			PhotonNetwork.lobby,
			" GameServer:",
			PhotonNetwork.ServerAddress
		}));
	}

	public void OnCreatedRoom()
	{
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"SupportLogger OnCreatedRoom(",
			PhotonNetwork.room,
			"). ",
			PhotonNetwork.lobby,
			" GameServer:",
			PhotonNetwork.ServerAddress
		}));
	}

	public void OnLeftRoom()
	{
		UnityEngine.Debug.Log("SupportLogger OnLeftRoom().");
	}

	public bool LogTrafficStats;
}
