using System;
using UnityEngine;

public class CubeDamage : MonoBehaviour
{
	private void Start()
	{
		this.Reset();
	}

	public void Reset()
	{
		this.cur_frame = 0;
		base.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
		base.GetComponent<Renderer>().material.color = Color.white;
		base.GetComponent<Renderer>().material.mainTexture = this.frames[this.cur_frame];
	}

	public void SetFrame(int frame)
	{
		if (frame > this.frames.Length - 1)
		{
			frame = this.frames.Length - 1;
		}
		if (frame < 0)
		{
			frame = 0;
		}
		this.cur_frame = frame;
		base.GetComponent<Renderer>().material.mainTexture = this.frames[this.cur_frame];
	}

	public Texture2D[] frames;

	public int cur_frame;
}
