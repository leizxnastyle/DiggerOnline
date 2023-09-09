using System;
using InventorySystem;
using SkinsSystem;
using UnityEngine;

public class is_InventoryCell : MonoBehaviour
{
	private void OnEnable()
	{
		InventaryObjManager.cells_update_event += this.HandleCellsUpdateEvent;
	}

	private void OnDisable()
	{
		InventaryObjManager.cells_update_event -= this.HandleCellsUpdateEvent;
	}

	private void HandleCellsUpdateEvent(int c_id)
	{
		if (c_id == this.cell_id && (this.cell_type == Cell_Type.CT_ARRMOR_BODY || this.cell_type == Cell_Type.CT_ARRMOR_HEAD || this.cell_type == Cell_Type.CT_ARRMOR_LEGS))
		{
			if (this.item_guid == Guid.Empty)
			{
				SkinsManager.UpdateSkin(0, this.ConvertToArrmorType(this.cell_type));
			}
			else
			{
				IS_mdl_Armor is_mdl_Armor = InventaryObjManager.GetItemFromBagId(this.item_guid) as IS_mdl_Armor;
				SkinsManager.UpdateSkin(is_mdl_Armor.SkinID, is_mdl_Armor.ArmorType);
			}
		}
	}

	private void Awake()
	{
		this.isEmpty = true;
		this.item_guid = Guid.Empty;
	}

	private void Start()
	{
		if ((this.cell_type == Cell_Type.CT_ARRMOR_BODY || this.cell_type == Cell_Type.CT_ARRMOR_HEAD || this.cell_type == Cell_Type.CT_ARRMOR_LEGS) && this.item_guid == Guid.Empty)
		{
			SkinsManager.UpdateSkin(0, this.ConvertToArrmorType(this.cell_type));
		}
	}

	public eIS_Arrmor_SUBT ConvertToArrmorType(Cell_Type ct)
	{
		if (ct == Cell_Type.CT_ARRMOR_BODY)
		{
			return eIS_Arrmor_SUBT.AS_BODY;
		}
		if (ct == Cell_Type.CT_ARRMOR_HEAD)
		{
			return eIS_Arrmor_SUBT.AS_HEAD;
		}
		if (ct == Cell_Type.CT_ARRMOR_LEGS)
		{
			return eIS_Arrmor_SUBT.AS_LEGS;
		}
		return eIS_Arrmor_SUBT.AS_BODY;
	}

	public int cell_id = -1;

	public Guid item_guid;

	public Cell_Type cell_type;

	public bool isEmpty = true;
}
