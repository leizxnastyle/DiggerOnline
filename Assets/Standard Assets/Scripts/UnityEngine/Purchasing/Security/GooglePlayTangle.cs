using System;

namespace UnityEngine.Purchasing.Security
{
	public class GooglePlayTangle
	{
		public static byte[] Data()
		{
			if (!GooglePlayTangle.IsPopulated)
			{
				return null;
			}
			return Obfuscator.DeObfuscate(GooglePlayTangle.data, GooglePlayTangle.order, GooglePlayTangle.key);
		}

		private static byte[] data = Convert.FromBase64String("+/vpcMrnKaU/a5J2SGSzLJvFwqw6XV98/jiPSYoKARaZ/RCE0uP6HhZ6MurkswAkbjVUXzNY8CXzA94FDM6PEV7+44hQUIq5RIb8NX/q6c+QgaMkR8N0ulXKbnRS1EjhRe5tThuYlpmpG5iTmxuYmJk4S4YzlINVhMgvcGyRb5dWoakgf/zIMYvz9jmYfRE5aNFtkyRA/ozF22neOrkjK5DlPbBUknFilJRz0U69c4jPFyb5kexPnL6EQvNF5MwEfaS5RJjSZxzLUP1RLl4Oj1TH6J37eTnc/VjriQo/I+OZDVrUDBbu+mIA0tZE+46YsCz7uYgj7hQJKrqKNrOZDAMsg4ipG5i7qZSfkLMf0R9ulJiYmJyZmm3DNbEiRO9T4JuamJmY");

		private static int[] order = new int[]
		{
			2,
			8,
			10,
			12,
			12,
			8,
			13,
			12,
			9,
			13,
			12,
			11,
			13,
			13,
			14
		};

		private static int key = 153;

		public static readonly bool IsPopulated = true;
	}
}
