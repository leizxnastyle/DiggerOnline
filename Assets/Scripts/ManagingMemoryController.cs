using System;
using UnityEngine;

public class ManagingMemoryController : MonoBehaviour
{
	private void Start()
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
		UnityEngine.Debug.Log("ManagingMemoryController Work END");
	}
}
