using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKUserOccupation
	{
		public static VKUserOccupation Deserialize(object UserOccupation)
		{
			VKUserOccupation vkuserOccupation = new VKUserOccupation();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)UserOccupation;
			object obj;
			if (dictionary.TryGetValue("type", out obj))
			{
				vkuserOccupation.type = (string)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("id", out obj2))
			{
				vkuserOccupation.id = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("name", out obj3))
			{
				vkuserOccupation.name = (string)obj3;
			}
			return vkuserOccupation;
		}

		public string type { get; set; }

		public long id { get; set; }

		public string name { get; set; }

		public static readonly string OCCUPATION_TYPE_WORK = "work";

		public static readonly string OCCUPATION_TYPE_SCHOOL = "school";

		public static readonly string OCCUPATION_TYPE_UNIVERSITY = "university";
	}
}
