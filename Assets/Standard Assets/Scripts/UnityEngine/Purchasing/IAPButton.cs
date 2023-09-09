using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.Purchasing
{
	[HelpURL("https://docs.unity3d.com/Manual/UnityIAP.html")]
	[AddComponentMenu("Unity IAP/IAP Button")]
	[RequireComponent(typeof(Button))]
	public class IAPButton : MonoBehaviour
	{
		private void Start()
		{
			Button component = base.GetComponent<Button>();
			if (this.buttonType == IAPButton.ButtonType.Purchase)
			{
				if (component)
				{
					component.onClick.AddListener(new UnityAction(this.PurchaseProduct));
				}
				if (string.IsNullOrEmpty(this.productId))
				{
					UnityEngine.Debug.LogError("IAPButton productId is empty");
				}
				if (!CodelessIAPStoreListener.Instance.HasProductInCatalog(this.productId))
				{
					UnityEngine.Debug.LogWarning("The product catalog has no product with the ID \"" + this.productId + "\"");
				}
			}
			else if (this.buttonType == IAPButton.ButtonType.Restore && component)
			{
				component.onClick.AddListener(new UnityAction(this.Restore));
			}
		}

		private void OnEnable()
		{
			if (this.buttonType == IAPButton.ButtonType.Purchase)
			{
				CodelessIAPStoreListener.Instance.AddButton(this);
				if (CodelessIAPStoreListener.initializationComplete)
				{
					this.UpdateText();
				}
			}
		}

		private void OnDisable()
		{
			if (this.buttonType == IAPButton.ButtonType.Purchase)
			{
				CodelessIAPStoreListener.Instance.RemoveButton(this);
			}
		}

		private void PurchaseProduct()
		{
			if (this.buttonType == IAPButton.ButtonType.Purchase)
			{
				UnityEngine.Debug.Log("IAPButton.PurchaseProduct() with product ID: " + this.productId);
				CodelessIAPStoreListener.Instance.InitiatePurchase(this.productId);
			}
		}

		private void Restore()
		{
			if (this.buttonType == IAPButton.ButtonType.Restore)
			{
				if (Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerARM)
				{
					CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IMicrosoftExtensions>().RestoreTransactions();
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.tvOS)
				{
					CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(new Action<bool>(this.OnTransactionsRestored));
				}
				else if (Application.platform == RuntimePlatform.Android && StandardPurchasingModule.Instance().appStore == AppStore.SamsungApps)
				{
					CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<ISamsungAppsExtensions>().RestoreTransactions(new Action<bool>(this.OnTransactionsRestored));
				}
				else if (Application.platform == RuntimePlatform.Android && StandardPurchasingModule.Instance().appStore == AppStore.CloudMoolah)
				{
					CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IMoolahExtension>().RestoreTransactionID(delegate(RestoreTransactionIDState restoreTransactionIDState)
					{
						this.OnTransactionsRestored(restoreTransactionIDState != RestoreTransactionIDState.RestoreFailed && restoreTransactionIDState != RestoreTransactionIDState.NotKnown);
					});
				}
				else
				{
					UnityEngine.Debug.LogWarning(Application.platform.ToString() + " is not a supported platform for the Codeless IAP restore button");
				}
			}
		}

		private void OnTransactionsRestored(bool success)
		{
			UnityEngine.Debug.Log("Transactions restored: " + success);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			UnityEngine.Debug.Log(string.Format("IAPButton.ProcessPurchase(PurchaseEventArgs {0} - {1})", e, e.purchasedProduct.definition.id));
			this.onPurchaseComplete.Invoke(e.purchasedProduct);
			return (!this.consumePurchase) ? PurchaseProcessingResult.Pending : PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
		{
			UnityEngine.Debug.Log(string.Format("IAPButton.OnPurchaseFailed(Product {0}, PurchaseFailureReason {1})", product, reason));
			this.onPurchaseFailed.Invoke(product, reason);
		}

		internal void UpdateText()
		{
			Product product = CodelessIAPStoreListener.Instance.GetProduct(this.productId);
			if (product != null)
			{
				if (this.titleText != null)
				{
					this.titleText.text = product.metadata.localizedTitle;
				}
				if (this.descriptionText != null)
				{
					this.descriptionText.text = product.metadata.localizedDescription;
				}
				if (this.priceText != null)
				{
					this.priceText.text = product.metadata.localizedPriceString;
				}
			}
		}

		[HideInInspector]
		public string productId;

		[Tooltip("The type of this button, can be either a purchase or a restore button")]
		public IAPButton.ButtonType buttonType;

		[Tooltip("Consume the product immediately after a successful purchase")]
		public bool consumePurchase = true;

		[Tooltip("Event fired after a successful purchase of this product")]
		public IAPButton.OnPurchaseCompletedEvent onPurchaseComplete;

		[Tooltip("Event fired after a failed purchase of this product")]
		public IAPButton.OnPurchaseFailedEvent onPurchaseFailed;

		[Tooltip("[Optional] Displays the localized title from the app store")]
		public Text titleText;

		[Tooltip("[Optional] Displays the localized description from the app store")]
		public Text descriptionText;

		[Tooltip("[Optional] Displays the localized price from the app store")]
		public Text priceText;

		public enum ButtonType
		{
			Purchase,
			Restore
		}

		[Serializable]
		public class OnPurchaseCompletedEvent : UnityEvent<Product>
		{
		}

		[Serializable]
		public class OnPurchaseFailedEvent : UnityEvent<Product, PurchaseFailureReason>
		{
		}
	}
}
