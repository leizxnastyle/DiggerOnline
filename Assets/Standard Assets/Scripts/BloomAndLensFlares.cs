using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Bloom and Lens Flares (3.4)")]
[Serializable]
public class BloomAndLensFlares : PostEffectsBase
{
	public BloomAndLensFlares()
	{
		this.screenBlendMode = BloomScreenBlendMode.Screen;
		this.sepBlurSpread = 1.5f;
		this.useSrcAlphaAsMask = 0.5f;
		this.bloomIntensity = 1f;
		this.bloomThreshhold = 0.5f;
		this.bloomBlurIterations = 2;
		this.hollywoodFlareBlurIterations = 2;
		this.lensflareMode = LensflareStyle34.Anamorphic;
		this.hollyStretchWidth = 3.5f;
		this.lensflareIntensity = 1f;
		this.lensflareThreshhold = 0.3f;
		this.flareColorA = new Color(0.4f, 0.4f, 0.8f, 0.75f);
		this.flareColorB = new Color(0.4f, 0.8f, 0.8f, 0.75f);
		this.flareColorC = new Color(0.8f, 0.4f, 0.8f, 0.75f);
		this.flareColorD = new Color(0.8f, 0.4f, (float)0, 0.75f);
		this.blurWidth = 1f;
	}

	public virtual void Start()
	{
		this.CreateMaterials();
		this.CheckSupport(false);
	}

	public virtual void CreateMaterials()
	{
		this.screenBlend = this.CheckShaderAndCreateMaterial(this.screenBlendShader, this.screenBlend);
		this.lensFlareMaterial = this.CheckShaderAndCreateMaterial(this.lensFlareShader, this.lensFlareMaterial);
		this.vignetteMaterial = this.CheckShaderAndCreateMaterial(this.vignetteShader, this.vignetteMaterial);
		this.separableBlurMaterial = this.CheckShaderAndCreateMaterial(this.separableBlurShader, this.separableBlurMaterial);
		this.addBrightStuffBlendOneOneMaterial = this.CheckShaderAndCreateMaterial(this.addBrightStuffOneOneShader, this.addBrightStuffBlendOneOneMaterial);
		this.hollywoodFlaresMaterial = this.CheckShaderAndCreateMaterial(this.hollywoodFlaresShader, this.hollywoodFlaresMaterial);
		this.brightPassFilterMaterial = this.CheckShaderAndCreateMaterial(this.brightPassFilterShader, this.brightPassFilterMaterial);
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		RenderTexture temporary = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		RenderTexture temporary4 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		float num = 1f * (float)source.width / (1f * (float)source.height);
		float num2 = 0.001953125f;
		Graphics.Blit(source, temporary, this.screenBlend, 2);
		Graphics.Blit(temporary, temporary2, this.screenBlend, 2);
		RenderTexture.ReleaseTemporary(temporary);
		this.BrightFilter(this.bloomThreshhold, this.useSrcAlphaAsMask, temporary2, temporary3);
		if (this.bloomBlurIterations < 1)
		{
			this.bloomBlurIterations = 1;
		}
		for (int i = 0; i < this.bloomBlurIterations; i++)
		{
			float num3 = (float)this.bloomBlurIterations * 1f * this.sepBlurSpread;
			this.separableBlurMaterial.SetVector("offsets", new Vector4((float)0, num3 * num2, (float)0, (float)0));
			Graphics.Blit((i != 0) ? temporary2 : temporary3, temporary4, this.separableBlurMaterial);
			this.separableBlurMaterial.SetVector("offsets", new Vector4(num3 / num * num2, (float)0, (float)0, (float)0));
			Graphics.Blit(temporary4, temporary2, this.separableBlurMaterial);
		}
		if (this.lensflares)
		{
			if (this.lensflareMode == LensflareStyle34.Ghosting)
			{
				this.BrightFilter(this.lensflareThreshhold, (float)0, temporary3, temporary4);
				this.separableBlurMaterial.SetVector("offsets", new Vector4((float)0, 2f / (1f * (float)temporary2.height), (float)0, (float)0));
				Graphics.Blit(temporary4, temporary3, this.separableBlurMaterial);
				this.separableBlurMaterial.SetVector("offsets", new Vector4(2f / (1f * (float)temporary2.width), (float)0, (float)0, (float)0));
				Graphics.Blit(temporary3, temporary4, this.separableBlurMaterial);
				this.Vignette(0.975f, temporary4, temporary3);
				this.BlendFlares(temporary3, temporary2);
			}
			else
			{
				this.hollywoodFlaresMaterial.SetVector("_Threshhold", new Vector4(this.lensflareThreshhold, 1f / (1f - this.lensflareThreshhold), (float)0, (float)0));
				this.hollywoodFlaresMaterial.SetVector("tintColor", new Vector4(this.flareColorA.r, this.flareColorA.g, this.flareColorA.b, this.flareColorA.a) * this.flareColorA.a * this.lensflareIntensity);
				Graphics.Blit(temporary4, temporary3, this.hollywoodFlaresMaterial, 2);
				Graphics.Blit(temporary3, temporary4, this.hollywoodFlaresMaterial, 3);
				this.hollywoodFlaresMaterial.SetVector("offsets", new Vector4(this.sepBlurSpread * 1f / num * num2, (float)0, (float)0, (float)0));
				this.hollywoodFlaresMaterial.SetFloat("stretchWidth", this.hollyStretchWidth);
				Graphics.Blit(temporary4, temporary3, this.hollywoodFlaresMaterial, 1);
				this.hollywoodFlaresMaterial.SetFloat("stretchWidth", this.hollyStretchWidth * 2f);
				Graphics.Blit(temporary3, temporary4, this.hollywoodFlaresMaterial, 1);
				this.hollywoodFlaresMaterial.SetFloat("stretchWidth", this.hollyStretchWidth * 4f);
				Graphics.Blit(temporary4, temporary3, this.hollywoodFlaresMaterial, 1);
				if (this.lensflareMode == LensflareStyle34.Anamorphic)
				{
					for (int j = 0; j < this.hollywoodFlareBlurIterations; j++)
					{
						this.separableBlurMaterial.SetVector("offsets", new Vector4(this.hollyStretchWidth * 2f / num * num2, (float)0, (float)0, (float)0));
						Graphics.Blit(temporary3, temporary4, this.separableBlurMaterial);
						this.separableBlurMaterial.SetVector("offsets", new Vector4(this.hollyStretchWidth * 2f / num * num2, (float)0, (float)0, (float)0));
						Graphics.Blit(temporary4, temporary3, this.separableBlurMaterial);
					}
					this.AddTo(1f, temporary3, temporary2);
				}
				else
				{
					for (int k = 0; k < this.hollywoodFlareBlurIterations; k++)
					{
						this.separableBlurMaterial.SetVector("offsets", new Vector4(this.hollyStretchWidth * 2f / num * num2, (float)0, (float)0, (float)0));
						Graphics.Blit(temporary3, temporary4, this.separableBlurMaterial);
						this.separableBlurMaterial.SetVector("offsets", new Vector4(this.hollyStretchWidth * 2f / num * num2, (float)0, (float)0, (float)0));
						Graphics.Blit(temporary4, temporary3, this.separableBlurMaterial);
					}
					this.Vignette(1f, temporary3, temporary4);
					this.BlendFlares(temporary4, temporary3);
					this.AddTo(1f, temporary3, temporary2);
				}
			}
		}
		this.screenBlend.SetFloat("_Intensity", this.bloomIntensity);
		this.screenBlend.SetTexture("_ColorBuffer", source);
		Graphics.Blit(temporary2, destination, this.screenBlend, (int)this.screenBlendMode);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
		RenderTexture.ReleaseTemporary(temporary4);
	}

	private void AddTo(float intensity, RenderTexture from, RenderTexture to)
	{
		this.addBrightStuffBlendOneOneMaterial.SetFloat("intensity", intensity);
		Graphics.Blit(from, to, this.addBrightStuffBlendOneOneMaterial);
	}

	private void BlendFlares(RenderTexture from, RenderTexture to)
	{
		this.lensFlareMaterial.SetVector("colorA", new Vector4(this.flareColorA.r, this.flareColorA.g, this.flareColorA.b, this.flareColorA.a) * this.lensflareIntensity);
		this.lensFlareMaterial.SetVector("colorB", new Vector4(this.flareColorB.r, this.flareColorB.g, this.flareColorB.b, this.flareColorB.a) * this.lensflareIntensity);
		this.lensFlareMaterial.SetVector("colorC", new Vector4(this.flareColorC.r, this.flareColorC.g, this.flareColorC.b, this.flareColorC.a) * this.lensflareIntensity);
		this.lensFlareMaterial.SetVector("colorD", new Vector4(this.flareColorD.r, this.flareColorD.g, this.flareColorD.b, this.flareColorD.a) * this.lensflareIntensity);
		Graphics.Blit(from, to, this.lensFlareMaterial);
	}

	private void BrightFilter(float thresh, float useAlphaAsMask, RenderTexture from, RenderTexture to)
	{
		this.brightPassFilterMaterial.SetVector("threshhold", new Vector4(thresh, 1f / (1f - thresh), (float)0, (float)0));
		this.brightPassFilterMaterial.SetFloat("useSrcAlphaAsMask", useAlphaAsMask);
		Graphics.Blit(from, to, this.brightPassFilterMaterial);
	}

	private void Vignette(float amount, RenderTexture from, RenderTexture to)
	{
		this.vignetteMaterial.SetFloat("vignetteIntensity", amount);
		Graphics.Blit(from, to, this.vignetteMaterial);
	}

	public override void Main()
	{
	}

	public TweakMode34 tweakMode;

	public BloomScreenBlendMode screenBlendMode;

	public float sepBlurSpread;

	public float useSrcAlphaAsMask;

	public float bloomIntensity;

	public float bloomThreshhold;

	public int bloomBlurIterations;

	public bool lensflares;

	public int hollywoodFlareBlurIterations;

	public LensflareStyle34 lensflareMode;

	public float hollyStretchWidth;

	public float lensflareIntensity;

	public float lensflareThreshhold;

	public Color flareColorA;

	public Color flareColorB;

	public Color flareColorC;

	public Color flareColorD;

	public float blurWidth;

	public Shader lensFlareShader;

	private Material lensFlareMaterial;

	public Shader vignetteShader;

	private Material vignetteMaterial;

	public Shader separableBlurShader;

	private Material separableBlurMaterial;

	public Shader addBrightStuffOneOneShader;

	private Material addBrightStuffBlendOneOneMaterial;

	public Shader screenBlendShader;

	private Material screenBlend;

	public Shader hollywoodFlaresShader;

	private Material hollywoodFlaresMaterial;

	public Shader brightPassFilterShader;

	private Material brightPassFilterMaterial;
}
