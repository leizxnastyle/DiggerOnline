using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKPushSettings
	{
		public static VKPushSettings Deserialize(object settings)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)settings;
			VKPushSettings vkpushSettings = new VKPushSettings();
			object obj;
			if (dictionary.TryGetValue("disabled_until", out obj))
			{
				vkpushSettings.disabled_until = (int)((long)obj);
			}
			object obj2;
			if (dictionary.TryGetValue("sound", out obj2))
			{
				vkpushSettings.sound = (int)((long)obj2);
			}
			return vkpushSettings;
		}

		public int disabled_until { get; set; }

		public int sound { get; set; }
	}
}
