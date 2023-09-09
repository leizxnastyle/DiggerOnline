using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Blur")]
[ExecuteInEditMode]
public class BlurEffect : MonoBehaviour
{
	protected static Material material
	{
		get
		{
			if (BlurEffect.m_Material == null)
			{
				BlurEffect.m_Material = new Material(BlurEffect.blurMatString);
				BlurEffect.m_Material.hideFlags = HideFlags.HideAndDontSave;
				BlurEffect.m_Material.shader.hideFlags = HideFlags.HideAndDontSave;
			}
			return BlurEffect.m_Material;
		}
	}

	protected void OnDisable()
	{
		if (BlurEffect.m_Material)
		{
			UnityEngine.Object.DestroyImmediate(BlurEffect.m_Material.shader);
			UnityEngine.Object.DestroyImmediate(BlurEffect.m_Material);
		}
	}

	protected void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
			return;
		}
		if (!BlurEffect.material.shader.isSupported)
		{
			base.enabled = false;
			return;
		}
	}

	public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
	{
		float num = 0.5f + (float)iteration * this.blurSpread;
		Graphics.BlitMultiTap(source, dest, BlurEffect.material, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(source, dest, BlurEffect.material, new Vector2[]
		{
			new Vector2(-num, -num),
			new Vector2(-num, num),
			new Vector2(num, num),
			new Vector2(num, -num)
		});
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		this.DownSample4x(source, temporary);
		bool flag = true;
		for (int i = 0; i < this.iterations; i++)
		{
			if (flag)
			{
				this.FourTapCone(temporary, temporary2, i);
			}
			else
			{
				this.FourTapCone(temporary2, temporary, i);
			}
			flag = !flag;
		}
		if (flag)
		{
			Graphics.Blit(temporary, destination);
		}
		else
		{
			Graphics.Blit(temporary2, destination);
		}
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
	}

	public int iterations = 3;

	public float blurSpread = 0.6f;

	private static string blurMatString = "Shader \"BlurConeTap\" {\n\tProperties { _MainTex (\"\", any) = \"\" {} }\n\tSubShader {\n\t\tPass {\n\t\t\tZTest Always Cull Off ZWrite Off Fog { Mode Off }\n\t\t\tSetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant alpha}\n\t\t\tSetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant + previous}\n\t\t\tSetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant + previous}\n\t\t\tSetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant + previous}\n\t\t}\n\t}\n\tFallback off\n}";

	private static Material m_Material;
}
