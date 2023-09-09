using System;
using UnityEngine;

public class VoiceChatSettings : MonoBehaviour
{
	public static VoiceChatSettings Instance
	{
		get
		{
			if (VoiceChatSettings.instance == null)
			{
				VoiceChatSettings.instance = (UnityEngine.Object.FindObjectOfType(typeof(VoiceChatSettings)) as VoiceChatSettings);
			}
			return VoiceChatSettings.instance;
		}
	}

	public int Frequency
	{
		get
		{
			return this.frequency;
		}
		private set
		{
			this.frequency = value;
		}
	}

	public bool LocalDebug
	{
		get
		{
			return this.localDebug;
		}
		set
		{
			this.localDebug = value;
		}
	}

	public VoiceChatPreset Preset
	{
		get
		{
			return this.preset;
		}
		set
		{
			this.preset = value;
			switch (this.preset)
			{
			case VoiceChatPreset.Speex_8K:
				this.Frequency = 8000;
				this.SampleSize = 320;
				this.Compression = VoiceChatCompression.Speex;
				break;
			case VoiceChatPreset.Speex_16K:
				this.Frequency = 16000;
				this.SampleSize = 640;
				this.Compression = VoiceChatCompression.Speex;
				break;
			case VoiceChatPreset.Alaw_4k:
				this.Frequency = 4096;
				this.SampleSize = 128;
				this.Compression = VoiceChatCompression.Alaw;
				break;
			case VoiceChatPreset.Alaw_8k:
				this.Frequency = 8192;
				this.SampleSize = 256;
				this.Compression = VoiceChatCompression.Alaw;
				break;
			case VoiceChatPreset.Alaw_16k:
				this.Frequency = 16384;
				this.SampleSize = 512;
				this.Compression = VoiceChatCompression.Alaw;
				break;
			case VoiceChatPreset.Alaw_Zlib_4k:
				this.Frequency = 4096;
				this.SampleSize = 128;
				this.Compression = VoiceChatCompression.AlawZlib;
				break;
			case VoiceChatPreset.Alaw_Zlib_8k:
				this.Frequency = 8192;
				this.SampleSize = 256;
				this.Compression = VoiceChatCompression.AlawZlib;
				break;
			case VoiceChatPreset.Alaw_Zlib_16k:
				this.Frequency = 16384;
				this.SampleSize = 512;
				this.Compression = VoiceChatCompression.AlawZlib;
				break;
			}
		}
	}

	public VoiceChatCompression Compression
	{
		get
		{
			return this.compression;
		}
		private set
		{
			this.compression = value;
		}
	}

	public int SampleSize
	{
		get
		{
			return this.sampleSize;
		}
		private set
		{
			this.sampleSize = value;
		}
	}

	public double SampleTime
	{
		get
		{
			return (double)this.SampleSize / (double)this.Frequency;
		}
	}

	private static VoiceChatSettings instance;

	[SerializeField]
	private int frequency = 16000;

	[SerializeField]
	private int sampleSize = 640;

	[SerializeField]
	private VoiceChatCompression compression = VoiceChatCompression.Speex;

	[SerializeField]
	private VoiceChatPreset preset = VoiceChatPreset.Speex_16K;

	[SerializeField]
	private bool localDebug;
}
