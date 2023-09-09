using System;
using InventorySystem.Utils;

namespace InventorySystem
{
	[Serializable]
	public class IS_mdl_Sword : IS_mdl_Item
	{
		public IS_mdl_Sword()
		{
		}

		public IS_mdl_Sword(int id, string name, int weight, string goPath, string iconName, eIS_ItemType itemType, int attack_pwr, int attack_rng, int attack_speed, eIS_Sword_SUBT sword_type, bool storage = false) : base(id, name, weight, goPath, iconName, itemType, storage, 1)
		{
			this._attack_pwr = attack_pwr;
			this._attack_rng = attack_rng;
			this._attack_speed = attack_speed;
			this._sword_type = sword_type;
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

		public int AttackRng
		{
			get
			{
				return this._attack_rng;
			}
			protected set
			{
				this._attack_rng = value;
			}
		}

		public int AttackSpeed
		{
			get
			{
				return this._attack_speed;
			}
			protected set
			{
				this._attack_speed = value;
			}
		}

		public eIS_Sword_SUBT SwordType
		{
			get
			{
				return this._sword_type;
			}
			protected set
			{
				this._sword_type = value;
			}
		}

		public override string Serialize()
		{
			return XMLSerializator<IS_mdl_Sword>.Serialize(this);
		}

		public override void Deserialize(string data)
		{
			IS_mdl_Sword o_in = XMLSerializator<IS_mdl_Sword>.Deserialize(data);
			XMLSerializator<IS_mdl_Sword>.CopyClassData(o_in, this);
		}

		protected int _attack_pwr;

		protected int _attack_rng;

		protected int _attack_speed;

		protected eIS_Sword_SUBT _sword_type;
	}
}
