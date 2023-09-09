using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Global Fog")]
[Serializable]
public class GlobalFog : PostEffectsBase
{
	public GlobalFog()
	{
		this.fogMode = GlobalFog.FogMode.AbsoluteYAndDistance;
		this.CAMERA_NEAR = 0.5f;
		this.CAMERA_FAR = 50f;
		this.CAMERA_FOV = 60f;
		this.CAMERA_ASPECT_RATIO = 1.333333f;
		this.startDistance = 200f;
		this.globalDensity = 1f;
		this.heightScale = 100f;
		this.globalFogColor = Color.grey;
	}

	public virtual void CreateMaterials()
	{
		this.fogMaterial = this.CheckShaderAndCreateMaterial(this.fogShader, this.fogMaterial);
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
		bool flag = true;
		if (flag)
		{
			this.CAMERA_NEAR = this.GetComponent<Camera>().nearClipPlane;
			this.CAMERA_FAR = this.GetComponent<Camera>().farClipPlane;
			this.CAMERA_FOV = this.GetComponent<Camera>().fieldOfView;
			this.CAMERA_ASPECT_RATIO = this.GetComponent<Camera>().aspect;
		}
		Matrix4x4 identity = Matrix4x4.identity;
		Vector4 vector = default(Vector4);
		Vector3 vector2 = default(Vector3);
		float num = this.CAMERA_FOV * 0.5f;
		Vector3 b = this.GetComponent<Camera>().transform.right * this.CAMERA_NEAR * Mathf.Tan(num * 0.0174532924f) * this.CAMERA_ASPECT_RATIO;
		Vector3 b2 = this.GetComponent<Camera>().transform.up * this.CAMERA_NEAR * Mathf.Tan(num * 0.0174532924f);
		Vector3 vector3 = this.GetComponent<Camera>().transform.forward * this.CAMERA_NEAR - b + b2;
		float num2 = vector3.magnitude * this.CAMERA_FAR / this.CAMERA_NEAR;
		vector3.Normalize();
		vector3 *= num2;
		Vector3 vector4 = this.GetComponent<Camera>().transform.forward * this.CAMERA_NEAR + b + b2;
		vector4.Normalize();
		vector4 *= num2;
		Vector3 vector5 = this.GetComponent<Camera>().transform.forward * this.CAMERA_NEAR + b - b2;
		vector5.Normalize();
		vector5 *= num2;
		Vector3 vector6 = this.GetComponent<Camera>().transform.forward * this.CAMERA_NEAR - b - b2;
		vector6.Normalize();
		vector6 *= num2;
		identity.SetRow(0, vector3);
		identity.SetRow(1, vector4);
		identity.SetRow(2, vector5);
		identity.SetRow(3, vector6);
		this.fogMaterial.SetMatrix("_FrustumCornersWS", identity);
		this.fogMaterial.SetVector("_CameraWS", this.GetComponent<Camera>().transform.position);
		this.fogMaterial.SetVector("_StartDistance", new Vector4(1f / this.startDistance, num2 - this.startDistance));
		this.fogMaterial.SetVector("_Y", new Vector4(this.height, 1f / this.heightScale));
		this.fogMaterial.SetFloat("_GlobalDensity", this.globalDensity * 0.01f);
		this.fogMaterial.SetColor("_FogColor", this.globalFogColor);
		GlobalFog.CustomGraphicsBlit(source, destination, this.fogMaterial, (int)this.fogMode);
	}

	public static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
	{
		RenderTexture.active = dest;
		fxMaterial.SetTexture("_MainTex", source);
		GL.PushMatrix();
		GL.LoadOrtho();
		fxMaterial.SetPass(passNr);
		GL.Begin(7);
		GL.MultiTexCoord2(0, (float)0, (float)0);
		GL.Vertex3((float)0, (float)0, 3f);
		GL.MultiTexCoord2(0, 1f, (float)0);
		GL.Vertex3(1f, (float)0, 2f);
		GL.MultiTexCoord2(0, 1f, 1f);
		GL.Vertex3(1f, 1f, 1f);
		GL.MultiTexCoord2(0, (float)0, 1f);
		GL.Vertex3((float)0, 1f, (float)0);
		GL.End();
		GL.PopMatrix();
	}

	public override void Main()
	{
	}

	public GlobalFog.FogMode fogMode;

	private float CAMERA_NEAR;

	private float CAMERA_FAR;

	private float CAMERA_FOV;

	private float CAMERA_ASPECT_RATIO;

	public float startDistance;

	public float globalDensity;

	public float heightScale;

	public float height;

	public Color globalFogColor;

	public Shader fogShader;

	private Material fogMaterial;

	[Serializable]
	public enum FogMode
	{
		AbsoluteYAndDistance,
		AbsoluteY,
		Distance,
		RelativeYAndDistance
	}
}
