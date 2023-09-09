using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExitGames.Client.Photon.Chat
{
	internal class ChatPeer : PhotonPeer
	{
		public ChatPeer(IPhotonPeerListener listener, ConnectionProtocol protocol) : base(listener, protocol)
		{
			if (protocol == ConnectionProtocol.WebSocket || protocol == ConnectionProtocol.WebSocketSecure)
			{
				UnityEngine.Debug.Log("Using SocketWebTcp");
				base.SocketImplementation = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp");
			}
		}

		public string NameServerAddress
		{
			get
			{
				return this.GetNameServerAddress();
			}
		}

		internal virtual bool IsProtocolSecure
		{
			get
			{
				return base.UsedProtocol == ConnectionProtocol.WebSocketSecure;
			}
		}

		private string GetNameServerAddress()
		{
			ConnectionProtocol usedProtocol = base.UsedProtocol;
			int num = 0;
			ChatPeer.ProtocolToNameServerPort.TryGetValue(usedProtocol, out num);
			string arg = string.Empty;
			if (usedProtocol == ConnectionProtocol.WebSocket)
			{
				arg = "ws://";
			}
			else if (usedProtocol == ConnectionProtocol.WebSocketSecure)
			{
				arg = "wss://";
			}
			return string.Format("{0}{1}:{2}", arg, "ns.exitgames.com", num);
		}

		public bool Connect()
		{
			base.Listener.DebugReturn(DebugLevel.INFO, "Connecting to nameserver " + this.NameServerAddress);
			return this.Connect(this.NameServerAddress, "NameServer");
		}

		public bool AuthenticateOnNameServer(string appId, string appVersion, string region, AuthenticationValues authValues)
		{
			if (base.DebugOut >= DebugLevel.INFO)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[220] = appVersion;
			dictionary[224] = appId;
			dictionary[210] = region;
			if (authValues != null)
			{
				if (!string.IsNullOrEmpty(authValues.UserId))
				{
					dictionary[225] = authValues.UserId;
				}
				if (authValues != null && authValues.AuthType != CustomAuthenticationType.None)
				{
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
			return this.OpCustom(230, dictionary, true, 0, base.IsEncryptionAvailable);
		}

		public const string NameServerHost = "ns.exitgames.com";

		public const string NameServerHttp = "http://ns.exitgamescloud.com:80/photon/n";

		private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>
		{
			{
				ConnectionProtocol.Udp,
				5058
			},
			{
				ConnectionProtocol.Tcp,
				4533
			},
			{
				ConnectionProtocol.WebSocket,
				9093
			},
			{
				ConnectionProtocol.WebSocketSecure,
				19093
			}
		};
	}
}
