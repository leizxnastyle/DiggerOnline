using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ProfileINI
{
	public static int level
	{
		get
		{
			return ProfileINI.LevelExp(ProfileINI.experience);
		}
	}

	public static int LevelExp(int exp)
	{
		return Mathf.Clamp(Mathf.FloorToInt(Mathf.Log((float)(exp / 25), 2f)) + 1, 1, 30);
	}

	public static int ExpForLevel(int level)
	{
		return (level > 1) ? Mathf.RoundToInt(Mathf.Pow(2f, (float)(level - 1)) * 25f) : 0;
	}

	public static int GetActualSkin()
	{
		return (!(TeamBattle.Instance != null)) ? ProfileINI.skin : TeamBattle.Instance.GetSkin();
	}

	public static string GetSlotMapName(int slotID)
	{
		string text;
		if (!ProfileINI.slotMapNames.TryGetValue(slotID, out text) || text == string.Empty)
		{
			text = Localize.GetText("MY_MAPS_MAP", null) + " " + slotID;
		}
		return text;
	}

	public static void Save()
	{
		PlayerPrefs.SetString("last_server_name", ProfileINI.server_name);
		PlayerPrefs.SetString("last_server_about", ProfileINI.server_about);
		Utils.PlayerPrefsSetBool("new_gamers_is_look", ProfileINI.newgamersislook);
		PlayerPrefs.SetInt("camera_type", ProfileINI.camera_type);
		PlayerPrefs.SetInt("tutorial_watch", ProfileINI.tutorial_watch);
		PlayerPrefs.SetInt("tutorial_watch", ProfileINI.tutorial_watch);
		if (PlayerPrefs.HasKey("sound_volume"))
		{
			PlayerPrefs.SetFloat("sound_volume", ProfileINI.sound_volume);
		}
		if (PlayerPrefs.HasKey("ambient_volume"))
		{
			PlayerPrefs.SetFloat("ambient_volume", ProfileINI.ambient_volume);
		}
		PlayerPrefs.SetFloat("draw_distance", ProfileINI.draw_distance);
		PlayerPrefs.SetInt("autoJump", (!ProfileINI.autoJump) ? 0 : 1);
		PlayerPrefs.SetInt("oneTapSet", (!ProfileINI.oneTapSet) ? 0 : 1);
		PlayerPrefs.SetInt("full_screen", (!ProfileINI.full_screen) ? 0 : 1);
		PlayerPrefs.SetInt("showBaloons", (!ProfileINI.showBaloons) ? 0 : 1);
		PlayerPrefs.SetInt("showSelfBaloons", (!ProfileINI.showSelfBaloons) ? 0 : 1);
		PlayerPrefs.SetInt("bloom", (!ProfileINI.bloom) ? 0 : 1);
		PlayerPrefs.SetInt("ambientOcclusion", (!ProfileINI.ambientOcclusion) ? 0 : 1);
	}

	public static void ToggleFullScreen()
	{
		ProfileINI.SetFullScreen(!ProfileINI.full_screen);
	}

	public static void SetFullScreen(bool flag)
	{
		if (ProfileINI.full_screen == flag)
		{
			return;
		}
		ProfileINI.full_screen = flag;
		ProfileINI.Save();
		if (!flag)
		{
			Screen.SetResolution(900, 600, false);
		}
		else
		{
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
			ProfileINI.RefreshScreenSize();
		}
	}

	public static void RefreshScreenSize()
	{
		Resolution currentResolution = Screen.currentResolution;
		if (WorldGameObjectX.Instance != null)
		{
			WorldGameObjectX.Instance.HitEffect.GetComponent<GUITexture>().pixelInset = new Rect((float)(-(float)currentResolution.width / 2), (float)(-(float)currentResolution.height / 2), (float)currentResolution.width, (float)currentResolution.height);
			WorldGameObjectX.Instance.ZombieEffect.GetComponent<GUITexture>().pixelInset = new Rect((float)(-(float)currentResolution.width / 2), (float)(-(float)currentResolution.height / 2), (float)currentResolution.width, (float)currentResolution.height);
		}
	}

	public static void LoadFromPrefs()
	{
		ProfileINI.tutorial_watch = PlayerPrefs.GetInt("tutorial_watch");
		if (PlayerPrefs.HasKey("full_screen"))
		{
			ProfileINI.full_screen = (PlayerPrefs.GetInt("full_screen") == 1);
		}
		ProfileINI.server_name = PlayerPrefs.GetString("last_server_name");
		ProfileINI.server_about = PlayerPrefs.GetString("last_server_about");
		ProfileINI.camera_type = PlayerPrefs.GetInt("camera_type");
		ProfileINI.newgamersislook = Utils.PlayerPrefsGetBool("new_gamers_is_look", false);
		ProfileINI.showBaloons = (PlayerPrefs.GetInt("showBaloons", 1) != 0);
		ProfileINI.showSelfBaloons = (PlayerPrefs.GetInt("showSelfBaloons", 1) != 0);
		ProfileINI.ambientOcclusion = (PlayerPrefs.GetInt("ambientOcclusion", 1) != 0);
		ProfileINI.bloom = (PlayerPrefs.GetInt("bloom", 1) != 0);
		if (PlayerPrefs.HasKey("draw_distance"))
		{
			ProfileINI.draw_distance = PlayerPrefs.GetFloat("draw_distance");
		}
		else
		{
			ProfileINI.draw_distance = 1f;
		}
		ProfileINI.autoJump = (PlayerPrefs.GetInt("autoJump", 1) != 0);
		ProfileINI.oneTapSet = (PlayerPrefs.GetInt("oneTapSet", 1) != 0);
		if (ProfileINI.mouse_sens == 0f)
		{
			ProfileINI.mouse_sens = 4f;
		}
		ProfileINI.purchasesList[StorePurchase.STANDART_SKIN] = 1;
		ProfileINI.SetPurchaseTime(StorePurchase.STANDART_SKIN, new DateTime(2200, 1, 1));
		ProfileINI.ilang = PlayerPrefs.GetInt("locale", 0);
	}

	public static void Reset()
	{
		ProfileINI.mouse_sens = 4f;
		ProfileINI.nickname = string.Empty;
		ProfileINI.experience = 0;
		ProfileINI.tutorial_watch = 0;
		ProfileINI.money[1] = 0;
		ProfileINI.money[0] = 1000;
		ProfileINI.options_destroy = 1;
		ProfileINI.sound_volume = 0.7f;
		ProfileINI.ambient_volume = 0.7f;
		ProfileINI.full_screen = true;
		ProfileINI.camera_type = 1;
		ProfileINI.showBaloons = true;
		ProfileINI.showSelfBaloons = true;
		ProfileINI.ambientOcclusion = true;
		ProfileINI.autoJump = true;
		ProfileINI.oneTapSet = true;
		ProfileINI.bloom = true;
		ProfileINI.SetPurchaseValue(StorePurchase.STANDART_SKIN, 1);
		ProfileINI.SetPurchaseTime(StorePurchase.STANDART_SKIN, new DateTime(2200, 1, 1));
	}

	public static int GetPurchaseValue(StorePurchase purchase)
	{
		SecuredValue<int> securedValue;
		ProfileINI.purchasesList.TryGetValue(purchase, out securedValue);
		if (securedValue == null)
		{
			return 0;
		}
		return securedValue;
	}

	public static bool CheckPurchaseValue(StorePurchase purchase)
	{
		SecuredValue<int> securedValue;
		ProfileINI.purchasesList.TryGetValue(purchase, out securedValue);
		return securedValue == null || securedValue.IsSecured();
	}

	public static void SetPurchaseCooldown(StorePurchase purchase, float duration)
	{
		ProfileINI.purchasesCooldown[purchase] = Time.time + duration;
	}

	public static float GetPurchaseCooldown(StorePurchase purchase)
	{
		SecuredValue<float> securedValue;
		if (ProfileINI.purchasesCooldown.TryGetValue(purchase, out securedValue))
		{
			float num = Mathf.Max(0f, securedValue.Value - Time.time);
			if (num == 0f)
			{
				ProfileINI.purchasesCooldown.Remove(purchase);
			}
			return num;
		}
		return 0f;
	}

	public static float GetPurchaseCooldown01(StorePurchase purchase)
	{
		float purchaseCooldown = ProfileINI.GetPurchaseCooldown(purchase);
		if (purchaseCooldown > 0f)
		{
			return purchaseCooldown / Store.Purchases[purchase].Cooldown.Value;
		}
		return 0f;
	}

	public static IEnumerator SetSkin(int Skin)
	{
		ProfileINI.skin = Skin;
		WWWForm form = new WWWForm();
		form.AddField("skin", Skin);
		form.AddField("id", VKAPI.INSTANCE._viewerId);
		form.AddField("auth_key", VKAPI.INSTANCE._authKey);
		WWW php_load = new WWW(SettingsManager.ServerURL[0] + "SetSkinX.php", form);
		yield return php_load;
		UnityEngine.Debug.Log(php_load.text);
		yield break;
	}

	public static IEnumerator SaveWeaponSkin()
	{
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VKAPI.INSTANCE._viewerId);
		form.AddField("skin_data", ProfileINI.WeaponSkinData.Save());
		form.AddField("auth_key", VKAPI.INSTANCE._authKey);
		WWW phpLoad = new WWW(SettingsManager.ServerURL[0] + "SetWeaponSkin.php", form);
		yield return phpLoad;
		UnityEngine.Debug.Log(phpLoad.text);
		yield break;
	}

	public static DateTime GetPurchaseTime(StorePurchase purchase)
	{
		DateTime result;
		ProfileINI.purchasesTime.TryGetValue(purchase, out result);
		return result;
	}

	public static void SetPurchaseValue(StorePurchase purchase, int value)
	{
		string text = new StackTrace().ToString();
		string[] array = text.Split(new string[]
		{
			"at"
		}, StringSplitOptions.RemoveEmptyEntries);
		if (array[2].Contains("MainMenu") || array[2].Contains("App"))
		{
			ProfileINI.purchasesList[purchase] = value;
		}
	}

	public static void AddPurchaseValue(StorePurchase purchase, int value)
	{
		if (!ProfileINI.purchasesList.ContainsKey(purchase))
		{
			ProfileINI.purchasesList.Add(purchase, value);
		}
	}

	public static void SetPurchaseTime(StorePurchase purchase, DateTime time)
	{
		ProfileINI.purchasesTime[purchase] = time;
	}

	public static string nickname;

	public static string Language;

	public static string server_name;

	public static string server_about;

	public static SecuredValue<int> skin = 0;

	public static float mouse_sens;

	public static float sound_volume = 1f;

	public static bool full_screen = false;

	public static float sound_scale = 1f;

	public static float ambient_volume;

	public static float draw_distance;

	public static int tutorial_watch;

	public static int options_destroy;

	public static int camera_type;

	public static int[] money = new int[Enum.GetNames(typeof(Currency)).Length];

	public static bool newgamersislook;

	public static SecuredValue<bool> was_banned = false;

	public static bool server_was_closed = false;

	public static DateTime server_time;

	public static bool showBaloons = true;

	public static bool showSelfBaloons = true;

	public static bool ambientOcclusion = true;

	public static bool autoJump = true;

	public static bool oneTapSet = true;

	public static bool bloom = false;

	public static int ilang = 0;

	public static SecuredValue<int> experience = 0;

	public static SecuredValue<bool> all_inclusive = false;

	public static bool showNonFreeServer = true;

	public static Dictionary<int, string> slotMapNames = new Dictionary<int, string>();

	private static Dictionary<StorePurchase, SecuredValue<int>> purchasesList = new Dictionary<StorePurchase, SecuredValue<int>>();

	private static Dictionary<StorePurchase, DateTime> purchasesTime = new Dictionary<StorePurchase, DateTime>();

	private static Dictionary<StorePurchase, SecuredValue<float>> purchasesCooldown = new Dictionary<StorePurchase, SecuredValue<float>>();

	public static WeaponSkinData WeaponSkinData;
}
