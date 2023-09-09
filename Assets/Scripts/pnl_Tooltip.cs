using System;
using InventorySystem;
using UnityEngine;

public class pnl_Tooltip : MonoBehaviour
{
	private void Awake()
	{
		this.lb_name = base.transform.FindChild("lb_item_name").GetComponent<UILabel>();
		this.lb_stat = base.transform.FindChild("lb_item_best_count").GetComponent<UILabel>();
		this.sp_icon = base.transform.FindChild("item_icon").GetComponent<UISprite>();
		this.main_c = this.lb_name.color;
		this.fx_c = this.lb_name.effectColor;
	}

	private void Start()
	{
		this.Hide();
	}

	public void Show(Vector3 cel_pos, IS_mdl_Item item, int iconSize = 0)
	{
		this.isShow = true;
		this.lb_name.text = Localize.GetText(item.Name, null);
		if (item.ItemType == eIS_ItemType.IT_ARRMOR)
		{
			IS_mdl_Armor is_mdl_Armor = item as IS_mdl_Armor;
			this.lb_stat.text = is_mdl_Armor.Defense + "\n" + Localize.GetText("tt_Defense", null);
		}
		else if (item.ItemType == eIS_ItemType.IT_SWORD)
		{
			IS_mdl_Sword is_mdl_Sword = item as IS_mdl_Sword;
			this.lb_stat.text = is_mdl_Sword.AttackPwr + "\n" + Localize.GetText("tt_Attack", null);
		}
		else if (item.ItemType == eIS_ItemType.IT_BOW)
		{
			IS_mdl_Bow is_mdl_Bow = item as IS_mdl_Bow;
			this.lb_stat.text = is_mdl_Bow.AttackPwr + "\n" + Localize.GetText("tt_Attack", null);
		}
		else if (item.ItemType == eIS_ItemType.IT_FOOD)
		{
			IS_mdl_Food is_mdl_Food = item as IS_mdl_Food;
			this.lb_stat.text = string.Concat(new object[]
			{
				"+",
				is_mdl_Food.Life_Add,
				"\n",
				Localize.GetText("tt_Medication", null)
			});
		}
		else if (item.ItemType == eIS_ItemType.IT_ARROW)
		{
			IS_mdl_Arrow is_mdl_Arrow = item as IS_mdl_Arrow;
			this.lb_stat.text = string.Concat(new object[]
			{
				"+",
				is_mdl_Arrow.AttackBonus,
				"\n",
				Localize.GetText("tt_Atk_Bonus", null)
			});
		}
		this.lb_stat.color = this.main_c;
		this.lb_stat.effectColor = this.fx_c;
		this.lb_name.color = this.main_c;
		this.lb_name.effectColor = this.fx_c;
		this.sp_icon.spriteName = item.IconName;
		if (iconSize != 0)
		{
			this.sp_icon.transform.localScale = new Vector3((float)iconSize, (float)iconSize, 1f);
		}
		base.transform.position = cel_pos;
		Vector3 localPosition = base.transform.localPosition;
		localPosition.z = -200f;
		if (cel_pos.x < 0f)
		{
			localPosition.x += 140f;
		}
		else
		{
			localPosition.x -= 140f;
		}
		base.transform.localPosition = localPosition;
	}

	public void Hide()
	{
		if (base.transform != null)
		{
			base.transform.position = new Vector3(10000f, 0f, -20f);
		}
	}

	public void HideShop()
	{
		this.isShow = false;
		base.transform.position = new Vector3(10000f, 0f, -20f);
	}

	public bool IsShowNow()
	{
		return this.isShow;
	}

	private UILabel lb_name;

	private UILabel lb_stat;

	private UISprite sp_icon;

	private bool isShow;

	private Color main_c;

	private Color fx_c;
}
