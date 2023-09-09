using System;

public class VoiceChatShortPool : VoiceChatPool<short[]>
{
	private VoiceChatShortPool()
	{
	}

	protected override short[] Create()
	{
		return new short[VoiceChatSettings.Instance.SampleSize];
	}

	public static readonly VoiceChatShortPool Instance = new VoiceChatShortPool();
}
