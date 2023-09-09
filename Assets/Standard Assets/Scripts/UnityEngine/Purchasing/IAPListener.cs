using System;
using UnityEngine.Events;

namespace UnityEngine.Purchasing
{
	[AddComponentMenu("Unity IAP/IAP Listener")]
	[HelpURL("https://docs.unity3d.com/Manual/UnityIAP.html")]
	public class IAPListener : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this.dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
			CodelessIAPStoreListener.Instance.AddListener(this);
		}

		private void OnDisable()
		{
			CodelessIAPStoreListener.Instance.RemoveListener(this);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			UnityEngine.Debug.Log(string.Format("IAPListener.ProcessPurchase(PurchaseEventArgs {0} - {1})", e, e.purchasedProduct.definition.id));
			this.onPurchaseComplete.Invoke(e.purchasedProduct);
			return (!this.consumePurchase) ? PurchaseProcessingResult.Pending : PurchaseProcessingResult.Complete;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
		{
			UnityEngine.Debug.Log(string.Format("IAPListener.OnPurchaseFailed(Product {0}, PurchaseFailureReason {1})", product, reason));
			this.onPurchaseFailed.Invoke(product, reason);
		}

		[Tooltip("Consume successful purchases immediately")]
		public bool consumePurchase = true;

		[Tooltip("Preserve this GameObject when a new scene is loaded")]
		public bool dontDestroyOnLoad = true;

		[Tooltip("Event fired after a successful purchase of this product")]
		public IAPListener.OnPurchaseCompletedEvent onPurchaseComplete;

		[Tooltip("Event fired after a failed purchase of this product")]
		public IAPListener.OnPurchaseFailedEvent onPurchaseFailed;

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
