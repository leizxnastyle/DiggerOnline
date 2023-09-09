using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Crease")]
[Serializable]
public class Crease : PostEffectsBase
{
	public Crease()
	{
		this.intensity = 0.5f;
		this.softness = 1;
		this.spread = 1f;
	}

	public virtual void CreateMaterials()
	{
		this.blurMaterial = this.CheckShaderAndCreateMaterial(this.blurShader, this.blurMaterial);
		this.depthFetchMaterial = this.CheckShaderAndCreateMaterial(this.depthFetchShader, this.depthFetchMaterial);
		this.creaseApplyMaterial = this.CheckShaderAndCreateMaterial(this.creaseApplyShader, this.creaseApplyMaterial);
	}

	public virtual void Start()
	{
		this.CreateMaterials();
		this.CheckSupport(true);
	}

	public virtual void OnEnable()
	{
		this.GetComponent<Camera>().depthTextureMode = (this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth);
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		float num = 1f * (float)source.width / (1f * (float)source.height);
		float num2 = 0.001953125f;
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
		Graphics.Blit(source, temporary, this.depthFetchMaterial);
		Graphics.Blit(temporary, temporary2);
		for (int i = 0; i < this.softness; i++)
		{
			this.blurMaterial.SetVector("offsets", new Vector4((float)0, this.spread * num2, (float)0, (float)0));
			Graphics.Blit(temporary2, temporary3, this.blurMaterial);
			this.blurMaterial.SetVector("offsets", new Vector4(this.spread * num2 / num, (float)0, (float)0, (float)0));
			Graphics.Blit(temporary3, temporary2, this.blurMaterial);
		}
		this.creaseApplyMaterial.SetTexture("_HrDepthTex", temporary);
		this.creaseApplyMaterial.SetTexture("_LrDepthTex", temporary2);
		this.creaseApplyMaterial.SetFloat("intensity", this.intensity);
		Graphics.Blit(source, destination, this.creaseApplyMaterial);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
	}

	public override void Main()
	{
	}

	public float intensity;

	public int softness;

	public float spread;

	public Shader blurShader;

	private Material blurMaterial;

	public Shader depthFetchShader;

	private Material depthFetchMaterial;

	public Shader creaseApplyShader;

	private Material creaseApplyMaterial;
}
