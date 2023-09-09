using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Color Correction (Ramp)")]
[ExecuteInEditMode]
public class ColorCorrectionEffect : ImageEffectBase
{
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.material.SetTexture("_RampTex", this.textureRamp);
		Graphics.Blit(source, destination, base.material);
	}

	public Texture textureRamp;
}
