using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKPlace
	{
		public static VKPlace Deserialize(object place)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)place;
			VKPlace vkplace = new VKPlace();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkplace.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("title", out obj2))
			{
				vkplace.title = (string)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("latitude", out obj3))
			{
				vkplace.latitude = (int)((long)obj3);
			}
			object obj4;
			if (dictionary.TryGetValue("longitude", out obj4))
			{
				vkplace.longitude = (int)((long)obj4);
			}
			object obj5;
			if (dictionary.TryGetValue("type", out obj5))
			{
				vkplace.type = (string)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("country", out obj6))
			{
				vkplace.country = (long)obj6;
			}
			object obj7;
			if (dictionary.TryGetValue("city", out obj7))
			{
				vkplace.city = (long)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("address", out obj8))
			{
				vkplace.address = (string)obj8;
			}
			return vkplace;
		}

		public long id { get; set; }

		public string title { get; set; }

		public int latitude { get; set; }

		public int longitude { get; set; }

		public string type { get; set; }

		public long country { get; set; }

		public long city { get; set; }

		public string address { get; set; }
	}
}
