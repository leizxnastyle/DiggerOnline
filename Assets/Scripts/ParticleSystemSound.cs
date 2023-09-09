using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class ParticleSystemSound : MonoBehaviour
{
	public ParticleSystemSound()
	{
		this._shootPitchMax = 1.25f;
		this._shootPitchMin = 0.75f;
		this._shootVolumeMax = 0.75f;
		this._shootVolumeMin = 0.25f;
		this._explosionPitchMax = 1.25f;
		this._explosionPitchMin = 0.75f;
		this._explosionVolumeMax = 0.75f;
		this._explosionVolumeMin = 0.25f;
		this._crackleDelay = 0.25f;
		this._crackleMultiplier = 3;
		this._cracklePitchMax = 1.25f;
		this._cracklePitchMin = 0.75f;
		this._crackleVolumeMax = 0.75f;
		this._crackleVolumeMin = 0.25f;
	}

	public virtual void LateUpdate()
	{
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[this.GetComponent<ParticleSystem>().particleCount];
		int particles = this.GetComponent<ParticleSystem>().GetParticles(array);
		for (int i = 0; i < particles; i++)
		{
			if (this._explosionSound.Length > 0 && array[i].lifetime < Time.deltaTime)
			{
				SoundController.instance.Play(this._explosionSound[UnityEngine.Random.Range(0, this._explosionSound.Length)], UnityEngine.Random.Range(this._explosionVolumeMax, this._explosionVolumeMin), UnityEngine.Random.Range(this._explosionPitchMin, this._explosionPitchMax), array[i].position);
				if (this._crackleSound.Length > 0)
				{
					for (int j = 0; j < this._crackleMultiplier; j++)
					{
						this.StartCoroutine(this.Crackle(array[i].position, this._crackleDelay + (float)j * 0.1f));
					}
				}
			}
			if (this._shootSound.Length > 0 && array[i].lifetime >= array[i].startLifetime - Time.deltaTime)
			{
				SoundController.instance.Play(this._shootSound[UnityEngine.Random.Range(0, this._shootSound.Length)], UnityEngine.Random.Range(this._shootVolumeMax, this._shootVolumeMin), UnityEngine.Random.Range(this._shootPitchMin, this._shootPitchMax), array[i].position);
			}
		}
	}

	public virtual IEnumerator Crackle(Vector3 pos, float delay)
	{
		return new ParticleSystemSound._0024Crackle_002412(pos, delay, this).GetEnumerator();
	}

	public virtual void Main()
	{
	}

	public AudioClip[] _shootSound;

	public float _shootPitchMax;

	public float _shootPitchMin;

	public float _shootVolumeMax;

	public float _shootVolumeMin;

	public AudioClip[] _explosionSound;

	public float _explosionPitchMax;

	public float _explosionPitchMin;

	public float _explosionVolumeMax;

	public float _explosionVolumeMin;

	public AudioClip[] _crackleSound;

	public float _crackleDelay;

	public int _crackleMultiplier;

	public float _cracklePitchMax;

	public float _cracklePitchMin;

	public float _crackleVolumeMax;

	public float _crackleVolumeMin;

	[CompilerGenerated]
	[Serializable]
	internal sealed class _0024Crackle_002412 : GenericGenerator<WaitForSeconds>
	{
		public _0024Crackle_002412(Vector3 pos, float delay, ParticleSystemSound self_)
		{
			this._0024pos_002416 = pos;
			this._0024delay_002417 = delay;
			this._0024self__002418 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new ParticleSystemSound._0024Crackle_002412._0024(this._0024pos_002416, this._0024delay_002417, this._0024self__002418);
		}

		internal Vector3 _0024pos_002416;

		internal float _0024delay_002417;

		internal ParticleSystemSound _0024self__002418;
	}
}
