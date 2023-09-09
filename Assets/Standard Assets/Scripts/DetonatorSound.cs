using System;
using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Sound")]
public class DetonatorSound : DetonatorComponent
{
	public override void Init()
	{
		this._soundComponent = base.gameObject.AddComponent<AudioSource>();
	}

	private void Update()
	{
		this._soundComponent.pitch = Time.timeScale;
		if (this._delayedExplosionStarted)
		{
			this._explodeDelay -= Time.deltaTime;
			if (this._explodeDelay <= 0f)
			{
				this.Explode();
			}
		}
	}

	public override void Explode()
	{
		if (this.detailThreshold > this.detail)
		{
			return;
		}
		if (!this._delayedExplosionStarted)
		{
			this._explodeDelay = this.explodeDelayMin + UnityEngine.Random.value * (this.explodeDelayMax - this.explodeDelayMin);
		}
		if (this._explodeDelay <= 0f)
		{
			if (Vector3.Distance(Camera.main.transform.position, base.transform.position) < this.distanceThreshold)
			{
				this._idx = (int)(UnityEngine.Random.value * (float)this.nearSounds.Length);
				this._soundComponent.PlayOneShot(this.nearSounds[this._idx]);
			}
			else
			{
				this._idx = (int)(UnityEngine.Random.value * (float)this.farSounds.Length);
				this._soundComponent.PlayOneShot(this.farSounds[this._idx]);
			}
			this._delayedExplosionStarted = false;
			this._explodeDelay = 0f;
		}
		else
		{
			this._delayedExplosionStarted = true;
		}
	}

	public void Reset()
	{
	}

	public AudioClip[] nearSounds;

	public AudioClip[] farSounds;

	public float distanceThreshold = 50f;

	public float minVolume = 0.4f;

	public float maxVolume = 1f;

	public float rolloffFactor = 0.5f;

	private AudioSource _soundComponent;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	private int _idx;
}
