using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class VKAPI : MonoBehaviour
{
	public bool IsInit
	{
		get
		{
			return this._isInit;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (VKAPI.INSTANCE == null)
		{
			VKAPI.INSTANCE = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
	}

	public string GetSecret()
	{
		return Crypto.DHJKVS("EAAAABYXupiVGpvNWUt0v1V1Q2iD+tP2HYFVYKVlxqP3nrijFud0DcHZEEUQLhPbBGVvxQ==", "5mXfvdih");
	}

	public void SetApiUrl(string url)
	{
		this._apiUrl = url + ((!url.EndsWith("?")) ? "?" : string.Empty);
	}

	public void SetUrl(string url)
	{
		this._GameUrl = url;
		this._mapID = this.GetUrlInt(url, "map_id_", string.Empty);
		this._gameID = this.GetUrlString(url, "game_id_", string.Empty);
	}

	private string GetUrlString(string url, string param, string end = "")
	{
		int num = url.IndexOf(param);
		if (num == -1)
		{
			return string.Empty;
		}
		if (end == string.Empty)
		{
			return url.Substring(num + param.Length);
		}
		int num2 = url.IndexOf(end, num);
		return url.Substring(num + param.Length, num2 - (num + param.Length));
	}

	private int GetUrlInt(string url, string param, string end = "")
	{
		int result = 0;
		if (int.TryParse(this.GetUrlString(url, param, end), out result))
		{
			return result;
		}
		return 0;
	}

	public void SetAppId(string appId)
	{
		this._appId = appId;
	}

	public void SetSessionKey(string key)
	{
		this._sessionKey = key;
	}

	public void SetAuthKey(string key)
	{
		this._authKey = key;
	}

	public void SetSettings(int Settings)
	{
		this._Settings = Settings;
		UnityEngine.Debug.Log("Settings" + this._Settings);
	}

	public void SetViewerId(string viewerId)
	{
		this._viewerId = viewerId;
	}

	public void SetSocial(string social)
	{
		App.Instance.CurPlatform = App.Platform.VK;
	}

	public void SetLocalize(string lang)
	{
		if (lang == "RU")
		{
			Localize.Locale = Localize.LocaleType.RU;
			KGUI.SetLocale(Localize.LocaleType.RU);
			KGUI.SetNodes("welcome.language_ru", Localize.Locale == Localize.LocaleType.RU, false);
			KGUI.SetNodes("welcome.language_en", Localize.Locale == Localize.LocaleType.EN, false);
		}
		else if (lang == "EN")
		{
			Localize.Locale = Localize.LocaleType.EN;
			KGUI.SetLocale(Localize.LocaleType.EN);
			KGUI.SetNodes("welcome.language_ru", Localize.Locale == Localize.LocaleType.RU, false);
			KGUI.SetNodes("welcome.language_en", Localize.Locale == Localize.LocaleType.EN, false);
		}
	}

	public void ApiInitialized(int a)
	{
		UnityEngine.Debug.Log("ApiInitialized");
		this._isInit = true;
		MainMenu.Instance.SetLoadingText("LOADING_PROFILE", string.Empty);
		base.StartCoroutine(App.Instance.LoadProfile());
	}

	public static string Md5(string value)
	{
		MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = Encoding.ASCII.GetBytes(value);
		array = md5CryptoServiceProvider.ComputeHash(array);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString("x2").ToLower();
		}
		return text;
	}

	public static string MD52(string password)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		string result;
		try
		{
			MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = md5CryptoServiceProvider.ComputeHash(bytes);
			string text = string.Empty;
			foreach (byte b in array)
			{
				if (b < 16)
				{
					text = text + "0" + b.ToString("x");
				}
				else
				{
					text += b.ToString("x");
				}
			}
			result = text;
		}
		catch
		{
			throw;
		}
		return result;
	}

	public void ToggleFullscreen()
	{
		UnityEngine.Debug.Log("ToggleFullscreen --- >");
		ProfileINI.ToggleFullScreen();
	}

	public static VKAPI INSTANCE;

	public string VK_API_SECRET = string.Empty;

	public bool TestMode = true;

	public string Version = "5.0";

	[NonSerialized]
	public int _mapID;

	[NonSerialized]
	public string _gameID = string.Empty;

	public string _roomId = string.Empty;

	private string _apiUrl = string.Empty;

	private string _GameUrl = string.Empty;

	public string _viewerId = string.Empty;

	public int _Settings;

	public string _authKey = string.Empty;

	[NonSerialized]
	public string _sessionKey = string.Empty;

	private string _appId = string.Empty;

	private bool _isInit;

	public struct RequestParameter
	{
		public RequestParameter(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}

		public string ToString()
		{
			return this.Name + "=" + this.Value;
		}

		public string Name;

		public string Value;
	}
}
