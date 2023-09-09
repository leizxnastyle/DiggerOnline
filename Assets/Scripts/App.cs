using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using com.playGenesis.VkUnityPlugin;
using com.playGenesis.VkUnityPlugin.MiniJSON;
using Photon;
using UnityEngine;
using UnityEngine.SceneManagement;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class App : Photon.MonoBehaviour
{
	public string GetProtocol()
	{
		return "https://";
	}

	public string GenerateKey(int length)
	{
		System.Random random = new System.Random();
		char[] array = new char[]
		{
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'I',
			'J',
			'K',
			'L',
			'M',
			'N',
			'O',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			'Y',
			'Z',
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'g',
			'h',
			'i',
			'j',
			'k',
			'l',
			'm',
			'n',
			'o',
			'p',
			'q',
			'r',
			's',
			't',
			'u',
			'v',
			'w',
			'x',
			'y',
			'z',
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9'
		};
		string text = string.Empty;
		for (int i = 0; i < length; i++)
		{
			text += array[random.Next(0, 35)];
		}
		return text;
	}

	public string KeyEncrypt(string message)
	{
		string result;
		try
		{
			string xmlString = "<RSAKeyValue><Modulus>t3A/x4DoMsQNVdIWe0rYmBy98tIQz+2/M5eiqL7IxS2h7L3mHLZVOHeTzDWbBGLHIx8la1oWUXPGUr/31Mgbvuc8PDXW0Oh011LWUYYPgaFLKqwvnS7T8ehNYLNkHxEeiKrtMPHSp0n6IfChKAvUZbdPs9IjAr1kO2lVNJ5Ickc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
			RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider();
			rsacryptoServiceProvider.FromXmlString(xmlString);
			result = Convert.ToBase64String(rsacryptoServiceProvider.Encrypt(Encoding.UTF8.GetBytes(message), true));
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("Problem with Encrypt:\n" + ex.Message.ToString());
			result = string.Empty;
		}
		return result;
	}

	public void SetCurrentLocale()
	{
		if (Application.systemLanguage.ToString() == "Russian")
		{
			Localize.Locale = Localize.LocaleType.RU;
			KGUI.SetLocale(Localize.LocaleType.RU);
		}
		else
		{
			Localize.Locale = Localize.LocaleType.EN;
			KGUI.SetLocale(Localize.LocaleType.EN);
		}
	}

	private void Awake()
	{
		App.IsCheckAllIn = false;
		bool flag = !PlayerPrefs.HasKey("full_screen");
		if (flag && this.CurPlatform == App.Platform.STEAM)
		{
			ProfileINI.SetFullScreen(true);
		}
		if (App.Instance == null)
		{
			App.Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (PhotonNetwork.connected)
		{
			MainMenu.Instance.HideLoading();
			WorldGameObjectX.Instance.MapExit();
			if (ProfileINI.was_banned)
			{
				MainMenu.Instance.ShowHint("HINT_YOU_HAVE_BAN", false);
				ProfileINI.was_banned = false;
			}
			else if (ProfileINI.server_was_closed)
			{
				MainMenu.Instance.ShowHint("HINT_SERVER_CLOSED", false);
				ProfileINI.server_was_closed = false;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			MainMenu.Instance.HideLoading();
			App.Instance.Start();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		ProfileINI.WeaponSkinData = new WeaponSkinData();
		MainMenu.Instance.SwitchMenu(Menu.Authorizathion, null, null);
		Application.ExternalCall("SendSettingsToPlayer", new object[0]);
	}

	public void VkontakteAndroidAuth()
	{
		UnityEngine.Debug.Log("VkontakteAndroidAuth");
		KGUI.SetNodes("Authorizathion.Image", false, false);
		KGUI.SetNodes("Authorizathion.AuthLabel", true, false);
		this.vkapi = VkApi.VkApiInstance;
		this.vkapi.LoggedIn += this.onVKLogin;
		this.vkapi.Login();
	}

	private void onVKLogin()
	{
		this.vkapi.LoggedIn -= this.onVKLogin;
		if (this.vkapi.IsUserLoggedIn)
		{
			UnityEngine.Debug.Log(VkApi.CurrentToken.user_id);
			UnityEngine.Debug.Log(VkApi.CurrentToken.access_token);
			base.StartCoroutine(this.GetAuthKeyForVKAndroid(VkApi.CurrentToken.user_id, VkApi.CurrentToken.access_token));
		}
	}

	private IEnumerator GetAuthKeyForVKAndroid(string user_id, string user_tocken)
	{
		WWWForm Form = new WWWForm();
		Form.AddField("user_id", user_id);
		Form.AddField("user_token", user_tocken);
		WWW phpLoad = new WWW(SettingsManager.ServerURL[0] + "LoadAuthKeyForAndroid.php", Form);
		yield return phpLoad;
		UnityEngine.Debug.Log(user_id);
		UnityEngine.Debug.Log(user_tocken);
		UnityEngine.Debug.Log(phpLoad.text);
		Dictionary<string, object> res = Json.Deserialize(phpLoad.text) as Dictionary<string, object>;
		App.AuthCodeJson json = JsonUtility.FromJson<App.AuthCodeJson>(phpLoad.text);
		UnityEngine.Debug.Log(phpLoad.text);
		UnityEngine.Debug.Log(json.result);
		UnityEngine.Debug.Log(json.auth_code);
		if ((long)res["result"] == 1L)
		{
			VKAPI.INSTANCE._viewerId = user_id;
			VKAPI.INSTANCE._authKey = (string)res["auth_code"];
			UnityEngine.Debug.Log(VKAPI.INSTANCE._viewerId);
			UnityEngine.Debug.Log(VKAPI.INSTANCE._authKey);
			base.StartCoroutine(this.LoadProfile());
		}
		yield break;
	}

	public void ConnectToShard(bool random)
	{
		this.CurrentShard = 0;
		if (!random)
		{
			this._CurShardTest = 1;
			this._WaitingShardPopulation = false;
			this.TestNextShard();
		}
		else
		{
			if (VKAPI.INSTANCE._gameID == string.Empty)
			{
				this.CurrentShard = UnityEngine.Random.Range(0, this.PlatformShardCount[(int)this.CurPlatform]) + 1;
			}
			else
			{
				this.CurrentShard = int.Parse(VKAPI.INSTANCE._gameID.Substring(0, 2));
			}
			this.CurrentShard = 1;
			MainMenu.Instance.HideLoading();
			MainMenu.Instance.ShowLoading("LOADING_SERVER", string.Empty);
			PhotonNetwork.autoJoinLobby = true;
			PhotonNetwork.ConnectUsingSettings(string.Concat(new object[]
			{
				this.CurPlatform.ToString(),
				this.GameVersion,
				"Qshard_",
				this.CurrentShard
			}));
		}
	}

	public void TestNextShard()
	{
		if (this._CurShardTest > this.PlatformShardCount[(int)this.CurPlatform])
		{
			this._CurShardTest = 0;
			MainMenu.Instance.HideLoading();
			MainMenu.Instance.SwitchMenu(Menu.ServerSelect, null, null);
			UnityEngine.Debug.Log("MavrinTest");
		}
		else
		{
			MainMenu.Instance.ShowLoading("LOADING_GET_SHARDS", string.Concat(new object[]
			{
				" [",
				this._CurShardTest,
				"/",
				this.PlatformShardCount[(int)this.CurPlatform],
				"]"
			}));
			PhotonNetwork.autoJoinLobby = false;
			PhotonNetwork.ConnectUsingSettings(string.Concat(new object[]
			{
				this.CurPlatform.ToString(),
				this.GameVersion,
				"Qshard_",
				this._CurShardTest
			}));
		}
	}

	public void WaitingShardPopulation()
	{
		if (this._CurShardTest > 0)
		{
			this._WaitingShardPopulationTime += Time.deltaTime;
			if (this._WaitingShardPopulationTime < 5f)
			{
				if (PhotonNetwork.countOfRooms > 0)
				{
					this.ShardPopulation[this._CurShardTest - 1] = PhotonNetwork.countOfRooms;
					PhotonNetwork.Disconnect();
					this._CurShardTest++;
					this._WaitingShardPopulation = false;
				}
			}
			else
			{
				this.ShardPopulation[this._CurShardTest - 1] = PhotonNetwork.countOfRooms;
				PhotonNetwork.Disconnect();
				this._CurShardTest++;
				this._WaitingShardPopulation = false;
			}
		}
	}

	public void OnCheatDetected(string name)
	{
		if (SceneManager.GetActiveScene().name == "Game")
		{
			WorldGameObjectX.Instance.ExitGame("CHEAT DETECT 1");
		}
	}

	private IEnumerator SpeedHackDetected()
	{
		for (;;)
		{
			this.last_realtime = Time.time;
			this.last_datetime = DateTime.Now;
			yield return new WaitForSeconds(5f);
			float good = (float)(DateTime.Now - this.last_datetime).Seconds;
			float current = Time.time - this.last_realtime;
			UnityEngine.Debug.Log(good);
			UnityEngine.Debug.Log(current);
			if (Math.Abs(good - current) > 0.5f)
			{
				this.OnCheatDetected("speed_hack");
			}
		}
		yield break;
	}

	private void Update()
	{
		ProfileINI.server_time = ProfileINI.server_time.AddSeconds((double)Time.deltaTime);
		if (this._WaitingShardPopulation)
		{
			this.WaitingShardPopulation();
		}
	}

	public void OnConnectedToMaster()
	{
		if (this._CurShardTest > 0)
		{
			this._WaitingShardPopulation = true;
			this._WaitingShardPopulationTime = 0f;
		}
	}

	public void OnConnectedToPhoton()
	{
		if (this.CurrentShard > 0)
		{
			if (VKAPI.INSTANCE._gameID == string.Empty)
			{
				MainMenu.Instance.HideLoading();
				MainMenu.Instance.SwitchMenu(Menu.Start, null, null);
				if (VKAPI.INSTANCE._mapID > 1)
				{
					int mapID = VKAPI.INSTANCE._mapID;
					VKAPI.INSTANCE._mapID = 0;
					MainMenu.Instance.SwitchMenu(Menu.TopMaps, mapID, null);
				}
			}
			else
			{
				MainMenu.Instance.SetLoadingText(Localize.GetText("LOADING_WAIT_ROOMS", null), string.Empty);
			}
		}
	}

	private void OnReceivedRoomListUpdate()
	{
		if (this.CurrentShard > 0 && VKAPI.INSTANCE._gameID != string.Empty)
		{
			base.StartCoroutine(this.TryConnectToRoomProcess());
		}
	}

	private IEnumerator TryConnectToRoomProcess()
	{
		MainMenu.Instance.SetLoadingText("GAME_URL_FIND_ROOM", string.Empty);
		string roomName = VKAPI.INSTANCE._gameID.Substring(2);
		VKAPI.INSTANCE._gameID = string.Empty;
		RoomInfo foundedRoom = null;
		for (int i = 0; i < 10; i++)
		{
			RoomInfo[] allBattlesRoomList = PhotonNetwork.GetRoomList();
			foreach (RoomInfo room in allBattlesRoomList)
			{
				if (room.name == roomName)
				{
					foundedRoom = room;
					break;
				}
			}
			if (foundedRoom != null)
			{
				break;
			}
			yield return new WaitForSeconds(1f);
		}
		MainMenu.Instance.HideLoading();
		MainMenu.Instance.SwitchMenu(Menu.Start, null, null);
		if (foundedRoom != null)
		{
			Info.Instance.Location = "Link";
			MainMenu.Instance.JoinGame(foundedRoom);
		}
		else
		{
			MainMenu.Instance.ShowHint("GAME_URL_GAME_NOT_FOUND", false);
		}
		yield break;
	}

	private void OnDisconnectedFromPhoton()
	{
		base.StartCoroutine(this.HandleDisconnection());
	}

	private void OnFailedToConnectToPhoton()
	{
		base.StartCoroutine(this.HandleDisconnection());
	}

	private void OnApplicationQuit()
	{
		this._AppQuit = true;
	}

	private IEnumerator HandleDisconnection()
	{
		yield return 0;
		if (this._AppQuit)
		{
			yield break;
		}
		if (this._CurShardTest > 0)
		{
			this.TestNextShard();
			yield break;
		}
		MainMenu.Instance.HideMenu();
		MainMenu.Instance.HideLoading();
		if (SceneManager.GetActiveScene().name == "Game" && WorldGameObjectX.Instance.IsWorldGenerated)
		{
			MainMenu.Instance.ShowLoading("LOADING_LEAVING", string.Empty);
			SceneManager.LoadScene("Menu");
		}
		else if (SceneManager.GetActiveScene().name == "Menu")
		{
			this.ConnectToShard(false);
		}
		yield break;
	}

	public void ShowGame()
	{
		Screen.SetResolution(900, 600, false);
	}

	public void HideGame()
	{
		Screen.SetResolution(1, 1, false);
	}

	public void OnBallanceChanged()
	{
		base.StartCoroutine(this.LoadProfile2(false));
		Bar.Instance.SetCoins();
	}

	public void CheckTimePurchases()
	{
		foreach (KeyValuePair<StorePurchase, Store.PurchaseInfo> keyValuePair in Store.Purchases)
		{
			Store.Pay cost = keyValuePair.Value.Cost;
			if (cost is Store.TimedPay && ProfileINI.GetPurchaseValue(keyValuePair.Key) > 0 && ProfileINI.GetPurchaseTime(keyValuePair.Key) < ProfileINI.server_time)
			{
				ProfileINI.SetPurchaseValue(keyValuePair.Key, 0);
				MainMenu.Instance.PurchaseUse(keyValuePair.Key, false);
			}
		}
	}

	public IEnumerator LoadProfile()
	{
		if (!ManagerAudio.AudioLoaded)
		{
			base.StartCoroutine(SoundManager.Instance.LoadingSound());
		}
		base.StartCoroutine(Protect.Instance.GetBan());
		base.StartCoroutine(this.CheckEveryDayBonus());
		base.StartCoroutine(Bank.Instance.GetSales());
		yield return null;
		yield break;
	}

	public IEnumerator CheckEveryDayBonus()
	{
		WWWForm everydayBonusForm = new WWWForm();
		everydayBonusForm.AddField("id", VKAPI.INSTANCE._viewerId);
		System.Random r = new System.Random();
		everydayBonusForm.AddField("random", r.Next());
		everydayBonusForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		WWW everydayBonus = null;
		everydayBonus = new WWW(SettingsManager.ServerURL[0] + "GetBonusX.php", everydayBonusForm);
		yield return everydayBonus;
		if (everydayBonus.error != null)
		{
			UnityEngine.Debug.LogError(everydayBonus.error);
			yield break;
		}
		int bonusGemsIndex = everydayBonus.text.IndexOf("BONUS_GEMS");
		if (bonusGemsIndex != -1)
		{
			MainMenu.Instance.BonusGemsEndTime = DateTime.Parse(everydayBonus.text.Substring(everydayBonus.text.IndexOf("BONUS_GEMS") + 11));
		}
		if (everydayBonus.text.StartsWith("OK"))
		{
			string[] data = everydayBonus.text.Split(new char[]
			{
				' '
			});
			int[] bonusData = new int[]
			{
				int.Parse(data[1]),
				int.Parse(data[2]),
				int.Parse(data[3])
			};
			MainMenu.Instance.HideLoading();
			MainMenu.Instance.SwitchMenu(Menu.Bonus, bonusData, null);
		}
		else
		{
			yield return base.StartCoroutine(this.LoadProfile2(true));
		}
		yield break;
	}

	public IEnumerator LoadProfile2(bool from_loading)
	{
		WWWForm getProfileForm = new WWWForm();
		getProfileForm.AddField("id", VKAPI.INSTANCE._viewerId);
		getProfileForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		System.Random r = new System.Random();
		int randomI = r.Next();
		int serverRND = 0;
		string social_name = string.Empty;
		string player_id = string.Empty;
		getProfileForm.AddField("random", randomI);
		string yoba_key = this.GenerateKey(20);
		getProfileForm.AddField("yoba_key", this.KeyEncrypt(yoba_key));
		WWW getProfile = new WWW(SettingsManager.ServerURL[0] + "GetProfile.php", getProfileForm);
		yield return getProfile;
		if (base.gameObject.GetComponent<ContentUpdater>() == null)
		{
			base.gameObject.AddComponent<ContentUpdater>();
		}
		if (getProfile.text.StartsWith("ACTIVATION"))
		{
			MainMenu.Instance.SetLoadingText("ACTIVATION_REQUIRED", string.Empty);
			yield break;
		}
		string serverText = getProfile.text.Substring(0, getProfile.text.IndexOf('\n'));
		UnityEngine.Debug.Log("Enter parsing profile...");
		if (serverText.Substring(0, 13) == "Saved Profile")
		{
			string profile_params = serverText.Substring(15);
			string[] param_vector = profile_params.Split(new char[]
			{
				','
			});
			for (int i = 0; i < param_vector.Length; i++)
			{
				string[] pare = param_vector[i].Split(new char[]
				{
					'='
				});
				if (pare[0].Trim() == "rnd")
				{
					int.TryParse(pare[1].Trim(), out serverRND);
					if (serverRND == 0)
					{
						serverRND = -4123;
					}
				}
				if (pare[0].Trim() == "soc")
				{
					social_name = pare[1].Trim();
				}
				if (pare[0].Trim() == "player")
				{
					player_id = pare[1].Trim();
				}
				if (pare[0].Trim() == "name")
				{
					ProfileINI.nickname = pare[1].Trim();
					this.CheckName(ProfileINI.nickname);
				}
				if (pare[0].Trim() == "crystal")
				{
					ProfileINI.money[1] = Convert.ToInt32(pare[1].Trim());
				}
				if (pare[0].Trim() == "coins")
				{
					ProfileINI.money[0] = Convert.ToInt32(pare[1].Trim());
				}
				if (pare[0].Trim() == "st")
				{
					ProfileINI.server_time = Utils.StringToDateTime(pare[1].Trim());
				}
				if (pare[0].Trim() == "skin")
				{
					ProfileINI.skin = int.Parse(pare[1].Trim());
				}
				if (pare[0].Length > 3 && pare[0].Substring(0, 3) == "inv")
				{
					string[] inv_param = pare[1].Split(new char[]
					{
						'\t'
					});
					StorePurchase pe = (StorePurchase)Convert.ToInt32(pare[0].Substring(3));
					int pi = Convert.ToInt32(inv_param[0]);
					ProfileINI.SetPurchaseValue(pe, pi);
					if (pi > 0 && Store.Purchases.ContainsKey(pe))
					{
						Store.Pay pay = Store.Purchases[pe].Cost;
						if (pay is Store.TimedPay)
						{
							DateTime time_end = Utils.StringToDateTime(inv_param[1]);
							ProfileINI.SetPurchaseTime(pe, time_end);
						}
					}
				}
				if (pare[0].Trim() == "experience")
				{
					ProfileINI.experience = Convert.ToInt32(pare[1].Trim());
				}
				if (pare[0].Trim().StartsWith("slot"))
				{
					int slotID = int.Parse(pare[0].Trim().Substring(4));
					ProfileINI.slotMapNames[slotID] = pare[1].Trim();
				}
				if (pare[0].Trim() == "ply_setting")
				{
					if (pare[1] != string.Empty && pare[1] != "0")
					{
						string[] settingData = pare[1].Split(new char[]
						{
							';'
						});
						ProfileINI.mouse_sens = float.Parse(settingData[0]);
						ProfileINI.sound_volume = float.Parse(settingData[1]);
						ProfileINI.ambient_volume = float.Parse(settingData[2]);
						if (settingData.Length > 3)
						{
							ProfileINI.showNonFreeServer = (float.Parse(settingData[3]) > 0f);
						}
						else
						{
							ProfileINI.showNonFreeServer = true;
						}
					}
					else
					{
						ProfileINI.mouse_sens = 4f;
						ProfileINI.sound_volume = 0.8f;
						ProfileINI.ambient_volume = 0.5f;
						ProfileINI.showNonFreeServer = true;
					}
				}
				if (pare[0].Trim() == "weapon_skin" && pare[1] != string.Empty && pare[1] != "0")
				{
					ProfileINI.WeaponSkinData.Load(pare[1]);
				}
			}
			bool profile_is_valid = true;
			if (serverRND != randomI)
			{
				profile_is_valid = false;
			}
			if (social_name == "vk" && this.CurPlatform != App.Platform.VK)
			{
				profile_is_valid = false;
			}
			if (social_name == "ws" && this.CurPlatform != App.Platform.WEBSITE)
			{
				profile_is_valid = false;
			}
			if (social_name == "steam" && this.CurPlatform != App.Platform.STEAM)
			{
				profile_is_valid = false;
			}
			if (social_name != "vk" && social_name != "ws" && social_name != "ok" && social_name != "mm" && social_name != "steam" && social_name != "fb")
			{
				profile_is_valid = false;
			}
			if (VKAPI.INSTANCE._viewerId != player_id)
			{
				profile_is_valid = false;
			}
			if (profile_is_valid)
			{
				if (ProfileINI.nickname == string.Empty || ProfileINI.nickname == "Копатель" || ProfileINI.nickname == VKAPI.INSTANCE._viewerId)
				{
					MainMenu.Instance.HideLoading();
					MainMenu.Instance.SwitchMenu(Menu.ChangeName, false, null);
				}
				else if (from_loading)
				{
					ProfileINI.LoadFromPrefs();
					this.CheckTimePurchases();
					this.ConnectToShard(true);
				}
			}
			base.StartCoroutine(Levels.Instance.SetLevel());
			Bar.Instance.SetCoins();
		}
		else
		{
			UnityEngine.Debug.Log("Reset");
			ProfileINI.Reset();
			ProfileINI.Save();
			MainMenu.Instance.HideLoading();
			MainMenu.Instance.SwitchMenu(Menu.ChangeName, false, null);
		}
		yield break;
	}

	private void CheckName(string name)
	{
		if (!Regex.IsMatch(name, "^[a-zA-Zа-яА-Я0-9_-]{3,12}$"))
		{
			string name2 = Regex.Replace(name, "[^a-zA-Zа-яА-Я0-9_-]", string.Empty);
			MainMenu.Instance.ChangeNameProcessOut(name2);
		}
	}

	public static App Instance;

	public static bool FromLauncher;

	public static bool IsCheckAllIn;

	public App.Platform CurPlatform;

	internal string GameVersion = "202";

	internal GameINI Settings;

	internal System.Random Rnd = new System.Random();

	internal AntiMat AntiMatSystem = new AntiMat();

	internal int[] PlatformShardCount = new int[]
	{
		1,
		1,
		2,
		3,
		2,
		2
	};

	internal int MaxShardPopulation = 900;

	internal int[] ShardPopulation = new int[14];

	internal int CurrentShard;

	private float _WaitingShardPopulationTime;

	private bool _WaitingShardPopulation;

	private int _CurShardTest;

	private bool _AppQuit;

	private VkApi vkapi;

	private float last_realtime;

	private DateTime last_datetime;

	private DateTime olddt;

	public enum Platform
	{
		VK,
		WEBSITE,
		FACEBOOK,
		STEAM
	}

	public class AuthCodeJson
	{
		public int result;

		public string auth_code;
	}
}
