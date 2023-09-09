using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKPrivacy
	{
		public static VKPrivacy Deserialize(object Privacy)
		{
			VKPrivacy vkprivacy = new VKPrivacy();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Privacy;
			object obj;
			if (dictionary.TryGetValue("type", out obj))
			{
				vkprivacy.type = (string)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("users", out obj2))
			{
				vkprivacy.users = new List<long>();
				foreach (object obj3 in ((List<object>)obj2))
				{
					vkprivacy.users.Add((long)obj3);
				}
			}
			object obj4;
			if (dictionary.TryGetValue("lists", out obj4))
			{
				vkprivacy.lists = new List<long>();
				foreach (object obj5 in ((List<object>)obj4))
				{
					vkprivacy.lists.Add((long)obj5);
				}
			}
			object obj6;
			if (dictionary.TryGetValue("except_lists", out obj6))
			{
				vkprivacy.except_lists = new List<long>();
				foreach (object obj7 in ((List<object>)obj6))
				{
					vkprivacy.except_lists.Add((long)obj7);
				}
			}
			object obj8;
			if (dictionary.TryGetValue("except_users", out obj8))
			{
				vkprivacy.except_users = new List<long>();
				foreach (object obj9 in ((List<object>)obj8))
				{
					vkprivacy.except_users.Add((long)obj9);
				}
			}
			return vkprivacy;
		}

		public string type { get; set; }

		public List<long> users { get; set; }

		public List<long> lists { get; set; }

		public List<long> except_lists { get; set; }

		public List<long> except_users { get; set; }
	}
}
