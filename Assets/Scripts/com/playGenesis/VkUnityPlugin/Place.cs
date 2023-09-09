using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class Place
	{
		public static Place Deserialize(object place)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)place;
			Place place2 = new Place();
			object obj;
			if (dictionary.TryGetValue("title", out obj))
			{
				place2.title = (string)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("address", out obj2))
			{
				place2.address = (string)obj2;
			}
			object obj3;
			if (dictionary.TryGetValue("latitude", out obj3))
			{
				place2.latitude = (double)obj3;
			}
			object obj4;
			if (dictionary.TryGetValue("longitude", out obj4))
			{
				place2.longitude = (double)obj4;
			}
			object obj5;
			if (dictionary.TryGetValue("country", out obj5))
			{
				place2.country = (string)obj5;
			}
			object obj6;
			if (dictionary.TryGetValue("icon", out obj6))
			{
				place2.icon = (string)obj6;
			}
			object obj7;
			if (dictionary.TryGetValue("type", out obj7))
			{
				place2.type = (string)obj;
			}
			object obj8;
			if (dictionary.TryGetValue("group_id", out obj8))
			{
				place2.group_id = (long)obj8;
			}
			object obj9;
			if (dictionary.TryGetValue("group_photo", out obj9))
			{
				place2.group_photo = (string)obj9;
			}
			object obj10;
			if (dictionary.TryGetValue("checkins", out obj10))
			{
				place2.checkins = (int)((long)obj10);
			}
			object obj11;
			if (dictionary.TryGetValue("updated", out obj11))
			{
				place2.updated = (long)obj11;
			}
			return place2;
		}

		public string title { get; set; }

		public string address { get; set; }

		public double latitude { get; set; }

		public double longitude { get; set; }

		public string country { get; set; }

		public string city { get; set; }

		public string icon { get; set; }

		public string type { get; set; }

		public long group_id { get; set; }

		public string group_photo { get; set; }

		public int checkins { get; set; }

		public long updated { get; set; }
	}
}
