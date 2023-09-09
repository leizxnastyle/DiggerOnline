using System;
using System.Xml.Serialization;
using InventorySystem.Utils;

namespace InventorySystem
{
	[XmlInclude(typeof(IS_mdl_Food))]
	[XmlInclude(typeof(IS_mdl_Bow))]
	[XmlInclude(typeof(IS_mdl_Sword))]
	[XmlInclude(typeof(IS_mdl_Armor))]
	[Serializable]
	public class IS_mdl_Item : IISItem, IXMLS
	{
		public IS_mdl_Item()
		{
			this.Initialization();
		}

		public IS_mdl_Item(int id, string name, int weight, string goPath, string iconName, eIS_ItemType itemType, bool storage = false, int cn = 1)
		{
			this.Initialization();
			this._id = id;
			this._name = name;
			this._weight = weight;
			this._goPath = goPath;
			this._iconName = iconName;
			this._itemType = itemType;
			this._isStorage = storage;
			this._count = cn;
		}

		public int Id
		{
			get
			{
				return this._id;
			}
			protected set
			{
				this._id = value;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			protected set
			{
				this._name = value;
			}
		}

		public int Weight
		{
			get
			{
				return this._weight;
			}
			protected set
			{
				this._weight = value;
			}
		}

		public string GoPath
		{
			get
			{
				return this._goPath;
			}
			protected set
			{
				this._goPath = value;
			}
		}

		public string IconName
		{
			get
			{
				return this._iconName;
			}
			protected set
			{
				this._iconName = value;
			}
		}

		public eIS_ItemType ItemType
		{
			get
			{
				return this._itemType;
			}
			protected set
			{
				this._itemType = value;
			}
		}

		public bool IsStorage
		{
			get
			{
				return this._isStorage;
			}
			protected set
			{
				this._isStorage = value;
			}
		}

		public int Count
		{
			get
			{
				return this._count;
			}
			protected set
			{
				this._count = value;
			}
		}

		public T GetItemsFromType<T>(T obj)
		{
			return obj;
		}

		public virtual T GetItemType<T>() where T : IS_mdl_Item
		{
			return (T)((object)this);
		}

		public virtual void Initialization()
		{
		}

		public virtual string Serialize()
		{
			return XMLSerializator<IS_mdl_Item>.Serialize(this);
		}

		public virtual void Deserialize(string data)
		{
			IS_mdl_Item o_in = XMLSerializator<IS_mdl_Item>.Deserialize(data);
			XMLSerializator<IS_mdl_Item>.CopyClassData(o_in, this);
		}

		protected int _id;

		protected string _name;

		protected int _weight;

		protected string _goPath;

		protected string _iconName;

		protected eIS_ItemType _itemType;

		protected bool _isStorage;

		protected int _count;
	}
}
