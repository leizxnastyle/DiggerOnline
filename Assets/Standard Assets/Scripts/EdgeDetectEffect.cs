using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Edge Detection (Color)")]
public class EdgeDetectEffect : ImageEffectBase
{
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.material.SetFloat("_Treshold", this.threshold * this.threshold);
		Graphics.Blit(source, destination, base.material);
	}

	public float threshold = 0.2f;
}
