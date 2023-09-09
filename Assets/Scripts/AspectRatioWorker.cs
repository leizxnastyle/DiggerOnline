using System;
using UnityEngine;

public static class AspectRatioWorker
{
	public static Vector2 GetAspectRatio(int x, int y)
	{
		float num = (float)x / (float)y;
		int num2 = 0;
		do
		{
			num2++;
		}
		while (Math.Round((double)(num * (float)num2), 2) != (double)Mathf.RoundToInt(num * (float)num2));
		return new Vector2((float)Math.Round((double)(num * (float)num2), 2), (float)num2);
	}

	public static Vector2 GetAspectRatio(Vector2 xy)
	{
		float num = xy.x / xy.y;
		int num2 = 0;
		do
		{
			num2++;
		}
		while (Math.Round((double)(num * (float)num2), 2) != (double)Mathf.RoundToInt(num * (float)num2));
		return new Vector2((float)Math.Round((double)(num * (float)num2), 2), (float)num2);
	}

	public static Vector2 GetAspectRatio(int x, int y, bool debug)
	{
		float num = (float)x / (float)y;
		int num2 = 0;
		do
		{
			num2++;
		}
		while (Math.Round((double)(num * (float)num2), 2) != (double)Mathf.RoundToInt(num * (float)num2));
		if (debug)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Aspect ratio is ",
				num * (float)num2,
				":",
				num2,
				" (Resolution: ",
				x,
				"x",
				y,
				")"
			}));
		}
		return new Vector2((float)Math.Round((double)(num * (float)num2), 2), (float)num2);
	}

	public static Vector2 GetAspectRatio(Vector2 xy, bool debug)
	{
		float num = xy.x / xy.y;
		int num2 = 0;
		do
		{
			num2++;
		}
		while (Math.Round((double)(num * (float)num2), 2) != (double)Mathf.RoundToInt(num * (float)num2));
		if (debug)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Aspect ratio is ",
				num * (float)num2,
				":",
				num2,
				" (Resolution: ",
				xy.x,
				"x",
				xy.y,
				")"
			}));
		}
		return new Vector2((float)Math.Round((double)(num * (float)num2), 2), (float)num2);
	}
}
