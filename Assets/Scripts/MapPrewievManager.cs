using System;
using System.Collections.Generic;
using UnityEngine;

public static class MapPrewievManager
{
	public static void AddMapPrewiew(string link, Texture2D t)
	{
		if (!MapPrewievManager.mapPrewiev.ContainsKey(link))
		{
			MapPrewievManager.mapPrewiev.Add(link, t);
		}
	}

	public static bool IsExist(string link)
	{
		return MapPrewievManager.mapPrewiev.ContainsKey(link);
	}

	public static Texture2D GetMapPrewiew(string link)
	{
		if (MapPrewievManager.mapPrewiev.ContainsKey(link))
		{
			return MapPrewievManager.mapPrewiev[link];
		}
		return null;
	}

	public static void ClearMapPrewiew()
	{
		MapPrewievManager.mapPrewiev.Clear();
		GC.Collect();
	}

	private static Dictionary<string, Texture2D> mapPrewiev = new Dictionary<string, Texture2D>();
}
