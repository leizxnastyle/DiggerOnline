using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKLikes
	{
		public static VKLikes Deserialize(object likes)
		{
			VKLikes vklikes = new VKLikes();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)likes;
			object obj;
			if (dictionary.TryGetValue("count", out obj))
			{
				vklikes.count = (int)((long)obj);
			}
			object obj2;
			if (dictionary.TryGetValue("can_publish", out obj2))
			{
				vklikes.can_publish = (int)((long)obj2);
			}
			object obj3;
			if (dictionary.TryGetValue("user_likes", out obj3))
			{
				vklikes.user_likes = (int)((long)obj3);
			}
			object obj4;
			if (dictionary.TryGetValue("can_like", out obj4))
			{
				vklikes.can_like = (int)((long)obj4);
			}
			return vklikes;
		}

		public int count { get; set; }

		public int user_likes { get; set; }

		public int can_like { get; set; }

		public int can_publish { get; set; }
	}
}
