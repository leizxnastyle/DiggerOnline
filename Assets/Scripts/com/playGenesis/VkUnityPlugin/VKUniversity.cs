using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKUniversity
	{
		public static VKUniversity Deserialize(object University)
		{
			VKUniversity vkuniversity = new VKUniversity();
			Dictionary<string, object> dictionary = (Dictionary<string, object>)University;
			object obj;
			if (dictionary.TryGetValue("id", out obj))
			{
				vkuniversity.id = (long)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("country", out obj2))
			{
				vkuniversity.country = (long)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("city", out obj3))
			{
				vkuniversity.city = (long)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("name", out obj4))
			{
				vkuniversity.name = (string)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("faculty", out obj5))
			{
				vkuniversity.faculty = (long)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("faculty_name", out obj6))
			{
				vkuniversity.faculty_name = (string)obj6;
			}
			object obj7;
			if (dictionary.TryGetValue("chair", out obj7))
			{
				vkuniversity.chair = (long)obj7;
			}
			object obj8;
			if (dictionary.TryGetValue("chair_name", out obj8))
			{
				vkuniversity.chair_name = (string)obj8;
			}
			object obj9;
			if (dictionary.TryGetValue("graduation", out obj9))
			{
				vkuniversity.graduation = (int)((long)obj9);
			}
			return vkuniversity;
		}

		public long id { get; set; }

		public long country { get; set; }

		public long city { get; set; }

		public string name { get; set; }

		public long faculty { get; set; }

		public string faculty_name { get; set; }

		public long chair { get; set; }

		public string chair_name { get; set; }

		public int graduation { get; set; }
	}
}
