using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HideSeek : TeamBattle
{
	public static void SetStartPoint(EntityType et)
	{
		if (et == EntityType.TEAM_SPAWN_ARROW_RED)
		{
			HideSeek.isRed = true;
		}
		else if (et == EntityType.TEAM_SPAWN_ARROW_BLUE)
		{
			HideSeek.isBlue = true;
		}
	}

	public static void SetRandomItem()
	{
		HideSeek.hide_item_id = UnityEngine.Random.Range(1, 12);
	}

	private void Start()
	{
		this.select_side = HideSeek.HIDE_SEEK_TEAM.NONE;
		this.ServerStartGame();
	}

	private new void Update()
	{
		WorldGameObjectX.Instance.IEShowPlayerCount();
		if (HG_WorkController.hgstatus == GameStatus.GS_WAIT)
		{
			if (Time.time > this._lastcheck + (float)this._wait_time)
			{
				int num = 0;
				foreach (PlayerNode playerNode in WorldGameObjectX.Instance.PlayerList)
				{
					if (playerNode.Avatar != null)
					{
						num++;
					}
				}
				KGUI.SetControlText("hud.battle.txt_generic_tip", string.Concat(new object[]
				{
					Localize.GetText("WAIT_PLAYER_TO_START", null),
					num,
					"/",
					HideSeek.max_hide_player + HideSeek.max_seek_player
				}));
				this._lastcheck = Time.time;
			}
		}
		else if (HG_WorkController.hgstatus == GameStatus.GS_PRE_START && this.select_side == HideSeek.HIDE_SEEK_TEAM.SEEK)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		}
		else if (HG_WorkController.hgstatus == GameStatus.GS_START && this.select_side == HideSeek.HIDE_SEEK_TEAM.HIDE && UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
		{
			if (this.CanWhistle && this.IsCanWhistle())
			{
				WorldGameObjectX.Instance.photonView.RPC("MakeWhistle", PhotonTargets.All, new object[]
				{
					WorldGameObjectX.Instance.MainPlayer.transform.position
				});
				this.ModifyPlayerData((int)this.select_side, WorldGameObjectX.Instance.MainPlayerNode.Name, 5, 0, 0);
				base.StartCoroutine(this.Whistle());
			}
			else
			{
				Chat.SendInfoF(string.Format(Localize.GetText("HS_WAIT_END_WISP", null), this.reload_time.ToString()), false);
			}
		}
	}

	private bool IsCanWhistle()
	{
		long num = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
		if (num - this._lastWhistleTime > 20L)
		{
			this._lastWhistleTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
			return true;
		}
		return false;
	}

	private IEnumerator Whistle()
	{
		Chat.SendInfoF(Localize.GetText("HS_PLUS_2", null), false);
		this.CanWhistle = false;
		UISprite sprite = KGUI.FindNode("hud.hide_seek.anchor_bottom_left.Whistle", false).GetComponent<UISprite>();
		sprite.fillAmount = 0f;
		this.reload_time = 20;
		do
		{
			yield return new WaitForSeconds(1f);
			this.reload_time--;
			sprite.fillAmount += 0.05f;
		}
		while (this.reload_time != 0);
		this.CanWhistle = true;
		yield break;
	}

	public override void ModifyPlayerData(int teamIndex, string playerName, int score, int kills, int deads)
	{
		TeamBattle.Player player = this._Teams[teamIndex][playerName];
		if (player != null)
		{
			if (score != 0)
			{
				TeamBattle.Player player2 = player;
				player2.Score += score;
			}
			else
			{
				player.Score = 0;
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
	protected override void OnPlayerDead(string killer, string victim, int score)
	{
		if (this._GameEnded)
		{
			return;
		}
		if (killer != victim)
		{
			int playerTeam = base.GetPlayerTeam(killer);
			if (playerTeam != 0)
			{
				this.ModifyPlayerData(playerTeam, killer, 20, 0, 0);
				base.AddTeamScore(playerTeam, score);
			}
		}
		int playerTeam2 = base.GetPlayerTeam(victim);
		if (playerTeam2 != 0)
		{
			this.ModifyPlayerData(playerTeam2, victim, 0, 0, 0);
		}
	}

	public override void SpawnToGame()
	{
		WorldGameObjectX.Instance.Respawn();
	}

	protected new void ServerStartGame()
	{
		this._GameTime = 5000f;
		this._GameEnded = false;
		KGUI.SetControlText("hud.battle.txt_time", string.Empty);
	}

	public override void PhotonEventWork(byte eventCode, object content, int senderId)
	{
		if (eventCode == 5)
		{
			string[] array = content.ToString().Split(new char[]
			{
				'_'
			});
			if (array[0] == "hgtime")
			{
				this.ShowTimeMsg((string)content);
			}
			else if (array[0] == "spawn")
			{
				this.SpawnBattleTo((string)content);
			}
			else if (array[0] == "goplay")
			{
				base.StartCoroutine("StartPlayBattle", (string)content);
			}
			else if (array[0] == "winer")
			{
				base.StartCoroutine("Winer", (string)content);
			}
		}
		else
		{
			base.PhotonEventWork(eventCode, content, senderId);
		}
	}

	private void ShowTimeMsg(string time)
	{
		string[] array = time.Split(new char[]
		{
			'_'
		});
		if (array[0] == "hgtime")
		{
			HG_WorkController.hgstatus = (GameStatus)int.Parse(array[2].ToString());
			if (array[2] == "1")
			{
				WorldGameObjectX.SetCustomPropertiesToHG();
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_Left_to_spawn", null) + array[1]);
			}
			else if (array[2] == "2")
			{
				KGUI.SetControlText("hud.battle.txt_generic_tip", string.Format(Localize.GetText("HS_NOW_HIDE", null), array[1]));
			}
			else if (array[2] == "3")
			{
				KGUI.SetControlText("hud.battle.txt_time", array[1].ToString());
			}
		}
	}

	private IEnumerator ShowHideTeamCount()
	{
		KGUI.SetControlText("hud.battle.txt_generic_tip", string.Format(Localize.GetText("HS_HIDE_COUNT", null), (this._Teams[1].Players.Count - 1).ToString()));
		yield return new WaitForSeconds(3f);
		yield break;
	}

	private void SpawnBattleTo(string tospawn)
	{
		this._isSpawn = true;
		string[] array = tospawn.Split(new char[]
		{
			'_'
		});
		if (array[0] == "spawn")
		{
			List<float> list = (from s in array[1].Split(new char[]
			{
				';'
			})
			select float.Parse(s)).ToList<float>();
			Vector3 pos = new Vector3(list[0], list[1] + 0.5f, list[2]);
			if (this.select_side == HideSeek.HIDE_SEEK_TEAM.SEEK)
			{
				WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
				this.spawnTo = pos;
			}
			else if (this.select_side == HideSeek.HIDE_SEEK_TEAM.HIDE)
			{
				WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(true);
				base.TeleportPlayerBattlePoint(pos);
			}
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>().hideSeekPlayerController.StartGame(HideSeek.hide_item_id);
		}
	}

	private IEnumerator StartPlayBattle(string msg)
	{
		HG_WorkController.hgstatus = GameStatus.GS_START;
		KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("HS_TIME_TO_SEEK", null));
		WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(true);
		if (this.select_side == HideSeek.HIDE_SEEK_TEAM.SEEK)
		{
			base.TeleportPlayerBattlePoint(this.spawnTo);
		}
		else if (this.select_side == HideSeek.HIDE_SEEK_TEAM.HIDE)
		{
			KGUI.SetNodes("hud.hide_seek", true, false);
		}
		yield return new WaitForSeconds(3f);
		KGUI.SetControlText("hud.battle.txt_generic_tip", string.Empty);
		KGUI.SetControlText("hud.battle.txt_generic_tip", string.Format(Localize.GetText("HS_HIDE_COUNT", null), this._Teams[1].Players.Count.ToString()));
		if (MainMenu.Instance.SelectedBuildItem != StorePurchase.NONE)
		{
			Chat.SendInfoF(HG_WorkController._player.PlayerName + " " + Localize.GetText("HG_GET_GUILD_MSG", null) + Localize.GetText("PURCHASE_" + Store.Purchases[MainMenu.Instance.SelectedBuildItem].Name, null), false);
		}
		yield break;
	}

	private IEnumerator Winer(string msg)
	{
		yield return new WaitForSeconds(2f);
		string[] data = msg.Split(new char[]
		{
			'_'
		});
		if (data[0] == "winer")
		{
			try
			{
				UnityEngine.Debug.Log("-------------- winer " + msg);
				HideSeek.HIDE_SEEK_TEAM hst = (HideSeek.HIDE_SEEK_TEAM)int.Parse(data[1]);
				foreach (TeamBattle.Player pp in this._Teams[(int)this.select_side].Players)
				{
					if (pp.Name == PhotonNetwork.playerName)
					{
						UnityEngine.Debug.Log("pp.Score = " + pp.Score);
						if (hst == this.select_side)
						{
							MainMenu.Instance.SwitchMenu(Menu.BattleResult, true, pp.Score);
						}
						else
						{
							MainMenu.Instance.SwitchMenu(Menu.BattleResult, false, pp.Score);
						}
						break;
					}
				}
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				UnityEngine.Debug.Log("ERROR CHECK HG WINNER " + ex.StackTrace);
			}
			yield return new WaitForSeconds(3f);
		}
		yield break;
	}

	[PunRPC]
	public override void StartPlay(int teamIndex)
	{
		this.SendPlayerTeamToSrver(teamIndex);
		base.StartPlay(teamIndex);
	}

	private void SendPlayerTeamToSrver(int teamIndex)
	{
		this.select_side = (HideSeek.HIDE_SEEK_TEAM)teamIndex;
		WorldGameObjectX.Instance.photonView.RPC("AddNewHideSeekPlayerInTeam", PhotonTargets.All, new object[]
		{
			teamIndex
		});
		UnityEngine.Debug.Log("StartPlay " + teamIndex);
	}

	public override void RefreshTabMenu(bool open = false)
	{
		base.RefreshTabMenu(open);
		KGUI.SetNodes("team_battle.button_close", false, false);
		KGUI.SetNodes("team_battle.button_red_team", true, false);
		KGUI.SetNodes("team_battle.button_blue_team", true, false);
		if (HG_WorkController.hgstatus != GameStatus.GS_WAIT || this.select_side != HideSeek.HIDE_SEEK_TEAM.NONE)
		{
			KGUI.SetNodes("team_battle.button_red_team", false, false);
			KGUI.SetNodes("team_battle.button_blue_team", false, false);
		}
		else
		{
			if (this._Teams[1].Players.Count >= HideSeek.max_hide_player)
			{
				KGUI.SetNodes("team_battle.button_red_team", false, false);
			}
			if (this._Teams[2].Players.Count >= HideSeek.max_seek_player)
			{
				KGUI.SetNodes("team_battle.button_blue_team", false, false);
			}
		}
	}

	public void SendKillingInfo(string dead_name)
	{
		Chat.SendInfoF(string.Format(Localize.GetText("HS_PLAYER_SEEKING", null), dead_name), false);
		base.StartCoroutine(this.ShowHideTeamCount());
		UnityEngine.Debug.Log("-----------------OnPlayerDead");
	}

	public static int max_hide_player = 8;

	public static int max_seek_player = 1;

	public static bool isRed;

	public static bool isBlue;

	public static int hide_item_id;

	private float _lastcheck;

	private int _wait_time = 2;

	public HideSeek.HIDE_SEEK_TEAM select_side;

	public int select_hide_item;

	private Vector3 spawnTo = Vector3.zero;

	private bool _isSpawn;

	private bool CanWhistle = true;

	private int reload_time;

	public static int hs_game_point;

	private long _lastWhistleTime;

	public enum HIDE_SEEK_TEAM
	{
		NONE,
		HIDE,
		SEEK
	}
}
