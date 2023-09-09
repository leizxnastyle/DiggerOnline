using System;

public class VoiceChatFloatPool : VoiceChatPool<float[]>
{
	private VoiceChatFloatPool()
	{
	}

	protected override float[] Create()
	{
		return new float[VoiceChatSettings.Instance.SampleSize];
	}

	public static readonly VoiceChatFloatPool Instance = new VoiceChatFloatPool();
}
