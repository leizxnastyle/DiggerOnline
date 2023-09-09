using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Camera Info")]
[ExecuteInEditMode]
[Serializable]
public class CameraInfo : MonoBehaviour
{
	public virtual void Main()
	{
	}

	public DepthTextureMode currentDepthMode;

	public RenderingPath currentRenderPath;

	public int currentPostFxCount;
}
