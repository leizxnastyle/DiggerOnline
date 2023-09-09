using System;
using UnityEngine;

public class GooglePurchase
{
	public static GooglePurchase FromJson(string json)
	{
		GooglePurchase googlePurchase = JsonUtility.FromJson<GooglePurchase>(json);
		googlePurchase.PayloadData = PayloadData.FromJson(googlePurchase.Payload);
		return googlePurchase;
	}

	public PayloadData PayloadData;

	public string Store;

	public string TransactionID;

	public string Payload;
}
