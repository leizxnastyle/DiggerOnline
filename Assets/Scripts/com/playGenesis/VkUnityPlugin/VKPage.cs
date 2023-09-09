using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKPage
	{
		public static VKPage Deserialize(object page)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)page;
			VKPage vkpage = new VKPage();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkpage.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("group_id", out obj2))
			{
				vkpage.group_id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("creator_id", out obj3))
			{
				vkpage.creator_id = (long)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("title", out obj4))
			{
				vkpage.title = (string)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("current_user_can_edit", out obj5))
			{
				vkpage.current_user_can_edit = (int)((long)obj5);
			}
			object obj6;
			if (dictionary.TryGetValue("current_user_can_edit_access", out obj6))
			{
				vkpage.current_user_can_edit_access = (int)((long)obj6);
			}
			object obj7;
			if (dictionary.TryGetValue("who_can_view", out obj7))
			{
				vkpage.who_can_view = (int)((long)obj7);
			}
			object obj8;
			if (dictionary.TryGetValue("who_can_edit", out obj8))
			{
				vkpage.who_can_edit = (int)((long)obj8);
			}
			object obj9;
			if (dictionary.TryGetValue("edited", out obj9))
			{
				vkpage.edited = (int)((long)obj9);
			}
			object obj10;
			if (dictionary.TryGetValue("created", out obj10))
			{
				vkpage.created = (int)((long)obj10);
			}
			object obj11;
			if (dictionary.TryGetValue("editor_id", out obj11))
			{
				vkpage.editor_id = (long)obj11;
			}
			object obj12;
			if (dictionary.TryGetValue("views", out obj12))
			{
				vkpage.views = (int)((long)obj12);
			}
			object obj13;
			if (dictionary.TryGetValue("parent", out obj13))
			{
				vkpage.parent = (string)obj13;
			}
			object obj14;
			if (dictionary.TryGetValue("parent2", out obj14))
			{
				vkpage.parent2 = (string)obj14;
			}
			object obj15;
			if (dictionary.TryGetValue("source", out obj15))
			{
				vkpage.source = (string)obj15;
			}
			object obj16;
			if (dictionary.TryGetValue("html", out obj16))
			{
				vkpage.html = (string)obj16;
			}
			object obj17;
			if (dictionary.TryGetValue("view_url", out obj17))
			{
				vkpage.view_url = (string)obj17;
			}
			return vkpage;
		}

		public long id { get; set; }

		public long group_id { get; set; }

		public long creator_id { get; set; }

		public string title { get; set; }

		public int current_user_can_edit { get; set; }

		public int current_user_can_edit_access { get; set; }

		public int who_can_view { get; set; }

		public int who_can_edit { get; set; }

		public int edited { get; set; }

		public int created { get; set; }

		public long editor_id { get; set; }

		public int views { get; set; }

		public string parent { get; set; }

		public string parent2 { get; set; }

		public string source { get; set; }

		public string html { get; set; }

		public string view_url { get; set; }
	}
}
