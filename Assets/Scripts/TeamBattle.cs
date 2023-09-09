using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamBattle : GameType
{
	public void FastEndOn(int w)
	{
		this.EndGame(w);
	}

	public int RedTeamScore
	{
		get
		{
			return this._Teams[1].Score;
		}
	}

	public int BlueTeamScore
	{
		get
		{
			return this._Teams[2].Score;
		}
	}

	protected void Awake()
	{
		TeamBattle.Instance = this;
		for (int i = 0; i < this._Teams.Length; i++)
		{
			this._Teams[i] = new TeamBattle.Team();
		}
	}

	protected virtual void Update()
	{
		if (!(this is HungerGames) || !(this is Arcade) || !(this is HideSeek))
		{
			this._GameTime -= Time.deltaTime;
		}
		if (PhotonNetwork.isMasterClient && Time.time - this._TimeLastSynchronization >= 5f)
		{
			base.photonView.RPC("SetGameTime", PhotonTargets.Others, new object[]
			{
				this._GameTime
			});
		}
		if (!GameType.IsHungerGamesMode && !GameType.IsArcadeMode && !GameType.IsHideSeek)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)Math.Max(0f, this._GameTime));
			KGUI.SetControlText("hud.battle.txt_time", timeSpan.Minutes.ToString("D2") + ":" + timeSpan.Seconds.ToString("D2"));
		}
		else if (GameType.IsHideSeek)
		{
			if (HG_WorkController.hgstatus != GameStatus.GS_START)
			{
				KGUI.SetControlText("hud.battle.txt_time", string.Empty);
			}
		}
		else
		{
			KGUI.SetControlText("hud.battle.txt_time", string.Empty);
			this._GameTime = 200f;
		}
		if (PhotonNetwork.isMasterClient && this._GameTime < 0f && !(this is ZombieVirus) && !(this is HungerGames))
		{
			if (!this._GameEnded)
			{
				int num = (this.BlueTeamScore != this.RedTeamScore) ? ((this.BlueTeamScore <= this.RedTeamScore) ? 1 : 2) : -1;
				base.photonView.RPC("EndGame", PhotonTargets.All, new object[]
				{
					num
				});
			}
			else
			{
				base.photonView.RPC("RestartGame", PhotonTargets.All, new object[0]);
			}
		}
	}

	private IEnumerator Start()
	{
		if (!App.Instance.Settings.isServer)
		{
			yield break;
		}
		while (!WorldGameObjectX.Instance.IsWorldGenerated)
		{
			yield return 0;
		}
		this.StartPlay(this.GetDefaultTeam());
		this.ServerStartGame();
		yield break;
	}

	public virtual void SpawnToGame()
	{
	}

	protected virtual void ServerStartGame()
	{
		this._GameTime = TeamBattle.RoundDuration;
		this._GameEnded = false;
	}

	protected virtual void OnPhotonPlayerConnected(PhotonPlayer connectedPlayer)
	{
		if (PhotonNetwork.isMasterClient)
		{
			for (int i = 0; i < this._Teams.Length; i++)
			{
				base.photonView.RPC("SetTeamData", connectedPlayer, new object[]
				{
					i,
					this._Teams[i].Score
				});
				foreach (TeamBattle.Player player in this._Teams[i].Players)
				{
					base.photonView.RPC("AddToTeam", connectedPlayer, new object[]
					{
						i,
						player.Name,
						player.Data
					});
				}
			}
			base.photonView.RPC("StartPlay", connectedPlayer, new object[]
			{
				this.GetDefaultTeam()
			});
			base.photonView.RPC("SetGameTime", connectedPlayer, new object[]
			{
				this._GameTime
			});
		}
		KGUI.SetNodes("hud.txt_battle_more_players_tip", PhotonNetwork.playerList.Length < 4 && GameType.IsNeedCheckTips(), false);
	}

	protected virtual void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
	}

	protected virtual void OnPhotonPlayerDisconnected(PhotonPlayer disconnectedPlayer)
	{
		for (int i = 0; i < 3; i++)
		{
			if (this._Teams[i][disconnectedPlayer.name] != null)
			{
				this._Teams[i][disconnectedPlayer.name] = null;
			}
		}
		KGUI.SetNodes("hud.txt_battle_more_players_tip", PhotonNetwork.playerList.Length < 4 && GameType.IsNeedCheckTips(), false);
	}

	[PunRPC]
	public virtual void SetGameTime(float time)
	{
		this._GameTime = time;
	}

	[PunRPC]
	public virtual void StartPlay(int teamIndex)
	{
		if (!WorldGameObjectX.Instance.IsWorldGenerated)
		{
			this._TryStartTeam = teamIndex;
			base.StopCoroutine("TryStartPlayProcess");
			base.StartCoroutine("TryStartPlayProcess");
			return;
		}
		object[] playerData = null;
		for (int i = 0; i < 3; i++)
		{
			if (this._Teams[i][PhotonNetwork.playerName] != null)
			{
				playerData = this._Teams[i][PhotonNetwork.playerName].Data;
				this.RemoveFromTeam(i, PhotonNetwork.playerName, null);
				break;
			}
		}
		this.AddToTeam(teamIndex, PhotonNetwork.playerName, playerData, null);
		WorldGameObjectX.Instance.TeleportMainPlayerToSpawnPoint();
		for (int j = 0; j < 3; j++)
		{
			foreach (TeamBattle.Player player in this._Teams[j].Players)
			{
				if (player.Name != PhotonNetwork.playerName)
				{
					this.SetupPlayer(j, player.Name);
				}
			}
		}
		base.photonView.RPC("SetPlayerTeam", PhotonTargets.AllViaServer, new object[]
		{
			teamIndex
		});
	}

	protected IEnumerator TryStartPlayProcess()
	{
		while (!WorldGameObjectX.Instance.IsWorldGenerated)
		{
			yield return 0;
		}
		yield return 0;
		this.StartPlay(this._TryStartTeam);
		yield break;
	}

	protected void SetupPlayer(int teamIndex, string playerName)
	{
		GameObject gameObject = null;
		PlayerNode playerNode = null;
		GameObject mainPlayer = WorldGameObjectX.Instance.MainPlayer;
		if (playerName == PhotonNetwork.playerName)
		{
			gameObject = mainPlayer;
		}
		else
		{
			playerNode = WorldGameObjectX.Instance.FindPlayerByNameX(playerName);
			if (playerNode != null)
			{
				gameObject = playerNode.Avatar;
			}
		}
		if (mainPlayer == null || gameObject == null)
		{
			return;
		}
		gameObject.GetComponent<Nickname>().SetColor(TeamBattle.GetTeamColor(teamIndex));
		gameObject.GetComponent<Nickname>().SetRaycast(true);
		bool flag = teamIndex == 0;
		if (mainPlayer == gameObject)
		{
			if (flag)
			{
				if (gameObject.GetComponent<CameraController>().IsThirdPerson)
				{
					gameObject.GetComponent<CameraController>().SetFirstPerson(true);
				}
				gameObject.GetComponent<SkinManager>().DisableAllHands();
			}
			else
			{
				gameObject.GetComponent<SkinManager>().SetHand(ProfileINI.GetActualSkin());
			}
			gameObject.GetComponent<CameraController>().DisableCameraSwitching = flag;
			MainMenu.Instance.RefreshFlying();
			if (!GameType.IsHungerGamesMode)
			{
				MainMenu.Instance.SetMenu(Menu.Hud, true, null, null);
				WorldGameObjectX.Instance.FindPlayerByNameX(playerName).Shield = 0f;
				WorldGameObjectX.Instance.FindPlayerByNameX(playerName).Life = 1f;
			}
		}
		else
		{
			bool flag2 = GameType.IsObserving();
			if (gameObject.activeInHierarchy && gameObject.GetComponent<Collider>().enabled && mainPlayer.activeInHierarchy && mainPlayer.GetComponent<Collider>().enabled)
			{
				Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), mainPlayer.GetComponent<Collider>(), flag || flag2);
			}
			if (base.GetType() == typeof(TeamBattle))
			{
				if (flag)
				{
					gameObject.GetComponent<SkinManager>().DisableAllSkins();
				}
				if (teamIndex == 1)
				{
					gameObject.GetComponent<SkinManager>().SetSkin(23);
					gameObject.GetComponent<PlayerNetwork>().RefreshNativeWeapon(23);
				}
				else if (teamIndex == 2)
				{
					gameObject.GetComponent<SkinManager>().SetSkin(10);
					gameObject.GetComponent<PlayerNetwork>().RefreshNativeWeapon(10);
				}
			}
			else if (flag)
			{
				gameObject.GetComponent<SkinManager>().DisableAllSkins();
			}
			else
			{
				gameObject.GetComponent<SkinManager>().SetSkin(playerNode.Skin);
			}
			if (flag)
			{
				gameObject.GetComponent<Nickname>().Hide();
			}
			else if (this.GetPlayerTeam(null) != teamIndex)
			{
				gameObject.GetComponent<Nickname>().Show();
			}
			else if (this is ZombieVirus || this is Deathmatch)
			{
				gameObject.GetComponent<Nickname>().Show();
			}
			else
			{
				gameObject.GetComponent<Nickname>().ShowMyTeamPlayer();
			}
		}
	}

	[PunRPC]
	public virtual void AddToTeam(int teamIndex, string playerName, object[] playerData, PhotonMessageInfo info = null)
	{
		this._Teams[teamIndex][playerName] = new TeamBattle.Player(playerName, playerData);
		if (info == null)
		{
			base.photonView.RPC("AddToTeam", PhotonTargets.Others, new object[]
			{
				teamIndex,
				playerName,
				playerData
			});
		}
		this.SetupPlayer(teamIndex, playerName);
		this.SortPlayers(teamIndex);
		this.RefreshTabMenu(false);
	}

	[PunRPC]
	protected virtual void RemoveFromTeam(int teamIndex, string playerName, PhotonMessageInfo info = null)
	{
		this._Teams[teamIndex][playerName] = null;
		if (info == null)
		{
			base.photonView.RPC("RemoveFromTeam", PhotonTargets.Others, new object[]
			{
				teamIndex,
				playerName
			});
		}
		this.SortPlayers(teamIndex);
		this.RefreshTabMenu(false);
	}

	[PunRPC]
	public virtual void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		if (this._Teams[teamIndex][playerName] == null)
		{
			return;
		}
		this._Teams[teamIndex][playerName].Data = playerData;
		this.SortPlayers(teamIndex);
		this.RefreshTabMenu(false);
	}

	public virtual int GetDefaultTeam()
	{
		return 0;
	}

	public virtual int GetSkin()
	{
		if (base.GetType() != typeof(TeamBattle))
		{
			return ProfileINI.skin;
		}
		if (this.GetPlayerTeam(WorldGameObjectX.Instance.MainPlayerNode.Name) == 1)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>().RefreshNativeWeapon(23);
			return 23;
		}
		if (this.GetPlayerTeam(WorldGameObjectX.Instance.MainPlayerNode.Name) == 2)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>().RefreshNativeWeapon(10);
			return 10;
		}
		return ProfileINI.skin;
	}

	public int GetPlayerTeam(string playerName = null)
	{
		if (playerName == null)
		{
			playerName = PhotonNetwork.playerName;
		}
		for (int i = 0; i < 3; i++)
		{
			if (this._Teams[i][playerName] != null)
			{
				return i;
			}
		}
		return -1;
	}

	public virtual bool IsTeammate(string playerName)
	{
		if (this._GameEnded)
		{
			return true;
		}
		for (int i = 1; i < 3; i++)
		{
			if (this._Teams[i][PhotonNetwork.playerName] != null)
			{
				return this._Teams[i][playerName] != null;
			}
		}
		return false;
	}

	public virtual bool IsOpposite(string playerName)
	{
		if (this._GameEnded)
		{
			return false;
		}
		for (int i = 1; i < 3; i++)
		{
			if (this._Teams[i][playerName] != null)
			{
				return this._Teams[i][PhotonNetwork.playerName] == null;
			}
		}
		return false;
	}

	public bool IsObserving(string playerName)
	{
		return this._Teams[0][playerName] != null;
	}

	public virtual void OnDead(string killer, string victim, int score)
	{
		UnityEngine.Debug.Log("OnPlayerDead TB");
		base.photonView.RPC("OnPlayerDead", PhotonTargets.MasterClient, new object[]
		{
			killer,
			victim,
			score
		});
	}

	[PunRPC]
	protected virtual void OnPlayerDead(string killer, string victim, int score)
	{
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"OnPlayerDead ----- ",
			killer,
			" ",
			victim,
			" ",
			score,
			" ",
			this._GameEnded
		}));
		if (this._GameEnded)
		{
			return;
		}
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			" ----- ",
			killer,
			" ",
			victim,
			" ",
			score
		}));
		if (killer != victim)
		{
			int playerTeam = this.GetPlayerTeam(killer);
			if (playerTeam != 0)
			{
				this.ModifyPlayerData(playerTeam, killer, score, 1, 0);
				this.AddTeamScore(playerTeam, score);
			}
		}
		int playerTeam2 = this.GetPlayerTeam(victim);
		if (playerTeam2 != 0)
		{
			this.ModifyPlayerData(playerTeam2, victim, 0, 0, 1);
		}
	}

	[PunRPC]
	protected virtual void EndGame(int winner)
	{
		WorldGameObjectX.Instance.MainPlayer.GetComponent<CameraController>().SniperScopeOff();
		this._GameEnded = true;
		this._WinnerTeam = winner;
		this._GameTime = 20f;
		int num = -1;
		int num2 = 0;
		this._TeamsResult = new TeamBattle.Team[this._Teams.Length];
		for (int i = 0; i < this._Teams.Length; i++)
		{
			this._TeamsResult[i] = this._Teams[i].GetCopy();
			this._Teams[i].Score = 0;
			foreach (TeamBattle.Player player in this._Teams[i].Players)
			{
				if (player.Name == PhotonNetwork.playerName)
				{
					num = i;
					num2 = player.Score;
				}
				player.Reset();
			}
		}
		this.StartPlay(0);
		if (winner == num)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BattleWin, null);
			MainMenu.Instance.SwitchMenu(Menu.BattleResult, true, num2);
		}
		else if (num == 1 || num == 2)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BattleLose, null);
			MainMenu.Instance.SwitchMenu(Menu.BattleResult, false, num2);
		}
		else
		{
			MainMenu.Instance.SwitchMenu(Menu.TeamBattle, null, null);
		}
	}

	[PunRPC]
	protected virtual void RestartGame()
	{
		this._GameEnded = false;
		this._GameTime = TeamBattle.RoundDuration;
		this._TeamsResult = null;
		WorldGameObjectX.Instance.RestartLevel();
		this.RefreshTabMenu(true);
	}

	public void AddTeamScore(int teamIndex, int score)
	{
		if (PhotonNetwork.playerList.Length < 4)
		{
			return;
		}
		base.photonView.RPC("SetTeamData", PhotonTargets.All, new object[]
		{
			teamIndex,
			this._Teams[teamIndex].Score + score
		});
	}

	public virtual void ModifyPlayerData(int teamIndex, string playerName, int score, int kills, int deads)
	{
		if (PhotonNetwork.playerList.Length < 4)
		{
			return;
		}
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			teamIndex,
			" ",
			playerName,
			" ",
			score,
			" ",
			kills,
			" ",
			deads
		}));
		TeamBattle.Player player = this._Teams[teamIndex][playerName];
		if (player != null)
		{
			if (score != 0)
			{
				TeamBattle.Player player2 = player;
				player2.Score += score;
			}
			if (kills != 0)
			{
				TeamBattle.Player player3 = player;
				player3.Kills += kills;
			}
			if (deads != 0)
			{
				TeamBattle.Player player4 = player;
				player4.Deads += deads;
			}
			base.photonView.RPC("UpdatePlayerData", PhotonTargets.All, new object[]
			{
				teamIndex,
				player.Name,
				player.Data
			});
		}
	}

	[PunRPC]
	protected virtual void SetTeamData(int teamIndex, int teamScore)
	{
		this._Teams[teamIndex].Score = teamScore;
		KGUI.SetControlText("hud.battle.txt_red_score", string.Empty + TeamBattle.Instance.RedTeamScore);
		KGUI.SetControlText("hud.battle.txt_blue_score", string.Empty + TeamBattle.Instance.BlueTeamScore);
	}

	public virtual Vector3 GenerateSpawnPoint()
	{
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			if (this._Teams[i][PhotonNetwork.playerName] != null)
			{
				if (i == 0)
				{
					vector = GameType.FindRandomPlace(null, 10f);
				}
				else
				{
					vector = this.GetSpawnPoint(i);
				}
				break;
			}
		}
		if (vector == Vector3.zero)
		{
			vector = this.GetSpawnPoint(0);
		}
		int num = Mathf.Clamp((int)vector.x + UnityEngine.Random.Range(0, 7) - 3, 0, WorldData.Instance.WidthInBlocks - 1);
		int num2 = Mathf.Clamp((int)vector.z + UnityEngine.Random.Range(0, 7) - 3, 0, WorldData.Instance.HeightInBlocks - 1);
		int num3 = (int)vector.y;
		for (int j = 0; j < 4; j++)
		{
			if (WorldData.Instance.GetBlockType(num, num2, num3 + j) == BlockType.Air)
			{
				vector = new Vector3((float)num + 0.5f, (float)(num3 + j) + 0.5f, (float)num2 + 0.5f);
				break;
			}
			if (j > 0 && WorldData.Instance.GetBlockType(num, num2, num3 - j) == BlockType.Air)
			{
				vector = new Vector3((float)num + 0.5f, (float)(num3 - j) + 0.5f, (float)num2 + 0.5f);
				break;
			}
		}
		return vector;
	}

	public Vector3 GetSpawnPoint(int teamIndex)
	{
		SpawnArrow[] array = UnityEngine.Object.FindObjectsOfType(typeof(SpawnArrow)) as SpawnArrow[];
		List<SpawnArrow> list = new List<SpawnArrow>();
		foreach (SpawnArrow spawnArrow in array)
		{
			if (spawnArrow.TeamIndex == teamIndex)
			{
				list.Add(spawnArrow);
			}
		}
		if (list.Count == 0)
		{
			return GameType.FindRandomPlace(null, 10f);
		}
		return list[UnityEngine.Random.Range(0, list.Count)].transform.position;
	}

	public static Color GetTeamColor(int teamIndex)
	{
		switch (teamIndex)
		{
		case 0:
			return Color.green;
		case 1:
			return Color.red;
		case 2:
			return Color.blue;
		case 3:
			return Color.white;
		default:
			return Color.gray;
		}
	}

	protected void SortPlayers(int teamIndex)
	{
		this._Teams[teamIndex].Players.Sort((TeamBattle.Player a, TeamBattle.Player b) => b.Score - a.Score);
	}

	public virtual void RefreshTabMenu(bool open = false)
	{
		if (open && MainMenu.Instance.CurMenu != Menu.TeamBattle)
		{
			MainMenu.Instance.SwitchMenu(Menu.TeamBattle, null, null);
			return;
		}
		TeamBattle.Team[] array = (this._TeamsResult == null) ? this._Teams : this._TeamsResult;
		this._TeamsTabMenu = new TeamBattle.Team[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			this._TeamsTabMenu[i] = array[i].GetCopy();
		}
		this.RefreshTeamMenu(this._TeamsTabMenu[1], "grid_red_team");
		this.RefreshTeamMenu(this._TeamsTabMenu[2], "grid_blue_team");
		int playerTeam = this.GetPlayerTeam(null);
		KGUI.SetNodes("team_battle.button_observe", false, false);
		KGUI.SetNodes("team_battle.button_red_team", playerTeam != 1 && !this._GameEnded, false);
		KGUI.SetNodes("team_battle.button_blue_team", playerTeam != 2 && !this._GameEnded, false);
	}

	private void RefreshTeamMenu(TeamBattle.Team team, string gridName)
	{
		KGUI.ResizeGrid("team_battle." + gridName, team.Players.Count, delegate(GameObject slot, int index)
		{
			TeamBattle.Player player = team.Players[index];
			slot.transform.Find("txt_nickname").GetComponent<UILabel>().text = string.Empty + player.Name;
			slot.transform.Find("txt_score").GetComponent<UILabel>().text = string.Empty + player.Score;
			KGUI.SetControlSprite(slot.transform.Find("level_icon"), "level_" + player.Level, 0f);
			MainMenu.Instance.AddVoiceHighlight(player.Node, slot.transform.Find("ibtn_voice"));
		}, null);
	}

	public void CheckPlayerCheating(int teamIndex, int tabMenuIndex)
	{
		if (ProfileINI.level >= 12)
		{
			TeamBattle.Player player = this._TeamsTabMenu[teamIndex].Players[tabMenuIndex];
			WorldGameObjectX.Instance.StartCheckCheating(player.Name);
		}
		else
		{
			MainMenu.Instance.HideMenu();
			MainMenu.Instance.ShowHint(Localize.GetText("MAP_VOTE_ERROR", null), true);
		}
	}

	public int GetPlayerLevel(int teamIndex, int tabMenuIndex)
	{
		return this._TeamsTabMenu[teamIndex].Players[tabMenuIndex].Level;
	}

	public string GetPlayerViewerID(int teamIndex, int tabMenuIndex)
	{
		return this._TeamsTabMenu[teamIndex].Players[tabMenuIndex].ViewerID;
	}

	public PlayerNode GetPlayerNode(int teamIndex, int tabMenuIndex)
	{
		return this._TeamsTabMenu[teamIndex].Players[tabMenuIndex].Node;
	}

	[PunRPC]
	public void SetPlayerTeam(int teamIndex)
	{
	}

	public virtual void AddKill(string killer)
	{
	}

	public virtual bool IsNeedShowWeaponDialog()
	{
		return false;
	}

	public virtual void SetEnableFallingMode(bool flag)
	{
	}

	public virtual void PhotonEventWork(byte eventCode, object content, int senderId)
	{
		if (eventCode == 2)
		{
			if (content is string)
			{
				UnityEngine.Debug.Log("--SEND CONTENT" + (string)content);
				if (content.ToString().Contains("|"))
				{
					string[] array = content.ToString().Split(new char[]
					{
						'|'
					});
					Chat.SendInfoF(array[1] + " " + Localize.GetText(array[0], null), false);
				}
				else
				{
					Chat.SendInfoF(Localize.GetText((string)content, null), false);
				}
			}
			else if (content is string[])
			{
				string[] array2 = (string[])content;
				string text = string.Empty;
				for (int i = 0; i < array2.Length; i++)
				{
					text += Localize.GetText(array2[i], null);
				}
				Chat.SendInfoF(text, false);
			}
		}
		else if (eventCode == 7)
		{
			string[] array3 = content.ToString().Split(new char[]
			{
				':'
			});
			if (array3.Length > 1)
			{
				MainMenu.Instance.SetKickReason(Localize.GetText("CHEAT_USE_" + array3[1], null));
			}
			else
			{
				MainMenu.Instance.SetKickReason(array3[0]);
			}
		}
	}

	public virtual void MainPlayerDeadProcess()
	{
	}

	public void TeleportPlayerBattlePoint(Vector3 pos)
	{
		if (WorldGameObjectX.Instance.MainPlayerNode != null && WorldGameObjectX.Instance.MainPlayerNode.Avatar != null)
		{
			WorldGameObjectX.Instance.MainPlayerNode.Avatar.transform.position = pos;
		}
	}

	public Vector3 GetHGSpawnPosition(Vector3 startv3point)
	{
		List<PlayerNode> list = new List<PlayerNode>();
		foreach (PlayerNode playerNode in WorldGameObjectX.Instance.PlayerList)
		{
			if (playerNode.Life > 0f && !GameType.IsObserving(playerNode.Name))
			{
				list.Add(playerNode);
			}
		}
		List<PlayerNode> list2 = (from t in list
		orderby t.Name
		select t).ToList<PlayerNode>();
		int num = -1;
		if (list2.Contains(WorldGameObjectX.Instance.MainPlayerNode))
		{
			num = list2.IndexOf(WorldGameObjectX.Instance.MainPlayerNode);
		}
		int num2 = num;
		switch (num2 + 1)
		{
		case 0:
			return Vector3.zero;
		case 1:
			return new Vector3(startv3point.x + 3f, startv3point.y, startv3point.z);
		case 2:
			return new Vector3(startv3point.x - 3f, startv3point.y, startv3point.z + 3f);
		case 3:
			return new Vector3(startv3point.x - 3f, startv3point.y, startv3point.z - 3f);
		default:
			return Vector3.zero;
		}
	}

	public void LookAtTarrget(Vector3 to)
	{
		if (WorldGameObjectX.Instance.MainPlayerNode != null && WorldGameObjectX.Instance.MainPlayerNode.Avatar != null)
		{
			Vector3 vector = to - WorldGameObjectX.Instance.MainPlayerNode.Avatar.transform.position;
			float angle = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			WorldGameObjectX.Instance.MainPlayerNode.Avatar.transform.rotation = Quaternion.AngleAxis(angle, WorldGameObjectX.Instance.MainPlayerNode.Avatar.transform.up);
		}
	}

	public const int TeamsCount = 3;

	public const int UnknownTeamIndex = -1;

	public const int ObserversTeamIndex = 0;

	public const int RedTeamIndex = 1;

	public const int BlueTeamIndex = 2;

	public const float SynchronizationPeriod = 5f;

	public const int MinPlayersToScore = 4;

	public static float RoundDuration = 600f;

	public static TeamBattle Instance;

	internal bool EnableVoiceChat = true;

	protected TeamBattle.Team[] _Teams = new TeamBattle.Team[3];

	protected TeamBattle.Team[] _TeamsResult;

	protected TeamBattle.Team[] _TeamsTabMenu;

	protected SecuredValue<bool> _GameEnded = false;

	protected SecuredValue<int> _WinnerTeam = 0;

	protected SecuredValue<float> _GameTime = TeamBattle.RoundDuration;

	private float _TimeLastSynchronization;

	protected SecuredValue<int> _TryStartTeam = 0;

	protected class Player
	{
		public Player(string name, object[] data)
		{
			this.Name = name;
			if (data != null)
			{
				this.Data = data;
			}
		}

		public object[] Data
		{
			get
			{
				return new object[]
				{
					this.Score,
					this.Kills,
					this.Deads
				};
			}
			set
			{
				this.Score = (int)value[0];
				this.Kills = (int)value[1];
				this.Deads = (int)value[2];
			}
		}

		public void Reset()
		{
			this.Score = 0;
			this.Kills = 0;
			this.Deads = 0;
		}

		public PlayerNode Node
		{
			get
			{
				return WorldGameObjectX.Instance.FindPlayerByNameX(this.Name);
			}
		}

		public int Level
		{
			get
			{
				PlayerNode node = this.Node;
				return (node == null) ? 1 : node.Level;
			}
		}

		public string ViewerID
		{
			get
			{
				PlayerNode node = this.Node;
				return (node == null) ? string.Empty : node.ViewerID;
			}
		}

		public SecuredValue<string> Name;

		public SecuredValue<int> Score = 0;

		public SecuredValue<int> Kills = 0;

		public SecuredValue<int> Deads = 0;
	}

	protected class Team
	{
		public TeamBattle.Player this[string playerName]
		{
			get
			{
				int num = this.Players.FindIndex((TeamBattle.Player player) => player.Name == playerName);
				return (num == -1) ? null : this.Players[num];
			}
			set
			{
				int num = this.Players.FindIndex((TeamBattle.Player player) => player.Name == playerName);
				if (value != null)
				{
					if (num != -1)
					{
						this.Players[num] = value;
					}
					else
					{
						this.Players.Add(value);
					}
				}
				else if (num != -1)
				{
					this.Players.RemoveAt(num);
				}
			}
		}

		public TeamBattle.Team GetCopy()
		{
			TeamBattle.Team team = new TeamBattle.Team();
			team.Score = this.Score.Value;
			foreach (TeamBattle.Player player in this.Players)
			{
				team.Players.Add(new TeamBattle.Player(player.Name, player.Data));
			}
			return team;
		}

		public List<TeamBattle.Player> Players = new List<TeamBattle.Player>();

		public SecuredValue<int> Score = 0;
	}
}
