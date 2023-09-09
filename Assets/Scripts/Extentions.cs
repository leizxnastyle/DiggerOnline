using System;

public static class Extentions
{
	public static string safe(this string str)
	{
		if (str == null)
		{
			return string.Empty;
		}
		return str;
	}
}
