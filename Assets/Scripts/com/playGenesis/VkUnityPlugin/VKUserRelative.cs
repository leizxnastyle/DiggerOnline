using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKUserRelative
	{
		public static VKUserRelative Deserialize(object UserRelative)
		{
			VKUserRelative vkuserRelative = new VKUserRelative();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)UserRelative;
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkuserRelative.id = (long)((int)((long)obj));
			}
			object obj2;
			if (dictionary.TryGetValue("name", out obj2))
			{
				vkuserRelative.name = (string)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("type", out obj3))
			{
				vkuserRelative.name = (string)obj3;
			}
			return vkuserRelative;
		}

		public long id { get; set; }

		public string name { get; set; }

		public string type { get; set; }
	}
}
