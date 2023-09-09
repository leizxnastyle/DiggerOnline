using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExitGames.Client.Photon;
using UnityEngine;

internal class NetworkingPeer : LoadbalancingPeer, IPhotonPeerListener
{
	public NetworkingPeer(string playername, ConnectionProtocol connectionProtocol) : base(connectionProtocol)
	{
		base.Listener = this;
		if (PhotonHandler.PingImplementation == null)
		{
			PhotonHandler.PingImplementation = typeof(PingMono);
		}
		base.LimitOfUnreliableCommands = 40;
		this.lobby = TypedLobby.Default;
		this.PlayerName = playername;
		this.mLocalActor = new PhotonPlayer(true, -1, this.playername);
		this.AddNewPlayer(this.mLocalActor.ID, this.mLocalActor);
		this.rpcShortcuts = new Dictionary<string, int>(PhotonNetwork.PhotonServerSettings.RpcList.Count);
		for (int i = 0; i < PhotonNetwork.PhotonServerSettings.RpcList.Count; i++)
		{
			string key = PhotonNetwork.PhotonServerSettings.RpcList[i];
			this.rpcShortcuts[key] = i;
		}
		this.State = global::PeerState.PeerCreated;
	}

	protected internal string mAppVersionPun
	{
		get
		{
			return string.Format("{0}_{1}", PhotonNetwork.gameVersion, "1.64.2");
		}
	}

	public AuthenticationValues CustomAuthenticationValues { get; set; }

	public string NameServerAddress
	{
		get
		{
			return this.GetNameServerAddress();
		}
	}

	public string MasterServerAddress { get; protected internal set; }

	public string mGameserver { get; internal set; }

	protected internal ServerConnection server { get; private set; }

	public PeerState State { get; internal set; }

	public bool IsUsingNameServer { get; protected internal set; }

	public bool IsAuthorizeSecretAvailable
	{
		get
		{
			return this.CustomAuthenticationValues != null && !string.IsNullOrEmpty(this.CustomAuthenticationValues.Token);
		}
	}

	public List<Region> AvailableRegions { get; protected internal set; }

	public CloudRegionCode CloudRegion { get; protected internal set; }

	private bool requestLobbyStatistics
	{
		get
		{
			return PhotonNetwork.EnableLobbyStatistics && this.server == ServerConnection.MasterServer;
		}
	}

	public TypedLobby lobby { get; set; }

	public int mPlayersOnMasterCount { get; internal set; }

	public int mGameCount { get; internal set; }

	public int mPlayersInRoomsCount { get; internal set; }

	protected internal int FriendsListAge
	{
		get
		{
			return (!this.isFetchingFriends && this.friendListTimestamp != 0) ? (Environment.TickCount - this.friendListTimestamp) : 0;
		}
	}

	public string PlayerName
	{
		get
		{
			return this.playername;
		}
		set
		{
			if (string.IsNullOrEmpty(value) || value.Equals(this.playername))
			{
				return;
			}
			if (this.mLocalActor != null)
			{
				this.mLocalActor.name = value;
			}
			this.playername = value;
			if (this.CurrentGame != null)
			{
				this.SendPlayerName();
			}
		}
	}

	public Room CurrentGame
	{
		get
		{
			if (this.mCurrentGame != null && this.mCurrentGame.isLocalClientInside)
			{
				return this.mCurrentGame;
			}
			return null;
		}
		private set
		{
			this.mCurrentGame = value;
		}
	}

	public PhotonPlayer mLocalActor { get; internal set; }

	public int mMasterClientId
	{
		get
		{
			if (PhotonNetwork.offlineMode)
			{
				return this.mLocalActor.ID;
			}
			return (this.CurrentGame != null) ? this.CurrentGame.masterClientId : 0;
		}
		private set
		{
			if (this.CurrentGame != null)
			{
				this.CurrentGame.masterClientId = value;
			}
		}
	}

	private string GetNameServerAddress()
	{
		ConnectionProtocol usedProtocol = base.UsedProtocol;
		int num = 0;
		NetworkingPeer.ProtocolToNameServerPort.TryGetValue(usedProtocol, out num);
		string arg = string.Empty;
		if (usedProtocol == ConnectionProtocol.WebSocket)
		{
			arg = "ws://";
		}
		else if (usedProtocol == ConnectionProtocol.WebSocketSecure)
		{
			arg = "wss://";
		}
		return string.Format("{0}{1}:{2}", arg, "ns.exitgames.com", num);
	}

	public override bool Connect(string serverAddress, string applicationName)
	{
		UnityEngine.Debug.LogError("Avoid using this directly. Thanks.");
		return false;
	}

	public bool Connect(string serverAddress, ServerConnection type)
	{
		if (PhotonHandler.AppQuits)
		{
			UnityEngine.Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
			return false;
		}
		if (PhotonNetwork.connectionStateDetailed == global::PeerState.Disconnecting)
		{
			UnityEngine.Debug.LogError("Connect() failed. Can't connect while disconnecting (still). Current state: " + PhotonNetwork.connectionStateDetailed);
			return false;
		}
		bool flag = base.Connect(serverAddress, string.Empty);
		if (flag)
		{
			switch (type)
			{
			case ServerConnection.MasterServer:
				this.State = global::PeerState.ConnectingToMasterserver;
				break;
			case ServerConnection.GameServer:
				this.State = global::PeerState.ConnectingToGameserver;
				break;
			case ServerConnection.NameServer:
				this.State = global::PeerState.ConnectingToNameServer;
				break;
			}
		}
		return flag;
	}

	public bool ConnectToNameServer()
	{
		if (PhotonHandler.AppQuits)
		{
			UnityEngine.Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
			return false;
		}
		this.IsUsingNameServer = true;
		this.CloudRegion = CloudRegionCode.none;
		if (this.State == global::PeerState.ConnectedToNameServer)
		{
			return true;
		}
		string nameServerAddress = this.NameServerAddress;
		if (!base.Connect(nameServerAddress, "ns"))
		{
			return false;
		}
		this.State = global::PeerState.ConnectingToNameServer;
		return true;
	}

	public bool ConnectToRegionMaster(CloudRegionCode region)
	{
		if (PhotonHandler.AppQuits)
		{
			UnityEngine.Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
			return false;
		}
		this.IsUsingNameServer = true;
		this.CloudRegion = region;
		if (this.State == global::PeerState.ConnectedToNameServer)
		{
			AuthenticationValues authenticationValues;
			if ((authenticationValues = this.CustomAuthenticationValues) == null)
			{
				authenticationValues = new AuthenticationValues
				{
					UserId = this.PlayerName
				};
			}
			AuthenticationValues authValues = authenticationValues;
			return this.OpAuthenticate(this.mAppId, this.mAppVersionPun, authValues, region.ToString(), this.requestLobbyStatistics);
		}
		string nameServerAddress = this.NameServerAddress;
		if (!base.Connect(nameServerAddress, "ns"))
		{
			return false;
		}
		this.State = global::PeerState.ConnectingToNameServer;
		return true;
	}

	public bool GetRegions()
	{
		if (this.server != ServerConnection.NameServer)
		{
			return false;
		}
		bool flag = this.OpGetRegions(this.mAppId);
		if (flag)
		{
			this.AvailableRegions = null;
		}
		return flag;
	}

	public override void Disconnect()
	{
		if (base.PeerState == PeerStateValue.Disconnected)
		{
			if (!PhotonHandler.AppQuits)
			{
				UnityEngine.Debug.LogWarning(string.Format("Can't execute Disconnect() while not connected. Nothing changed. State: {0}", this.State));
			}
			return;
		}
		this.State = global::PeerState.Disconnecting;
		base.Disconnect();
	}

	private void DisconnectToReconnect()
	{
		switch (this.server)
		{
		case ServerConnection.MasterServer:
			this.State = global::PeerState.DisconnectingFromMasterserver;
			base.Disconnect();
			break;
		case ServerConnection.GameServer:
			this.State = global::PeerState.DisconnectingFromGameserver;
			base.Disconnect();
			break;
		case ServerConnection.NameServer:
			this.State = global::PeerState.DisconnectingFromNameServer;
			base.Disconnect();
			break;
		}
	}

	private void LeftLobbyCleanup()
	{
		this.mGameList = new Dictionary<string, RoomInfo>();
		this.mGameListCopy = new RoomInfo[0];
		if (this.insideLobby)
		{
			this.insideLobby = false;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftLobby, new object[0]);
		}
	}

	private void LeftRoomCleanup()
	{
		bool flag = this.CurrentGame != null;
		bool flag2 = (this.CurrentGame == null) ? PhotonNetwork.autoCleanUpPlayerObjects : this.CurrentGame.autoCleanUp;
		this.hasSwitchedMC = false;
		this.CurrentGame = null;
		this.mActors = new Dictionary<int, PhotonPlayer>();
		this.mPlayerListCopy = new PhotonPlayer[0];
		this.mOtherPlayerListCopy = new PhotonPlayer[0];
		this.allowedReceivingGroups = new HashSet<int>();
		this.blockSendingGroups = new HashSet<int>();
		this.mGameList = new Dictionary<string, RoomInfo>();
		this.mGameListCopy = new RoomInfo[0];
		this.isFetchingFriends = false;
		this.ChangeLocalID(-1);
		if (flag2)
		{
			this.LocalCleanupAnythingInstantiated(true);
			PhotonNetwork.manuallyAllocatedViewIds = new List<int>();
		}
		if (flag)
		{
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom, new object[0]);
		}
	}

	protected internal void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
	{
		if (this.tempInstantiationData.Count > 0)
		{
			UnityEngine.Debug.LogWarning("It seems some instantiation is not completed, as instantiation data is used. You should make sure instantiations are paused when calling this method. Cleaning now, despite this.");
		}
		if (destroyInstantiatedGameObjects)
		{
			HashSet<GameObject> hashSet = new HashSet<GameObject>();
			foreach (PhotonView photonView in this.photonViewList.Values)
			{
				if (photonView.isRuntimeInstantiated)
				{
					hashSet.Add(photonView.gameObject);
				}
			}
			foreach (GameObject go in hashSet)
			{
				this.RemoveInstantiatedGO(go, true);
			}
		}
		this.tempInstantiationData.Clear();
		PhotonNetwork.lastUsedViewSubId = 0;
		PhotonNetwork.lastUsedViewSubIdStatic = 0;
	}

	private void ReadoutProperties(ExitGames.Client.Photon.Hashtable gameProperties, ExitGames.Client.Photon.Hashtable pActorProperties, int targetActorNr)
	{
		if (pActorProperties != null && pActorProperties.Count > 0)
		{
			if (targetActorNr > 0)
			{
				PhotonPlayer playerWithId = this.GetPlayerWithId(targetActorNr);
				if (playerWithId != null)
				{
					ExitGames.Client.Photon.Hashtable actorPropertiesForActorNr = this.GetActorPropertiesForActorNr(pActorProperties, targetActorNr);
					playerWithId.InternalCacheProperties(actorPropertiesForActorNr);
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[]
					{
						playerWithId,
						actorPropertiesForActorNr
					});
				}
			}
			else
			{
				foreach (object obj in pActorProperties.Keys)
				{
					int num = (int)obj;
					ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)pActorProperties[obj];
					string name = (string)hashtable[byte.MaxValue];
					PhotonPlayer photonPlayer = this.GetPlayerWithId(num);
					if (photonPlayer == null)
					{
						photonPlayer = new PhotonPlayer(false, num, name);
						this.AddNewPlayer(num, photonPlayer);
					}
					photonPlayer.InternalCacheProperties(hashtable);
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[]
					{
						photonPlayer,
						hashtable
					});
				}
			}
		}
		if (this.CurrentGame != null && gameProperties != null)
		{
			this.CurrentGame.InternalCacheProperties(gameProperties);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, new object[]
			{
				gameProperties
			});
			if (PhotonNetwork.automaticallySyncScene)
			{
				this.LoadLevelIfSynced();
			}
		}
	}

	private void AddNewPlayer(int ID, PhotonPlayer player)
	{
		if (!this.mActors.ContainsKey(ID))
		{
			this.mActors[ID] = player;
			this.RebuildPlayerListCopies();
		}
		else
		{
			UnityEngine.Debug.LogError("Adding player twice: " + ID);
		}
	}

	private void RemovePlayer(int ID, PhotonPlayer player)
	{
		this.mActors.Remove(ID);
		if (!player.isLocal)
		{
			this.RebuildPlayerListCopies();
		}
	}

	private void RebuildPlayerListCopies()
	{
		this.mPlayerListCopy = new PhotonPlayer[this.mActors.Count];
		this.mActors.Values.CopyTo(this.mPlayerListCopy, 0);
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		foreach (PhotonPlayer photonPlayer in this.mPlayerListCopy)
		{
			if (!photonPlayer.isLocal)
			{
				list.Add(photonPlayer);
			}
		}
		this.mOtherPlayerListCopy = list.ToArray();
	}

	private void ResetPhotonViewsOnSerialize()
	{
		foreach (PhotonView photonView in this.photonViewList.Values)
		{
			photonView.lastOnSerializeDataSent = null;
		}
	}

	private void HandleEventLeave(int actorID, EventData evLeave)
	{
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			UnityEngine.Debug.Log("HandleEventLeave for player ID: " + actorID);
		}
		if (actorID < 0 || !this.mActors.ContainsKey(actorID))
		{
			UnityEngine.Debug.LogError(string.Format("Received event Leave for unknown player ID: {0}", actorID));
			return;
		}
		PhotonPlayer playerWithId = this.GetPlayerWithId(actorID);
		if (playerWithId == null)
		{
			UnityEngine.Debug.LogError("HandleEventLeave for player ID: " + actorID + " has no PhotonPlayer!");
		}
		if (evLeave.Parameters.ContainsKey(203))
		{
			int num = (int)evLeave[203];
			if (num != 0)
			{
				this.mMasterClientId = (int)evLeave[203];
				this.UpdateMasterClient();
			}
		}
		else if (!this.CurrentGame.serverSideMasterClient)
		{
			this.CheckMasterClient(actorID);
		}
		if (this.CurrentGame != null && this.CurrentGame.autoCleanUp)
		{
			this.DestroyPlayerObjects(actorID, true);
		}
		this.RemovePlayer(actorID, playerWithId);
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, new object[]
		{
			playerWithId
		});
	}

	private void CheckMasterClient(int leavingPlayerId)
	{
		bool flag = this.mMasterClientId == leavingPlayerId;
		bool flag2 = leavingPlayerId > 0;
		if (flag2 && !flag)
		{
			return;
		}
		int num;
		if (this.mActors.Count <= 1)
		{
			num = this.mLocalActor.ID;
		}
		else
		{
			num = int.MaxValue;
			foreach (int num2 in this.mActors.Keys)
			{
				if (num2 < num && num2 != leavingPlayerId)
				{
					num = num2;
				}
			}
		}
		this.mMasterClientId = num;
		if (flag2)
		{
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, new object[]
			{
				this.GetPlayerWithId(num)
			});
		}
	}

	protected internal void UpdateMasterClient()
	{
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, new object[]
		{
			PhotonNetwork.masterClient
		});
	}

	private static int ReturnLowestPlayerId(PhotonPlayer[] players, int playerIdToIgnore)
	{
		if (players == null || players.Length == 0)
		{
			return -1;
		}
		int num = int.MaxValue;
		foreach (PhotonPlayer photonPlayer in players)
		{
			if (photonPlayer.ID != playerIdToIgnore)
			{
				if (photonPlayer.ID < num)
				{
					num = photonPlayer.ID;
				}
			}
		}
		return num;
	}

	protected internal bool SetMasterClient(int playerId, bool sync)
	{
		bool flag = this.mMasterClientId != playerId;
		if (!flag || !this.mActors.ContainsKey(playerId))
		{
			return false;
		}
		if (sync && !this.OpRaiseEvent(208, new ExitGames.Client.Photon.Hashtable
		{
			{
				1,
				playerId
			}
		}, true, null))
		{
			return false;
		}
		this.hasSwitchedMC = true;
		this.CurrentGame.masterClientId = playerId;
		NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, new object[]
		{
			this.GetPlayerWithId(playerId)
		});
		return true;
	}

	public bool SetMasterClient(int nextMasterId)
	{
		ExitGames.Client.Photon.Hashtable gameProperties = new ExitGames.Client.Photon.Hashtable
		{
			{
				248,
				nextMasterId
			}
		};
		ExitGames.Client.Photon.Hashtable expectedProperties = new ExitGames.Client.Photon.Hashtable
		{
			{
				248,
				this.mMasterClientId
			}
		};
		return base.OpSetPropertiesOfRoom(gameProperties, expectedProperties, false);
	}

	private ExitGames.Client.Photon.Hashtable GetActorPropertiesForActorNr(ExitGames.Client.Photon.Hashtable actorProperties, int actorNr)
	{
		if (actorProperties.ContainsKey(actorNr))
		{
			return (ExitGames.Client.Photon.Hashtable)actorProperties[actorNr];
		}
		return actorProperties;
	}

	protected internal PhotonPlayer GetPlayerWithId(int number)
	{
		if (this.mActors == null)
		{
			return null;
		}
		PhotonPlayer result = null;
		this.mActors.TryGetValue(number, out result);
		return result;
	}

	private void SendPlayerName()
	{
		if (this.State == global::PeerState.Joining)
		{
			this.mPlayernameHasToBeUpdated = true;
			return;
		}
		if (this.mLocalActor != null)
		{
			this.mLocalActor.name = this.PlayerName;
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable[byte.MaxValue] = this.PlayerName;
			if (this.mLocalActor.ID > 0)
			{
				base.OpSetPropertiesOfActor(this.mLocalActor.ID, hashtable, null, false);
				this.mPlayernameHasToBeUpdated = false;
			}
		}
	}

	private void GameEnteredOnGameServer(OperationResponse operationResponse)
	{
		if (operationResponse.ReturnCode != 0)
		{
			switch (operationResponse.OperationCode)
			{
			case 225:
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					UnityEngine.Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
					if (operationResponse.ReturnCode == 32758)
					{
						UnityEngine.Debug.Log("Most likely the game became empty during the switch to GameServer.");
					}
				}
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, new object[]
				{
					operationResponse.ReturnCode,
					operationResponse.DebugMessage
				});
				break;
			case 226:
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					UnityEngine.Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
					if (operationResponse.ReturnCode == 32758)
					{
						UnityEngine.Debug.Log("Most likely the game became empty during the switch to GameServer.");
					}
				}
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, new object[]
				{
					operationResponse.ReturnCode,
					operationResponse.DebugMessage
				});
				break;
			case 227:
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					UnityEngine.Debug.Log("Create failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
				}
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, new object[]
				{
					operationResponse.ReturnCode,
					operationResponse.DebugMessage
				});
				break;
			}
			this.DisconnectToReconnect();
			return;
		}
		this.CurrentGame = new Room(this.enterRoomParamsCache.RoomName, null)
		{
			isLocalClientInside = true
		};
		this.State = global::PeerState.Joined;
		if (operationResponse.Parameters.ContainsKey(252))
		{
			int[] actorsInRoom = (int[])operationResponse.Parameters[252];
			this.UpdatedActorList(actorsInRoom);
		}
		int newID = (int)operationResponse[254];
		this.ChangeLocalID(newID);
		ExitGames.Client.Photon.Hashtable pActorProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[249];
		ExitGames.Client.Photon.Hashtable gameProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[248];
		this.ReadoutProperties(gameProperties, pActorProperties, 0);
		if (!this.CurrentGame.serverSideMasterClient)
		{
			this.CheckMasterClient(-1);
		}
		if (this.mPlayernameHasToBeUpdated)
		{
			this.SendPlayerName();
		}
		switch (operationResponse.OperationCode)
		{
		case 227:
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
			break;
		}
	}

	private ExitGames.Client.Photon.Hashtable GetLocalActorProperties()
	{
		if (PhotonNetwork.player != null)
		{
			return PhotonNetwork.player.allProperties;
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[byte.MaxValue] = this.PlayerName;
		return hashtable;
	}

	public void ChangeLocalID(int newID)
	{
		if (this.mLocalActor == null)
		{
			UnityEngine.Debug.LogWarning(string.Format("Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}", this.mLocalActor, this.mActors == null, newID));
		}
		if (this.mActors.ContainsKey(this.mLocalActor.ID))
		{
			this.mActors.Remove(this.mLocalActor.ID);
		}
		this.mLocalActor.InternalChangeLocalID(newID);
		this.mActors[this.mLocalActor.ID] = this.mLocalActor;
		this.RebuildPlayerListCopies();
	}

	public bool OpCreateGame(LoadbalancingPeer.EnterRoomParams enterRoomParams)
	{
		bool flag = this.server == ServerConnection.GameServer;
		enterRoomParams.OnGameServer = flag;
		enterRoomParams.PlayerProperties = this.GetLocalActorProperties();
		if (!flag)
		{
			this.enterRoomParamsCache = enterRoomParams;
		}
		this.mLastJoinType = JoinType.CreateGame;
		return base.OpCreateRoom(enterRoomParams);
	}

	public override bool OpJoinRoom(LoadbalancingPeer.EnterRoomParams opParams)
	{
		bool flag = this.server == ServerConnection.GameServer;
		opParams.OnGameServer = flag;
		if (!flag)
		{
			this.enterRoomParamsCache = opParams;
		}
		this.mLastJoinType = ((!opParams.CreateIfNotExists) ? JoinType.JoinGame : JoinType.JoinOrCreateOnDemand);
		return base.OpJoinRoom(opParams);
	}

	public override bool OpJoinRandomRoom(LoadbalancingPeer.OpJoinRandomRoomParams opJoinRandomRoomParams)
	{
		this.enterRoomParamsCache = new LoadbalancingPeer.EnterRoomParams();
		this.enterRoomParamsCache.Lobby = opJoinRandomRoomParams.TypedLobby;
		this.mLastJoinType = JoinType.JoinRandomGame;
		return base.OpJoinRandomRoom(opJoinRandomRoomParams);
	}

	public virtual bool OpLeave()
	{
		if (this.State != global::PeerState.Joined)
		{
			UnityEngine.Debug.LogWarning("Not sending leave operation. State is not 'Joined': " + this.State);
			return false;
		}
		return this.OpCustom(254, null, true, 0);
	}

	public override bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
	{
		return !PhotonNetwork.offlineMode && base.OpRaiseEvent(eventCode, customEventContent, sendReliable, raiseEventOptions);
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		if (level == DebugLevel.ERROR)
		{
			UnityEngine.Debug.LogError(message);
		}
		else if (level == DebugLevel.WARNING)
		{
			UnityEngine.Debug.LogWarning(message);
		}
		else if (level == DebugLevel.INFO && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			UnityEngine.Debug.Log(message);
		}
		else if (level == DebugLevel.ALL && PhotonNetwork.logLevel == PhotonLogLevel.Full)
		{
			UnityEngine.Debug.Log(message);
		}
	}

	public void OnOperationResponse(OperationResponse operationResponse)
	{
		if (PhotonNetwork.networkingPeer.State == global::PeerState.Disconnecting)
		{
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				UnityEngine.Debug.Log("OperationResponse ignored while disconnecting. Code: " + operationResponse.OperationCode);
			}
			return;
		}
		if (operationResponse.ReturnCode == 0)
		{
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				UnityEngine.Debug.Log(operationResponse.ToString());
			}
		}
		else if (operationResponse.ReturnCode == -3)
		{
			UnityEngine.Debug.LogError("Operation " + operationResponse.OperationCode + " could not be executed (yet). Wait for state JoinedLobby or ConnectedToMaster and their callbacks before calling operations. WebRPCs need a server-side configuration. Enum OperationCode helps identify the operation.");
		}
		else if (operationResponse.ReturnCode == 32752)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Operation ",
				operationResponse.OperationCode,
				" failed in a server-side plugin. Check the configuration in the Dashboard. Message from server-plugin: ",
				operationResponse.DebugMessage
			}));
		}
		else if (operationResponse.ReturnCode == 32760)
		{
			UnityEngine.Debug.LogWarning("Operation failed: " + operationResponse.ToStringFull());
		}
		else
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Operation failed: ",
				operationResponse.ToStringFull(),
				" Server: ",
				this.server
			}));
		}
		if (operationResponse.Parameters.ContainsKey(221))
		{
			if (this.CustomAuthenticationValues == null)
			{
				this.CustomAuthenticationValues = new AuthenticationValues();
			}
			this.CustomAuthenticationValues.Token = (operationResponse[221] as string);
		}
		byte operationCode = operationResponse.OperationCode;
		switch (operationCode)
		{
		case 219:
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnWebRpcResponse, new object[]
			{
				operationResponse
			});
			break;
		case 220:
			if (operationResponse.ReturnCode == 32767)
			{
				UnityEngine.Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account.", new object[0]));
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, new object[]
				{
					DisconnectCause.InvalidAuthentication
				});
				this.State = global::PeerState.Disconnecting;
				this.Disconnect();
			}
			else if (operationResponse.ReturnCode != 0)
			{
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"GetRegions failed. Can't provide regions list. Error: ",
					operationResponse.ReturnCode,
					": ",
					operationResponse.DebugMessage
				}));
			}
			else
			{
				string[] array = operationResponse[210] as string[];
				string[] array2 = operationResponse[230] as string[];
				if (array == null || array2 == null || array.Length != array2.Length)
				{
					UnityEngine.Debug.LogError(string.Concat(new object[]
					{
						"The region arrays from Name Server are not ok. Must be non-null and same length. ",
						array == null,
						" ",
						array2 == null,
						"\n",
						operationResponse.ToStringFull()
					}));
				}
				else
				{
					this.AvailableRegions = new List<Region>(array.Length);
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i];
						if (!string.IsNullOrEmpty(text))
						{
							text = text.ToLower();
							CloudRegionCode cloudRegionCode = Region.Parse(text);
							bool flag = true;
							if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion && PhotonNetwork.PhotonServerSettings.EnabledRegions != (CloudRegionFlag)0)
							{
								CloudRegionFlag cloudRegionFlag = Region.ParseFlag(text);
								flag = ((PhotonNetwork.PhotonServerSettings.EnabledRegions & cloudRegionFlag) != (CloudRegionFlag)0);
								if (!flag && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
								{
									UnityEngine.Debug.Log("Skipping region because it's not in PhotonServerSettings.EnabledRegions: " + cloudRegionCode);
								}
							}
							if (flag)
							{
								this.AvailableRegions.Add(new Region
								{
									Code = cloudRegionCode,
									HostAndPort = array2[i]
								});
							}
						}
					}
					if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
					{
						PhotonHandler.PingAvailableRegionsAndConnectToBest();
					}
				}
			}
			break;
		default:
			switch (operationCode)
			{
			case 251:
			{
				ExitGames.Client.Photon.Hashtable pActorProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[249];
				ExitGames.Client.Photon.Hashtable gameProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[248];
				this.ReadoutProperties(gameProperties, pActorProperties, 0);
				break;
			}
			case 252:
				break;
			case 253:
				break;
			case 254:
				this.DisconnectToReconnect();
				break;
			default:
				UnityEngine.Debug.LogWarning(string.Format("OperationResponse unhandled: {0}", operationResponse.ToString()));
				break;
			}
			break;
		case 222:
		{
			bool[] array3 = operationResponse[1] as bool[];
			string[] array4 = operationResponse[2] as string[];
			if (array3 != null && array4 != null && this.friendListRequested != null && array3.Length == this.friendListRequested.Length)
			{
				List<FriendInfo> list = new List<FriendInfo>(this.friendListRequested.Length);
				for (int j = 0; j < this.friendListRequested.Length; j++)
				{
					list.Insert(j, new FriendInfo
					{
						Name = this.friendListRequested[j],
						Room = array4[j],
						IsOnline = array3[j]
					});
				}
				PhotonNetwork.Friends = list;
			}
			else
			{
				UnityEngine.Debug.LogError("FindFriends failed to apply the result, as a required value wasn't provided or the friend list length differed from result.");
			}
			this.friendListRequested = null;
			this.isFetchingFriends = false;
			this.friendListTimestamp = Environment.TickCount;
			if (this.friendListTimestamp == 0)
			{
				this.friendListTimestamp = 1;
			}
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnUpdatedFriendList, new object[0]);
			break;
		}
		case 225:
			if (operationResponse.ReturnCode != 0)
			{
				if (operationResponse.ReturnCode == 32760)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
					{
						UnityEngine.Debug.Log("JoinRandom failed: No open game. Calling: OnPhotonRandomJoinFailed() and staying on master server.");
					}
				}
				else if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					UnityEngine.Debug.LogWarning(string.Format("JoinRandom failed: {0}.", operationResponse.ToStringFull()));
				}
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, new object[]
				{
					operationResponse.ReturnCode,
					operationResponse.DebugMessage
				});
			}
			else
			{
				string roomName = (string)operationResponse[byte.MaxValue];
				this.enterRoomParamsCache.RoomName = roomName;
				this.mGameserver = (string)operationResponse[230];
				this.DisconnectToReconnect();
			}
			break;
		case 226:
			if (this.server != ServerConnection.GameServer)
			{
				if (operationResponse.ReturnCode != 0)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						UnityEngine.Debug.Log(string.Format("JoinRoom failed (room maybe closed by now). Client stays on masterserver: {0}. State: {1}", operationResponse.ToStringFull(), this.State));
					}
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, new object[]
					{
						operationResponse.ReturnCode,
						operationResponse.DebugMessage
					});
				}
				else
				{
					this.mGameserver = (string)operationResponse[230];
					this.DisconnectToReconnect();
				}
			}
			else
			{
				this.GameEnteredOnGameServer(operationResponse);
			}
			break;
		case 227:
			if (this.server == ServerConnection.GameServer)
			{
				this.GameEnteredOnGameServer(operationResponse);
			}
			else if (operationResponse.ReturnCode != 0)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					UnityEngine.Debug.LogWarning(string.Format("CreateRoom failed, client stays on masterserver: {0}.", operationResponse.ToStringFull()));
				}
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, new object[]
				{
					operationResponse.ReturnCode,
					operationResponse.DebugMessage
				});
			}
			else
			{
				string text2 = (string)operationResponse[byte.MaxValue];
				if (!string.IsNullOrEmpty(text2))
				{
					this.enterRoomParamsCache.RoomName = text2;
				}
				this.mGameserver = (string)operationResponse[230];
				this.DisconnectToReconnect();
			}
			break;
		case 228:
			this.State = global::PeerState.Authenticated;
			this.LeftLobbyCleanup();
			break;
		case 229:
			this.State = global::PeerState.JoinedLobby;
			this.insideLobby = true;
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedLobby, new object[0]);
			break;
		case 230:
			if (operationResponse.ReturnCode != 0)
			{
				if (operationResponse.ReturnCode == -2)
				{
					UnityEngine.Debug.LogError(string.Format("If you host Photon yourself, make sure to start the 'Instance LoadBalancing' " + base.ServerAddress, new object[0]));
				}
				else if (operationResponse.ReturnCode == 32767)
				{
					UnityEngine.Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account.", new object[0]));
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, new object[]
					{
						DisconnectCause.InvalidAuthentication
					});
				}
				else if (operationResponse.ReturnCode == 32755)
				{
					UnityEngine.Debug.LogError(string.Format("Custom Authentication failed (either due to user-input or configuration or AuthParameter string format). Calling: OnCustomAuthenticationFailed()", new object[0]));
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationFailed, new object[]
					{
						operationResponse.DebugMessage
					});
				}
				else
				{
					UnityEngine.Debug.LogError(string.Format("Authentication failed: '{0}' Code: {1}", operationResponse.DebugMessage, operationResponse.ReturnCode));
				}
				this.State = global::PeerState.Disconnecting;
				this.Disconnect();
				if (operationResponse.ReturnCode == 32757)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						UnityEngine.Debug.LogWarning(string.Format("Currently, the limit of users is reached for this title. Try again later. Disconnecting", new object[0]));
					}
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonMaxCccuReached, new object[0]);
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, new object[]
					{
						DisconnectCause.MaxCcuReached
					});
				}
				else if (operationResponse.ReturnCode == 32756)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						UnityEngine.Debug.LogError(string.Format("The used master server address is not available with the subscription currently used. Got to Photon Cloud Dashboard or change URL. Disconnecting.", new object[0]));
					}
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, new object[]
					{
						DisconnectCause.InvalidRegion
					});
				}
				else if (operationResponse.ReturnCode == 32753)
				{
					if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
					{
						UnityEngine.Debug.LogError(string.Format("The authentication ticket expired. You need to connect (and authenticate) again. Disconnecting.", new object[0]));
					}
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, new object[]
					{
						DisconnectCause.AuthenticationTicketExpired
					});
				}
			}
			else if (this.server == ServerConnection.NameServer)
			{
				this.MasterServerAddress = (operationResponse[230] as string);
				this.DisconnectToReconnect();
			}
			else if (this.server == ServerConnection.MasterServer)
			{
				if (PhotonNetwork.autoJoinLobby)
				{
					this.State = global::PeerState.Authenticated;
					this.OpJoinLobby(this.lobby);
				}
				else
				{
					this.State = global::PeerState.ConnectedToMaster;
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster, new object[0]);
				}
			}
			else if (this.server == ServerConnection.GameServer)
			{
				this.State = global::PeerState.Joining;
				this.enterRoomParamsCache.PlayerProperties = this.GetLocalActorProperties();
				this.enterRoomParamsCache.OnGameServer = true;
				if (this.mLastJoinType == JoinType.JoinGame || this.mLastJoinType == JoinType.JoinRandomGame || this.mLastJoinType == JoinType.JoinOrCreateOnDemand)
				{
					this.OpJoinRoom(this.enterRoomParamsCache);
				}
				else if (this.mLastJoinType == JoinType.CreateGame)
				{
					this.OpCreateGame(this.enterRoomParamsCache);
				}
			}
			break;
		}
	}

	public override bool OpFindFriends(string[] friendsToFind)
	{
		if (this.isFetchingFriends)
		{
			return false;
		}
		this.friendListRequested = friendsToFind;
		this.isFetchingFriends = true;
		return base.OpFindFriends(friendsToFind);
	}

	public void OnStatusChanged(StatusCode statusCode)
	{
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			UnityEngine.Debug.Log(string.Format("OnStatusChanged: {0}", statusCode.ToString()));
		}
		switch (statusCode)
		{
		case StatusCode.SecurityExceptionOnConnect:
		case StatusCode.ExceptionOnConnect:
			this.State = global::PeerState.PeerCreated;
			if (this.CustomAuthenticationValues != null)
			{
				this.CustomAuthenticationValues.Token = null;
			}
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, new object[]
			{
				(DisconnectCause)statusCode
			});
			return;
		case StatusCode.Connect:
			if (this.State == global::PeerState.ConnectingToNameServer)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
				{
					UnityEngine.Debug.Log("Connected to NameServer.");
				}
				this.server = ServerConnection.NameServer;
				if (this.CustomAuthenticationValues != null)
				{
					this.CustomAuthenticationValues.Token = null;
				}
			}
			if (this.State == global::PeerState.ConnectingToGameserver)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
				{
					UnityEngine.Debug.Log("Connected to gameserver.");
				}
				this.server = ServerConnection.GameServer;
				this.State = global::PeerState.ConnectedToGameserver;
			}
			if (this.State == global::PeerState.ConnectingToMasterserver)
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
				{
					UnityEngine.Debug.Log("Connected to masterserver.");
				}
				this.server = ServerConnection.MasterServer;
				this.State = global::PeerState.Authenticating;
				if (this.IsInitialConnect)
				{
					this.IsInitialConnect = false;
					NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToPhoton, new object[0]);
				}
			}
			if (!base.IsProtocolSecure)
			{
				base.EstablishEncryption();
			}
			else
			{
				UnityEngine.Debug.Log("Skipping EstablishEncryption. Protocol is secure.");
			}
			if (this.IsAuthorizeSecretAvailable || base.IsProtocolSecure)
			{
				AuthenticationValues authenticationValues;
				if ((authenticationValues = this.CustomAuthenticationValues) == null)
				{
					authenticationValues = new AuthenticationValues
					{
						UserId = this.PlayerName
					};
				}
				AuthenticationValues authValues = authenticationValues;
				this.didAuthenticate = this.OpAuthenticate(this.mAppId, this.mAppVersionPun, authValues, this.CloudRegion.ToString(), this.requestLobbyStatistics);
				if (this.didAuthenticate)
				{
					this.State = global::PeerState.Authenticating;
				}
			}
			return;
		case StatusCode.Disconnect:
			this.didAuthenticate = false;
			this.isFetchingFriends = false;
			if (this.server == ServerConnection.GameServer)
			{
				this.LeftRoomCleanup();
			}
			if (this.server == ServerConnection.MasterServer)
			{
				this.LeftLobbyCleanup();
			}
			if (this.State == global::PeerState.DisconnectingFromMasterserver)
			{
				if (this.Connect(this.mGameserver, ServerConnection.GameServer))
				{
					this.State = global::PeerState.ConnectingToGameserver;
				}
			}
			else if (this.State == global::PeerState.DisconnectingFromGameserver || this.State == global::PeerState.DisconnectingFromNameServer)
			{
				if (this.Connect(this.MasterServerAddress, ServerConnection.MasterServer))
				{
					this.State = global::PeerState.ConnectingToMasterserver;
				}
			}
			else
			{
				if (this.CustomAuthenticationValues != null)
				{
					this.CustomAuthenticationValues.Token = null;
				}
				this.State = global::PeerState.PeerCreated;
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnDisconnectedFromPhoton, new object[0]);
			}
			return;
		case StatusCode.Exception:
			if (this.IsInitialConnect)
			{
				UnityEngine.Debug.LogError("Exception while connecting to: " + base.ServerAddress + ". Check if the server is available.");
				if (base.ServerAddress == null || base.ServerAddress.StartsWith("127.0.0.1"))
				{
					UnityEngine.Debug.LogWarning("The server address is 127.0.0.1 (localhost): Make sure the server is running on this machine. Android and iOS emulators have their own localhost.");
					if (base.ServerAddress == this.mGameserver)
					{
						UnityEngine.Debug.LogWarning("This might be a misconfiguration in the game server config. You need to edit it to a (public) address.");
					}
				}
				this.State = global::PeerState.PeerCreated;
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, new object[]
				{
					(DisconnectCause)statusCode
				});
			}
			else
			{
				this.State = global::PeerState.PeerCreated;
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, new object[]
				{
					(DisconnectCause)statusCode
				});
			}
			this.Disconnect();
			return;
		case StatusCode.QueueOutgoingReliableWarning:
		case StatusCode.QueueOutgoingUnreliableWarning:
		case StatusCode.QueueOutgoingAcksWarning:
		case StatusCode.QueueSentWarning:
			return;
		case StatusCode.SendError:
			return;
		case StatusCode.QueueIncomingReliableWarning:
		case StatusCode.QueueIncomingUnreliableWarning:
			UnityEngine.Debug.Log(statusCode + ". This client buffers many incoming messages. This is OK temporarily. With lots of these warnings, check if you send too much or execute messages too slow. " + ((!PhotonNetwork.isMessageQueueRunning) ? "Your isMessageQueueRunning is false. This can cause the issue temporarily." : string.Empty));
			return;
		case StatusCode.ExceptionOnReceive:
		case StatusCode.TimeoutDisconnect:
		case StatusCode.DisconnectByServer:
		case StatusCode.DisconnectByServerUserLimit:
		case StatusCode.DisconnectByServerLogic:
			if (this.IsInitialConnect)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					statusCode,
					" while connecting to: ",
					base.ServerAddress,
					". Check if the server is available."
				}));
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, new object[]
				{
					(DisconnectCause)statusCode
				});
			}
			else
			{
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, new object[]
				{
					(DisconnectCause)statusCode
				});
			}
			if (this.CustomAuthenticationValues != null)
			{
				this.CustomAuthenticationValues.Token = null;
			}
			this.Disconnect();
			return;
		case StatusCode.EncryptionEstablished:
			if (this.server == ServerConnection.NameServer)
			{
				this.State = global::PeerState.ConnectedToNameServer;
				if (!this.didAuthenticate && this.CloudRegion == CloudRegionCode.none)
				{
					this.OpGetRegions(this.mAppId);
				}
			}
			if (!this.didAuthenticate && (!this.IsUsingNameServer || this.CloudRegion != CloudRegionCode.none))
			{
				AuthenticationValues authenticationValues2;
				if ((authenticationValues2 = this.CustomAuthenticationValues) == null)
				{
					authenticationValues2 = new AuthenticationValues
					{
						UserId = this.PlayerName
					};
				}
				AuthenticationValues authValues2 = authenticationValues2;
				this.didAuthenticate = this.OpAuthenticate(this.mAppId, this.mAppVersionPun, authValues2, this.CloudRegion.ToString(), this.requestLobbyStatistics);
				if (this.didAuthenticate)
				{
					this.State = global::PeerState.Authenticating;
				}
			}
			return;
		case StatusCode.EncryptionFailedToEstablish:
		{
			UnityEngine.Debug.LogError("Encryption wasn't established: " + statusCode + ". Going to authenticate anyways.");
			AuthenticationValues authenticationValues3;
			if ((authenticationValues3 = this.CustomAuthenticationValues) == null)
			{
				authenticationValues3 = new AuthenticationValues
				{
					UserId = this.PlayerName
				};
			}
			AuthenticationValues authValues3 = authenticationValues3;
			this.OpAuthenticate(this.mAppId, this.mAppVersionPun, authValues3, this.CloudRegion.ToString(), this.requestLobbyStatistics);
			return;
		}
		}
		UnityEngine.Debug.LogError("Received unknown status code: " + statusCode);
	}

	public void OnEvent(EventData photonEvent)
	{
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			UnityEngine.Debug.Log(string.Format("OnEvent: {0}", photonEvent.ToString()));
		}
		int num = -1;
		PhotonPlayer photonPlayer = null;
		if (photonEvent.Parameters.ContainsKey(254))
		{
			num = (int)photonEvent[254];
			photonPlayer = this.GetPlayerWithId(num);
		}
		byte code = photonEvent.Code;
		switch (code)
		{
		case 200:
			this.ExecuteRpc(photonEvent[245] as ExitGames.Client.Photon.Hashtable, photonPlayer);
			break;
		case 201:
		case 206:
		{
			ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int networkTime = (int)hashtable[0];
			short correctPrefix = -1;
			short num2 = 1;
			if (hashtable.ContainsKey(1))
			{
				correctPrefix = (short)hashtable[1];
				num2 = 2;
			}
			short num3 = num2;
			while ((int)num3 < hashtable.Count)
			{
				this.OnSerializeRead(hashtable[num3] as ExitGames.Client.Photon.Hashtable, photonPlayer, networkTime, correctPrefix);
				num3 += 1;
			}
			break;
		}
		case 202:
			this.DoInstantiate((ExitGames.Client.Photon.Hashtable)photonEvent[245], photonPlayer, null);
			break;
		case 203:
			if (photonPlayer == null || !photonPlayer.isMasterClient)
			{
				UnityEngine.Debug.LogError("Error: Someone else(" + photonPlayer + ") then the masterserver requests a disconnect!");
			}
			else
			{
				PhotonNetwork.LeaveRoom();
			}
			break;
		case 204:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int num4 = (int)hashtable2[0];
			PhotonView photonView = null;
			if (this.photonViewList.TryGetValue(num4, out photonView))
			{
				this.RemoveInstantiatedGO(photonView.gameObject, true);
			}
			else if (base.DebugOut >= DebugLevel.ERROR)
			{
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"Ev Destroy Failed. Could not find PhotonView with instantiationId ",
					num4,
					". Sent by actorNr: ",
					num
				}));
			}
			break;
		}
		default:
			switch (code)
			{
			case 224:
			{
				string[] array = photonEvent[213] as string[];
				byte[] array2 = photonEvent[212] as byte[];
				int[] array3 = photonEvent[229] as int[];
				int[] array4 = photonEvent[228] as int[];
				this.LobbyStatistics.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					TypedLobbyInfo typedLobbyInfo = new TypedLobbyInfo();
					typedLobbyInfo.Name = array[i];
					typedLobbyInfo.Type = (LobbyType)array2[i];
					typedLobbyInfo.PlayerCount = array3[i];
					typedLobbyInfo.RoomCount = array4[i];
					this.LobbyStatistics.Add(typedLobbyInfo);
				}
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLobbyStatisticsUpdate, new object[0]);
				break;
			}
			default:
				switch (code)
				{
				case 253:
				{
					int num5 = (int)photonEvent[253];
					ExitGames.Client.Photon.Hashtable gameProperties = null;
					ExitGames.Client.Photon.Hashtable pActorProperties = null;
					if (num5 == 0)
					{
						gameProperties = (ExitGames.Client.Photon.Hashtable)photonEvent[251];
					}
					else
					{
						pActorProperties = (ExitGames.Client.Photon.Hashtable)photonEvent[251];
					}
					this.ReadoutProperties(gameProperties, pActorProperties, num5);
					break;
				}
				case 254:
					this.HandleEventLeave(num, photonEvent);
					break;
				case 255:
				{
					ExitGames.Client.Photon.Hashtable properties = (ExitGames.Client.Photon.Hashtable)photonEvent[249];
					if (photonPlayer == null)
					{
						bool isLocal = this.mLocalActor.ID == num;
						this.AddNewPlayer(num, new PhotonPlayer(isLocal, num, properties));
						this.ResetPhotonViewsOnSerialize();
					}
					else
					{
						photonPlayer.InternalCacheProperties(properties);
					}
					if (num == this.mLocalActor.ID)
					{
						int[] actorsInRoom = (int[])photonEvent[252];
						this.UpdatedActorList(actorsInRoom);
						if (this.mLastJoinType == JoinType.JoinOrCreateOnDemand && this.mLocalActor.ID == 1)
						{
							NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
						}
						NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
					}
					else
					{
						NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerConnected, new object[]
						{
							this.mActors[num]
						});
					}
					break;
				}
				default:
					if (photonEvent.Code < 200)
					{
						if (PhotonNetwork.OnEventCall != null)
						{
							object content = photonEvent[245];
							PhotonNetwork.OnEventCall(photonEvent.Code, content, num);
						}
						else
						{
							UnityEngine.Debug.LogWarning("Warning: Unhandled event " + photonEvent + ". Set PhotonNetwork.OnEventCall.");
						}
					}
					break;
				}
				break;
			case 226:
				this.mPlayersInRoomsCount = (int)photonEvent[229];
				this.mPlayersOnMasterCount = (int)photonEvent[227];
				this.mGameCount = (int)photonEvent[228];
				break;
			case 228:
				break;
			case 229:
			{
				ExitGames.Client.Photon.Hashtable hashtable3 = (ExitGames.Client.Photon.Hashtable)photonEvent[222];
				foreach (DictionaryEntry dictionaryEntry in hashtable3)
				{
					string text = (string)dictionaryEntry.Key;
					RoomInfo roomInfo = new RoomInfo(text, (ExitGames.Client.Photon.Hashtable)dictionaryEntry.Value);
					if (roomInfo.removedFromList)
					{
						this.mGameList.Remove(text);
					}
					else
					{
						this.mGameList[text] = roomInfo;
					}
				}
				this.mGameListCopy = new RoomInfo[this.mGameList.Count];
				this.mGameList.Values.CopyTo(this.mGameListCopy, 0);
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate, new object[0]);
				break;
			}
			case 230:
			{
				this.mGameList = new Dictionary<string, RoomInfo>();
				ExitGames.Client.Photon.Hashtable hashtable4 = (ExitGames.Client.Photon.Hashtable)photonEvent[222];
				foreach (DictionaryEntry dictionaryEntry2 in hashtable4)
				{
					string text2 = (string)dictionaryEntry2.Key;
					this.mGameList[text2] = new RoomInfo(text2, (ExitGames.Client.Photon.Hashtable)dictionaryEntry2.Value);
				}
				this.mGameListCopy = new RoomInfo[this.mGameList.Count];
				this.mGameList.Values.CopyTo(this.mGameListCopy, 0);
				NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate, new object[0]);
				break;
			}
			}
			break;
		case 207:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int num6 = (int)hashtable2[0];
			if (num6 >= 0)
			{
				this.DestroyPlayerObjects(num6, true);
			}
			else
			{
				if (base.DebugOut >= DebugLevel.INFO)
				{
					UnityEngine.Debug.Log("Ev DestroyAll! By PlayerId: " + num);
				}
				this.DestroyAll(true);
			}
			break;
		}
		case 208:
		{
			ExitGames.Client.Photon.Hashtable hashtable2 = (ExitGames.Client.Photon.Hashtable)photonEvent[245];
			int playerId = (int)hashtable2[1];
			this.SetMasterClient(playerId, false);
			break;
		}
		case 209:
		{
			int[] array5 = (int[])photonEvent.Parameters[245];
			int num7 = array5[0];
			int num8 = array5[1];
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Ev OwnershipRequest: ",
				photonEvent.Parameters.ToStringFull(),
				" ViewID: ",
				num7,
				" from: ",
				num8,
				" Time: ",
				Environment.TickCount % 1000
			}));
			PhotonView photonView2 = PhotonView.Find(num7);
			if (photonView2 == null)
			{
				UnityEngine.Debug.LogWarning("Can't find PhotonView of incoming OwnershipRequest. ViewId not found: " + num7);
			}
			else
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"Ev OwnershipRequest PhotonView.ownershipTransfer: ",
					photonView2.ownershipTransfer,
					" .ownerId: ",
					photonView2.ownerId,
					" isOwnerActive: ",
					photonView2.isOwnerActive,
					". This client's player: ",
					PhotonNetwork.player.ToStringFull()
				}));
				switch (photonView2.ownershipTransfer)
				{
				case OwnershipOption.Fixed:
					UnityEngine.Debug.LogWarning("Ownership mode == fixed. Ignoring request.");
					break;
				case OwnershipOption.Takeover:
					if (num8 == photonView2.ownerId)
					{
						photonView2.ownerId = num;
					}
					break;
				case OwnershipOption.Request:
					if ((num8 == PhotonNetwork.player.ID || PhotonNetwork.player.isMasterClient) && (photonView2.ownerId == PhotonNetwork.player.ID || (PhotonNetwork.player.isMasterClient && !photonView2.isOwnerActive)))
					{
						NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnOwnershipRequest, new object[]
						{
							photonView2,
							photonPlayer
						});
					}
					break;
				}
			}
			break;
		}
		case 210:
		{
			int[] array6 = (int[])photonEvent.Parameters[245];
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Ev OwnershipTransfer. ViewID ",
				array6[0],
				" to: ",
				array6[1],
				" Time: ",
				Environment.TickCount % 1000
			}));
			int viewID = array6[0];
			int ownerId = array6[1];
			PhotonView photonView3 = PhotonView.Find(viewID);
			photonView3.ownerId = ownerId;
			break;
		}
		}
	}

	protected internal void UpdatedActorList(int[] actorsInRoom)
	{
		foreach (int num in actorsInRoom)
		{
			if (this.mLocalActor.ID != num && !this.mActors.ContainsKey(num))
			{
				this.AddNewPlayer(num, new PhotonPlayer(false, num, string.Empty));
			}
		}
	}

	private void SendVacantViewIds()
	{
		UnityEngine.Debug.Log("SendVacantViewIds()");
		List<int> list = new List<int>();
		foreach (PhotonView photonView in this.photonViewList.Values)
		{
			if (!photonView.isOwnerActive)
			{
				list.Add(photonView.viewID);
			}
		}
		UnityEngine.Debug.Log("Sending vacant view IDs. Length: " + list.Count);
		this.OpRaiseEvent(211, list.ToArray(), true, null);
	}

	public static void SendMonoMessage(PhotonNetworkingMessage methodString, params object[] parameters)
	{
		HashSet<GameObject> hashSet;
		if (PhotonNetwork.SendMonoMessageTargets != null)
		{
			hashSet = PhotonNetwork.SendMonoMessageTargets;
		}
		else
		{
			hashSet = PhotonNetwork.FindGameObjectsWithComponent(PhotonNetwork.SendMonoMessageTargetType);
		}
		string methodName = methodString.ToString();
		object value = (parameters == null || parameters.Length != 1) ? parameters : parameters[0];
		foreach (GameObject gameObject in hashSet)
		{
			gameObject.SendMessage(methodName, value, SendMessageOptions.DontRequireReceiver);
		}
	}

	protected internal void ExecuteRpc(ExitGames.Client.Photon.Hashtable rpcData, PhotonPlayer sender)
	{
		if (rpcData == null || !rpcData.ContainsKey(0))
		{
			UnityEngine.Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClass.DictionaryToString(rpcData));
			return;
		}
		int num = (int)rpcData[0];
		int num2 = 0;
		if (rpcData.ContainsKey(1))
		{
			num2 = (int)((short)rpcData[1]);
		}
		string text;
		if (rpcData.ContainsKey(5))
		{
			int num3 = (int)((byte)rpcData[5]);
			if (num3 > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
			{
				UnityEngine.Debug.LogError("Could not find RPC with index: " + num3 + ". Going to ignore! Check PhotonServerSettings.RpcList");
				return;
			}
			text = PhotonNetwork.PhotonServerSettings.RpcList[num3];
		}
		else
		{
			text = (string)rpcData[3];
		}
		object[] array = null;
		if (rpcData.ContainsKey(4))
		{
			array = (object[])rpcData[4];
		}
		if (array == null)
		{
			array = new object[0];
		}
		PhotonView photonView = this.GetPhotonView(num);
		if (photonView == null)
		{
			int num4 = num / PhotonNetwork.MAX_VIEW_IDS;
			bool flag = num4 == this.mLocalActor.ID;
			bool flag2 = num4 == sender.ID;
			if (flag)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					"Received RPC \"",
					text,
					"\" for viewID ",
					num,
					" but this PhotonView does not exist! View was/is ours.",
					(!flag2) ? " Remote called." : " Owner called.",
					" By: ",
					sender.ID
				}));
			}
			else
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					"Received RPC \"",
					text,
					"\" for viewID ",
					num,
					" but this PhotonView does not exist! Was remote PV.",
					(!flag2) ? " Remote called." : " Owner called.",
					" By: ",
					sender.ID,
					" Maybe GO was destroyed but RPC not cleaned up."
				}));
			}
			return;
		}
		if (photonView.prefix != num2)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Received RPC \"",
				text,
				"\" on viewID ",
				num,
				" with a prefix of ",
				num2,
				", our prefix is ",
				photonView.prefix,
				". The RPC has been ignored."
			}));
			return;
		}
		if (text == string.Empty)
		{
			UnityEngine.Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClass.DictionaryToString(rpcData));
			return;
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			UnityEngine.Debug.Log("Received RPC: " + text);
		}
		if (photonView.group != 0 && !this.allowedReceivingGroups.Contains(photonView.group))
		{
			return;
		}
		Type[] array2 = new Type[0];
		if (array.Length > 0)
		{
			array2 = new Type[array.Length];
			int num5 = 0;
			foreach (object obj in array)
			{
				if (obj == null)
				{
					array2[num5] = null;
				}
				else
				{
					array2[num5] = obj.GetType();
				}
				num5++;
			}
		}
		int num6 = 0;
		int num7 = 0;
		if (!PhotonNetwork.UseRpcMonoBehaviourCache || photonView.RpcMonoBehaviours == null || photonView.RpcMonoBehaviours.Length == 0)
		{
			photonView.RefreshRpcMonoBehaviourCache();
		}
		for (int j = 0; j < photonView.RpcMonoBehaviours.Length; j++)
		{
			MonoBehaviour monoBehaviour = photonView.RpcMonoBehaviours[j];
			if (monoBehaviour == null)
			{
				UnityEngine.Debug.LogError("ERROR You have missing MonoBehaviours on your gameobjects!");
			}
			else
			{
				Type type = monoBehaviour.GetType();
				List<MethodInfo> list = null;
				if (this.monoRPCMethodsCache.ContainsKey(type))
				{
					list = this.monoRPCMethodsCache[type];
				}
				if (list == null)
				{
					List<MethodInfo> methods = SupportClass.GetMethods(type, typeof(PunRPC));
					this.monoRPCMethodsCache[type] = methods;
					list = methods;
				}
				if (list != null)
				{
					for (int k = 0; k < list.Count; k++)
					{
						MethodInfo methodInfo = list[k];
						if (methodInfo.Name.Equals(text))
						{
							num7++;
							ParameterInfo[] parameters = methodInfo.GetParameters();
							if (parameters.Length == array2.Length)
							{
								if (this.CheckTypeMatch(parameters, array2))
								{
									num6++;
									object obj2 = methodInfo.Invoke(monoBehaviour, array);
									if (methodInfo.ReturnType == typeof(IEnumerator))
									{
										monoBehaviour.StartCoroutine((IEnumerator)obj2);
									}
								}
							}
							else if (parameters.Length - 1 == array2.Length)
							{
								if (this.CheckTypeMatch(parameters, array2) && parameters[parameters.Length - 1].ParameterType == typeof(PhotonMessageInfo))
								{
									num6++;
									int timestamp = (int)rpcData[2];
									object[] array3 = new object[array.Length + 1];
									array.CopyTo(array3, 0);
									array3[array3.Length - 1] = new PhotonMessageInfo(sender, timestamp, photonView);
									object obj3 = methodInfo.Invoke(monoBehaviour, array3);
									if (methodInfo.ReturnType == typeof(IEnumerator))
									{
										monoBehaviour.StartCoroutine((IEnumerator)obj3);
									}
								}
							}
							else if (parameters.Length == 1 && parameters[0].ParameterType.IsArray)
							{
								num6++;
								object obj4 = methodInfo.Invoke(monoBehaviour, new object[]
								{
									array
								});
								if (methodInfo.ReturnType == typeof(IEnumerator))
								{
									monoBehaviour.StartCoroutine((IEnumerator)obj4);
								}
							}
						}
					}
				}
			}
		}
		if (num6 != 1)
		{
			string text2 = string.Empty;
			foreach (Type type2 in array2)
			{
				if (text2 != string.Empty)
				{
					text2 += ", ";
				}
				if (type2 == null)
				{
					text2 += "null";
				}
				else
				{
					text2 += type2.Name;
				}
			}
			if (num6 == 0)
			{
				if (num7 == 0)
				{
					UnityEngine.Debug.LogError(string.Concat(new object[]
					{
						"PhotonView with ID ",
						num,
						" has no method \"",
						text,
						"\" marked with the [PunRPC](C#) or @PunRPC(JS) property! Args: ",
						text2
					}));
				}
				else
				{
					UnityEngine.Debug.LogError(string.Concat(new object[]
					{
						"PhotonView with ID ",
						num,
						" has no method \"",
						text,
						"\" that takes ",
						array2.Length,
						" argument(s): ",
						text2
					}));
				}
			}
			else
			{
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"PhotonView with ID ",
					num,
					" has ",
					num6,
					" methods \"",
					text,
					"\" that takes ",
					array2.Length,
					" argument(s): ",
					text2,
					". Should be just one?"
				}));
			}
		}
	}

	private bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
	{
		if (methodParameters.Length < callParameterTypes.Length)
		{
			return false;
		}
		for (int i = 0; i < callParameterTypes.Length; i++)
		{
			Type parameterType = methodParameters[i].ParameterType;
			if (callParameterTypes[i] != null && !parameterType.Equals(callParameterTypes[i]))
			{
				return false;
			}
		}
		return true;
	}

	internal ExitGames.Client.Photon.Hashtable SendInstantiate(string prefabName, Vector3 position, Quaternion rotation, int group, int[] viewIDs, object[] data, bool isGlobalObject)
	{
		int num = viewIDs[0];
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = prefabName;
		if (position != Vector3.zero)
		{
			hashtable[1] = position;
		}
		if (rotation != Quaternion.identity)
		{
			hashtable[2] = rotation;
		}
		if (group != 0)
		{
			hashtable[3] = group;
		}
		if (viewIDs.Length > 1)
		{
			hashtable[4] = viewIDs;
		}
		if (data != null)
		{
			hashtable[5] = data;
		}
		if (this.currentLevelPrefix > 0)
		{
			hashtable[8] = this.currentLevelPrefix;
		}
		hashtable[6] = base.ServerTimeInMilliSeconds;
		hashtable[7] = num;
		this.OpRaiseEvent(202, hashtable, true, new RaiseEventOptions
		{
			CachingOption = ((!isGlobalObject) ? EventCaching.AddToRoomCache : EventCaching.AddToRoomCacheGlobal)
		});
		return hashtable;
	}

	internal GameObject DoInstantiate(ExitGames.Client.Photon.Hashtable evData, PhotonPlayer photonPlayer, GameObject resourceGameObject)
	{
		string text = (string)evData[0];
		int timestamp = (int)evData[6];
		int num = (int)evData[7];
		Vector3 position;
		if (evData.ContainsKey(1))
		{
			position = (Vector3)evData[1];
		}
		else
		{
			position = Vector3.zero;
		}
		Quaternion rotation = Quaternion.identity;
		if (evData.ContainsKey(2))
		{
			rotation = (Quaternion)evData[2];
		}
		int num2 = 0;
		if (evData.ContainsKey(3))
		{
			num2 = (int)evData[3];
		}
		short prefix = 0;
		if (evData.ContainsKey(8))
		{
			prefix = (short)evData[8];
		}
		int[] array;
		if (evData.ContainsKey(4))
		{
			array = (int[])evData[4];
		}
		else
		{
			array = new int[]
			{
				num
			};
		}
		object[] array2;
		if (evData.ContainsKey(5))
		{
			array2 = (object[])evData[5];
		}
		else
		{
			array2 = null;
		}
		if (num2 != 0 && !this.allowedReceivingGroups.Contains(num2))
		{
			return null;
		}
		if (this.ObjectPool != null)
		{
			GameObject gameObject = this.ObjectPool.Instantiate(text, position, rotation);
			PhotonView[] photonViewsInChildren = gameObject.GetPhotonViewsInChildren();
			if (photonViewsInChildren.Length != array.Length)
			{
				throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
			}
			for (int i = 0; i < photonViewsInChildren.Length; i++)
			{
				photonViewsInChildren[i].didAwake = false;
				photonViewsInChildren[i].viewID = 0;
				photonViewsInChildren[i].prefix = (int)prefix;
				photonViewsInChildren[i].instantiationId = num;
				photonViewsInChildren[i].isRuntimeInstantiated = true;
				photonViewsInChildren[i].instantiationDataField = array2;
				photonViewsInChildren[i].didAwake = true;
				photonViewsInChildren[i].viewID = array[i];
			}
			gameObject.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(photonPlayer, timestamp, null), SendMessageOptions.DontRequireReceiver);
			return gameObject;
		}
		else
		{
			if (resourceGameObject == null)
			{
				if (!NetworkingPeer.UsePrefabCache || !NetworkingPeer.PrefabCache.TryGetValue(text, out resourceGameObject))
				{
					resourceGameObject = (GameObject)Resources.Load(text, typeof(GameObject));
					if (NetworkingPeer.UsePrefabCache)
					{
						NetworkingPeer.PrefabCache.Add(text, resourceGameObject);
					}
				}
				if (resourceGameObject == null)
				{
					UnityEngine.Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + text + "]. Please verify you have this gameobject in a Resources folder.");
					return null;
				}
			}
			PhotonView[] photonViewsInChildren2 = resourceGameObject.GetPhotonViewsInChildren();
			if (photonViewsInChildren2.Length != array.Length)
			{
				throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
			}
			for (int j = 0; j < array.Length; j++)
			{
				photonViewsInChildren2[j].viewID = array[j];
				photonViewsInChildren2[j].prefix = (int)prefix;
				photonViewsInChildren2[j].instantiationId = num;
				photonViewsInChildren2[j].isRuntimeInstantiated = true;
			}
			this.StoreInstantiationData(num, array2);
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(resourceGameObject, position, rotation);
			for (int k = 0; k < array.Length; k++)
			{
				photonViewsInChildren2[k].viewID = 0;
				photonViewsInChildren2[k].prefix = -1;
				photonViewsInChildren2[k].prefixBackup = -1;
				photonViewsInChildren2[k].instantiationId = -1;
				photonViewsInChildren2[k].isRuntimeInstantiated = false;
			}
			this.RemoveInstantiationData(num);
			gameObject2.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(photonPlayer, timestamp, null), SendMessageOptions.DontRequireReceiver);
			return gameObject2;
		}
	}

	private void StoreInstantiationData(int instantiationId, object[] instantiationData)
	{
		this.tempInstantiationData[instantiationId] = instantiationData;
	}

	public object[] FetchInstantiationData(int instantiationId)
	{
		object[] result = null;
		if (instantiationId == 0)
		{
			return null;
		}
		this.tempInstantiationData.TryGetValue(instantiationId, out result);
		return result;
	}

	private void RemoveInstantiationData(int instantiationId)
	{
		this.tempInstantiationData.Remove(instantiationId);
	}

	public void DestroyPlayerObjects(int playerId, bool localOnly)
	{
		if (playerId <= 0)
		{
			UnityEngine.Debug.LogError("Failed to Destroy objects of playerId: " + playerId);
			return;
		}
		if (!localOnly)
		{
			this.OpRemoveFromServerInstantiationsOfPlayer(playerId);
			this.OpCleanRpcBuffer(playerId);
			this.SendDestroyOfPlayer(playerId);
		}
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		foreach (PhotonView photonView in this.photonViewList.Values)
		{
			if (photonView.CreatorActorNr == playerId)
			{
				hashSet.Add(photonView.gameObject);
			}
		}
		foreach (GameObject go in hashSet)
		{
			this.RemoveInstantiatedGO(go, true);
		}
		foreach (PhotonView photonView2 in this.photonViewList.Values)
		{
			if (photonView2.ownerId == playerId)
			{
				photonView2.ownerId = photonView2.CreatorActorNr;
			}
		}
	}

	public void DestroyAll(bool localOnly)
	{
		if (!localOnly)
		{
			this.OpRemoveCompleteCache();
			this.SendDestroyOfAll();
		}
		this.LocalCleanupAnythingInstantiated(true);
	}

	protected internal void RemoveInstantiatedGO(GameObject go, bool localOnly)
	{
		if (go == null)
		{
			UnityEngine.Debug.LogError("Failed to 'network-remove' GameObject because it's null.");
			return;
		}
		PhotonView[] componentsInChildren = go.GetComponentsInChildren<PhotonView>(true);
		if (componentsInChildren == null || componentsInChildren.Length <= 0)
		{
			UnityEngine.Debug.LogError("Failed to 'network-remove' GameObject because has no PhotonView components: " + go);
			return;
		}
		PhotonView photonView = componentsInChildren[0];
		int creatorActorNr = photonView.CreatorActorNr;
		int instantiationId = photonView.instantiationId;
		if (!localOnly)
		{
			if (!photonView.isMine)
			{
				UnityEngine.Debug.LogError("Failed to 'network-remove' GameObject. Client is neither owner nor masterClient taking over for owner who left: " + photonView);
				return;
			}
			if (instantiationId < 1)
			{
				UnityEngine.Debug.LogError("Failed to 'network-remove' GameObject because it is missing a valid InstantiationId on view: " + photonView + ". Not Destroying GameObject or PhotonViews!");
				return;
			}
		}
		if (!localOnly)
		{
			this.ServerCleanInstantiateAndDestroy(instantiationId, creatorActorNr, photonView.isRuntimeInstantiated);
		}
		for (int i = componentsInChildren.Length - 1; i >= 0; i--)
		{
			PhotonView photonView2 = componentsInChildren[i];
			if (!(photonView2 == null))
			{
				if (photonView2.instantiationId >= 1)
				{
					this.LocalCleanPhotonView(photonView2);
				}
				if (!localOnly)
				{
					this.OpCleanRpcBuffer(photonView2);
				}
			}
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			UnityEngine.Debug.Log("Network destroy Instantiated GO: " + go.name);
		}
		if (this.ObjectPool != null)
		{
			PhotonView[] photonViewsInChildren = go.GetPhotonViewsInChildren();
			for (int j = 0; j < photonViewsInChildren.Length; j++)
			{
				photonViewsInChildren[j].viewID = 0;
			}
			this.ObjectPool.Destroy(go);
		}
		else
		{
			UnityEngine.Object.Destroy(go);
		}
	}

	public int GetInstantiatedObjectsId(GameObject go)
	{
		int result = -1;
		if (go == null)
		{
			UnityEngine.Debug.LogError("GetInstantiatedObjectsId() for GO == null.");
			return result;
		}
		PhotonView[] photonViewsInChildren = go.GetPhotonViewsInChildren();
		if (photonViewsInChildren != null && photonViewsInChildren.Length > 0 && photonViewsInChildren[0] != null)
		{
			return photonViewsInChildren[0].instantiationId;
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			UnityEngine.Debug.Log("GetInstantiatedObjectsId failed for GO: " + go);
		}
		return result;
	}

	private void ServerCleanInstantiateAndDestroy(int instantiateId, int creatorId, bool isRuntimeInstantiated)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[7] = instantiateId;
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache,
			TargetActors = new int[]
			{
				creatorId
			}
		};
		this.OpRaiseEvent(202, hashtable, true, raiseEventOptions);
		ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
		hashtable2[0] = instantiateId;
		raiseEventOptions = null;
		if (!isRuntimeInstantiated)
		{
			raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.CachingOption = EventCaching.AddToRoomCacheGlobal;
			UnityEngine.Debug.Log("Destroying GO as global. ID: " + instantiateId);
		}
		this.OpRaiseEvent(204, hashtable2, true, raiseEventOptions);
	}

	private void SendDestroyOfPlayer(int actorNr)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = actorNr;
		this.OpRaiseEvent(207, hashtable, true, null);
	}

	private void SendDestroyOfAll()
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = -1;
		this.OpRaiseEvent(207, hashtable, true, null);
	}

	private void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache,
			TargetActors = new int[]
			{
				actorNr
			}
		};
		this.OpRaiseEvent(202, null, true, raiseEventOptions);
	}

	protected internal void RequestOwnership(int viewID, int fromOwner)
	{
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"RequestOwnership(): ",
			viewID,
			" from: ",
			fromOwner,
			" Time: ",
			Environment.TickCount % 1000
		}));
		this.OpRaiseEvent(209, new int[]
		{
			viewID,
			fromOwner
		}, true, new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All
		});
	}

	protected internal void TransferOwnership(int viewID, int playerID)
	{
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"TransferOwnership() view ",
			viewID,
			" to: ",
			playerID,
			" Time: ",
			Environment.TickCount % 1000
		}));
		this.OpRaiseEvent(210, new int[]
		{
			viewID,
			playerID
		}, true, new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All
		});
	}

	public bool LocalCleanPhotonView(PhotonView view)
	{
		view.removedFromLocalViewList = true;
		return this.photonViewList.Remove(view.viewID);
	}

	public PhotonView GetPhotonView(int viewID)
	{
		PhotonView photonView = null;
		this.photonViewList.TryGetValue(viewID, out photonView);
		if (photonView == null)
		{
			PhotonView[] array = UnityEngine.Object.FindObjectsOfType(typeof(PhotonView)) as PhotonView[];
			foreach (PhotonView photonView2 in array)
			{
				if (photonView2.viewID == viewID)
				{
					if (photonView2.didAwake)
					{
						UnityEngine.Debug.LogWarning("Had to lookup view that wasn't in photonViewList: " + photonView2);
					}
					return photonView2;
				}
			}
		}
		return photonView;
	}

	public void RegisterPhotonView(PhotonView netView)
	{
		if (!Application.isPlaying)
		{
			this.photonViewList = new Dictionary<int, PhotonView>();
			return;
		}
		if (netView.viewID == 0)
		{
			UnityEngine.Debug.Log("PhotonView register is ignored, because viewID is 0. No id assigned yet to: " + netView);
			return;
		}
		if (this.photonViewList.ContainsKey(netView.viewID))
		{
			if (!(netView != this.photonViewList[netView.viewID]))
			{
				return;
			}
			UnityEngine.Debug.LogError(string.Format("PhotonView ID duplicate found: {0}. New: {1} old: {2}. Maybe one wasn't destroyed on scene load?! Check for 'DontDestroyOnLoad'. Destroying old entry, adding new.", netView.viewID, netView, this.photonViewList[netView.viewID]));
			this.RemoveInstantiatedGO(this.photonViewList[netView.viewID].gameObject, true);
		}
		this.photonViewList.Add(netView.viewID, netView);
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			UnityEngine.Debug.Log("Registered PhotonView: " + netView.viewID);
		}
	}

	public void OpCleanRpcBuffer(int actorNumber)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache,
			TargetActors = new int[]
			{
				actorNumber
			}
		};
		this.OpRaiseEvent(200, null, true, raiseEventOptions);
	}

	public void OpRemoveCompleteCacheOfPlayer(int actorNumber)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache,
			TargetActors = new int[]
			{
				actorNumber
			}
		};
		this.OpRaiseEvent(0, null, true, raiseEventOptions);
	}

	public void OpRemoveCompleteCache()
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache,
			Receivers = ReceiverGroup.MasterClient
		};
		this.OpRaiseEvent(0, null, true, raiseEventOptions);
	}

	private void RemoveCacheOfLeftPlayers()
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[244] = 0;
		dictionary[247] = 7;
		this.OpCustom(253, dictionary, true, 0);
	}

	public void CleanRpcBufferIfMine(PhotonView view)
	{
		if (view.ownerId != this.mLocalActor.ID && !this.mLocalActor.isMasterClient)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Cannot remove cached RPCs on a PhotonView thats not ours! ",
				view.owner,
				" scene: ",
				view.isSceneView
			}));
			return;
		}
		this.OpCleanRpcBuffer(view);
	}

	public void OpCleanRpcBuffer(PhotonView view)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = view.viewID;
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache
		};
		this.OpRaiseEvent(200, hashtable, true, raiseEventOptions);
	}

	public void RemoveRPCsInGroup(int group)
	{
		foreach (KeyValuePair<int, PhotonView> keyValuePair in this.photonViewList)
		{
			PhotonView value = keyValuePair.Value;
			if (value.group == group)
			{
				this.CleanRpcBufferIfMine(value);
			}
		}
	}

	public void SetLevelPrefix(short prefix)
	{
		this.currentLevelPrefix = prefix;
	}

	internal void RPC(PhotonView view, string methodName, PhotonPlayer player, bool encrypt, params object[] parameters)
	{
		if (this.blockSendingGroups.Contains(view.group))
		{
			return;
		}
		if (view.viewID < 1)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Illegal view ID:",
				view.viewID,
				" method: ",
				methodName,
				" GO:",
				view.gameObject.name
			}));
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Sending RPC \"",
				methodName,
				"\" to player[",
				player,
				"]"
			}));
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = view.viewID;
		if (view.prefix > 0)
		{
			hashtable[1] = (short)view.prefix;
		}
		hashtable[2] = base.ServerTimeInMilliSeconds;
		int num = 0;
		if (this.rpcShortcuts.TryGetValue(methodName, out num))
		{
			hashtable[5] = (byte)num;
		}
		else
		{
			hashtable[3] = methodName;
		}
		if (parameters != null && parameters.Length > 0)
		{
			hashtable[4] = parameters;
		}
		if (this.mLocalActor == player)
		{
			this.ExecuteRpc(hashtable, player);
		}
		else
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				TargetActors = new int[]
				{
					player.ID
				},
				Encrypt = encrypt
			};
			this.OpRaiseEvent(200, hashtable, true, raiseEventOptions);
		}
	}

	internal void RPC(PhotonView view, string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
	{
		if (this.blockSendingGroups.Contains(view.group))
		{
			return;
		}
		if (view.viewID < 1)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Illegal view ID:",
				view.viewID,
				" method: ",
				methodName,
				" GO:",
				view.gameObject.name
			}));
		}
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Sending RPC \"",
				methodName,
				"\" to ",
				target
			}));
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = view.viewID;
		if (view.prefix > 0)
		{
			hashtable[1] = (short)view.prefix;
		}
		hashtable[2] = base.ServerTimeInMilliSeconds;
		int num = 0;
		if (this.rpcShortcuts.TryGetValue(methodName, out num))
		{
			hashtable[5] = (byte)num;
		}
		else
		{
			hashtable[3] = methodName;
		}
		if (parameters != null && parameters.Length > 0)
		{
			hashtable[4] = parameters;
		}
		if (target == PhotonTargets.All)
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				InterestGroup = (byte)view.group,
				Encrypt = encrypt
			};
			this.OpRaiseEvent(200, hashtable, true, raiseEventOptions);
			this.ExecuteRpc(hashtable, this.mLocalActor);
		}
		else if (target == PhotonTargets.Others)
		{
			RaiseEventOptions raiseEventOptions2 = new RaiseEventOptions
			{
				InterestGroup = (byte)view.group,
				Encrypt = encrypt
			};
			this.OpRaiseEvent(200, hashtable, true, raiseEventOptions2);
		}
		else if (target == PhotonTargets.AllBuffered)
		{
			RaiseEventOptions raiseEventOptions3 = new RaiseEventOptions
			{
				CachingOption = EventCaching.AddToRoomCache,
				Encrypt = encrypt
			};
			this.OpRaiseEvent(200, hashtable, true, raiseEventOptions3);
			this.ExecuteRpc(hashtable, this.mLocalActor);
		}
		else if (target == PhotonTargets.OthersBuffered)
		{
			RaiseEventOptions raiseEventOptions4 = new RaiseEventOptions
			{
				CachingOption = EventCaching.AddToRoomCache,
				Encrypt = encrypt
			};
			this.OpRaiseEvent(200, hashtable, true, raiseEventOptions4);
		}
		else if (target == PhotonTargets.MasterClient)
		{
			if (this.mMasterClientId == this.mLocalActor.ID)
			{
				this.ExecuteRpc(hashtable, this.mLocalActor);
			}
			else
			{
				RaiseEventOptions raiseEventOptions5 = new RaiseEventOptions
				{
					Receivers = ReceiverGroup.MasterClient,
					Encrypt = encrypt
				};
				this.OpRaiseEvent(200, hashtable, true, raiseEventOptions5);
			}
		}
		else if (target == PhotonTargets.AllViaServer)
		{
			RaiseEventOptions raiseEventOptions6 = new RaiseEventOptions
			{
				InterestGroup = (byte)view.group,
				Receivers = ReceiverGroup.All,
				Encrypt = encrypt
			};
			this.OpRaiseEvent(200, hashtable, true, raiseEventOptions6);
			if (PhotonNetwork.offlineMode)
			{
				this.ExecuteRpc(hashtable, this.mLocalActor);
			}
		}
		else if (target == PhotonTargets.AllBufferedViaServer)
		{
			RaiseEventOptions raiseEventOptions7 = new RaiseEventOptions
			{
				InterestGroup = (byte)view.group,
				Receivers = ReceiverGroup.All,
				CachingOption = EventCaching.AddToRoomCache,
				Encrypt = encrypt
			};
			this.OpRaiseEvent(200, hashtable, true, raiseEventOptions7);
			if (PhotonNetwork.offlineMode)
			{
				this.ExecuteRpc(hashtable, this.mLocalActor);
			}
		}
		else
		{
			UnityEngine.Debug.LogError("Unsupported target enum: " + target);
		}
	}

	public void SetReceivingEnabled(int group, bool enabled)
	{
		if (group <= 0)
		{
			UnityEngine.Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + group + ". The group number should be at least 1.");
			return;
		}
		if (enabled)
		{
			if (!this.allowedReceivingGroups.Contains(group))
			{
				this.allowedReceivingGroups.Add(group);
				byte[] groupsToAdd = new byte[]
				{
					(byte)group
				};
				this.OpChangeGroups(null, groupsToAdd);
			}
		}
		else if (this.allowedReceivingGroups.Contains(group))
		{
			this.allowedReceivingGroups.Remove(group);
			byte[] groupsToRemove = new byte[]
			{
				(byte)group
			};
			this.OpChangeGroups(groupsToRemove, null);
		}
	}

	public void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
	{
		List<byte> list = new List<byte>();
		List<byte> list2 = new List<byte>();
		if (enableGroups != null)
		{
			foreach (int num in enableGroups)
			{
				if (num <= 0)
				{
					UnityEngine.Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + num + ". The group number should be at least 1.");
				}
				else if (!this.allowedReceivingGroups.Contains(num))
				{
					this.allowedReceivingGroups.Add(num);
					list.Add((byte)num);
				}
			}
		}
		if (disableGroups != null)
		{
			foreach (int num2 in disableGroups)
			{
				if (num2 <= 0)
				{
					UnityEngine.Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + num2 + ". The group number should be at least 1.");
				}
				else if (list.Contains((byte)num2))
				{
					UnityEngine.Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled disableGroups contains a group that is also in the enableGroups: " + num2 + ".");
				}
				else if (this.allowedReceivingGroups.Contains(num2))
				{
					this.allowedReceivingGroups.Remove(num2);
					list2.Add((byte)num2);
				}
			}
		}
		this.OpChangeGroups((list2.Count <= 0) ? null : list2.ToArray(), (list.Count <= 0) ? null : list.ToArray());
	}

	public void SetSendingEnabled(int group, bool enabled)
	{
		if (!enabled)
		{
			this.blockSendingGroups.Add(group);
		}
		else
		{
			this.blockSendingGroups.Remove(group);
		}
	}

	public void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
	{
		if (enableGroups != null)
		{
			foreach (int item in enableGroups)
			{
				if (this.blockSendingGroups.Contains(item))
				{
					this.blockSendingGroups.Remove(item);
				}
			}
		}
		if (disableGroups != null)
		{
			foreach (int item2 in disableGroups)
			{
				if (!this.blockSendingGroups.Contains(item2))
				{
					this.blockSendingGroups.Add(item2);
				}
			}
		}
	}

	public void NewSceneLoaded()
	{
		if (this.loadingLevelAndPausedNetwork)
		{
			this.loadingLevelAndPausedNetwork = false;
			PhotonNetwork.isMessageQueueRunning = true;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, PhotonView> keyValuePair in this.photonViewList)
		{
			PhotonView value = keyValuePair.Value;
			if (value == null)
			{
				list.Add(keyValuePair.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			int key = list[i];
			this.photonViewList.Remove(key);
		}
		if (list.Count > 0 && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			UnityEngine.Debug.Log("New level loaded. Removed " + list.Count + " scene view IDs from last level.");
		}
	}

	public void RunViewUpdate()
	{
		if (!PhotonNetwork.connected || PhotonNetwork.offlineMode)
		{
			return;
		}
		if (this.mActors == null || this.mActors.Count <= 1)
		{
			return;
		}
		this.dataPerGroupReliable.Clear();
		this.dataPerGroupUnreliable.Clear();
		foreach (KeyValuePair<int, PhotonView> keyValuePair in this.photonViewList)
		{
			PhotonView value = keyValuePair.Value;
			if (value.synchronization != ViewSynchronization.Off)
			{
				if (value.isMine)
				{
					if (value.gameObject.activeInHierarchy)
					{
						if (!this.blockSendingGroups.Contains(value.group))
						{
							ExitGames.Client.Photon.Hashtable hashtable = this.OnSerializeWrite(value);
							if (hashtable != null)
							{
								if (value.synchronization == ViewSynchronization.ReliableDeltaCompressed || value.mixedModeIsReliable)
								{
									if (hashtable.ContainsKey(1) || hashtable.ContainsKey(2))
									{
										if (!this.dataPerGroupReliable.ContainsKey(value.group))
										{
											this.dataPerGroupReliable[value.group] = new ExitGames.Client.Photon.Hashtable();
											this.dataPerGroupReliable[value.group][0] = base.ServerTimeInMilliSeconds;
											if (this.currentLevelPrefix >= 0)
											{
												this.dataPerGroupReliable[value.group][1] = this.currentLevelPrefix;
											}
										}
										ExitGames.Client.Photon.Hashtable hashtable2 = this.dataPerGroupReliable[value.group];
										hashtable2.Add((short)hashtable2.Count, hashtable);
									}
								}
								else
								{
									if (!this.dataPerGroupUnreliable.ContainsKey(value.group))
									{
										this.dataPerGroupUnreliable[value.group] = new ExitGames.Client.Photon.Hashtable();
										this.dataPerGroupUnreliable[value.group][0] = base.ServerTimeInMilliSeconds;
										if (this.currentLevelPrefix >= 0)
										{
											this.dataPerGroupUnreliable[value.group][1] = this.currentLevelPrefix;
										}
									}
									ExitGames.Client.Photon.Hashtable hashtable3 = this.dataPerGroupUnreliable[value.group];
									hashtable3.Add((short)hashtable3.Count, hashtable);
								}
							}
						}
					}
				}
			}
		}
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		foreach (KeyValuePair<int, ExitGames.Client.Photon.Hashtable> keyValuePair2 in this.dataPerGroupReliable)
		{
			raiseEventOptions.InterestGroup = (byte)keyValuePair2.Key;
			this.OpRaiseEvent(206, keyValuePair2.Value, true, raiseEventOptions);
		}
		foreach (KeyValuePair<int, ExitGames.Client.Photon.Hashtable> keyValuePair3 in this.dataPerGroupUnreliable)
		{
			raiseEventOptions.InterestGroup = (byte)keyValuePair3.Key;
			this.OpRaiseEvent(201, keyValuePair3.Value, false, raiseEventOptions);
		}
	}

	private ExitGames.Client.Photon.Hashtable OnSerializeWrite(PhotonView view)
	{
		PhotonStream photonStream = new PhotonStream(true, null);
		PhotonMessageInfo info = new PhotonMessageInfo(this.mLocalActor, base.ServerTimeInMilliSeconds, view);
		view.SerializeView(photonStream, info);
		if (photonStream.Count == 0)
		{
			return null;
		}
		object[] array = photonStream.data.ToArray();
		if (view.synchronization == ViewSynchronization.UnreliableOnChange)
		{
			if (this.AlmostEquals(array, view.lastOnSerializeDataSent))
			{
				if (view.mixedModeIsReliable)
				{
					return null;
				}
				view.mixedModeIsReliable = true;
				view.lastOnSerializeDataSent = array;
			}
			else
			{
				view.mixedModeIsReliable = false;
				view.lastOnSerializeDataSent = array;
			}
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable[0] = view.viewID;
		hashtable[1] = array;
		if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed)
		{
			bool flag = this.DeltaCompressionWrite(view, hashtable);
			view.lastOnSerializeDataSent = array;
			if (!flag)
			{
				return null;
			}
		}
		return hashtable;
	}

	private void OnSerializeRead(ExitGames.Client.Photon.Hashtable data, PhotonPlayer sender, int networkTime, short correctPrefix)
	{
		int num = (int)data[0];
		PhotonView photonView = this.GetPhotonView(num);
		if (photonView == null)
		{
			UnityEngine.Debug.LogWarning(string.Concat(new object[]
			{
				"Received OnSerialization for view ID ",
				num,
				". We have no such PhotonView! Ignored this if you're leaving a room. State: ",
				this.State
			}));
			return;
		}
		if (photonView.prefix > 0 && (int)correctPrefix != photonView.prefix)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[]
			{
				"Received OnSerialization for view ID ",
				num,
				" with prefix ",
				correctPrefix,
				". Our prefix is ",
				photonView.prefix
			}));
			return;
		}
		if (photonView.group != 0 && !this.allowedReceivingGroups.Contains(photonView.group))
		{
			return;
		}
		if (photonView.synchronization == ViewSynchronization.ReliableDeltaCompressed)
		{
			if (!this.DeltaCompressionRead(photonView, data))
			{
				if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
				{
					UnityEngine.Debug.Log(string.Concat(new object[]
					{
						"Skipping packet for ",
						photonView.name,
						" [",
						photonView.viewID,
						"] as we haven't received a full packet for delta compression yet. This is OK if it happens for the first few frames after joining a game."
					}));
				}
				return;
			}
			photonView.lastOnSerializeDataReceived = (data[1] as object[]);
		}
		if (sender.ID != photonView.ownerId && (!photonView.isSceneView || !sender.isMasterClient))
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Adjusting owner to sender of updates. From: ",
				photonView.ownerId,
				" to: ",
				sender.ID
			}));
			photonView.ownerId = sender.ID;
		}
		object[] incomingData = data[1] as object[];
		PhotonStream stream = new PhotonStream(false, incomingData);
		PhotonMessageInfo info = new PhotonMessageInfo(sender, networkTime, photonView);
		photonView.DeserializeView(stream, info);
	}

	private bool AlmostEquals(object[] lastData, object[] currentContent)
	{
		if (lastData == null && currentContent == null)
		{
			return true;
		}
		if (lastData == null || currentContent == null || lastData.Length != currentContent.Length)
		{
			return false;
		}
		for (int i = 0; i < currentContent.Length; i++)
		{
			object one = currentContent[i];
			object two = lastData[i];
			if (!this.ObjectIsSameWithInprecision(one, two))
			{
				return false;
			}
		}
		return true;
	}

	private bool DeltaCompressionWrite(PhotonView view, ExitGames.Client.Photon.Hashtable data)
	{
		if (view.lastOnSerializeDataSent == null)
		{
			return true;
		}
		object[] lastOnSerializeDataSent = view.lastOnSerializeDataSent;
		object[] array = data[1] as object[];
		if (array == null)
		{
			return false;
		}
		if (lastOnSerializeDataSent.Length != array.Length)
		{
			return true;
		}
		object[] array2 = new object[array.Length];
		int num = 0;
		List<int> list = new List<int>();
		for (int i = 0; i < array2.Length; i++)
		{
			object obj = array[i];
			object two = lastOnSerializeDataSent[i];
			if (this.ObjectIsSameWithInprecision(obj, two))
			{
				num++;
			}
			else
			{
				array2[i] = array[i];
				if (obj == null)
				{
					list.Add(i);
				}
			}
		}
		if (num > 0)
		{
			data.Remove(1);
			if (num == array.Length)
			{
				return false;
			}
			data[2] = array2;
			if (list.Count > 0)
			{
				data[3] = list.ToArray();
			}
		}
		return true;
	}

	private bool DeltaCompressionRead(PhotonView view, ExitGames.Client.Photon.Hashtable data)
	{
		if (data.ContainsKey(1))
		{
			return true;
		}
		if (view.lastOnSerializeDataReceived == null)
		{
			return false;
		}
		object[] array = data[2] as object[];
		if (array == null)
		{
			return false;
		}
		int[] array2 = data[3] as int[];
		if (array2 == null)
		{
			array2 = new int[0];
		}
		object[] lastOnSerializeDataReceived = view.lastOnSerializeDataReceived;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == null && !array2.Contains(i))
			{
				object obj = lastOnSerializeDataReceived[i];
				array[i] = obj;
			}
		}
		data[1] = array;
		return true;
	}

	private bool ObjectIsSameWithInprecision(object one, object two)
	{
		if (one == null || two == null)
		{
			return one == null && two == null;
		}
		if (!one.Equals(two))
		{
			if (one is Vector3)
			{
				Vector3 target = (Vector3)one;
				Vector3 second = (Vector3)two;
				if (target.AlmostEquals(second, PhotonNetwork.precisionForVectorSynchronization))
				{
					return true;
				}
			}
			else if (one is Vector2)
			{
				Vector2 target2 = (Vector2)one;
				Vector2 second2 = (Vector2)two;
				if (target2.AlmostEquals(second2, PhotonNetwork.precisionForVectorSynchronization))
				{
					return true;
				}
			}
			else if (one is Quaternion)
			{
				Quaternion target3 = (Quaternion)one;
				Quaternion second3 = (Quaternion)two;
				if (target3.AlmostEquals(second3, PhotonNetwork.precisionForQuaternionSynchronization))
				{
					return true;
				}
			}
			else if (one is float)
			{
				float target4 = (float)one;
				float second4 = (float)two;
				if (target4.AlmostEquals(second4, PhotonNetwork.precisionForFloatSynchronization))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	protected internal static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
	{
		mi = null;
		if (monob == null || string.IsNullOrEmpty(methodType))
		{
			return false;
		}
		List<MethodInfo> methods = SupportClass.GetMethods(monob.GetType(), null);
		for (int i = 0; i < methods.Count; i++)
		{
			MethodInfo methodInfo = methods[i];
			if (methodInfo.Name.Equals(methodType))
			{
				mi = methodInfo;
				return true;
			}
		}
		return false;
	}

	protected internal void LoadLevelIfSynced()
	{
		if (!PhotonNetwork.automaticallySyncScene || PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
		{
			return;
		}
		if (!PhotonNetwork.room.customProperties.ContainsKey("curScn"))
		{
			return;
		}
		object obj = PhotonNetwork.room.customProperties["curScn"];
		if (obj is int)
		{
			if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != (int)obj)
			{
				PhotonNetwork.LoadLevel((int)obj);
			}
		}
		else if (obj is string && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != (string)obj)
		{
			PhotonNetwork.LoadLevel((string)obj);
		}
	}

	protected internal void SetLevelInPropsIfSynced(object levelId)
	{
		if (!PhotonNetwork.automaticallySyncScene || !PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
		{
			return;
		}
		if (levelId == null)
		{
			UnityEngine.Debug.LogError("Parameter levelId can't be null!");
			return;
		}
		if (PhotonNetwork.room.customProperties.ContainsKey("curScn"))
		{
			object obj = PhotonNetwork.room.customProperties["curScn"];
			if (obj is int && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == (int)obj)
			{
				return;
			}
			if (obj is string && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals((string)obj))
			{
				return;
			}
		}
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		if (levelId is int)
		{
			hashtable["curScn"] = (int)levelId;
		}
		else if (levelId is string)
		{
			hashtable["curScn"] = (string)levelId;
		}
		else
		{
			UnityEngine.Debug.LogError("Parameter levelId must be int or string!");
		}
		PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
		this.SendOutgoingCommands();
	}

	public void SetApp(string appId, string gameVersion)
	{
		this.mAppId = appId.Trim();
		if (!string.IsNullOrEmpty(gameVersion))
		{
			PhotonNetwork.gameVersion = gameVersion.Trim();
		}
	}

	public bool WebRpc(string uriPath, object parameters)
	{
		return this.OpCustom(219, new Dictionary<byte, object>
		{
			{
				209,
				uriPath
			},
			{
				208,
				parameters
			}
		}, true);
	}

	public const string NameServerHost = "ns.exitgames.com";

	public const string NameServerHttp = "http://ns.exitgamescloud.com:80/photon/n";

	protected internal const string CurrentSceneProperty = "curScn";

	protected internal string mAppId;

	private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>
	{
		{
			ConnectionProtocol.Udp,
			5058
		},
		{
			ConnectionProtocol.Tcp,
			4533
		},
		{
			ConnectionProtocol.WebSocket,
			9093
		},
		{
			ConnectionProtocol.WebSocketSecure,
			19093
		}
	};

	public bool IsInitialConnect;

	private bool didAuthenticate;

	protected internal List<TypedLobbyInfo> LobbyStatistics = new List<TypedLobbyInfo>();

	public bool insideLobby;

	public Dictionary<string, RoomInfo> mGameList = new Dictionary<string, RoomInfo>();

	public RoomInfo[] mGameListCopy = new RoomInfo[0];

	private bool isFetchingFriends;

	private string[] friendListRequested;

	private int friendListTimestamp;

	private string playername = string.Empty;

	private bool mPlayernameHasToBeUpdated;

	private LoadbalancingPeer.EnterRoomParams enterRoomParamsCache;

	private JoinType mLastJoinType;

	private Room mCurrentGame;

	public Dictionary<int, PhotonPlayer> mActors = new Dictionary<int, PhotonPlayer>();

	public PhotonPlayer[] mOtherPlayerListCopy = new PhotonPlayer[0];

	public PhotonPlayer[] mPlayerListCopy = new PhotonPlayer[0];

	public bool hasSwitchedMC;

	private HashSet<int> allowedReceivingGroups = new HashSet<int>();

	private HashSet<int> blockSendingGroups = new HashSet<int>();

	protected internal Dictionary<int, PhotonView> photonViewList = new Dictionary<int, PhotonView>();

	private readonly Dictionary<int, ExitGames.Client.Photon.Hashtable> dataPerGroupReliable = new Dictionary<int, ExitGames.Client.Photon.Hashtable>();

	private readonly Dictionary<int, ExitGames.Client.Photon.Hashtable> dataPerGroupUnreliable = new Dictionary<int, ExitGames.Client.Photon.Hashtable>();

	protected internal short currentLevelPrefix;

	protected internal bool loadingLevelAndPausedNetwork;

	public static bool UsePrefabCache = true;

	internal IPunPrefabPool ObjectPool;

	public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

	private Dictionary<Type, List<MethodInfo>> monoRPCMethodsCache = new Dictionary<Type, List<MethodInfo>>();

	private readonly Dictionary<string, int> rpcShortcuts;

	private Dictionary<int, object[]> tempInstantiationData = new Dictionary<int, object[]>();
}
