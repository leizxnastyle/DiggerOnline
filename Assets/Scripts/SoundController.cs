using System;
using UnityEngine;

[Serializable]
public class SoundController : MonoBehaviour
{
	public SoundController()
	{
		this._audioChannels = 10;
		this._masterVol = 0.5f;
		this._soundVol = (float)1;
		this._musicVol = (float)1;
	}

	public virtual void OnApplicationQuit()
	{
		SoundController.instance = null;
	}

	public virtual void Start()
	{
		if (SoundController.instance)
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}
		else
		{
			SoundController.instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
		}
		this.AddChannels();
		UnityEngine.Object.DontDestroyOnLoad(this.transform.gameObject);
	}

	public virtual void StopMusic(bool fade)
	{
		this.PlayMusic(null, (float)0, (float)1, fade);
	}

	public virtual void FadeUpMusic()
	{
		if (this._musicChannels[this._musicChannel].volume < this._fadeTo)
		{
			this._musicChannels[this._musicChannel].volume = this._musicChannels[this._musicChannel].volume + 0.0025f;
		}
		else
		{
			this.CancelInvoke("FadeUpMusic");
		}
	}

	public virtual void FadeDownMusic()
	{
		int num = 0;
		if (this._musicChannel == 0)
		{
			num = 1;
		}
		if (this._musicChannels[num].volume > (float)0)
		{
			this._musicChannels[num].volume = this._musicChannels[num].volume - 0.0025f;
		}
		else
		{
			this._musicChannels[num].Stop();
			this.CancelInvoke("FadeDownMusic");
		}
	}

	public virtual void UpdateMusicVolume()
	{
		for (int i = 0; i < 2; i++)
		{
			this._musicChannels[i].volume = this._currentMusicVol * this._masterVol * this._musicVol;
		}
	}

	public virtual void AddChannels()
	{
		this.channels = new AudioSource[this._audioChannels];
		this._musicChannels = new AudioSource[2];
		if (this.channels.Length <= this._audioChannels)
		{
			for (int i = 0; i < this._audioChannels; i++)
			{
				GameObject gameObject = new GameObject();
				gameObject.AddComponent(typeof(AudioSource));
				gameObject.name = "AudioChannel " + i;
				gameObject.transform.parent = this.transform;
				this.channels[i] = (AudioSource)gameObject.GetComponent(typeof(AudioSource));
				if (this._linearRollOff)
				{
					this.channels[i].rolloffMode = AudioRolloffMode.Linear;
				}
			}
		}
		for (int j = 0; j < 2; j++)
		{
			GameObject gameObject2 = new GameObject();
			gameObject2.AddComponent(typeof(AudioSource));
			gameObject2.name = "MusicChannel " + j;
			gameObject2.transform.parent = this.transform;
			this._musicChannels[j] = (AudioSource)gameObject2.GetComponent(typeof(AudioSource));
			this._musicChannels[j].loop = true;
			this._musicChannels[j].volume = (float)0;
			if (this._linearRollOff)
			{
				this._musicChannels[j].rolloffMode = AudioRolloffMode.Linear;
			}
		}
	}

	public virtual void PlayMusic(AudioClip clip, float volume, float pitch, bool fade)
	{
		if (!fade)
		{
			this._musicChannels[this._musicChannel].volume = (float)0;
		}
		if (this._musicChannel == 0)
		{
			this._musicChannel = 1;
		}
		else
		{
			this._musicChannel = 0;
		}
		this._currentMusicVol = volume;
		this._musicChannels[this._musicChannel].clip = clip;
		if (fade)
		{
			this._fadeTo = volume * this._masterVol * this._musicVol;
			this.InvokeRepeating("FadeUpMusic", 0.01f, 0.01f);
			this.InvokeRepeating("FadeDownMusic", 0.01f, 0.01f);
		}
		else
		{
			this._musicChannels[this._musicChannel].volume = volume * this._masterVol * this._musicVol;
		}
		this._musicChannels[this._musicChannel].GetComponent<AudioSource>().pitch = pitch;
		this._musicChannels[this._musicChannel].GetComponent<AudioSource>().Play();
	}

	public virtual void Play(int audioClipIndex, float volume, float pitch)
	{
		if (this.channel < this.channels.Length - 1)
		{
			this.channel++;
		}
		else
		{
			this.channel = 0;
		}
		if (audioClipIndex < this._audioClips.Length)
		{
			this.channels[this.channel].clip = this._audioClips[audioClipIndex];
			this.channels[this.channel].GetComponent<AudioSource>().volume = volume * this._masterVol * this._soundVol;
			this.channels[this.channel].GetComponent<AudioSource>().pitch = pitch;
			this.channels[this.channel].GetComponent<AudioSource>().Play();
		}
	}

	public virtual void Play(AudioClip clip, float volume, float pitch, Vector3 position)
	{
		if (this.channel < this.channels.Length - 1)
		{
			this.channel++;
		}
		else
		{
			this.channel = 0;
		}
		this.channels[this.channel].clip = clip;
		this.channels[this.channel].GetComponent<AudioSource>().volume = volume * this._masterVol * this._soundVol;
		this.channels[this.channel].GetComponent<AudioSource>().pitch = pitch;
		this.channels[this.channel].transform.position = position;
		this.channels[this.channel].GetComponent<AudioSource>().Play();
	}

	public virtual void StopAll()
	{
		for (int i = 0; i < this.channels.Length; i++)
		{
			this.channels[i].Stop();
		}
	}

	public virtual void Main()
	{
	}

	public AudioClip[] _audioClips;

	public int _audioChannels;

	public float _masterVol;

	public float _soundVol;

	public float _musicVol;

	public bool _linearRollOff;

	public AudioSource[] channels;

	public int channel;

	public AudioSource[] _musicChannels;

	public int _musicChannel;

	private float _currentMusicVol;

	private float _fadeTo;

	[NonSerialized]
	public static SoundController instance;
}
