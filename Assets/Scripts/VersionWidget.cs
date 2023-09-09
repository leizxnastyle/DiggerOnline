using System;
using UnityEngine;

public class VersionWidget : MonoBehaviour
{
	private void Start()
	{
		KGUI.OnLocaleChenged += this.OnLocaleChenged;
		this.OnLocaleChenged();
	}

	private void OnLocaleChenged()
	{
		UnityEngine.Debug.Log("OnLocaleChenged");
		WorldGameObjectX.Instance.MapExit();
	}

	public string Version;
}
