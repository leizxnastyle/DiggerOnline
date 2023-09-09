using System;
using System.Security.Cryptography;
using System.Text;

public class ProtectHash
{
	public static string GetHash(MD5 Hash, string Input)
	{
		byte[] array = Hash.ComputeHash(Encoding.UTF8.GetBytes(Input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}
}
