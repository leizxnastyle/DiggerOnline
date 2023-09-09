using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class VoiceChatExtensions
{
	public static void WritePacket(this BitStream stream, VoiceChatPacket packet)
	{
		VoiceChatExtensions.PackShort packShort = default(VoiceChatExtensions.PackShort);
		short compression = (short)packet.Compression;
		stream.Serialize(ref packet.Length);
		stream.Serialize(ref compression);
		for (int i = 0; i < packet.Length; i += 2)
		{
			packShort.Byte0 = packet.Data[i];
			if (i + 1 < packet.Length)
			{
				packShort.Byte1 = packet.Data[i + 1];
			}
			stream.Serialize(ref packShort.Short);
		}
	}

	public static VoiceChatPacket ReadPacket(this BitStream stream)
	{
		short num = 0;
		VoiceChatExtensions.PackShort packShort = default(VoiceChatExtensions.PackShort);
		VoiceChatPacket result = default(VoiceChatPacket);
		stream.Serialize(ref result.Length);
		stream.Serialize(ref num);
		result.Compression = (VoiceChatCompression)num;
		result.Data = VoiceChatBytePool.Instance.Get();
		for (int i = 0; i < result.Length; i += 2)
		{
			stream.Serialize(ref packShort.Short);
			result.Data[i] = packShort.Byte0;
			if (i + 1 < result.Length)
			{
				result.Data[i + 1] = packShort.Byte1;
			}
		}
		return result;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct PackShort
	{
		[FieldOffset(0)]
		public short Short;

		[FieldOffset(0)]
		public byte Byte0;

		[FieldOffset(1)]
		public byte Byte1;
	}
}
