using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public static class PhotonNetwork
{
	static PhotonNetwork()
	{
		Application.runInBackground = true;
		GameObject gameObject = new GameObject();
		PhotonNetwork.photonMono = gameObject.AddComponent<PhotonHandler>();
		gameObject.name = "PhotonMono";
		gameObject.hideFlags = HideFlags.HideInHierarchy;
		ConnectionProtocol protocol = PhotonNetwork.PhotonServerSettings.Protocol;
		PhotonNetwork.networkingPeer = new NetworkingPeer(string.Empty, protocol);
		PhotonNetwork.networkingPeer.QuickResendAttempts = 2;
		PhotonNetwork.networkingPeer.SentCountAllowance = 7;
		CustomTypes.Register();
	}

	public static string gameVersion { get; set; }

	public static string ServerAddress
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? "<not connected>" : PhotonNetwork.networkingPeer.ServerAddress;
		}
	}

	public static bool connected
	{
		get
		{
			return PhotonNetwork.offlineMode || (PhotonNetwork.networkingPeer != null && (!PhotonNetwork.networkingPeer.IsInitialConnect && PhotonNetwork.networkingPeer.State != PeerState.PeerCreated && PhotonNetwork.networkingPeer.State != PeerState.Disconnected && PhotonNetwork.networkingPeer.State != PeerState.Disconnecting) && PhotonNetwork.networkingPeer.State != PeerState.ConnectingToNameServer);
		}
	}

	public static bool connecting
	{
		get
		{
			return PhotonNetwork.networkingPeer.IsInitialConnect && !PhotonNetwork.offlineMode;
		}
	}

	public static bool connectedAndReady
	{
		get
		{
			if (!PhotonNetwork.connected)
			{
				return false;
			}
			if (PhotonNetwork.offlineMode)
			{
				return true;
			}
			PeerState connectionStateDetailed = PhotonNetwork.connectionStateDetailed;
			switch (connectionStateDetailed)
			{
			case PeerState.ConnectingToMasterserver:
			case PeerState.Disconnecting:
			case PeerState.Disconnected:
			case PeerState.ConnectingToNameServer:
			case PeerState.Authenticating:
				break;
			default:
				switch (connectionStateDetailed)
				{
				case PeerState.ConnectingToGameserver:
				case PeerState.Joining:
					break;
				default:
					if (connectionStateDetailed != PeerState.PeerCreated)
					{
						return true;
					}
					break;
				}
				break;
			}
			return false;
		}
	}

	public static ConnectionState connectionState
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return ConnectionState.Connected;
			}
			if (PhotonNetwork.networkingPeer == null)
			{
				return ConnectionState.Disconnected;
			}
			PeerStateValue peerState = PhotonNetwork.networkingPeer.PeerState;
			switch (peerState)
			{
			case PeerStateValue.Disconnected:
				return ConnectionState.Disconnected;
			case PeerStateValue.Connecting:
				return ConnectionState.Connecting;
			default:
				if (peerState != PeerStateValue.InitializingApplication)
				{
					return ConnectionState.Disconnected;
				}
				return ConnectionState.InitializingApplication;
			case PeerStateValue.Connected:
				return ConnectionState.Connected;
			case PeerStateValue.Disconnecting:
				return ConnectionState.Disconnecting;
			}
		}
	}

	public static PeerState connectionStateDetailed
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return (PhotonNetwork.offlineModeRoom == null) ? PeerState.ConnectedToMaster : PeerState.Joined;
			}
			if (PhotonNetwork.networkingPeer == null)
			{
				return PeerState.Disconnected;
			}
			return PhotonNetwork.networkingPeer.State;
		}
	}

	public static ServerConnection Server
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? ServerConnection.NameServer : PhotonNetwork.networkingPeer.server;
		}
	}

	public static AuthenticationValues AuthValues
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? null : PhotonNetwork.networkingPeer.CustomAuthenticationValues;
		}
		set
		{
			if (PhotonNetwork.networkingPeer != null)
			{
				PhotonNetwork.networkingPeer.CustomAuthenticationValues = value;
			}
		}
	}

	public static Room room
	{
		get
		{
			if (PhotonNetwork.isOfflineMode)
			{
				return PhotonNetwork.offlineModeRoom;
			}
			return PhotonNetwork.networkingPeer.CurrentGame;
		}
	}

	public static PhotonPlayer player
	{
		get
		{
			if (PhotonNetwork.networkingPeer == null)
			{
				return null;
			}
			return PhotonNetwork.networkingPeer.mLocalActor;
		}
	}

	public static PhotonPlayer masterClient
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return PhotonNetwork.player;
			}
			if (PhotonNetwork.networkingPeer == null)
			{
				return null;
			}
			return PhotonNetwork.networkingPeer.GetPlayerWithId(PhotonNetwork.networkingPeer.mMasterClientId);
		}
	}

	public static string playerName
	{
		get
		{
			return PhotonNetwork.networkingPeer.PlayerName;
		}
		set
		{
			PhotonNetwork.networkingPeer.PlayerName = value;
		}
	}

	public static PhotonPlayer[] playerList
	{
		get
		{
			if (PhotonNetwork.networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return PhotonNetwork.networkingPeer.mPlayerListCopy;
		}
	}

	public static PhotonPlayer[] otherPlayers
	{
		get
		{
			if (PhotonNetwork.networkingPeer == null)
			{
				return new PhotonPlayer[0];
			}
			return PhotonNetwork.networkingPeer.mOtherPlayerListCopy;
		}
	}

	public static List<FriendInfo> Friends { get; internal set; }

	public static int FriendsListAge
	{
		get
		{
			return (PhotonNetwork.networkingPeer == null) ? 0 : PhotonNetwork.networkingPeer.FriendsListAge;
		}
	}

	public static IPunPrefabPool PrefabPool
	{
		get
		{
			return PhotonNetwork.networkingPeer.ObjectPool;
		}
		set
		{
			PhotonNetwork.networkingPeer.ObjectPool = value;
		}
	}

	public static bool offlineMode
	{
		get
		{
			return PhotonNetwork.isOfflineMode;
		}
		set
		{
			if (value == PhotonNetwork.isOfflineMode)
			{
				return;
			}
			if (value && PhotonNetwork.connected)
			{
				UnityEngine.Debug.LogError("Can't start OFFLINE mode while connected!");
				return;
			}
			if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
			{
				PhotonNetwork.networkingPeer.Disconnect();
			}
			PhotonNetwork.isOfflineMode = value;
			if (PhotonNetwork.isOfflineMode)
			{
				PhotonNetwork.networkingPeer.ChangeLocalID(-1);
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster, new object[0]);
			}
			else
			{
				PhotonNetwork.offlineModeRoom = null;
				PhotonNetwork.networkingPeer.ChangeLocalID(-1);
			}
		}
	}

	public static bool automaticallySyncScene
	{
		get
		{
			return PhotonNetwork._mAutomaticallySyncScene;
		}
		set
		{
			PhotonNetwork._mAutomaticallySyncScene = value;
			if (PhotonNetwork._mAutomaticallySyncScene && PhotonNetwork.room != null)
			{
				PhotonNetwork.networkingPeer.LoadLevelIfSynced();
			}
		}
	}

	public static bool autoCleanUpPlayerObjects
	{
		get
		{
			return PhotonNetwork.m_autoCleanUpPlayerObjects;
		}
		set
		{
			if (PhotonNetwork.room != null)
			{
				UnityEngine.Debug.LogError("Setting autoCleanUpPlayerObjects while in a room is not supported.");
			}
			else
			{
				PhotonNetwork.m_autoCleanUpPlayerObjects = value;
			}
		}
	}

	public static bool autoJoinLobby
	{
		get
		{
			return PhotonNetwork.PhotonServerSettings.JoinLobby;
		}
		set
		{
			PhotonNetwork.PhotonServerSettings.JoinLobby = value;
		}
	}

	public static bool EnableLobbyStatistics
	{
		get
		{
			return PhotonNetwork.PhotonServerSettings.EnableLobbyStatistics;
		}
		set
		{
			PhotonNetwork.PhotonServerSettings.EnableLobbyStatistics = value;
		}
	}

	public static List<TypedLobbyInfo> LobbyStatistics
	{
		get
		{
			return PhotonNetwork.networkingPeer.LobbyStatistics;
		}
		private set
		{
			PhotonNetwork.networkingPeer.LobbyStatistics = value;
		}
	}

	public static bool insideLobby
	{
		get
		{
			return PhotonNetwork.networkingPeer.insideLobby;
		}
	}

	public static TypedLobby lobby
	{
		get
		{
			return PhotonNetwork.networkingPeer.lobby;
		}
		set
		{
			PhotonNetwork.networkingPeer.lobby = value;
		}
	}

	public static int sendRate
	{
		get
		{
			return 1000 / PhotonNetwork.sendInterval;
		}
		set
		{
			PhotonNetwork.sendInterval = 1000 / value;
			if (PhotonNetwork.photonMono != null)
			{
				PhotonNetwork.photonMono.updateInterval = PhotonNetwork.sendInterval;
			}
			if (value < PhotonNetwork.sendRateOnSerialize)
			{
				PhotonNetwork.sendRateOnSerialize = value;
			}
		}
	}

	public static int sendRateOnSerialize
	{
		get
		{
			return 1000 / PhotonNetwork.sendIntervalOnSerialize;
		}
		set
		{
			if (value > PhotonNetwork.sendRate)
			{
				UnityEngine.Debug.LogError("Error, can not set the OnSerialize SendRate more often then the overall SendRate");
				value = PhotonNetwork.sendRate;
			}
			PhotonNetwork.sendIntervalOnSerialize = 1000 / value;
			if (PhotonNetwork.photonMono != null)
			{
				PhotonNetwork.photonMono.updateIntervalOnSerialize = PhotonNetwork.sendIntervalOnSerialize;
			}
		}
	}

	public static bool isMessageQueueRunning
	{
		get
		{
			return PhotonNetwork.m_isMessageQueueRunning;
		}
		set
		{
			if (value)
			{
				PhotonHandler.StartFallbackSendAckThread();
			}
			PhotonNetwork.networkingPeer.IsSendingOnlyAcks = !value;
			PhotonNetwork.m_isMessageQueueRunning = value;
		}
	}

	public static int unreliableCommandsLimit
	{
		get
		{
			return PhotonNetwork.networkingPeer.LimitOfUnreliableCommands;
		}
		set
		{
			PhotonNetwork.networkingPeer.LimitOfUnreliableCommands = value;
		}
	}

	public static double time
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return (double)Time.time;
			}
			uint serverTimeInMilliSeconds = (uint)PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds;
			double num = serverTimeInMilliSeconds;
			return num / 1000.0;
		}
	}

	public static int ServerTimestamp
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return Environment.TickCount;
			}
			return PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds;
		}
	}

	public static bool isMasterClient
	{
		get
		{
			return PhotonNetwork.offlineMode || PhotonNetwork.networkingPeer.mMasterClientId == PhotonNetwork.player.ID;
		}
	}

	public static bool inRoom
	{
		get
		{
			return PhotonNetwork.connectionStateDetailed == PeerState.Joined;
		}
	}

	public static bool isNonMasterClientInRoom
	{
		get
		{
			return !PhotonNetwork.isMasterClient && PhotonNetwork.room != null;
		}
	}

	public static int countOfPlayersOnMaster
	{
		get
		{
			return PhotonNetwork.networkingPeer.mPlayersOnMasterCount;
		}
	}

	public static int countOfPlayersInRooms
	{
		get
		{
			return PhotonNetwork.networkingPeer.mPlayersInRoomsCount;
		}
	}

	public static int countOfPlayers
	{
		get
		{
			return PhotonNetwork.networkingPeer.mPlayersInRoomsCount + PhotonNetwork.networkingPeer.mPlayersOnMasterCount;
		}
	}

	public static int countOfRooms
	{
		get
		{
			return PhotonNetwork.networkingPeer.mGameCount;
		}
	}

	public static bool NetworkStatisticsEnabled
	{
		get
		{
			return PhotonNetwork.networkingPeer.TrafficStatsEnabled;
		}
		set
		{
			PhotonNetwork.networkingPeer.TrafficStatsEnabled = value;
		}
	}

	public static int ResentReliableCommands
	{
		get
		{
			return PhotonNetwork.networkingPeer.ResentReliableCommands;
		}
	}

	public static bool CrcCheckEnabled
	{
		get
		{
			return PhotonNetwork.networkingPeer.CrcEnabled;
		}
		set
		{
			if (!PhotonNetwork.connected && !PhotonNetwork.connecting)
			{
				PhotonNetwork.networkingPeer.CrcEnabled = value;
			}
			else
			{
				UnityEngine.Debug.Log("Can't change CrcCheckEnabled while being connected. CrcCheckEnabled stays " + PhotonNetwork.networkingPeer.CrcEnabled);
			}
		}
	}

	public static int PacketLossByCrcCheck
	{
		get
		{
			return PhotonNetwork.networkingPeer.PacketLossByCrc;
		}
	}

	public static int MaxResendsBeforeDisconnect
	{
		get
		{
			return PhotonNetwork.networkingPeer.SentCountAllowance;
		}
		set
		{
			if (value < 3)
			{
				value = 3;
			}
			if (value > 10)
			{
				value = 10;
			}
			PhotonNetwork.networkingPeer.SentCountAllowance = value;
		}
	}

	public static int QuickResends
	{
		get
		{
			return (int)PhotonNetwork.networkingPeer.QuickResendAttempts;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > 3)
			{
				value = 3;
			}
			PhotonNetwork.networkingPeer.QuickResendAttempts = (byte)value;
		}
	}

	public static void SwitchToProtocol(ConnectionProtocol cp)
	{
		if (PhotonNetwork.networkingPeer.UsedProtocol == cp)
		{
			return;
		}
		try
		{
			PhotonNetwork.networkingPeer.Disconnect();
			PhotonNetwork.networkingPeer.StopThread();
		}
		catch
		{
		}
		PhotonNetwork.networkingPeer = new NetworkingPeer(string.Empty, cp)
		{
			CustomAuthenticationValues = PhotonNetwork.networkingPeer.CustomAuthenticationValues,
			PlayerName = PhotonNetwork.networkingPeer.PlayerName,
			mLocalActor = PhotonNetwork.networkingPeer.mLocalActor,
			DebugOut = PhotonNetwork.networkingPeer.DebugOut,
			CrcEnabled = PhotonNetwork.networkingPeer.CrcEnabled,
			QuickResendAttempts = PhotonNetwork.networkingPeer.QuickResendAttempts,
			DisconnectTimeout = PhotonNetwork.networkingPeer.DisconnectTimeout,
			lobby = PhotonNetwork.networkingPeer.lobby,
			LimitOfUnreliableCommands = PhotonNetwork.networkingPeer.LimitOfUnreliableCommands,
			SentCountAllowance = PhotonNetwork.networkingPeer.SentCountAllowance,
			TrafficStatsEnabled = PhotonNetwork.networkingPeer.TrafficStatsEnabled
		};
		UnityEngine.Debug.LogWarning("Protocol switched to: " + cp + ".");
	}

	public static bool ConnectUsingSettings(string gameVersion)
	{
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.NotSet)
		{
			UnityEngine.Debug.LogError("You did not select a Hosting Type in your PhotonServerSettings. Please set it up or don't use ConnectUsingSettings().");
			return false;
		}
		PhotonNetwork.SwitchToProtocol(PhotonNetwork.PhotonServerSettings.Protocol);
		PhotonNetwork.networkingPeer.SetApp(PhotonNetwork.PhotonServerSettings.AppID, gameVersion);
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			PhotonNetwork.offlineMode = true;
			return true;
		}
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Debug.LogWarning("ConnectUsingSettings() disabled the offline mode. No longer offline.");
		}
		PhotonNetwork.offlineMode = false;
		PhotonNetwork.isMessageQueueRunning = true;
		PhotonNetwork.networkingPeer.IsInitialConnect = true;
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.SelfHosted || !PhotonNetwork.UseNameServer)
		{
			PhotonNetwork.networkingPeer.IsUsingNameServer = false;
			PhotonNetwork.networkingPeer.MasterServerAddress = ((PhotonNetwork.PhotonServerSettings.ServerPort != 0) ? (PhotonNetwork.PhotonServerSettings.ServerAddress + ":" + PhotonNetwork.PhotonServerSettings.ServerPort) : PhotonNetwork.PhotonServerSettings.ServerAddress);
			return PhotonNetwork.networkingPeer.Connect(PhotonNetwork.networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
		}
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
		{
			return PhotonNetwork.ConnectToBestCloudServer(gameVersion);
		}
		return PhotonNetwork.networkingPeer.ConnectToRegionMaster(PhotonNetwork.PhotonServerSettings.PreferredRegion);
	}

	public static bool ConnectToMaster(string masterServerAddress, int port, string appID, string gameVersion)
	{
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode = false;
			UnityEngine.Debug.LogWarning("ConnectToMaster() disabled the offline mode. No longer offline.");
		}
		if (!PhotonNetwork.isMessageQueueRunning)
		{
			PhotonNetwork.isMessageQueueRunning = true;
			UnityEngine.Debug.LogWarning("ConnectToMaster() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
		}
		PhotonNetwork.networkingPeer.SetApp(appID, gameVersion);
		PhotonNetwork.networkingPeer.IsUsingNameServer = false;
		PhotonNetwork.networkingPeer.IsInitialConnect = true;
		PhotonNetwork.networkingPeer.MasterServerAddress = ((port != 0) ? (masterServerAddress + ":" + port) : masterServerAddress);
		return PhotonNetwork.networkingPeer.Connect(PhotonNetwork.networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
	}

	public static bool ConnectToBestCloudServer(string gameVersion)
	{
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectToBestCloudServer() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			return PhotonNetwork.ConnectUsingSettings(gameVersion);
		}
		PhotonNetwork.networkingPeer.IsInitialConnect = true;
		PhotonNetwork.networkingPeer.SetApp(PhotonNetwork.PhotonServerSettings.AppID, gameVersion);
		CloudRegionCode bestRegionCodeInPreferences = PhotonHandler.BestRegionCodeInPreferences;
		if (bestRegionCodeInPreferences != CloudRegionCode.none)
		{
			UnityEngine.Debug.Log("Best region found in PlayerPrefs. Connecting to: " + bestRegionCodeInPreferences);
			return PhotonNetwork.networkingPeer.ConnectToRegionMaster(bestRegionCodeInPreferences);
		}
		return PhotonNetwork.networkingPeer.ConnectToNameServer();
	}

	public static bool ConnectToRegion(CloudRegionCode region, string gameVersion)
	{
		if (PhotonNetwork.networkingPeer.PeerState != PeerStateValue.Disconnected)
		{
			UnityEngine.Debug.LogWarning("ConnectToRegion() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.networkingPeer.PeerState);
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings == null)
		{
			UnityEngine.Debug.LogError("Can't connect: ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
			return false;
		}
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
		{
			return PhotonNetwork.ConnectUsingSettings(gameVersion);
		}
		PhotonNetwork.networkingPeer.IsInitialConnect = true;
		PhotonNetwork.networkingPeer.SetApp(PhotonNetwork.PhotonServerSettings.AppID, gameVersion);
		if (region != CloudRegionCode.none)
		{
			UnityEngine.Debug.Log("ConnectToRegion: " + region);
			return PhotonNetwork.networkingPeer.ConnectToRegionMaster(region);
		}
		return false;
	}

	public static void OverrideBestCloudServer(CloudRegionCode region)
	{
		PhotonHandler.BestRegionCodeInPreferences = region;
	}

	public static void RefreshCloudServerRating()
	{
		throw new NotImplementedException("not available at the moment");
	}

	public static void NetworkStatisticsReset()
	{
		PhotonNetwork.networkingPeer.TrafficStatsReset();
	}

	public static string NetworkStatisticsToString()
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.offlineMode)
		{
			return "Offline or in OfflineMode. No VitalStats available.";
		}
		return PhotonNetwork.networkingPeer.VitalStatsToString(false);
	}

	[Obsolete("Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.")]
	public static void InitializeSecurity()
	{
	}

	private static bool VerifyCanUseNetwork()
	{
		if (PhotonNetwork.connected)
		{
			return true;
		}
		UnityEngine.Debug.LogError("Cannot send messages when not connected. Either connect to Photon OR use offline mode!");
		return false;
	}

	public static void Disconnect()
	{
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode = false;
			PhotonNetwork.offlineModeRoom = null;
			PhotonNetwork.networkingPeer.State = PeerState.Disconnecting;
			PhotonNetwork.networkingPeer.OnStatusChanged(StatusCode.Disconnect);
			return;
		}
		if (PhotonNetwork.networkingPeer == null)
		{
			return;
		}
		PhotonNetwork.networkingPeer.Disconnect();
	}

	public static bool FindFriends(string[] friendsToFind)
	{
		return PhotonNetwork.networkingPeer != null && !PhotonNetwork.isOfflineMode && PhotonNetwork.networkingPeer.OpFindFriends(friendsToFind);
	}

	public static bool CreateRoom(string roomName)
	{
		return PhotonNetwork.CreateRoom(roomName, null, null, null);
	}

	public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		return PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby, null);
	}

	private static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers)
	{
		if (PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("CreateRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			PhotonNetwork.EnterOfflineRoom(roomName, roomOptions, true);
			return true;
		}
		else
		{
			if (PhotonNetwork.networkingPeer.server != ServerConnection.MasterServer || !PhotonNetwork.connectedAndReady)
			{
				UnityEngine.Debug.LogError("CreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
				return false;
			}
			typedLobby = (typedLobby ?? ((!PhotonNetwork.networkingPeer.insideLobby) ? null : PhotonNetwork.networkingPeer.lobby));
			LoadbalancingPeer.EnterRoomParams enterRoomParams = new LoadbalancingPeer.EnterRoomParams();
			enterRoomParams.RoomName = roomName;
			enterRoomParams.RoomOptions = roomOptions;
			enterRoomParams.Lobby = typedLobby;
			return PhotonNetwork.networkingPeer.OpCreateGame(enterRoomParams);
		}
	}

	public static bool JoinRoom(string roomName)
	{
		if (PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("JoinRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			PhotonNetwork.EnterOfflineRoom(roomName, null, true);
			return true;
		}
		else
		{
			if (PhotonNetwork.networkingPeer.server != ServerConnection.MasterServer || !PhotonNetwork.connectedAndReady)
			{
				UnityEngine.Debug.LogError("JoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
				return false;
			}
			if (string.IsNullOrEmpty(roomName))
			{
				UnityEngine.Debug.LogError("JoinRoom failed. A roomname is required. If you don't know one, how will you join?");
				return false;
			}
			LoadbalancingPeer.EnterRoomParams enterRoomParams = new LoadbalancingPeer.EnterRoomParams();
			enterRoomParams.RoomName = roomName;
			return PhotonNetwork.networkingPeer.OpJoinRoom(enterRoomParams);
		}
	}

	public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
	{
		if (PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("JoinOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			PhotonNetwork.EnterOfflineRoom(roomName, roomOptions, true);
			return true;
		}
		else
		{
			if (PhotonNetwork.networkingPeer.server != ServerConnection.MasterServer || !PhotonNetwork.connectedAndReady)
			{
				UnityEngine.Debug.LogError("JoinOrCreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
				return false;
			}
			if (string.IsNullOrEmpty(roomName))
			{
				UnityEngine.Debug.LogError("JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
				return false;
			}
			typedLobby = (typedLobby ?? ((!PhotonNetwork.networkingPeer.insideLobby) ? null : PhotonNetwork.networkingPeer.lobby));
			LoadbalancingPeer.EnterRoomParams enterRoomParams = new LoadbalancingPeer.EnterRoomParams();
			enterRoomParams.RoomName = roomName;
			enterRoomParams.RoomOptions = roomOptions;
			enterRoomParams.Lobby = typedLobby;
			enterRoomParams.CreateIfNotExists = true;
			enterRoomParams.PlayerProperties = PhotonNetwork.player.customProperties;
			return PhotonNetwork.networkingPeer.OpJoinRoom(enterRoomParams);
		}
	}

	public static bool JoinRandomRoom()
	{
		return PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null);
	}

	public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
	{
		return PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, null, null);
	}

	public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
	{
		if (PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.offlineModeRoom != null)
			{
				UnityEngine.Debug.LogError("JoinRandomRoom failed. In offline mode you still have to leave a room to enter another.");
				return false;
			}
			PhotonNetwork.EnterOfflineRoom("offline room", null, true);
			return true;
		}
		else
		{
			if (PhotonNetwork.networkingPeer.server != ServerConnection.MasterServer || !PhotonNetwork.connectedAndReady)
			{
				UnityEngine.Debug.LogError("JoinRandomRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
				return false;
			}
			typedLobby = (typedLobby ?? ((!PhotonNetwork.networkingPeer.insideLobby) ? null : PhotonNetwork.networkingPeer.lobby));
			LoadbalancingPeer.OpJoinRandomRoomParams opJoinRandomRoomParams = new LoadbalancingPeer.OpJoinRandomRoomParams();
			opJoinRandomRoomParams.ExpectedCustomRoomProperties = expectedCustomRoomProperties;
			opJoinRandomRoomParams.ExpectedMaxPlayers = expectedMaxPlayers;
			opJoinRandomRoomParams.MatchingType = matchingType;
			opJoinRandomRoomParams.TypedLobby = typedLobby;
			opJoinRandomRoomParams.SqlLobbyFilter = sqlLobbyFilter;
			return PhotonNetwork.networkingPeer.OpJoinRandomRoom(opJoinRandomRoomParams);
		}
	}

	private static void EnterOfflineRoom(string roomName, RoomOptions roomOptions, bool createdRoom)
	{
		PhotonNetwork.offlineModeRoom = new Room(roomName, roomOptions);
		PhotonNetwork.networkingPeer.ChangeLocalID(1);
		PhotonNetwork.offlineModeRoom.masterClientId = 1;
		if (createdRoom)
		{
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
		}
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
	}

	public static bool JoinLobby()
	{
		return PhotonNetwork.JoinLobby(null);
	}

	public static bool JoinLobby(TypedLobby typedLobby)
	{
		if (PhotonNetwork.connected && PhotonNetwork.Server == ServerConnection.MasterServer)
		{
			if (typedLobby == null)
			{
				typedLobby = TypedLobby.Default;
			}
			bool flag = PhotonNetwork.networkingPeer.OpJoinLobby(typedLobby);
			if (flag)
			{
				PhotonNetwork.networkingPeer.lobby = typedLobby;
			}
			return flag;
		}
		return false;
	}

	public static bool LeaveLobby()
	{
		return PhotonNetwork.connected && PhotonNetwork.Server == ServerConnection.MasterServer && PhotonNetwork.networkingPeer.OpLeaveLobby();
	}

	public static bool LeaveRoom()
	{
		if (PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineModeRoom = null;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom, new object[0]);
			return true;
		}
		if (PhotonNetwork.room == null)
		{
			UnityEngine.Debug.LogWarning("PhotonNetwork.room is null. You don't have to call LeaveRoom() when you're not in one. State: " + PhotonNetwork.connectionStateDetailed);
		}
		return PhotonNetwork.networkingPeer.OpLeave();
	}

	public static RoomInfo[] GetRoomList()
	{
		if (PhotonNetwork.offlineMode || PhotonNetwork.networkingPeer == null)
		{
			return new RoomInfo[0];
		}
		return PhotonNetwork.networkingPeer.mGameListCopy;
	}

	public static void SetPlayerCustomProperties(Hashtable customProperties)
	{
		if (customProperties == null)
		{
			customProperties = new Hashtable();
			foreach (object obj in PhotonNetwork.player.customProperties.Keys)
			{
				customProperties[(string)obj] = null;
			}
		}
		if (PhotonNetwork.room != null && PhotonNetwork.room.isLocalClientInside)
		{
			PhotonNetwork.player.SetCustomProperties(customProperties, null, false);
		}
		else
		{
			PhotonNetwork.player.InternalCacheProperties(customProperties);
		}
	}

	public static void RemovePlayerCustomProperties(string[] customPropertiesToDelete)
	{
		if (customPropertiesToDelete == null || customPropertiesToDelete.Length == 0 || PhotonNetwork.player.customProperties == null)
		{
			PhotonNetwork.player.customProperties = new Hashtable();
			return;
		}
		foreach (string key in customPropertiesToDelete)
		{
			if (PhotonNetwork.player.customProperties.ContainsKey(key))
			{
				PhotonNetwork.player.customProperties.Remove(key);
			}
		}
	}

	public static bool RaiseEvent(byte eventCode, object eventContent, bool sendReliable, RaiseEventOptions options)
	{
		if (!PhotonNetwork.inRoom || eventCode >= 200)
		{
			UnityEngine.Debug.LogWarning("RaiseEvent() failed. Your event is not being sent! Check if your are in a Room and the eventCode must be less than 200 (0..199).");
			return false;
		}
		return PhotonNetwork.networkingPeer.OpRaiseEvent(eventCode, eventContent, sendReliable, options);
	}

	public static int AllocateViewID()
	{
		int num = PhotonNetwork.AllocateViewID(PhotonNetwork.player.ID);
		PhotonNetwork.manuallyAllocatedViewIds.Add(num);
		return num;
	}

	public static int AllocateSceneViewID()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			UnityEngine.Debug.LogError("Only the Master Client can AllocateSceneViewID(). Check PhotonNetwork.isMasterClient!");
			return -1;
		}
		int num = PhotonNetwork.AllocateViewID(0);
		PhotonNetwork.manuallyAllocatedViewIds.Add(num);
		return num;
	}

	private static int AllocateViewID(int ownerId)
	{
		if (ownerId == 0)
		{
			int num = PhotonNetwork.lastUsedViewSubIdStatic;
			int num2 = ownerId * PhotonNetwork.MAX_VIEW_IDS;
			for (int i = 1; i < PhotonNetwork.MAX_VIEW_IDS; i++)
			{
				num = (num + 1) % PhotonNetwork.MAX_VIEW_IDS;
				if (num != 0)
				{
					int num3 = num + num2;
					if (!PhotonNetwork.networkingPeer.photonViewList.ContainsKey(num3))
					{
						PhotonNetwork.lastUsedViewSubIdStatic = num;
						return num3;
					}
				}
			}
			throw new Exception(string.Format("AllocateViewID() failed. Room (user {0}) is out of 'scene' viewIDs. It seems all available are in use.", ownerId));
		}
		int num4 = PhotonNetwork.lastUsedViewSubId;
		int num5 = ownerId * PhotonNetwork.MAX_VIEW_IDS;
		for (int j = 1; j < PhotonNetwork.MAX_VIEW_IDS; j++)
		{
			num4 = (num4 + 1) % PhotonNetwork.MAX_VIEW_IDS;
			if (num4 != 0)
			{
				int num6 = num4 + num5;
				if (!PhotonNetwork.networkingPeer.photonViewList.ContainsKey(num6) && !PhotonNetwork.manuallyAllocatedViewIds.Contains(num6))
				{
					PhotonNetwork.lastUsedViewSubId = num4;
					return num6;
				}
			}
		}
		throw new Exception(string.Format("AllocateViewID() failed. User {0} is out of subIds, as all viewIDs are used.", ownerId));
	}

	private static int[] AllocateSceneViewIDs(int countOfNewViews)
	{
		int[] array = new int[countOfNewViews];
		for (int i = 0; i < countOfNewViews; i++)
		{
			array[i] = PhotonNetwork.AllocateViewID(0);
		}
		return array;
	}

	public static void UnAllocateViewID(int viewID)
	{
		PhotonNetwork.manuallyAllocatedViewIds.Remove(viewID);
		if (PhotonNetwork.networkingPeer.photonViewList.ContainsKey(viewID))
		{
			UnityEngine.Debug.LogWarning(string.Format("UnAllocateViewID() should be called after the PhotonView was destroyed (GameObject.Destroy()). ViewID: {0} still found in: {1}", viewID, PhotonNetwork.networkingPeer.photonViewList[viewID]));
		}
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group)
	{
		return PhotonNetwork.Instantiate(prefabName, position, rotation, group, null);
	}

	public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
	{
		if (!PhotonNetwork.connected || (PhotonNetwork.InstantiateInRoomOnly && !PhotonNetwork.inRoom))
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Failed to Instantiate prefab: ",
				prefabName,
				". Client should be in a room. Current connectionStateDetailed: ",
				PhotonNetwork.connectionStateDetailed
			}));
			return null;
		}
		GameObject gameObject;
		if (!PhotonNetwork.UsePrefabCache || !PhotonNetwork.PrefabCache.TryGetValue(prefabName, out gameObject))
		{
			gameObject = (GameObject)Resources.Load(prefabName, typeof(GameObject));
			if (PhotonNetwork.UsePrefabCache)
			{
				PhotonNetwork.PrefabCache.Add(prefabName, gameObject);
			}
		}
		if (gameObject == null)
		{
			UnityEngine.Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
			return null;
		}
		if (gameObject.GetComponent<PhotonView>() == null)
		{
			UnityEngine.Debug.LogError("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component.");
			return null;
		}
		Component[] photonViewsInChildren = gameObject.GetPhotonViewsInChildren();
		int[] array = new int[photonViewsInChildren.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = PhotonNetwork.AllocateViewID(PhotonNetwork.player.ID);
		}
		Hashtable evData = PhotonNetwork.networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, false);
		return PhotonNetwork.networkingPeer.DoInstantiate(evData, PhotonNetwork.networkingPeer.mLocalActor, gameObject);
	}

	public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
	{
		if (!PhotonNetwork.connected || (PhotonNetwork.InstantiateInRoomOnly && !PhotonNetwork.inRoom))
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Failed to InstantiateSceneObject prefab: ",
				prefabName,
				". Client should be in a room. Current connectionStateDetailed: ",
				PhotonNetwork.connectionStateDetailed
			}));
			return null;
		}
		if (!PhotonNetwork.isMasterClient)
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client is not the MasterClient in this room.");
			return null;
		}
		GameObject gameObject;
		if (!PhotonNetwork.UsePrefabCache || !PhotonNetwork.PrefabCache.TryGetValue(prefabName, out gameObject))
		{
			gameObject = (GameObject)Resources.Load(prefabName, typeof(GameObject));
			if (PhotonNetwork.UsePrefabCache)
			{
				PhotonNetwork.PrefabCache.Add(prefabName, gameObject);
			}
		}
		if (gameObject == null)
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
			return null;
		}
		if (gameObject.GetComponent<PhotonView>() == null)
		{
			UnityEngine.Debug.LogError("Failed to InstantiateSceneObject prefab:" + prefabName + ". Prefab must have a PhotonView component.");
			return null;
		}
		Component[] photonViewsInChildren = gameObject.GetPhotonViewsInChildren();
		int[] array = PhotonNetwork.AllocateSceneViewIDs(photonViewsInChildren.Length);
		if (array == null)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Failed to InstantiateSceneObject prefab: ",
				prefabName,
				". No ViewIDs are free to use. Max is: ",
				PhotonNetwork.MAX_VIEW_IDS
			}));
			return null;
		}
		Hashtable evData = PhotonNetwork.networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, true);
		return PhotonNetwork.networkingPeer.DoInstantiate(evData, PhotonNetwork.networkingPeer.mLocalActor, gameObject);
	}

	public static int GetPing()
	{
		return PhotonNetwork.networkingPeer.RoundTripTime;
	}

	public static void FetchServerTimestamp()
	{
		if (PhotonNetwork.networkingPeer != null)
		{
			PhotonNetwork.networkingPeer.FetchServerTimestamp();
		}
	}

	public static void SendOutgoingCommands()
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		while (PhotonNetwork.networkingPeer.SendOutgoingCommands())
		{
		}
	}

	public static bool CloseConnection(PhotonPlayer kickPlayer)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return false;
		}
		if (!PhotonNetwork.player.isMasterClient)
		{
			UnityEngine.Debug.LogError("CloseConnection: Only the masterclient can kick another player.");
			return false;
		}
		if (kickPlayer == null)
		{
			UnityEngine.Debug.LogError("CloseConnection: No such player connected!");
			return false;
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			TargetActors = new int[]
			{
				kickPlayer.ID
			}
		};
		return PhotonNetwork.networkingPeer.OpRaiseEvent(203, null, true, raiseEventOptions);
	}

	public static bool SetMasterClient(PhotonPlayer masterClientPlayer)
	{
		if (!PhotonNetwork.inRoom || !PhotonNetwork.VerifyCanUseNetwork() || PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.logLevel == PhotonLogLevel.Informational)
			{
				UnityEngine.Debug.Log("Can not SetMasterClient(). Not in room or in offlineMode.");
			}
			return false;
		}
		if (PhotonNetwork.room.serverSideMasterClient)
		{
			Hashtable gameProperties = new Hashtable
			{
				{
					248,
					masterClientPlayer.ID
				}
			};
			Hashtable expectedProperties = new Hashtable
			{
				{
					248,
					PhotonNetwork.networkingPeer.mMasterClientId
				}
			};
			return PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(gameProperties, expectedProperties, false);
		}
		return PhotonNetwork.isMasterClient && PhotonNetwork.networkingPeer.SetMasterClient(masterClientPlayer.ID, true);
	}

	public static void Destroy(PhotonView targetView)
	{
		if (targetView != null)
		{
			PhotonNetwork.networkingPeer.RemoveInstantiatedGO(targetView.gameObject, !PhotonNetwork.inRoom);
		}
		else
		{
			UnityEngine.Debug.LogError("Destroy(targetPhotonView) failed, cause targetPhotonView is null.");
		}
	}

	public static void Destroy(GameObject targetGo)
	{
		PhotonNetwork.networkingPeer.RemoveInstantiatedGO(targetGo, !PhotonNetwork.inRoom);
	}

	public static void DestroyPlayerObjects(PhotonPlayer targetPlayer)
	{
		if (PhotonNetwork.player == null)
		{
			UnityEngine.Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
		}
		PhotonNetwork.DestroyPlayerObjects(targetPlayer.ID);
	}

	public static void DestroyPlayerObjects(int targetPlayerId)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (PhotonNetwork.player.isMasterClient || targetPlayerId == PhotonNetwork.player.ID)
		{
			PhotonNetwork.networkingPeer.DestroyPlayerObjects(targetPlayerId, false);
		}
		else
		{
			UnityEngine.Debug.LogError("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + PhotonNetwork.isMasterClient);
		}
	}

	public static void DestroyAll()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.networkingPeer.DestroyAll(false);
		}
		else
		{
			UnityEngine.Debug.LogError("Couldn't call DestroyAll() as only the master client is allowed to call this.");
		}
	}

	public static void RemoveRPCs(PhotonPlayer targetPlayer)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (!targetPlayer.isLocal && !PhotonNetwork.isMasterClient)
		{
			UnityEngine.Debug.LogError("Error; Only the MasterClient can call RemoveRPCs for other players.");
			return;
		}
		PhotonNetwork.networkingPeer.OpCleanRpcBuffer(targetPlayer.ID);
	}

	public static void RemoveRPCs(PhotonView targetPhotonView)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.CleanRpcBufferIfMine(targetPhotonView);
	}

	public static void RemoveRPCsInGroup(int targetGroup)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.RemoveRPCsInGroup(targetGroup);
	}

	internal static void RPC(PhotonView view, string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (PhotonNetwork.room == null)
		{
			UnityEngine.Debug.LogWarning("Cannot send RPCs in Lobby! RPC dropped.");
			return;
		}
		if (PhotonNetwork.networkingPeer != null)
		{
			if (PhotonNetwork.networkingPeer.hasSwitchedMC && target == PhotonTargets.MasterClient)
			{
				PhotonNetwork.networkingPeer.RPC(view, methodName, PhotonNetwork.masterClient, encrypt, parameters);
			}
			else
			{
				PhotonNetwork.networkingPeer.RPC(view, methodName, target, encrypt, parameters);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
		}
	}

	internal static void RPC(PhotonView view, string methodName, PhotonPlayer targetPlayer, bool encrpyt, params object[] parameters)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		if (PhotonNetwork.room == null)
		{
			UnityEngine.Debug.LogWarning("Cannot send RPCs in Lobby, only processed locally");
			return;
		}
		if (PhotonNetwork.player == null)
		{
			UnityEngine.Debug.LogError("Error; Sending RPC to player null! Aborted \"" + methodName + "\"");
		}
		if (PhotonNetwork.networkingPeer != null)
		{
			PhotonNetwork.networkingPeer.RPC(view, methodName, targetPlayer, encrpyt, parameters);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
		}
	}

	public static void CacheSendMonoMessageTargets(Type type)
	{
		if (type == null)
		{
			type = PhotonNetwork.SendMonoMessageTargetType;
		}
		PhotonNetwork.SendMonoMessageTargets = PhotonNetwork.FindGameObjectsWithComponent(type);
	}

	public static HashSet<GameObject> FindGameObjectsWithComponent(Type type)
	{
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		Component[] array = (Component[])UnityEngine.Object.FindObjectsOfType(type);
		for (int i = 0; i < array.Length; i++)
		{
			hashSet.Add(array[i].gameObject);
		}
		return hashSet;
	}

	public static void SetReceivingEnabled(int group, bool enabled)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetReceivingEnabled(group, enabled);
	}

	public static void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetReceivingEnabled(enableGroups, disableGroups);
	}

	public static void SetSendingEnabled(int group, bool enabled)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetSendingEnabled(group, enabled);
	}

	public static void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetSendingEnabled(enableGroups, disableGroups);
	}

	public static void SetLevelPrefix(short prefix)
	{
		if (!PhotonNetwork.VerifyCanUseNetwork())
		{
			return;
		}
		PhotonNetwork.networkingPeer.SetLevelPrefix(prefix);
	}

	public static void LoadLevel(int levelNumber)
	{
		PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(levelNumber);
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.networkingPeer.loadingLevelAndPausedNetwork = true;
		UnityEngine.SceneManagement.SceneManager.LoadScene(levelNumber);
	}

	public static void LoadLevel(string levelName)
	{
		PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(levelName);
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.networkingPeer.loadingLevelAndPausedNetwork = true;
		UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
	}

	public static bool WebRpc(string name, object parameters)
	{
		return PhotonNetwork.networkingPeer.WebRpc(name, parameters);
	}

	public const string versionPUN = "1.64.2";

	internal const string serverSettingsAssetFile = "PhotonServerSettings";

	internal const string serverSettingsAssetPath = "Assets/Photon Unity Networking/Resources/PhotonServerSettings.asset";

	internal static readonly PhotonHandler photonMono;

	internal static NetworkingPeer networkingPeer;

	public static readonly int MAX_VIEW_IDS = 1000;

	public static ServerSettings PhotonServerSettings = (ServerSettings)Resources.Load("PhotonServerSettings", typeof(ServerSettings));

	public static bool InstantiateInRoomOnly = true;

	public static PhotonLogLevel logLevel = PhotonLogLevel.ErrorsOnly;

	public static float precisionForVectorSynchronization = 9.9E-05f;

	public static float precisionForQuaternionSynchronization = 1f;

	public static float precisionForFloatSynchronization = 0.01f;

	public static bool UseRpcMonoBehaviourCache;

	public static bool UsePrefabCache = true;

	public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

	public static HashSet<GameObject> SendMonoMessageTargets;

	public static Type SendMonoMessageTargetType = typeof(MonoBehaviour);

	public static bool UseNameServer = false;

	private static bool isOfflineMode = false;

	private static Room offlineModeRoom = null;

	[Obsolete("Used for compatibility with Unity networking only.")]
	public static int maxConnections;

	private static bool _mAutomaticallySyncScene = false;

	private static bool m_autoCleanUpPlayerObjects = true;

	private static int sendInterval = 50;

	private static int sendIntervalOnSerialize = 100;

	private static bool m_isMessageQueueRunning = true;

	public static float BackgroundTimeout = 0f;

	public static PhotonNetwork.EventCallback OnEventCall;

	internal static int lastUsedViewSubId = 0;

	internal static int lastUsedViewSubIdStatic = 0;

	internal static List<int> manuallyAllocatedViewIds = new List<int>();

	public delegate void EventCallback(byte eventCode, object content, int senderId);
}
