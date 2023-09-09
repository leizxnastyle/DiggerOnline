using System;

namespace UnityEngine.Purchasing.Security
{
	public class UnityChannelTangle
	{
		public static byte[] Data()
		{
			if (!UnityChannelTangle.IsPopulated)
			{
				return null;
			}
			return Obfuscator.DeObfuscate(UnityChannelTangle.data, UnityChannelTangle.order, UnityChannelTangle.key);
		}

		private static byte[] data = Convert.FromBase64String(string.Empty);

		private static int[] order = new int[0];

		private static int key = 0;

		public static readonly bool IsPopulated = false;
	}
}
