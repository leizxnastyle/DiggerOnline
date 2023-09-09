using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKUserPersonal
	{
		public static VKUserPersonal Deserialize(object UserPersonal)
		{
			VKUserPersonal vkuserPersonal = new VKUserPersonal();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)UserPersonal;
			object obj;
			if (dictionary.TryGetValue("political", out obj))
			{
				vkuserPersonal.political = (int)((long)obj);
			}
			object obj2;
			if (dictionary.TryGetValue("langs", out obj2))
			{
				vkuserPersonal.langs = new List<string>();
				foreach (object obj3 in ((List<object>)obj2))
				{
					vkuserPersonal.langs.Add((string)obj3);
				}
			}
			object obj4;
			if (dictionary.TryGetValue("religion", out obj4))
			{
				vkuserPersonal.religion = (string)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("inspired_by", out obj5))
			{
				vkuserPersonal.inspired_by = (string)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("people_main", out obj6))
			{
				vkuserPersonal.people_main = (int)((long)obj6);
			}
			object obj7;
			if (dictionary.TryGetValue("life_main", out obj7))
			{
				vkuserPersonal.life_main = (int)((long)obj7);
			}
			object obj8;
			if (dictionary.TryGetValue("smoking", out obj8))
			{
				vkuserPersonal.smoking = (int)((long)obj8);
			}
			object obj9;
			if (dictionary.TryGetValue("alcohol", out obj9))
			{
				vkuserPersonal.alcohol = (int)((long)obj9);
			}
			return vkuserPersonal;
		}

		public int political { get; set; }

		public List<string> langs { get; set; }

		public string religion { get; set; }

		public string inspired_by { get; set; }

		public int people_main { get; set; }

		public int life_main { get; set; }

		public int smoking { get; set; }

		public int alcohol { get; set; }
	}
}
