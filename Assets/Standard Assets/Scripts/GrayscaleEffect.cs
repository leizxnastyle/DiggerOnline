using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Grayscale")]
public class GrayscaleEffect : ImageEffectBase
{
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.material.SetTexture("_RampTex", this.textureRamp);
		base.material.SetFloat("_RampOffset", this.rampOffset);
		Graphics.Blit(source, destination, base.material);
	}

	public Texture textureRamp;

	public float rampOffset;
}
