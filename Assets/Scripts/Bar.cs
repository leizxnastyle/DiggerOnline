using System;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
	private void Awake()
	{
		if (Bar.Instance == null)
		{
			Bar.Instance = this;
		}
	}

	public void SetCoins()
	{
		this.CoinsValue.GetComponent<Text>().text = string.Empty + ProfileINI.money[0].ToString();
	}

	public void SetPage(int Page)
	{
		KGUI.SetNodes("input_text", false, false);
		SoundManager.Instance.Play(SoundManager.Sound.MenuClick, this.Audio.GetComponent<AudioSource>());
		switch (Page)
		{
		case 1:
			MainMenu.Instance.SwitchMenu(Menu.Start, null, null);
			break;
		case 2:
			MainMenu.Instance.SwitchMenu(Menu.Shop, null, null);
			KGUI.SetNodes("Shop.Canvas", false, false);
			break;
		case 3:
			MainMenu.Instance.SwitchMenu(Menu.Profile, null, null);
			break;
		case 4:
			MainMenu.Instance.SwitchMenu(Menu.TopMaps, null, null);
			break;
		case 5:
			MainMenu.Instance.SwitchMenu(Menu.Community, null, null);
			break;
		case 6:
			MainMenu.Instance.SwitchMenu(Menu.Ratings, null, null);
			break;
		case 7:
			MainMenu.Instance.SwitchMenu(Menu.Achievements, null, null);
			break;
		case 8:
			MainMenu.Instance.SwitchMenu(Menu.Settings, null, null);
			break;
		case 9:
			MainMenu.Instance.SwitchMenu(Menu.Bank, null, null);
			break;
		}
	}

	public void SetLocalization()
	{
		for (int i = 0; i < 8; i++)
		{
			this.Localization[i].GetComponent<Text>().text = Localize.GetText("key700" + i, null);
		}
	}

	public static Bar Instance;

	public AudioSource Audio;

	public Text[] Localization;

	public Text CoinsValue;
}
