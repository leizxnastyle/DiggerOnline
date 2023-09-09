using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Fisheye")]
[ExecuteInEditMode]
[Serializable]
public class Fisheye : PostEffectsBase
{
	public Fisheye()
	{
		this.strengthX = 0.05f;
		this.strengthY = 0.05f;
	}

	public virtual void CreateMaterials()
	{
		this.fisheyeMaterial = this.CheckShaderAndCreateMaterial(this.fishEyeShader, this.fisheyeMaterial);
	}

	public virtual void Start()
	{
		this.CreateMaterials();
		this.CheckSupport(false);
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		float num = 0.15625f;
		float num2 = (float)source.width * 1f / ((float)source.height * 1f);
		this.fisheyeMaterial.SetVector("intensity", new Vector4(this.strengthX * num2 * num, this.strengthY * num, this.strengthX * num2 * num, this.strengthY * num));
		Graphics.Blit(source, destination, this.fisheyeMaterial);
	}

	public override void Main()
	{
	}

	public float strengthX;

	public float strengthY;

	public Shader fishEyeShader;

	private Material fisheyeMaterial;
}
