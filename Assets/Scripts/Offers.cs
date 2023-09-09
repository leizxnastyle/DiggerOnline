using System;
using UnityEngine;
using UnityEngine.UI;

public class Offers : MonoBehaviour
{
	private void Awake()
	{
		if (Offers.Instance == null)
		{
			Offers.Instance = this;
		}
	}

	public void SetOffersBanners()
	{
		UnityEngine.Debug.Log("AddGameToMenu");
		Application.ExternalCall("AddGameToMenu", new object[0]);
	}

	public void SetOffers()
	{
		KGUI.SetNodes("Start.Canvas.Slot1.Offer1", false, false);
		KGUI.SetNodes("Start.Canvas.Slot1.Offer2", false, false);
		KGUI.SetNodes("Start.Canvas.Slot1.Offer3", false, false);
		KGUI.SetNodes("Start.Canvas.Slot1.Offer4", false, false);
		KGUI.SetNodes("Start.Canvas.Slot2.Offer1", false, false);
		KGUI.SetNodes("Start.Canvas.Slot2.Offer2", false, false);
		KGUI.SetNodes("Start.Canvas.Slot2.Offer3", false, false);
		KGUI.SetNodes("Start.Canvas.Slot2.Offer4", false, false);
		KGUI.SetNodes("Start.Canvas.Slot3", false, false);
		this.Slot1 = 1;
		this.Slot2 = 1;
		if (this.Slot2 != 0)
		{
			KGUI.SetNodes("Start.Canvas.Slot2.Offer" + this.Slot2, true, false);
		}
		if (this.Slot1 != 0)
		{
			KGUI.SetNodes("Start.Canvas.Slot1.Offer" + this.Slot1, true, false);
		}
		if (this.Slot3 != 0)
		{
			KGUI.SetNodes("Start.Canvas.Slot3", true, false);
		}
		for (int i = 0; i < 8; i++)
		{
			this.Localization[i].GetComponent<Text>().text = Localize.GetText("key600" + i, null);
		}
	}

	public void BuyOffer(int Offer)
	{
		SoundManager.Instance.Play(SoundManager.Sound.MenuClick, this.Audio.GetComponent<AudioSource>());
		switch (Offer)
		{
		case 1:
			MainMenu.Instance.SwitchMenu(Menu.Shop, Store.TabType.Decorations_Star, null);
			break;
		case 2:
			MainMenu.Instance.BuyPurchase(StorePurchase.ARCHER_SKIN, 0);
			break;
		case 3:
			MainMenu.Instance.BuyPurchase(StorePurchase.COOK_SKIN, 0);
			break;
		case 4:
			MainMenu.Instance.BuyPurchase(StorePurchase.ZOMBIE_SKIN, 0);
			break;
		case 5:
			MainMenu.Instance.SwitchMenu(Menu.Shop, Store.TabType.Skins, null);
			MainMenu.Instance.ShowPurchaseItem(StorePurchase.POLICEMAN_GIRL);
			break;
		case 6:
			MainMenu.Instance.BuyPurchase(StorePurchase.WSKIN_SG553_DRAGON, 0);
			break;
		case 7:
			MainMenu.Instance.BuyPurchase(StorePurchase.WSKIN_SG553_SPACE, 0);
			break;
		case 8:
			MainMenu.Instance.BuyPurchase(StorePurchase.BOAT, 0);
			break;
		}
	}

	public static Offers Instance;

	public AudioSource Audio;

	public Text[] Localization;

	private int Slot1;

	private int Slot2;

	private int Slot3;
}
