using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Tilt shift")]
[ExecuteInEditMode]
[Serializable]
public class TiltShift : PostEffectsBase
{
	public TiltShift()
	{
		this.renderTextureDivider = 2;
		this.blurIterations = 2;
		this.enableForegroundBlur = true;
		this.foregroundBlurIterations = 2;
		this.maxBlurSpread = 1.5f;
		this.focalPoint = 30f;
		this.smoothness = 1.65f;
		this.distance01 = 0.2f;
		this.end01 = 1f;
		this.curve = 1f;
	}

	public virtual void CreateMaterials()
	{
		this.tiltShiftMaterial = this.CheckShaderAndCreateMaterial(this.tiltShiftShader, this.tiltShiftMaterial);
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
		this.renderTextureDivider = ((this.renderTextureDivider >= 1) ? this.renderTextureDivider : 1);
		this.renderTextureDivider = ((this.renderTextureDivider <= 4) ? this.renderTextureDivider : 4);
		this.blurIterations = ((this.blurIterations >= 1) ? this.blurIterations : 0);
		this.blurIterations = ((this.blurIterations <= 4) ? this.blurIterations : 4);
		float num3 = this.GetComponent<Camera>().WorldToViewportPoint(this.focalPoint * this.GetComponent<Camera>().transform.forward + this.GetComponent<Camera>().transform.position).z / this.GetComponent<Camera>().farClipPlane;
		this.distance01 = num3;
		this.start01 = (float)0;
		this.end01 = 1f;
		this.start01 = Mathf.Min(num3 - Mathf.Epsilon, this.start01);
		this.end01 = Mathf.Max(num3 + Mathf.Epsilon, this.end01);
		this.curve = this.smoothness * this.distance01;
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, 0);
		RenderTexture temporary3 = RenderTexture.GetTemporary(source.width / this.renderTextureDivider, source.height / this.renderTextureDivider, 0);
		RenderTexture temporary4 = RenderTexture.GetTemporary(source.width / this.renderTextureDivider, source.height / this.renderTextureDivider, 0);
		this.tiltShiftMaterial.SetVector("_SimpleDofParams", new Vector4(this.start01, this.distance01, this.end01, this.curve));
		this.tiltShiftMaterial.SetTexture("_Coc", temporary);
		if (this.enableForegroundBlur)
		{
			Graphics.Blit(source, temporary, this.tiltShiftMaterial, 0);
			Graphics.Blit(temporary, temporary3);
			for (int i = 0; i < this.foregroundBlurIterations; i++)
			{
				this.tiltShiftMaterial.SetVector("offsets", new Vector4((float)0, this.maxBlurSpread * 0.75f * num2, (float)0, (float)0));
				Graphics.Blit(temporary3, temporary4, this.tiltShiftMaterial, 3);
				this.tiltShiftMaterial.SetVector("offsets", new Vector4(this.maxBlurSpread * 0.75f / num * num2, (float)0, (float)0, (float)0));
				Graphics.Blit(temporary4, temporary3, this.tiltShiftMaterial, 3);
			}
			Graphics.Blit(temporary3, temporary2, this.tiltShiftMaterial, 7);
			this.tiltShiftMaterial.SetTexture("_Coc", temporary2);
		}
		else
		{
			RenderTexture.active = temporary;
			GL.Clear(false, true, Color.black);
		}
		Graphics.Blit(source, temporary, this.tiltShiftMaterial, 5);
		this.tiltShiftMaterial.SetTexture("_Coc", temporary);
		Graphics.Blit(source, temporary4);
		for (int j = 0; j < this.blurIterations; j++)
		{
			this.tiltShiftMaterial.SetVector("offsets", new Vector4((float)0, this.maxBlurSpread * 1f * num2, (float)0, (float)0));
			Graphics.Blit(temporary4, temporary3, this.tiltShiftMaterial, 6);
			this.tiltShiftMaterial.SetVector("offsets", new Vector4(this.maxBlurSpread * 1f / num * num2, (float)0, (float)0, (float)0));
			Graphics.Blit(temporary3, temporary4, this.tiltShiftMaterial, 6);
		}
		this.tiltShiftMaterial.SetTexture("_Blurred", temporary4);
		Graphics.Blit(source, destination, this.tiltShiftMaterial, (!this.visualizeCoc) ? 1 : 4);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		RenderTexture.ReleaseTemporary(temporary3);
		RenderTexture.ReleaseTemporary(temporary4);
	}

	public override void Main()
	{
	}

	public Shader tiltShiftShader;

	private Material tiltShiftMaterial;

	public int renderTextureDivider;

	public int blurIterations;

	public bool enableForegroundBlur;

	public int foregroundBlurIterations;

	public float maxBlurSpread;

	public float focalPoint;

	public float smoothness;

	public bool visualizeCoc;

	private float start01;

	private float distance01;

	private float end01;

	private float curve;
}
