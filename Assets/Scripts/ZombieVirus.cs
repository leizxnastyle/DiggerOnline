using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieVirus : TeamBattle
{
	protected override void ServerStartGame()
	{
		base.ServerStartGame();
		this._State = ZombieVirus.GameState.None;
		base.StartCoroutine(this.GameProcess());
	}

	protected override void OnPhotonPlayerConnected(PhotonPlayer connectedPlayer)
	{
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("SetGameState", connectedPlayer, new object[]
			{
				(int)this._State
			});
		}
		base.OnPhotonPlayerConnected(connectedPlayer);
	}

	protected override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (newMasterClient.isLocal)
		{
			base.StartCoroutine(this.GameProcess());
		}
	}

	private IEnumerator GameProcess()
	{
		if (this._State == ZombieVirus.GameState.None)
		{
			base.photonView.RPC("SetGameState", PhotonTargets.All, new object[]
			{
				1
			});
			base.photonView.RPC("SetGameTime", PhotonTargets.All, new object[]
			{
				0f
			});
		}
		if (this._State == ZombieVirus.GameState.WaitPlayers)
		{
			while (this._Teams[2].Players.Count + this._Teams[1].Players.Count < 3)
			{
				yield return 0;
			}
			base.photonView.RPC("SetGameState", PhotonTargets.All, new object[]
			{
				2
			});
			base.photonView.RPC("SetGameTime", PhotonTargets.All, new object[]
			{
				30f
			});
		}
		if (this._State == ZombieVirus.GameState.WaitInfection)
		{
			yield return new WaitForSeconds(this._GameTime);
			MainMenu.Instance.HideMenu();
			base.photonView.RPC("SetGameState", PhotonTargets.All, new object[]
			{
				3
			});
			base.photonView.RPC("SetGameTime", PhotonTargets.All, new object[]
			{
				TeamBattle.RoundDuration
			});
			WorldGameObjectX.Instance.PlaySoundForAll(SoundManager.Sound.ZombieVirusInfection);
			List<TeamBattle.Player> allPlayers = new List<TeamBattle.Player>();
			allPlayers.AddRange(this._Teams[2].Players);
			allPlayers.AddRange(this._Teams[1].Players);
			Utils.Shuffle(allPlayers);
			int zombieCount = Mathf.CeilToInt((float)allPlayers.Count * 30f / 100f);
			for (int i = 0; i < allPlayers.Count; i++)
			{
				PlayerNode playerNode = WorldGameObjectX.Instance.FindPlayerByNameX(allPlayers[i].Name);
				PhotonPlayer photonPlayer = playerNode.NetPlayer;
				int teamIndex = (i >= zombieCount) ? 2 : 1;
				base.photonView.RPC("StartPlay", photonPlayer, new object[]
				{
					teamIndex
				});
			}
		}
		if (this._State == ZombieVirus.GameState.Infectioned)
		{
			while (this._Teams[2].Players.Count > 0)
			{
				if (this._GameTime <= 0f && !this._GameEnded)
				{
					base.photonView.RPC("SetGameState", PhotonTargets.All, new object[]
					{
						4
					});
					base.photonView.RPC("SetGameTime", PhotonTargets.All, new object[]
					{
						20f
					});
				}
				yield return 0;
			}
			base.photonView.RPC("SetGameState", PhotonTargets.All, new object[]
			{
				4
			});
			base.photonView.RPC("SetGameTime", PhotonTargets.All, new object[]
			{
				20f
			});
		}
		if (this._State == ZombieVirus.GameState.EndGame)
		{
			yield return new WaitForSeconds(this._GameTime);
			base.photonView.RPC("SetGameState", PhotonTargets.All, new object[]
			{
				0
			});
			base.photonView.RPC("SetGameTime", PhotonTargets.All, new object[]
			{
				0f
			});
			base.StartCoroutine(this.GameProcess());
			List<TeamBattle.Player> allPlayers2 = new List<TeamBattle.Player>();
			allPlayers2.AddRange(this._Teams[2].Players);
			allPlayers2.AddRange(this._Teams[1].Players);
			for (int j = 0; j < allPlayers2.Count; j++)
			{
				PlayerNode playerNode2 = WorldGameObjectX.Instance.FindPlayerByNameX(allPlayers2[j].Name);
				PhotonPlayer photonPlayer2 = playerNode2.NetPlayer;
				base.photonView.RPC("StartPlay", photonPlayer2, new object[]
				{
					2
				});
			}
			yield break;
		}
		yield break;
	}

	[PunRPC]
	private void SetGameState(int state)
	{
		this._State = (ZombieVirus.GameState)state;
		this._GameEnded = false;
		if (this._State == ZombieVirus.GameState.WaitPlayers)
		{
			this.SetTip("HUD_FEW_PLAYERS_TIP");
			base.StartCoroutine(this.ShowSelectWeaponMenu());
			UnityEngine.Debug.Log("isSetWeaponInGame ShowSelectWeaponMenu");
		}
		else if (this._State == ZombieVirus.GameState.WaitInfection)
		{
			this.SetTip("HUD_UNTIL_INFECTION_TIP");
		}
		else if (this._State == ZombieVirus.GameState.EndGame)
		{
			this.SetTip("HUD_GAME_ENDED");
		}
		else
		{
			this.SetTip(string.Empty);
		}
	}

	private void SetTip(string text)
	{
		KGUI.SetControlText("hud.battle.txt_generic_tip", (text.Length <= 0) ? string.Empty : Localize.GetText(text, null));
	}

	[PunRPC]
	public override void StartPlay(int teamIndex)
	{
		this.StartPlayPrepea(teamIndex);
		if (!WorldGameObjectX.Instance.IsWorldGenerated)
		{
			return;
		}
		if (teamIndex == 1)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetSpeedModifier(1.3f);
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerMotor>().JumpModifier = 2f;
			PlayerNetwork component = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>();
			component.SelectWeapon(0);
			for (int i = 0; i < component.MainWeapon.Length; i++)
			{
				component.MainWeapon[i] = null;
			}
			MainMenu.Instance.RefreshBattleWeapon(0);
			base.StartCoroutine(this.ZombieTipProcess());
		}
		else
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetSpeedModifier(1f);
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerMotor>().JumpModifier = 1f;
		}
	}

	public IEnumerator ShowSelectWeaponMenu()
	{
		yield return new WaitForSeconds(1f);
		UnityEngine.Debug.Log(this._State);
		MainMenu.Instance.SwitchMenu(Menu.SelectWeapon, false, null);
		yield break;
	}

	public void StartPlayPrepea(int teamIndex)
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
		for (int j = 0; j < 3; j++)
		{
			foreach (TeamBattle.Player player in this._Teams[j].Players)
			{
				if (player.Name != PhotonNetwork.playerName)
				{
					base.SetupPlayer(j, player.Name);
				}
			}
		}
		base.photonView.RPC("SetPlayerTeam", PhotonTargets.AllViaServer, new object[]
		{
			teamIndex
		});
	}

	public override Vector3 GenerateSpawnPoint()
	{
		return GameType.FindRandomPlace(null, 10f);
	}

	private IEnumerator KillRespawnProcess()
	{
		if (!WorldGameObjectX.Instance.MainPlayerDead)
		{
			WorldGameObjectX.Instance.KillPlayer();
		}
		yield return new WaitForSeconds(3f);
		if (WorldGameObjectX.Instance.MainPlayerDead)
		{
			WorldGameObjectX.Instance.Respawn();
		}
		yield break;
	}

	private IEnumerator ZombieTipProcess()
	{
		this.SetTip("HUD_YOU_ARE_ZOMBIE_TIP");
		yield return new WaitForSeconds(10f);
		this.SetTip(string.Empty);
		yield break;
	}

	[PunRPC]
	protected override void OnPlayerDead(string killer, string victim, int score)
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
		base.OnPlayerDead(killer, victim, score);
		if (base.GetPlayerTeam(victim) == 2)
		{
			PlayerNode playerNode = WorldGameObjectX.Instance.FindPlayerByNameX(victim);
			if (playerNode != null)
			{
				base.photonView.RPC("StartPlay", playerNode.NetPlayer, new object[]
				{
					1
				});
			}
		}
	}

	public override int GetDefaultTeam()
	{
		if (this._State == ZombieVirus.GameState.Infectioned)
		{
			return 1;
		}
		return 2;
	}

	public override int GetSkin()
	{
		if (base.GetPlayerTeam(null) == 1)
		{
			return 6;
		}
		return base.GetSkin();
	}

	public override void RefreshTabMenu(bool open)
	{
		base.RefreshTabMenu(open);
		KGUI.SetNodes("team_battle.button_red_team", this._State == ZombieVirus.GameState.Infectioned && base.GetPlayerTeam(null) != 1, false);
		KGUI.SetNodes("team_battle.button_blue_team", false, false);
	}

	private void RefreshScore()
	{
		KGUI.SetControlText("hud.battle.txt_zombies_count", string.Empty + this._Teams[1].Players.Count);
		KGUI.SetControlText("hud.battle.txt_survivors_count", string.Empty + this._Teams[2].Players.Count);
	}

	protected override void Update()
	{
		base.Update();
		if (base.GetPlayerTeam(null) == 1)
		{
			if (Time.time >= this._NextZombieSound)
			{
				this._NextZombieSound = Time.time + 10f + UnityEngine.Random.Range(0f, 10f);
				SoundManager.Instance.PlayAtPoint(SoundManager.Sound.ZombieActivity, WorldGameObjectX.Instance.MainPlayer.transform.position);
			}
			Color color = WorldGameObjectX.Instance.ZombieEffect.GetComponent<GUITexture>().color;
			color.a = 0.1f + Mathf.PingPong(Time.time / 2f, 0.4f);
			WorldGameObjectX.Instance.ZombieEffect.GetComponent<GUITexture>().color = color;
		}
		else
		{
			Color color2 = WorldGameObjectX.Instance.ZombieEffect.GetComponent<GUITexture>().color;
			if (color2.a != 0f)
			{
				color2.a = 0f;
				WorldGameObjectX.Instance.ZombieEffect.GetComponent<GUITexture>().color = color2;
			}
		}
	}

	public override bool IsTeammate(string playerName)
	{
		return this._State != ZombieVirus.GameState.Infectioned || base.IsTeammate(playerName);
	}

	public override bool IsOpposite(string playerName)
	{
		return this._State == ZombieVirus.GameState.Infectioned && base.IsOpposite(playerName);
	}

	public override void OnDead(string killer, string victim, int score)
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
		base.OnDead(killer, victim, score);
	}

	[PunRPC]
	public override void AddToTeam(int teamIndex, string playerName, object[] playerData, PhotonMessageInfo info = null)
	{
		base.AddToTeam(teamIndex, playerName, playerData, info);
		this.RefreshScore();
	}

	[PunRPC]
	protected override void RemoveFromTeam(int teamIndex, string playerName, PhotonMessageInfo info = null)
	{
		base.RemoveFromTeam(teamIndex, playerName, info);
		this.RefreshScore();
	}

	[PunRPC]
	protected override void EndGame(int winner)
	{
		base.EndGame(winner);
	}

	[PunRPC]
	public override void SetGameTime(float time)
	{
		base.SetGameTime(time);
	}

	[PunRPC]
	protected override void SetTeamData(int teamIndex, int teamScore)
	{
		base.SetTeamData(teamIndex, teamScore);
		this.RefreshScore();
	}

	[PunRPC]
	public override void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		base.UpdatePlayerData(teamIndex, playerName, playerData);
	}

	public override bool IsNeedShowWeaponDialog()
	{
		if (this._State == ZombieVirus.GameState.WaitPlayers)
		{
			base.StartCoroutine(this.ShowSelectWeaponMenu());
			return true;
		}
		return false;
	}

	public const int ZombiesTeamIndex = 1;

	public const int SurvivorsTeamIndex = 2;

	public const float InfectionTime = 30f;

	public const float ZombiesPercent = 30f;

	public const int MinPlayersForStart = 3;

	public const float ZombieSoundDuration = 10f;

	private ZombieVirus.GameState _State;

	private float _NextZombieSound;

	private enum GameState
	{
		None,
		WaitPlayers,
		WaitInfection,
		Infectioned,
		EndGame
	}
}
