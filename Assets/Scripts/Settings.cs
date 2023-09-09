using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		if (Settings.Instance == null)
		{
			Settings.Instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		PhotonNetwork.UseNameServer = true;
	}

	public static Settings Instance;
}
