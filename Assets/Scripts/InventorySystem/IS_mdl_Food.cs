using System;
using InventorySystem.Utils;

namespace InventorySystem
{
	[Serializable]
	public class IS_mdl_Food : IS_mdl_Item
	{
		public IS_mdl_Food()
		{
		}

		public IS_mdl_Food(int id, string name, int weight, string goPath, string iconName, eIS_ItemType itemType, int ladd, bool storage = false) : base(id, name, weight, goPath, iconName, itemType, storage, 1)
		{
			this._life_add = ladd;
		}

		public int Life_Add
		{
			get
			{
				return this._life_add;
			}
			protected set
			{
				this._life_add = value;
			}
		}

		public override string Serialize()
		{
			return XMLSerializator<IS_mdl_Food>.Serialize(this);
		}

		public override void Deserialize(string data)
		{
			IS_mdl_Food o_in = XMLSerializator<IS_mdl_Food>.Deserialize(data);
			XMLSerializator<IS_mdl_Food>.CopyClassData(o_in, this);
		}

		private int _life_add;
	}
}
