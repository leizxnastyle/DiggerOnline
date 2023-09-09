using System;
using UnityEngine;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKToken : EventArgs
	{
		public static VKToken ParseSerializeTokenFromNaviteSdk(string message)
		{
			string[] array = message.Split(new char[]
			{
				'#'
			});
			return new VKToken
			{
				access_token = array[0],
				tokenRecievedTime = DateTime.Now,
				expires_in = ((int.Parse(array[1]) != 0) ? int.Parse(array[1]) : 999999),
				user_id = array[2]
			};
		}

		public static bool IsValidToken(VKToken ti)
		{
			return ti.tokenRecievedTime.AddSeconds((double)ti.expires_in) > DateTime.Now;
		}

		public static int TokenValidFor()
		{
			VKToken currentToken = VkApi.CurrentToken;
			return (int)(currentToken.tokenRecievedTime.AddSeconds((double)currentToken.expires_in) - DateTime.Now).TotalSeconds;
		}

		public static void ResetToken()
		{
			VkApi.CurrentToken = new VKToken
			{
				access_token = string.Empty,
				tokenRecievedTime = DateTime.Parse("1/1/1992"),
				expires_in = 1,
				user_id = string.Empty
			};
			PlayerPrefs.SetString("vkaccess_token", string.Empty);
			PlayerPrefs.SetInt("vkexpires_in", 0);
			PlayerPrefs.SetString("vkuser_id", string.Empty);
			PlayerPrefs.SetString("vktokenRecievedTime", "1/1/1992");
		}

		public void Save()
		{
			PlayerPrefs.SetString("vkaccess_token", this.access_token);
			PlayerPrefs.SetInt("vkexpires_in", this.expires_in);
			PlayerPrefs.SetString("vkuser_id", this.user_id);
			PlayerPrefs.SetString("vktokenRecievedTime", this.tokenRecievedTime.ToString());
		}

		public static VKToken LoadPersistent()
		{
			DateTime dateTime = DateTime.Parse("1/1/1990");
			DateTime.TryParse(PlayerPrefs.GetString("vktokenRecievedTime", string.Empty), out dateTime);
			return new VKToken
			{
				access_token = PlayerPrefs.GetString("vkaccess_token", string.Empty),
				expires_in = PlayerPrefs.GetInt("vkexpires_in", 0),
				tokenRecievedTime = dateTime,
				user_id = PlayerPrefs.GetString("vkuser_id", string.Empty)
			};
		}

		public static string ParseFromAuthUrl(string url)
		{
			string[] array = url.Split(new char[]
			{
				'#'
			})[1].Split(new char[]
			{
				'&'
			});
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			foreach (string text4 in array)
			{
				string a = text4.Split(new char[]
				{
					'='
				})[0];
				string text5 = text4.Split(new char[]
				{
					'='
				})[1];
				if (a == "access_token")
				{
					text = text5;
				}
				else if (a == "expires_in")
				{
					text2 = text5;
				}
				else if (a == "user_id")
				{
					text3 = text5;
				}
			}
			return string.Concat(new string[]
			{
				text,
				"#",
				text2,
				"#",
				text3
			});
		}

		public string access_token;

		public int expires_in;

		public string user_id;

		public DateTime tokenRecievedTime;
	}
}
