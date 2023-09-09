using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Vignette and Chromatic Aberration")]
[Serializable]
public class Vignetting : PostEffectsBase
{
	public Vignetting()
	{
		this.intensity = 0.375f;
		this.chromaticAberration = 0.2f;
		this.blur = 0.1f;
		this.blurSpread = 1.5f;
	}

	public virtual void Start()
	{
		this.CreateMaterials();
		this.CheckSupport(false);
	}

	public virtual void CreateMaterials()
	{
		this.vignetteMaterial = this.CheckShaderAndCreateMaterial(this.vignetteShader, this.vignetteMaterial);
		this.separableBlurMaterial = this.CheckShaderAndCreateMaterial(this.separableBlurShader, this.separableBlurMaterial);
		this.chromAberrationMaterial = this.CheckShaderAndCreateMaterial(this.chromAberrationShader, this.chromAberrationMaterial);
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		float num = 1f * (float)source.width / (1f * (float)source.height);
		float num2 = 0.001953125f;
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)source.width / 2f), (int)((float)source.height / 2f), 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		RenderTexture temporary4 = RenderTexture.GetTemporary((int)((float)source.width / 4f), (int)((float)source.height / 4f), 0);
		Graphics.Blit(source, temporary2, this.chromAberrationMaterial, 0);
		Graphics.Blit(temporary2, temporary3);
		for (int i = 0; i < 2; i++)
		{
			this.separableBlurMaterial.SetVector("offsets", new Vector4((float)0, this.blurSpread * num2, (float)0, (float)0));
			Graphics.Blit(temporary3, temporary4, this.separableBlurMaterial);
			this.separableBlurMaterial.SetVector("offsets", new Vector4(this.blurSpread * num2 / num, (float)0, (float)0, (float)0));
			Graphics.Blit(temporary4, temporary3, this.separableBlurMaterial);
		}
		this.vignetteMaterial.SetFloat("_Intensity", this.intensity);
		this.vignetteMaterial.SetFloat("_Blur", this.blur);
		this.vignetteMaterial.SetTexture("_VignetteTex", temporary3);
		Graphics.Blit(source, temporary, this.vignetteMaterial);
		this.chromAberrationMaterial.SetFloat("_ChromaticAberration", this.chromaticAberration);
		Graphics.Blit(temporary, destination, this.chromAberrationMaterial, 1);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
		RenderTexture.ReleaseTemporary(temporary4);
	}

	public override void Main()
	{
	}

	public float intensity;

	public float chromaticAberration;

	public float blur;

	public float blurSpread;

	public Shader vignetteShader;

	private Material vignetteMaterial;

	public Shader separableBlurShader;

	private Material separableBlurMaterial;

	public Shader chromAberrationShader;

	private Material chromAberrationMaterial;
}
