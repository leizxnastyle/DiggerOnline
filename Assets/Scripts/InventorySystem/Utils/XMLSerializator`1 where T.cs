using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace InventorySystem.Utils
{
	public class XMLSerializator<T> where T : class
	{
		public static string Serialize(object o)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			StringWriter stringWriter = new StringWriter();
			xmlSerializer.Serialize(stringWriter, o);
			return stringWriter.ToString();
		}

		public static T Deserialize(string data)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			StringReader textReader = new StringReader(data);
			return (T)((object)xmlSerializer.Deserialize(textReader));
		}

		public static void CopyClassData(object o_in, object o_out)
		{
			Type type = o_out.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			int num = 0;
			foreach (FieldInfo fieldInfo in fields)
			{
				try
				{
					FieldInfo field = o_in.GetType().GetField(fieldInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					fieldInfo.SetValue(o_out, field.GetValue(o_in));
				}
				catch (Exception ex)
				{
				}
				num++;
			}
		}
	}
}
