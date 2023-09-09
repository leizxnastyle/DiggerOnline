using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKPhoto
	{
		public static VKPhoto Deserialize(object photo)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)photo;
			VKPhoto vkphoto = new VKPhoto();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkphoto.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("album_id", out obj2))
			{
				vkphoto.album_id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("owner_id", out obj3))
			{
				vkphoto.owner_id = (long)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("user_id", out obj4))
			{
				vkphoto.user_id = (long)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("photo_75", out obj5))
			{
				vkphoto.photo_75 = (string)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("photo_130", out obj6))
			{
				vkphoto.photo_130 = (string)obj6;
			}
			object obj7;
			if (dictionary.TryGetValue("photo_604", out obj7))
			{
				vkphoto.photo_604 = (string)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("photo_807", out obj8))
			{
				vkphoto.photo_807 = (string)obj8;
			}
			object obj9;
			if (dictionary.TryGetValue("photo_1280", out obj9))
			{
				vkphoto.photo_1280 = (string)obj9;
			}
			object obj10;
			if (dictionary.TryGetValue("photo_2560", out obj10))
			{
				vkphoto.photo_2560 = (string)obj10;
			}
			object obj11;
			if (dictionary.TryGetValue("width", out obj11))
			{
				vkphoto.width = (int)((long)obj11);
			}
			object obj12;
			if (dictionary.TryGetValue("height", out obj12))
			{
				vkphoto.height = (int)((long)obj12);
			}
			object obj13;
			if (dictionary.TryGetValue("text", out obj13))
			{
				vkphoto.text = (string)obj13;
			}
			object obj14;
			if (dictionary.TryGetValue("date", out obj14))
			{
				vkphoto.date = (int)((long)obj14);
			}
			return vkphoto;
		}

		public long id { get; set; }

		public long album_id { get; set; }

		public long owner_id { get; set; }

		public long user_id { get; set; }

		public string photo_75 { get; set; }

		public string photo_130 { get; set; }

		public string photo_604 { get; set; }

		public string photo_807 { get; set; }

		public string photo_1280 { get; set; }

		public string photo_2560 { get; set; }

		public int width { get; set; }

		public int height { get; set; }

		public string text { get; set; }

		public int date { get; set; }
	}
}
