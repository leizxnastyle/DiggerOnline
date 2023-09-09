using System;

namespace UnityEngine.Purchasing
{
	public static class IAPConfigurationHelper
	{
		public static void PopulateConfigurationBuilder(ref ConfigurationBuilder builder, ProductCatalog catalog)
		{
			foreach (ProductCatalogItem productCatalogItem in catalog.allValidProducts)
			{
				IDs ds = null;
				if (productCatalogItem.allStoreIDs.Count > 0)
				{
					ds = new IDs();
					foreach (StoreID storeID in productCatalogItem.allStoreIDs)
					{
						ds.Add(storeID.id, new string[]
						{
							storeID.store
						});
					}
				}
				builder.AddProduct(productCatalogItem.id, productCatalogItem.type, ds);
			}
		}
	}
}
