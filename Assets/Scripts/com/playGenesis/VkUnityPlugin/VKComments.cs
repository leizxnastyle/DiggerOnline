using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKComments
	{
		public static VKComments Deserialize(object comments)
		{
			VKComments vkcomments = new VKComments();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)comments;
			object obj;
			if (dictionary.TryGetValue("count", out obj))
			{
				vkcomments.count = (int)((long)obj);
			}
			object obj2;
			if (dictionary.TryGetValue("can_post", out obj2))
			{
				vkcomments.can_post = (int)((long)obj2);
			}
			return vkcomments;
		}

		public int count { get; set; }

		public int can_post { get; set; }
	}
}
