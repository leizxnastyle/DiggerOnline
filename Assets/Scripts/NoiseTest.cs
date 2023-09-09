using System;
using UnityEngine;

public class NoiseTest : MonoBehaviour
{
	private void Start()
	{
		this.m_ParticleEmitter = base.gameObject.GetComponent<ParticleEmitter>();
	}

	private void Update()
	{
		UnityEngine.Debug.Log(this.m_ParticleEmitter.particleCount);
		int num = 0;
		Particle[] particles = this.m_ParticleEmitter.particles;
		for (int i = 0; i < 1024; i++)
		{
			for (int j = 0; j < 1024; j++)
			{
				for (int k = 0; k < 128; k++)
				{
					float num2 = PerlinSimplexNoise.noise((float)i * 0.001f, (float)j * 0.001f, (float)k * 0.001f) * 64f;
					num2 += PerlinSimplexNoise.noise((float)i * 0.01f, (float)j * 0.01f, (float)k * 0.01f) * 32f;
					num2 += PerlinSimplexNoise.noise((float)i * 0.1f, (float)j * 0.1f, (float)k * 0.1f) * 4f;
					particles[num].position = new Vector3((float)(i + k), num2, (float)j);
					if (num >= particles.Length - 1)
					{
						this.UpdateDisplay(particles);
						return;
					}
					num++;
				}
			}
		}
	}

	private void UpdateDisplay(Particle[] particles)
	{
		this.m_ParticleEmitter.particles = particles;
	}

	private ParticleEmitter m_ParticleEmitter;
}
