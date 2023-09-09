using System;
using UnityEngine;

public class PayloadData
{
	public static PayloadData FromJson(string json)
	{
		PayloadData payloadData = JsonUtility.FromJson<PayloadData>(json);
		payloadData.JsonData = JsonUtility.FromJson<JsonData>(payloadData.json);
		return payloadData;
	}

	public JsonData JsonData;

	public string signature;

	public string json;
}
