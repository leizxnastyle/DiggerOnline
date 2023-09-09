using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

public class GooglePlayPurchase : MonoBehaviour, IStoreListener
{
	public void Start()
	{
		this.InitializePurchasing();
	}

	public void InitializePurchasing()
	{
		if (this.IsInitialized)
		{
			return;
		}
		ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay), new IPurchasingModule[0]);
		configurationBuilder.AddProduct("com.dmitrievgames.digger.500g", ProductType.Consumable);
		configurationBuilder.AddProduct("com.dmitrievgames.digger.3000g", ProductType.Consumable);
		configurationBuilder.AddProduct("com.dmitrievgames.digger.7000g", ProductType.Consumable);
		configurationBuilder.AddProduct("com.dmitrievgames.digger.12500g", ProductType.Consumable);
		configurationBuilder.AddProduct("com.dmitrievgames.digger.25000g", ProductType.Consumable);
		configurationBuilder.AddProduct("com.dmitrievgames.digger.75000g", ProductType.Consumable);
		UnityPurchasing.Initialize(this, configurationBuilder);
	}

	public bool IsInitialized
	{
		get
		{
			return GooglePlayPurchase.m_StoreController != null;
		}
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		UnityEngine.Debug.Log("Store inited");
		GooglePlayPurchase.m_StoreController = controller;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		UnityEngine.Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		UnityEngine.Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		if (!this.IsInitialized)
		{
			return PurchaseProcessingResult.Complete;
		}
		if (e.purchasedProduct == null)
		{
			UnityEngine.Debug.LogWarning("Attempted to process purchasewith unknown product. Ignoring");
			return PurchaseProcessingResult.Complete;
		}
		if (string.IsNullOrEmpty(e.purchasedProduct.receipt))
		{
			UnityEngine.Debug.LogWarning("Attempted to process purchase with no receipt: ignoring");
			return PurchaseProcessingResult.Complete;
		}
		UnityEngine.Debug.Log("Processing transaction: " + e.purchasedProduct.transactionID);
		GooglePurchase googlePurchase = GooglePurchase.FromJson(e.purchasedProduct.receipt);
		string productId = string.Empty;
		bool flag = true;
		CrossPlatformValidator crossPlatformValidator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.bundleIdentifier);
		try
		{
			IPurchaseReceipt[] array = crossPlatformValidator.Validate(e.purchasedProduct.receipt);
			UnityEngine.Debug.Log("Receipt is valid. Contents:");
			foreach (IPurchaseReceipt purchaseReceipt in array)
			{
				productId = purchaseReceipt.productID;
				UnityEngine.Debug.Log(purchaseReceipt.productID);
				UnityEngine.Debug.Log(purchaseReceipt.purchaseDate);
				UnityEngine.Debug.Log(purchaseReceipt.transactionID);
			}
		}
		catch (IAPSecurityException)
		{
			UnityEngine.Debug.Log("Invalid receipt, not unlocking content");
			flag = false;
		}
		if (flag)
		{
			base.StartCoroutine(this.ValidateServerReciept(productId, e.purchasedProduct.receipt));
		}
		return PurchaseProcessingResult.Complete;
	}

	private IEnumerator ValidateServerReciept(string productId, string receipt)
	{
		UnityEngine.Debug.Log("ValidateServerReciept");
		UnityEngine.Debug.Log(productId);
		WWWForm purchaseForm = new WWWForm();
		purchaseForm.AddField("id", VKAPI.INSTANCE._viewerId);
		purchaseForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		purchaseForm.AddField("productId", productId);
		purchaseForm.AddField("receipt", receipt);
		WWW purchase = null;
		purchase = new WWW(SettingsManager.ServerURL[0] + "PaymentsGoolgePlay.php", purchaseForm);
		yield return purchase;
		UnityEngine.Debug.Log(purchase.text);
		UnityEngine.Debug.Log(purchase.error);
		if (purchase.text.Trim() == "ok")
		{
			UnityEngine.Debug.Log("Balance Changed");
			App.Instance.OnBallanceChanged();
		}
		yield break;
	}

	public void BuyProductID(string productId)
	{
		string productId2 = string.Format("com.dmitrievgames.digger.{0}g", productId);
		if (!this.IsInitialized)
		{
			throw new Exception("IAP Service is not initialized!");
		}
		GooglePlayPurchase.m_StoreController.InitiatePurchase(productId2);
	}

	private static IStoreController m_StoreController;
}
