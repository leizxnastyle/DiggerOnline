using System;
using System.Linq;
using Exocortex.DSP;
using UnityEngine;

public class VoiceChatRecorder : MonoBehaviour
{
	public event Action<VoiceChatPacket> NewSample;

	public static VoiceChatRecorder Instance
	{
		get
		{
			if (VoiceChatRecorder.instance == null)
			{
				VoiceChatRecorder.instance = (UnityEngine.Object.FindObjectOfType(typeof(VoiceChatRecorder)) as VoiceChatRecorder);
			}
			return VoiceChatRecorder.instance;
		}
	}

	public KeyCode PushToTalkKey
	{
		get
		{
			return this.pushToTalkKey;
		}
		set
		{
			this.pushToTalkKey = value;
		}
	}

	public KeyCode ToggleToTalkKey
	{
		get
		{
			return this.toggleToTalkKey;
		}
		set
		{
			this.toggleToTalkKey = value;
		}
	}

	public bool AutoDetectSpeech
	{
		get
		{
			return this.autoDetectSpeaking;
		}
		set
		{
			this.autoDetectSpeaking = value;
		}
	}

	public int NetworkId { get; set; }

	public string Device
	{
		get
		{
			return this.device;
		}
		set
		{
			if (value != null && !Microphone.devices.Contains(value))
			{
				UnityEngine.Debug.LogError(value + " is not a valid microphone device");
				return;
			}
			this.device = value;
		}
	}

	public bool HasDefaultDevice
	{
		get
		{
			return this.device == null;
		}
	}

	public bool HasSpecificDevice
	{
		get
		{
			return this.device != null;
		}
	}

	public bool IsTransmitting
	{
		get
		{
			return this.transmitToggled || this.forceTransmit > 0f || UnityEngine.Input.GetKey(this.pushToTalkKey);
		}
	}

	public bool IsRecording
	{
		get
		{
			return this.recording;
		}
	}

	public string[] AvailableDevices
	{
		get
		{
			return Microphone.devices;
		}
	}

	private void Start()
	{
		if (VoiceChatRecorder.instance != null && VoiceChatRecorder.instance != this)
		{
			UnityEngine.Object.Destroy(this);
			UnityEngine.Debug.LogError("Only one instance of VoiceChatRecorder can exist");
			return;
		}
		this.NetworkId = -1;
		VoiceChatRecorder.instance = this;
	}

	private void OnEnable()
	{
		if (VoiceChatRecorder.instance != null && VoiceChatRecorder.instance != this)
		{
			UnityEngine.Object.Destroy(this);
			UnityEngine.Debug.LogError("Only one instance of VoiceChatRecorder can exist");
			return;
		}
		VoiceChatRecorder.instance = this;
	}

	private void OnDisable()
	{
		VoiceChatRecorder.instance = null;
	}

	private void OnDestroy()
	{
		VoiceChatRecorder.instance = null;
	}

	private void Update()
	{
		this.pushToTalkKey = ((!Chat.IsEnabled()) ? KeyCode.P : KeyCode.None);
		if (!this.recording)
		{
			return;
		}
		this.forceTransmit -= Time.deltaTime;
		if (UnityEngine.Input.GetKeyUp(this.toggleToTalkKey))
		{
			this.transmitToggled = !this.transmitToggled;
		}
		bool transmit = this.transmitToggled || UnityEngine.Input.GetKey(this.pushToTalkKey);
		int position = Microphone.GetPosition(this.Device);
		if (position < this.previousPosition)
		{
			while (this.sampleIndex < this.recordFrequency)
			{
				this.ReadSample(transmit);
			}
			this.sampleIndex = 0;
		}
		this.previousPosition = position;
		while (this.sampleIndex + this.recordSampleSize <= position)
		{
			this.ReadSample(transmit);
		}
	}

	private void Resample(float[] src, float[] dst)
	{
		if (src.Length == dst.Length)
		{
			Array.Copy(src, 0, dst, 0, src.Length);
		}
		else
		{
			float num = 1f / (float)dst.Length;
			for (int i = 0; i < dst.Length; i++)
			{
				float num2 = num * (float)i * (float)src.Length;
				dst[i] = src[(int)num2];
			}
		}
	}

	private void ReadSample(bool transmit)
	{
		this.clip.GetData(this.sampleBuffer, this.sampleIndex);
		float[] array = VoiceChatFloatPool.Instance.Get();
		this.Resample(this.sampleBuffer, array);
		this.sampleIndex += this.recordSampleSize;
		float num = float.MinValue;
		int num2 = -1;
		if (this.autoDetectSpeaking && !transmit)
		{
			for (int i = 0; i < this.fftBuffer.Length; i++)
			{
				this.fftBuffer[i] = 0f;
			}
			Array.Copy(array, 0, this.fftBuffer, 0, array.Length);
			Fourier.FFT(this.fftBuffer, this.fftBuffer.Length / 2, FourierDirection.Forward);
			for (int j = 0; j < this.fftBuffer.Length; j++)
			{
				if (this.fftBuffer[j] > num)
				{
					num = this.fftBuffer[j];
					num2 = j;
				}
			}
		}
		if (this.NewSample != null && (transmit || this.forceTransmit > 0f || num2 >= this.autoDetectIndex))
		{
			if (num2 >= this.autoDetectIndex)
			{
				if (this.forceTransmit <= 0f)
				{
					while (this.previousSampleBuffer.Count > 0)
					{
						this.TransmitBuffer(this.previousSampleBuffer.Remove());
					}
				}
				this.forceTransmit = this.forceTransmitTime;
			}
			this.TransmitBuffer(array);
		}
		else
		{
			if (this.previousSampleBuffer.Count == this.previousSampleBuffer.Capacity)
			{
				VoiceChatFloatPool.Instance.Return(this.previousSampleBuffer.Remove());
			}
			this.previousSampleBuffer.Add(array);
		}
	}

	private void TransmitBuffer(float[] buffer)
	{
		VoiceChatPacket obj = VoiceChatUtils.Compress(buffer);
		obj.NetworkId = this.NetworkId;
		this.NewSample(obj);
	}

	public bool StartRecording()
	{
		if (this.NetworkId == -1 && !VoiceChatSettings.Instance.LocalDebug)
		{
			UnityEngine.Debug.LogError("NetworkId is -1");
			return false;
		}
		if (this.recording)
		{
			UnityEngine.Debug.LogError("Already recording");
			return false;
		}
		this.targetFrequency = VoiceChatSettings.Instance.Frequency;
		this.targetSampleSize = VoiceChatSettings.Instance.SampleSize;
		int num;
		int num2;
		Microphone.GetDeviceCaps(this.Device, out num, out num2);
		this.recordFrequency = ((num != 0 || num2 != 0) ? num2 : 44100);
		this.recordSampleSize = this.recordFrequency / (this.targetFrequency / this.targetSampleSize);
		this.clip = Microphone.Start(this.Device, true, 1, this.recordFrequency);
		this.sampleBuffer = new float[this.recordSampleSize];
		this.fftBuffer = new float[VoiceChatUtils.ClosestPowerOfTwo(this.targetSampleSize)];
		this.recording = true;
		return this.recording;
	}

	public void StopRecording()
	{
		this.clip = null;
		this.recording = false;
	}

	private static VoiceChatRecorder instance;

	[SerializeField]
	private KeyCode toggleToTalkKey = KeyCode.O;

	[SerializeField]
	private KeyCode pushToTalkKey = KeyCode.P;

	[SerializeField]
	private bool autoDetectSpeaking;

	[SerializeField]
	private int autoDetectIndex = 4;

	[SerializeField]
	private float forceTransmitTime = 2f;

	private int previousPosition;

	private int sampleIndex;

	private string device;

	private AudioClip clip;

	private bool transmitToggled;

	private bool recording;

	private float forceTransmit;

	private int recordFrequency;

	private int recordSampleSize;

	private int targetFrequency;

	private int targetSampleSize;

	private float[] fftBuffer;

	private float[] sampleBuffer;

	private VoiceChatCircularBuffer<float[]> previousSampleBuffer = new VoiceChatCircularBuffer<float[]>(5);
}
