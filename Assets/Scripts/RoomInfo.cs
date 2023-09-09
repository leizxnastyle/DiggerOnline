using System;
using ExitGames.Client.Photon;

public class RoomInfo
{
	protected internal RoomInfo(string roomName, Hashtable properties)
	{
		this.InternalCacheProperties(properties);
		this.nameField = roomName;
	}

	public bool removedFromList { get; internal set; }

	protected internal bool serverSideMasterClient { get; private set; }

	public Hashtable customProperties
	{
		get
		{
			return this.customPropertiesField;
		}
	}

	public string name
	{
		get
		{
			return this.nameField;
		}
	}

	public int playerCount { get; private set; }

	public bool isLocalClientInside { get; set; }

	public byte maxPlayers
	{
		get
		{
			return this.maxPlayersField;
		}
	}

	public bool open
	{
		get
		{
			return this.openField;
		}
	}

	public bool visible
	{
		get
		{
			return this.visibleField;
		}
	}

	public override bool Equals(object p)
	{
		Room room = p as Room;
		return room != null && this.nameField.Equals(room.nameField);
	}

	public override int GetHashCode()
	{
		return this.nameField.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", new object[]
		{
			this.nameField,
			(!this.visibleField) ? "hidden" : "visible",
			(!this.openField) ? "closed" : "open",
			this.maxPlayersField,
			this.playerCount
		});
	}

	public string ToStringFull()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
		{
			this.nameField,
			(!this.visibleField) ? "hidden" : "visible",
			(!this.openField) ? "closed" : "open",
			this.maxPlayersField,
			this.playerCount,
			this.customPropertiesField.ToStringFull()
		});
	}

	protected internal void InternalCacheProperties(Hashtable propertiesToCache)
	{
		if (propertiesToCache == null || propertiesToCache.Count == 0 || this.customPropertiesField.Equals(propertiesToCache))
		{
			return;
		}
		if (propertiesToCache.ContainsKey(251))
		{
			this.removedFromList = (bool)propertiesToCache[251];
			if (this.removedFromList)
			{
				return;
			}
		}
		if (propertiesToCache.ContainsKey(255))
		{
			this.maxPlayersField = (byte)propertiesToCache[byte.MaxValue];
		}
		if (propertiesToCache.ContainsKey(253))
		{
			this.openField = (bool)propertiesToCache[253];
		}
		if (propertiesToCache.ContainsKey(254))
		{
			this.visibleField = (bool)propertiesToCache[254];
		}
		if (propertiesToCache.ContainsKey(252))
		{
			this.playerCount = (int)((byte)propertiesToCache[252]);
		}
		if (propertiesToCache.ContainsKey(249))
		{
			this.autoCleanUpField = (bool)propertiesToCache[249];
		}
		if (propertiesToCache.ContainsKey(248))
		{
			this.serverSideMasterClient = true;
			bool flag = this.masterClientIdField != 0;
			this.masterClientIdField = (int)propertiesToCache[248];
			if (flag)
			{
				PhotonNetwork.networkingPeer.UpdateMasterClient();
			}
		}
		this.customPropertiesField.MergeStringKeys(propertiesToCache);
	}

	private Hashtable customPropertiesField = new Hashtable();

	protected byte maxPlayersField;

	protected bool openField = true;

	protected bool visibleField = true;

	protected bool autoCleanUpField = PhotonNetwork.autoCleanUpPlayerObjects;

	protected string nameField;

	protected internal int masterClientIdField;
}
