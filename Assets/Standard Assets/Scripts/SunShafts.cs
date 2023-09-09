using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Sun Shafts")]
[Serializable]
public class SunShafts : PostEffectsBase
{
	public SunShafts()
	{
		this.resolution = SunShaftsResolution.Normal;
		this.screenBlendMode = ShaftsScreenBlendMode.Screen;
		this.radialBlurIterations = 2;
		this.sunColor = Color.white;
		this.sunShaftBlurRadius = 2.5f;
		this.sunShaftIntensity = 1.15f;
		this.useSkyBoxAlpha = 0.75f;
		this.maxRadius = 0.75f;
		this.useDepthTexture = true;
	}

	public virtual void CreateMaterials()
	{
		this.sunShaftsMaterial = this.CheckShaderAndCreateMaterial(this.sunShaftsShader, this.sunShaftsMaterial);
		this.simpleClearMaterial = this.CheckShaderAndCreateMaterial(this.simpleClearShader, this.simpleClearMaterial);
	}

	public virtual void Start()
	{
		this.CreateMaterials();
		this.CheckSupport(this.useDepthTexture);
		if (this.useDepthTexture)
		{
			this.GetComponent<Camera>().depthTextureMode = (this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth);
		}
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		if (this.useDepthTexture)
		{
			this.GetComponent<Camera>().depthTextureMode = (this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth);
		}
		float num = 4f;
		if (this.resolution == SunShaftsResolution.Normal)
		{
			num = 2f;
		}
		else if (this.resolution == SunShaftsResolution.High)
		{
			num = 1f;
		}
		Vector3 vector = Vector3.one * 0.5f;
		if (this.sunTransform)
		{
			vector = this.GetComponent<Camera>().WorldToViewportPoint(this.sunTransform.position);
		}
		else
		{
			vector = new Vector3(0.5f, 0.5f, (float)0);
		}
		RenderTexture temporary = RenderTexture.GetTemporary((int)((float)source.width / num), (int)((float)source.height / num), 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary((int)((float)source.width / num), (int)((float)source.height / num), 0);
		this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(1f, 1f, (float)0, (float)0) * this.sunShaftBlurRadius);
		this.sunShaftsMaterial.SetVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, this.maxRadius));
		this.sunShaftsMaterial.SetFloat("_NoSkyBoxMask", 1f - this.useSkyBoxAlpha);
		if (!this.useDepthTexture)
		{
			RenderTexture temporary3 = RenderTexture.GetTemporary(source.width, source.height, 0);
			RenderTexture.active = temporary3;
			GL.ClearWithSkybox(false, this.GetComponent<Camera>());
			this.sunShaftsMaterial.SetTexture("_Skybox", temporary3);
			Graphics.Blit(source, temporary2, this.sunShaftsMaterial, 3);
			RenderTexture.ReleaseTemporary(temporary3);
		}
		else
		{
			Graphics.Blit(source, temporary2, this.sunShaftsMaterial, 2);
		}
		this.DrawBorder(temporary2, this.simpleClearMaterial);
		this.radialBlurIterations = this.ClampBlurIterationsToSomethingThatMakesSense(this.radialBlurIterations);
		float num2 = this.sunShaftBlurRadius * 0.00130208337f;
		this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, (float)0, (float)0));
		this.sunShaftsMaterial.SetVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, this.maxRadius));
		for (int i = 0; i < this.radialBlurIterations; i++)
		{
			Graphics.Blit(temporary2, temporary, this.sunShaftsMaterial, 1);
			num2 = this.sunShaftBlurRadius * (((float)i * 2f + 1f) * 6f) / 768f;
			this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, (float)0, (float)0));
			Graphics.Blit(temporary, temporary2, this.sunShaftsMaterial, 1);
			num2 = this.sunShaftBlurRadius * (((float)i * 2f + 2f) * 6f) / 768f;
			this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(num2, num2, (float)0, (float)0));
		}
		if (vector.z >= (float)0)
		{
			this.sunShaftsMaterial.SetVector("_SunColor", new Vector4(this.sunColor.r, this.sunColor.g, this.sunColor.b, this.sunColor.a) * this.sunShaftIntensity);
		}
		else
		{
			this.sunShaftsMaterial.SetVector("_SunColor", Vector4.zero);
		}
		this.sunShaftsMaterial.SetTexture("_ColorBuffer", temporary2);
		Graphics.Blit(source, destination, this.sunShaftsMaterial, (this.screenBlendMode != ShaftsScreenBlendMode.Screen) ? 4 : 0);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary);
	}

	private int ClampBlurIterationsToSomethingThatMakesSense(int its)
	{
		return (its >= 1) ? ((its <= 4) ? its : 4) : 1;
	}

	public override void Main()
	{
	}

	public SunShaftsResolution resolution;

	public ShaftsScreenBlendMode screenBlendMode;

	public Transform sunTransform;

	public int radialBlurIterations;

	public Color sunColor;

	public float sunShaftBlurRadius;

	public float sunShaftIntensity;

	public float useSkyBoxAlpha;

	public float maxRadius;

	public bool useDepthTexture;

	public Shader sunShaftsShader;

	private Material sunShaftsMaterial;

	public Shader simpleClearShader;

	private Material simpleClearMaterial;
}
