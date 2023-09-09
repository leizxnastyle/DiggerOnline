using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Keyboard : MonoBehaviour
{
	private void Awake()
	{
		if (Keyboard.Instance == null)
		{
			Keyboard.Instance = this;
		}
	}

	private void Update()
	{
		if (SceneManager.GetActiveScene().name == "Game")
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.C))
			{
				if (App.Instance.Settings.gameType == GameINI.GameType.BUILDING)
				{
					WorldGameObjectX.Instance.ToggleInventory();
				}
				else if (App.Instance.Settings.gameType == GameINI.GameType.HUNGER_GAMES)
				{
					MainMenu.Instance.OpenCloseInventory();
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.I) && App.Instance.Settings.gameType == GameINI.GameType.HUNGER_GAMES)
			{
				Chat.SendWarning(Localize.GetText("key4007", null), false);
			}
		}
	}

	public static Keyboard Instance;
}
