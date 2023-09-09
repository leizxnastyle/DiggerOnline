using System;
using System.Collections.Generic;
using UnityEngine;

public class VoiceChatNetworkProxy : MonoBehaviour
{
	private void Start()
	{
		if (base.GetComponent<NetworkView>().isMine)
		{
			VoiceChatRecorder.Instance.NewSample += this.OnNewSample;
		}
		if (Network.isServer)
		{
			this.assignedNetworkId = ++VoiceChatNetworkProxy.networkIdCounter;
			base.GetComponent<NetworkView>().RPC("SetNetworkId", base.GetComponent<NetworkView>().owner, new object[]
			{
				this.assignedNetworkId
			});
		}
		if (Network.isClient && !base.GetComponent<NetworkView>().isMine)
		{
			base.gameObject.AddComponent<AudioSource>();
			this.player = base.gameObject.AddComponent<VoiceChatPlayer>();
		}
	}

	private void OnNewSample(VoiceChatPacket packet)
	{
		this.packets.Enqueue(packet);
	}

	[PunRPC]
	private void SetNetworkId(int networkId)
	{
		VoiceChatRecorder.Instance.NetworkId = networkId;
	}

	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		int count = this.packets.Count;
		if (stream.isWriting)
		{
			stream.Serialize(ref count);
			while (this.packets.Count > 0)
			{
				VoiceChatPacket packet = this.packets.Dequeue();
				stream.WritePacket(packet);
				if (packet.Data.Length == VoiceChatSettings.Instance.SampleSize)
				{
					VoiceChatBytePool.Instance.Return(packet.Data);
				}
			}
		}
		else if (Network.isServer)
		{
			stream.Serialize(ref count);
			for (int i = 0; i < count; i++)
			{
				this.packets.Enqueue(stream.ReadPacket());
				if (Network.connections.Length < 2)
				{
					this.packets.Dequeue();
				}
			}
		}
		else
		{
			stream.Serialize(ref count);
			for (int j = 0; j < count; j++)
			{
				VoiceChatPacket packet2 = stream.ReadPacket();
				if (this.player != null)
				{
					this.player.OnNewSample(packet2);
				}
			}
		}
	}

	private static int networkIdCounter;

	private int assignedNetworkId = -1;

	private VoiceChatPlayer player;

	private Queue<VoiceChatPacket> packets = new Queue<VoiceChatPacket>(16);
}
