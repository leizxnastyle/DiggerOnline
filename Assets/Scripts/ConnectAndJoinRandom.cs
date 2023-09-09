using System;
using Photon;
using UnityEngine;

public class ConnectAndJoinRandom : Photon.MonoBehaviour
{
	public virtual void Start()
	{
		PhotonNetwork.autoJoinLobby = false;
	}

	public virtual void Update()
	{
		if (this.ConnectInUpdate && this.AutoConnect && !PhotonNetwork.connected)
		{
			UnityEngine.Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");
			this.ConnectInUpdate = false;
			PhotonNetwork.ConnectUsingSettings(this.Version + "." + UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
		}
	}

	public virtual void OnConnectedToMaster()
	{
		if (PhotonNetwork.networkingPeer.AvailableRegions != null)
		{
			UnityEngine.Debug.LogWarning(string.Concat(new object[]
			{
				"List of available regions counts ",
				PhotonNetwork.networkingPeer.AvailableRegions.Count,
				". First: ",
				PhotonNetwork.networkingPeer.AvailableRegions[0],
				" \t Current Region: ",
				PhotonNetwork.networkingPeer.CloudRegion
			}));
		}
		UnityEngine.Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
		PhotonNetwork.JoinRandomRoom();
	}

	public virtual void OnPhotonRandomJoinFailed()
	{
		UnityEngine.Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
		PhotonNetwork.CreateRoom(null, new RoomOptions
		{
			maxPlayers = 4
		}, null);
	}

	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		UnityEngine.Debug.LogError("Cause: " + cause);
	}

	public void OnJoinedRoom()
	{
		UnityEngine.Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
	}

	public void OnJoinedLobby()
	{
		UnityEngine.Debug.Log("OnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().");
	}

	public bool AutoConnect = true;

	public byte Version = 1;

	private bool ConnectInUpdate = true;
}
