using System;
using InventorySystem.Utils;

namespace InventorySystem
{
	[Serializable]
	public class IS_mdl_Bow : IS_mdl_Item
	{
		public IS_mdl_Bow()
		{
		}

		public IS_mdl_Bow(int id, string name, int weight, string goPath, string iconName, eIS_ItemType itemType, int dmg, bool storage = false) : base(id, name, weight, goPath, iconName, itemType, storage, 1)
		{
			this._attack_pwr = dmg;
		}

		public int AttackPwr
		{
			get
			{
				return this._attack_pwr;
			}
			protected set
			{
				this._attack_pwr = value;
			}
		}

		public override string Serialize()
		{
			return XMLSerializator<IS_mdl_Bow>.Serialize(this);
		}

		public override void Deserialize(string data)
		{
			IS_mdl_Bow o_in = XMLSerializator<IS_mdl_Bow>.Deserialize(data);
			XMLSerializator<IS_mdl_Bow>.CopyClassData(o_in, this);
		}

		private int _attack_pwr;
	}
}
