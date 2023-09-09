using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Antialiasing (Image based)")]
[ExecuteInEditMode]
[Serializable]
public class AntialiasingAsPostEffect : PostEffectsBase
{
	public AntialiasingAsPostEffect()
	{
		this.mode = AAMode.FXAA2;
		this.offsetScale = 0.2f;
		this.blurRadius = 18f;
	}

	public virtual void CreateMaterials()
	{
		this.materialFXAAPreset2 = this.CheckShaderAndCreateMaterial(this.shaderFXAAPreset2, this.materialFXAAPreset2);
		this.materialFXAAPreset3 = this.CheckShaderAndCreateMaterial(this.shaderFXAAPreset3, this.materialFXAAPreset3);
		this.materialFXAAII = this.CheckShaderAndCreateMaterial(this.shaderFXAAII, this.materialFXAAII);
		this.nfaa = this.CheckShaderAndCreateMaterial(this.nfaaShader, this.nfaa);
		this.ssaa = this.CheckShaderAndCreateMaterial(this.ssaaShader, this.ssaa);
		this.dlaa = this.CheckShaderAndCreateMaterial(this.dlaaShader, this.dlaa);
	}

	public virtual void Start()
	{
		this.CreateMaterials();
		this.CheckSupport(false);
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		if (this.mode < AAMode.NFAA)
		{
			Material mat;
			if (this.mode == AAMode.FXAA1PresetB)
			{
				mat = this.materialFXAAPreset3;
			}
			else if (this.mode == AAMode.FXAA1PresetA)
			{
				mat = this.materialFXAAPreset2;
			}
			else
			{
				mat = this.materialFXAAII;
			}
			if (this.mode == AAMode.FXAA1PresetA)
			{
				source.anisoLevel = 4;
			}
			Graphics.Blit(source, destination, mat);
			if (this.mode == AAMode.FXAA1PresetA)
			{
				source.anisoLevel = 0;
			}
		}
		else if (this.mode == AAMode.SSAA)
		{
			Graphics.Blit(source, destination, this.ssaa);
		}
		else if (this.mode == AAMode.DLAA)
		{
			source.anisoLevel = 0;
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit(source, temporary, this.dlaa, 0);
			Graphics.Blit(temporary, destination, this.dlaa, (!this.dlaaSharp) ? 1 : 2);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (this.mode == AAMode.NFAA)
		{
			source.anisoLevel = 0;
			this.nfaa.SetFloat("_OffsetScale", this.offsetScale);
			this.nfaa.SetFloat("_BlurRadius", this.blurRadius);
			Graphics.Blit(source, destination, this.nfaa, (!this.showGeneratedNormals) ? 0 : 1);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}

	public override void Main()
	{
	}

	public AAMode mode;

	public bool showGeneratedNormals;

	public float offsetScale;

	public float blurRadius;

	public bool dlaaSharp;

	public Shader ssaaShader;

	private Material ssaa;

	public Shader dlaaShader;

	private Material dlaa;

	public Shader nfaaShader;

	private Material nfaa;

	public Shader shaderFXAAPreset2;

	private Material materialFXAAPreset2;

	public Shader shaderFXAAPreset3;

	private Material materialFXAAPreset3;

	public Shader shaderFXAAII;

	private Material materialFXAAII;
}
