using System;

public class Ctf : TeamBattle
{
	[PunRPC]
	public override void AddToTeam(int teamIndex, string playerName, object[] playerData, PhotonMessageInfo info = null)
	{
		base.AddToTeam(teamIndex, playerName, playerData, info);
	}

	[PunRPC]
	protected override void RemoveFromTeam(int teamIndex, string playerName, PhotonMessageInfo info = null)
	{
		base.RemoveFromTeam(teamIndex, playerName, info);
	}

	[PunRPC]
	protected override void OnPlayerDead(string killer, string victim, int score)
	{
		base.OnPlayerDead(killer, victim, score);
	}

	[PunRPC]
	protected override void EndGame(int winner)
	{
		base.EndGame(winner);
	}

	[PunRPC]
	public override void StartPlay(int teamIndex)
	{
		base.StartPlay(teamIndex);
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
	public override void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		base.UpdatePlayerData(teamIndex, playerName, playerData);
	}

	public const int FlagScore = 30;

	public const float FlagTakeDistance = 3f;
}
