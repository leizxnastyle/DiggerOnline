using System;
using UnityEngine;

public class SpawnArrow : EntityBase
{
	public int TeamIndex
	{
		get
		{
			return (this.Type != EntityType.SPAWN_ARROW) ? ((this.Type != EntityType.TEAM_SPAWN_ARROW_RED) ? 2 : 1) : 0;
		}
	}

	protected override void Creation(object[] data)
	{
		if (this.Type == EntityType.SPAWN_ARROW)
		{
			if (SpawnArrow.CurSpawn == null)
			{
				SpawnArrow.CurSpawn = this;
				if (App.Instance.Settings.gameType == GameINI.GameType.HUNGER_GAMES || App.Instance.Settings.gameType == GameINI.GameType.ARCADE || App.Instance.Settings.gameType == GameINI.GameType.HIDE_SEEK)
				{
					HG_WorkController.CurStartSpawn = base.transform.position;
					base.GetComponent<Collider>().enabled = false;
					if (App.Instance.Settings.gameType == GameINI.GameType.HUNGER_GAMES)
					{
						base.transform.FindChild("checkpoint_flag").gameObject.SetActive(false);
					}
					return;
				}
			}
		}
		else if (App.Instance.Settings.gameType == GameINI.GameType.HIDE_SEEK)
		{
			WorldGameObjectX.Instance.photonView.RPC("BattleStartPointTeam", PhotonTargets.All, new object[]
			{
				string.Concat(new object[]
				{
					base.transform.position.x,
					";",
					base.transform.position.y,
					";",
					base.transform.position.z
				}),
				this.TeamIndex
			});
			HideSeek.SetStartPoint(this.Type);
		}
		if (App.Instance.Settings.gameType == GameINI.GameType.TEAM_BATTLE)
		{
			base.transform.FindChild("Plane").gameObject.SetActive(false);
		}
	}

	protected override void Destruction()
	{
		if (SpawnArrow.CurSpawn == this && App.Instance.Settings.gameType != GameINI.GameType.HUNGER_GAMES)
		{
			SpawnArrow.CurSpawn = null;
			SpawnArrow[] array = UnityEngine.Object.FindObjectsOfType(typeof(SpawnArrow)) as SpawnArrow[];
			foreach (SpawnArrow spawnArrow in array)
			{
				if (spawnArrow.Type == EntityType.SPAWN_ARROW)
				{
					SpawnArrow.CurSpawn = spawnArrow;
					break;
				}
			}
		}
	}

	protected override void Updating()
	{
		if (this.Type == EntityType.SPAWN_ARROW && SpawnArrow.CurSpawn != this && WorldGameObjectX.Instance.MainPlayerEyes != null && base.GetComponent<Collider>().bounds.Contains(WorldGameObjectX.Instance.MainPlayerEyes.transform.position))
		{
			SpawnArrow.CurSpawn = this;
			SoundManager.Instance.PlayAtPoint(SoundManager.Sound.CtfFlagCaptured, WorldGameObjectX.Instance.MainPlayerEyes.transform.position);
			Chat.SendInfoF(Localize.GetText("CHECKPOINT", null), false);
		}
	}

	public override void OnLeftMouseHit(string playerName)
	{
		if (TeamBattle.Instance != null)
		{
			Chat.SendInfoF(Localize.GetText("CANT_DESTROY_SPWAN", null), false);
		}
		else
		{
			base.OnLeftMouseHit(playerName);
		}
	}

	public override void OnButtonE(string player_name)
	{
		if (TeamBattle.Instance != null)
		{
			Chat.SendInfoF(Localize.GetText("CANT_MOVE_SPWAN", null), false);
			return;
		}
		if (Level.Instance.IsAdmin(null))
		{
			WorldGameObjectX.Instance.TakeEnity(base.gameObject);
		}
		else
		{
			Chat.SendInfoF(Localize.GetText("CANT_MOVE_SPWAN_ADMIN", null), false);
		}
	}

	public Vector3 GetSpawnPosition()
	{
		Vector3 position = base.transform.position;
		return new Vector3(position.x, Mathf.Round(position.y), position.z);
	}

	public static SpawnArrow CurSpawn;
}
