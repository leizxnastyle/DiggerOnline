using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Store;
using UnityEngine.UI;

[AddComponentMenu("Unity IAP/Demo")]
public class IAPDemo : MonoBehaviour, IStoreListener
{
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		this.m_Controller = controller;
		this.m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
		this.m_SamsungExtensions = extensions.GetExtension<ISamsungAppsExtensions>();
		this.m_MoolahExtensions = extensions.GetExtension<IMoolahExtension>();
		this.m_MicrosoftExtensions = extensions.GetExtension<IMicrosoftExtensions>();
		this.m_UnityChannelExtensions = extensions.GetExtension<IUnityChannelExtensions>();
		this.m_TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();
		this.InitUI(controller.products.all);
		this.m_AppleExtensions.RegisterPurchaseDeferredListener(new Action<Product>(this.OnDeferred));
		UnityEngine.Debug.Log("Available items:");
		foreach (Product product in controller.products.all)
		{
			if (product.availableToPurchase)
			{
				UnityEngine.Debug.Log(string.Join(" - ", new string[]
				{
					product.metadata.localizedTitle,
					product.metadata.localizedDescription,
					product.metadata.isoCurrencyCode,
					product.metadata.localizedPrice.ToString(),
					product.metadata.localizedPriceString,
					product.transactionID,
					product.receipt
				}));
			}
		}
		this.AddProductUIs(this.m_Controller.products.all);
		this.LogProductDefinitions();
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		UnityEngine.Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
		UnityEngine.Debug.Log("Receipt: " + e.purchasedProduct.receipt);
		this.m_LastTransactionID = e.purchasedProduct.transactionID;
		this.m_PurchaseInProgress = false;
		if (this.m_IsUnityChannelSelected)
		{
			UnifiedReceipt unifiedReceipt = JsonUtility.FromJson<UnifiedReceipt>(e.purchasedProduct.receipt);
			if (unifiedReceipt != null && !string.IsNullOrEmpty(unifiedReceipt.Payload))
			{
				UnityChannelPurchaseReceipt unityChannelPurchaseReceipt = JsonUtility.FromJson<UnityChannelPurchaseReceipt>(unifiedReceipt.Payload);
				UnityEngine.Debug.LogFormat("UnityChannel receipt: storeSpecificId = {0}, transactionId = {1}, orderQueryToken = {2}", new object[]
				{
					unityChannelPurchaseReceipt.storeSpecificId,
					unityChannelPurchaseReceipt.transactionId,
					unityChannelPurchaseReceipt.orderQueryToken
				});
			}
		}
		this.UpdateProductUI(e.purchasedProduct);
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
	{
		UnityEngine.Debug.Log("Purchase failed: " + item.definition.id);
		UnityEngine.Debug.Log(r);
		UnityEngine.Debug.Log("Store specific error code: " + this.m_TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
		if (this.m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
		{
			UnityEngine.Debug.Log("Purchase failure description message: " + this.m_TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message);
		}
		if (this.m_IsUnityChannelSelected)
		{
			string lastPurchaseError = this.m_UnityChannelExtensions.GetLastPurchaseError();
			IAPDemo.UnityChannelPurchaseError unityChannelPurchaseError = JsonUtility.FromJson<IAPDemo.UnityChannelPurchaseError>(lastPurchaseError);
			if (unityChannelPurchaseError != null && unityChannelPurchaseError.purchaseInfo != null)
			{
				IAPDemo.UnityChannelPurchaseInfo purchaseInfo = unityChannelPurchaseError.purchaseInfo;
				UnityEngine.Debug.LogFormat("UnityChannel purchaseInfo: productCode = {0}, gameOrderId = {1}, orderQueryToken = {2}", new object[]
				{
					purchaseInfo.productCode,
					purchaseInfo.gameOrderId,
					purchaseInfo.orderQueryToken
				});
			}
			if (r == PurchaseFailureReason.Unknown && unityChannelPurchaseError != null && unityChannelPurchaseError.error != null && unityChannelPurchaseError.error.Equals("DuplicateTransaction"))
			{
				UnityEngine.Debug.Log("Duplicate transaction detected, unlock this item");
			}
		}
		this.m_PurchaseInProgress = false;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		UnityEngine.Debug.Log("Billing failed to initialize!");
		switch (error)
		{
		case InitializationFailureReason.PurchasingUnavailable:
			UnityEngine.Debug.Log("Billing disabled!");
			break;
		case InitializationFailureReason.NoProductsAvailable:
			UnityEngine.Debug.Log("No products available for purchase!");
			break;
		case InitializationFailureReason.AppNotKnown:
			UnityEngine.Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
			break;
		}
	}

	public void Awake()
	{
		StandardPurchasingModule standardPurchasingModule = StandardPurchasingModule.Instance();
		standardPurchasingModule.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
		ConfigurationBuilder builder = ConfigurationBuilder.Instance(standardPurchasingModule, new IPurchasingModule[0]);
		builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = false;
		this.m_IsGooglePlayStoreSelected = (Application.platform == RuntimePlatform.Android && standardPurchasingModule.appStore == AppStore.GooglePlay);
		builder.Configure<IMoolahConfiguration>().appKey = "d93f4564c41d463ed3d3cd207594ee1b";
		builder.Configure<IMoolahConfiguration>().hashKey = "cc";
		builder.Configure<IMoolahConfiguration>().SetMode(CloudMoolahMode.AlwaysSucceed);
		this.m_IsCloudMoolahStoreSelected = (Application.platform == RuntimePlatform.Android && standardPurchasingModule.appStore == AppStore.CloudMoolah);
		this.m_IsUnityChannelSelected = (Application.platform == RuntimePlatform.Android && standardPurchasingModule.appStore == AppStore.XiaomiMiPay);
		builder.Configure<IUnityChannelConfiguration>().fetchReceiptPayloadOnPurchase = this.m_FetchReceiptPayloadOnPurchase;
		ProductCatalog productCatalog = ProductCatalog.LoadDefaultCatalog();
		foreach (ProductCatalogItem productCatalogItem in productCatalog.allValidProducts)
		{
			if (productCatalogItem.allStoreIDs.Count > 0)
			{
				IDs ds = new IDs();
				foreach (StoreID storeID in productCatalogItem.allStoreIDs)
				{
					ds.Add(storeID.id, new string[]
					{
						storeID.store
					});
				}
				builder.AddProduct(productCatalogItem.id, productCatalogItem.type, ds);
			}
			else
			{
				builder.AddProduct(productCatalogItem.id, productCatalogItem.type);
			}
		}
		builder.AddProduct("100.gold.coins", ProductType.Consumable, new IDs
		{
			{
				"com.unity3d.unityiap.unityiapdemo.100goldcoins.7",
				new string[]
				{
					"MacAppStore"
				}
			},
			{
				"000000596586",
				new string[]
				{
					"TizenStore"
				}
			},
			{
				"com.ff",
				new string[]
				{
					"MoolahAppStore"
				}
			},
			{
				"100.gold.coins",
				new string[]
				{
					"AmazonApps"
				}
			}
		});
		builder.AddProduct("500.gold.coins", ProductType.Consumable, new IDs
		{
			{
				"com.unity3d.unityiap.unityiapdemo.500goldcoins.7",
				new string[]
				{
					"MacAppStore"
				}
			},
			{
				"000000596581",
				new string[]
				{
					"TizenStore"
				}
			},
			{
				"com.ee",
				new string[]
				{
					"MoolahAppStore"
				}
			},
			{
				"500.gold.coins",
				new string[]
				{
					"AmazonApps"
				}
			}
		});
		builder.AddProduct("sword", ProductType.NonConsumable, new IDs
		{
			{
				"com.unity3d.unityiap.unityiapdemo.sword.7",
				new string[]
				{
					"MacAppStore"
				}
			},
			{
				"000000596583",
				new string[]
				{
					"TizenStore"
				}
			},
			{
				"sword",
				new string[]
				{
					"AmazonApps"
				}
			}
		});
		builder.Configure<ISamsungAppsConfiguration>().SetMode(SamsungAppsMode.AlwaysSucceed);
		this.m_IsSamsungAppsStoreSelected = (Application.platform == RuntimePlatform.Android && standardPurchasingModule.appStore == AppStore.SamsungApps);
		builder.Configure<ITizenStoreConfiguration>().SetGroupId("100000085616");
		Action initializeUnityIap = delegate()
		{
			UnityPurchasing.Initialize(this, builder);
		};
		if (!this.m_IsUnityChannelSelected)
		{
			initializeUnityIap();
		}
		else
		{
			AppInfo appInfo = new AppInfo();
			appInfo.appId = "abc123appId";
			appInfo.appKey = "efg456appKey";
			appInfo.clientId = "hij789clientId";
			appInfo.clientKey = "klm012clientKey";
			appInfo.debug = false;
			this.unityChannelLoginHandler = new IAPDemo.UnityChannelLoginHandler();
			this.unityChannelLoginHandler.initializeFailedAction = delegate(string message)
			{
				UnityEngine.Debug.LogError("Failed to initialize and login to UnityChannel: " + message);
			};
			this.unityChannelLoginHandler.initializeSucceededAction = delegate()
			{
				initializeUnityIap();
			};
			StoreService.Initialize(appInfo, this.unityChannelLoginHandler);
		}
	}

	private void OnTransactionsRestored(bool success)
	{
		UnityEngine.Debug.Log("Transactions restored.");
	}

	private void OnDeferred(Product item)
	{
		UnityEngine.Debug.Log("Purchase deferred: " + item.definition.id);
	}

	private void InitUI(IEnumerable<Product> items)
	{
		this.restoreButton.gameObject.SetActive(this.NeedRestoreButton());
		this.loginButton.gameObject.SetActive(this.NeedLoginButton());
		this.validateButton.gameObject.SetActive(this.NeedValidateButton());
		this.ClearProductUIs();
		this.restoreButton.onClick.AddListener(new UnityAction(this.RestoreButtonClick));
		this.loginButton.onClick.AddListener(new UnityAction(this.LoginButtonClick));
		this.validateButton.onClick.AddListener(new UnityAction(this.ValidateButtonClick));
		this.versionText.text = "Unity version: " + Application.unityVersion + "\nIAP version: 1.20.1";
	}

	public void PurchaseButtonClick(string productID)
	{
		if (this.m_PurchaseInProgress)
		{
			UnityEngine.Debug.Log("Please wait, purchase in progress");
			return;
		}
		if (this.m_Controller == null)
		{
			UnityEngine.Debug.LogError("Purchasing is not initialized");
			return;
		}
		if (this.m_Controller.products.WithID(productID) == null)
		{
			UnityEngine.Debug.LogError("No product has id " + productID);
			return;
		}
		if (this.NeedLoginButton() && !this.m_IsLoggedIn)
		{
			UnityEngine.Debug.LogWarning("Purchase notifications will not be forwarded server-to-server. Login incomplete.");
		}
		this.m_PurchaseInProgress = true;
		this.m_Controller.InitiatePurchase(this.m_Controller.products.WithID(productID), "aDemoDeveloperPayload");
	}

	public void RestoreButtonClick()
	{
		if (this.m_IsCloudMoolahStoreSelected)
		{
			if (!this.m_IsLoggedIn)
			{
				UnityEngine.Debug.LogError("CloudMoolah purchase restoration aborted. Login incomplete.");
			}
			else
			{
				this.m_MoolahExtensions.RestoreTransactionID(delegate(RestoreTransactionIDState restoreTransactionIDState)
				{
					UnityEngine.Debug.Log("restoreTransactionIDState = " + restoreTransactionIDState.ToString());
					bool success = restoreTransactionIDState != RestoreTransactionIDState.RestoreFailed && restoreTransactionIDState != RestoreTransactionIDState.NotKnown;
					this.OnTransactionsRestored(success);
				});
			}
		}
		else if (this.m_IsSamsungAppsStoreSelected)
		{
			this.m_SamsungExtensions.RestoreTransactions(new Action<bool>(this.OnTransactionsRestored));
		}
		else if (Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerARM)
		{
			this.m_MicrosoftExtensions.RestoreTransactions();
		}
		else
		{
			this.m_AppleExtensions.RestoreTransactions(new Action<bool>(this.OnTransactionsRestored));
		}
	}

	public void LoginButtonClick()
	{
		if (!this.m_IsUnityChannelSelected)
		{
			UnityEngine.Debug.Log("Login is only required for the Xiaomi store");
			return;
		}
		this.unityChannelLoginHandler.loginSucceededAction = delegate(UserInfo userInfo)
		{
			this.m_IsLoggedIn = true;
			UnityEngine.Debug.LogFormat("Succeeded logging into UnityChannel. channel {0}, userId {1}, userLoginToken {2} ", new object[]
			{
				userInfo.channel,
				userInfo.userId,
				userInfo.userLoginToken
			});
		};
		this.unityChannelLoginHandler.loginFailedAction = delegate(string message)
		{
			this.m_IsLoggedIn = false;
			UnityEngine.Debug.LogError("Failed logging into UnityChannel. " + message);
		};
		StoreService.Login(this.unityChannelLoginHandler);
	}

	public void ValidateButtonClick()
	{
		if (!this.m_IsUnityChannelSelected)
		{
			UnityEngine.Debug.Log("Remote purchase validation is only supported for the Xiaomi store");
			return;
		}
		string txId = this.m_LastTransactionID;
		this.m_UnityChannelExtensions.ValidateReceipt(txId, delegate(bool success, string signData, string signature)
		{
			UnityEngine.Debug.LogFormat("ValidateReceipt transactionId {0}, success {1}, signData {2}, signature {3}", new object[]
			{
				txId,
				success,
				signData,
				signature
			});
		});
	}

	private void ClearProductUIs()
	{
		foreach (KeyValuePair<string, IAPDemoProductUI> keyValuePair in this.m_ProductUIs)
		{
			UnityEngine.Object.Destroy(keyValuePair.Value.gameObject);
		}
		this.m_ProductUIs.Clear();
	}

	private void AddProductUIs(Product[] products)
	{
		this.ClearProductUIs();
		RectTransform component = this.productUITemplate.GetComponent<RectTransform>();
		float height = component.rect.height;
		Vector3 vector = component.localPosition;
		this.contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)products.Length * height);
		foreach (Product product in products)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.productUITemplate.gameObject);
			gameObject.transform.SetParent(this.productUITemplate.transform.parent, false);
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
			component2.localPosition = vector;
			vector += Vector3.down * height;
			gameObject.SetActive(true);
			IAPDemoProductUI component3 = gameObject.GetComponent<IAPDemoProductUI>();
			component3.SetProduct(product, new Action<string>(this.PurchaseButtonClick));
			this.m_ProductUIs[product.definition.id] = component3;
		}
	}

	private void UpdateProductUI(Product p)
	{
		if (this.m_ProductUIs.ContainsKey(p.definition.id))
		{
			this.m_ProductUIs[p.definition.id].SetProduct(p, new Action<string>(this.PurchaseButtonClick));
		}
	}

	private void UpdateProductPendingUI(Product p, int secondsRemaining)
	{
		if (this.m_ProductUIs.ContainsKey(p.definition.id))
		{
			this.m_ProductUIs[p.definition.id].SetPendingTime(secondsRemaining);
		}
	}

	private bool NeedRestoreButton()
	{
		return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.tvOS || Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerARM || this.m_IsSamsungAppsStoreSelected || this.m_IsCloudMoolahStoreSelected;
	}

	private bool NeedLoginButton()
	{
		return this.m_IsUnityChannelSelected;
	}

	private bool NeedValidateButton()
	{
		return this.m_IsUnityChannelSelected;
	}

	private void LogProductDefinitions()
	{
		Product[] all = this.m_Controller.products.all;
		foreach (Product product in all)
		{
			UnityEngine.Debug.Log(string.Format("id: {0}\nstore-specific id: {1}\ntype: {2}\n", product.definition.id, product.definition.storeSpecificId, product.definition.type.ToString()));
		}
	}

	private IStoreController m_Controller;

	private IAppleExtensions m_AppleExtensions;

	private IMoolahExtension m_MoolahExtensions;

	private ISamsungAppsExtensions m_SamsungExtensions;

	private IMicrosoftExtensions m_MicrosoftExtensions;

	private IUnityChannelExtensions m_UnityChannelExtensions;

	private ITransactionHistoryExtensions m_TransactionHistoryExtensions;

	private bool m_IsGooglePlayStoreSelected;

	private bool m_IsSamsungAppsStoreSelected;

	private bool m_IsCloudMoolahStoreSelected;

	private bool m_IsUnityChannelSelected;

	private string m_LastTransactionID;

	private bool m_IsLoggedIn;

	private IAPDemo.UnityChannelLoginHandler unityChannelLoginHandler;

	private bool m_FetchReceiptPayloadOnPurchase;

	private bool m_PurchaseInProgress;

	private Dictionary<string, IAPDemoProductUI> m_ProductUIs = new Dictionary<string, IAPDemoProductUI>();

	public GameObject productUITemplate;

	public RectTransform contentRect;

	public Button restoreButton;

	public Button loginButton;

	public Button validateButton;

	public Text versionText;

	[Serializable]
	public class UnityChannelPurchaseError
	{
		public string error;

		public IAPDemo.UnityChannelPurchaseInfo purchaseInfo;
	}

	[Serializable]
	public class UnityChannelPurchaseInfo
	{
		public string productCode;

		public string gameOrderId;

		public string orderQueryToken;
	}

	private class UnityChannelLoginHandler : ILoginListener
	{
		public void OnInitialized()
		{
			this.initializeSucceededAction();
		}

		public void OnInitializeFailed(string message)
		{
			this.initializeFailedAction(message);
		}

		public void OnLogin(UserInfo userInfo)
		{
			this.loginSucceededAction(userInfo);
		}

		public void OnLoginFailed(string message)
		{
			this.loginFailedAction(message);
		}

		internal Action initializeSucceededAction;

		internal Action<string> initializeFailedAction;

		internal Action<UserInfo> loginSucceededAction;

		internal Action<string> loginFailedAction;
	}
}
