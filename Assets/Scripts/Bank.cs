using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bank : MonoBehaviour
{
	private void Awake()
	{
		if (Bank.Instance == null)
		{
			Bank.Instance = this;
		}
		this.AndroidPurchase = base.gameObject.AddComponent<GooglePlayPurchase>();
	}

	public void BuyCoins(int Count)
	{
		SoundManager.Instance.Play(SoundManager.Sound.MenuClick, this.Audio.GetComponent<AudioSource>());
		this.AndroidPurchase.BuyProductID(Count.ToString());
	}

	public void PaymentSuccessful()
	{
		base.StartCoroutine(App.Instance.LoadProfile2(false));
	}

	public void SetPrice()
	{
		for (int i = 0; i < 6; i++)
		{
			if (i != 0)
			{
				this.PriceVotes[i].GetComponent<Text>().text = Localize.GetText("key5001", null) + ManagerBank.PriceVotes[i].ToString() + Localize.GetText("key5003", null);
			}
			else
			{
				this.PriceVotes[i].GetComponent<Text>().text = Localize.GetText("key5001", null) + ManagerBank.PriceVotes[i].ToString() + Localize.GetText("key5002", null);
			}
			if (ManagerBank.BonusX2)
			{
				if (i != 5)
				{
					this.Label[i].GetComponent<Image>().enabled = true;
					this.PriceCoins[i].GetComponent<Text>().text = (ManagerBank.PriceCoins[i] * 2).ToString() + Localize.GetText("key5000", null);
				}
				else
				{
					this.Label[i].GetComponent<Image>().enabled = false;
					this.PriceCoins[i].GetComponent<Text>().text = ManagerBank.PriceCoins[i].ToString() + Localize.GetText("key5000", null);
				}
			}
			else
			{
				this.Label[i].GetComponent<Image>().enabled = false;
				this.PriceCoins[i].GetComponent<Text>().text = ManagerBank.PriceCoins[i].ToString() + Localize.GetText("key5000", null);
			}
		}
	}

	public IEnumerator GetSales()
	{
		WWWForm Form = new WWWForm();
		Form.AddField("PlayerID", VKAPI.INSTANCE._viewerId);
		Form.AddField("AuthKey", VKAPI.INSTANCE._authKey);
		WWW phpLoad = new WWW(SettingsManager.ServerURL[2] + "GetSales.php", Form);
		yield return phpLoad;
		if (phpLoad.text == "ON")
		{
			ManagerBank.BonusX2 = true;
		}
		yield break;
	}

	public static Bank Instance;

	public AudioSource Audio;

	public Text[] PriceCoins;

	public Text[] PriceVotes;

	public Image[] Label;

	private GooglePlayPurchase AndroidPurchase;
}
