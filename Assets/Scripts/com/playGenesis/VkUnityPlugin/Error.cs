using System;
using System.Collections.Generic;
using com.playGenesis.VkUnityPlugin.MiniJSON;

namespace com.playGenesis.VkUnityPlugin
{
	[Serializable]
	public class Error : EventArgs
	{
		public static Error Deserialize(string json)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			object obj;
			if (dictionary.TryGetValue("error", out obj))
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)obj;
				Error error = new Error();
				object obj2;
				if (dictionary2.TryGetValue("error_code", out obj2))
				{
					error.error_code = ((long)obj2).ToString();
				}
				object obj3;
				if (dictionary2.TryGetValue("error_msg", out obj3))
				{
					error.error_msg = (string)obj3;
				}
				return error;
			}
			return null;
		}

		public static Error ParseSerializedFromFromNativeSdk(string errormessage)
		{
			string[] array = errormessage.Split(new char[]
			{
				'#'
			});
			return new Error
			{
				error_code = array[0],
				error_msg = array[1]
			};
		}

		public static Error ParseVkError(string resp)
		{
			if (string.IsNullOrEmpty(resp))
			{
				return new Error
				{
					error_code = "404",
					error_msg = "No connection"
				};
			}
			Error error = Error.Deserialize(resp);
			if (error != null && !string.IsNullOrEmpty(error.error_code))
			{
				return new Error
				{
					error_code = error.error_code,
					error_msg = error.error_msg
				};
			}
			return null;
		}

		public string error_code;

		public string error_msg;

		public string fullJson;
	}
}
