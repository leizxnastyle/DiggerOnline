using System;
using UnityEngine;

public class PoolsDebugBehaviour : MonoBehaviour
{
	public void OnGUI()
	{
		if (Time.time > this.nextUpdateTime)
		{
			this.nextUpdateTime = Time.time + 1f;
			this.UpdateText();
		}
		GUI.Label(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.text);
	}

	private void UpdateText()
	{
		this.text = string.Format("PChunkList pool size: {0} {1} {2}\n{3}", new object[]
		{
			PChunkList.pool.AcquiredCount,
			PChunkList.pool.ReleasedCount,
			PChunkList.pool.PoolCount,
			PMeshData.GetDebugInfo()
		});
	}

	private string text = string.Empty;

	private float nextUpdateTime;
}
