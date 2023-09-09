using System;
using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Info : Photon.MonoBehaviour
{
	private void Awake()
	{
		if (Info.Instance == null)
		{
			Info.Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void Update()
	{
		if (SceneManager.GetActiveScene().name == "Game")
		{
			this.GameMode = string.Empty + App.Instance.Settings.gameType;
		}
	}

	public static Info Instance;

	public string Location = string.Empty;

	public string GameMode = string.Empty;
}
