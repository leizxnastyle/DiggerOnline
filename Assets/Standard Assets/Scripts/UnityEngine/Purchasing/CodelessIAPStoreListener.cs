using System;
using System.Collections.Generic;
using UnityEngine.Purchasing.Extension;

namespace UnityEngine.Purchasing
{
	public class CodelessIAPStoreListener : IStoreListener
	{
		private CodelessIAPStoreListener()
		{
			this.catalog = ProductCatalog.LoadDefaultCatalog();
		}

		[RuntimeInitializeOnLoadMethod]
		private static void InitializeCodelessPurchasingOnLoad()
		{
			ProductCatalog productCatalog = ProductCatalog.LoadDefaultCatalog();
			if (productCatalog.enableCodelessAutoInitialization && !productCatalog.IsEmpty() && CodelessIAPStoreListener.instance == null)
			{
				CodelessIAPStoreListener.CreateCodelessIAPStoreListenerInstance();
			}
		}

		private static void InitializePurchasing()
		{
			StandardPurchasingModule standardPurchasingModule = StandardPurchasingModule.Instance();
			standardPurchasingModule.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
			ConfigurationBuilder builder = ConfigurationBuilder.Instance(standardPurchasingModule, new IPurchasingModule[0]);
			IAPConfigurationHelper.PopulateConfigurationBuilder(ref builder, CodelessIAPStoreListener.instance.catalog);
			UnityPurchasing.Initialize(CodelessIAPStoreListener.instance, builder);
			CodelessIAPStoreListener.unityPurchasingInitialized = true;
		}

		public static CodelessIAPStoreListener Instance
		{
			get
			{
				if (CodelessIAPStoreListener.instance == null)
				{
					CodelessIAPStoreListener.CreateCodelessIAPStoreListenerInstance();
				}
				return CodelessIAPStoreListener.instance;
			}
		}

		private static void CreateCodelessIAPStoreListenerInstance()
		{
			CodelessIAPStoreListener.instance = new CodelessIAPStoreListener();
			if (!CodelessIAPStoreListener.unityPurchasingInitialized)
			{
				UnityEngine.Debug.Log("Initializing UnityPurchasing via Codeless IAP");
				CodelessIAPStoreListener.InitializePurchasing();
			}
		}

		public IStoreController StoreController
		{
			get
			{
				return this.controller;
			}
		}

		public IExtensionProvider ExtensionProvider
		{
			get
			{
				return this.extensions;
			}
		}

		public bool HasProductInCatalog(string productID)
		{
			foreach (ProductCatalogItem productCatalogItem in this.catalog.allProducts)
			{
				if (productCatalogItem.id == productID)
				{
					return true;
				}
			}
			return false;
		}

		public Product GetProduct(string productID)
		{
			if (this.controller != null && this.controller.products != null && !string.IsNullOrEmpty(productID))
			{
				return this.controller.products.WithID(productID);
			}
			UnityEngine.Debug.LogError("CodelessIAPStoreListener attempted to get unknown product " + productID);
			return null;
		}

		public void AddButton(IAPButton button)
		{
			this.activeButtons.Add(button);
		}

		public void RemoveButton(IAPButton button)
		{
			this.activeButtons.Remove(button);
		}

		public void AddListener(IAPListener listener)
		{
			this.activeListeners.Add(listener);
		}

		public void RemoveListener(IAPListener listener)
		{
			this.activeListeners.Remove(listener);
		}

		public void InitiatePurchase(string productID)
		{
			if (this.controller == null)
			{
				UnityEngine.Debug.LogError("Purchase failed because Purchasing was not initialized correctly");
				foreach (IAPButton iapbutton in this.activeButtons)
				{
					if (iapbutton.productId == productID)
					{
						iapbutton.OnPurchaseFailed(null, PurchaseFailureReason.PurchasingUnavailable);
					}
				}
				return;
			}
			this.controller.InitiatePurchase(productID);
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			CodelessIAPStoreListener.initializationComplete = true;
			this.controller = controller;
			this.extensions = extensions;
			foreach (IAPButton iapbutton in this.activeButtons)
			{
				iapbutton.UpdateText();
			}
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			UnityEngine.Debug.LogError(string.Format("Purchasing failed to initialize. Reason: {0}", error.ToString()));
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (IAPButton iapbutton in this.activeButtons)
			{
				if (iapbutton.productId == e.purchasedProduct.definition.id)
				{
					if (iapbutton.ProcessPurchase(e) == PurchaseProcessingResult.Complete)
					{
						flag = true;
					}
					flag2 = true;
				}
			}
			foreach (IAPListener iaplistener in this.activeListeners)
			{
				if (iaplistener.ProcessPurchase(e) == PurchaseProcessingResult.Complete)
				{
					flag = true;
				}
				flag2 = true;
			}
			if (!flag2)
			{
				UnityEngine.Debug.LogError("Purchase not correctly processed for product \"" + e.purchasedProduct.definition.id + "\". Add an active IAPButton to process this purchase, or add an IAPListener to receive any unhandled purchase events.");
			}
			return (!flag) ? PurchaseProcessingResult.Pending : PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
		{
			bool flag = false;
			foreach (IAPButton iapbutton in this.activeButtons)
			{
				if (iapbutton.productId == product.definition.id)
				{
					iapbutton.OnPurchaseFailed(product, reason);
					flag = true;
				}
			}
			foreach (IAPListener iaplistener in this.activeListeners)
			{
				iaplistener.OnPurchaseFailed(product, reason);
				flag = true;
			}
			if (!flag)
			{
				UnityEngine.Debug.LogError("Failed purchase not correctly handled for product \"" + product.definition.id + "\". Add an active IAPButton to handle this failure, or add an IAPListener to receive any unhandled purchase failures.");
			}
		}

		private static CodelessIAPStoreListener instance;

		private List<IAPButton> activeButtons = new List<IAPButton>();

		private List<IAPListener> activeListeners = new List<IAPListener>();

		private static bool unityPurchasingInitialized;

		protected IStoreController controller;

		protected IExtensionProvider extensions;

		protected ProductCatalog catalog;

		public static bool initializationComplete;
	}
}
