using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;
using UnityEngine;

internal class Utils
{
	public static string MaxStringLength(string str, int length)
	{
		if (str.Length > length)
		{
			return str.Substring(0, length);
		}
		return str;
	}

	public static DateTime StringToDateTime(string str)
	{
		int value = Convert.ToInt32(str.Substring(0, 4));
		int value2 = Convert.ToInt32(str.Substring(5, 2));
		int value3 = Convert.ToInt32(str.Substring(8, 2));
		int hour = Convert.ToInt32(str.Substring(11, 2));
		int minute = Convert.ToInt32(str.Substring(14, 2));
		return new DateTime(Mathf.Clamp(value, 1, 9999), Mathf.Clamp(value2, 1, 12), Mathf.Clamp(value3, 1, 31), hour, minute, 0);
	}

	public static byte[] UnzipByte(byte[] compbytes, int new_length)
	{
		MemoryStream memoryStream = null;
		BZip2InputStream bzip2InputStream = null;
		byte[] result;
		try
		{
			memoryStream = new MemoryStream(compbytes);
			bzip2InputStream = new BZip2InputStream(memoryStream);
			byte[] array = new byte[new_length];
			bzip2InputStream.Read(array, 0, array.Length);
			bzip2InputStream.Close();
			memoryStream.Close();
			result = array;
		}
		finally
		{
			if (bzip2InputStream != null)
			{
				bzip2InputStream.Dispose();
			}
			if (memoryStream != null)
			{
				memoryStream.Dispose();
			}
		}
		return result;
	}

	public static byte[] ZipByte(byte[] buffer)
	{
		MemoryStream memoryStream = null;
		BZip2OutputStream bzip2OutputStream = null;
		byte[] result;
		try
		{
			memoryStream = new MemoryStream();
			bzip2OutputStream = new BZip2OutputStream(memoryStream);
			bzip2OutputStream.Write(buffer, 0, buffer.Length);
			bzip2OutputStream.Close();
			result = memoryStream.ToArray();
			memoryStream.Close();
		}
		finally
		{
			if (bzip2OutputStream != null)
			{
				bzip2OutputStream.Dispose();
			}
			if (memoryStream != null)
			{
				memoryStream.Dispose();
			}
		}
		return result;
	}

	public static string ZipString(string sBuffer)
	{
		MemoryStream memoryStream = null;
		BZip2OutputStream bzip2OutputStream = null;
		string result;
		try
		{
			memoryStream = new MemoryStream();
			int length = sBuffer.Length;
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.ASCII))
			{
				binaryWriter.Write(length);
				bzip2OutputStream = new BZip2OutputStream(memoryStream);
				bzip2OutputStream.Write(Encoding.ASCII.GetBytes(sBuffer), 0, sBuffer.Length);
				bzip2OutputStream.Close();
				result = Convert.ToBase64String(memoryStream.ToArray());
				memoryStream.Close();
				binaryWriter.Close();
			}
		}
		finally
		{
			if (bzip2OutputStream != null)
			{
				bzip2OutputStream.Dispose();
			}
			if (memoryStream != null)
			{
				memoryStream.Dispose();
			}
		}
		return result;
	}

	public static string UnzipString(string compbytes)
	{
		MemoryStream memoryStream = null;
		BZip2InputStream bzip2InputStream = null;
		string @string;
		try
		{
			memoryStream = new MemoryStream(Convert.FromBase64String(compbytes));
			using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.ASCII))
			{
				int num = binaryReader.ReadInt32();
				bzip2InputStream = new BZip2InputStream(memoryStream);
				byte[] array = new byte[num];
				bzip2InputStream.Read(array, 0, array.Length);
				bzip2InputStream.Close();
				memoryStream.Close();
				@string = Encoding.ASCII.GetString(array);
				binaryReader.Close();
			}
		}
		finally
		{
			if (bzip2InputStream != null)
			{
				bzip2InputStream.Dispose();
			}
			if (memoryStream != null)
			{
				memoryStream.Dispose();
			}
		}
		return @string;
	}

	public static void SetLayerRecursively(GameObject obj, int newLayer)
	{
		if (null == obj)
		{
			return;
		}
		obj.layer = newLayer;
		foreach (object obj2 in obj.transform)
		{
			Transform transform = (Transform)obj2;
			if (!(null == transform))
			{
				Utils.SetLayerRecursively(transform.gameObject, newLayer);
			}
		}
	}

	public static void PlayerPrefsSetBool(string name, bool value)
	{
		PlayerPrefs.SetInt(name, (!value) ? 0 : 1);
	}

	public static bool PlayerPrefsGetBool(string name)
	{
		return PlayerPrefs.GetInt(name) == 1;
	}

	public static bool PlayerPrefsGetBool(string name, bool defaultValue)
	{
		if (PlayerPrefs.HasKey(name))
		{
			return Utils.PlayerPrefsGetBool(name);
		}
		return defaultValue;
	}

	public static string RSAEncrypt(string message)
	{
		string result;
		try
		{
			string xmlString = "<RSAKeyValue><Modulus>t3A/x4DoMsQNVdIWe0rYmBy98tIQz+2/M5eiqL7IxS2h7L3mHLZVOHeTzDWbBGLHIx8la1oWUXPGUr/31Mgbvuc8PDXW0Oh011LWUYYPgaFLKqwvnS7T8ehNYLNkHxEeiKrtMPHSp0n6IfChKAvUZbdPs9IjAr1kO2lVNJ5Ickc=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
			RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider();
			rsacryptoServiceProvider.FromXmlString(xmlString);
			result = Convert.ToBase64String(rsacryptoServiceProvider.Encrypt(Encoding.UTF8.GetBytes(message), true));
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("Problem with RSA:\n" + ex.Message.ToString());
			result = string.Empty;
		}
		return result;
	}

	public static string RSADecrypt(string cryptedMessage)
	{
		string result;
		try
		{
			string xmlString = "<RSAKeyValue><Modulus>t3A/x4DoMsQNVdIWe0rYmBy98tIQz+2/M5eiqL7IxS2h7L3mHLZVOHeTzDWbBGLHIx8la1oWUXPGUr/31Mgbvuc8PDXW0Oh011LWUYYPgaFLKqwvnS7T8ehNYLNkHxEeiKrtMPHSp0n6IfChKAvUZbdPs9IjAr1kO2lVNJ5Ickc=</Modulus><Exponent>AQAB</Exponent><P>6Ww86GpbXesa3Ja43qW4f41C9Wvga+1NfhLHL7eAVk54ktXL1hnFcMUAVEkZ6gc5eSc0FErJ9LeRVZl+L8EA3w==</P><Q>yS5afQ2STt2M12W0X4+WjIL5eFcEIT1mCbuwxKfC+uyu9YwPGmyZ31qblGQB1v2BRx5Y4+nPutGnq97giw2zmQ==</Q><DP>dSpfaEn6kqnwUclWAmwsr9m4QnhjrcjvnNjQIqN5R9kbrJikOFO3R2ObRrNqzo3Ry35iJc1kkfGSgeUJ8e5vAQ==</DP><DQ>BrBe29eOrMaa/zJne+HnTIaySrkB7yT+doZiCzfUuMkTzCA7BFLmDtIRaC6nDQHiwX2V0QYeHQUJgSJo7tamyQ==</DQ><InverseQ>oR1DCzYfxCp47qvgv8NfR11/CbjCkWhB+IJNbA2NTsYfidMyP8l9Z1XQXhNWquwuWvI9fe8kF8Vb0MCC4i/cbA==</InverseQ><D>L9ihaiIfbo5ghBAGOSZulE565QkRxYpXotdcX/NTXsrmP1/Q05BjbhUSgeNBicVR98E5+2RGCfzOr+rOc7u2MIW328tV1KNN6CXg44AyU5tFAbtalFXmVRTKRV79n2vPnRhri4VPJj7vVqJgb4xb2lYRWcziOJialWgN2+Q3buE=</D></RSAKeyValue>";
			RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider();
			rsacryptoServiceProvider.FromXmlString(xmlString);
			byte[] bytes = rsacryptoServiceProvider.Decrypt(Convert.FromBase64String(cryptedMessage), true);
			result = Encoding.UTF8.GetString(bytes);
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.Log("Problem with RSA:\n" + ex.Message.ToString());
			result = string.Empty;
		}
		return result;
	}

	public static Vector3 ScreenToNGUIPos(Vector3 pos)
	{
		float num = 1f / UIRoot.list[0].pixelSizeAdjustment;
		return new Vector3((pos.x - (float)(Screen.width / 2)) / num, (pos.y - (float)(Screen.height / 2)) / num, 1f);
	}

	public static void Shuffle(IList list)
	{
		RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider();
		int i = list.Count;
		while (i > 1)
		{
			byte[] array = new byte[1];
			do
			{
				rngcryptoServiceProvider.GetBytes(array);
			}
			while ((int)array[0] >= i * (255 / i));
			int index = (int)array[0] % i;
			i--;
			object value = list[index];
			list[index] = list[i];
			list[i] = value;
		}
	}

	public static void SetActiveRecursively(GameObject go, bool enable)
	{
		go.SetActive(enable);
		foreach (object obj in go.transform)
		{
			Transform transform = (Transform)obj;
			Utils.SetActiveRecursively(transform.gameObject, enable);
		}
	}

	public static void SetActiveRecursively(Transform node, bool enable)
	{
		node.gameObject.SetActive(enable);
		foreach (object obj in node)
		{
			Transform node2 = (Transform)obj;
			Utils.SetActiveRecursively(node2, enable);
		}
	}

	public static void UpdateTextures(Transform node, List<Texture2D> textures)
	{
		if (node == null)
		{
			return;
		}
		if (node.GetComponent<Renderer>() != null)
		{
			foreach (Texture2D texture2D in textures)
			{
				foreach (Material material in node.GetComponent<Renderer>().materials)
				{
					if (material.mainTexture != null && material.mainTexture.name == texture2D.name)
					{
						material.mainTexture = texture2D;
					}
				}
			}
		}
		foreach (object obj in node)
		{
			Transform node2 = (Transform)obj;
			Utils.UpdateTextures(node2, textures);
		}
	}
}
