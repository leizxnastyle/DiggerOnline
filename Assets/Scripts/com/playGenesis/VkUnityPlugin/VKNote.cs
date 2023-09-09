using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKNote
	{
		public static VKNote Deserialize(object note)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)note;
			VKNote vknote = new VKNote();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vknote.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("user_id", out obj2))
			{
				vknote.user_id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("owner_id", out obj3))
			{
				vknote.owner_id = (long)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("text", out obj4))
			{
				vknote.text = (string)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("title", out obj5))
			{
				vknote.title = (string)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("comments", out obj6))
			{
				vknote.comments = (int)((long)obj6);
			}
			object obj7;
			if (dictionary.TryGetValue("read_comments", out obj7))
			{
				vknote.read_comments = (int)((long)obj7);
			}
			object obj8;
			if (dictionary.TryGetValue("view_url", out obj8))
			{
				vknote.view_url = (string)obj8;
			}
			return vknote;
		}

		public long id { get; set; }

		public long user_id { get; set; }

		public long owner_id
		{
			get
			{
				return this.user_id;
			}
			set
			{
				this.user_id = value;
			}
		}

		public string title { get; set; }

		public string text { get; set; }

		public int comments { get; set; }

		public int read_comments { get; set; }

		public string view_url { get; set; }
	}
}
