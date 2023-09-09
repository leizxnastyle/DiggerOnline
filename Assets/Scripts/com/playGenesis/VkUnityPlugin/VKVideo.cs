using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKVideo
	{
		public static VKVideo Deserialize(object video)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)video;
			VKVideo vkvideo = new VKVideo();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkvideo.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("owner_id", out obj2))
			{
				vkvideo.owner_id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("title", out obj3))
			{
				vkvideo.title = (string)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("duration", out obj4))
			{
				vkvideo.duration = (int)((long)obj4);
			}
			object obj5;
			if (dictionary.TryGetValue("date", out obj5))
			{
				vkvideo.date = (int)((long)obj5);
			}
			object obj6;
			if (dictionary.TryGetValue("views", out obj6))
			{
				vkvideo.views = (int)((long)obj6);
			}
			object obj7;
			if (dictionary.TryGetValue("photo_130", out obj7))
			{
				vkvideo.photo_130 = (string)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("photo_320", out obj8))
			{
				vkvideo.photo_320 = (string)obj8;
			}
			object obj9;
			if (dictionary.TryGetValue("photo_640", out obj9))
			{
				vkvideo.photo_640 = (string)obj9;
			}
			object obj10;
			if (dictionary.TryGetValue("player", out obj10))
			{
				vkvideo.player = (string)obj10;
			}
			if (dictionary.TryGetValue("id", out obj))
			{
				vkvideo.id = (long)obj;
			}
			return vkvideo;
		}

		public long id { get; set; }

		public long owner_id { get; set; }

		public string title { get; set; }

		public int duration { get; set; }

		public string description { get; set; }

		public int date { get; set; }

		public int views { get; set; }

		public string photo_130 { get; set; }

		public string photo_320 { get; set; }

		public string photo_640 { get; set; }

		public string player { get; set; }
	}
}
