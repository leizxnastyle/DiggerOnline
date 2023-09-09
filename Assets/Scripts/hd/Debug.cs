using System;
using UnityEngine;

namespace hd
{
	public static class Debug
	{
		public static void Log(object msg)
		{
			if (Debug.EnableLog)
			{
				UnityEngine.Debug.Log(msg);
			}
		}

		public static void LogWarning(object msg)
		{
			if (Debug.EnableLogWarning)
			{
				UnityEngine.Debug.LogWarning(msg);
			}
		}

		public static void LogError(object msg)
		{
			if (Debug.EnableLogError)
			{
				UnityEngine.Debug.LogError(msg);
			}
		}

		public static bool EnableLog;

		public static bool EnableLogWarning = true;

		public static bool EnableLogError = true;
	}
}
