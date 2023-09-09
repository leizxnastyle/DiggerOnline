using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExitGames.Client.Photon;
using InventorySystem;
using Photon;
using PreviewModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldGameObjectX : Photon.MonoBehaviour
{
	public ChunkGameObject[,,,] ChunkObjects
	{
		get
		{
			return this._ChunkObjects;
		}
	}

	public SimplePreviewModel Preview { get; set; }

	public int FindPlayerInd(PhotonPlayer player)
	{
		for (int i = 0; i < this.PlayerList.Count; i++)
		{
			if (this.PlayerList[i].NetPlayer == player)
			{
				return i;
			}
		}
		return -1;
	}

	public PlayerNode FindPlayer(PhotonPlayer player)
	{
		for (int i = 0; i < this.PlayerList.Count; i++)
		{
			if (this.PlayerList[i].NetPlayer == player)
			{
				return this.PlayerList[i];
			}
		}
		return null;
	}

	public PlayerNode FindPlayerByNameX(string name)
	{
		for (int i = 0; i < this.PlayerList.Count; i++)
		{
			if (this.PlayerList[i].Name == name)
			{
				return this.PlayerList[i];
			}
		}
		return null;
	}

	public PlayerNode FindPlayerByAvatar(GameObject avatar)
	{
		for (int i = 0; i < this.PlayerList.Count; i++)
		{
			if (this.PlayerList[i].Avatar == avatar)
			{
				return this.PlayerList[i];
			}
		}
		return null;
	}

	public PlayerNode FindPlayerByViewerID(string viewerID)
	{
		for (int i = 0; i < this.PlayerList.Count; i++)
		{
			if (this.PlayerList[i].ViewerID == viewerID)
			{
				return this.PlayerList[i];
			}
		}
		return null;
	}

	public EntityBase FindEntityByID(int id)
	{
		foreach (EntityBase entityBase in EntityBase.Entities)
		{
			if (entityBase.GetComponent<PhotonView>().viewID == id)
			{
				return entityBase;
			}
		}
		return null;
	}

	public void ClearEntities()
	{
		foreach (EntityBase entityBase in EntityBase.Entities)
		{
			UnityEngine.Object.Destroy(entityBase.gameObject);
		}
		EntityBase.Entities.Clear();
	}

	public WorldData WorldData
	{
		get
		{
			return this._WorldData;
		}
	}

	public World World
	{
		get
		{
			return this._World;
		}
	}

	public void Awake()
	{
		WorldGameObjectX.Instance = this;
		MasterServer.ipAddress = "67.225.180.24";
		MasterServer.port = 23466;
		for (int i = 0; i < this.listSelectel.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.listSelectel[i]);
			this._selects.Add(gameObject.tag, gameObject);
		}
		for (int j = 0; j < this.listDamage.Length; j++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.listDamage[j]);
			this._damage.Add(gameObject2.tag, gameObject2);
		}
		this._Selectel = this._selects["default"];
		this.CubeDamage = this._damage["default"];
		this.CubeDamage.GetComponent<MeshRenderer>().enabled = false;
		this._WorldCamera = GameObject.Find("WorldCamera");
		this._DefaultSkyBox = RenderSettings.skybox;
		if (GameType.IsHungerGamesMode && HG_WorkController.hgstatus == GameStatus.GS_WAIT)
		{
			WorldGameObjectX.LevelChest = new Dictionary<int, HG_Spawn>();
		}
	}

	private void Start()
	{
		PhotonNetwork.OnEventCall = (PhotonNetwork.EventCallback)Delegate.Combine(PhotonNetwork.OnEventCall, new PhotonNetwork.EventCallback(this.OnPhotonEvent));
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Players"), LayerMask.NameToLayer("SmallDecor"));
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Players"), LayerMask.NameToLayer("ParticleCubesDestroy"));
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Players"), LayerMask.NameToLayer("StorePreview"));
		this.StartSpeedHackDetection();
		base.StartCoroutine(this.CheckTimePurchasesProcess());
	}

	private void OnDestroy()
	{
		PhotonNetwork.OnEventCall = null;
	}

	public void StartServer()
	{
		App.Instance.Settings.ServerRating = 10;
		int num = (int)((App.Instance.Settings.gameType != GameINI.GameType.HIDE_SEEK) ? App.Instance.Settings.mapPopulation : 9);
		bool isOnline = App.Instance.Settings.isOnline;
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("player_id", App.Instance.Settings.playerID);
		hashtable.Add("slot_id", App.Instance.Settings.slotID);
		hashtable.Add("public_id", App.Instance.Settings.publicMapID);
		hashtable.Add("password", App.Instance.Settings.Password);
		hashtable.Add("map_name", App.Instance.Settings.server_name);
		hashtable.Add("map_about", App.Instance.Settings.server_about);
		hashtable.Add("is_watch", App.Instance.Settings.isWatch);
		hashtable.Add("game_type", (int)App.Instance.Settings.gameType);
		hashtable.Add("game_status", GameStatus.GS_LOAD);
		hashtable.Add("time", App.Instance.Settings.mapTime);
		string[] array = new string[hashtable.Keys.Count];
		hashtable.Keys.CopyTo(array, 0);
		RoomOptions roomOptions = new RoomOptions
		{
			isVisible = isOnline,
			isOpen = true,
			maxPlayers = (byte)num,
			cleanupCacheOnLeave = PhotonNetwork.autoCleanUpPlayerObjects,
			customRoomProperties = hashtable,
			customRoomPropertiesForLobby = array
		};
		PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
	}

	private void OnPhotonEvent(byte eventCode, object content, int senderId)
	{
		if (eventCode == 3)
		{
			this._CheckCheating = false;
			KGUI.SetControlText("hud.txt_cheating", string.Empty);
		}
		else if (GameType.BattleMode())
		{
			TeamBattle.Instance.PhotonEventWork(eventCode, content, senderId);
		}
		else if (eventCode == 2)
		{
			UnityEngine.Debug.Log("--SEND CONTENT" + (string)content);
		}
	}

	public int GetPlayerCount(bool isWin)
	{
		int num = 0;
		foreach (PlayerNode playerNode in this.PlayerList)
		{
			if (playerNode != null && playerNode.Life > 0f)
			{
				num++;
			}
		}
		if (!isWin && num == 0)
		{
			num = 1;
		}
		return num;
	}

	public void IEShowPlayerCount()
	{
		if (Info.Instance.GameMode == "ARCADE" && HG_WorkController.hgstatus == GameStatus.GS_START)
		{
			int playerCount = this.GetPlayerCount(true);
			if (playerCount >= 2)
			{
				string text = Localize.GetText("ARCADE_PLAYER_LOST", null);
				text = text.Replace("%COUNT%", playerCount.ToString());
				KGUI.SetControlText("hud.battle.txt_generic_tip", text);
			}
		}
	}

	public void IEShowPlayerCountArcade(int coff = 0)
	{
		if (Info.Instance.GameMode == "ARCADE" && HG_WorkController.hgstatus == GameStatus.GS_START)
		{
			int playerCount = this.GetPlayerCount(true);
			if (playerCount >= 2)
			{
				string text = Localize.GetText("ARCADE_PLAYER_LOST", null);
				text = text.Replace("%COUNT%", playerCount.ToString());
				string text2 = Localize.GetText("ARCADE_SPEED_LOST", null);
				text2 = text2.Replace("%SPEED%", coff.ToString());
				text = text + "\n" + text2;
				KGUI.SetControlText("hud.battle.txt_generic_tip_arcade", text);
			}
		}
	}

	private IEnumerator WaitEndCheckPosition()
	{
		yield return new WaitForSeconds(2f);
		CheatFinderManager.SetStayPosition();
		yield break;
	}

	[PunRPC]
	public void ExitGame(string msg = "")
	{
		UnityEngine.Debug.Log("RPC ExitGame");
		KGUI.SetControlText("hud.txt_cheating", string.Empty);
		MainMenu.Instance.ShowLoading("LOADING_LEAVING", (!GameType.IsHungerGamesMode) ? string.Empty : ("\n" + msg));
		KGUI.HideWeaponIcon();
		KGUI.SetControlText("hud.battle.txt_generic_tip", string.Empty);
		this._CheckCheating = false;
		KGUI.SetControlText("hud.txt_cheating", string.Empty);
		if (Info.Instance.GameMode == "HUNGER_GAMES" || Info.Instance.GameMode == "ARCADE" || Info.Instance.GameMode == "HIDE_SEEK")
		{
			HG_WorkController.hgstatus = GameStatus.GS_WAIT;
			if (Info.Instance.GameMode == "HUNGER_GAMES")
			{
				InventaryObjManager.SetZeroItemInPlayer();
			}
		}
		PhotonNetwork.LeaveRoom();
		this.ClearMap();
		base.StartCoroutine(this.WaitAndLoad());
	}

	private IEnumerator WaitAndLoad()
	{
		yield return new WaitForSeconds(0.3f);
		SceneManager.LoadSceneAsync("Menu");
		UnityEngine.Object.Destroy(this);
		yield break;
	}

	[PunRPC]
	public void ServerClosed(PhotonMessageInfo info)
	{
		if (TeamBattle.Instance != null)
		{
			return;
		}
		if (!Level.Instance.IsAdmin(info.sender.name) && !Level.Instance.IsModerator(info.sender.name))
		{
			return;
		}
		ProfileINI.server_was_closed = true;
		SceneManager.LoadScene("Menu");
		PhotonNetwork.LeaveRoom();
	}

	public void CloseServerAndExit()
	{
		base.photonView.RPC("ServerClosed", PhotonTargets.Others, new object[0]);
		this.ExitGame(string.Empty);
		this.MapExit();
	}

	public void MapExit()
	{
		Protect.Instance.CheckDLL(UnityEngine.Random.Range(0, 14));
		UnityEngine.Debug.Log("MapExit");
		Bar.Instance.SetCoins();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		MainMenu.Instance.CinematicCamera = false;
		Info.Instance.GameMode = string.Empty;
		if (Info.Instance.Location == "MapsList" || Info.Instance.Location == "Link")
		{
			MainMenu.Instance.SwitchMenu(Menu.Servers, null, null);
			UnityEngine.Debug.Log("MapExit: MapsList");
		}
		if (Info.Instance.Location == "MyMaps")
		{
			MainMenu.Instance.SwitchMenu(Menu.MyMaps, null, null);
			UnityEngine.Debug.Log("MapExit: MyMaps");
		}
		if (Info.Instance.Location == "FastGame")
		{
			MainMenu.Instance.SwitchMenu(Menu.FastGame, null, null);
			UnityEngine.Debug.Log("MapExit: FastGame");
		}
		if (Info.Instance.Location == "TopMaps")
		{
			MainMenu.Instance.SwitchMenu(Menu.TopMaps, null, null);
			UnityEngine.Debug.Log("MapExit: MapsList");
		}
	}

	public void PlayerCanBuild(PlayerNode player, bool canBuild)
	{
		if (player == null)
		{
			return;
		}
		if (canBuild)
		{
			Level.Instance.AddBuilderSafe(player.Name);
		}
		else
		{
			Level.Instance.RemoveBuilder(player.Name, null);
		}
	}

	public void BanPlayer(PlayerNode player)
	{
		if (player == null)
		{
			return;
		}
		Level.Instance.AddBanned(player.Name, null);
		base.photonView.RPC("TakeBan", player.NetPlayer, new object[0]);
		Chat.SendWarning(player.Name + Localize.GetText("PLAYER_BANNED", null), true);
		int num = this.FindPlayerInd(player.NetPlayer);
		if (num != -1)
		{
			this.PlayerList.RemoveAt(num);
		}
		MainMenu.Instance.RefreshTabMenu();
	}

	public void UnbanPlayer(string playerName)
	{
		if (!Level.Instance.IsBanned(playerName))
		{
			return;
		}
		Level.Instance.RemoveBanned(playerName, null);
		Chat.SendInfoF(playerName + Localize.GetText("PLAYER_UNBANNED", null), true);
		MainMenu.Instance.RefreshTabMenu();
	}

	[PunRPC]
	public void TakeBan(PhotonMessageInfo info)
	{
		if (TeamBattle.Instance != null)
		{
			return;
		}
		if (!Level.Instance.IsAdmin(info.sender.name) && !Level.Instance.IsModerator(info.sender.name))
		{
			return;
		}
		if (Level.Instance.IsAdmin(null))
		{
			return;
		}
		ProfileINI.was_banned = true;
		SceneManager.LoadScene("Menu");
		PhotonNetwork.LeaveRoom();
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		UnityEngine.Debug.Log("OnPhotonPlayerDisconnected");
		int num = this.FindPlayerInd(player);
		if (num != -1)
		{
			Chat.SendInfoF(Localize.GetText("PlayerDisconnect1", null) + player.name + Localize.GetText("PlayerDisconnect2", null), false);
			this.PlayerList.RemoveAt(num);
			MainMenu.Instance.RefreshTabMenu();
		}
	}

	[PunRPC]
	public void SendPlayerProperties(string name, int skin, string viewer_id, bool want_level, PhotonPlayer player, int level, string weaponSkin, PhotonMessageInfo info)
	{
		if (Level.Instance.IsBanned(name))
		{
			base.photonView.RPC("TakeBan", info.sender, new object[0]);
			Chat.SendInfoF(name + Localize.GetText("PLAYER_BANNED_ENTER", null), false);
			return;
		}
		PlayerNode playerNode = new PlayerNode();
		playerNode.Name = name;
		playerNode.ViewerID = viewer_id;
		playerNode.NetPlayer = player;
		playerNode.Skin = skin;
		playerNode.Level = level;
		if (weaponSkin != string.Empty)
		{
			playerNode.WeaponSkin = (from s in weaponSkin.Split(new char[]
			{
				';'
			})
			select int.Parse(s)).ToArray<int>();
		}
		this.PlayerList.Add(playerNode);
		MainMenu.Instance.RefreshTabMenu();
		if (player.isLocal)
		{
			playerNode.IsMainPlayer = true;
			this.MainPlayerNode = playerNode;
			PlayerNode mainPlayerNode = this.MainPlayerNode;
			mainPlayerNode.isLifeOfShieldChanged = (LifeOfShieldChanged)Delegate.Combine(mainPlayerNode.isLifeOfShieldChanged, new LifeOfShieldChanged(delegate(string pName, float pLife, float pShield)
			{
				base.photonView.RPC("PlayerLifeOfShieldChanged", PhotonTargets.All, new object[]
				{
					pName,
					pLife,
					pShield
				});
			}));
		}
		if (PhotonNetwork.isMasterClient && want_level)
		{
			base.StartCoroutine(this.SendLevel(info.sender));
		}
		if (Level.Instance.IsAdmin(null) && !Level.Instance.IsBuilder(name) && (App.Instance.Settings.gameType != GameINI.GameType.BUILDING || !ProfileINI.newgamersislook))
		{
			Level.Instance.AddBuilderSafe(name);
		}
	}

	[PunRPC]
	public void SetPlayerLevel(PhotonPlayer player, int level)
	{
		PlayerNode playerNode = this.FindPlayer(player);
		playerNode.Level = level;
		playerNode.Avatar.GetComponent<Nickname>().SetLevel(level);
		MainMenu.Instance.RefreshTabMenu();
	}

	[PunRPC]
	private void ChangePlayerSkin(string playerName, int skin)
	{
		if (!GameType.IsHungerGamesMode)
		{
			PlayerNode playerNode = this.FindPlayerByNameX(playerName);
			playerNode.Skin = skin;
			if (playerNode.Avatar != null)
			{
				if (playerNode.Avatar.GetComponent<SkinManager>().CurSkin != -1)
				{
					playerNode.Avatar.GetComponent<SkinManager>().SetSkin(skin);
				}
				playerNode.Avatar.GetComponent<PlayerNetwork>().RefreshNativeWeapon(skin);
			}
		}
	}

	public IEnumerator SendLevel(PhotonPlayer player)
	{
		while (!this.IsWorldGenerated || !this._World.zipedLevel.finished)
		{
			yield return 0;
		}
		GameINI gameIni = App.Instance.Settings;
		base.photonView.RPC("StartLoadLevel", player, new object[]
		{
			this._WorldData.ChunkBlockWidth,
			this._WorldData.ChunkBlockHeight,
			this._WorldData.ChunkBlockDepth,
			this._WorldData.ChunksWide,
			this._WorldData.ChunksHigh,
			this._WorldData.ChunksDeep,
			(byte)gameIni.mapType,
			this._World.zipedLevel.ziped.Length,
			World.ZipLevelThread.chunk_size,
			(int)gameIni.gameType,
			gameIni.destroyable
		});
		yield break;
	}

	[PunRPC]
	private void StartLoadLevel(int chunkBlockWidth, int chunkBlockHeight, int chunkBlockDepth, int chunkWide, int chunkHigh, int chunkDeep, byte mapType, int zippedLevelSize, int zippedLevelChunkSize, int gametype, bool destroyable)
	{
		GameINI settings = App.Instance.Settings;
		settings.mapType = (GameINI.MapType)mapType;
		settings.gameType = (GameINI.GameType)gametype;
		settings.destroyable = destroyable;
		this.InitWorldStructures(chunkBlockWidth, chunkBlockHeight, chunkBlockDepth, chunkWide, chunkHigh, chunkDeep);
		this._CurZipedLevelChunk = 0;
		this._ZipedLevelChunkCount = 0;
		this._World.zipedLevel.ziped = new byte[zippedLevelSize];
		this._World.zipedLevel.finished = false;
		World.ZipLevelThread.chunk_size = zippedLevelChunkSize;
		this._ZipedChunkReceiveTime = Time.time;
		base.photonView.RPC("LoadChunk", PhotonTargets.MasterClient, new object[0]);
		MainMenu.Instance.ShowLoading("LOADING_LOADING", string.Empty);
	}

	[PunRPC]
	public void LoadChunk(PhotonMessageInfo info)
	{
		UnityEngine.Debug.Log("LoadChunk");
		base.StartCoroutine(this.SendLevelData(info.sender));
	}

	public IEnumerator SendLevelData(PhotonPlayer player)
	{
		UnityEngine.Debug.Log("SendLevelData 1");
		List<byte[]> level_list = this._World.zipedLevel.GetCurrentZipedLevel();
		List<byte[]> delta_list = this._World.CurLevelDelta.GetCurrentLevelDelta();
		base.photonView.RPC("SendLevelPartSizes", player, new object[]
		{
			level_list.Count,
			delta_list.Count
		});
		yield return new WaitForSeconds(2f);
		for (int i = 0; i < level_list.Count; i++)
		{
			base.photonView.RPC("R", player, new object[]
			{
				0,
				i,
				level_list[i]
			});
			yield return new WaitForSeconds(0.01f);
		}
		for (int j = 0; j < delta_list.Count; j++)
		{
			base.photonView.RPC("R", player, new object[]
			{
				1,
				j,
				delta_list[j]
			});
			yield return new WaitForSeconds(0.01f);
		}
		UnityEngine.Debug.Log("SendLevelData 2");
		yield break;
	}

	[PunRPC]
	private void SendLevelPartSizes(int levelSize, int deltaSize)
	{
		KGUI.SetNodes("loading.button_fight", true, false);
		this._ZipedLevelChunkCount = levelSize + deltaSize;
		this._ZipedChunkReceiveTime = Time.time;
	}

	[PunRPC]
	private void R(int type, int place, byte[] zipBlocks)
	{
		this._ZipedChunkReceiveTime = Time.time;
		KGUI.SetNodes("loading.button_fight", true, false);
		if (type == 0)
		{
			zipBlocks.CopyTo(this._World.zipedLevel.ziped, place * World.ZipLevelThread.chunk_size);
		}
		if (type == 1)
		{
			this._World.CurLevelDelta.Add(place * 1500, zipBlocks);
		}
		this._CurZipedLevelChunk++;
		MainMenu.Instance.SetLoadingText("LOADING_LOADING", " " + this._CurZipedLevelChunk * 100 / this._ZipedLevelChunkCount + "%");
		if (this._ZipedLevelChunkCount != 0 && this._CurZipedLevelChunk >= this._ZipedLevelChunkCount && !this.IsWorldGenerated)
		{
			this._World.zipedLevel.finished = true;
			byte[] buffer = Utils.UnzipByte(this._World.zipedLevel.ziped, 2 * this._WorldData.ChunksWide * this._WorldData.ChunksHigh * this._WorldData.ChunksDeep * WorldData.Instance.m_ChunkBufferLength);
			MemoryStream input = new MemoryStream(buffer);
			BinaryReader binaryReader = new BinaryReader(input);
			for (int i = 0; i < this._WorldData.ChunksWide; i++)
			{
				for (int j = 0; j < this._WorldData.ChunksHigh; j++)
				{
					for (int k = 0; k < this._WorldData.ChunksDeep; k++)
					{
						byte[] blocksBuffer = binaryReader.ReadBytes(this._WorldData.m_ChunkBufferLength);
						this._WorldData.Chunks[i, j, k].SetBlocksBuffer(blocksBuffer);
						this._WorldData.Chunks[i, j, k].NeedsRegeneration = false;
						byte[] blocksKindBuffer = binaryReader.ReadBytes(this._WorldData.m_ChunkBufferLength);
						this._WorldData.Chunks[i, j, k].SetBlocksKindBuffer(blocksKindBuffer);
					}
				}
			}
			this._World.CurLevelDelta.MoveToLevel();
			this.LevelDeltaLoading.MoveToLevel();
			base.StartCoroutine(this.OnLevelRecieve());
		}
	}

	public IEnumerator OnLevelRecieve()
	{
		this._ZipedChunkReceiveTime = 0f;
		yield return base.StartCoroutine(this._World.LoadWorldProcess());
		Chat.SendInfoF(Localize.GetText("PlayerConnect1", null) + PhotonNetwork.playerName + Localize.GetText("PlayerConnect2", null), true);
		yield break;
	}

	public void RestartLevel()
	{
		if (!this.IsWorldGenerated)
		{
			this.ExitGame(string.Empty);
			return;
		}
		this._World.CurLevelDelta = new LevelDelta();
		this.LevelDeltaLoading = new LevelDelta();
	}

	private void OnFailedToConnect_OBSELETE(NetworkConnectionError error)
	{
		UnityEngine.Debug.Log("Could not connect to server: " + error);
	}

	private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.RegistrationSucceeded)
		{
			UnityEngine.Debug.Log("Server registered");
		}
	}

	public IEnumerator LoadWorldFromStorage(string playerID, int slotID)
	{
		MainMenu.Instance.SetLoadingText("LOADING_LOADING", string.Empty);
		KGUI.SetNodes("loading.button_fight", true, false);
		yield return 0;
		UnityEngine.Debug.Log("LoadWorldFromStorage Start Load World From Server");
		yield return base.StartCoroutine(this.LoadWorldFromServer(playerID, slotID));
		UnityEngine.Debug.Log("LoadWorldFromStorage End Load World From Server = " + (this._World != null));
		if (this._World == null)
		{
			this.ExitGame(string.Empty);
			yield break;
		}
		this._World.ZipLevel();
		yield return base.StartCoroutine(this._World.LoadWorldProcess());
		yield break;
	}

	public IEnumerator LoadWorldFromServer(string playerID, int slotID)
	{
		string mapPath;
		if (slotID != -1)
		{
			string subDirName = VKAPI.MD52(playerID).Substring(0, 2);
			string fileName = string.Concat(new object[]
			{
				playerID,
				"_",
				slotID,
				".map"
			});
			System.Random random = new System.Random();
			mapPath = string.Concat(new object[]
			{
				SettingsManager.ServerURL[1],
				"worlds/",
				subDirName,
				"/",
				fileName,
				"?random=",
				random.Next()
			});
		}
		else
		{
			mapPath = playerID;
		}
		UnityEngine.Debug.Log("Map URL: " + mapPath);
		WWW level = new WWW(mapPath);
		yield return level;
		if (level.error != null)
		{
			UnityEngine.Debug.Log("Map loaded with error! " + level.error);
			yield break;
		}
		byte[] buffer = level.bytes;
		MemoryStream ms = new MemoryStream(buffer);
		BinaryReader br = new BinaryReader(ms);
		int unpackSize = br.ReadInt32();
		byte[] zipBuffer = br.ReadBytes(buffer.Length - 4);
		byte[] unzipBuffer = Utils.UnzipByte(zipBuffer, unpackSize);
		ms = new MemoryStream(unzipBuffer);
		br = new BinaryReader(ms);
		int format = (int)br.ReadByte();
		if (format > 5)
		{
			UnityEngine.Debug.Log("Wrong format! (" + format + ")");
			yield break;
		}
		int chunkBlockWidth = br.ReadInt32();
		int chunkBlockHeight = br.ReadInt32();
		int chunkBlockDepth = br.ReadInt32();
		int chunksWide = br.ReadInt32();
		int chunksHigh = br.ReadInt32();
		int chunksDeep = br.ReadInt32();
		if (format == 1)
		{
			App.Instance.Settings.mapType = GameINI.MapType.STANDART;
		}
		if (format >= 2)
		{
			App.Instance.Settings.mapType = (GameINI.MapType)br.ReadByte();
			string viewerID = br.ReadString();
		}
		if (format >= 3)
		{
			br.ReadByte();
		}
		bool isLightsBuf = false;
		if (format < 4)
		{
			int lenToEnd = (int)(br.BaseStream.Length - br.BaseStream.Position);
			int blocksBufLen = chunkBlockWidth * chunkBlockHeight * chunkBlockDepth * chunksWide * chunksHigh * chunksDeep * 2;
			if (lenToEnd > blocksBufLen + 4)
			{
				br.BaseStream.Seek((long)blocksBufLen, SeekOrigin.Current);
				int entityBufLen = br.ReadInt32();
				if (blocksBufLen + 4 + entityBufLen == lenToEnd)
				{
					isLightsBuf = true;
				}
				br.BaseStream.Seek((long)(-(long)(4 + blocksBufLen)), SeekOrigin.Current);
			}
		}
		MemoryStream initialLevel = new MemoryStream();
		if (chunkBlockDepth != 128 || chunksDeep != 1)
		{
			this.InitWorldStructures(chunkBlockWidth, chunkBlockHeight, chunkBlockDepth, chunksWide, chunksHigh, chunksDeep);
			PChunkList allChunks = this._WorldData.AllChunks;
			foreach (Chunk chunk in allChunks)
			{
				byte[] buf = br.ReadBytes(this._WorldData.m_ChunkBufferLength);
				chunk.SetBlocksBuffer(buf);
				initialLevel.Write(buf, 0, buf.Length);
				if (format > 4)
				{
					byte[] kindBuf = br.ReadBytes(this._WorldData.m_ChunkBufferLength);
					chunk.SetBlocksKindBuffer(kindBuf);
					initialLevel.Write(kindBuf, 0, kindBuf.Length);
				}
				else
				{
					byte[] zeroBuf = new byte[this._WorldData.m_ChunkBufferLength];
					initialLevel.Write(zeroBuf, 0, zeroBuf.Length);
				}
				if (isLightsBuf)
				{
					br.ReadBytes(this._WorldData.m_ChunkBufferLength);
				}
			}
			allChunks.Release();
		}
		else
		{
			chunkBlockDepth = 16;
			chunksDeep = 8;
			this.InitWorldStructures(chunkBlockWidth, chunkBlockHeight, chunkBlockDepth, chunksWide, chunksHigh, chunksDeep);
			PChunkList allChunks2 = this._WorldData.AllChunks;
			for (int x = 0; x < chunksWide; x++)
			{
				for (int y = 0; y < chunksHigh; y++)
				{
					byte[] wholeData = br.ReadBytes(chunkBlockWidth * chunkBlockHeight * 128);
					if (isLightsBuf)
					{
						br.ReadBytes(chunkBlockWidth * chunkBlockHeight * 128);
					}
					for (int z = 0; z < chunksDeep; z++)
					{
						byte[] data = new byte[wholeData.Length / 8];
						Array.Copy(wholeData, z * data.Length, data, 0, data.Length);
						this._WorldData.Chunks[x, y, z].SetBlocksBuffer(data);
						initialLevel.Write(data, 0, data.Length);
						byte[] zeroBuf2 = new byte[this._WorldData.m_ChunkBufferLength];
						initialLevel.Write(zeroBuf2, 0, zeroBuf2.Length);
					}
				}
			}
			allChunks2.Release();
		}
		if (!this.selfLoad)
		{
			int entityBufferLength = br.ReadInt32();
			byte[] entityBuffer = br.ReadBytes(entityBufferLength);
			yield return base.StartCoroutine(this._World.SetEntityBuffer(entityBuffer));
		}
		yield break;
	}

	public void InitWorldStructures(int ch_Bw, int ch_Bh, int ch_Bd, int ch_w, int ch_h, int ch_d)
	{
		this._WorldData = new WorldData();
		this._WorldData.SetDimensions(ch_w, ch_h, ch_d, ch_Bw, ch_Bh, ch_Bd);
		this._ChunksParent = base.transform.FindChild("Chunks");
		this._WorldDecorations = new List<IDecoration>
		{
			new StandardTreeDecorator(this._WorldData)
		};
		ITerrainGenerationMethod terrainGenerationMethod = new FortressWarStandartTerrain();
		if (App.Instance.Settings.mapType == GameINI.MapType.STANDART)
		{
			if (MapsGen.MapTypeNumber == 0)
			{
				terrainGenerationMethod = new FortressWarStandartTerrain();
			}
			if (MapsGen.MapTypeNumber == 1)
			{
				terrainGenerationMethod = new FlatLand();
			}
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.FLAT)
		{
			terrainGenerationMethod = new FlatLand();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.LAVA)
		{
			terrainGenerationMethod = new LavaLand();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.PLATFORM)
		{
			terrainGenerationMethod = new PlatformLand();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.SAND)
		{
			terrainGenerationMethod = new SandLand();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.OCEAN)
		{
			terrainGenerationMethod = new OceanLand();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.ISLAND)
		{
			terrainGenerationMethod = new IslandLand();
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.SNOWLAND)
		{
			if (MapsGen.MapTypeNumber == 0)
			{
				terrainGenerationMethod = new SnowLand();
			}
			if (MapsGen.MapTypeNumber == 1)
			{
				terrainGenerationMethod = new SnowLand_Flat();
			}
			this._WorldDecorations = new List<IDecoration>
			{
				new SnowTreeDecorator(this._WorldData)
			};
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.AUTUMN)
		{
			if (MapsGen.MapTypeNumber == 0)
			{
				terrainGenerationMethod = new AutumnLand();
			}
			if (MapsGen.MapTypeNumber == 1)
			{
				terrainGenerationMethod = new AutumnLand_Flat();
			}
			this._WorldDecorations = new List<IDecoration>
			{
				new AutumnTreeDecorator(this._WorldData)
			};
		}
		if (!ProfileINI.ambientOcclusion)
		{
			this._World = new World(this.WorldData, new TerrainGenerator(this.WorldData, new BatchProcessor<Chunk>(), terrainGenerationMethod), new LightProcessor(this.WorldData), new NewMeshGenerator(new BatchProcessor<Chunk>(), this.WorldData), new WorldDecorator(this.WorldData, new BatchProcessor<Chunk>(), this._WorldDecorations));
			this._World = new World(this.WorldData, new TerrainGenerator(this.WorldData, new BatchProcessor<Chunk>(), terrainGenerationMethod), new LightProcessor(this.WorldData), new OptMeshGenerator(new BatchProcessor<Chunk>(), this.WorldData), new WorldDecorator(this.WorldData, new BatchProcessor<Chunk>(), this._WorldDecorations));
		}
		else
		{
			this._World = new World(this.WorldData, new TerrainGenerator(this.WorldData, new BatchProcessor<Chunk>(), terrainGenerationMethod), new LightProcessor(this.WorldData), new NewMeshGenerator(new BatchProcessor<Chunk>(), this.WorldData), new WorldDecorator(this.WorldData, new BatchProcessor<Chunk>(), this._WorldDecorations));
			this._World = new World(this.WorldData, new TerrainGenerator(this.WorldData, new BatchProcessor<Chunk>(), terrainGenerationMethod), new LightProcessor(this.WorldData), new MeshGeneratorSmothLight(new BatchProcessor<Chunk>(), this.WorldData), new WorldDecorator(this.WorldData, new BatchProcessor<Chunk>(), this._WorldDecorations));
		}
		this._World.InitializeGridChunks();
		this.InitializeTextures();
		this._ChunkObjects = new ChunkGameObject[3, this.WorldData.ChunksWide, this.WorldData.ChunksHigh, this.WorldData.ChunksDeep];
	}

	public void GrowServerRating()
	{
		if (App.Instance.Settings.ServerRating < 250)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("rating", App.Instance.Settings.ServerRating + 1);
			PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
			GameINI settings = App.Instance.Settings;
			settings.ServerRating += 1;
		}
	}

	public void LowerServerRating()
	{
		if (App.Instance.Settings.ServerRating > 0)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("rating", App.Instance.Settings.ServerRating - 1);
			PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
			GameINI settings = App.Instance.Settings;
			settings.ServerRating -= 1;
		}
	}

	private void OnCreatedRoom()
	{
		UnityEngine.Debug.Log("OnCreatedRoom");
		base.photonView.RPC("SendPlayerProperties", PhotonTargets.AllBuffered, new object[]
		{
			ProfileINI.nickname,
			ProfileINI.GetActualSkin(),
			VKAPI.INSTANCE._viewerId,
			false,
			PhotonNetwork.player,
			ProfileINI.level,
			ProfileINI.WeaponSkinData.Save()
		});
		if (!App.Instance.Settings.loadingSavedMap)
		{
			if (App.Instance.Settings.mapSize == GameINI.MapSize.SMALL)
			{
				this.InitWorldStructures(16, 16, 16, 8, 8, 8);
			}
			else if (App.Instance.Settings.mapSize == GameINI.MapSize.TINY)
			{
				this.InitWorldStructures(16, 16, 16, 4, 4, 4);
			}
			else if (App.Instance.Settings.mapSize == GameINI.MapSize.MEDIUM)
			{
				this.InitWorldStructures(16, 16, 16, 12, 12, 12);
			}
			else if (App.Instance.Settings.mapSize == GameINI.MapSize.LARGE)
			{
				this.InitWorldStructures(16, 16, 16, 16, 16, 16);
			}
			base.StartCoroutine(this._World.GenerateWorldCourutin());
		}
		else
		{
			base.StartCoroutine(this.LoadWorldFromStorage(App.Instance.Settings.playerID, App.Instance.Settings.slotID));
		}
	}

	public void StartSelfLoadMap(string map_url)
	{
		this.selfLoad = true;
		GameINI settings = App.Instance.Settings;
		settings.gameType = (GameINI.GameType)((int)PhotonNetwork.room.customProperties["game_type"]);
		base.StartCoroutine(this.LoadWorldFromStorage(map_url, -1));
	}

	private void OnPhotonCreateRoomFailed()
	{
		UnityEngine.Debug.Log("OnPhotonCreateRoomFailed");
	}

	public void SetNamesForAllPlayers()
	{
		for (int i = 0; i < this.PlayerList.Count; i++)
		{
			if (this.PlayerList[i].Avatar != null)
			{
				this.PlayerList[i].Avatar.GetComponent<Nickname>().SetPlayer(this.PlayerList[i]);
			}
		}
	}

	public void SetSkinsForAllPlayers()
	{
		for (int i = 0; i < this.PlayerList.Count; i++)
		{
			if (this.PlayerList[i].Avatar != null)
			{
				this.PlayerList[i].Avatar.GetComponent<SkinManager>().SetSkin(this.PlayerList[i].Skin);
			}
		}
	}

	public void SetAmbientSoundOnLevel()
	{
		if (!ManagerAudio.AudioLoaded)
		{
			base.Invoke("SetAmbientSoundOnLevel", 1f);
			return;
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.LAVA || App.Instance.Settings.mapType == GameINI.MapType.PLATFORM || App.Instance.Settings.mapType == GameINI.MapType.FLAT || App.Instance.Settings.mapType == GameINI.MapType.AUTUMN)
		{
			this.Audio.GetComponent<AudioSource>().clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopForest);
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.ISLAND)
		{
			this.Audio.GetComponent<AudioSource>().clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopForest);
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.STANDART)
		{
			this.Audio.GetComponent<AudioSource>().clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopForest);
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.SNOWLAND)
		{
			this.Audio.GetComponent<AudioSource>().clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopSnow);
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.SAND)
		{
			this.Audio.GetComponent<AudioSource>().clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopDesert);
		}
		if (App.Instance.Settings.mapType == GameINI.MapType.OCEAN)
		{
			this.Audio.GetComponent<AudioSource>().clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopOcean);
		}
		this.Audio.gameObject.GetComponent<AudioSource>().volume = ProfileINI.ambient_volume;
		this.Audio.gameObject.GetComponent<AudioSource>().Play();
	}

	public void TeleportMainPlayerToSpawnPoint()
	{
		Vector3 vector = this.GenerateSpawnPoint();
		this.MainPlayer.transform.position = vector;
		base.photonView.RPC("TeleportPlayer", PhotonTargets.Others, new object[]
		{
			vector
		});
	}

	[PunRPC]
	private void TeleportPlayer(Vector3 pos, PhotonMessageInfo info)
	{
		PlayerNode playerNode = this.FindPlayer(info.sender);
		if (playerNode != null && playerNode.Avatar != null)
		{
			playerNode.Avatar.transform.position = pos;
		}
	}

	public Vector3 GenerateSpawnPoint()
	{
		if (Info.Instance.GameMode == "HUNGER_GAMES")
		{
			if (!(this.MainPlayer != null))
			{
				return new Vector3((float)(this._WorldData.WidthInBlocks / 2), (float)this._WorldData.DepthInBlocks, (float)(this._WorldData.HeightInBlocks / 2));
			}
			if (HG_WorkController.isT && this.MainPlayer.GetComponent<PlayerNetwork>().PlayerTeam == 0)
			{
				return new Vector3((float)(this._WorldData.WidthInBlocks / 2), (float)(this._WorldData.DepthInBlocks / 4), (float)(this._WorldData.HeightInBlocks / 2));
			}
			if (HG_WorkController.CurStartSpawn != Vector3.zero)
			{
				return new Vector3((float)((int)HG_WorkController.CurStartSpawn.x), (float)((int)HG_WorkController.CurStartSpawn.y), (float)((int)HG_WorkController.CurStartSpawn.z));
			}
		}
		else if (Info.Instance.GameMode == "ARCADE")
		{
			if (HG_WorkController.isT && this.MainPlayer.GetComponent<PlayerNetwork>().PlayerTeam == 0)
			{
				return new Vector3((float)(this._WorldData.WidthInBlocks / 2), (float)(this._WorldData.DepthInBlocks / 4), (float)(this._WorldData.HeightInBlocks / 2));
			}
			if (HG_WorkController.CurStartSpawn != Vector3.zero)
			{
				return new Vector3((float)((int)HG_WorkController.CurStartSpawn.x), (float)((int)HG_WorkController.CurStartSpawn.y), (float)((int)HG_WorkController.CurStartSpawn.z));
			}
		}
		else if (GameType.IsZombieGamesMode)
		{
			if (TeamBattle.Instance != null)
			{
				TeamBattle.Instance.IsNeedShowWeaponDialog();
				return TeamBattle.Instance.GenerateSpawnPoint();
			}
		}
		else if (Info.Instance.GameMode == "HIDE_SEEK")
		{
			if (HG_WorkController.isT && this.MainPlayer.GetComponent<PlayerNetwork>().PlayerTeam == 0)
			{
				return new Vector3((float)(this._WorldData.WidthInBlocks / 2), (float)(this._WorldData.DepthInBlocks / 4), (float)(this._WorldData.HeightInBlocks / 2));
			}
			if (HG_WorkController.CurStartSpawn != Vector3.zero)
			{
				return new Vector3((float)((int)HG_WorkController.CurStartSpawn.x), (float)((int)HG_WorkController.CurStartSpawn.y), (float)((int)HG_WorkController.CurStartSpawn.z));
			}
		}
		if (TeamBattle.Instance != null)
		{
			return TeamBattle.Instance.GenerateSpawnPoint();
		}
		if (SpawnArrow.CurSpawn != null)
		{
			return SpawnArrow.CurSpawn.GetSpawnPosition();
		}
		return new Vector3((float)(this._WorldData.WidthInBlocks / 2), (float)this._WorldData.DepthInBlocks, (float)(this._WorldData.HeightInBlocks / 2));
	}

	public void CompleteWorldGeneration()
	{
		this._WorldCamera.SetActive(false);
		this.IsWorldGenerated = true;
		if (this.MainPlayer == null)
		{
			if (App.Instance.Settings.gameType != GameINI.GameType.HUNGER_GAMES && App.Instance.Settings.gameType != GameINI.GameType.ARCADE)
			{
				if (App.Instance.Settings.gameType != GameINI.GameType.HIDE_SEEK)
				{
					this.MainPlayer = PhotonNetwork.Instantiate(this.Player_Prefab.name, this.GenerateSpawnPoint(), base.transform.rotation, 0);
					goto IL_101;
				}
			}
			try
			{
				this.MainPlayer = PhotonNetwork.Instantiate(this.HGPlayer_Prefab.name, this.GenerateSpawnPoint(), base.transform.rotation, 0);
				KGUI.SetControlText("hud.battle.txt_generic_tip", string.Empty);
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add("game_status", GameStatus.GS_WAIT);
				PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.StackTrace);
			}
			IL_101:
			this.MainPlayer.name = "MAIN_PLAYER";
			this.MainPlayerEyes = this.MainPlayer.transform.FindChild("recoil/Main Camera").GetComponentInChildren<Camera>();
			if (SystemInfo.systemMemorySize > 8000)
			{
				this.MainPlayerEyes.gameObject.GetComponent<Camera>().farClipPlane = 600f;
			}
			else if (SystemInfo.systemMemorySize > 4000)
			{
				this.MainPlayerEyes.gameObject.GetComponent<Camera>().farClipPlane = 400f;
			}
			else if (SystemInfo.systemMemorySize > 2000)
			{
				this.MainPlayerEyes.gameObject.GetComponent<Camera>().farClipPlane = 200f;
			}
			this.SetNamesForAllPlayers();
			this.SetSkinsForAllPlayers();
		}
		else
		{
			this.MainPlayer.transform.position = Vector3.zero;
		}
		MainMenu.Instance.RefreshAcceleration();
		MainMenu.Instance.RefreshFlying();
		this.ShowGameGUI();
		this.SetAmbientSoundOnLevel();
		World.Instance.SetCaching();
		EntityBase.OnWorldGenerated();
		if (App.Instance.Settings.isServer)
		{
			GameType.Activate();
		}
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		GC.WaitForPendingFinalizers();
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		this.MainPlayer.transform.position = this.GenerateSpawnPoint();
		if (App.Instance.Settings.gameType == GameINI.GameType.HUNGER_GAMES)
		{
			TeamBattle.Instance.SpawnToGame();
		}
	}

	public void ShowGameGUI()
	{
		MainMenu.Instance.HideLoading();
		MainMenu.Instance.HideMenu();
		MainMenu.Instance.SetMenu(Menu.Hud, true, null, null);
		MainMenu.Instance.AddWCToMenu();
		if (TeamBattle.Instance != null)
		{
			if (TeamBattle.Instance is Arcade)
			{
				TeamBattle.Instance.SpawnToGame();
			}
			else if (TeamBattle.Instance is Deathmatch)
			{
				MainMenu.Instance.SwitchMenu(Menu.Deathmatch, true, null);
			}
			else if (TeamBattle.Instance is HideSeek)
			{
				MainMenu.Instance.SwitchMenu(Menu.TeamBattle, true, null);
			}
			else if (TeamBattle.Instance is TeamBattle)
			{
				MainMenu.Instance.SwitchMenu(Menu.TeamBattle, true, null);
			}
		}
		else if (ProfileINI.tutorial_watch == 0)
		{
			ProfileINI.tutorial_watch = 1;
			ProfileINI.Save();
			MainMenu.Instance.SwitchMenu(Menu.Tutorial, null, null);
		}
		ProfileINI.RefreshScreenSize();
		this.UpdateKubokText();
		if (ContentUpdater.NumberContent == 4)
		{
			MainMenu.Instance.HideLoadContentText();
		}
	}

	public void UpdateKubokText()
	{
		if (this.KubokCountInGame > 0)
		{
			KGUI.SetNodes("hud.gold_cup", true, false);
			if (Level.Instance.IsAdmin(null))
			{
				KGUI.SetControlText("hud.gold_cup.txt_text1", Localize.GetText("HUD_GOLD_CUP_HIDE", null));
				KGUI.SetControlText("hud.gold_cup.txt_text2", Localize.GetText("HUD_GOLD_CUP_ALL1", null) + this.KubokCountInGame + Localize.GetText("HUD_GOLD_CUP_ALL2", null));
			}
			else
			{
				KGUI.SetControlText("hud.gold_cup.txt_text1", Localize.GetText("HUD_GOLD_CUP_AVAILABLE", null));
				KGUI.SetControlText("hud.gold_cup.txt_text2", string.Concat(new object[]
				{
					Localize.GetText("HUD_GOLD_CUP_FIND1", null),
					this.KubokFindInGame,
					Localize.GetText("HUD_GOLD_CUP_FIND2", null),
					this.KubokCountInGame
				}));
				if (this.KubokFindInGame > this.KubokCountInGame)
				{
					KGUI.SetNodes("hud.gold_cup", false, false);
				}
			}
		}
		else
		{
			KGUI.SetNodes("hud.gold_cup", false, false);
		}
	}

	private void ProcessChunkGameObject(Chunk chunk, int chunkType)
	{
		if (chunk == null)
		{
			return;
		}
		if ((chunkType == 0 && chunk.LandData.IsEmpty) || (chunkType == 1 && chunk.WaterData.IsEmpty) || (chunkType == 2 && chunk.GlassData.IsEmpty))
		{
			if (this._ChunkObjects[chunkType, chunk.X, chunk.Y, chunk.Z] != null)
			{
				UnityEngine.Object.Destroy(this._ChunkObjects[chunkType, chunk.X, chunk.Y, chunk.Z].gameObject);
			}
			if (chunkType == 0)
			{
				chunk.LandRegeneration = false;
				chunk.LandChunk = null;
			}
			else if (chunkType == 1)
			{
				chunk.WaterRegeneration = false;
				chunk.WaterChunk = null;
			}
			else
			{
				chunk.GlassRegeneration = false;
				chunk.GlassChunk = null;
			}
			return;
		}
		if (this._ChunkObjects[chunkType, chunk.X, chunk.Y, chunk.Z] == null)
		{
			string chunkName = ((chunkType != 0) ? ((chunkType != 1) ? "Glass_" : "Water_") : string.Empty) + chunk.ToString();
			Vector3 chunkPos = new Vector3((float)(chunk.X * this.WorldData.ChunkBlockWidth) + this.WorldData.GlobalXOffset, (float)(chunk.Z * this.WorldData.ChunkBlockDepth), (float)(chunk.Y * this.WorldData.ChunkBlockHeight) + this.WorldData.GlobalZOffset);
			this._ChunkObjects[chunkType, chunk.X, chunk.Y, chunk.Z] = this.CreateChunkGameObject(chunkType, chunkName, chunkPos, null);
		}
		ChunkGameObject chunkGameObject = this._ChunkObjects[chunkType, chunk.X, chunk.Y, chunk.Z];
		if (chunkType == 0)
		{
			chunkGameObject.CreateFromChunk(chunk.LandData);
			chunk.LandRegeneration = false;
			chunk.LandChunk = chunkGameObject.transform;
		}
		else if (chunkType == 1)
		{
			chunkGameObject.CreateFromChunk(chunk.WaterData);
			chunk.WaterRegeneration = false;
			chunk.WaterChunk = chunkGameObject.transform;
		}
		else
		{
			chunkGameObject.CreateFromChunk(chunk.GlassData);
			chunk.GlassRegeneration = false;
			chunk.GlassChunk = chunkGameObject.transform;
		}
	}

	public ChunkGameObject CreateChunkGameObject(int chunkType, string chunkName, Vector3 chunkPos, Chunk.MeshData meshData)
	{
		Transform original = (chunkType != 0) ? ((chunkType != 1) ? this.GlassPrefab : this.WaterPrefab) : this.Chunk_Prefab;
		Transform transform = UnityEngine.Object.Instantiate(original, chunkPos, Quaternion.identity) as Transform;
		transform.parent = this._ChunksParent;
		transform.name = chunkName;
		ChunkGameObject component = transform.GetComponent<ChunkGameObject>();
		if (meshData != null)
		{
			component.CreateFromChunk(meshData);
		}
		return component;
	}

	private void OnMouseDown()
	{
		UnityEngine.Debug.Log("OnMoseDown");
	}

	public void UpdateStorePreview(int side = 0)
	{
		if (this.EntityPreview != null)
		{
			if (this.EntityPreviewIsTaked && this.EntityPreviewTaked == null)
			{
				this.SetEntityFromPreview();
				return;
			}
			EntityType type = this.EntityPreview.Type;
			bool rotation = this.EnityParametrs[(int)type].Rotation;
			if (type == EntityType.DARK_CASTLE_DOOR || type == EntityType.LOCK_DOOR_RED || type == EntityType.LOCK_DOOR_YELLOW || type == EntityType.LOCK_DOOR_BLUE || type == EntityType.LOCK_DOOR_GREEN || type == EntityType.LOCK_DOOR_WHITE || type == EntityType.LOCK_DOOR_WHITE || type == EntityType.LOCK_DOOR_RED || type == EntityType.LOCK_KEY_RED || type == EntityType.LOCK_KEY_YELLOW || type == EntityType.LOCK_KEY_BLUE || type == EntityType.LOCK_KEY_GREEN || type == EntityType.LOCK_KEY_WHITE || type == EntityType.TEAM_DOOR_BLUE || type == EntityType.TEAM_DOOR_RED || type == EntityType.FOOD_SPOON || type == EntityType.FOOD_FORK || type == EntityType.FOOD_MUG || type == EntityType.FLAG_RED || type == EntityType.FLAG_BLUE || type == EntityType.GIFT_ARMOR || type == EntityType.GIFT_GRENADE || type == EntityType.GIFT_AMMO || type == EntityType.GIFT_LIFE || type == EntityType.FOOD_JUG || type == EntityType.FOOD_METAL_PLATE || type == EntityType.FOOD_BREAD || type == EntityType.FOOD_CHEES || type == EntityType.FOOD_CHIKEN || type == EntityType.FOOD_SVININA || type == EntityType.FOOD_FISH || type == EntityType.FOOD_STEIK || type == EntityType.FOOD_ORANGE || type == EntityType.FOOD_PIZZA || type == EntityType.FOOD_COLA || type == EntityType.BURGER || type == EntityType.HG_ARENA_SPAWN_POINT || type == EntityType.HG_APOS_SPAWN_POINT || type == EntityType.FOOD_PIROG || type == EntityType.FOOD_CAKE || type == EntityType.WEAT || type == EntityType.DOORW || type == EntityType.DOORM || type == EntityType.BELLSMALL || type == EntityType.FISH_1 || type == EntityType.FISH_2 || type == EntityType.FISH_3 || type == EntityType.BOAT || type == EntityType.TROLLEY || type == EntityType.CAT || type == EntityType.CAT_BLACK || type == EntityType.CAT_STRIPED || type == EntityType.DOG || type == EntityType.CHICKEN || type == EntityType.BOOK || type == EntityType.NY_TREE || type == EntityType.MILITARY_ARMORY || type == EntityType.MILITARY_BED || type == EntityType.MILITARY_LAMP || type == EntityType.MILITARY_DOOR || this.IsCarter(type) || this.IsCIOrCE(type) || type == EntityType.PL_CACTUS || type == EntityType.PL_WATERLILY || type == EntityType.PL_WATERLILY2 || type == EntityType.HOUSEPLANTS1 || type == EntityType.HOUSEPLANTS2 || type == EntityType.MUSH_WHITE || type == EntityType.MUSH_FOX || type == EntityType.MUSH_AMANITA || type == EntityType.MUSH_ORANGE_CAP || type == EntityType.MUSH_TOADSTOOL || type == EntityType.NY_16_TREE || type == EntityType.NY_16_BIG_TREE || type == EntityType.NY_16_GIFT || type == EntityType.NY_16_GIFT_BAG || type == EntityType.NY_16_SNOWMAN || type == EntityType.NY_16_WREATH || type == EntityType.NY_16_SOCK || type == EntityType.FIREWORK1 || type == EntityType.FIREWORK2 || type == EntityType.FIREWORK3 || type == EntityType.FIREWORK4 || type == EntityType.FLAG1 || type == EntityType.FLAG2 || type == EntityType.FLAG3 || type == EntityType.FLAG4 || type == EntityType.FLAG5 || type == EntityType.FLAG6 || type == EntityType.FLAG7 || type == EntityType.FLAG8 || type == EntityType.FLAG9 || type == EntityType.FLAG10 || type == EntityType.HOUSEPLANTS3 || type == EntityType.HOUSEPLANTS4 || type == EntityType.HOUSEPLANTS5)
			{
				if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f || side < 0)
				{
					iTween.RotateAdd(this.EntityPreview.gameObject, new Vector3(0f, 90f, 0f), 1f);
				}
				else if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f || side > 0)
				{
					iTween.RotateAdd(this.EntityPreview.gameObject, new Vector3(0f, -90f, 0f), 1f);
				}
			}
			else if (rotation)
			{
				if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f || side < 0)
				{
					iTween.RotateAdd(this.EntityPreview.gameObject, new Vector3(0f, 90f, 0f), 1f);
				}
				else if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f || side > 0)
				{
					iTween.RotateAdd(this.EntityPreview.gameObject, new Vector3(0f, -90f, 0f), 1f);
				}
			}
			else if ((type != EntityType.PICTURE3_2 && type != EntityType.TABLICHKAW && type != EntityType.LADDER) || type != EntityType.CE_STREET_LADDER)
			{
				if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f)
				{
					iTween.RotateAdd(this.EntityPreview.gameObject, new Vector3(0f, 0f, 90f), 1f);
				}
				else if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f)
				{
					iTween.RotateAdd(this.EntityPreview.gameObject, new Vector3(0f, 0f, -90f), 1f);
				}
			}
			if (CameraController.Instance == null)
			{
				return;
			}
			bool food = this.EnityParametrs[(int)type].food;
			Ray ray = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
			int layerMask;
			if (food)
			{
				layerMask = (1 << LayerMask.NameToLayer("Entity") | 1 << LayerMask.NameToLayer("Terrain"));
			}
			else
			{
				layerMask = 1 << LayerMask.NameToLayer("Terrain");
			}
			int layerMask2 = 1 << LayerMask.NameToLayer("Water");
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, CameraController.RaycastDistance, layerMask) || Physics.Raycast(ray, out hit, CameraController.RaycastDistance, layerMask2))
			{
				Vector3 position = hit.point + hit.normal * 0.01f;
				IntVect intVect = new IntVect((int)position.x, (int)position.z, (int)position.y);
				BlockType blockType = this._WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z);
				if (type == EntityType.TABLICHKAW || type == EntityType.LADDER || type == EntityType.PICTURE3_2 || (type >= EntityType.PAINTING_01 && type <= EntityType.PAINTING_13) || type == EntityType.MILITARY_CAM || type == EntityType.MILITARY_LANTERN || type == EntityType.MILITARY_BRIEFING_BOARD || type == EntityType.DARK_CASTLE_DARK_PICTURE_01 || type == EntityType.DARK_CASTLE_DARK_PICTURE_02 || type == EntityType.DARK_CASTLE_DARK_PICTURE_03 || type == EntityType.DARK_CASTLE_HERALDRY || type == EntityType.DARK_CASTLE_STANDARD_01 || type == EntityType.DARK_CASTLE_STANDARD_02 || type == EntityType.CE_SMALL_SREET_LANTERN || type == EntityType.CE_STREET_CLOCK || type == EntityType.CE_STREET_LADDER)
				{
					Vector3 position2;
					Vector3 localEulerAngles;
					this.FindClippingPos(hit, out position2, out localEulerAngles);
					this.EntityPreview.transform.localEulerAngles = localEulerAngles;
					this.EntityPreview.transform.position = position2;
				}
				else if (type == EntityType.TORCH_FLOOR || type == EntityType.TORCH_FLOOR_STONE || type == EntityType.DARK_CASTLE_TORCH_02)
				{
					Vector3 position3;
					this.ProcessTorchFloorAdd(hit, out position3);
					iTween.MoveTo(this.EntityPreview.gameObject, position3, 0.5f);
				}
				else if (type == EntityType.TORCH || type == EntityType.TORCH_STONE || type == EntityType.DARK_CASTLE_TORCH_01)
				{
					Vector3 position4;
					Vector3 localEulerAngles2;
					this.ProcessTorchAdd(hit, out position4, out localEulerAngles2);
					this.EntityPreview.transform.localEulerAngles = localEulerAngles2;
					iTween.MoveTo(this.EntityPreview.gameObject, position4, 0.5f);
				}
				else if (type == EntityType.BOAT || type == EntityType.BEACH3)
				{
					if (blockType == BlockType.Water)
					{
						position = hit.point + hit.normal * 0.01f;
						intVect = new IntVect((int)position.x, (int)position.z, (int)position.y);
						iTween.MoveTo(this.EntityPreview.gameObject, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 1f, (float)intVect.Y + 0.5f), 0.5f);
					}
					else
					{
						position = hit.point + hit.normal * 0.01f;
						intVect = new IntVect((int)position.x, (int)position.z, (int)position.y);
						iTween.MoveTo(this.EntityPreview.gameObject, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 0.25f, (float)intVect.Y + 0.5f), 0.5f);
					}
				}
				else if (type == EntityType.TEAM_DOOR_RED || type == EntityType.TEAM_DOOR_BLUE)
				{
					position = hit.point + hit.normal * 0.01f;
					intVect = new IntVect((int)position.x, (int)position.z, (int)position.y);
					if ((this.EntityPreview.transform.rotation.eulerAngles.y > 350f && this.EntityPreview.transform.rotation.eulerAngles.y < 360f) || (this.EntityPreview.transform.rotation.eulerAngles.y > 0f && this.EntityPreview.transform.rotation.eulerAngles.y < 10f))
					{
						iTween.MoveTo(this.EntityPreview.gameObject, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y), 0.5f);
					}
					else if (this.EntityPreview.transform.rotation.eulerAngles.y > 170f && this.EntityPreview.transform.rotation.eulerAngles.y < 190f)
					{
						iTween.MoveTo(this.EntityPreview.gameObject, new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y), 0.5f);
					}
					else
					{
						iTween.MoveTo(this.EntityPreview.gameObject, new Vector3((float)intVect.X, (float)intVect.Z, (float)intVect.Y + 0.5f), 0.5f);
					}
				}
				else if (food)
				{
					position = hit.point;
					intVect = new IntVect((int)position.x, (int)position.z, (int)position.y);
					this.EntityPreview.transform.position = position;
				}
				else
				{
					position = hit.point + hit.normal * 0.01f;
					intVect = new IntVect((int)position.x, (int)position.z, (int)position.y);
					Vector3 vector = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
					if (type == EntityType.ATOMBOMB)
					{
						vector += new Vector3(0f, 0.5f, 0f);
					}
					BlockKind blockKind = this._WorldData.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
					switch (blockKind)
					{
					case (BlockKind)139:
						blockKind = BlockKind.HalfWallNorth;
						break;
					case (BlockKind)140:
						blockKind = BlockKind.QuarterOnWallSouth;
						break;
					case (BlockKind)141:
						blockKind = BlockKind.HalfWallEast;
						break;
					case (BlockKind)142:
						blockKind = BlockKind.HalfWallWest;
						break;
					case (BlockKind)145:
						blockKind = BlockKind.QuarterOnWallEast;
						break;
					case (BlockKind)148:
						blockKind = BlockKind.QuarterOnWallWest;
						break;
					}
					switch (blockKind)
					{
					case BlockKind.Half:
						vector += new Vector3(0f, 0.5f, 0f);
						break;
					case BlockKind.HalfWallNorth:
						vector += new Vector3(0f, 0f, 0.5f);
						break;
					case BlockKind.HalfWallSouth:
						vector -= new Vector3(0f, 0f, 0.5f);
						break;
					case BlockKind.HalfWallEast:
						vector -= new Vector3(0.5f, 0f, 0f);
						break;
					case BlockKind.HalfWallWest:
						vector += new Vector3(0.5f, 0f, 0f);
						break;
					case BlockKind.Quarter:
						vector += new Vector3(0f, 0.25f, 0f);
						break;
					case BlockKind.QuarterOnWallEast:
						vector -= new Vector3(0.25f, 0f, 0f);
						break;
					case BlockKind.QuarterOnWallNorth:
						vector += new Vector3(0f, 0f, 0.25f);
						break;
					case BlockKind.QuarterOnWallSouth:
						vector -= new Vector3(0f, 0f, 0.25f);
						break;
					case BlockKind.QuarterOnWallWest:
						vector += new Vector3(0.25f, 0f, 0f);
						break;
					case BlockKind.Third:
						vector += new Vector3(0f, 0.75f, 0f);
						break;
					case BlockKind.ThirdOnWallEast:
						vector -= new Vector3(0.75f, 0f, 0f);
						break;
					case BlockKind.ThirdOnWallWest:
						vector += new Vector3(0.75f, 0f, 0f);
						break;
					case BlockKind.ThirdOnWallSouth:
						vector -= new Vector3(0f, 0f, 0.75f);
						break;
					case BlockKind.ThirdOnWallNorth:
						vector += new Vector3(0f, 0f, 0.75f);
						break;
					}
					iTween.MoveTo(this.EntityPreview.gameObject, vector, 0.5f);
				}
				MainMenu.CrosshairAction crosshairAction = MainMenu.CrosshairAction.None;
				if (type != EntityType.TORCH && type != EntityType.TORCH_STONE)
				{
					crosshairAction |= MainMenu.CrosshairAction.Wheel;
					crosshairAction |= MainMenu.CrosshairAction.Put;
				}
				MainMenu.Instance.SetCrosshairInfo(this.EnityParametrs[(int)type].name, crosshairAction);
			}
		}
	}

	private bool IsCarter(EntityType entityType)
	{
		return entityType == EntityType.CARPET_01 || entityType == EntityType.CARPET_02 || entityType == EntityType.CARPET_03 || entityType == EntityType.CARPET_04 || entityType == EntityType.CARPET_05 || entityType == EntityType.CARPET_06 || entityType == EntityType.CARPET_07 || entityType == EntityType.CARPET_08 || entityType == EntityType.CARPET_09 || entityType == EntityType.CARPET_10 || entityType == EntityType.CARPET_11TH || entityType == EntityType.CARPET_11TV;
	}

	private bool IsCIOrCE(EntityType entityType)
	{
		return entityType == EntityType.CE_ATTENTION_BOARD || entityType == EntityType.CE_BIG_GARBAGE_BOX_CLOSE || entityType == EntityType.CE_BIG_GARBAGE_BOX_OPEN || entityType == EntityType.CE_BIG_STREET_LANTERN || entityType == EntityType.CE_CONE || entityType == EntityType.CE_FG || entityType == EntityType.CE_GARBAGE_BOX || entityType == EntityType.CE_MAIL_BOX || entityType == EntityType.CE_METAL_DOOR || entityType == EntityType.CE_PHONEBOX || entityType == EntityType.CE_STREET_BENCH || entityType == EntityType.CE_STREET_CLOCK || entityType == EntityType.CE_WOOD_DOOR || entityType == EntityType.CI_BATH || entityType == EntityType.CI_BED || entityType == EntityType.CI_BEDROOM_CASE || entityType == EntityType.CI_BEDROOM_LAMP || entityType == EntityType.CI_BIG_LAMP || entityType == EntityType.CI_BOWL || entityType == EntityType.CI_COFFEE_CUP || entityType == EntityType.CI_DOOR || entityType == EntityType.CI_KITCHEN_BOARD || entityType == EntityType.CI_KITCHEN_CHAIR || entityType == EntityType.CI_KITCHEN_PART || entityType == EntityType.CI_KITCHEN_PART_CORNER || entityType == EntityType.CI_KITCHEN_TABLE || entityType == EntityType.CI_KITCHEN_WASHING || entityType == EntityType.CI_WLAPTOP || entityType == EntityType.CI_LOUDSPEAKERS || entityType == EntityType.CI_MICROWAVE || entityType == EntityType.CI_OFFICE_CHAIR || entityType == EntityType.CI_OFFICE_LAMP || entityType == EntityType.CI_OFFICE_TABLE || entityType == EntityType.CI_OFFICE_CASE || entityType == EntityType.CI_OVEN || entityType == EntityType.CI_PLATE || entityType == EntityType.CI_REFRIGERATOR || entityType == EntityType.CI_SAUCER || entityType == EntityType.CI_SHOWER || entityType == EntityType.CI_SMALL_LAMP || entityType == EntityType.CI_SMALL_TABLE || entityType == EntityType.CI_SOAP || entityType == EntityType.CI_SOFA || entityType == EntityType.CI_SOFT_CHAIR || entityType == EntityType.CI_TV || entityType == EntityType.CI_WARDROBE || entityType == EntityType.CI_WASHBASIN || entityType == EntityType.CI_WWASHING_MACHINE || entityType == EntityType.CI_WC;
	}

	private void UpdateSelectelAndCursorText()
	{
		if (CameraController.Instance == null)
		{
			return;
		}
		this.EntityUnderCursor = null;
		Ray ray = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		Ray ray2 = CameraController.RaycastCamera.ScreenPointToRay(new Vector3(CameraController.Instance.aimZone.GetPos(0, TouchCoordSys.SCREEN_PX).x, (float)Screen.height - CameraController.Instance.aimZone.GetPos(0, TouchCoordSys.SCREEN_PX).y, 0f));
		int layerMask = 1 << LayerMask.NameToLayer("SmallDecor") | 1 << LayerMask.NameToLayer("Entity");
		int layerMask2 = 1 << LayerMask.NameToLayer("Terrain");
		int layerMask3 = 1 << LayerMask.NameToLayer("Water");
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, CameraController.RaycastDistance, layerMask))
		{
			this._Selectel.GetComponent<MeshRenderer>().enabled = false;
			EntityBase entityBase = EntityBase.FindEntity(raycastHit.transform.gameObject);
			EntityType type = entityBase.Type;
			this.EntityUnderCursor = entityBase;
			MainMenu.CrosshairAction crosshairAction = MainMenu.CrosshairAction.None;
			if (type == EntityType.DOORM || type == EntityType.DOORW || type == EntityType.MILITARY_DOOR || type == EntityType.DARK_CASTLE_DOOR || type == EntityType.CI_DOOR || type == EntityType.CE_METAL_DOOR || type == EntityType.CE_WOOD_DOOR || type == EntityType.CI_REFRIGERATOR)
			{
				crosshairAction = (MainMenu.CrosshairAction)5;
			}
			else if (type == EntityType.TABLICHKAF || type == EntityType.TABLICHKAW || type == EntityType.BOOK || type == EntityType.DARK_CASTLE_BOOK_CONSOLE)
			{
				crosshairAction = (MainMenu.CrosshairAction)17;
			}
			else if (type == EntityType.TORCH || type == EntityType.TORCH_FLOOR || type == EntityType.TORCH_STONE || type == EntityType.TORCH_FLOOR_STONE || type == EntityType.DARK_CASTLE_TORCH_01 || type == EntityType.DARK_CASTLE_TORCH_02)
			{
				crosshairAction = MainMenu.CrosshairAction.Take;
			}
			else if (type == EntityType.TNT || type == EntityType.ATOMBOMB)
			{
				crosshairAction = (MainMenu.CrosshairAction)9;
			}
			else if (type == EntityType.FIREWORK1 || type == EntityType.FIREWORK2 || type == EntityType.FIREWORK3 || type == EntityType.FIREWORK4)
			{
				crosshairAction = (MainMenu.CrosshairAction)33;
			}
			else if (type == EntityType.BOAT || type == EntityType.TROLLEY || type == EntityType.BED || type == EntityType.BED_STONE || type == EntityType.CHAIR || type == EntityType.CHAIR_STONE || type == EntityType.BENCH_WOOD || type == EntityType.BENCH_STONE || type == EntityType.MILITARY_BED || type == EntityType.MILITARY_CHAIR || type == EntityType.MILITARY_BENCH || type == EntityType.DARK_CASTLE_BED || type == EntityType.DARK_CASTLE_BENCH || type == EntityType.DARK_CASTLE_CHAIR || type == EntityType.DARK_CASTLE_THRONE || type == EntityType.CE_STREET_BENCH || type == EntityType.CI_KITCHEN_CHAIR || type == EntityType.CI_OFFICE_CHAIR || type == EntityType.CI_SOFA || type == EntityType.CI_SOFT_CHAIR || type == EntityType.CI_WC)
			{
				crosshairAction = (MainMenu.CrosshairAction)3;
			}
			else if (!GameType.BattleMode() || (type != EntityType.FLAG_RED && type != EntityType.FLAG_BLUE))
			{
				crosshairAction = MainMenu.CrosshairAction.Take;
			}
			else if ((type >= EntityType.PAINTING_01 && type <= EntityType.PAINTING_13) || (type >= EntityType.DARK_CASTLE_DARK_PICTURE_01 && type <= EntityType.DARK_CASTLE_DARK_PICTURE_03))
			{
				crosshairAction = MainMenu.CrosshairAction.Take;
			}
			bool action = this.EnityParametrs[(int)type].Action;
			if (action)
			{
				crosshairAction = (MainMenu.CrosshairAction)33;
			}
			MainMenu.Instance.SetCrosshairInfo(entityBase.Name, crosshairAction);
		}
		else if (Physics.Raycast(ray2, out raycastHit, CameraController.RaycastDistance, layerMask2) || Physics.Raycast(ray, out raycastHit, CameraController.RaycastDistance, layerMask3))
		{
			if (!GameType.IsHungerGamesMode && !GameType.IsArcadeMode && !GameType.IsDeathmatchGamesMode && !GameType.IsTeamBattleMode)
			{
				this._Selectel.GetComponent<MeshRenderer>().enabled = true;
				Vector3 vector = raycastHit.point - raycastHit.normal * 0.01f;
				IntVect position = new IntVect((int)Math.Floor((double)vector.x), (int)Math.Floor((double)vector.z), (int)Math.Floor((double)vector.y));
				this._Selectel.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
				BlockKind blockKind = WorldData.Instance.GetBlockKind(position.X, position.Y, position.Z);
				this._Selectel = this.GetSelectel(blockKind);
				this.SetSelectelPosition(blockKind, position);
				this.SetSelectelRotation(blockKind);
				this._Selectel.SetActive(!App.Instance.Settings.isWatch);
			}
			else
			{
				this._Selectel.GetComponent<MeshRenderer>().enabled = false;
			}
			if (this.EntityUnderCursor == null)
			{
				MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.None);
			}
		}
		else
		{
			MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.None);
			this._Selectel.SetActive(false);
		}
	}

	public Vector3 GetSelectelPos()
	{
		return this._Selectel.transform.position;
	}

	public Quaternion GetSelectelRot()
	{
		return this._Selectel.transform.rotation;
	}

	public Vector3 GetSelectelScale()
	{
		return this._Selectel.transform.localScale;
	}

	public GameObject GetCubeDamage()
	{
		return this._damage[this._Selectel.tag];
	}

	private GameObject GetSelectel(BlockKind kind)
	{
		this._selects[this._Selectel.tag].SetActive(false);
		if (kind.IsDefault())
		{
			return this._selects["default"];
		}
		if (kind.IsHalf() || kind == (BlockKind)129)
		{
			return this._selects["half"];
		}
		if (kind.IsQuarter() || kind == (BlockKind)144)
		{
			return this._selects["quarter"];
		}
		if (kind.IsDiagonal())
		{
			return this._selects["diagonal"];
		}
		if (kind.IsCorner())
		{
			return this._selects["corner"];
		}
		if (kind.IsStair())
		{
			return this._selects["staire"];
		}
		if (kind.IsFence() || kind == (BlockKind)130)
		{
			return this._selects["fence"];
		}
		if (kind >= (BlockKind)190 && kind <= (BlockKind)193)
		{
			return this._selects["corner"];
		}
		if (kind >= (BlockKind)135 && kind <= (BlockKind)138)
		{
			return this._selects["staire"];
		}
		if (kind >= (BlockKind)131 && kind <= (BlockKind)134)
		{
			return this._selects["diagonal"];
		}
		if (kind.IsCornerStair() || kind >= (BlockKind)214 || kind <= (BlockKind)217)
		{
			return this._selects["corner_stair"];
		}
		return this._selects["default"];
	}

	private void SetSelectelPosition(BlockKind kind, IntVect position)
	{
		switch (kind)
		{
		case BlockKind.Half:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z, (float)position.Y + 0.5f);
			break;
		default:
			switch (kind)
			{
			case (BlockKind)129:
				this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
				return;
			default:
				switch (kind)
				{
				case (BlockKind)190:
				case (BlockKind)191:
				case (BlockKind)192:
				case (BlockKind)193:
					break;
				default:
					switch (kind)
					{
					case (BlockKind)214:
					case (BlockKind)215:
					case (BlockKind)216:
					case (BlockKind)217:
						break;
					default:
						this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z, (float)position.Y + 0.5f);
						return;
					}
					break;
				}
				break;
			case (BlockKind)131:
			case (BlockKind)132:
			case (BlockKind)133:
			case (BlockKind)134:
			case (BlockKind)135:
			case (BlockKind)136:
			case (BlockKind)137:
			case (BlockKind)138:
				break;
			case (BlockKind)144:
				this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.75f, (float)position.Y + 0.5f);
				return;
			}
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 1f, (float)position.Y + 0.5f);
			break;
		case BlockKind.HalfWallNorth:
		case BlockKind.HalfWallSouth:
		case BlockKind.HalfWallEast:
		case BlockKind.HalfWallWest:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.Quarter:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z, (float)position.Y + 0.5f);
			break;
		case BlockKind.QuarterOnWallEast:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.75f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.QuarterOnWallNorth:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 0.25f);
			break;
		case BlockKind.QuarterOnWallSouth:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 0.75f);
			break;
		case BlockKind.QuarterOnWallWest:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.25f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.FenceOnWallSouthNorth:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 1f);
			break;
		case BlockKind.FenceOnWallEastWest:
			this._Selectel.transform.position = new Vector3((float)position.X + 1f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.DiagonalOnWallWestRight:
			this._Selectel.transform.position = new Vector3((float)position.X + 1f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.DiagonalOnWallWestLeft:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y);
			break;
		case BlockKind.DiagonalOnWallWestBottom:
			this._Selectel.transform.position = new Vector3((float)position.X + 1f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.DiagonalOnWallEastRight:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.DiagonalOnWallEastLeft:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.DiagonalOnWallEastBottom:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.DiagonalOnWallSouthRight:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.DiagonalOnWallSouthLeft:
			this._Selectel.transform.position = new Vector3((float)position.X + 1f, (float)position.Z + 0.5f, (float)position.Y + 0.5f);
			break;
		case BlockKind.DiagonalOnWallSouthBottom:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y + 1f);
			break;
		case BlockKind.DiagonalOnWallNorthRight:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y);
			break;
		case BlockKind.DiagonalOnWallNorthLeft:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y);
			break;
		case BlockKind.DiagonalOnWallNorthBottom:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z + 0.5f, (float)position.Y);
			break;
		case BlockKind.FenceWest:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.17f, (float)position.Z, (float)position.Y + 0.17f);
			break;
		case BlockKind.FenceEast:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.83f, (float)position.Z, (float)position.Y + 0.17f);
			break;
		case BlockKind.FenceNorth:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.17f, (float)position.Z, (float)position.Y + 0.83f);
			break;
		case BlockKind.FenceSouth:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.83f, (float)position.Z, (float)position.Y + 0.17f);
			break;
		case BlockKind.EastFenceWest:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.17f, (float)position.Y + 0.83f);
			break;
		case BlockKind.EastFenceEast:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.17f, (float)position.Y + 0.17f);
			break;
		case BlockKind.EastFenceNorth:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.83f, (float)position.Y + 0.83f);
			break;
		case BlockKind.EastFenceSouth:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.83f, (float)position.Y + 0.17f);
			break;
		case BlockKind.WestFenceWest:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.17f, (float)position.Y + 0.83f);
			break;
		case BlockKind.WestFenceEast:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.17f, (float)position.Y + 0.17f);
			break;
		case BlockKind.WestFenceNorth:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.83f, (float)position.Y + 0.83f);
			break;
		case BlockKind.WestFenceSouth:
			this._Selectel.transform.position = new Vector3((float)position.X, (float)position.Z + 0.83f, (float)position.Y + 0.17f);
			break;
		case BlockKind.NorthFenceWest:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.17f, (float)position.Z + 0.17f, (float)position.Y);
			break;
		case BlockKind.NorthFenceEast:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.83f, (float)position.Z + 0.17f, (float)position.Y);
			break;
		case BlockKind.NorthFenceNorth:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.17f, (float)position.Z + 0.83f, (float)position.Y);
			break;
		case BlockKind.NorthFenceSouth:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.83f, (float)position.Z + 0.83f, (float)position.Y);
			break;
		case BlockKind.SouthFenceWest:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.17f, (float)position.Z + 0.17f, (float)position.Y);
			break;
		case BlockKind.SouthFenceEast:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.83f, (float)position.Z + 0.17f, (float)position.Y);
			break;
		case BlockKind.SouthFenceNorth:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.17f, (float)position.Z + 0.83f, (float)position.Y);
			break;
		case BlockKind.SouthFenceSouth:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.83f, (float)position.Z + 0.83f, (float)position.Y);
			break;
		case BlockKind.CornerStairEast:
		case BlockKind.CornerStairWest:
		case BlockKind.CornerStairSouth:
		case BlockKind.CornerStairNorth:
			this._Selectel.transform.position = new Vector3((float)position.X + 0.5f, (float)position.Z, (float)position.Y + 0.5f);
			break;
		}
	}

	private void SetSelectelRotation(BlockKind kind)
	{
		switch (kind)
		{
		case BlockKind.Half:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
			break;
		default:
			switch (kind)
			{
			case (BlockKind)131:
				this._Selectel.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
				break;
			case (BlockKind)132:
				this._Selectel.transform.rotation = Quaternion.Euler(90f, 270f, 0f);
				break;
			case (BlockKind)133:
				this._Selectel.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
				break;
			case (BlockKind)134:
				this._Selectel.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
				break;
			case (BlockKind)135:
				this._Selectel.transform.rotation = Quaternion.Euler(90f, 270f, 0f);
				break;
			case (BlockKind)136:
				this._Selectel.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
				break;
			case (BlockKind)137:
				this._Selectel.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
				break;
			case (BlockKind)138:
				this._Selectel.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
				break;
			default:
				switch (kind)
				{
				case (BlockKind)190:
					this._Selectel.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
					break;
				case (BlockKind)191:
					this._Selectel.transform.rotation = Quaternion.Euler(90f, 270f, 0f);
					break;
				case (BlockKind)192:
					this._Selectel.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
					break;
				case (BlockKind)193:
					this._Selectel.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
					break;
				default:
					switch (kind)
					{
					case (BlockKind)214:
						this._Selectel.transform.rotation = Quaternion.Euler(90f, 270f, 0f);
						break;
					case (BlockKind)215:
						this._Selectel.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
						break;
					case (BlockKind)216:
						this._Selectel.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
						break;
					case (BlockKind)217:
						this._Selectel.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
						break;
					default:
						this._Selectel.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
						break;
					}
					break;
				}
				break;
			}
			break;
		case BlockKind.DiagonalWest:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 270f, 0f);
			break;
		case BlockKind.DiagonalEast:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 90f, 0f);
			break;
		case BlockKind.DiagonalNorth:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 180f, 0f);
			break;
		case BlockKind.StairNorth:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 270f, 0f);
			break;
		case BlockKind.StairSouth:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 90f, 0f);
			break;
		case BlockKind.StairEast:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 180f, 0f);
			break;
		case BlockKind.HalfWallNorth:
			this._Selectel.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
			break;
		case BlockKind.HalfWallSouth:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		case BlockKind.HalfWallEast:
			this._Selectel.transform.rotation = Quaternion.Euler(180f, 270f, 0f);
			break;
		case BlockKind.HalfWallWest:
			this._Selectel.transform.rotation = Quaternion.Euler(180f, 90f, 0f);
			break;
		case BlockKind.Quarter:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
			break;
		case BlockKind.QuarterOnWallEast:
			this._Selectel.transform.rotation = Quaternion.Euler(180f, 270f, 0f);
			break;
		case BlockKind.QuarterOnWallNorth:
			this._Selectel.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
			break;
		case BlockKind.QuarterOnWallSouth:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		case BlockKind.QuarterOnWallWest:
			this._Selectel.transform.rotation = Quaternion.Euler(180f, 90f, 0f);
			break;
		case BlockKind.FenceOnWallSouthNorth:
			this._Selectel.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
			break;
		case BlockKind.FenceOnWallEastWest:
			this._Selectel.transform.rotation = Quaternion.Euler(180f, 90f, 0f);
			break;
		case BlockKind.DiagonalOnWallWestRight:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 270f, 90f);
			break;
		case BlockKind.DiagonalOnWallWestLeft:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			break;
		case BlockKind.DiagonalOnWallWestTop:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 90f, 0f);
			break;
		case BlockKind.DiagonalOnWallWestBottom:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 270f, 180f);
			break;
		case BlockKind.DiagonalOnWallEastRight:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 90f, 90f);
			break;
		case BlockKind.DiagonalOnWallEastLeft:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 90f, 270f);
			break;
		case BlockKind.DiagonalOnWallEastTop:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 270f, 0f);
			break;
		case BlockKind.DiagonalOnWallEastBottom:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 90f, 180f);
			break;
		case BlockKind.DiagonalOnWallSouthRight:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 90f, 270f);
			break;
		case BlockKind.DiagonalOnWallSouthLeft:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 270f, 90f);
			break;
		case BlockKind.DiagonalOnWallSouthTop:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
			break;
		case BlockKind.DiagonalOnWallSouthBottom:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 180f, 180f);
			break;
		case BlockKind.DiagonalOnWallNorthRight:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			break;
		case BlockKind.DiagonalOnWallNorthLeft:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
			break;
		case BlockKind.DiagonalOnWallNorthTop:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 180f, 0f);
			break;
		case BlockKind.DiagonalOnWallNorthBottom:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
			break;
		case BlockKind.CornerEast:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 270f, 0f);
			break;
		case BlockKind.CornerNorth:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 180f, 0f);
			break;
		case BlockKind.CornerSouth:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 90f, 0f);
			break;
		case BlockKind.EastFenceWest:
		case BlockKind.EastFenceEast:
		case BlockKind.EastFenceNorth:
		case BlockKind.EastFenceSouth:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
			break;
		case BlockKind.WestFenceWest:
		case BlockKind.WestFenceEast:
		case BlockKind.WestFenceNorth:
		case BlockKind.WestFenceSouth:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
			break;
		case BlockKind.NorthFenceWest:
		case BlockKind.NorthFenceEast:
		case BlockKind.NorthFenceNorth:
		case BlockKind.NorthFenceSouth:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		case BlockKind.SouthFenceWest:
		case BlockKind.SouthFenceEast:
		case BlockKind.SouthFenceNorth:
		case BlockKind.SouthFenceSouth:
			this._Selectel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			break;
		case BlockKind.CornerStairEast:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
			break;
		case BlockKind.CornerStairWest:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 180f, 0f);
			break;
		case BlockKind.CornerStairSouth:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 270f, 0f);
			break;
		case BlockKind.CornerStairNorth:
			this._Selectel.transform.rotation = Quaternion.Euler(270f, 90f, 0f);
			break;
		}
	}

	public void ProcessContinuousDamage()
	{
		if (this.MakeContinuousDamage && this.MainPlayerDead)
		{
			this.MakeContinuousDamage = false;
		}
		if (this.MakeContinuousDamage)
		{
			if (!this._ContinuousDamageEnabled)
			{
				this._ContinuousDamageEnabled = true;
				this._ContinuousDamageLastHitTime = 0f;
			}
			if (Time.time - this._ContinuousDamageLastHitTime >= 1f)
			{
				if (this._ContinuousDamageLastHitTime > 0f)
				{
					this.MakeContinuousDamage = false;
				}
				this._ContinuousDamageLastHitTime = Time.time;
				this.MainPlayerNode.DoDamage(0.4f, true);
				SoundManager.Instance.Play(SoundManager.Sound.FootLava, this.MainPlayerEyes.GetComponent<AudioSource>());
			}
			Color color = this.HitEffect.GetComponent<GUITexture>().color;
			color.a = 0.1f + Mathf.PingPong(Time.time / 2f, 0.4f);
			this.HitEffect.GetComponent<GUITexture>().color = color;
		}
		else if (this._ContinuousDamageEnabled)
		{
			this._ContinuousDamageEnabled = false;
			Color color2 = this.HitEffect.GetComponent<GUITexture>().color;
			color2.a = 0f;
			this.HitEffect.GetComponent<GUITexture>().color = color2;
		}
	}

	private void ProcessLava()
	{
		if (!this.MainPlayer)
		{
			return;
		}
		if (GameType.IsObserving())
		{
			return;
		}
		bool flag = false;
		int num = -1;
		while (num <= 2 && !flag)
		{
			int num2 = -1;
			while (num2 <= 1 && !flag)
			{
				int num3 = -1;
				while (num3 <= 1 && !flag)
				{
					int x = (int)(this.MainPlayer.transform.position.x + 0.6f * (float)num2);
					int y = (int)(this.MainPlayer.transform.position.z + 0.6f * (float)num3);
					int z = (int)(this.MainPlayer.transform.position.y + 0.1f + (float)num);
					if (this._WorldData.GetBlockType(x, y, z) == BlockType.Lava)
					{
						this.MakeContinuousDamage = true;
						flag = true;
					}
					num3++;
				}
				num2++;
			}
			num++;
		}
	}

	private void ProcessIce()
	{
		if (!this.MainPlayer)
		{
			return;
		}
		BlockType blockType = this._WorldData.GetBlockType((int)this.MainPlayerEyes.transform.position.x, (int)this.MainPlayerEyes.transform.position.z, (int)this.MainPlayerEyes.transform.position.y - 2);
		if (blockType == BlockType.Ice)
		{
			this.MainPlayer.SendMessage("SetIceBody", true);
			this._OnIce = true;
		}
		else if (!this._OnIce)
		{
			this.MainPlayer.SendMessage("SetIceBody", false);
		}
		else if (blockType != BlockType.Air)
		{
			this.MainPlayer.SendMessage("SetIceBody", false);
		}
	}

	public bool FaceInWater
	{
		get
		{
			return this._FaceInWater;
		}
	}

	private void ProcessWater()
	{
		if (!this.MainPlayer)
		{
			return;
		}
		Camera raycastCamera = CameraController.RaycastCamera;
		BlockType blockType = this._WorldData.GetBlockType((int)raycastCamera.transform.position.x, (int)(raycastCamera.transform.position.z + 0.15f), (int)raycastCamera.transform.position.y);
		if (blockType == BlockType.Water)
		{
			RenderSettings.fogColor = TimeOfDay.WaterFogColor;
			RenderSettings.fogDensity = 0.04f;
			RenderSettings.skybox = null;
			raycastCamera.transform.GetComponent<ColorCorrectionCurves>().enabled = true;
			if (!this._FaceInWater)
			{
				if (ManagerAudio.AudioLoaded)
				{
					this.Audio.GetComponent<AudioSource>().clip = SoundManager.Instance.GetClip(SoundManager.Sound.LoopUnderwater);
					this.Audio.GetComponent<AudioSource>().volume = ProfileINI.ambient_volume;
					this.Audio.GetComponent<AudioSource>().Play();
					ProfileINI.sound_scale = 0.25f;
				}
				this._FaceInWater = true;
			}
		}
		else
		{
			RenderSettings.fogDensity = 0.007f;
			RenderSettings.fogColor = TimeOfDay.FogColor;
			RenderSettings.skybox = this._DefaultSkyBox;
			raycastCamera.transform.GetComponent<ColorCorrectionCurves>().enabled = false;
			if (this._FaceInWater)
			{
				ProfileINI.sound_scale = 1f;
				SoundManager.Instance.Play(SoundManager.Sound.WaterOut, this.MainPlayerEyes.GetComponent<AudioSource>());
				this.SetAmbientSoundOnLevel();
				this._FaceInWater = false;
				KGUI.ResizeGrid("hud.boobles", 0, delegate(GameObject slot, int index)
				{
				}, null);
			}
		}
		Transform transform = this.MainPlayer.transform;
		int num = (int)transform.position.x;
		int num2 = (int)transform.position.z;
		int num3 = (int)transform.position.y;
		blockType = this._WorldData.GetBlockType(num, num2, num3);
		if (blockType == BlockType.Water)
		{
			bool flag = false;
			float num4 = (float)num3 + 1f;
			if (this._WorldData.GetBlockType(num, num2, num3 + 1) == BlockType.Water)
			{
				num4 += 1f;
			}
			if (num4 - transform.position.y < 0.7f)
			{
				bool flag2 = false;
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						BlockType blockType2 = this._WorldData.GetBlockType(num + i, num2 + j, num3);
						if (blockType2 != BlockType.Water && blockType2 != BlockType.Air)
						{
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					flag = true;
				}
			}
			this.MainPlayer.SendMessage("SetUnderWaterBody", new bool[]
			{
				true,
				flag
			});
		}
		else
		{
			this.MainPlayer.SendMessage("SetUnderWaterBody", new bool[2]);
		}
		if (blockType == BlockType.Water && !this._InWater)
		{
			SoundManager.Instance.Play(SoundManager.Sound.WaterIn, this.MainPlayerEyes.GetComponent<AudioSource>());
			this._InWater = true;
		}
		else if (blockType != BlockType.Water && this._InWater)
		{
			SoundManager.Instance.Play(SoundManager.Sound.WaterOut, this.MainPlayerEyes.GetComponent<AudioSource>());
			this._InWater = false;
		}
		if (this._FaceInWater)
		{
			this._FaceInWaterTime += Time.deltaTime;
			if (App.Instance.Settings.gameType == GameINI.GameType.BUILDING && (Level.Instance.IsAdmin(null) || Level.Instance.IsModerator(null)))
			{
				this._FaceInWaterTime = 0.0001f;
			}
			if (this._FaceInWaterTime >= 20f)
			{
				this.MakeContinuousDamage = true;
			}
		}
		else if (this._FaceInWaterTime > 0f)
		{
			this._FaceInWaterTime -= Time.deltaTime * 3f;
		}
		this._FaceInWaterTime = Mathf.Clamp(this._FaceInWaterTime, 0f, 20f);
		float num5 = Mathf.Clamp01(1f - this._FaceInWaterTime / 20f) * 10f;
		if (num5 != this._WaterBooblesCount)
		{
			this._WaterBooblesCount = num5;
			bool flag3 = this._WaterBooblesCount < 10f && !this.MainPlayerDead;
			int showCount = Mathf.CeilToInt(this._WaterBooblesCount);
			KGUI.ResizeGrid("hud.boobles", (!flag3) ? 0 : showCount, delegate(GameObject slot, int index)
			{
				if (index == showCount - 1)
				{
					slot.GetComponent<UISprite>().fillAmount = this._WaterBooblesCount % 1f;
				}
				else
				{
					slot.GetComponent<UISprite>().fillAmount = 1f;
				}
			}, null);
		}
	}

	public void ClearMap()
	{
		if (this.IsWorldGenerated)
		{
			for (int i = 0; i < this.WorldData.ChunksWide; i++)
			{
				for (int j = 0; j < this.WorldData.ChunksHigh; j++)
				{
					for (int k = 0; k < this.WorldData.ChunksDeep; k++)
					{
						World.DestroyChunk(this.WorldData.Chunks[i, j, k]);
					}
				}
			}
			this.IsWorldGenerated = false;
		}
		GameObject gameObject = GameObject.Find("Chunks");
		foreach (object obj in gameObject.transform)
		{
			Transform transform = (Transform)obj;
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		this._World = null;
	}

	public void CreateNewMap()
	{
		if (this._World.IsHomeWorld)
		{
			MainMenu.Instance.ShowLoading(string.Empty, string.Empty);
			this.ClearEntities();
			this.IsWorldGenerated = false;
			base.StartCoroutine(this._World.GenerateWorldCourutin());
		}
	}

	public void LadderFlyTest()
	{
		if (this.player_on_ladder)
		{
			this.last_ladder_delta += Time.deltaTime;
			if ((double)this.last_ladder_delta > 0.1)
			{
				this.player_on_ladder = false;
				this.MainPlayer.SendMessage("SetLadderBody", false);
				MainMenu.Instance.RefreshFlying();
			}
		}
	}

	private void LateUpdate()
	{
		if (this.MainPlayer != null && this.IsWorldGenerated && !MainMenu.Instance.Flying)
		{
			if (this.MainPlayer.transform.position.x < 1f)
			{
				this.MainPlayer.transform.position = new Vector3(1f, this.MainPlayer.transform.position.y, this.MainPlayer.transform.position.z);
			}
			if (this.MainPlayer.transform.position.z < 1f)
			{
				this.MainPlayer.transform.position = new Vector3(this.MainPlayer.transform.position.x, this.MainPlayer.transform.position.y, 1f);
			}
			if (this.MainPlayer.transform.position.x > (float)this._WorldData.GetMaxBlockX())
			{
				this.MainPlayer.transform.position = new Vector3((float)this._WorldData.GetMaxBlockX(), this.MainPlayer.transform.position.y, this.MainPlayer.transform.position.z);
			}
			if (this.MainPlayer.transform.position.z > (float)this._WorldData.GetMaxBlockY())
			{
				this.MainPlayer.transform.position = new Vector3(this.MainPlayer.transform.position.x, this.MainPlayer.transform.position.y, (float)this._WorldData.GetMaxBlockY());
			}
		}
	}

	private void Update()
	{
		this.BufferUp();
		if (this.IsWorldGenerated)
		{
			this.LadderFlyTest();
			Menu menu = (!(TeamBattle.Instance != null)) ? Menu.TabMenu : ((!(TeamBattle.Instance is Deathmatch)) ? Menu.TeamBattle : Menu.Deathmatch);
			if (!MainMenu.Instance.MenuActive && UnityEngine.Input.GetKeyDown(KeyCode.Tab))
			{
				MainMenu.Instance.ToggleMenu(menu, null, null);
			}
			else if (UnityEngine.Input.GetKeyUp(KeyCode.Tab) && (MainMenu.Instance.CurMenu == menu || MainMenu.Instance.CurMenu == Menu.Profile) && !GameType.IsHideSeek)
			{
				MainMenu.Instance.HideMenu();
			}
			else if (UnityEngine.Input.GetKeyUp(KeyCode.Tab) && (MainMenu.Instance.CurMenu == menu || MainMenu.Instance.CurMenu == Menu.Profile) && GameType.IsHideSeek && TeamBattle.Instance is HideSeek)
			{
				HideSeek hideSeek = (HideSeek)TeamBattle.Instance;
				if (hideSeek.select_side != HideSeek.HIDE_SEEK_TEAM.NONE)
				{
					MainMenu.Instance.HideMenu();
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKeyDown(KeyCode.BackQuote))
			{
				if (MainMenu.Instance.IsShowHint)
				{
					MainMenu.Instance.HideHint();
				}
				else if (MainMenu.Instance.MenuActive)
				{
					if (TeamBattle.Instance is HideSeek)
					{
						if (MainMenu.Instance.CurMenu != Menu.TeamBattle)
						{
							MainMenu.Instance.ToggleMenu(Menu.None, null, null);
						}
					}
					else
					{
						MainMenu.Instance.ToggleMenu(Menu.None, null, null);
					}
				}
				else
				{
					MainMenu.Instance.SwitchMenu(Menu.GameMenu, null, null);
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
			{
				if (!MainMenu.Instance.MenuActive)
				{
					Cursor.lockState = CursorLockMode.Locked;
					this.EnableMouseLook();
				}
				else
				{
					Cursor.lockState = CursorLockMode.None;
				}
			}
			if (this._CheckCheating && DateTime.Now < this._CheatingVotingTime + TimeSpan.FromSeconds(30.0))
			{
				string text = string.Format(Localize.GetText("CHEATER_QUESTIONS", null), this._CheatingPlayerName);
				if (!this._CheatingVoted)
				{
					text += Localize.GetText("CHEATER_QUESTIONS2", null);
				}
				else
				{
					text = text + ((!this._CheatingVotedYes) ? Localize.GetText("CHEATER_NO", null) : Localize.GetText("CHEATER_YES", null)) + "\n";
				}
				text += string.Format(Localize.GetText("CHEATER_TIME", null), 30 - (int)(DateTime.Now - this._CheatingVotingTime).TotalSeconds);
				KGUI.SetControlText("hud.txt_cheating", text);
				if (!this._CheatingVoted && !Chat.IsEnabled() && !MainMenu.Instance.MenuActive)
				{
					KeyCode key = (Localize.Locale != Localize.LocaleType.RU) ? KeyCode.Y : KeyCode.L;
					KeyCode key2 = (Localize.Locale != Localize.LocaleType.RU) ? KeyCode.N : KeyCode.Y;
					if (UnityEngine.Input.GetKeyDown(key) || UnityEngine.Input.GetKeyDown(key2))
					{
						this._CheatingVoted = true;
						this._CheatingVotedYes = UnityEngine.Input.GetKeyDown(key);
						base.photonView.RPC("VoteCheating", PhotonTargets.AllViaServer, new object[]
						{
							this._CheatingVotedYes
						});
					}
				}
			}
			if (this.MainPlayer != null && this.MainPlayer.transform.position.y < 0f)
			{
				this.TeleportMainPlayerToSpawnPoint();
			}
			int actualSkin = ProfileINI.GetActualSkin();
			if (actualSkin != this.MainPlayerNode.Skin && !GameType.IsHungerGamesMode)
			{
				this.MainPlayerNode.Skin = actualSkin;
				base.photonView.RPC("ChangePlayerSkin", PhotonTargets.OthersBuffered, new object[]
				{
					this.MainPlayerNode.Name,
					actualSkin
				});
			}
			this.ProcessWater();
			this.ProcessIce();
			this.ProcessLava();
			this.ProcessContinuousDamage();
			this.UpdateStorePreview(0);
			this.UpdateSelectelAndCursorText();
			if (!Chat.IsEnabled() && !MainMenu.Instance.MenuActive)
			{
				if (!GameType.BattleMode())
				{
					if (this.EntityPreview == null)
					{
						KGUI.EnableNodes("hud_mobile.Entity", false, false);
					}
					else
					{
						KGUI.EnableNodes("hud_mobile.Entity", true, false);
					}
				}
				if (CameraController.Instance.aimZone.GetTouchDuration(0) > 0.5f && !CameraController.Instance.isRotateCamera && this.EntityPreview == null)
				{
					if (this.EntityUnderCursor != null)
					{
						this.EntityUnderCursor.OnButtonE(ProfileINI.nickname);
						MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.None);
					}
				}
				if (CameraController.Instance.aimZone != null && CameraController.Instance.aimZone.JustTapped() && !CameraController.Instance.aimZone.JustDoubleTapped() && this.EntityPreview == null)
				{
					if (this.EntityUnderCursor != null)
					{
						this.EntityUnderCursor.OnButtonF(ProfileINI.nickname);
						MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.None);
					}
					else if (this.MainPlayer != null && !this.MainPlayer.GetComponent<CharacterController>().enabled)
					{
						Trolley.KickPlayer(this.MainPlayer);
						Boat.KickPlayer(this.MainPlayer);
					}
				}
			}
			this._World.RegenerateChunks();
			this.CreateFinishedChunk();
			this.ProcessPlayerInput();
			this.DisplayDiggings();
		}
		else if (this._ZipedChunkReceiveTime != 0f && Time.time - this._ZipedChunkReceiveTime > 5f)
		{
			UnityEngine.Debug.Log("Exit waiting level data");
			if (!App.Instance.Settings.isServer)
			{
				this.ExitGame(string.Empty);
			}
		}
	}

	[PunRPC]
	public void Pause()
	{
		UnityEngine.Debug.Break();
	}

	public void InitializeTextures()
	{
		this.WorldTextureAtlas = new Texture2D(2048, 2048);
		this.WorldData.WorldTextureAtlasUvs = this.WorldTextureAtlas.PackTextures(this.World_Textures, 0);
		this.WorldTextureAtlas.filterMode = FilterMode.Point;
		this.WorldTextureAtlas.anisoLevel = 9;
		this.WorldTextureAtlas.Apply();
		this.WorldData.GenerateUVCoordinatesForAllBlocks();
		ChunkGameObject[] array = (ChunkGameObject[])UnityEngine.Object.FindObjectsOfType(typeof(ChunkGameObject));
		foreach (ChunkGameObject chunkGameObject in array)
		{
			chunkGameObject.GetComponent<MeshRenderer>().material.mainTexture = this.WorldTextureAtlas;
		}
	}

	public void EnableMouseLook()
	{
		if (this.MainPlayer != null)
		{
			this.MainPlayer.GetComponent<MouseLook>().enabled = true;
			this.MainPlayer.transform.Find("recoil/Main Camera").GetComponent<MouseLook>().enabled = true;
		}
	}

	public void DisableMouseLook()
	{
		if (this.MainPlayer != null)
		{
			this.MainPlayer.GetComponent<MouseLook>().enabled = false;
			this.MainPlayer.transform.Find("recoil/Main Camera").GetComponent<MouseLook>().enabled = false;
		}
	}

	private void ProcessTorchFloorAdd(RaycastHit hit, out Vector3 pos)
	{
		Vector3 vector = hit.point + hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)vector.x, (int)vector.z, (int)vector.y);
		if (hit.normal.Equals(new Vector3(0f, 1f, 0f)))
		{
			pos = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
		}
		else
		{
			pos = new Vector3(0f, 0f, 0f);
		}
		BlockKind blockKind = this._WorldData.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
		BlockKind blockKind2 = blockKind;
		if (blockKind2 != BlockKind.Half)
		{
			if (blockKind2 != BlockKind.Quarter)
			{
				if (blockKind2 == BlockKind.Third)
				{
					pos += new Vector3(0f, 0.75f, 0f);
				}
			}
			else
			{
				pos += new Vector3(0f, 0.25f, 0f);
			}
		}
		else
		{
			pos += new Vector3(0f, 0.5f, 0f);
		}
	}

	private void FindClippingPos(RaycastHit hit, out Vector3 pos, out Vector3 rotate)
	{
		Vector3 vector = hit.point + hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)vector.x, (int)vector.z, (int)vector.y);
		rotate = Vector3.zero;
		pos = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
		BlockKind blockKind = this._WorldData.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
		switch (blockKind)
		{
		case (BlockKind)139:
			blockKind = BlockKind.HalfWallNorth;
			break;
		case (BlockKind)140:
			blockKind = BlockKind.QuarterOnWallSouth;
			break;
		case (BlockKind)141:
			blockKind = BlockKind.HalfWallEast;
			break;
		case (BlockKind)142:
			blockKind = BlockKind.HalfWallWest;
			break;
		case (BlockKind)145:
			blockKind = BlockKind.QuarterOnWallEast;
			break;
		case (BlockKind)148:
			blockKind = BlockKind.QuarterOnWallWest;
			break;
		}
		if (hit.normal.z == -1f)
		{
			rotate = new Vector3(0f, 90f, 0f);
			BlockKind blockKind2 = blockKind;
			if (blockKind2 != BlockKind.HalfWallSouth)
			{
				if (blockKind2 != BlockKind.QuarterOnWallSouth)
				{
					if (blockKind2 == BlockKind.ThirdOnWallSouth)
					{
						pos -= new Vector3(0f, 0f, 0.75f);
					}
				}
				else
				{
					pos -= new Vector3(0f, 0f, 0.25f);
				}
			}
			else
			{
				pos -= new Vector3(0f, 0f, 0.5f);
			}
		}
		else if (hit.normal.z > 0f)
		{
			rotate = new Vector3(0f, 270f, 0f);
			BlockKind blockKind2 = blockKind;
			if (blockKind2 != BlockKind.HalfWallNorth)
			{
				if (blockKind2 != BlockKind.QuarterOnWallNorth)
				{
					if (blockKind2 == BlockKind.ThirdOnWallNorth)
					{
						pos += new Vector3(0f, 0f, 0.75f);
					}
				}
				else
				{
					pos += new Vector3(0f, 0f, 0.25f);
				}
			}
			else
			{
				pos += new Vector3(0f, 0f, 0.5f);
			}
		}
		else if (hit.normal.x < 0f)
		{
			rotate = new Vector3(0f, 180f, 0f);
			BlockKind blockKind2 = blockKind;
			if (blockKind2 != BlockKind.HalfWallEast)
			{
				if (blockKind2 != BlockKind.QuarterOnWallEast)
				{
					if (blockKind2 == BlockKind.ThirdOnWallEast)
					{
						pos -= new Vector3(0.75f, 0f, 0f);
					}
				}
				else
				{
					pos -= new Vector3(0.25f, 0f, 0f);
				}
			}
			else
			{
				pos -= new Vector3(0.5f, 0f, 0f);
			}
		}
		else if (hit.normal.x > 0f)
		{
			rotate = new Vector3(0f, 0f, 0f);
			BlockKind blockKind2 = blockKind;
			if (blockKind2 != BlockKind.HalfWallWest)
			{
				if (blockKind2 != BlockKind.QuarterOnWallWest)
				{
					if (blockKind2 == BlockKind.ThirdOnWallWest)
					{
						pos += new Vector3(0.75f, 0f, 0f);
					}
				}
				else
				{
					pos += new Vector3(0.25f, 0f, 0f);
				}
			}
			else
			{
				pos += new Vector3(0.5f, 0f, 0f);
			}
		}
		else if (hit.normal.y > 0f)
		{
			rotate = new Vector3(0f, 0f, 90f);
			pos += new Vector3(0.5f, 0.5f, 0f);
			BlockKind blockKind2 = blockKind;
			if (blockKind2 != BlockKind.Half)
			{
				if (blockKind2 != BlockKind.Quarter)
				{
					if (blockKind2 == BlockKind.Third)
					{
						pos += new Vector3(0f, 0.75f, 0f);
					}
				}
				else
				{
					pos += new Vector3(0f, 0.25f, 0f);
				}
			}
			else
			{
				pos += new Vector3(0f, 0.5f, 0f);
			}
		}
		else if (hit.normal.y < 0f)
		{
			rotate = new Vector3(0f, 0f, 270f);
			pos += new Vector3(-0.5f, 0.5f, 0f);
			if (blockKind.IsFlip() && blockKind.DoFlip() == BlockKind.Half)
			{
				pos += new Vector3(0f, -0.5f, 0f);
			}
		}
		if (Vector3.Angle(hit.normal, new Vector3(0.7f, 0f, -0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallEastLeft)
		{
			rotate = new Vector3(0f, 45f, 0f);
			pos += new Vector3(0.35f, 0f, -0.35f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(0.7f, 0f, 0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallEastRight)
		{
			rotate = new Vector3(0f, -45f, 0f);
			pos += new Vector3(0.35f, 0f, 0.35f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(0.7f, -0.7f, 0f)) == 0f && blockKind == BlockKind.DiagonalOnWallEastBottom)
		{
			rotate = new Vector3(0f, 0f, -45f);
			pos += new Vector3(0.05f, -0.2f, 0f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(0.7f, 0.7f, 0f)) == 0f && blockKind == BlockKind.DiagonalOnWallEastTop)
		{
			rotate = new Vector3(0f, 0f, 45f);
			pos += new Vector3(0.7f, 0.5f, 0f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(-0.7f, 0f, -0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallSouthLeft)
		{
			rotate = new Vector3(0f, -225f, 0f);
			pos += new Vector3(-0.35f, 0f, -0.35f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(0.7f, 0f, -0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallSouthRight)
		{
			rotate = new Vector3(0f, 45f, 0f);
			pos += new Vector3(0.35f, 0f, -0.35f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(0f, -0.7f, -0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallSouthBottom)
		{
			rotate = new Vector3(0f, 90f, -45f);
			pos += new Vector3(0.5f, -0.7f, 0f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(0f, 0.7f, -0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallSouthTop)
		{
			rotate = new Vector3(0f, 90f, 45f);
			pos += new Vector3(-0.5f, 0f, -0.7f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(-0.7f, 0f, 0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallWestLeft)
		{
			rotate = new Vector3(0f, 225f, 0f);
			pos += new Vector3(-0.35f, 0f, 0.35f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(-0.7f, 0f, -0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallWestRight)
		{
			rotate = new Vector3(0f, -225f, 0f);
			pos += new Vector3(-0.35f, 0f, -0.35f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(-0.7f, -0.7f, 0f)) == 0f && blockKind == BlockKind.DiagonalOnWallWestBottom)
		{
			rotate = new Vector3(0f, 180f, -45f);
			pos += new Vector3(0f, -0.2f, 0f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(-0.7f, 0.7f, 0f)) == 0f && blockKind == BlockKind.DiagonalOnWallWestTop)
		{
			rotate = new Vector3(0f, 180f, 45f);
			pos += new Vector3(-0.7f, 0.5f, 0f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(0.7f, 0f, 0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallNorthLeft)
		{
			rotate = new Vector3(0f, -45f, 0f);
			pos += new Vector3(0.35f, 0f, 0.35f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(-0.7f, 0f, 0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallNorthRight)
		{
			rotate = new Vector3(0f, 225f, 0f);
			pos += new Vector3(-0.35f, 0f, 0.35f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(0f, -0.7f, 0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallNorthBottom)
		{
			rotate = new Vector3(0f, -90f, -45f);
			pos += new Vector3(0f, -0.2f, 0f);
		}
		if (Vector3.Angle(hit.normal, new Vector3(0f, 0.7f, 0.7f)) == 0f && blockKind == BlockKind.DiagonalOnWallNorthTop)
		{
			rotate = new Vector3(0f, -90f, 45f);
			pos += new Vector3(0f, 0.5f, 0.7f);
		}
	}

	private void ProcessTorchAdd(RaycastHit hit, out Vector3 pos, out Vector3 rotate)
	{
		Vector3 vector = hit.point + hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)vector.x, (int)vector.z, (int)vector.y);
		BlockKind blockKind = this._WorldData.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
		switch (blockKind)
		{
		case (BlockKind)139:
			blockKind = BlockKind.HalfWallNorth;
			break;
		case (BlockKind)140:
			blockKind = BlockKind.QuarterOnWallSouth;
			break;
		case (BlockKind)141:
			blockKind = BlockKind.HalfWallEast;
			break;
		case (BlockKind)142:
			blockKind = BlockKind.HalfWallWest;
			break;
		case (BlockKind)145:
			blockKind = BlockKind.QuarterOnWallEast;
			break;
		case (BlockKind)148:
			blockKind = BlockKind.QuarterOnWallWest;
			break;
		}
		if (hit.normal.z < 0f)
		{
			pos = new Vector3((float)intVect.X + 0.51f, (float)intVect.Z + 0.01f, (float)intVect.Y + 0.49f);
			rotate = new Vector3(270f, 270f, 0f);
			BlockKind blockKind2 = blockKind;
			if (blockKind2 != BlockKind.HalfWallSouth)
			{
				if (blockKind2 != BlockKind.QuarterOnWallSouth)
				{
					if (blockKind2 == BlockKind.ThirdOnWallSouth)
					{
						pos -= new Vector3(0f, 0f, 0.75f);
					}
				}
				else
				{
					pos -= new Vector3(0f, 0f, 0.25f);
				}
			}
			else
			{
				pos -= new Vector3(0f, 0f, 0.5f);
			}
		}
		else if (hit.normal.z > 0f)
		{
			pos = new Vector3((float)intVect.X + 0.51f, (float)intVect.Z + 0.01f, (float)intVect.Y + 0.49f);
			rotate = new Vector3(270f, 90f, 0f);
			BlockKind blockKind2 = blockKind;
			if (blockKind2 != BlockKind.HalfWallNorth)
			{
				if (blockKind2 != BlockKind.QuarterOnWallNorth)
				{
					if (blockKind2 == BlockKind.ThirdOnWallNorth)
					{
						pos += new Vector3(0f, 0f, 0.75f);
					}
				}
				else
				{
					pos += new Vector3(0f, 0f, 0.25f);
				}
			}
			else
			{
				pos += new Vector3(0f, 0f, 0.5f);
			}
		}
		else if (hit.normal.x < 0f)
		{
			pos = new Vector3((float)intVect.X + 0.51f, (float)intVect.Z + 0.01f, (float)intVect.Y + 0.49f);
			rotate = new Vector3(270f, 0f, 0f);
			BlockKind blockKind2 = blockKind;
			if (blockKind2 != BlockKind.HalfWallEast)
			{
				if (blockKind2 != BlockKind.QuarterOnWallEast)
				{
					if (blockKind2 == BlockKind.ThirdOnWallEast)
					{
						pos -= new Vector3(0.75f, 0f, 0f);
					}
				}
				else
				{
					pos -= new Vector3(0.25f, 0f, 0f);
				}
			}
			else
			{
				pos -= new Vector3(0.5f, 0f, 0f);
			}
		}
		else if (hit.normal.x > 0f)
		{
			pos = new Vector3((float)intVect.X + 0.51f, (float)intVect.Z + 0.01f, (float)intVect.Y + 0.49f);
			rotate = new Vector3(270f, 180f, 0f);
			BlockKind blockKind2 = blockKind;
			if (blockKind2 != BlockKind.HalfWallWest)
			{
				if (blockKind2 != BlockKind.QuarterOnWallWest)
				{
					if (blockKind2 == BlockKind.ThirdOnWallWest)
					{
						pos += new Vector3(0.75f, 0f, 0f);
					}
				}
				else
				{
					pos += new Vector3(0.25f, 0f, 0f);
				}
			}
			else
			{
				pos += new Vector3(0.5f, 0f, 0f);
			}
		}
		else
		{
			pos = new Vector3(0f, 0f, 0f);
			rotate = new Vector3(0f, 0f, 0f);
		}
	}

	public void ToggleInventory()
	{
		if (!Input.anyKey || this.MainPlayerDead || Chat.IsEnabled() || Book.Current != null)
		{
			return;
		}
		if (App.Instance.Settings.isWatch || GameType.BuildDisabled())
		{
			return;
		}
		MainMenu.Instance.ToggleMenu(Menu.Inventory, null, null);
		MainMenu.Instance.SetMenu(Menu.ItemsPack, false, null, null);
		if (MainMenu.Instance.CurMenu == Menu.Inventory && this.EntityPreview != null)
		{
			this.SetEntityFromPreview();
		}
	}

	public void ToggleEmotions()
	{
		if (App.Instance.Settings.isWatch)
		{
			return;
		}
		MainMenu.Instance.ToggleMenu(Menu.Emotions, null, null);
	}

	public void ToggleShop()
	{
		if (App.Instance.Settings.isWatch)
		{
			return;
		}
		MainMenu.Instance.ToggleMenu(Menu.Shop, null, null);
		MainMenu.Instance.SetMenu(Menu.ItemsPack, false, null, null);
	}

	public void ToggleSave()
	{
		if (App.Instance.Settings.isWatch || !Level.Instance.IsAdmin(null) || !GameType.CanSaveMap())
		{
			return;
		}
	}

	private void ProcessPlayerInput()
	{
		if (!Input.anyKey || this.MainPlayerDead || Chat.IsEnabled() || Book.Current != null)
		{
			return;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
		{
			this.ToggleEmotions();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.M) && !GameType.IsHungerGamesMode)
		{
			this.ToggleShop();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.F10) && GameType.CanSaveMap())
		{
			Protect.Instance.CheckDLL(UnityEngine.Random.Range(0, 14));
			MainMenu.Instance.SaveMap(null);
		}
	}

	public void OnBuildTerrain()
	{
		if (GameType.IsZombieGamesMode && this.MainPlayerNode.ZombieBlock <= 0)
		{
			Chat.SendWarning(Localize.GetText("CANT_BUILD_ZT_END", null), false);
			return;
		}
		if (App.Instance.Settings.isWatch)
		{
			return;
		}
		if (GameType.IsZombieGamesMode)
		{
			return;
		}
		if (GameType.IsHungerGamesMode)
		{
			return;
		}
		if (GameType.IsHideSeek)
		{
			return;
		}
		if (!Level.Instance.CanBuild)
		{
			Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP", null), false);
			return;
		}
		if (CameraController.Instance == null)
		{
			return;
		}
		if (this.CurrentBlock == BlockType.Air && this.CurrentBlockEntity == EntityType.AIR)
		{
			return;
		}
		Ray ray = CameraController.RaycastCamera.ScreenPointToRay(new Vector3(CameraController.Instance.aimZone.GetTapPos(TouchCoordSys.SCREEN_PX).x, (float)Screen.height - CameraController.Instance.aimZone.GetTapPos(TouchCoordSys.SCREEN_PX).y, 0f));
		int layerMask = 1 << LayerMask.NameToLayer("Terrain");
		int layerMask2 = 1 << LayerMask.NameToLayer("Water");
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, CameraController.RaycastDistance, layerMask) || Physics.Raycast(ray, out hit, CameraController.RaycastDistance, layerMask2))
		{
			Vector3 vector = hit.point + hit.normal * 0.01f;
			IntVect intVect = new IntVect((int)vector.x, (int)vector.z, (int)vector.y);
			if (this.CurrentBlock != BlockType.Air)
			{
				if (Store.Blocks[this.CurrentBlock].Purchase.Value != StorePurchase.NONE && ProfileINI.GetPurchaseValue(Store.Blocks[this.CurrentBlock].Purchase) <= 0)
				{
					return;
				}
				if (!this.BuildCheckCollisionWithPlayer(hit))
				{
					return;
				}
				if (Rail.Find(intVect.X, intVect.Y, intVect.Z) != null)
				{
					return;
				}
				BlockType blockType = this._WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z);
				BlockKind blockKind = this._WorldData.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
				int num = (int)blockKind;
				if (num != 129)
				{
					if (num != 144)
					{
						if (num == 153)
						{
							blockKind = BlockKind.Third;
						}
					}
					else
					{
						blockKind = BlockKind.Quarter;
					}
				}
				else
				{
					blockKind = BlockKind.Half;
				}
				BlockType blockType2 = this._WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z + 1);
				BlockType blockType3 = this._WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z - 1);
				IntVect intVect2 = intVect;
				this.CurrentKind = this.CurrentCommonKind.GetKind(ray.direction, hit.normal.normalized);
				if (this.Preview != null && this.Preview.IsPreview)
				{
					this.CurrentKind = this.Preview.Kind;
				}
				BlockKind blockKind2 = this.CurrentKind;
				if (blockKind == BlockKind.Half && this.CurrentKind == BlockKind.Half && blockType == this.CurrentBlock)
				{
					blockKind2 = BlockKind.Default;
				}
				else
				{
					if (blockKind == BlockKind.HalfWallEast && this.CurrentKind == BlockKind.HalfWallEast && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.Default;
					}
					if (blockKind == BlockKind.HalfWallWest && this.CurrentKind == BlockKind.HalfWallWest && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.Default;
					}
					if (blockKind == BlockKind.HalfWallSouth && this.CurrentKind == BlockKind.HalfWallSouth && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.Default;
					}
					if (blockKind == BlockKind.HalfWallNorth && this.CurrentKind == BlockKind.HalfWallNorth && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.Default;
					}
				}
				if (blockKind == BlockKind.Quarter && this.CurrentKind == BlockKind.Quarter && blockType == this.CurrentBlock)
				{
					blockKind2 = BlockKind.Half;
				}
				else
				{
					if (blockKind == BlockKind.QuarterOnWallEast && this.CurrentKind == BlockKind.QuarterOnWallEast && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.HalfWallEast;
					}
					if (blockKind == BlockKind.QuarterOnWallWest && this.CurrentKind == BlockKind.QuarterOnWallWest && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.HalfWallWest;
					}
					if (blockKind == BlockKind.QuarterOnWallSouth && this.CurrentKind == BlockKind.QuarterOnWallSouth && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.HalfWallSouth;
					}
					if (blockKind == BlockKind.QuarterOnWallNorth && this.CurrentKind == BlockKind.QuarterOnWallNorth && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.HalfWallNorth;
					}
				}
				if (blockKind == BlockKind.Half && this.CurrentKind == BlockKind.Quarter && blockType == this.CurrentBlock)
				{
					blockKind2 = BlockKind.Third;
				}
				else
				{
					if (blockKind == BlockKind.HalfWallEast && this.CurrentKind == BlockKind.QuarterOnWallEast && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.ThirdOnWallEast;
					}
					if (blockKind == BlockKind.HalfWallWest && this.CurrentKind == BlockKind.QuarterOnWallWest && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.ThirdOnWallWest;
					}
					if (blockKind == BlockKind.HalfWallSouth && this.CurrentKind == BlockKind.QuarterOnWallSouth && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.ThirdOnWallSouth;
					}
					if (blockKind == BlockKind.HalfWallNorth && this.CurrentKind == BlockKind.QuarterOnWallNorth && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.ThirdOnWallNorth;
					}
				}
				if (blockKind == BlockKind.Quarter && this.CurrentKind == BlockKind.Half && blockType == this.CurrentBlock)
				{
					blockKind2 = BlockKind.Third;
				}
				else
				{
					if (blockKind == BlockKind.QuarterOnWallEast && this.CurrentKind == BlockKind.HalfWallEast && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.ThirdOnWallEast;
					}
					if (blockKind == BlockKind.QuarterOnWallWest && this.CurrentKind == BlockKind.HalfWallWest && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.ThirdOnWallWest;
					}
					if (blockKind == BlockKind.QuarterOnWallSouth && this.CurrentKind == BlockKind.HalfWallSouth && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.ThirdOnWallSouth;
					}
					if (blockKind == BlockKind.QuarterOnWallNorth && this.CurrentKind == BlockKind.HalfWallNorth && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.ThirdOnWallNorth;
					}
				}
				if (blockKind == BlockKind.Third && this.CurrentKind == BlockKind.Quarter && blockType == this.CurrentBlock)
				{
					blockKind2 = BlockKind.Default;
				}
				else
				{
					if (blockKind == BlockKind.ThirdOnWallEast && this.CurrentKind == BlockKind.QuarterOnWallEast && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.Default;
					}
					if (blockKind == BlockKind.ThirdOnWallWest && this.CurrentKind == BlockKind.QuarterOnWallWest && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.Default;
					}
					if (blockKind == BlockKind.ThirdOnWallSouth && this.CurrentKind == BlockKind.QuarterOnWallSouth && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.Default;
					}
					if (blockKind == BlockKind.ThirdOnWallNorth && this.CurrentKind == BlockKind.QuarterOnWallNorth && blockType == this.CurrentBlock)
					{
						blockKind2 = BlockKind.Default;
					}
				}
				if (blockKind.IsFence() && blockKind2.IsFence())
				{
					if (blockKind == BlockKind.Fence && (blockKind2 == BlockKind.FenceOnWallSouthNorth || blockKind2 == BlockKind.FenceOnWallEastWest))
					{
						if (Vector3.Angle(hit.normal, new Vector3(1f, 0f, 0f)) == 0f)
						{
							intVect2 = new IntVect(intVect2.X + 1, intVect2.Y, intVect2.Z);
						}
						if (Vector3.Angle(hit.normal, new Vector3(-1f, 0f, 0f)) == 0f)
						{
							intVect2 = new IntVect(intVect2.X - 1, intVect2.Y, intVect2.Z);
						}
						if (Vector3.Angle(hit.normal, new Vector3(0f, 0f, 1f)) == 0f)
						{
							intVect2 = new IntVect(intVect2.X, intVect2.Y + 1, intVect2.Z);
						}
						if (Vector3.Angle(hit.normal, new Vector3(0f, 0f, -1f)) == 0f)
						{
							intVect2 = new IntVect(intVect2.X, intVect2.Y - 1, intVect2.Z);
						}
					}
					else if (blockKind == BlockKind.FenceOnWallEastWest && blockKind2 == BlockKind.FenceOnWallSouthNorth)
					{
						if (Vector3.Angle(hit.normal, new Vector3(0f, 0f, 1f)) == 0f)
						{
							intVect2 = new IntVect(intVect2.X, intVect2.Y + 1, intVect2.Z);
						}
						if (Vector3.Angle(hit.normal, new Vector3(0f, 0f, -1f)) == 0f)
						{
							intVect2 = new IntVect(intVect2.X, intVect2.Y - 1, intVect2.Z);
						}
					}
					else if (blockKind == BlockKind.FenceOnWallSouthNorth && blockKind2 == BlockKind.FenceOnWallEastWest)
					{
						if (Vector3.Angle(hit.normal, new Vector3(1f, 0f, 0f)) == 0f)
						{
							intVect2 = new IntVect(intVect2.X + 1, intVect2.Y, intVect2.Z);
						}
						if (Vector3.Angle(hit.normal, new Vector3(-1f, 0f, 0f)) == 0f)
						{
							intVect2 = new IntVect(intVect2.X - 1, intVect2.Y, intVect2.Z);
						}
					}
					else if ((blockKind == BlockKind.FenceOnWallSouthNorth || blockKind == BlockKind.FenceOnWallEastWest) && blockKind2 == BlockKind.Fence)
					{
						if (Vector3.Angle(hit.normal, new Vector3(0f, 1f, 0f)) == 0f)
						{
							intVect2 = new IntVect(intVect2.X, intVect2.Y, intVect2.Z + 1);
						}
						if (Vector3.Angle(hit.normal, new Vector3(0f, -1f, 0f)) == 0f)
						{
							intVect2 = new IntVect(intVect2.X, intVect2.Y, intVect2.Z - 1);
						}
					}
				}
				if ((blockType == BlockType.Water && this.CurrentBlock == BlockType.Water) || ((blockKind.IsHalf() || blockKind.IsQuarter() || blockKind.IsThird()) && (blockKind2.IsStair() || blockKind2.IsDiagonal())) || ((blockKind2.IsHalf() || blockKind2.IsQuarter() || blockKind2.IsThird()) && (blockKind.IsStair() || blockKind.IsDiagonal())) || ((blockKind.IsDiagonal() || blockKind.IsStair()) && (blockKind2.IsStair() || blockKind2.IsDiagonal())))
				{
					intVect2 = new IntVect(intVect2.X, intVect2.Y, intVect2.Z + 1);
				}
				bool flag = blockType2 != BlockType.Air && blockType3 == BlockType.Air;
				if (flag)
				{
					blockKind2 = blockKind2.DoFlip();
				}
				if (GameType.IsZombieGamesMode && this.MainPlayerNode.ZombieBlock > 0)
				{
					base.photonView.RPC("PlayerZombieGameBuildBlock", PhotonTargets.All, new object[]
					{
						this.MainPlayerNode.Name
					});
				}
				base.photonView.RPC("AddBlockAt", PhotonTargets.All, new object[]
				{
					(int)this.CurrentBlock,
					(int)blockKind2,
					intVect2.X,
					intVect2.Y,
					intVect2.Z
				});
			}
			else
			{
				if (Store.Entities[this.CurrentBlockEntity].Purchase.Value != StorePurchase.NONE && ProfileINI.GetPurchaseValue(Store.Entities[this.CurrentBlockEntity].Purchase) <= 0)
				{
					return;
				}
				if (this.CurrentBlockEntity == EntityType.RAIL)
				{
					bool flag2 = false;
					Rail rail = Rail.Find(intVect.X, intVect.Y, intVect.Z);
					if (rail != null)
					{
						for (int i = 0; i < 6; i++)
						{
							if (++WorldGameObjectX._CurBlockSibling > 5)
							{
								WorldGameObjectX._CurBlockSibling = 0;
							}
							if (WorldGameObjectX._CurBlockSibling != rail.PlaneIndex)
							{
								int num2 = (WorldGameObjectX._CurBlockSibling != 3) ? ((WorldGameObjectX._CurBlockSibling != 2) ? 0 : -1) : 1;
								int num3 = (WorldGameObjectX._CurBlockSibling != 5) ? ((WorldGameObjectX._CurBlockSibling != 4) ? 0 : -1) : 1;
								int num4 = (WorldGameObjectX._CurBlockSibling != 1) ? ((WorldGameObjectX._CurBlockSibling != 0) ? 0 : -1) : 1;
								if (this._WorldData.GetBlockType(intVect.X + num2, intVect.Y + num3, intVect.Z + num4) != BlockType.Air)
								{
									rail.SelfDelete();
									rail = null;
									vector = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
									vector += Rail.PlanePosition[WorldGameObjectX._CurBlockSibling];
									flag2 = true;
									break;
								}
							}
						}
						if (rail != null)
						{
							return;
						}
					}
					else if (this._WorldData.GetBlockType(intVect.X, intVect.Y, intVect.Z) == BlockType.Water)
					{
						return;
					}
					if (!flag2)
					{
						MainMenu.Instance.PurchaseUse(StorePurchase.RAIL, false);
					}
					else
					{
						base.photonView.RPC("ReplaceRail", PhotonTargets.AllViaServer, new object[0]);
					}
				}
				else
				{
					MainMenu.Instance.PurchaseUse(Store.Entities[this.CurrentBlockEntity].Purchase, false);
				}
				PhotonView photonView = base.photonView;
				string methodName = "AddEntityNetwork";
				PhotonTargets target = PhotonTargets.MasterClient;
				object[] array = new object[6];
				array[0] = (int)this.CurrentBlockEntity;
				array[1] = vector;
				array[2] = Quaternion.identity;
				array[3] = ProfileINI.nickname;
				array[4] = VKAPI.INSTANCE._viewerId;
				photonView.RPC(methodName, target, array);
			}
		}
	}

	public bool BuildCheckCollisionWithPlayer(RaycastHit hit)
	{
		for (int i = 0; i < this.PlayerList.Count; i++)
		{
			if (!(this.PlayerList[i].Avatar == null))
			{
				float magnitude = (this.PlayerList[i].Avatar.transform.position - hit.point).magnitude;
				if ((double)magnitude < 1.2)
				{
					return false;
				}
			}
		}
		if (hit.normal.Equals(new Vector3(0f, 1f, 0f)))
		{
			float magnitude2 = (this.MainPlayer.transform.position - hit.point).magnitude;
			if ((double)magnitude2 < 1.2)
			{
				return false;
			}
		}
		else if ((double)hit.distance < 1.6)
		{
			return false;
		}
		return true;
	}

	public void OnPlayerHitTerrain()
	{
		if (CameraController.Instance == null)
		{
			return;
		}
		Ray ray = CameraController.RaycastCamera.ScreenPointToRay(new Vector3(CameraController.Instance.aimZone.GetPos(0, TouchCoordSys.SCREEN_PX).x, (float)Screen.height - CameraController.Instance.aimZone.GetPos(0, TouchCoordSys.SCREEN_PX).y, 0f));
		int layerMask = 1 << LayerMask.NameToLayer("Entity") | 1 << LayerMask.NameToLayer("SmallDecor");
		RaycastHit raycastHit;
		if (!GameType.HitDisabled() && Physics.Raycast(ray, out raycastHit, CameraController.RaycastDistance, layerMask) && !GameType.IsHideSeek)
		{
			if (App.Instance.Settings.isWatch)
			{
				return;
			}
			EntityBase.FindEntity(raycastHit.transform.gameObject).OnLeftMouseHit(ProfileINI.nickname);
			return;
		}
		else
		{
			layerMask = 1 << LayerMask.NameToLayer("Terrain");
			if (!GameType.DigDisabled() && Physics.Raycast(ray, out raycastHit, CameraController.RaycastDistance, layerMask) && !GameType.IsHideSeek)
			{
				if (App.Instance.Settings.isWatch)
				{
					return;
				}
				if (GameType.IsHungerGamesMode)
				{
					return;
				}
				Vector3 vector = raycastHit.point + ray.direction.normalized * 0.01f;
				IntVect targetLocation = new IntVect((int)vector.x, (int)vector.z, (int)vector.y);
				this._World.Dig(targetLocation, raycastHit.point, raycastHit.normal, false);
				return;
			}
			else
			{
				int layerMask2 = 1 << LayerMask.NameToLayer("Water");
				if (GameType.DigDisabled() || !Physics.Raycast(ray, out raycastHit, CameraController.RaycastDistance, layerMask2) || GameType.IsHideSeek)
				{
					if (GameType.BattleMode() && !GameType.IsObserving())
					{
						layerMask = 1 << LayerMask.NameToLayer("Players");
						if (Physics.Raycast(ray, out raycastHit, CameraController.RaycastDistance, layerMask) && raycastHit.collider.gameObject != this.MainPlayer)
						{
							PlayerNode playerNode = this.FindPlayerByAvatar(raycastHit.collider.gameObject);
							float num = Vector3.Distance(raycastHit.collider.gameObject.transform.position, this.MainPlayer.transform.position);
							if (playerNode != null && TeamBattle.Instance.IsOpposite(playerNode.NetPlayer.name))
							{
								playerNode.Avatar.GetComponent<PhotonView>().RPC("OnHit", PhotonTargets.All, new object[]
								{
									this.MainPlayer.transform.forward,
									this.MainPlayer.transform.position
								});
								this.PunchPlayer(playerNode.NetPlayer, this.MainPlayer.transform.forward, 0.5f, this.MainPlayer.GetComponent<PlayerNetwork>().Knife.arrayId, playerNode.Avatar, false);
							}
							return;
						}
					}
					SoundManager.Instance.Play(SoundManager.Sound.WeaponSwish, this.MainPlayerEyes.GetComponent<AudioSource>());
					return;
				}
				if (App.Instance.Settings.isWatch)
				{
					return;
				}
				if (GameType.IsHungerGamesMode)
				{
					return;
				}
				Vector3 vector2 = raycastHit.point + ray.direction.normalized * 0.01f;
				IntVect targetLocation2 = new IntVect((int)vector2.x, (int)vector2.z, (int)vector2.y);
				this._World.Dig(targetLocation2, raycastHit.point, raycastHit.normal, false);
				return;
			}
		}
	}

	public void SendSoundRpc(int sound_type, Vector3 sound_pos)
	{
		if (this.bSkipSoundRpc)
		{
			this.bSkipSoundRpc = !this.bSkipSoundRpc;
			return;
		}
		base.photonView.RPC("SoundRPC", PhotonTargets.Others, new object[]
		{
			sound_type,
			sound_pos
		});
		this.bSkipSoundRpc = !this.bSkipSoundRpc;
	}

	[PunRPC]
	public void SoundRPC(int sound_type, Vector3 sound_pos)
	{
		if (this.MainPlayer != null)
		{
			SoundManager.Instance.PlayAtPoint((SoundManager.Sound)sound_type, sound_pos);
		}
	}

	[PunRPC]
	private void RemoveBlockAt(int x, int y, int z, bool silent, bool isArcadeCall, PhotonMessageInfo info = null)
	{
		if (!App.Instance.Settings.destroyable)
		{
			return;
		}
		if (this.IsWorldGenerated)
		{
			if (Level.Instance.IsBuilder(info.sender.name) || isArcadeCall)
			{
				this._World.RemoveBlockAt(new IntVect(x, y, z), silent, isArcadeCall);
			}
		}
		else
		{
			this.LevelDeltaLoading.Add(BlockType.Air, x, y, z, true, BlockKind.Default);
		}
	}

	[PunRPC]
	private void RemoveBlocksAt(int x, int y, int z, int radius, PhotonMessageInfo info)
	{
		if (!App.Instance.Settings.destroyable)
		{
			return;
		}
		if (this.IsWorldGenerated)
		{
			if (Level.Instance.IsBuilder(info.sender.name))
			{
				this._World.RemoveBlocksAt(new IntVect(x, y, z), radius);
			}
		}
		else
		{
			for (int i = -radius; i <= radius; i++)
			{
				for (int j = -radius; j <= radius; j++)
				{
					for (int k = -radius; k <= radius; k++)
					{
						this.LevelDeltaLoading.Add(BlockType.Air, x + i, y + j, z + k, true, BlockKind.Default);
					}
				}
			}
		}
	}

	[PunRPC]
	private void AddBlockAt(int id, int kind, int x, int y, int z, PhotonMessageInfo info)
	{
		if (this.IsWorldGenerated)
		{
			if (Level.Instance.IsBuilder(info.sender.name))
			{
				this._World.AddBlockAt(new IntVect(x, y, z), (BlockType)id, (BlockKind)kind);
			}
		}
		else
		{
			this.LevelDeltaLoading.Add((BlockType)id, x, y, z, false, (BlockKind)kind);
		}
	}

	public void CreateFinishedChunk()
	{
		while (this.WorldData.FinishedLandChunks.Count > 0)
		{
			this.ProcessChunkGameObject(this.WorldData.GetFinishedLandChunk(), 0);
		}
		while (this.WorldData.FinishedWaterChunks.Count > 0)
		{
			this.ProcessChunkGameObject(this.WorldData.GetFinishedWaterChunk(), 1);
		}
		while (this.WorldData.FinishedGlassChunks.Count > 0)
		{
			this.ProcessChunkGameObject(this.WorldData.GetFinishedGlassChunk(), 2);
		}
	}

	private void OnApplicationQuit()
	{
	}

	[PunRPC]
	public void AddEntityNetwork(int type, Vector3 pos, Quaternion rot, string creator, string creatorID, object[] data)
	{
		Protect.Instance.CheckDLL(UnityEngine.Random.Range(0, 14));
		if (Info.Instance.GameMode != "BUILDING")
		{
			return;
		}
		SoundManager.Instance.Play(SoundManager.Sound.SetDecor, this.MainPlayerEyes.GetComponent<AudioSource>());
		type = Mathf.Abs(type);
		this._World.AddEntityNetwork((EntityType)type, pos, rot, creator, creatorID, data);
	}

	[PunRPC]
	public void MoveEntityNetwork(int entityViewID, Vector3 pos, Quaternion rot, PhotonMessageInfo info)
	{
		if (Info.Instance.GameMode != "BUILDING")
		{
			return;
		}
		SoundManager.Instance.Play(SoundManager.Sound.SetDecor, this.MainPlayerEyes.GetComponent<AudioSource>());
		PhotonView photonView = PhotonView.Find(entityViewID);
		if (photonView == null)
		{
			return;
		}
		EntityBase component = photonView.GetComponent<EntityBase>();
		if (component == null)
		{
			return;
		}
		component.transform.position = pos;
		component.transform.rotation = rot;
		component.ObjectIsMoved();
		if (component is Trolley)
		{
			((Trolley)component).OnManualMove();
		}
	}

	[PunRPC]
	public void DeleteEntityNetworkAll(int id, PhotonMessageInfo info)
	{
		if (!Level.Instance.IsBuilder(info.sender.name))
		{
			return;
		}
		if (PhotonNetwork.isMasterClient)
		{
			return;
		}
		EntityBase entityBase = this.FindEntityByID(id);
		if (entityBase != null)
		{
			EntityBase.Entities.Remove(entityBase);
			entityBase.SendMessage("Destruction");
		}
	}

	[PunRPC]
	public void DeleteEntityNetworkMasterClient(int id, PhotonMessageInfo info)
	{
		if (!Level.Instance.IsBuilder(info.sender.name))
		{
			return;
		}
		EntityBase entityBase = this.FindEntityByID(id);
		if (entityBase != null)
		{
			EntityBase.Entities.Remove(entityBase);
			entityBase.SendMessage("Destruction");
			PhotonNetwork.Destroy(entityBase.gameObject);
		}
	}

	public void SetEntityFromPreview()
	{
		if (this.EntityPreview == null)
		{
			return;
		}
		if (!this.EntityPreviewIsTaked)
		{
			base.photonView.RPC("AddEntityNetwork", PhotonTargets.MasterClient, new object[]
			{
				(int)this.EntityPreview.Type,
				this.EntityPreview.transform.position,
				this.EntityPreview.transform.rotation,
				ProfileINI.nickname,
				VKAPI.INSTANCE._viewerId,
				this.EntityPreview.GetData()
			});
		}
		else if (this.EntityPreviewTaked != null)
		{
			base.photonView.RPC("MoveEntityNetwork", PhotonTargets.All, new object[]
			{
				this.EntityPreviewTaked.photonView.viewID,
				this.EntityPreview.transform.position,
				this.EntityPreview.transform.rotation
			});
		}
		UnityEngine.Object.Destroy(this.EntityPreview.gameObject);
		this.EntityPreview = null;
		this.EntityPreviewIsTaked = false;
		this.EntityPreviewTaked = null;
	}

	public void AddPreview(EntityType ind)
	{
		foreach (EntityBase entityBase in EntityBase.Entities)
		{
			if (entityBase.GetComponent<PhotonView>().viewID >= 950)
			{
				Chat.SendWarning(Localize.GetText("key4001", null), false);
				return;
			}
		}
		Vector3 position = this.MainPlayer.transform.position;
		Quaternion rotation = this.EnityParametrs[(int)ind].gameObject.transform.rotation;
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.EnityParametrs[(int)ind].gameObject, position, rotation);
		this.EntityPreview = gameObject.GetComponent<EntityBase>();
		this.EntityPreview.InitializePreview(ind, null);
		this.EntityPreviewIsTaked = false;
		this.EntityPreviewTaked = null;
	}

	public void TakeEnity(GameObject entityGO)
	{
		if (!App.Instance.Settings.isWatch)
		{
			if (Level.Instance.CanBuild)
			{
				EntityBase component = entityGO.GetComponent<EntityBase>();
				Vector3 position = component.transform.position;
				Quaternion rotation = component.transform.rotation;
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.EnityParametrs[(int)component.Type].gameObject, position, rotation);
				this.EntityPreview = gameObject.GetComponent<EntityBase>();
				this.EntityPreview.InitializePreview(component.Type, component.GetData());
				MaterialExt.SetTransparent(this.EntityPreview.gameObject, 0.5f);
				this.EntityPreviewIsTaked = true;
				this.EntityPreviewTaked = component;
			}
			else
			{
				Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP", null), false);
			}
		}
		else
		{
			Chat.SendWarning(Localize.GetText("CANT_EDIT_MAP_IN_WATCH", null), false);
		}
	}

	private void DisplayDiggings()
	{
		if (this._World.Diggings.Count == 0)
		{
			return;
		}
		Vector3 position = this._World.Diggings.Dequeue();
		UnityEngine.Object.Instantiate(this.Sparks, position, Quaternion.identity);
	}

	public void OnMainPlayerDead()
	{
		base.StartCoroutine(this.MainPlayerDeadProcess());
	}

	private IEnumerator MainPlayerDeadProcess()
	{
		this.MainPlayerDead = true;
		CameraController cameraController = this.MainPlayer.GetComponent<CameraController>();
		cameraController.SetThirdPerson(true, 45f, false);
		cameraController.DisableInput = true;
		this.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		float waitDuration = 3f;
		if (this.MainPlayer.GetComponent<Animation>() != null && this.MainPlayer.GetComponent<Animation>()["death"] != null)
		{
			this.MainPlayer.GetComponent<Animation>().CrossFade("death");
			this.MainPlayer.GetComponent<NetworkSyncAnimation>().SyncAnimation(NetworkSyncAnimation.AnimState.death);
			waitDuration += this.MainPlayer.GetComponent<Animation>()["death"].length;
		}
		yield return new WaitForSeconds(waitDuration);
		if (!this.MainPlayerDead)
		{
			yield break;
		}
		if (Info.Instance.GameMode == "ARCADE" || Info.Instance.GameMode == "HUNGER_GAMES")
		{
			TeamBattle.Instance.MainPlayerDeadProcess();
			base.StartCoroutine(bs.AddUpdate(delegate
			{
				if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0) && Cursor.lockState == CursorLockMode.Locked && this.MainPlayerDead && this.MainPlayer.GetComponent<PlayerNetwork>().PlayerTeam != 0)
				{
					TeamBattle.Instance.StartPlay(0);
					MainMenu.Instance.HideMenu();
				}
				return this.MainPlayerDead;
			}));
		}
		else
		{
			KGUI.SetNodes("hud.txt_respawn_tip", true, false);
			base.StartCoroutine(bs.AddUpdate(delegate
			{
				if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0) && Cursor.lockState == CursorLockMode.Locked && this.MainPlayerDead)
				{
					if (GameType.IsHideSeek)
					{
						TeamBattle.Instance.StartPlay(2);
						HG_WorkController.CurStartSpawn = TeamBattle.Instance.GetSpawnPoint(2);
						this.Respawn();
					}
					else
					{
						this.Respawn();
					}
				}
				return this.MainPlayerDead;
			}));
		}
		yield break;
	}

	public void Respawn()
	{
		Protect.Instance.CheckDLL(UnityEngine.Random.Range(0, 14));
		KGUI.SetNodes("hud.txt_respawn_tip", false, false);
		this.MainPlayerDead = false;
		this._FaceInWaterTime = 0f;
		if (this.MainPlayerNode != null)
		{
			this.MainPlayerNode.Life = this.MainPlayerNode.MaxLife;
			if (!GameType.IsHungerGamesMode)
			{
				this.MainPlayerNode.Shield = 0f;
			}
			if (!this.MainPlayer.GetComponent<CharacterController>().enabled)
			{
				Trolley.KickPlayer(this.MainPlayer);
				Boat.KickPlayer(this.MainPlayer);
			}
			this.MainPlayer.GetComponent<PlayerInput>().SetMovement(true);
			CameraController component = this.MainPlayer.GetComponent<CameraController>();
			component.DisableInput = false;
			component.SetSavedCamera();
			this.TeleportMainPlayerToSpawnPoint();
			foreach (Gun gun in component.GetComponent<PlayerNetwork>().Guns)
			{
				gun.Reset();
			}
			component.Reset();
			this.EnableMouseLook();
			Cursor.lockState = CursorLockMode.Locked;
		}
		this.MainPlayer.GetComponent<PlayerNetwork>().Reset(false);
		UnityEngine.Debug.Log("Respawn");
		base.photonView.RPC("RespawnRPC", PhotonTargets.Others, new object[]
		{
			PhotonNetwork.player,
			this.MainPlayer.transform.position
		});
		this.MainPlayer.GetComponent<PlayerNetwork>().photonView.RPC("SetPos", PhotonTargets.All, new object[]
		{
			this.MainPlayer.transform.position
		});
	}

	[PunRPC]
	private void RespawnRPC(PhotonPlayer player, Vector3 spawnPoint)
	{
		PlayerNode playerNode = this.FindPlayer(player);
		if (playerNode != null && playerNode.Avatar != null)
		{
			playerNode.Avatar.transform.position = spawnPoint;
			playerNode.Avatar.GetComponent<PlayerNetwork>().Reset(false);
		}
	}

	public void PunchPlayer(PhotonPlayer player, Vector3 dir, float damage = 0.5f, int gunId = -1, GameObject awt = null, bool isJamp = false)
	{
		if (GameType.IsDeathmatchGamesMode && this.FindPlayer(player).Avatar != null && !this.FindPlayer(player).Avatar.GetComponent<PlayerNetwork>().IsCanDMG)
		{
			return;
		}
		if (GameType.IsArcadeMode && gunId != 770077)
		{
			return;
		}
		if (GameType.IsHideSeek && HG_WorkController.hgstatus == GameStatus.GS_WAIT)
		{
			return;
		}
		bool flag = true;
		bool flag2 = true;
		base.photonView.RPC("HitRedness", PhotonTargets.All, new object[]
		{
			player
		});
		PlayerNode playerNode = this.FindPlayer(player);
		if (playerNode != null && playerNode.Avatar != null)
		{
			int num = 0;
			if (GameType.IsHungerGamesMode)
			{
				if (HG_WorkController._player.CurentSelectedItem == null)
				{
					num = 0;
				}
				else
				{
					num = HG_WorkController._player.CurentSelectedItem.Id;
				}
			}
			playerNode.Avatar.GetComponent<PhotonView>().RPC("HitBy", PhotonTargets.All, new object[]
			{
				PhotonNetwork.player,
				(!GameType.IsHungerGamesMode) ? gunId : num
			});
			if (!GameType.IsHungerGamesMode)
			{
				base.photonView.RPC("ReceiveDamage", player, new object[]
				{
					dir,
					damage,
					flag,
					flag2,
					playerNode.ViewerID
				});
			}
			else if (HG_WorkController._player.CurentSelectedItem != null && HG_WorkController._player.CurentSelectedItem is IS_mdl_Sword)
			{
				PlayerNetwork component = this.MainPlayer.GetComponent<PlayerNetwork>();
				IS_mdl_Sword is_mdl_Sword = component.CurentSelectedItem as IS_mdl_Sword;
				int attackPwr = is_mdl_Sword.AttackPwr;
				int arrmorCount = awt.GetComponent<PlayerNetwork>().SkinControl.GetArrmorCount();
				int num2 = attackPwr - arrmorCount;
				if (num2 < 0)
				{
					num2 = 0;
				}
				base.photonView.RPC("ReceiveDamage", player, new object[]
				{
					dir,
					(float)num2 / 100f,
					flag,
					flag2,
					playerNode.ViewerID
				});
			}
			else if (HG_WorkController._player.CurentSelectedItem == null && HG_WorkController.IsStartPlay)
			{
				PlayerNetwork component2 = this.MainPlayer.GetComponent<PlayerNetwork>();
				int num3 = 8;
				int num4 = (!(awt != null)) ? 0 : awt.GetComponent<PlayerNetwork>().SkinControl.GetArrmorCount();
				int num5 = num3 - num4;
				if (num5 < 0)
				{
					num5 = 0;
				}
				base.photonView.RPC("ReceiveDamage", player, new object[]
				{
					dir,
					(float)num5 / 100f,
					flag,
					flag2,
					playerNode.ViewerID
				});
			}
			else if (isJamp)
			{
				base.photonView.RPC("ReceiveDamage", player, new object[]
				{
					dir,
					damage,
					flag,
					flag2,
					playerNode.ViewerID
				});
			}
		}
	}

	public void ArrowPunchPlayer(PhotonPlayer player, Vector3 dir, int damage, int gunId = -1, GameObject awt = null, string kick_name = "")
	{
		bool flag = true;
		bool flag2 = true;
		base.photonView.RPC("HitRedness", PhotonTargets.All, new object[]
		{
			player
		});
		PlayerNode playerNode = this.FindPlayer(player);
		if (playerNode != null && playerNode.Avatar != null)
		{
			PlayerNode playerNode2 = this.FindPlayerByNameX(kick_name);
			if (GameType.IsHungerGamesMode)
			{
				playerNode.Avatar.GetComponent<PhotonView>().RPC("HitBy", PhotonTargets.All, new object[]
				{
					playerNode2.NetPlayer,
					(HG_WorkController._player.CurentSelectedItem == null) ? 0 : HG_WorkController._player.CurentSelectedItem.Id
				});
				PlayerNetwork component = this.MainPlayer.GetComponent<PlayerNetwork>();
				int arrmorCount = awt.GetComponent<PlayerNetwork>().SkinControl.GetArrmorCount();
				int num = damage - arrmorCount;
				if (num < 0)
				{
					num = 0;
				}
				base.photonView.RPC("ReceiveDamage", player, new object[]
				{
					dir,
					(float)num / 100f,
					flag,
					flag2,
					playerNode.ViewerID
				});
			}
		}
	}

	[PunRPC]
	public void Explosion(Vector3 pos)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(bs._Igor.explosion, pos, Quaternion.identity);
		AudioSource component = gameObject.GetComponent<AudioSource>();
		component.volume = ProfileINI.sound_volume * ProfileINI.sound_scale;
		UnityEngine.Object.Destroy(gameObject, 3f);
	}

	[PunRPC]
	private void ReceiveDamage(Vector3 dir, float damage, bool jolt, bool spurn, string viewerID)
	{
		if (this.MainPlayerNode.Life <= 0f)
		{
			return;
		}
		this.MainPlayerNode.DoDamage(damage, false);
		if (jolt)
		{
			CameraController component = this.MainPlayer.GetComponent<CameraController>();
			if (component != null)
			{
				component.JoltScreen();
			}
		}
		if (spurn)
		{
			base.StartCoroutine(this.PunchMainPlayerProcess(dir * 3f, 0.2f));
		}
	}

	[PunRPC]
	private void DamageMe(float damage, string toname)
	{
	}

	private IEnumerator PunchMainPlayerProcess(Vector3 force, float duration)
	{
		float endTime = Time.time + duration;
		float tickTime = Time.time;
		while (Time.time < endTime)
		{
			yield return new WaitForSeconds(0.01f);
			if (this.MainPlayer == null)
			{
				yield break;
			}
			CharacterController characterController = this.MainPlayer.GetComponent<CharacterController>();
			if (characterController == null || !characterController.enabled)
			{
				yield break;
			}
			characterController.Move(force * (Time.time - tickTime));
			tickTime = Time.time;
		}
		yield break;
	}

	[PunRPC]
	private void HitRedness(PhotonPlayer player)
	{
		if (GameType.IsObserving())
		{
			return;
		}
		PlayerNode playerNode = this.FindPlayer(player);
		if (playerNode != null && playerNode.Avatar != null)
		{
			if (!GameType.IsArcadeMode)
			{
				playerNode.Avatar.GetComponent<bs>().PlayOneShot(bs._Igor.damageSound[UnityEngine.Random.Range(0, bs._Igor.damageSound.Length)]);
			}
			SkinManager component = playerNode.Avatar.GetComponent<SkinManager>();
			if (component != null)
			{
				component.Redness();
			}
		}
	}

	[PunRPC]
	private void SetPurchase(int purchase, int value, string time)
	{
	}

	private void StartSpeedHackDetection()
	{
		this._SHErrorCount = 0;
		this._SHOldDT = DateTime.Now;
		this._SHOldTick = (long)Environment.TickCount;
		base.InvokeRepeating("HeartLifeCrt", 2f, 2f);
	}

	private void HeartLifeCrt()
	{
		TimeSpan timeSpan = DateTime.Now - this._SHOldDT;
		this._SHOldDT = DateTime.Now;
		long num = (long)Environment.TickCount - this._SHOldTick;
		this._SHOldTick = (long)Environment.TickCount;
		if (timeSpan.TotalMilliseconds * this._SHErrorRate < (double)num)
		{
			this._SHErrorCount++;
		}
		if (this._SHErrorCount > 5)
		{
			UnityEngine.Debug.Log("SH detected");
			this.ExitGame("SH detected");
		}
	}

	public void KillPlayer()
	{
		if (this.MainPlayerNode.Life >= 0f)
		{
			this.PunchPlayer(PhotonNetwork.player, Vector3.zero, 100f, -1, null, false);
		}
	}

	public void PlaySoundForAll(SoundManager.Sound sound)
	{
		base.photonView.RPC("PlaySound", PhotonTargets.All, new object[]
		{
			(int)sound
		});
	}

	[PunRPC]
	private void PlaySound(int sound)
	{
		SoundManager.Instance.Play((SoundManager.Sound)sound, null);
	}

	public IEnumerator PhotonPurchaseBuy(string shopRequest, Action<string> callback)
	{
		bool waitAnswer = true;
		PhotonNetwork.EventCallback ec = delegate(byte eventCode, object content, int senderId)
		{
			if (eventCode == 1)
			{
				callback((string)content);
				waitAnswer = false;
			}
		};
		PhotonNetwork.OnEventCall = (PhotonNetwork.EventCallback)Delegate.Combine(PhotonNetwork.OnEventCall, ec);
		PhotonNetwork.RaiseEvent(1, shopRequest, true, RaiseEventOptions.Default);
		while (waitAnswer)
		{
			yield return new WaitForEndOfFrame();
		}
		PhotonNetwork.OnEventCall = (PhotonNetwork.EventCallback)Delegate.Remove(PhotonNetwork.OnEventCall, ec);
		yield break;
	}

	private IEnumerator CheckTimePurchasesProcess()
	{
		for (;;)
		{
			App.Instance.CheckTimePurchases();
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public void StartCheckCheating(string playerName)
	{
		PlayerNode playerNode = this.FindPlayerByNameX(playerName);
		if (playerNode == null)
		{
			return;
		}
		if (playerNode.Name == this.MainPlayerNode.Name)
		{
			Chat.SendWarning(Localize.GetText("CHEAT_VOTE_ERROR_MYSELF", null), false);
			return;
		}
		Chat.SendWarning(Localize.GetText("CHEAT_STARTER_NAME", null) + " " + MainMenu.FixCollorName(this.MainPlayerNode.Name), true);
		base.photonView.RPC("CheckCheating", PhotonTargets.AllViaServer, new object[]
		{
			playerNode.ViewerID
		});
	}

	[PunRPC]
	private void CheckCheating(string cheaterID, PhotonMessageInfo info)
	{
		PlayerNode playerNode = this.FindPlayerByViewerID(cheaterID);
		this._CheckCheating = true;
		this._CheatingVoted = false;
		this._CheatingVotedYes = false;
		this._CheatingVotingTime = DateTime.Now;
		this._CheatingPlayerName = ((playerNode == null) ? cheaterID : playerNode.Name);
		if (info.sender.isLocal)
		{
			this._CheatingVoted = true;
			this._CheatingVotedYes = true;
		}
		if (VKAPI.INSTANCE._viewerId == cheaterID)
		{
			this._CheatingVoted = true;
			this._CheatingVotedYes = false;
		}
	}

	[PunRPC]
	private void ChestInst(int chest_id, int chest_type)
	{
	}

	[PunRPC]
	private void GetFromChest(int chest_id, string chest_item)
	{
	}

	[PunRPC]
	private void GetOneItemFromChest(int chest_id, int chest_item, int cell_id)
	{
	}

	[PunRPC]
	private void AddItemToChest(int chest_id, int chest_item)
	{
	}

	[PunRPC]
	private void Medication(string player_name, int medication_id, int medication_point)
	{
		float num = (float)medication_point / 100f;
		this.FindPlayerByNameX(player_name).Life += num;
		if (this.FindPlayerByNameX(player_name).Life > 1f)
		{
			this.FindPlayerByNameX(player_name).Life = 1f;
		}
	}

	[PunRPC]
	private void BagInst(int chest_id, int chest_type, string item_in_id, string pos)
	{
	}

	[PunRPC]
	private void BattleStartPoint(string position)
	{
	}

	[PunRPC]
	private void BattleStartPointTeam(string position, int team)
	{
	}

	[PunRPC]
	private void ReportCMSG(string user_id, int ct)
	{
	}

	[PunRPC]
	private void SetSelectedPack(int pack)
	{
	}

	[PunRPC]
	private void PlayerLifeOfShieldChanged(string pName, float pLife, float pShield)
	{
		if (pName != this.MainPlayerNode.Name)
		{
			PlayerNode playerNode = this.FindPlayerByNameX(pName);
			playerNode.Life = pLife;
			playerNode.Shield = pShield;
			if (WorldGameObjectX.isPlayerLifeOfShieldChangedLocal != null)
			{
				WorldGameObjectX.isPlayerLifeOfShieldChangedLocal(pName);
			}
		}
	}

	[PunRPC]
	private void PlayerZombieGameBuildBlock(string pName)
	{
		if (pName == this.MainPlayerNode.Name)
		{
			this.MainPlayerNode.ZombieBlock--;
		}
	}

	[PunRPC]
	private void AddNewHideSeekPlayerInTeam(int hsteam)
	{
	}

	private void BufferUp()
	{
		if (Time.time > this.lastSend + this.sendSpeed)
		{
			this.lastSend = Time.time;
			if (this.MainPlayerNode != null)
			{
				base.photonView.RPC("CheckRealName", PhotonTargets.All, new object[]
				{
					this.MainPlayerNode.Name
				});
			}
		}
	}

	[PunRPC]
	private void CheckRealName(string name)
	{
	}

	[PunRPC]
	private void MakeWhistle(Vector3 v3)
	{
		try
		{
			SoundManager.Instance.PlayAtPoint(SoundManager.Sound.Treasure, v3);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log(ex.StackTrace);
		}
	}

	public static void SetCustomPropertiesToHG()
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("game_status", GameStatus.GS_PREPEA);
		PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
	}

	public List<StorePurchase> GetPurchasePack()
	{
		List<StorePurchase> list = new List<StorePurchase>();
		List<StorePurchase> list2 = new List<StorePurchase>
		{
			StorePurchase.HG_ARCHER_BUILD_5,
			StorePurchase.HG_ARCHER_BUILD_4,
			StorePurchase.HG_ARCHER_BUILD_3,
			StorePurchase.HG_ARCHER_BUILD_2,
			StorePurchase.HG_ARCHER_BUILD_1
		};
		List<StorePurchase> list3 = new List<StorePurchase>
		{
			StorePurchase.HG_KNIGHT_BUILD_5,
			StorePurchase.HG_KNIGHT_BUILD_4,
			StorePurchase.HG_KNIGHT_BUILD_3,
			StorePurchase.HG_KNIGHT_BUILD_2,
			StorePurchase.HG_KNIGHT_BUILD_1
		};
		foreach (StorePurchase storePurchase in list2)
		{
			if (storePurchase != StorePurchase.NONE && ProfileINI.GetPurchaseValue(storePurchase) != 0)
			{
				list.Add(storePurchase);
				break;
			}
		}
		foreach (StorePurchase storePurchase2 in list3)
		{
			if (storePurchase2 != StorePurchase.NONE && ProfileINI.GetPurchaseValue(storePurchase2) != 0)
			{
				list.Add(storePurchase2);
				break;
			}
		}
		return list;
	}

	public const int StartCountZombieBlock = 60;

	public const float ContinuousDamageHitTime = 1f;

	private const int WaterBooblesMaxCount = 10;

	private const float UnderwaterMaxTime = 20f;

	internal BlockKind CurrentKind;

	internal CommonBlockKind CurrentCommonKind;

	public GameObject Audio;

	public static Action<string> isPlayerLifeOfShieldChangedLocal;

	public static WorldGameObjectX Instance = null;

	public BlockParametrs[] BlockParametrs;

	public EnityParametrs[] EnityParametrs;

	public BlockLifes[] BlockLifes;

	public GameObject[] listSelectel;

	public GameObject[] listDamage;

	private Dictionary<string, GameObject> _damage = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> _selects = new Dictionary<string, GameObject>();

	private GameObject _Selectel;

	internal GameObject CubeDamage;

	public GameObject Sparks;

	public GameObject SparksCubes1;

	public GameObject SparksCubes2;

	private WorldData _WorldData;

	private World _World;

	internal bool IsWorldGenerated;

	private List<IDecoration> _WorldDecorations = new List<IDecoration>();

	private Material _DefaultSkyBox;

	public Transform Chunk_Prefab;

	public Transform WaterPrefab;

	public Transform GlassPrefab;

	public GameObject Player_Prefab;

	public GameObject HGPlayer_Prefab;

	internal GameObject MainPlayer;

	internal Camera MainPlayerEyes;

	internal PlayerNode MainPlayerNode;

	internal bool MainPlayerDead;

	private GameObject _WorldCamera;

	public Texture2D[] World_Textures;

	internal EntityBase EntityPreview;

	internal bool EntityPreviewIsTaked;

	internal EntityBase EntityPreviewTaked;

	internal EntityBase EntityUnderCursor;

	internal LevelDelta LevelDeltaLoading = new LevelDelta();

	internal Texture2D WorldTextureAtlas;

	private ChunkGameObject[,,,] _ChunkObjects;

	private Transform _ChunksParent;

	internal List<PlayerNode> PlayerList = new List<PlayerNode>();

	internal int KubokCountInGame;

	internal int KubokFindInGame;

	internal bool FindAllKubok;

	public GameObject HitEffect;

	public GameObject ZombieEffect;

	public StorePurchase selected_hg_pack = StorePurchase.NONE;

	public static Dictionary<int, HG_Spawn> LevelChest = new Dictionary<int, HG_Spawn>();

	public bool selfLoad;

	public List<GameObject> fireworkList;

	private int _CurZipedLevelChunk;

	private int _ZipedLevelChunkCount;

	private float _ZipedChunkReceiveTime;

	internal bool MakeContinuousDamage;

	private bool _ContinuousDamageEnabled;

	private float _ContinuousDamageLastHitTime;

	private bool _OnIce;

	private bool _InWater;

	private bool _FaceInWater;

	private float _FaceInWaterTime;

	private float _WaterBooblesCount;

	[NonSerialized]
	public float last_ladder_delta;

	[NonSerialized]
	public bool player_on_ladder;

	internal BlockType CurrentBlock = BlockType.WoodPlank;

	internal EntityType CurrentBlockEntity;

	private static int _CurBlockSibling = 0;

	public bool bSkipSoundRpc;

	private SecuredValue<double> _SHErrorRate = 1.15;

	private SecuredValue<DateTime> _SHOldDT;

	private SecuredValue<long> _SHOldTick;

	private SecuredValue<int> _SHErrorCount;

	private bool _CheckCheating;

	private bool _CheatingVoted;

	private bool _CheatingVotedYes;

	private DateTime _CheatingVotingTime;

	private string _CheatingPlayerName;

	private float lastSend;

	private float sendSpeed = 5f;
}
