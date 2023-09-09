using System;

namespace InventorySystem
{
	public class IS_mdl_InventoryItem
	{
		public IS_mdl_InventoryItem()
		{
		}

		public IS_mdl_InventoryItem(Guid bid, int iid, int cw, int c_id)
		{
			this.bag_id = bid;
			this.item_id = iid;
			this.count = cw;
			this.cell_id = c_id;
		}

		public Guid bag_id;

		public int item_id;

		public int count;

		public int cell_id;
	}
}
