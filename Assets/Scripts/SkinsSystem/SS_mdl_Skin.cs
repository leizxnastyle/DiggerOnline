using System;
using InventorySystem;
using UnityEngine;

namespace SkinsSystem
{
	public class SS_mdl_Skin
	{
		public SS_mdl_Body_Path GetPart(eIS_Arrmor_SUBT p_type)
		{
			if (p_type == eIS_Arrmor_SUBT.AS_HEAD)
			{
				return this.head;
			}
			if (p_type == eIS_Arrmor_SUBT.AS_BODY)
			{
				return this.body;
			}
			if (p_type == eIS_Arrmor_SUBT.AS_LEGS)
			{
				return this.legs;
			}
			return null;
		}

		public int skin_id;

		public string skin_name;

		public GameObject skin_obj;

		public SS_mdl_Body_Path head;

		public SS_mdl_Body_Path body;

		public SS_mdl_Body_Path legs;

		public int[] item_id = new int[3];
	}
}
