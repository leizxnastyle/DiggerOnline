using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKAlbum
	{
		public static VKAlbum Deserialize(object Album)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Album;
			VKAlbum vkalbum = new VKAlbum();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkalbum.id = (string)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("thumb_id", out obj2))
			{
				vkalbum.thumb_id = (string)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("owner_id", out obj3))
			{
				vkalbum.owner_id = (string)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("title", out obj4))
			{
				vkalbum.title = (string)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("description", out obj5))
			{
				vkalbum.description = (string)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("created", out obj6))
			{
				vkalbum.created = (string)obj6;
			}
			object obj7;
			if (dictionary.TryGetValue("updated", out obj7))
			{
				vkalbum.updated = (string)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("size", out obj8))
			{
				vkalbum.size = (int)((long)obj8);
			}
			object obj9;
			if (dictionary.TryGetValue("thumb_src", out obj9))
			{
				vkalbum.thumb_src = (string)obj9;
			}
			object privacy;
			if (dictionary.TryGetValue("privacy_view", out privacy))
			{
				vkalbum.privacy_view = VKPrivacy.Deserialize(privacy);
			}
			object privacy2;
			if (dictionary.TryGetValue("privacy_comment", out privacy2))
			{
				vkalbum.privacy_comment = VKPrivacy.Deserialize(privacy2);
			}
			return vkalbum;
		}

		public string id { get; set; }

		public string thumb_id { get; set; }

		public string owner_id { get; set; }

		public string title { get; set; }

		public string description { get; set; }

		public string created { get; set; }

		public string updated { get; set; }

		public int size { get; set; }

		public string thumb_src { get; set; }

		public VKPrivacy privacy_view { get; set; }

		public VKPrivacy privacy_comment { get; set; }
	}
}
