using System;

public class GameINI
{
	public GameINI.GameType gameType
	{
		get
		{
			return (!bs._Igor.teamBattle) ? this._gameType : GameINI.GameType.CTF;
		}
		set
		{
			this._gameType = value;
		}
	}

	public GameINI.MapSize mapSize;

	public GameINI.MapType mapType;

	public GameINI.MapTime mapTime;

	public string playerID;

	public int slotID;

	public bool loadingSavedMap;

	public int publicMapID;

	private GameINI.GameType _gameType;

	public byte mapPopulation;

	public bool isServer;

	public bool isServerAdministrator;

	public string Password;

	public bool isWatch;

	public byte ServerRating;

	public string server_name;

	public string server_about;

	public bool isOnline;

	public bool destroyable = true;

	public bool _firstSelect = true;

	public GameStatus gameStatus;

	public enum MapSize
	{
		SMALL,
		MEDIUM,
		LARGE,
		HUGE,
		TINY
	}

	public enum MapType
	{
		STANDART,
		FLAT,
		SAND,
		OCEAN,
		ISLAND,
		SNOWLAND,
		LAVA,
		PLATFORM,
		AUTUMN
	}

	public enum GameType
	{
		BUILDING,
		TEAM_BATTLE,
		DEATHMATCH,
		CTF,
		SURVIVAL,
		ZOMBIE_VIRUS,
		HUNGER_GAMES,
		ARCADE,
		HIDE_SEEK
	}

	public enum MapTime
	{
		DAY,
		NIGHT,
		SWITCH
	}
}
