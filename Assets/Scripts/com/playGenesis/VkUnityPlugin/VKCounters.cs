using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKCounters
	{
		public static VKCounters Deserialize(object Countries)
		{
			VKCounters vkcounters = new VKCounters();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Countries;
			object obj;
			if (dictionary.TryGetValue("albums", out obj))
			{
				vkcounters.albums = (int)((long)obj);
			}
			object obj2;
			if (dictionary.TryGetValue("videos", out obj2))
			{
				vkcounters.videos = (int)((long)obj2);
			}
			object obj3;
			if (dictionary.TryGetValue("audios", out obj3))
			{
				vkcounters.audios = (int)((long)obj3);
			}
			object obj4;
			if (dictionary.TryGetValue("notes", out obj4))
			{
				vkcounters.notes = (int)((long)obj4);
			}
			object obj5;
			if (dictionary.TryGetValue("groups", out obj5))
			{
				vkcounters.groups = (int)((long)obj5);
			}
			object obj6;
			if (dictionary.TryGetValue("photos", out obj6))
			{
				vkcounters.photos = (int)((long)obj6);
			}
			object obj7;
			if (dictionary.TryGetValue("friends", out obj7))
			{
				vkcounters.friends = (int)((long)obj7);
			}
			object obj8;
			if (dictionary.TryGetValue("online_friends", out obj8))
			{
				vkcounters.online_friends = (int)((long)obj8);
			}
			object obj9;
			if (dictionary.TryGetValue("mutual_friends", out obj9))
			{
				vkcounters.mutual_friends = (int)((long)obj9);
			}
			object obj10;
			if (dictionary.TryGetValue("user_videos", out obj10))
			{
				vkcounters.user_videos = (int)((long)obj10);
			}
			object obj11;
			if (dictionary.TryGetValue("user_photos", out obj11))
			{
				vkcounters.user_photos = (int)((long)obj11);
			}
			object obj12;
			if (dictionary.TryGetValue("followers", out obj12))
			{
				vkcounters.followers = (int)((long)obj12);
			}
			object obj13;
			if (dictionary.TryGetValue("subscriptions", out obj13))
			{
				vkcounters.subscriptions = (int)((long)obj13);
			}
			object obj14;
			if (dictionary.TryGetValue("topics", out obj14))
			{
				vkcounters.topics = (int)((long)obj14);
			}
			object obj15;
			if (dictionary.TryGetValue("docs", out obj15))
			{
				vkcounters.docs = (int)((long)obj15);
			}
			object obj16;
			if (dictionary.TryGetValue("pages", out obj16))
			{
				vkcounters.pages = (int)((long)obj16);
			}
			return vkcounters;
		}

		public int albums { get; set; }

		public int videos { get; set; }

		public int audios { get; set; }

		public int notes { get; set; }

		public int groups { get; set; }

		public int photos { get; set; }

		public int friends { get; set; }

		public int online_friends { get; set; }

		public int mutual_friends { get; set; }

		public int user_videos { get; set; }

		public int user_photos { get; set; }

		public int followers { get; set; }

		public int subscriptions { get; set; }

		public int topics { get; set; }

		public int docs { get; set; }

		public int pages { get; set; }
	}
}
