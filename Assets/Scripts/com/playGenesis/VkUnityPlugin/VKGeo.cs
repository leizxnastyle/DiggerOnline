using System;
using System.Collections.Generic;

namespace com.playGenesis.VkUnityPlugin
{
	public class VKGeo
	{
		public static VKGeo Deserialize(object geo)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)geo;
			VKGeo vkgeo = new VKGeo();
			object obj;
			if (dictionary.TryGetValue("type", out obj))
			{
				vkgeo.type = (string)obj;
			}
			object obj2;
			if (dictionary.TryGetValue("coordinates", out obj2))
			{
				vkgeo.coordinates = (string)obj2;
			}
			object place;
			if (dictionary.TryGetValue("place", out place))
			{
				vkgeo.place = Place.Deserialize(place);
			}
			return vkgeo;
		}

		public string type { get; set; }

		public string coordinates { get; set; }

		public Place place { get; set; }
	}
}
