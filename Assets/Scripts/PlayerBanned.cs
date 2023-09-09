using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBanned : MonoBehaviour
{
	private void Awake()
	{
		if (PlayerBanned.Instance == null)
		{
			PlayerBanned.Instance = this;
		}
	}

	public void BuyUnban()
	{
		SoundManager.Instance.Play(SoundManager.Sound.MenuClick, this.Audio.GetComponent<AudioSource>());
		Application.ExternalCall("BuyItem", new object[]
		{
			"unban"
		});
	}

	public void ShowBan()
	{
		if (SceneManager.GetActiveScene().name == "Game")
		{
			PhotonNetwork.Disconnect();
		}
		MainMenu.Instance.SwitchMenu(Menu.Ban, null, null);
		for (int i = 0; i < 3; i++)
		{
			this.Localization[i].GetComponent<Text>().text = Localize.GetText("key300" + i, null);
		}
	}

	public static PlayerBanned Instance;

	public AudioSource Audio;

	public Text[] Localization;
}
