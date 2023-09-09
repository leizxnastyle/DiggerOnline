using System;
using System.Collections.Generic;
using InventorySystem;
using LSD.Events;
using UnityEngine;

public class pnl_Inventory : MonoBehaviour
{
	private static event Action isReupdateStat;

	public static event Action isCloseWindow;

	public static event Action<List<is_InventoryCell>, List<is_InventoryCell>> isWeaponUpdate;

	public static List<is_InventoryCell> all_weapon_slot
	{
		get
		{
			if (pnl_Inventory.sweapon_cell != null)
			{
				List<is_InventoryCell> list = new List<is_InventoryCell>(pnl_Inventory.sweapon_cell);
				list.AddRange(pnl_Inventory.spower_cell);
				return list;
			}
			return new List<is_InventoryCell>();
		}
	}

	private void OnEnable()
	{
		pnl_Inventory.isReupdateStat = (Action)Delegate.Combine(pnl_Inventory.isReupdateStat, new Action(this.HandleisReupdateStat));
	}

	private void OnDisable()
	{
		pnl_Inventory.isReupdateStat = (Action)Delegate.Remove(pnl_Inventory.isReupdateStat, new Action(this.HandleisReupdateStat));
	}

	private void Awake()
	{
		pnl_Inventory.pInventory = this;
		pnl_Inventory.lb_life = base.transform.FindChild("RightSide").FindChild("PlayerStats").FindChild("lb_life").GetComponent<UILabel>();
		pnl_Inventory.lb_armor = base.transform.FindChild("RightSide").FindChild("PlayerStats").FindChild("lb_armor").GetComponent<UILabel>();
		pnl_Inventory.lb_dmg = base.transform.FindChild("RightSide").FindChild("PlayerStats").FindChild("lb_dmg").GetComponent<UILabel>();
		pnl_Inventory.lb_player_name = base.transform.FindChild("lb_Name").GetComponent<UILabel>();
	}

	private void Start()
	{
		this.b_close = base.transform.FindChild("ibtn_close").GetComponent<uii_BasicControl>();
		this.b_close.addEventListener("button_click", new LSD_EventHandler(this.ButtonClickHandler));
		this.b_left_arrow = base.transform.FindChild("RightSide").FindChild("ibtn_left_arrow").GetComponent<uii_BasicControl>();
		this.b_left_arrow.addEventListener("button_press", new LSD_EventHandler(this.ButtonClickHandler));
		this.b_right_arrow = base.transform.FindChild("RightSide").FindChild("ibtn_right_arrow").GetComponent<uii_BasicControl>();
		this.b_right_arrow.addEventListener("button_press", new LSD_EventHandler(this.ButtonClickHandler));
		pnl_Inventory.tooltip = base.transform.FindChild("Tooltip").GetComponent<pnl_Tooltip>();
		pnl_Inventory.lb_life.text = "100";
		pnl_Inventory.lb_armor.text = "0";
		pnl_Inventory.lb_dmg.text = "0";
		pnl_Inventory.sweapon_cell = this.weapon_cell;
		pnl_Inventory.spower_cell = this.power_cell;
	}

	public void ShowInventory()
	{
		pnl_Inventory.lb_player_name.text = HG_WorkController._player.PlayerName;
		InventaryObjManager.ShowInventar = true;
		this.char_mdl.SetActive(true);
		this.CharPanel.SetActive(true);
		this.ChestPanel.SetActive(false);
		if (InventaryObjManager.inventary != null)
		{
			InventaryObjManager.inventary.chest_items = new Dictionary<Guid, IS_mdl_InventoryItem>();
			InventaryObjManager.inventary.invManager.ShowInventory();
			pnl_Inventory.tooltip.Hide();
		}
	}

	public void ShowChest(List<int> item_ids)
	{
		pnl_Inventory.lb_player_name.text = HG_WorkController._player.PlayerName;
		this.char_mdl.SetActive(false);
		this.CharPanel.SetActive(false);
		this.ChestPanel.SetActive(true);
		InventaryObjManager.AddItemsToChest(item_ids);
		pnl_Inventory.tooltip.Hide();
	}

	private void ButtonClickHandler(object target, EventArgs args)
	{
		if (target == this.b_close)
		{
			if (pnl_Inventory.isCloseWindow != null)
			{
				pnl_Inventory.isCloseWindow();
			}
		}
		else if (target == this.b_left_arrow)
		{
			if (!this.isLPress)
			{
				this.isLPress = true;
			}
			else
			{
				this.isLPress = false;
			}
		}
		else if (target == this.b_right_arrow)
		{
			if (!this.isRPress)
			{
				this.isRPress = true;
			}
			else
			{
				this.isRPress = false;
			}
		}
	}

	private void Update()
	{
		if (this.isLPress)
		{
			this.char_mdl.transform.Rotate(Vector3.up, Time.deltaTime * 50f);
		}
		else if (this.isRPress)
		{
			this.char_mdl.transform.Rotate(Vector3.up, -Time.deltaTime * 50f);
		}
	}

	public static void ReUpdate()
	{
		if (pnl_Inventory.isReupdateStat != null)
		{
			pnl_Inventory.isReupdateStat();
		}
		if (pnl_Inventory.isWeaponUpdate != null)
		{
			pnl_Inventory.isWeaponUpdate(pnl_Inventory.sweapon_cell, pnl_Inventory.spower_cell);
		}
	}

	public static void IsTooltipShow(Vector3 poss, bool isShow, IS_mdl_InventoryItem item)
	{
		IS_mdl_Item itemById = IS_Manager.GetItemById(item.item_id);
		if (isShow)
		{
			pnl_Inventory.tooltip.Show(poss, itemById, 0);
		}
		else
		{
			pnl_Inventory.tooltip.Hide();
		}
	}

	public void HandleisReupdateStat()
	{
		int num = 0;
		foreach (is_InventoryCell is_InventoryCell in this.arrmor_cell)
		{
			if (is_InventoryCell.isEmpty)
			{
				if (is_InventoryCell.cell_type == Cell_Type.CT_ARRMOR_HEAD)
				{
					num++;
				}
				else if (is_InventoryCell.cell_type == Cell_Type.CT_ARRMOR_BODY)
				{
					num += 3;
				}
				else if (is_InventoryCell.cell_type == Cell_Type.CT_ARRMOR_LEGS)
				{
					num += 2;
				}
			}
			else
			{
				IS_mdl_Armor is_mdl_Armor = (IS_mdl_Armor)InventaryObjManager.GetItemFromBagId(is_InventoryCell.item_guid);
				if (is_mdl_Armor != null)
				{
					num += is_mdl_Armor.Defense;
				}
			}
		}
		int num2 = 0;
		foreach (is_InventoryCell is_InventoryCell2 in this.weapon_cell)
		{
			if (!is_InventoryCell2.isEmpty)
			{
				IS_mdl_Item itemFromBagId = InventaryObjManager.GetItemFromBagId(is_InventoryCell2.item_guid);
				if (itemFromBagId is IS_mdl_Sword)
				{
					int attackPwr = IS_Manager.GetCurentMdlData<IS_mdl_Sword>((IS_mdl_Sword)itemFromBagId).AttackPwr;
					if (attackPwr > num2)
					{
						num2 = attackPwr;
					}
				}
				else if (itemFromBagId is IS_mdl_Bow)
				{
					int attackPwr2 = IS_Manager.GetCurentMdlData<IS_mdl_Bow>((IS_mdl_Bow)itemFromBagId).AttackPwr;
					if (attackPwr2 > num2)
					{
						num2 = attackPwr2;
					}
				}
			}
		}
		pnl_Inventory.lb_armor.text = num.ToString();
		pnl_Inventory.lb_dmg.text = num2.ToString();
		pnl_Inventory.lb_life.text = HG_WorkController.GetPlayerLife().ToString();
		HG_WorkController.SetDMGAndArrmor(num2, num);
	}

	private uii_BasicControl b_close;

	private uii_BasicControl b_left_arrow;

	private uii_BasicControl b_right_arrow;

	private static UILabel lb_armor;

	public static UILabel lb_life;

	private static UILabel lb_dmg;

	private static UILabel lb_player_name;

	public List<is_InventoryCell> arrmor_cell;

	public List<is_InventoryCell> weapon_cell;

	public List<is_InventoryCell> power_cell;

	public static pnl_Tooltip tooltip;

	public static List<is_InventoryCell> sweapon_cell;

	public static List<is_InventoryCell> spower_cell;

	public GameObject char_mdl;

	public GameObject CharPanel;

	public GameObject ChestPanel;

	public static pnl_Inventory pInventory;

	private bool isRPress;

	private bool isLPress;
}
