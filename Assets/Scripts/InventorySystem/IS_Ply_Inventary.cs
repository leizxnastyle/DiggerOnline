using System;
using System.Collections.Generic;

namespace InventorySystem
{
	public class IS_Ply_Inventary
	{
		public IS_Ply_Inventary()
		{
			this.user_items = new Dictionary<Guid, IS_mdl_InventoryItem>();
			this.chest_items = new Dictionary<Guid, IS_mdl_InventoryItem>();
		}

		public IS_mdl_InventoryItem GetInventoryItem(Guid bag_id)
		{
			if (this.user_items.ContainsKey(bag_id))
			{
				return this.user_items[bag_id];
			}
			return null;
		}

		public void PutItemToInventory(IS_mdl_Item item, int cell_id)
		{
			if (item.IsStorage)
			{
				this.AddStorageItem(item, cell_id);
			}
			else
			{
				this.AddNoStorageItem(item, cell_id);
			}
			this.invManager.ShowInventory();
		}

		public void PutItemToInventory(IS_mdl_Item item)
		{
			int freeCellId = IS_Manager.GetFreeCellId();
			if (item.IsStorage)
			{
				this.AddStorageItem(item, freeCellId);
			}
			else
			{
				this.AddNoStorageItem(item, freeCellId);
			}
			this.invManager.ShowInventory();
		}

		private void AddNoStorageItem(IS_mdl_Item item, int cell_id)
		{
			Guid guid = Guid.NewGuid();
			if (cell_id == -1)
			{
				cell_id = IS_Manager.GetFreeCellId();
			}
			IS_mdl_InventoryItem value = new IS_mdl_InventoryItem(guid, item.Id, item.Count, cell_id);
			this.user_items.Add(guid, value);
		}

		private void AddStorageItem(IS_mdl_Item item, int cell_id)
		{
			foreach (IS_mdl_InventoryItem is_mdl_InventoryItem in this.user_items.Values)
			{
				if (is_mdl_InventoryItem.item_id == item.Id)
				{
					is_mdl_InventoryItem.count += item.Count;
					return;
				}
			}
			this.AddNoStorageItem(item, cell_id);
		}

		public bool IsEnableInPlayerInventory(int item_id)
		{
			foreach (IS_mdl_InventoryItem is_mdl_InventoryItem in this.user_items.Values)
			{
				if (is_mdl_InventoryItem.item_id == item_id)
				{
					return true;
				}
			}
			return false;
		}

		public void LayOutFromInventory(Guid bag_id)
		{
			if (this.user_items.ContainsKey(bag_id))
			{
				if (this.user_items[bag_id].count > 1)
				{
					int count = IS_Manager.GetItemById(this.user_items[bag_id].item_id).Count;
					this.user_items[bag_id].count -= count;
					if (this.user_items[bag_id].count == 0)
					{
						InventaryObjManager.inv_cells[this.user_items[bag_id].cell_id].isEmpty = true;
						this.user_items.Remove(bag_id);
					}
				}
				else
				{
					InventaryObjManager.inv_cells[this.user_items[bag_id].cell_id].isEmpty = true;
					this.user_items.Remove(bag_id);
				}
			}
			this.invManager.ShowInventory();
		}

		public void PutItemToChest(IS_mdl_Item item, int cell_id)
		{
			Guid guid = Guid.NewGuid();
			IS_mdl_InventoryItem value = new IS_mdl_InventoryItem(guid, item.Id, item.Count, cell_id);
			this.chest_items.Add(guid, value);
		}

		public void FromChestToInventory(IS_mdl_InventoryItem inv_item)
		{
			if (this.chest_items.ContainsKey(inv_item.bag_id))
			{
				this.chest_items.Remove(inv_item.bag_id);
				this.user_items.Add(inv_item.bag_id, inv_item);
			}
		}

		public void FromInventoryToChest(IS_mdl_InventoryItem inv_item)
		{
			if (this.user_items.ContainsKey(inv_item.bag_id))
			{
				this.user_items.Remove(inv_item.bag_id);
				this.chest_items.Add(inv_item.bag_id, inv_item);
			}
		}

		public void LayOutFromChest(Guid bag_id)
		{
			if (this.chest_items.ContainsKey(bag_id))
			{
				if (this.chest_items[bag_id].count > 1)
				{
					this.chest_items[bag_id].count--;
				}
				else
				{
					this.chest_items.Remove(bag_id);
				}
				this.invManager.ShowInventory();
			}
		}

		public bool CanUseItem(Guid bag_id)
		{
			return this.user_items.ContainsKey(bag_id);
		}

		public void UseItem(Guid bag_id)
		{
			if (this.user_items.ContainsKey(bag_id))
			{
				if (this.user_items[bag_id].count > 1)
				{
					this.user_items[bag_id].count--;
				}
				else
				{
					InventaryObjManager.inv_cells[this.user_items[bag_id].cell_id].isEmpty = true;
					this.user_items.Remove(bag_id);
				}
				pnl_Inventory.ReUpdate();
				this.invManager.ShowInventory();
				pnl_Inventory.tooltip.Hide();
			}
		}

		public bool CanShootFromBowMinusArrow(bool minus, ref int arrow_id)
		{
			List<int> list = new List<int>
			{
				29,
				30,
				31
			};
			foreach (int key in list)
			{
				if (!InventaryObjManager.inv_cells[key].isEmpty && InventaryObjManager.GetItemFromBagId(this.user_items[InventaryObjManager.inv_cells[key].item_guid].bag_id).ItemType == eIS_ItemType.IT_ARROW && this.user_items[InventaryObjManager.inv_cells[key].item_guid].count > 0)
				{
					if (arrow_id == -1)
					{
						arrow_id = InventaryObjManager.GetItemFromBagId(this.user_items[InventaryObjManager.inv_cells[key].item_guid].bag_id).Id;
					}
					if (minus)
					{
						this.user_items[InventaryObjManager.inv_cells[key].item_guid].count--;
					}
					if (this.user_items[InventaryObjManager.inv_cells[key].item_guid].count == 0)
					{
						this.LayOutFromInventory(InventaryObjManager.inv_cells[key].item_guid);
						MainMenu.Instance.HgWorkController.WeaponIsUpdate();
					}
					return true;
				}
			}
			return false;
		}

		public InventaryObjManager invManager;

		public Dictionary<Guid, IS_mdl_InventoryItem> user_items = new Dictionary<Guid, IS_mdl_InventoryItem>();

		public Dictionary<Guid, IS_mdl_InventoryItem> chest_items = new Dictionary<Guid, IS_mdl_InventoryItem>();
	}
}
