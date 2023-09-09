using System;
using UnityEngine;

[Serializable]
public class Detonator\u0020Spray\u0020Helper : MonoBehaviour
{
	public Detonator\u0020Spray\u0020Helper()
	{
		this.stopTimeMin = (float)10;
		this.stopTimeMax = (float)10;
	}

	public virtual void Start()
	{
		this.isReallyOn = this.GetComponent<ParticleEmitter>().emit;
		this.GetComponent<ParticleEmitter>().emit = false;
		this.spawnTime = Time.time;
		this.startTime = UnityEngine.Random.value * (this.startTimeMax - this.startTimeMin) + this.startTimeMin + Time.time;
		this.stopTime = UnityEngine.Random.value * (this.stopTimeMax - this.stopTimeMin) + this.stopTimeMin + Time.time;
		if (UnityEngine.Random.value > 0.5f)
		{
			this.GetComponent<Renderer>().material = this.firstMaterial;
		}
		else
		{
			this.GetComponent<Renderer>().material = this.secondMaterial;
		}
	}

	public virtual void FixedUpdate()
	{
		if (Time.time > this.startTime)
		{
			this.GetComponent<ParticleEmitter>().emit = this.isReallyOn;
		}
		if (Time.time > this.stopTime)
		{
			this.GetComponent<ParticleEmitter>().emit = false;
		}
	}

	public virtual void Main()
	{
	}

	public float startTimeMin;

	public float startTimeMax;

	public float stopTimeMin;

	public float stopTimeMax;

	public Material firstMaterial;

	public Material secondMaterial;

	private float startTime;

	private float stopTime;

	private float spawnTime;

	private bool isReallyOn;
}
