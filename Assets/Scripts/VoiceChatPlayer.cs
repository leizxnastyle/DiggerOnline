using System;
using NSpeex;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VoiceChatPlayer : MonoBehaviour
{
	public float LastRecvTime
	{
		get
		{
			return this.lastRecvTime;
		}
	}

	private void Start()
	{
		int num = VoiceChatSettings.Instance.Frequency * 10;
		base.GetComponent<AudioSource>().loop = true;
		base.GetComponent<AudioSource>().spatialBlend = 0f;
		base.GetComponent<AudioSource>().clip = AudioClip.Create("VoiceChat", num, 1, VoiceChatSettings.Instance.Frequency, false);
		this.data = new float[num];
		if (VoiceChatSettings.Instance.LocalDebug)
		{
			VoiceChatRecorder.Instance.NewSample += this.OnNewSample;
		}
	}

	private void Update()
	{
		if (base.GetComponent<AudioSource>().isPlaying)
		{
			if (this.lastTime > base.GetComponent<AudioSource>().time)
			{
				this.played += (double)base.GetComponent<AudioSource>().clip.length;
			}
			this.lastTime = base.GetComponent<AudioSource>().time;
			if (this.played + (double)base.GetComponent<AudioSource>().time >= this.received)
			{
				this.Stop();
				this.shouldPlay = false;
			}
		}
		else if (this.shouldPlay)
		{
			this.playDelay -= Time.deltaTime;
			if (this.playDelay <= 0f)
			{
				base.GetComponent<AudioSource>().Play();
			}
		}
	}

	private void Stop()
	{
		base.GetComponent<AudioSource>().Stop();
		base.GetComponent<AudioSource>().time = 0f;
		this.index = 0;
		this.played = 0.0;
		this.received = 0.0;
		this.lastTime = 0f;
	}

	public void OnNewSample(VoiceChatPacket packet)
	{
		this.lastRecvTime = Time.time;
		float[] array = null;
		int num = VoiceChatUtils.Decompress(this.speexDec, packet, out array);
		this.received += VoiceChatSettings.Instance.SampleTime;
		Array.Copy(array, 0, this.data, this.index, num);
		this.index += num;
		if (this.index >= base.GetComponent<AudioSource>().clip.samples)
		{
			this.index = 0;
		}
		base.GetComponent<AudioSource>().clip.SetData(this.data, 0);
		if (!base.GetComponent<AudioSource>().isPlaying)
		{
			this.shouldPlay = true;
			if (this.playDelay <= 0f)
			{
				this.playDelay = (float)VoiceChatSettings.Instance.SampleTime * (float)this.playbackDelay;
			}
		}
		VoiceChatFloatPool.Instance.Return(array);
	}

	private float lastTime;

	private double played;

	private double received;

	private int index;

	private float[] data;

	private float playDelay;

	private bool shouldPlay;

	private float lastRecvTime;

	private SpeexDecoder speexDec = new SpeexDecoder(BandMode.Narrow, true);

	[SerializeField]
	private int playbackDelay = 2;
}
