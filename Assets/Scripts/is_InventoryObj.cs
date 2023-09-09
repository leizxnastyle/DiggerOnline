using System;
using System.Collections;
using InventorySystem;
using SkinsSystem;
using UnityEngine;

public class is_InventoryObj : MonoBehaviour
{
	private void Awake()
	{
		this.s_icon = base.transform.FindChild("item_icon").GetComponent<UISprite>();
		this.s_bg = base.transform.FindChild("bg").GetComponent<UISprite>();
		this.l_counter = base.transform.FindChild("item_count").GetComponent<UILabel>();
		this.l_counter.text = string.Empty;
		this.start_d = this.s_icon.depth;
		this._isMoveNow = false;
	}

	public void Init(IS_mdl_InventoryItem i)
	{
		this.item = i;
		this.s_icon.spriteName = IS_Manager.GetItemById(this.item.item_id).IconName;
		if (i.count > 1)
		{
			this.l_counter.text = i.count.ToString();
		}
	}

	private void OnPress(bool pressed)
	{
		try
		{
			if (!pressed)
			{
				this.s_icon.depth = this.start_d;
				this.s_bg.color = new Color(1f, 1f, 1f, 0f);
				RaycastHit raycastHit;
				if (Physics.Raycast(base.transform.position, Vector3.forward, out raycastHit))
				{
					if (raycastHit.collider.tag == "iCell" && raycastHit.collider.GetComponent<is_InventoryCell>().cell_type == Cell_Type.CT_OUT)
					{
						if (this.item.cell_id <= 35 && InventaryObjManager.ShowInventar)
						{
							HG_WorkController._player.DropItem(this.item.item_id);
							InventaryObjManager.inventary.LayOutFromInventory(this.item.bag_id);
							Cell_Type cell_type = InventaryObjManager.inv_cells[this.item.cell_id].cell_type;
							if (cell_type == Cell_Type.CT_ARRMOR_BODY || cell_type == Cell_Type.CT_ARRMOR_HEAD || cell_type == Cell_Type.CT_ARRMOR_LEGS)
							{
								IS_mdl_Armor is_mdl_Armor = IS_Manager.GetItemById(this.item.item_id) as IS_mdl_Armor;
								SkinsManager.UpdateSkin(0, is_mdl_Armor.ArmorType);
							}
							pnl_Inventory.ReUpdate();
							UnityEngine.Object.Destroy(base.gameObject);
						}
						else
						{
							this.MoveToCell(this.item.cell_id);
						}
					}
					else if (raycastHit.collider.tag == "iCell" && IS_Manager.CanPutInCell(this.item.item_id, raycastHit.collider.GetComponent<is_InventoryCell>().cell_type))
					{
						int cell_id = raycastHit.collider.GetComponent<is_InventoryCell>().cell_id;
						if (InventaryObjManager.inv_cells.ContainsKey(cell_id) && InventaryObjManager.inv_cells[cell_id].isEmpty)
						{
							this.MoveToCell(cell_id);
						}
						else if (InventaryObjManager.inv_cells.ContainsKey(cell_id) && !InventaryObjManager.inv_cells[cell_id].isEmpty && IS_Manager.CanPutInCell(InventaryObjManager.GetInvObjectByCellId(cell_id).item.item_id, InventaryObjManager.inv_cells[this.item.cell_id].cell_type))
						{
							this.ReplaceToCell(cell_id);
						}
						else
						{
							this.MoveToCell(this.item.cell_id);
						}
					}
					else
					{
						this.MoveToCell(this.item.cell_id);
					}
				}
				else
				{
					this.MoveToCell(this.item.cell_id);
				}
				base.StartCoroutine(this.WaitAndShow());
			}
			else
			{
				if (this._isMoveNow)
				{
					return;
				}
				if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
				{
					base.StartCoroutine(this.WaitAndMove());
					return;
				}
				this.isPress = true;
				this.s_bg.color = new Color(1f, 1f, 1f, 1f);
				Vector3 localPosition = base.transform.localPosition;
				localPosition.z = -180f;
				base.transform.localPosition = localPosition;
				this.s_icon.depth = 25;
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError(ex.StackTrace);
		}
		pnl_Inventory.IsTooltipShow(base.transform.position, false, this.item);
	}

	private IEnumerator WaitAndMove()
	{
		this._isMoveNow = true;
		this.TryFastMove();
		yield return new WaitForSeconds(0.5f);
		this._isMoveNow = false;
		yield break;
	}

	private void TryFastMove()
	{
		if (HG_WorkController.curent_chest_open == -1)
		{
			this.MoveToCell(IS_Manager.GetFastMove(this.item.item_id, InventaryObjManager.inv_cells[this.item.cell_id].cell_type));
		}
		else
		{
			this.MoveToCell(IS_Manager.GetFastMoveChest(this.item.item_id, this.item.cell_id));
		}
	}

	private void OnHover(bool isOver)
	{
		if (isOver && !this.isPress)
		{
			pnl_Inventory.IsTooltipShow(base.transform.position, true, this.item);
		}
		else
		{
			pnl_Inventory.IsTooltipShow(base.transform.position, false, this.item);
		}
	}

	private IEnumerator WaitAndShow()
	{
		yield return new WaitForSeconds(0.1f);
		this.isPress = false;
		this.OnHover(true);
		yield break;
	}

	private void CheckGost()
	{
		foreach (is_InventoryCell is_InventoryCell in InventaryObjManager.inv_cells.Values)
		{
			if (!is_InventoryCell.isEmpty)
			{
				if (InventaryObjManager.inventary.GetInventoryItem(is_InventoryCell.item_guid) == null)
				{
					InventaryObjManager.inv_cells[this.item.cell_id].item_guid = Guid.Empty;
					InventaryObjManager.inv_cells[this.item.cell_id].isEmpty = true;
				}
			}
		}
	}

	private void MoveToCell(int cell_id)
	{
		base.transform.position = InventaryObjManager.inv_cells[cell_id].transform.position;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.z = -5f;
		base.transform.localPosition = localPosition;
		InventaryObjManager.inv_cells[this.item.cell_id].item_guid = Guid.Empty;
		InventaryObjManager.inv_cells[this.item.cell_id].isEmpty = true;
		InventaryObjManager.CellsUpdate(this.item.cell_id);
		this.CheckChestMove(this.item.cell_id, cell_id);
		if (this.item.count == 1 || InventaryObjManager.ShowInventar || !InventaryObjManager.inventary.IsEnableInPlayerInventory(this.item.item_id))
		{
			this.item.cell_id = cell_id;
			InventaryObjManager.inv_cells[this.item.cell_id].item_guid = this.item.bag_id;
			InventaryObjManager.inv_cells[this.item.cell_id].isEmpty = false;
			InventaryObjManager.CellsUpdate(this.item.cell_id);
			Cell_Type cell_type = InventaryObjManager.inv_cells[this.item.cell_id].cell_type;
			if (cell_type == Cell_Type.CT_ARRMOR_BODY || cell_type == Cell_Type.CT_ARRMOR_HEAD || cell_type == Cell_Type.CT_ARRMOR_LEGS)
			{
				IS_mdl_Armor is_mdl_Armor = IS_Manager.GetItemById(this.item.item_id) as IS_mdl_Armor;
				SkinsManager.UpdateSkin(is_mdl_Armor.SkinID, is_mdl_Armor.ArmorType);
			}
		}
		pnl_Inventory.ReUpdate();
	}

	public void MoveToCellNoFree(int cell_id)
	{
		base.transform.position = InventaryObjManager.inv_cells[cell_id].transform.position;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.z = -5f;
		base.transform.localPosition = localPosition;
		this.item.cell_id = cell_id;
		InventaryObjManager.inv_cells[this.item.cell_id].item_guid = this.item.bag_id;
		InventaryObjManager.inv_cells[this.item.cell_id].isEmpty = false;
		InventaryObjManager.CellsUpdate(this.item.cell_id);
		pnl_Inventory.ReUpdate();
	}

	private void ReplaceToCell(int cell_id)
	{
		is_InventoryObj invObjectByCellId = InventaryObjManager.GetInvObjectByCellId(cell_id);
		int cell_id2 = this.item.cell_id;
		this.MoveToCellNoFree(cell_id);
		invObjectByCellId.MoveToCellNoFree(cell_id2);
	}

	private void CheckChestMove(int from_cell_id, int to_cell_id)
	{
		if (from_cell_id >= 100 && to_cell_id < 100)
		{
			WorldGameObjectX.Instance.photonView.RPC("GetOneItemFromChest", PhotonTargets.All, new object[]
			{
				HG_WorkController.curent_chest_open,
				this.item.item_id,
				to_cell_id
			});
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (from_cell_id < 100 && to_cell_id >= 100)
		{
			WorldGameObjectX.Instance.photonView.RPC("AddItemToChest", PhotonTargets.All, new object[]
			{
				HG_WorkController.curent_chest_open,
				this.item.item_id
			});
			InventaryObjManager.inventary.LayOutFromInventory(this.item.bag_id);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public IS_mdl_InventoryItem item;

	private UISprite s_icon;

	private UISprite s_bg;

	private UILabel l_counter;

	private int start_d;

	private bool isPress;

	private bool _isMoveNow;
}
