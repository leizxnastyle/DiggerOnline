using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKDocument
	{
		public static VKDocument Deserialize(object doc)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)doc;
			VKDocument vkdocument = new VKDocument();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkdocument.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("owner_id", out obj2))
			{
				vkdocument.owner_id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("title", out obj3))
			{
				vkdocument.title = (string)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("size", out obj4))
			{
				vkdocument.size = (long)obj;
			}
			object obj5;
			if (dictionary.TryGetValue("ext", out obj5))
			{
				vkdocument.ext = (string)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("url", out obj6))
			{
				vkdocument.url = (string)obj6;
			}
			object obj7;
			if (dictionary.TryGetValue("photo_100", out obj7))
			{
				vkdocument.photo_100 = (string)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("photo_130", out obj8))
			{
				vkdocument.photo_130 = (string)obj8;
			}
			return vkdocument;
		}

		public long id { get; set; }

		public long owner_id { get; set; }

		private string title { get; set; }

		public long size { get; set; }

		public string ext { get; set; }

		public string url { get; set; }

		public string photo_100 { get; set; }

		public string photo_130 { get; set; }
	}
}
