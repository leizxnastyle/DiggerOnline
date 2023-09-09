using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKPostSource
	{
		public static VKPostSource Deserialize(object in_data)
		{
			VKPostSource vkpostSource = new VKPostSource();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)in_data;
			object obj;
			if (dictionary.TryGetValue("platform", out obj))
			{
				vkpostSource.platform = (string)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("type", out obj2))
			{
				vkpostSource.type = (string)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("data", out obj3))
			{
				vkpostSource.data = (string)obj3;
			}
			return vkpostSource;
		}

		public string platform { get; set; }

		public string type { get; set; }

		public string data { get; set; }
	}
}
