using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKGroup
	{
		public static VKGroup Deserialise(object group)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)group;
			VKGroup vkgroup = new VKGroup();
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkgroup.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("name", out obj2))
			{
				vkgroup.name = (string)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("screen_name", out obj3))
			{
				vkgroup.screen_name = (string)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("is_closed", out obj4))
			{
				vkgroup.is_closed = (int)((long)obj4);
			}
			object obj5;
			if (dictionary.TryGetValue("deactivated", out obj5))
			{
				vkgroup.deactivated = (string)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("is_admin", out obj6))
			{
				vkgroup.is_admin = (int)((long)obj6);
			}
			object obj7;
			if (dictionary.TryGetValue("admin_level", out obj7))
			{
				vkgroup.admin_level = (int)((long)obj7);
			}
			object obj8;
			if (dictionary.TryGetValue("is_member", out obj8))
			{
				vkgroup.is_member = (int)((long)obj8);
			}
			object obj9;
			if (dictionary.TryGetValue("type", out obj9))
			{
				vkgroup.type = (string)obj9;
			}
			object obj10;
			if (dictionary.TryGetValue("photo_50", out obj10))
			{
				vkgroup.photo_50 = (string)obj10;
			}
			object obj11;
			if (dictionary.TryGetValue("photo_100", out obj11))
			{
				vkgroup.photo_100 = (string)obj11;
			}
			object obj12;
			if (dictionary.TryGetValue("photo_200", out obj12))
			{
				vkgroup.photo_200 = (string)obj12;
			}
			object obj13;
			if (dictionary.TryGetValue("city", out obj13))
			{
				vkgroup.city = (long)obj13;
			}
			object obj14;
			if (dictionary.TryGetValue("country", out obj14))
			{
				vkgroup.country = (long)obj14;
			}
			object place;
			if (dictionary.TryGetValue("place", out place))
			{
				vkgroup.place = VKPlace.Deserialize(place);
			}
			object obj15;
			if (dictionary.TryGetValue("description", out obj15))
			{
				vkgroup.description = (string)obj15;
			}
			object obj16;
			if (dictionary.TryGetValue("wiki_page", out obj16))
			{
				vkgroup.wiki_page = (string)obj16;
			}
			object obj17;
			if (dictionary.TryGetValue("members_count", out obj17))
			{
				vkgroup.members_count = (int)((long)obj17);
			}
			object countries;
			if (dictionary.TryGetValue("counters", out countries))
			{
				vkgroup.counters = VKCounters.Deserialize(countries);
			}
			object obj18;
			if (dictionary.TryGetValue("start_date", out obj18))
			{
				vkgroup.start_date = (long)obj18;
			}
			object obj19;
			if (dictionary.TryGetValue("finish_date", out obj19))
			{
				vkgroup.finish_date = (long)obj19;
			}
			object obj20;
			if (dictionary.TryGetValue("can_post", out obj20))
			{
				vkgroup.can_post = (int)((long)obj20);
			}
			object obj21;
			if (dictionary.TryGetValue("can_see_all_posts", out obj21))
			{
				vkgroup.can_see_all_posts = (int)((long)obj21);
			}
			object obj22;
			if (dictionary.TryGetValue("can_upload_doc", out obj22))
			{
				vkgroup.can_upload_doc = (int)((long)obj22);
			}
			object obj23;
			if (dictionary.TryGetValue("can_create_topic", out obj23))
			{
				vkgroup.can_create_topic = (int)((long)obj23);
			}
			object obj24;
			if (dictionary.TryGetValue("activity", out obj24))
			{
				vkgroup.activity = (string)obj24;
			}
			object obj25;
			if (dictionary.TryGetValue("status", out obj25))
			{
				vkgroup.status = (string)obj25;
			}
			object obj26;
			if (dictionary.TryGetValue("contacts", out obj26))
			{
				vkgroup.contacts = (string)obj26;
			}
			object obj27;
			if (dictionary.TryGetValue("links", out obj27))
			{
				vkgroup.links = (string)obj27;
			}
			object obj28;
			if (dictionary.TryGetValue("fixed_post", out obj28))
			{
				vkgroup.fixed_post = (long)obj28;
			}
			object obj29;
			if (dictionary.TryGetValue("verified", out obj29))
			{
				vkgroup.verified = (int)((long)obj29);
			}
			object obj30;
			if (dictionary.TryGetValue("site", out obj30))
			{
				vkgroup.site = (string)obj30;
			}
			return vkgroup;
		}

		public long id { get; set; }

		public string name { get; set; }

		public string screen_name { get; set; }

		public int is_closed { get; set; }

		public string deactivated { get; set; }

		public int is_admin { get; set; }

		public int admin_level { get; set; }

		public int is_member { get; set; }

		public string type { get; set; }

		public string photo_50 { get; set; }

		public string photo_100 { get; set; }

		public string photo_200 { get; set; }

		public long city { get; set; }

		public long country { get; set; }

		public VKPlace place { get; set; }

		public string description { get; set; }

		public string wiki_page { get; set; }

		public int members_count { get; set; }

		public VKCounters counters { get; set; }

		public long start_date { get; set; }

		public long finish_date { get; set; }

		public int can_post { get; set; }

		public int can_see_all_posts { get; set; }

		public int can_upload_doc { get; set; }

		public int can_create_topic { get; set; }

		public string activity { get; set; }

		public string status { get; set; }

		public string contacts { get; set; }

		public string links { get; set; }

		public long fixed_post { get; set; }

		public int verified { get; set; }

		public string site { get; set; }
	}
}
