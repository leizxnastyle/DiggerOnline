using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKChat
	{
		public static VKChat Deserialize(object Chat)
		{
			VKChat vkchat = new VKChat();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Chat;
			object obj;
			if (dictionary.TryGetValue("type", out obj))
			{
				vkchat.type = (string)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("id", out obj2))
			{
				vkchat.id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("title", out obj3))
			{
				vkchat.title = (string)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("admin_id", out obj4))
			{
				vkchat.admin_id = (long)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("users", out obj5))
			{
				vkchat.users = new List<long>();
				foreach (object obj6 in ((List<object>)obj5))
				{
					vkchat.users.Add((long)obj6);
				}
			}
			object obj7;
			if (dictionary.TryGetValue("photo_100", out obj7))
			{
				vkchat.photo_100 = (string)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("photo_200", out obj8))
			{
				vkchat.photo_200 = (string)obj8;
			}
			return vkchat;
		}

		public string type { get; set; }

		public long id { get; set; }

		public string title { get; set; }

		public long admin_id { get; set; }

		public List<long> users { get; set; }

		public string photo_100 { get; set; }

		public string photo_200 { get; set; }
	}
}
