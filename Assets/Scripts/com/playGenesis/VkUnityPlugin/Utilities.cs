using System;
using System.Collections.Generic;
using com.playGenesis.VkUnityPlugin.MiniJSON;

namespace com.playGenesis.VkUnityPlugin
{
	public class Utilities
	{
		public static string GenerateFullHttpReqString(string request)
		{
			VkSettings vkSetts = VkApi.VkSetts;
			VKToken currentToken = VkApi.CurrentToken;
			string text;
			if (request.EndsWith("?"))
			{
				text = string.Concat(new string[]
				{
					request,
					"v=",
					vkSetts.apiVersion,
					"&access_token=",
					currentToken.access_token,
					"&https=1"
				});
			}
			else
			{
				text = string.Concat(new string[]
				{
					request,
					"&v=",
					vkSetts.apiVersion,
					"&access_token=",
					currentToken.access_token,
					"&https=1"
				});
			}
			text = Uri.EscapeUriString(text);
			while (text.Contains("#"))
			{
				text = text.Replace("#", "%23");
			}
			return VkApi.VkApiInstance.VkRequestUrlBase + text;
		}

		public static Dictionary<string, string> ParseUrlParams(string fullurl)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string text = fullurl.Split(new char[]
			{
				'#'
			})[1];
			string[] array = text.Split(new char[]
			{
				'&'
			});
			foreach (string text2 in array)
			{
				string key = text2.Split(new char[]
				{
					'='
				})[0];
				string value = text2.Split(new char[]
				{
					'='
				})[1];
				dictionary.Add(key, value);
			}
			return dictionary;
		}

		public static string ParseConfirmationUrl(string response)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(response) as Dictionary<string, object>;
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["error"];
			return (string)dictionary2["redirect_uri"];
		}

		public static string ParseConfirmationText(string response)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(response) as Dictionary<string, object>;
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["error"];
			return (string)dictionary2["confirmation_text"];
		}
	}
}
