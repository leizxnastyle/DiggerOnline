using System;
using UnityEngine;

[ExecuteInEditMode]
public class WaterSimple : MonoBehaviour
{
	private void Update()
	{
		int num = (int)(Time.time * this.framesPerSecond);
		num %= this.uvAnimationTileX * this.uvAnimationTileY;
		Vector2 scale = new Vector2(1f / (float)this.uvAnimationTileX, 1f / (float)this.uvAnimationTileY);
		int num2 = num % this.uvAnimationTileX;
		int num3 = num / this.uvAnimationTileX;
		Vector2 offset = new Vector2((float)num2 * scale.x, 1f - scale.y - (float)num3 * scale.y);
		base.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
		base.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
	}

	private int uvAnimationTileX = 1;

	private int uvAnimationTileY = 51;

	private float framesPerSecond = 10f;
}
