using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
[Serializable]
public class PostEffectsBase : MonoBehaviour
{
	public virtual Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		Material result;
		if (m2Create && m2Create.shader == s)
		{
			result = m2Create;
		}
		else if (!s)
		{
			UnityEngine.Debug.Log("Missing shader in " + this.ToString());
			this.enabled = false;
			result = null;
		}
		else if (!s.isSupported)
		{
			this.NotSupported();
			UnityEngine.Debug.LogError("The shader " + s.ToString() + " on effect " + this.ToString() + " is not supported on this platform!");
			result = null;
		}
		else
		{
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			result = ((!m2Create) ? null : m2Create);
		}
		return result;
	}

	public virtual Material CreateMaterial(Shader s, Material m2Create)
	{
		Material result;
		if (m2Create && m2Create.shader == s)
		{
			result = m2Create;
		}
		else if (!s)
		{
			UnityEngine.Debug.Log("Missing shader in " + this.ToString());
			result = null;
		}
		else if (!s.isSupported)
		{
			result = null;
		}
		else
		{
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			result = ((!m2Create) ? null : m2Create);
		}
		return result;
	}

	public virtual bool CheckSupport()
	{
		return this.CheckSupport(false);
	}

	public virtual bool CheckSupport(bool needDepth)
	{
		bool result;
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			this.NotSupported();
			result = false;
		}
		else if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			this.NotSupported();
			result = false;
		}
		else
		{
			result = true;
		}
		return result;
	}

	public virtual bool CheckShader(Shader s)
	{
		UnityEngine.Debug.Log("The shader " + s.ToString() + " on effect " + this.ToString() + " is not part of the Unity 3.2 effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package.");
		bool result;
		if (!s.isSupported)
		{
			this.NotSupported();
			result = false;
		}
		else
		{
			result = false;
		}
		return result;
	}

	public virtual void NotSupported()
	{
		UnityEngine.Debug.LogError("The image effect " + this.ToString() + "is not supported on this platform!");
		this.enabled = false;
	}

	public virtual void DrawBorder(RenderTexture dest, Material material)
	{
		float x = 0f;
		float x2 = 0f;
		float y = 0f;
		float y2 = 0f;
		RenderTexture.active = dest;
		bool flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			float y3 = 0f;
			float y4 = 0f;
			if (flag)
			{
				y3 = 1f;
				y4 = (float)0;
			}
			else
			{
				y3 = (float)0;
				y4 = 1f;
			}
			x = (float)0;
			x2 = (float)0 + 1f / ((float)dest.width * 1f);
			y = (float)0;
			y2 = 1f;
			GL.Begin(7);
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			x = 1f - 1f / ((float)dest.width * 1f);
			x2 = 1f;
			y = (float)0;
			y2 = 1f;
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			x = (float)0;
			x2 = 1f;
			y = (float)0;
			y2 = (float)0 + 1f / ((float)dest.height * 1f);
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			x = (float)0;
			x2 = 1f;
			y = 1f - 1f / ((float)dest.height * 1f);
			y2 = 1f;
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}

	public virtual void Main()
	{
	}
}
