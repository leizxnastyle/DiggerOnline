using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class HG_WorkController : MonoBehaviour
{
	public static bool IsStartPlay
	{
		get
		{
			return HG_WorkController.hgstatus != GameStatus.GS_WAIT && HG_WorkController.hgstatus != GameStatus.GS_PREPEA;
		}
	}

	private void OnDisable()
	{
		pnl_Inventory.isCloseWindow -= this.HandleisCloseInventoryWindow;
		MainMenu.isInventoryMenuOpen -= this.HandleisOpenInventoryWindow;
		MainMenu.isInventoryMenuClose -= this.HandleisCloseInventoryWindow;
		MainMenu.isInventoryMenuClose -= this.HandleisCloseInventoryWindow;
	}

	private void Awake()
	{
		HG_WorkController.kills = 0;
		HG_WorkController.golds = 0;
		HG_WorkController.hgstatus = GameStatus.GS_WAIT;
		this._hg_is_ss_preff = Resources.Load<GameObject>("DWork/Prefabs/InventorySystem");
		HG_WorkController._player = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>();
		this.w_cell_list = new List<is_InventoryCell>();
		this.p_cell_list = new List<is_InventoryCell>();
		HG_WorkController.isT = false;
	}

	public void Init(MainMenu mm)
	{
		this.OnDisable();
		this.Awake();
		HG_WorkController.hgstatus = GameStatus.GS_WAIT;
		if (!GameObject.Find("InventorySystem(Clone)"))
		{
			this.inventoryMenu = UnityEngine.Object.Instantiate<GameObject>(this._hg_is_ss_preff);
			HG_WorkController._menu_big_class = mm;
			this.HandleisCloseInventoryWindow();
			pnl_Inventory.isCloseWindow += this.HandleisCloseInventoryWindow;
			pnl_Inventory.isWeaponUpdate += this.HandleisWeaponIsUpdate;
			MainMenu.isInventoryMenuOpen += this.HandleisOpenInventoryWindow;
			MainMenu.isInventoryMenuClose += this.HandleisCloseInventoryWindow;
		}
		KGUI.SetNodes("hud.crosshair", false, false);
	}

	public void Init()
	{
		this.Awake();
		HG_WorkController.hgstatus = GameStatus.GS_WAIT;
		HG_WorkController._menu_big_class = MainMenu.Instance;
	}

	public void SetLastWeapon(int ls, int lc)
	{
		this.select_weapon = ls;
		this.select_cell = lc;
	}

	private void HandleisWeaponIsUpdate(List<is_InventoryCell> weapon_cell_list, List<is_InventoryCell> power_cell_list)
	{
		int num = this.select_weapon;
		int num2 = this.select_cell;
		if (weapon_cell_list != null && power_cell_list != null)
		{
			this.w_cell_list = weapon_cell_list;
			this.p_cell_list = power_cell_list;
			int num3 = 0;
			bool flag = false;
			foreach (is_InventoryCell is_InventoryCell in weapon_cell_list)
			{
				if (!is_InventoryCell.isEmpty)
				{
					IS_mdl_Item itemFromBagId = InventaryObjManager.GetItemFromBagId(is_InventoryCell.item_guid);
					if (itemFromBagId != null)
					{
						if (!flag)
						{
							this.select_weapon = itemFromBagId.Id;
							flag = true;
							this.select_cell = num3;
						}
						else if (itemFromBagId.Id == num && num3 == num2)
						{
							this.select_weapon = num;
							flag = true;
							this.select_cell = num2;
						}
					}
				}
				else if (!flag)
				{
					this.select_weapon = -1;
				}
				num3++;
			}
			List<int> list = new List<int>();
			foreach (is_InventoryCell is_InventoryCell2 in power_cell_list)
			{
				if (InventaryObjManager.GetItemFromBagId(is_InventoryCell2.item_guid) == null)
				{
					is_InventoryCell2.isEmpty = true;
				}
				else
				{
					if (!is_InventoryCell2.isEmpty && InventaryObjManager.GetItemFromBagId(is_InventoryCell2.item_guid).ItemType == eIS_ItemType.IT_FOOD)
					{
						IS_mdl_Food is_mdl_Food = InventaryObjManager.GetItemFromBagId(is_InventoryCell2.item_guid) as IS_mdl_Food;
						if (is_mdl_Food != null)
						{
							if (!flag)
							{
								this.select_weapon = is_mdl_Food.Id;
								flag = true;
								this.select_cell = num3;
							}
							else if (is_mdl_Food.Id == num && num3 == num2)
							{
								this.select_weapon = num;
								flag = true;
								this.select_cell = num2;
							}
						}
					}
					else if (!is_InventoryCell2.isEmpty && InventaryObjManager.GetItemFromBagId(is_InventoryCell2.item_guid).ItemType == eIS_ItemType.IT_ARROW)
					{
						IS_mdl_Arrow is_mdl_Arrow = InventaryObjManager.GetItemFromBagId(is_InventoryCell2.item_guid) as IS_mdl_Arrow;
						if (is_mdl_Arrow != null)
						{
							list.Add(is_mdl_Arrow.Id);
						}
					}
					else if (!flag)
					{
						this.select_weapon = -1;
					}
					num3++;
				}
			}
			HG_WorkController._menu_big_class.AddWeaponFromID(this.select_weapon, this.select_cell);
			HG_WorkController._player.SelectWeapon(this.select_weapon);
			HG_WorkController._player.SetWeaponAll(list);
		}
	}

	public void WeaponIsUpdate()
	{
		if (this.w_cell_list != null && this.p_cell_list != null)
		{
			this.HandleisWeaponIsUpdate(this.w_cell_list, this.p_cell_list);
		}
		else
		{
			HG_WorkController._menu_big_class.AddWeaponFromID(-1, 1);
		}
	}

	private void HandleisOpenInventoryWindow()
	{
		if (GameType.IsHungerGamesMode)
		{
			if (!HG_WorkController.isT || (HG_WorkController._player.CurentLife > 0 && HG_WorkController._player.PlayerTeam != 0))
			{
				HG_WorkController.curent_chest_open = -1;
				this.inventoryMenu.SetActive(true);
				pnl_Inventory.pInventory.ShowInventory();
				MainMenu.EnableMouseWork();
				pnl_Inventory.lb_life.text = HG_WorkController.GetPlayerLife().ToString();
			}
			else
			{
				HG_WorkController._menu_big_class.HideMenu();
			}
		}
	}

	private void HandleisCloseInventoryWindow()
	{
		if (GameType.IsHungerGamesMode)
		{
			HG_WorkController.curent_chest_open = -1;
			MainMenu.DisableMouseWork();
			HG_WorkController._menu_big_class.HideMenu();
			this.inventoryMenu.SetActive(false);
			base.StartCoroutine(this.WaitFoActive());
			KGUI.FindNode("hud", false).gameObject.SetActive(true);
			HG_WorkController._menu_big_class.AddWeaponFromID(this.select_weapon, this.select_cell);
		}
	}

	private IEnumerator WaitFoActive()
	{
		while (this.inventoryMenu != null && !this.inventoryMenu.activeSelf)
		{
			yield return new WaitForSeconds(0.1f);
		}
		pnl_Inventory.ReUpdate();
		yield return null;
		yield break;
	}

	public void OpenInventory()
	{
		this.HandleisOpenInventoryWindow();
	}

	public void CloseInventory()
	{
		this.HandleisCloseInventoryWindow();
	}

	public void OpenChest(List<int> items, int chest_id)
	{
		HG_WorkController.curent_chest_open = chest_id;
		pnl_Inventory.pInventory.ShowChest(items);
		MainMenu.EnableMouseWork();
		this.inventoryMenu.SetActive(true);
		KGUI.FindNode("hud", false).gameObject.SetActive(false);
		pnl_Inventory.lb_life.text = HG_WorkController.GetPlayerLife().ToString();
	}

	public static void UpdateSkin(int body_part_id, int skin_id)
	{
		if (HG_WorkController._player != null)
		{
			HG_WorkController._player.UpdateSkin(body_part_id, skin_id);
		}
	}

	public static int GetPlayerLife()
	{
		if (HG_WorkController._player != null)
		{
			return HG_WorkController._player.CurentLife;
		}
		return 0;
	}

	public static void SetDMGAndArrmor(int dmg, int arrmor)
	{
		if (HG_WorkController._player != null)
		{
			HG_WorkController._player.SetDMGAndArrmor(dmg, arrmor);
		}
	}

	internal IEnumerator UpdateInventory()
	{
		GameObject obj_ply = this.inventoryMenu.transform.FindChild("Player").gameObject;
		GameObject inv_pnl = this.inventoryMenu.transform.FindChild("UI Root (2D)").FindChild("Camera").FindChild("Anchor").FindChild("Inventary").gameObject;
		Vector3 beckPosition = obj_ply.transform.localPosition;
		inv_pnl.transform.localPosition = new Vector3(100000f, 0f, 0f);
		obj_ply.transform.localPosition = new Vector3(100000f, 0f, 0f);
		yield return new WaitForSeconds(0.3f);
		try
		{
			this.HandleisOpenInventoryWindow();
		}
		catch (Exception ex)
		{
			Exception e = ex;
			UnityEngine.Debug.LogError(e.StackTrace);
		}
		yield return new WaitForSeconds(0.5f);
		try
		{
			pnl_Inventory.pInventory.HandleisReupdateStat();
		}
		catch (Exception ex2)
		{
			Exception e2 = ex2;
			UnityEngine.Debug.LogError(e2.StackTrace);
		}
		try
		{
			this.HandleisCloseInventoryWindow();
		}
		catch (Exception ex3)
		{
			Exception e3 = ex3;
			UnityEngine.Debug.Log(e3.StackTrace);
		}
		yield return new WaitForSeconds(0.4f);
		inv_pnl.transform.localPosition = new Vector3(0f, 0f, -20f);
		obj_ply.transform.localPosition = beckPosition;
		HG_WorkController.isT = true;
		yield break;
	}

	internal static bool CanOpenInventory()
	{
		return HG_WorkController.hgstatus >= GameStatus.GS_START;
	}

	public static string SPPOINT_NAME = "HG_ARENA_SPAWN_POINT";

	private GameObject _hg_is_ss_preff;

	private GameObject _hud;

	private GameObject inventoryMenu;

	private int select_weapon = -1;

	public int select_cell = -1;

	private static MainMenu _menu_big_class;

	public static PlayerNetwork _player;

	public static Vector3 CurStartSpawn = Vector3.zero;

	public static int kills = 0;

	public static int golds = 0;

	public static int curent_chest_open = -1;

	private List<is_InventoryCell> w_cell_list;

	private List<is_InventoryCell> p_cell_list;

	public static GameStatus hgstatus = GameStatus.GS_WAIT;

	public static bool isT = false;
}
