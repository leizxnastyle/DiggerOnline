using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private void Awake()
	{
		if (SoundManager.Instance != null)
		{
			return;
		}
		SoundManager.Instance = this;
	}

	public IEnumerator LoadingSound()
	{
		SoundManager.Sound[] soundTypes = Enum.GetValues(typeof(SoundManager.Sound)) as SoundManager.Sound[];
		this._SortedSounds = new SoundManager.SoundInfo[soundTypes.Length];
		for (int i = 0; i < this._SortedSounds.Length; i++)
		{
			this._SortedSounds[i] = new SoundManager.SoundInfo
			{
				Type = soundTypes[i],
				AudioClips = new List<AudioClip>()
			};
		}
		foreach (SoundManager.SoundInfo sound in this.Sounds)
		{
			this._SortedSounds[(int)sound.Type].AudioClips.AddRange(sound.AudioClips);
		}
		WWW request = WWW.LoadFromCacheOrDownload("https://vk.diggerworld.ru/KO/Builds/Packages/FX.unity3d", 1);
		yield return request;
		if (request.error == null)
		{
			for (int j = 0; j < this._SortedSounds.Length; j++)
			{
				for (int k = -1; k < 10; k++)
				{
					string fileName = soundTypes[j].ToString();
					if (k >= 0)
					{
						fileName = fileName + "_" + (k + 1);
					}
					if (request.assetBundle.Contains(fileName))
					{
						this._SortedSounds[j].AudioClips.Add((AudioClip)request.assetBundle.LoadAsset(fileName));
					}
				}
			}
			ManagerAudio.AudioLoaded = true;
		}
		else
		{
			UnityEngine.Debug.Log("Sound loading error: " + request.error);
		}
		yield break;
	}

	public AudioClip GetClip(SoundManager.Sound sound)
	{
		if (this._SortedSounds == null)
		{
			return null;
		}
		List<AudioClip> audioClips = this._SortedSounds[(int)sound].AudioClips;
		return (audioClips.Count <= 0) ? null : audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
	}

	public void PlayAtPoint(SoundManager.Sound sound, Vector3 pos)
	{
		AudioClip clip = this.GetClip(sound);
		if (clip != null)
		{
			AudioSource.PlayClipAtPoint(clip, pos, ProfileINI.sound_volume * ProfileINI.sound_scale);
		}
	}

	public void Play(SoundManager.Sound sound, AudioSource source = null)
	{
		AudioClip clip = this.GetClip(sound);
		if (clip != null)
		{
			if (source == null)
			{
				source = base.GetComponent<AudioSource>();
			}
			source.PlayOneShot(clip, ProfileINI.sound_volume * ProfileINI.sound_scale);
		}
	}

	public static SoundManager Instance;

	public SoundManager.SoundInfo[] Sounds;

	private SoundManager.SoundInfo[] _SortedSounds;

	public enum Sound
	{
		BangDirt,
		BangGlass,
		BangGrass,
		BangMetal,
		BangSand,
		BangStone,
		BangWood,
		FootDirt,
		FootGlass,
		FootGrass,
		FootMetal,
		FootSand,
		FootStone,
		FootWood,
		HitDirt,
		HitGlass,
		HitGrass,
		HitMetal,
		HitSand,
		HitStone,
		HitWood,
		WaterIn,
		WaterOut,
		LoopForest,
		WeaponSwish,
		BangWater,
		FootSnow,
		HitSnow,
		BangSnow,
		FootIce,
		HitIce,
		BangIce,
		LoopSnow,
		Jump,
		LoopOcean,
		FootLava,
		SetDecor,
		Note2,
		LoopDesert,
		DoorOpen,
		DoorClose,
		TorchLoop,
		LoopUnderwater,
		Treasure,
		TeamDoorOpen,
		TeamDoorClose,
		MenuClick,
		ShopBuy,
		EverydayBonus,
		BattleWin,
		BattleLose,
		CtfFlagCaptured,
		CtfFlagDelivered,
		CtfFlagStolen,
		ZombieVirusInfection,
		TakeGift,
		ZombieActivity,
		ShortWhistle
	}

	[Serializable]
	public class SoundInfo
	{
		public SoundManager.Sound Type;

		public List<AudioClip> AudioClips;
	}
}
