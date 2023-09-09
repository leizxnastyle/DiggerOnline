using System;
using System.Collections.Generic;

public class VKVariable
{
	public static VKVariable Deserialize(object variable)
	{
		VKVariable vkvariable = new VKVariable();
		Dictionary<string, object> dictionary = (Dictionary<string, object>)variable;
		object obj;
		if (dictionary.TryGetValue("key", out obj))
		{
			vkvariable.key = (string)obj;
		}
		object obj2;
		if (dictionary.TryGetValue("value", out obj2))
		{
			vkvariable.value = (string)obj2;
		}
		return vkvariable;
	}

	public string key { get; set; }

	public string value { get; set; }
}
