using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EreshWork.Scripts.GameTypes;
using UnityEngine;

public class Arcade : Deathmatch
{
	private void Start()
	{
		Arcade.AddSpeed = 0;
		this.isSpawn = false;
		Arcade.Place = -1;
	}

	public override void SpawnToGame()
	{
		base.StartCoroutine(this.SPAWN());
	}

	private IEnumerator SPAWN()
	{
		KGUI.SetControlText("hud.battle.txt_time", string.Empty);
		yield return new WaitForSeconds(2f);
		if (!this.isSpawn)
		{
			UnityEngine.Debug.Log("SpawnToGame");
			MainMenu.Instance.HideMenu();
			if (HG_WorkController.hgstatus == GameStatus.GS_WAIT)
			{
				this.isSpawn = true;
				TeamBattle.Instance.StartPlay(1);
				if (WorldGameObjectX.Instance.MainPlayer == null)
				{
					UnityEngine.Debug.Log("MainPlayer is null");
				}
				UnityEngine.Debug.Log(HG_WorkController.hgstatus);
				PlayerNetwork player = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>();
				WorldGameObjectX.Instance.Respawn();
				KGUI.SetControlText("hud.battle.txt_time", string.Empty);
			}
		}
		yield break;
	}

	private new void Update()
	{
		WorldGameObjectX.Instance.IEShowPlayerCountArcade(Arcade.AddSpeed);
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
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("WAIT_PLAYER_TO_START", null) + num + "/8");
				this._lastcheck = Time.time;
			}
		}
		else if (HG_WorkController.hgstatus == GameStatus.GS_PRE_START)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		}
		else if (HG_WorkController.hgstatus == GameStatus.GS_START)
		{
			if (Time.time > this._lastTicTime + this._waitTicTime)
			{
				this._lastTicTime = Time.time;
				Arcade.AddSpeed++;
				WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetSpeedModifier(1f + 0.05f * (float)Arcade.AddSpeed);
				SoundManager.Instance.Play(SoundManager.Sound.CtfFlagDelivered, WorldGameObjectX.Instance.MainPlayerEyes.GetComponent<AudioSource>());
			}
		}
		else if (HG_WorkController.hgstatus == GameStatus.GS_PRE_ARENA)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		}
	}

	public override void RefreshTabMenu(bool open)
	{
		KGUI.SetNodes("deathmatch.button_observe", false, false);
		base.RefreshTabMenu(open);
	}

	public override bool IsTeammate(string playerName)
	{
		return this._GameEnded;
	}

	public override bool IsOpposite(string playerName)
	{
		return !this._GameEnded;
	}

	[PunRPC]
	public override void StartPlay(int teamIndex)
	{
		WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetSpeedModifier(1f);
		Arcade.AddSpeed = 0;
		base.StartPlay(teamIndex);
	}

	public override void SetEnableFallingMode(bool flag)
	{
		if (!flag)
		{
			if (WorldGameObjectX.Instance.MainPlayer.GetComponent<KillBlocksController>() != null)
			{
				UnityEngine.Object.Destroy(WorldGameObjectX.Instance.MainPlayer.GetComponent<KillBlocksController>());
				this._observingNow = false;
			}
		}
		else if (!this._observingNow)
		{
			KillBlocksController killBlocksController = WorldGameObjectX.Instance.MainPlayer.AddComponent<KillBlocksController>();
			killBlocksController.crashBlockAfterTime = this.crashBlockAfterTime;
			killBlocksController.fallingBlockPrefab = this.fallingBlockPrefab;
			this._observingNow = true;
		}
	}

	[PunRPC]
	protected override void EndGame(int winner)
	{
		this._GameEnded = true;
		this._GameTime = 20f;
	}

	public override void MainPlayerDeadProcess()
	{
	}

	public override void PhotonEventWork(byte eventCode, object content, int senderId)
	{
		if (eventCode == 5)
		{
			string[] array = content.ToString().Split(new char[]
			{
				'_'
			});
			string text = array[0];
			switch (text)
			{
			case "hgtime":
				this.ShowTimeMsg((string)content);
				break;
			case "spawn":
				this.SpawnBattleTo((string)content);
				break;
			case "goplay":
				base.StartCoroutine("StartPlayBattle", (string)content);
				break;
			case "winer":
				base.StartCoroutine("WinerHG", (string)content);
				break;
			case "place":
				this.EndArcadeGame(int.Parse(array[1]));
				break;
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
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_start_battle_after", null) + array[1]);
				CheatFinderManager.CheckStayPosition();
			}
			else if (array[2] == "3" && int.Parse(array[1]) <= 60)
			{
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_fight_in_the_arena", null) + array[1]);
			}
			else if (array[2] == "4")
			{
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_start_the_arena", null) + array[1]);
			}
		}
	}

	private void SpawnBattleTo(string tospawn)
	{
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
			base.TeleportPlayerBattlePoint(pos);
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		}
	}

	private IEnumerator StartPlayBattle(string msg)
	{
		this._lastTicTime = Time.time;
		HG_WorkController.hgstatus = GameStatus.GS_START;
		KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("arcade_in_the_attack", null));
		WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(true);
		yield return new WaitForSeconds(3f);
		KGUI.SetControlText("hud.battle.txt_generic_tip", string.Empty);
		if (MainMenu.Instance.SelectedBuildItem != StorePurchase.NONE)
		{
			Chat.SendInfoF(HG_WorkController._player.PlayerName + " " + Localize.GetText("HG_GET_GUILD_MSG", null) + Localize.GetText("PURCHASE_" + Store.Purchases[MainMenu.Instance.SelectedBuildItem].Name, null), false);
		}
		Chat.SendInfoF("+2" + Localize.GetText("HG_ADD_GOLD_GAME", null), false);
		HG_WorkController.golds += 2;
		TeamBattle.Instance.SetEnableFallingMode(true);
		yield break;
	}

	private IEnumerator WinerHG(string msg)
	{
		KillBlocksController rbc = HG_WorkController._player.GetComponent<KillBlocksController>();
		if (rbc != null)
		{
			UnityEngine.Object.Destroy(rbc);
		}
		yield return new WaitForSeconds(2f);
		string[] data = msg.Split(new char[]
		{
			'_'
		});
		if (data[0] == "winer")
		{
			try
			{
				int point = 25;
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_winner_is", null) + WorldGameObjectX.Instance.FindPlayerByViewerID(data[1]).Name);
				bool isWiner = WorldGameObjectX.Instance.FindPlayerByViewerID(data[1]).Name == HG_WorkController._player.PlayerName;
				if (isWiner)
				{
					MainMenu.Instance.SwitchMenu(Menu.BattleResult, isWiner, point);
					WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
				}
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				UnityEngine.Debug.Log("ERROR CHECK ARCADE WINNER " + ex.StackTrace);
			}
			yield return new WaitForSeconds(3f);
		}
		yield break;
	}

	public void EndArcadeGame(int place)
	{
		Arcade.Place = place;
		int num = 0;
		int num2 = place;
		switch (num2 + 1)
		{
		case 0:
			num = 0;
			place = 99;
			break;
		case 2:
			num += 25;
			break;
		case 3:
			num += 20;
			break;
		case 4:
			num += 15;
			break;
		case 5:
			num += 10;
			break;
		case 6:
			num += 5;
			break;
		case 7:
			num += 4;
			break;
		case 8:
			num += 3;
			break;
		case 9:
			num += 2;
			break;
		}
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"EndArcadeDie ",
			num,
			" count ",
			place
		}));
		MainMenu.Instance.SwitchMenu(Menu.BattleResult, false, num);
	}

	[SerializeField]
	private float crashBlockAfterTime = 0.5f;

	[SerializeField]
	private GameObject fallingBlockPrefab;

	public int GameGold;

	public int GameExp;

	private bool _observingNow;

	private float _lastcheck;

	private int _wait_time = 2;

	private bool isSpawn;

	public static int Place = -1;

	public static int AddSpeed;

	private float _lastTicTime;

	private float _waitTicTime = 25f;
}
