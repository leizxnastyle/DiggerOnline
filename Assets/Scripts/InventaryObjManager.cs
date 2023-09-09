using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using UnityEngine;

public class InventaryObjManager : MonoBehaviour
{
	public static event Action<int> cells_update_event;

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Awake()
	{
		this.item_cell_obj = (GameObject)Resources.Load("DWork/Prefabs/inv_cell", typeof(GameObject));
	}

	private void Start()
	{
		this.Init();
		this.ShowInventory();
	}

	private void CreateInventar()
	{
	}

	private void Init()
	{
		InventaryObjManager.inv_obj = new List<is_InventoryObj>();
		InventaryObjManager.inv_cells = new Dictionary<int, is_InventoryCell>();
		foreach (is_InventoryCell is_InventoryCell in this.pcells)
		{
			InventaryObjManager.inv_cells.Add(is_InventoryCell.cell_id, is_InventoryCell);
		}
		InventaryObjManager.inventary = new IS_Ply_Inventary();
		InventaryObjManager.inventary.invManager = this;
	}

	public void ShowInventory()
	{
		try
		{
			foreach (is_InventoryObj is_InventoryObj in InventaryObjManager.inv_obj)
			{
				if (is_InventoryObj != null)
				{
					UnityEngine.Object.Destroy(is_InventoryObj.gameObject);
				}
			}
			InventaryObjManager.inv_obj = new List<is_InventoryObj>();
			int num = 1;
			foreach (KeyValuePair<Guid, IS_mdl_InventoryItem> keyValuePair in InventaryObjManager.inventary.user_items)
			{
				if (!InventaryObjManager.ShowInventar && keyValuePair.Value.cell_id <= 25)
				{
					this.InstObj(keyValuePair.Value, keyValuePair.Value.cell_id);
					num++;
				}
				else if (InventaryObjManager.ShowInventar)
				{
					this.InstObj(keyValuePair.Value, keyValuePair.Value.cell_id);
					num++;
				}
			}
			if (InventaryObjManager.inventary.chest_items.Count > 0)
			{
				foreach (KeyValuePair<Guid, IS_mdl_InventoryItem> keyValuePair2 in InventaryObjManager.inventary.chest_items)
				{
					this.InstObj(keyValuePair2.Value, keyValuePair2.Value.cell_id);
				}
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Error Show Inventory\n" + ex.StackTrace);
		}
	}

	private void InstObj(IS_mdl_InventoryItem item, int index)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.item_cell_obj, base.transform.position, base.transform.rotation);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.transform.position = InventaryObjManager.inv_cells[index].transform.position;
		Vector3 localPosition = gameObject.transform.localPosition;
		localPosition.z = -5f;
		gameObject.transform.localPosition = localPosition;
		InventaryObjManager.inv_cells[index].isEmpty = false;
		InventaryObjManager.inv_cells[index].item_guid = item.bag_id;
		gameObject.GetComponent<is_InventoryObj>().Init(item);
		InventaryObjManager.inv_obj.Add(gameObject.GetComponent<is_InventoryObj>());
	}

	public static void SetZeroItemInPlayer()
	{
		if (InventaryObjManager.inventary != null)
		{
			InventaryObjManager.inventary.user_items = new Dictionary<Guid, IS_mdl_InventoryItem>();
			foreach (is_InventoryCell is_InventoryCell in InventaryObjManager.inv_cells.Values)
			{
				if (!is_InventoryCell.isEmpty)
				{
					is_InventoryCell.isEmpty = true;
				}
			}
		}
	}

	public static is_InventoryObj GetInvObjectByCellId(int cell_id)
	{
		foreach (is_InventoryObj is_InventoryObj in InventaryObjManager.inv_obj)
		{
			if (is_InventoryObj.item.cell_id == cell_id)
			{
				return is_InventoryObj;
			}
		}
		return null;
	}

	public static IS_mdl_Item GetItemFromBagId(Guid bag_id)
	{
		if (InventaryObjManager.inventary.user_items.Count > 0 && InventaryObjManager.inventary.user_items.ContainsKey(bag_id))
		{
			return IS_Manager.GetItemById(InventaryObjManager.inventary.user_items[bag_id].item_id);
		}
		return null;
	}

	public static void CellsUpdate(int cell_id)
	{
		if (InventaryObjManager.cells_update_event != null)
		{
			InventaryObjManager.cells_update_event(cell_id);
		}
	}

	public static void AddItemToInventary(int inv_id)
	{
		InventaryObjManager.inventary.PutItemToInventory(IS_Manager.GetItemById(inv_id));
	}

	public static void AddItemsToChest(List<int> new_items)
	{
		InventaryObjManager.ShowInventar = false;
		InventaryObjManager.inventary.chest_items = new Dictionary<Guid, IS_mdl_InventoryItem>();
		int num = 100;
		if (new_items.Count == 0)
		{
			InventaryObjManager.inventary.invManager.ShowInventory();
		}
		foreach (int item_id in new_items)
		{
			InventaryObjManager.inventary.PutItemToChest(IS_Manager.GetItemById(item_id), num);
			num++;
		}
		InventaryObjManager.inventary.invManager.ShowInventory();
	}

	internal static string GetDropItem()
	{
		List<int> list = new List<int>();
		foreach (IS_mdl_InventoryItem is_mdl_InventoryItem in InventaryObjManager.inventary.user_items.Values)
		{
			list.Add(is_mdl_InventoryItem.item_id);
			if (list.Count >= 15)
			{
				break;
			}
		}
		return string.Join(";", (from x in list
		select x.ToString()).ToArray<string>());
	}

	public List<is_InventoryCell> pcells;

	public static Dictionary<int, is_InventoryCell> inv_cells;

	public static List<is_InventoryObj> inv_obj;

	public static IS_Ply_Inventary inventary;

	private GameObject item_cell_obj;

	public static bool ShowInventar = true;
}
