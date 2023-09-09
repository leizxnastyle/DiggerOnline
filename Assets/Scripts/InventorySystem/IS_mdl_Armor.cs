using System;
using InventorySystem.Utils;

namespace InventorySystem
{
	[Serializable]
	public class IS_mdl_Armor : IS_mdl_Item
	{
		public IS_mdl_Armor()
		{
		}

		public IS_mdl_Armor(int id, string name, int weight, string goPath, string iconName, eIS_ItemType itemType, int defense, eIS_Arrmor_SUBT armor_type, int sid, bool storage = false) : base(id, name, weight, goPath, iconName, itemType, storage, 1)
		{
			this._defense = defense;
			this._armor_type = armor_type;
			this._skin_id = sid;
		}

		public int Defense
		{
			get
			{
				return this._defense;
			}
			protected set
			{
				this._defense = value;
			}
		}

		public eIS_Arrmor_SUBT ArmorType
		{
			get
			{
				return this._armor_type;
			}
			protected set
			{
				this._armor_type = value;
			}
		}

		public int SkinID
		{
			get
			{
				return this._skin_id;
			}
			protected set
			{
				this._skin_id = value;
			}
		}

		public override string Serialize()
		{
			return XMLSerializator<IS_mdl_Armor>.Serialize(this);
		}

		public override void Deserialize(string data)
		{
			IS_mdl_Armor o_in = XMLSerializator<IS_mdl_Armor>.Deserialize(data);
			XMLSerializator<IS_mdl_Armor>.CopyClassData(o_in, this);
		}

		protected int _defense;

		protected eIS_Arrmor_SUBT _armor_type;

		protected int _skin_id;
	}
}
