using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPDemoProductUI : MonoBehaviour
{
	public void SetProduct(Product p, Action<string> purchaseCallback)
	{
		this.titleText.text = p.metadata.localizedTitle;
		this.descriptionText.text = p.metadata.localizedDescription;
		this.priceText.text = p.metadata.localizedPriceString;
		this.receiptButton.interactable = p.hasReceipt;
		this.m_Receipt = p.receipt;
		this.m_ProductID = p.definition.id;
		this.m_PurchaseCallback = purchaseCallback;
		this.statusText.text = ((!p.availableToPurchase) ? "Unavailable" : "Available");
	}

	public void SetPendingTime(int secondsRemaining)
	{
		this.statusText.text = "Pending " + secondsRemaining.ToString();
	}

	public void PurchaseButtonClick()
	{
		if (this.m_PurchaseCallback != null && !string.IsNullOrEmpty(this.m_ProductID))
		{
			this.m_PurchaseCallback(this.m_ProductID);
		}
	}

	public void ReceiptButtonClick()
	{
		if (!string.IsNullOrEmpty(this.m_Receipt))
		{
			UnityEngine.Debug.Log("Receipt for " + this.m_ProductID + ": " + this.m_Receipt);
		}
	}

	public Button purchaseButton;

	public Button receiptButton;

	public Text titleText;

	public Text descriptionText;

	public Text priceText;

	public Text statusText;

	private string m_ProductID;

	private Action<string> m_PurchaseCallback;

	private string m_Receipt;
}
