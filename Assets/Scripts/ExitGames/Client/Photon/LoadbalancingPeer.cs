using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExitGames.Client.Photon
{
	internal class LoadbalancingPeer : PhotonPeer
	{
		public LoadbalancingPeer(ConnectionProtocol protocolType) : base(protocolType)
		{
		}

		internal bool IsProtocolSecure
		{
			get
			{
				return base.UsedProtocol == ConnectionProtocol.WebSocketSecure;
			}
		}

		public virtual bool OpGetRegions(string appId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[224] = appId;
			return this.OpCustom(220, dictionary, true, 0, true);
		}

		public virtual bool OpJoinLobby(TypedLobby lobby)
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinLobby()");
			}
			Dictionary<byte, object> dictionary = null;
			if (lobby != null && !lobby.IsDefault)
			{
				dictionary = new Dictionary<byte, object>();
				dictionary[213] = lobby.Name;
				dictionary[212] = (byte)lobby.Type;
			}
			return this.OpCustom(229, dictionary, true);
		}

		public virtual bool OpLeaveLobby()
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
			}
			return this.OpCustom(228, null, true);
		}

		private void RoomOptionsToOpParameters(Dictionary<byte, object> op, RoomOptions roomOptions)
		{
			if (roomOptions == null)
			{
				roomOptions = new RoomOptions();
			}
			Hashtable hashtable = new Hashtable();
			hashtable[253] = roomOptions.isOpen;
			hashtable[254] = roomOptions.isVisible;
			hashtable[250] = ((roomOptions.customRoomPropertiesForLobby != null) ? roomOptions.customRoomPropertiesForLobby : new string[0]);
			hashtable.MergeStringKeys(roomOptions.customRoomProperties);
			if (roomOptions.maxPlayers > 0)
			{
				hashtable[byte.MaxValue] = roomOptions.maxPlayers;
			}
			op[248] = hashtable;
			op[241] = roomOptions.cleanupCacheOnLeave;
			if (roomOptions.cleanupCacheOnLeave)
			{
				hashtable[249] = true;
			}
			if (roomOptions.suppressRoomEvents)
			{
				op[237] = true;
			}
		}

		public virtual bool OpCreateRoom(LoadbalancingPeer.EnterRoomParams opParams)
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpCreateRoom()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (!string.IsNullOrEmpty(opParams.RoomName))
			{
				dictionary[byte.MaxValue] = opParams.RoomName;
			}
			if (opParams.Lobby != null && !string.IsNullOrEmpty(opParams.Lobby.Name))
			{
				dictionary[213] = opParams.Lobby.Name;
				dictionary[212] = (byte)opParams.Lobby.Type;
			}
			if (opParams.OnGameServer)
			{
				if (opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0)
				{
					dictionary[249] = opParams.PlayerProperties;
					dictionary[250] = true;
				}
				this.RoomOptionsToOpParameters(dictionary, opParams.RoomOptions);
			}
			return this.OpCustom(227, dictionary, true);
		}

		public virtual bool OpJoinRoom(LoadbalancingPeer.EnterRoomParams opParams)
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRoom()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (!string.IsNullOrEmpty(opParams.RoomName))
			{
				dictionary[byte.MaxValue] = opParams.RoomName;
			}
			if (opParams.CreateIfNotExists)
			{
				dictionary[215] = 1;
				if (opParams.Lobby != null)
				{
					dictionary[213] = opParams.Lobby.Name;
					dictionary[212] = (byte)opParams.Lobby.Type;
				}
			}
			if (opParams.OnGameServer)
			{
				if (opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0)
				{
					dictionary[249] = opParams.PlayerProperties;
					dictionary[250] = true;
				}
				if (opParams.CreateIfNotExists)
				{
					this.RoomOptionsToOpParameters(dictionary, opParams.RoomOptions);
				}
			}
			return this.OpCustom(226, dictionary, true);
		}

		public virtual bool OpJoinRandomRoom(LoadbalancingPeer.OpJoinRandomRoomParams opJoinRandomRoomParams)
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandomRoom()");
			}
			Hashtable hashtable = new Hashtable();
			hashtable.MergeStringKeys(opJoinRandomRoomParams.ExpectedCustomRoomProperties);
			if (opJoinRandomRoomParams.ExpectedMaxPlayers > 0)
			{
				hashtable[byte.MaxValue] = opJoinRandomRoomParams.ExpectedMaxPlayers;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (hashtable.Count > 0)
			{
				dictionary[248] = hashtable;
			}
			if (opJoinRandomRoomParams.MatchingType != MatchmakingMode.FillRoom)
			{
				dictionary[223] = (byte)opJoinRandomRoomParams.MatchingType;
			}
			if (opJoinRandomRoomParams.TypedLobby != null && !string.IsNullOrEmpty(opJoinRandomRoomParams.TypedLobby.Name))
			{
				dictionary[213] = opJoinRandomRoomParams.TypedLobby.Name;
				dictionary[212] = (byte)opJoinRandomRoomParams.TypedLobby.Type;
			}
			if (!string.IsNullOrEmpty(opJoinRandomRoomParams.SqlLobbyFilter))
			{
				dictionary[245] = opJoinRandomRoomParams.SqlLobbyFilter;
			}
			return this.OpCustom(225, dictionary, true);
		}

		public virtual bool OpLeaveRoom(bool becomeInactive)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (becomeInactive)
			{
				dictionary[233] = becomeInactive;
			}
			return this.OpCustom(254, dictionary, true);
		}

		public virtual bool OpFindFriends(string[] friendsToFind)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (friendsToFind != null && friendsToFind.Length > 0)
			{
				dictionary[1] = friendsToFind;
			}
			return this.OpCustom(222, dictionary, true);
		}

		public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties)
		{
			return this.OpSetPropertiesOfActor(actorNr, actorProperties.StripToStringKeys(), null, false);
		}

		protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, bool webForward = false)
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
			}
			if (actorNr <= 0 || actorProperties == null)
			{
				if (base.DebugOut >= DebugLevel.INFO)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties != null.");
				}
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(251, actorProperties);
			dictionary.Add(254, actorNr);
			dictionary.Add(250, true);
			if (expectedProperties != null && expectedProperties.Count != 0)
			{
				dictionary.Add(231, expectedProperties);
			}
			if (webForward)
			{
				dictionary[234] = true;
			}
			UnityEngine.Debug.Log(dictionary.ToStringFull());
			return this.OpCustom(252, dictionary, true, 0, false);
		}

		protected void OpSetPropertyOfRoom(byte propCode, object value)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[propCode] = value;
			this.OpSetPropertiesOfRoom(hashtable, null, false);
		}

		public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
		{
			return this.OpSetPropertiesOfRoom(gameProperties.StripToStringKeys(), null, false);
		}

		protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, bool webForward = false)
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(251, gameProperties);
			dictionary.Add(250, true);
			if (expectedProperties != null && expectedProperties.Count != 0)
			{
				dictionary.Add(231, expectedProperties);
			}
			if (webForward)
			{
				dictionary[234] = true;
			}
			return this.OpCustom(252, dictionary, true, 0, false);
		}

		public virtual bool OpAuthenticate(string appId, string appVersion, AuthenticationValues authValues, string regionCode, bool getLobbyStatistics)
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (getLobbyStatistics)
			{
				dictionary[211] = true;
			}
			if (authValues != null && authValues.Token != null)
			{
				dictionary[221] = authValues.Token;
				return this.OpCustom(230, dictionary, true, 0, false);
			}
			dictionary[220] = appVersion;
			dictionary[224] = appId;
			if (!string.IsNullOrEmpty(regionCode))
			{
				dictionary[210] = regionCode;
			}
			if (authValues != null)
			{
				if (!string.IsNullOrEmpty(authValues.UserId))
				{
					dictionary[225] = authValues.UserId;
				}
				if (authValues.AuthType != CustomAuthenticationType.None)
				{
					if (!this.IsProtocolSecure && !base.IsEncryptionAvailable)
					{
						base.Listener.DebugReturn(DebugLevel.ERROR, "OpAuthenticate() failed. When you want Custom Authentication encryption is mandatory.");
						return false;
					}
					dictionary[217] = (byte)authValues.AuthType;
					if (!string.IsNullOrEmpty(authValues.Token))
					{
						dictionary[221] = authValues.Token;
					}
					else
					{
						if (!string.IsNullOrEmpty(authValues.AuthGetParameters))
						{
							dictionary[216] = authValues.AuthGetParameters;
						}
						if (authValues.AuthPostData != null)
						{
							dictionary[214] = authValues.AuthPostData;
						}
					}
				}
			}
			bool flag = this.OpCustom(230, dictionary, true, 0, base.IsEncryptionAvailable);
			if (!flag)
			{
				base.Listener.DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, CustomAuthenticationValues and if you're connected.");
			}
			return flag;
		}

		public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
		{
			if (base.DebugOut >= DebugLevel.ALL)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "OpChangeGroups()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (groupsToRemove != null)
			{
				dictionary[239] = groupsToRemove;
			}
			if (groupsToAdd != null)
			{
				dictionary[238] = groupsToAdd;
			}
			return this.OpCustom(248, dictionary, true, 0);
		}

		public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
		{
			this.opParameters.Clear();
			this.opParameters[244] = eventCode;
			if (customEventContent != null)
			{
				this.opParameters[245] = customEventContent;
			}
			if (raiseEventOptions == null)
			{
				raiseEventOptions = RaiseEventOptions.Default;
			}
			else
			{
				if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
				{
					this.opParameters[247] = (byte)raiseEventOptions.CachingOption;
				}
				if (raiseEventOptions.Receivers != ReceiverGroup.Others)
				{
					this.opParameters[246] = (byte)raiseEventOptions.Receivers;
				}
				if (raiseEventOptions.InterestGroup != 0)
				{
					this.opParameters[240] = raiseEventOptions.InterestGroup;
				}
				if (raiseEventOptions.TargetActors != null)
				{
					this.opParameters[252] = raiseEventOptions.TargetActors;
				}
				if (raiseEventOptions.ForwardToWebhook)
				{
					this.opParameters[234] = true;
				}
			}
			return this.OpCustom(253, this.opParameters, sendReliable, raiseEventOptions.SequenceChannel, raiseEventOptions.Encrypt);
		}

		private readonly Dictionary<byte, object> opParameters = new Dictionary<byte, object>();

		public class EnterRoomParams
		{
			public string RoomName;

			public RoomOptions RoomOptions;

			public TypedLobby Lobby;

			public Hashtable PlayerProperties;

			public bool OnGameServer = true;

			public bool CreateIfNotExists;
		}

		public class OpJoinRandomRoomParams
		{
			public Hashtable ExpectedCustomRoomProperties;

			public byte ExpectedMaxPlayers;

			public MatchmakingMode MatchingType;

			public TypedLobby TypedLobby;

			public string SqlLobbyFilter;
		}
	}
}
