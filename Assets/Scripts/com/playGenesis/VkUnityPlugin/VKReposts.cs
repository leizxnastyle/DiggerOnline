using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKReposts
	{
		public static VKReposts Deserialize(object reposts)
		{
			VKReposts vkreposts = new VKReposts();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)reposts;
			object obj;
			if (dictionary.TryGetValue("count", out obj))
			{
				vkreposts.count = (int)((long)obj);
			}
			object obj2;
			if (dictionary.TryGetValue("user_reposted", out obj2))
			{
				vkreposts.user_reposted = (int)((long)obj2);
			}
			return vkreposts;
		}

		public int count { get; set; }

		public int user_reposted { get; set; }
	}
}
