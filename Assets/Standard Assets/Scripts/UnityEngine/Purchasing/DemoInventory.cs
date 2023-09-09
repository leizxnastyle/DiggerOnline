using System;
using System.Collections.Generic;

namespace UnityEngine.Purchasing
{
	[AddComponentMenu("")]
	public class DemoInventory : MonoBehaviour
	{
    static Dictionary<string, int> _003C_003Ef__switch_0024map0;
		public void Fulfill(string productId)
		{
			if (productId != null)
			{
				if (DemoInventory._003C_003Ef__switch_0024map0 == null)
				{
					DemoInventory._003C_003Ef__switch_0024map0 = new Dictionary<string, int>(1)
					{
						{
							"100.gold.coins",
							0
						}
					};
				}
				int num;
				if (DemoInventory._003C_003Ef__switch_0024map0.TryGetValue(productId, out num))
				{
					if (num == 0)
					{
						UnityEngine.Debug.Log("You Got Money!");
						return;
					}
				}
			}
			UnityEngine.Debug.Log(string.Format("Unrecognized productId \"{0}\"", productId));
		}
	}
}
