using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKUserStatus
	{
		public static VKUserStatus Deserialize(object UserStatus)
		{
			VKUserStatus vkuserStatus = new VKUserStatus();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)UserStatus;
			object obj;
			if (dictionary.TryGetValue("time", out obj))
			{
				vkuserStatus.time = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("platform", out obj2))
			{
				vkuserStatus.platform = (int)((long)obj2);
			}
			return vkuserStatus;
		}

		public long time { get; set; }

		public int platform { get; set; }
	}
}
