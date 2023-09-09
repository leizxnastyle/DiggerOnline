using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Edge Detection (Geometry)")]
[Serializable]
public class EdgeDetectEffectNormals : PostEffectsBase
{
	public EdgeDetectEffectNormals()
	{
		this.mode = EdgeDetectMode.Thin;
		this.sensitivityDepth = 1f;
		this.sensitivityNormals = 1f;
		this.edgesOnlyBgColor = Color.white;
	}

	public virtual void CreateMaterials()
	{
		this.edgeDetectMaterial = this.CheckShaderAndCreateMaterial(this.edgeDetectShader, this.edgeDetectMaterial);
	}

	public virtual void Start()
	{
		this.CreateMaterials();
		this.CheckSupport(true);
	}

	public virtual void OnEnable()
	{
		this.GetComponent<Camera>().depthTextureMode = (this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.DepthNormals);
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.CreateMaterials();
		Vector2 vector = new Vector2(this.sensitivityDepth, this.sensitivityNormals);
		source.filterMode = FilterMode.Point;
		this.edgeDetectMaterial.SetVector("sensitivity", new Vector4(vector.x, vector.y, 1f, vector.y));
		this.edgeDetectMaterial.SetFloat("_BgFade", this.edgesOnly);
		Vector4 vector2 = this.edgesOnlyBgColor;
		this.edgeDetectMaterial.SetVector("_BgColor", vector2);
		if (this.mode == EdgeDetectMode.Thin)
		{
			Graphics.Blit(source, destination, this.edgeDetectMaterial, 0);
		}
		else
		{
			Graphics.Blit(source, destination, this.edgeDetectMaterial, 1);
		}
	}

	public override void Main()
	{
	}

	public EdgeDetectMode mode;

	public float sensitivityDepth;

	public float sensitivityNormals;

	public float edgesOnly;

	public Color edgesOnlyBgColor;

	public Shader edgeDetectShader;

	private Material edgeDetectMaterial;
}
