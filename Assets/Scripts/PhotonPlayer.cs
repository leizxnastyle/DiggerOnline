using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPlayer
{
	public PhotonPlayer(bool isLocal, int actorID, string name)
	{
		this.customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		this.nameField = name;
	}

	protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
	{
		this.customProperties = new Hashtable();
		this.isLocal = isLocal;
		this.actorID = actorID;
		this.InternalCacheProperties(properties);
	}

	public int ID
	{
		get
		{
			return this.actorID;
		}
	}

	public string name
	{
		get
		{
			return this.nameField;
		}
		set
		{
			if (!this.isLocal)
			{
				UnityEngine.Debug.LogError("Error: Cannot change the name of a remote player!");
				return;
			}
			if (string.IsNullOrEmpty(value) || value.Equals(this.nameField))
			{
				return;
			}
			this.nameField = value;
			PhotonNetwork.playerName = value;
		}
	}

	public bool isMasterClient
	{
		get
		{
			return PhotonNetwork.networkingPeer.mMasterClientId == this.ID;
		}
	}

	public Hashtable customProperties { get; internal set; }

	public Hashtable allProperties
	{
		get
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Merge(this.customProperties);
			hashtable[byte.MaxValue] = this.name;
			return hashtable;
		}
	}

	public override bool Equals(object p)
	{
		PhotonPlayer photonPlayer = p as PhotonPlayer;
		return photonPlayer != null && this.GetHashCode() == photonPlayer.GetHashCode();
	}

	public override int GetHashCode()
	{
		return this.ID;
	}

	internal void InternalChangeLocalID(int newID)
	{
		if (!this.isLocal)
		{
			UnityEngine.Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
			return;
		}
		this.actorID = newID;
	}

	internal void InternalCacheProperties(Hashtable properties)
	{
		if (properties == null || properties.Count == 0 || this.customProperties.Equals(properties))
		{
			return;
		}
		if (properties.ContainsKey(255))
		{
			this.nameField = (string)properties[byte.MaxValue];
		}
		if (properties.ContainsKey(254))
		{
		}
		this.customProperties.MergeStringKeys(properties);
		this.customProperties.StripKeysWithNullValues();
	}

	public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, bool webForward = false)
	{
		if (propertiesToSet == null)
		{
			return;
		}
		Hashtable hashtable = propertiesToSet.StripToStringKeys();
		Hashtable hashtable2 = expectedValues.StripToStringKeys();
		bool flag = hashtable2 == null || hashtable2.Count == 0;
		bool flag2 = this.actorID > 0 && !PhotonNetwork.offlineMode;
		if (flag2)
		{
			PhotonNetwork.networkingPeer.OpSetPropertiesOfActor(this.actorID, hashtable, hashtable2, webForward);
		}
		if (!flag2 || flag)
		{
			this.InternalCacheProperties(hashtable);
			NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[]
			{
				this,
				hashtable
			});
		}
	}

	public static PhotonPlayer Find(int ID)
	{
		if (PhotonNetwork.networkingPeer != null)
		{
			return PhotonNetwork.networkingPeer.GetPlayerWithId(ID);
		}
		return null;
	}

	public PhotonPlayer Get(int id)
	{
		return PhotonPlayer.Find(id);
	}

	public PhotonPlayer GetNext()
	{
		return this.GetNextFor(this.ID);
	}

	public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer)
	{
		if (currentPlayer == null)
		{
			return null;
		}
		return this.GetNextFor(currentPlayer.ID);
	}

	public PhotonPlayer GetNextFor(int currentPlayerId)
	{
		if (PhotonNetwork.networkingPeer == null || PhotonNetwork.networkingPeer.mActors == null || PhotonNetwork.networkingPeer.mActors.Count < 2)
		{
			return null;
		}
		Dictionary<int, PhotonPlayer> mActors = PhotonNetwork.networkingPeer.mActors;
		int num = int.MaxValue;
		int num2 = currentPlayerId;
		foreach (int num3 in mActors.Keys)
		{
			if (num3 < num2)
			{
				num2 = num3;
			}
			else if (num3 > currentPlayerId && num3 < num)
			{
				num = num3;
			}
		}
		return (num == int.MaxValue) ? mActors[num2] : mActors[num];
	}

	public override string ToString()
	{
		if (string.IsNullOrEmpty(this.name))
		{
			return string.Format("#{0:00}{1}", this.ID, (!this.isMasterClient) ? string.Empty : "(master)");
		}
		return string.Format("'{0}'{1}", this.name, (!this.isMasterClient) ? string.Empty : "(master)");
	}

	public string ToStringFull()
	{
		return string.Format("#{0:00} '{1}' {2}", this.ID, this.name, this.customProperties.ToStringFull());
	}

	private int actorID = -1;

	private string nameField = string.Empty;

	public readonly bool isLocal;

	public object TagObject;
}
