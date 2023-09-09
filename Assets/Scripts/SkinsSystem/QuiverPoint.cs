using System;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

namespace SkinsSystem
{
	public class QuiverPoint : MonoBehaviour
	{
		private void OnEnable()
		{
			pnl_Inventory.isWeaponUpdate += this.HandleisWeaponIsUpdate;
		}

		private void OnDisable()
		{
			pnl_Inventory.isWeaponUpdate -= this.HandleisWeaponIsUpdate;
		}

		private void HandleisWeaponIsUpdate(List<is_InventoryCell> weapon_cell_list, List<is_InventoryCell> power_cell_list)
		{
			foreach (is_InventoryCell is_InventoryCell in power_cell_list)
			{
				if (!is_InventoryCell.isEmpty)
				{
					if (InventaryObjManager.GetItemFromBagId(is_InventoryCell.item_guid).ItemType == eIS_ItemType.IT_ARROW)
					{
						this.InstanceWeaponToMdl(InventaryObjManager.GetItemFromBagId(is_InventoryCell.item_guid).GoPath);
					}
					return;
				}
			}
			if (this.last_inst_model != null)
			{
				UnityEngine.Object.Destroy(this.last_inst_model);
			}
		}

		private void InstanceWeaponToMdl(string preff_path)
		{
			if (preff_path != string.Empty)
			{
				if (this.last_inst_model != null)
				{
					UnityEngine.Object.Destroy(this.last_inst_model);
				}
				this.last_inst_model = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(preff_path, typeof(GameObject)));
				Vector3 localPosition = this.last_inst_model.transform.localPosition;
				Vector3 localScale = this.last_inst_model.transform.localScale;
				Quaternion localRotation = this.last_inst_model.transform.localRotation;
				this.last_inst_model.transform.parent = base.transform;
				this.last_inst_model.transform.localPosition = localPosition;
				this.last_inst_model.transform.localScale = localScale;
				this.last_inst_model.transform.localRotation = localRotation;
			}
		}

		private GameObject last_inst_model;
	}
}
