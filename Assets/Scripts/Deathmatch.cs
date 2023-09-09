using System;
using UnityEngine;

public class Deathmatch : TeamBattle
{
	public override void RefreshTabMenu(bool open)
	{
		if (open && MainMenu.Instance.CurMenu != Menu.Deathmatch)
		{
			MainMenu.Instance.SwitchMenu(Menu.Deathmatch, null, null);
			return;
		}
		TeamBattle.Team[] array = (this._TeamsResult == null) ? this._Teams : this._TeamsResult;
		this._TeamsTabMenu = new TeamBattle.Team[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			this._TeamsTabMenu[i] = array[i].GetCopy();
		}
		KGUI.ResizeGrid("deathmatch.grid_players", this._TeamsTabMenu[1].Players.Count, delegate(GameObject slot, int index)
		{
			TeamBattle.Player player = this._TeamsTabMenu[1].Players[index];
			slot.transform.Find("txt_nickname").GetComponent<UILabel>().text = string.Empty + MainMenu.FixCollorName(player.Name);
			if (GameType.IsArcadeMode)
			{
				slot.transform.Find("txt_score").GetComponent<UILabel>().text = string.Empty;
				slot.transform.Find("level_icon").gameObject.SetActive(false);
			}
			else
			{
				slot.transform.Find("txt_score").GetComponent<UILabel>().text = string.Empty + player.Score;
				KGUI.SetControlSprite(slot.transform.Find("level_icon"), "level_" + player.Level, 0f);
			}
			MainMenu.Instance.AddVoiceHighlight(player.Node, slot.transform.Find("ibtn_voice"));
		}, "deathmatch");
		KGUI.SetNodes("deathmatch.button_observe", false, false);
	}

	public override bool IsTeammate(string playerName)
	{
		return this._GameEnded;
	}

	public override bool IsOpposite(string playerName)
	{
		return !this._GameEnded;
	}

	public override Vector3 GenerateSpawnPoint()
	{
		WorldData worldData = bs._WorldGameObjectX.WorldData;
		Vector3 vector = new Vector3((float)UnityEngine.Random.Range(0, worldData.WidthInBlocks - 1), (float)worldData.DepthInBlocks, (float)UnityEngine.Random.Range(0, worldData.HeightInBlocks - 1)) + Vector3.one / 2f;
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, Vector3.down, out raycastHit, 1000f, 1 << LayerMask.NameToLayer("Terrain")))
		{
			return raycastHit.point + Vector3.up;
		}
		return vector;
	}

	[PunRPC]
	public override void AddToTeam(int teamIndex, string playerName, object[] playerData, PhotonMessageInfo info = null)
	{
		base.AddToTeam(teamIndex, playerName, playerData, info);
	}

	[PunRPC]
	protected override void EndGame(int winner)
	{
		WorldGameObjectX.Instance.MainPlayer.GetComponent<CameraController>().SniperScopeOff();
		this._GameEnded = true;
		this._GameTime = 20f;
		int num = -1;
		int num2 = 0;
		this._TeamsResult = new TeamBattle.Team[this._Teams.Length];
		for (int i = 0; i < 3; i++)
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
		if (num == 1 && this._TeamsResult[1].Players.Count > 0)
		{
			if (this._TeamsResult[1].Players.FindIndex((TeamBattle.Player x) => x.Name == PhotonNetwork.playerName) < 3)
			{
				SoundManager.Instance.Play(SoundManager.Sound.BattleWin, null);
				MainMenu.Instance.SwitchMenu(Menu.BattleResult, true, num2);
				return;
			}
		}
		if (num != 0)
		{
			SoundManager.Instance.Play(SoundManager.Sound.BattleLose, null);
			MainMenu.Instance.SwitchMenu(Menu.BattleResult, false, num2);
		}
		else
		{
			MainMenu.Instance.SwitchMenu(Menu.Deathmatch, null, null);
		}
	}

	[PunRPC]
	protected override void RestartGame()
	{
		base.RestartGame();
	}

	[PunRPC]
	protected override void OnPlayerDead(string killer, string victim, int score)
	{
		base.OnPlayerDead(killer, victim, score);
	}

	[PunRPC]
	protected override void RemoveFromTeam(int teamIndex, string playerName, PhotonMessageInfo info = null)
	{
		base.RemoveFromTeam(teamIndex, playerName, info);
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
	}

	[PunRPC]
	public override void StartPlay(int teamIndex)
	{
		base.StartPlay(teamIndex);
	}

	[PunRPC]
	public override void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		base.UpdatePlayerData(teamIndex, playerName, playerData);
	}
}
