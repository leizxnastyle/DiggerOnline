using System;

public struct VoiceChatPacket
{
	public VoiceChatCompression Compression;

	public int Length;

	public byte[] Data;

	public int NetworkId;
}
