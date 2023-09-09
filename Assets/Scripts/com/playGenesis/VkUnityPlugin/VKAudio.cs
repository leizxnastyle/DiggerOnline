using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKAudio
	{
		public static VKAudio Deserialize(object Audio)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Audio;
			VKAudio vkaudio = new VKAudio();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkaudio.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("owner_id", out obj2))
			{
				vkaudio.owner_id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("artist", out obj3))
			{
				vkaudio.artist = (string)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("title", out obj4))
			{
				vkaudio.title = (string)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("duration", out obj5))
			{
				vkaudio.duration = (int)((long)obj5);
			}
			object obj6;
			if (dictionary.TryGetValue("url", out obj6))
			{
				vkaudio.url = (string)obj6;
			}
			object obj7;
			if (dictionary.TryGetValue("lyrics_id", out obj7))
			{
				vkaudio.lyrics_id = (long)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("album_id", out obj8))
			{
				vkaudio.album_id = (long)obj8;
			}
			object obj9;
			if (dictionary.TryGetValue("genre_id", out obj9))
			{
				vkaudio.genre_id = (long)obj9;
			}
			return vkaudio;
		}

		public long id { get; set; }

		public long owner_id { get; set; }

		public string artist { get; set; }

		public string title { get; set; }

		public int duration { get; set; }

		public string url { get; set; }

		public long lyrics_id { get; set; }

		public long album_id { get; set; }

		public long genre_id { get; set; }
	}
}
