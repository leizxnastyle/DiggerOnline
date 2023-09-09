using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKUserExports
	{
		public static VKUserExports Deserialize(object UserExports)
		{
			VKUserExports vkuserExports = new VKUserExports();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)UserExports;
			object obj;
			if (dictionary.TryGetValue("twitter", out obj))
			{
				vkuserExports.twitter = (int)((long)obj);
			}
			object obj2;
			if (dictionary.TryGetValue("facebook", out obj2))
			{
				vkuserExports.facebook = (int)((long)obj2);
			}
			object obj3;
			if (dictionary.TryGetValue("livejournal", out obj3))
			{
				vkuserExports.livejournal = (int)((long)obj3);
			}
			object obj4;
			if (dictionary.TryGetValue("instagram", out obj4))
			{
				vkuserExports.instagram = (int)((long)obj4);
			}
			return vkuserExports;
		}

		public int twitter { get; set; }

		public int facebook { get; set; }

		public int livejournal { get; set; }

		public int instagram { get; set; }
	}
}
