using System;
using UnityEngine;

public class CameraViewController : MonoBehaviour
{
	private void Start()
	{
		float num = ProfileINI.draw_distance;
		if (num <= 0f)
		{
			num = 0.1f;
		}
		this.camera = base.transform.GetComponent<Camera>();
		this.farClipPlane = this.camera.farClipPlane;
		this.farClipPlane = this.farClipPlane / 100f * num * 100f;
		this.camera.farClipPlane = this.farClipPlane;
	}

	private Camera camera;

	private float farClipPlane = 200f;
}
