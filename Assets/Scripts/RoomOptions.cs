using System;
using ExitGames.Client.Photon;

public class RoomOptions
{
	public bool isVisible
	{
		get
		{
			return this.isVisibleField;
		}
		set
		{
			this.isVisibleField = value;
		}
	}

	public bool isOpen
	{
		get
		{
			return this.isOpenField;
		}
		set
		{
			this.isOpenField = value;
		}
	}

	public bool cleanupCacheOnLeave
	{
		get
		{
			return this.cleanupCacheOnLeaveField;
		}
		set
		{
			this.cleanupCacheOnLeaveField = value;
		}
	}

	public bool suppressRoomEvents
	{
		get
		{
			return this.suppressRoomEventsField;
		}
	}

	private bool isVisibleField = true;

	private bool isOpenField = true;

	public byte maxPlayers;

	private bool cleanupCacheOnLeaveField = PhotonNetwork.autoCleanUpPlayerObjects;

	public Hashtable customRoomProperties;

	public string[] customRoomPropertiesForLobby = new string[0];

	private bool suppressRoomEventsField;
}
