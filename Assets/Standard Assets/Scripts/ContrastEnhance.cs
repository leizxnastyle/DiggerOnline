using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Contrast Enhance (Unsharp Mask)")]
[ExecuteInEditMode]
[Serializable]
public class ContrastEnhance : PostEffectsBase
{
	public ContrastEnhance()
	{
		this.intensity = 0.5f;
		this.blurSpread = 1f;
	}

	public virtual void CreateMaterials()
	{
		this.contrastCompositeMaterial = this.CheckShaderAndCreateMaterial(this.contrastCompositeShader, this.contrastCompositeMaterial);
		this.separableBlurMaterial = this.CheckShaderAndCreateMaterial(this.separableBlurShader, this.separableBlurMaterial);
	}

	public virtual void Start()
	{
		this.CreateMaterials();
		this.CheckSupport(false);
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		RenderTexture temporary = RenderTexture.GetTemporary((int)((float)source.width / 2f), (int)((float)source.height / 2f), 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		Graphics.Blit(source, temporary);
		Graphics.Blit(temporary, temporary2);
		this.separableBlurMaterial.SetVector("offsets", new Vector4((float)0, this.blurSpread * 1f / (float)temporary2.height, (float)0, (float)0));
		Graphics.Blit(temporary2, temporary3, this.separableBlurMaterial);
		this.separableBlurMaterial.SetVector("offsets", new Vector4(this.blurSpread * 1f / (float)temporary2.width, (float)0, (float)0, (float)0));
		Graphics.Blit(temporary3, temporary2, this.separableBlurMaterial);
		this.contrastCompositeMaterial.SetTexture("_MainTexBlurred", temporary2);
		this.contrastCompositeMaterial.SetFloat("intensity", this.intensity);
		this.contrastCompositeMaterial.SetFloat("threshhold", this.threshhold);
		Graphics.Blit(source, destination, this.contrastCompositeMaterial);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
	}

	public override void Main()
	{
	}

	public float intensity;

	public float threshhold;

	private Material separableBlurMaterial;

	private Material contrastCompositeMaterial;

	public float blurSpread;

	public Shader separableBlurShader;

	public Shader contrastCompositeShader;
}
