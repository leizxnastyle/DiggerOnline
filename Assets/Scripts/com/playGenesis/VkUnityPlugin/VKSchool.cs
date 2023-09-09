using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKSchool
	{
		public static VKSchool Deserialize(object School)
		{
			VKSchool vkschool = new VKSchool();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)School;
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkschool.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("country", out obj2))
			{
				vkschool.country = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("city", out obj3))
			{
				vkschool.city = (long)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("name", out obj4))
			{
				vkschool.name = (string)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("year_from", out obj5))
			{
				vkschool.year_from = (int)((long)obj5);
			}
			object obj6;
			if (dictionary.TryGetValue("year_to", out obj6))
			{
				vkschool.year_to = (int)((long)obj6);
			}
			object obj7;
			if (dictionary.TryGetValue("year_graduated", out obj7))
			{
				vkschool.year_graduated = (int)((long)obj7);
			}
			object obj8;
			if (dictionary.TryGetValue("class", out obj8))
			{
				vkschool.@class = (string)obj8;
			}
			object obj9;
			if (dictionary.TryGetValue("speciality", out obj9))
			{
				vkschool.speciality = (string)obj9;
			}
			object obj10;
			if (dictionary.TryGetValue("type", out obj10))
			{
				vkschool.type = (long)obj10;
			}
			object obj11;
			if (dictionary.TryGetValue("type_str", out obj11))
			{
				vkschool.type_str = (string)obj11;
			}
			return vkschool;
		}

		public long id { get; set; }

		public long country { get; set; }

		public long city { get; set; }

		public string name { get; set; }

		public int year_from { get; set; }

		public int year_to { get; set; }

		public int year_graduated { get; set; }

		public string @class { get; set; }

		public string speciality { get; set; }

		public long type { get; set; }

		public string type_str { get; set; }
	}
}
