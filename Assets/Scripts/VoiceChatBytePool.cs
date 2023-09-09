using System;

public class VoiceChatBytePool : VoiceChatPool<byte[]>
{
	private VoiceChatBytePool()
	{
	}

	protected override byte[] Create()
	{
		return new byte[VoiceChatSettings.Instance.SampleSize];
	}

	public static readonly VoiceChatBytePool Instance = new VoiceChatBytePool();
}
