using System;
using System.Collections.Generic;

namespace InventorySystem
{
	public static class IS_Manager
	{
		public static void Init()
		{
			IS_Manager.game_items = new List<IS_mdl_Item>();
			IS_Manager.index_game_items = new Dictionary<int, int>();
			IS_Manager.GetGameItems();
			IS_Manager.GetItemIndexes();
		}

		public static void GetItemIndexes()
		{
			for (int i = 0; i < IS_Manager.game_items.Count; i++)
			{
				IS_Manager.index_game_items.Add(IS_Manager.game_items[i].Id, i);
			}
		}

		private static void GetGameItems()
		{
			ItemsAdder.Init();
		}

		public static IS_mdl_Item GetItemById(int item_id)
		{
			if (IS_Manager.index_game_items != null && IS_Manager.index_game_items.ContainsKey(item_id))
			{
				int num = IS_Manager.index_game_items[item_id];
				if (IS_Manager.game_items != null && IS_Manager.game_items.Count > num)
				{
					return IS_Manager.game_items[num];
				}
			}
			return null;
		}

		public static void AddItem(IS_mdl_Item item)
		{
			IS_Manager.game_items.Add(item);
		}

		public static bool CanPutInCell(int item_id, Cell_Type cell_type)
		{
			IS_mdl_Item itemById = IS_Manager.GetItemById(item_id);
			if (cell_type == Cell_Type.CT_ALL)
			{
				return true;
			}
			if ((itemById.ItemType == eIS_ItemType.IT_SWORD || itemById.ItemType == eIS_ItemType.IT_BOW) && cell_type == Cell_Type.CT_WEAPON)
			{
				return true;
			}
			if ((itemById.ItemType == eIS_ItemType.IT_POWER || itemById.ItemType == eIS_ItemType.IT_FOOD || itemById.ItemType == eIS_ItemType.IT_ARROW) && cell_type == Cell_Type.CT_ROWER)
			{
				return true;
			}
			if (itemById.ItemType == eIS_ItemType.IT_ARRMOR)
			{
				IS_mdl_Armor is_mdl_Armor = itemById as IS_mdl_Armor;
				return (is_mdl_Armor.ArmorType == eIS_Arrmor_SUBT.AS_HEAD && cell_type == Cell_Type.CT_ARRMOR_HEAD) || (is_mdl_Armor.ArmorType == eIS_Arrmor_SUBT.AS_BODY && cell_type == Cell_Type.CT_ARRMOR_BODY) || (is_mdl_Armor.ArmorType == eIS_Arrmor_SUBT.AS_LEGS && cell_type == Cell_Type.CT_ARRMOR_LEGS);
			}
			return false;
		}

		public static T GetCurentMdlData<T>(T obj)
		{
			return obj;
		}

		internal static int GetFastMove(int item_id, Cell_Type cell_type)
		{
			IS_mdl_Item itemById = IS_Manager.GetItemById(item_id);
			if (cell_type != Cell_Type.CT_ALL)
			{
				return IS_Manager.GetFreeCellId(Cell_Type.CT_ALL, 0);
			}
			if (itemById.ItemType == eIS_ItemType.IT_BOW || itemById.ItemType == eIS_ItemType.IT_SWORD)
			{
				return IS_Manager.GetFreeCellId(Cell_Type.CT_WEAPON, 0);
			}
			if (itemById.ItemType == eIS_ItemType.IT_FOOD || itemById.ItemType == eIS_ItemType.IT_ARROW)
			{
				return IS_Manager.GetFreeCellId(Cell_Type.CT_ROWER, 0);
			}
			if (itemById.ItemType == eIS_ItemType.IT_ARRMOR)
			{
				IS_mdl_Armor is_mdl_Armor = itemById as IS_mdl_Armor;
				if (is_mdl_Armor.ArmorType == eIS_Arrmor_SUBT.AS_HEAD)
				{
					return IS_Manager.GetFreeCellId(Cell_Type.CT_ARRMOR_HEAD, 0);
				}
				if (is_mdl_Armor.ArmorType == eIS_Arrmor_SUBT.AS_BODY)
				{
					return IS_Manager.GetFreeCellId(Cell_Type.CT_ARRMOR_BODY, 0);
				}
				if (is_mdl_Armor.ArmorType == eIS_Arrmor_SUBT.AS_LEGS)
				{
					return IS_Manager.GetFreeCellId(Cell_Type.CT_ARRMOR_LEGS, 0);
				}
			}
			return -1;
		}

		internal static int GetFastMoveChest(int item_id, int cel_id)
		{
			IS_mdl_Item itemById = IS_Manager.GetItemById(item_id);
			if (cel_id < 100)
			{
				return IS_Manager.GetFreeCellId(Cell_Type.CT_ALL, 100);
			}
			return IS_Manager.GetFreeCellId(Cell_Type.CT_ALL, 0);
		}

		public static int GetFreeCellId()
		{
			foreach (KeyValuePair<int, is_InventoryCell> keyValuePair in InventaryObjManager.inv_cells)
			{
				if (keyValuePair.Value.isEmpty)
				{
					return keyValuePair.Key;
				}
			}
			return -1;
		}

		public static int GetFreeCellId(Cell_Type cell_type, int from_cell = 0)
		{
			foreach (KeyValuePair<int, is_InventoryCell> keyValuePair in InventaryObjManager.inv_cells)
			{
				if (keyValuePair.Value.cell_id >= from_cell && keyValuePair.Value.cell_type == cell_type && keyValuePair.Value.isEmpty)
				{
					return keyValuePair.Key;
				}
			}
			return -1;
		}

		private static List<IS_mdl_Item> game_items = new List<IS_mdl_Item>();

		private static Dictionary<int, int> index_game_items = new Dictionary<int, int>();
	}
}
