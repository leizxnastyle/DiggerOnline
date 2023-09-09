using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKLink
	{
		public static VKLink Deserialize(object link)
		{
			VKLink vklink = new VKLink();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)link;
			object obj;
			if (dictionary.TryGetValue("url", out obj))
			{
				vklink.url = (string)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("title", out obj2))
			{
				vklink.title = (string)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("description", out obj3))
			{
				vklink.description = (string)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("image_src", out obj4))
			{
				vklink.image_src = (string)obj4;
			}
			return vklink;
		}

		public string url { get; set; }

		public string title { get; set; }

		public string description { get; set; }

		public string image_src { get; set; }
	}
}
