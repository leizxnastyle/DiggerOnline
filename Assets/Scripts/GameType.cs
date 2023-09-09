using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class GameType : Photon.MonoBehaviour
{
	public static bool BattleMode()
	{
		return TeamBattle.Instance != null;
	}

	public static bool IsHungerGamesMode
	{
		get
		{
			return TeamBattle.Instance != null && TeamBattle.Instance is HungerGames;
		}
	}

	public static bool IsArcadeMode
	{
		get
		{
			return TeamBattle.Instance != null && TeamBattle.Instance is Arcade;
		}
	}

	public static bool IsHideSeek
	{
		get
		{
			return TeamBattle.Instance != null && TeamBattle.Instance is HideSeek;
		}
	}

	public static bool IsZombieGamesMode
	{
		get
		{
			return TeamBattle.Instance != null && TeamBattle.Instance is ZombieVirus;
		}
	}

	public static bool IsDeathmatchGamesMode
	{
		get
		{
			return TeamBattle.Instance != null && TeamBattle.Instance is Deathmatch;
		}
	}

	public static bool IsTeamBattleMode
	{
		get
		{
			return TeamBattle.Instance != null && TeamBattle.Instance.GetType() == typeof(TeamBattle);
		}
	}

	public static bool IsObserving()
	{
		return TeamBattle.Instance != null && TeamBattle.Instance.IsObserving(PhotonNetwork.playerName);
	}

	public static bool IsObserving(string name)
	{
		return TeamBattle.Instance != null && TeamBattle.Instance.IsObserving(name);
	}

	public static bool BuildDisabled()
	{
		return GameType.BattleMode();
	}

	public static bool BlocksBuildDisabled()
	{
		return WorldGameObjectX.Instance.MainPlayerNode.IsZombie;
	}

	public static bool DigDisabled()
	{
		return false;
	}

	public static bool HitDisabled()
	{
		return GameType.BattleMode();
	}

	public static bool CanSaveMap()
	{
		return !GameType.BattleMode();
	}

	public static Vector3 FindRandomPlace(Vector3[] awayFrom = null, float awayDistance = 10f)
	{
		int i = 0;
		while (i < 100)
		{
			int num = UnityEngine.Random.Range(5, WorldData.Instance.GetMaxBlockX() - 5);
			int num2 = UnityEngine.Random.Range(5, WorldData.Instance.GetMaxBlockY() - 5);
			int j;
			for (j = WorldData.Instance.DepthInBlocks; j >= 0; j--)
			{
				if (j == 0 || WorldData.Instance.GetBlockType(num, num2, j - 1) != BlockType.Air)
				{
					break;
				}
			}
			Vector3 vector = new Vector3((float)num + 0.5f, (float)j + 0.5f, (float)num2 + 0.5f);
			if (awayFrom != null)
			{
				bool flag = false;
				foreach (Vector3 b in awayFrom)
				{
					if (Vector3.Distance(vector, b) < awayDistance)
					{
						flag = true;
						break;
					}
				}
				if (flag && (float)i < awayDistance - 1f)
				{
					i++;
					continue;
				}
			}
			return vector;
		}
		return Vector3.zero;
	}

	public static void Activate()
	{
		string text = null;
		string text2 = null;
		UnityEngine.Debug.Log("App.Instance.Settings.gameType " + App.Instance.Settings.gameType);
		if (App.Instance.Settings.gameType == GameINI.GameType.DEATHMATCH)
		{
			text = "GameType/Deathmatch";
		}
		else if (App.Instance.Settings.gameType == GameINI.GameType.TEAM_BATTLE)
		{
			int num = 0;
			int num2 = 0;
			List<GameObject> list = new List<GameObject>();
			List<GameObject> list2 = new List<GameObject>();
			foreach (SpawnArrow spawnArrow in UnityEngine.Object.FindObjectsOfType(typeof(SpawnArrow)))
			{
				if (spawnArrow.Type == EntityType.TEAM_SPAWN_ARROW_RED)
				{
					num++;
					list.Add(spawnArrow.gameObject);
				}
				else if (spawnArrow.Type == EntityType.TEAM_SPAWN_ARROW_BLUE)
				{
					num2++;
					list2.Add(spawnArrow.gameObject);
				}
			}
			if (GameType.CheckSpawnDistance(list, list2, 43))
			{
				text2 = "SPAWN_MIN_DISTANCE";
			}
			else if (num > 0 && num2 > 0)
			{
				text = "GameType/TeamBattle";
			}
			else if (num == 0 && !App.Instance.Settings.isWatch)
			{
				text2 = "HINT_MISSED_SPAWN_POINTS";
			}
			else if (num2 == 0)
			{
				text2 = "HINT_MISSED_BLUE_SPAWN_POINTS";
			}
		}
		else if (App.Instance.Settings.gameType == GameINI.GameType.ZOMBIE_VIRUS)
		{
			text = "GameType/ZombieVirus";
		}
		else if (App.Instance.Settings.gameType == GameINI.GameType.HUNGER_GAMES)
		{
			List<GameObject> list3 = new List<GameObject>();
			int num3 = 0;
			foreach (EntityBase entityBase in EntityBase.Entities)
			{
				if (entityBase.Type == EntityType.HG_APOS_SPAWN_POINT)
				{
					list3.Add(entityBase.gameObject);
				}
				else if (entityBase.Type == EntityType.HG_ARENA_SPAWN_POINT)
				{
					num3++;
				}
			}
			if (list3.Count < 8)
			{
				text2 = "HG_APOS_SPAWN_POINT_NO_EXIST_ALL";
			}
			else if (num3 != 1)
			{
				text2 = "HG_ARENA_SPAWN_POINT_NO_EXIST";
			}
			else if (list3.Count >= 8 && num3 == 1)
			{
				text = "GameType/Hunger_Games";
			}
		}
		else if (App.Instance.Settings.gameType == GameINI.GameType.ARCADE)
		{
			text = "GameType/Arcade";
		}
		else if (App.Instance.Settings.gameType == GameINI.GameType.HIDE_SEEK)
		{
			bool flag = false;
			foreach (EntityBase entityBase2 in EntityBase.Entities)
			{
				if (entityBase2.Type == EntityType.SPAWN_ARROW)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				text2 = "HS_NO_START_POINT";
			}
			else if (!HideSeek.isRed)
			{
				text2 = "HS_NO_HIDE_POINT";
			}
			else if (!HideSeek.isBlue)
			{
				text2 = "HS_NO_SEEK_POINT";
			}
			else
			{
				text = "GameType/Hide_Seek";
			}
		}
		if (text != null)
		{
			PhotonNetwork.InstantiateSceneObject(text, Vector3.zero, Quaternion.identity, 0, null);
		}
		else if (App.Instance.Settings.gameType != GameINI.GameType.BUILDING)
		{
			App.Instance.Settings.gameType = GameINI.GameType.BUILDING;
			Hashtable hashtable = new Hashtable();
			hashtable.Add("game_type", App.Instance.Settings.gameType);
			PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
			UnityEngine.Debug.Log("SET " + App.Instance.Settings.gameType);
		}
		WorldGameObjectX.Instance.ShowGameGUI();
		if (text2 != null)
		{
			MainMenu.Instance.ShowHint(text2, true);
		}
	}

	public static bool IsHGNoInstanceCheck
	{
		get
		{
			return App.Instance.Settings.gameType == GameINI.GameType.HUNGER_GAMES;
		}
	}

	private static bool CheckSpawnDistance(List<GameObject> spawnRed, List<GameObject> spawnBlue, int min_distance = 43)
	{
		foreach (GameObject gameObject in spawnBlue)
		{
			foreach (GameObject gameObject2 in spawnRed)
			{
				float num = Vector3.Distance(gameObject.transform.position, gameObject2.transform.position);
				if (num < (float)min_distance)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsNeedCheckTips()
	{
		return !GameType.IsArcadeMode && !GameType.IsHungerGamesMode && !GameType.IsHideSeek;
	}

	public static int GetOptionsDestroy()
	{
		if (GameType.IsArcadeMode)
		{
			return 0;
		}
		return ProfileINI.options_destroy;
	}
}
