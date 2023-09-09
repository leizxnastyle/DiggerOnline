using System;
using InventorySystem.Utils;

namespace InventorySystem
{
	[Serializable]
	public class IS_mdl_Arrow : IS_mdl_Item
	{
		public IS_mdl_Arrow()
		{
		}

		public IS_mdl_Arrow(int id, string name, int weight, string goPath, string iconName, eIS_ItemType itemType, int dmg, bool storage, int count) : base(id, name, weight, goPath, iconName, itemType, storage, count)
		{
			this._attack_bonus = dmg;
		}

		public int AttackBonus
		{
			get
			{
				return this._attack_bonus;
			}
			protected set
			{
				this._attack_bonus = value;
			}
		}

		public override string Serialize()
		{
			return XMLSerializator<IS_mdl_Arrow>.Serialize(this);
		}

		public override void Deserialize(string data)
		{
			IS_mdl_Arrow o_in = XMLSerializator<IS_mdl_Arrow>.Deserialize(data);
			XMLSerializator<IS_mdl_Arrow>.CopyClassData(o_in, this);
		}

		private int _attack_bonus;
	}
}
