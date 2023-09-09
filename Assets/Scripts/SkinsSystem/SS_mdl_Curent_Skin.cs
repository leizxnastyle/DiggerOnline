using System;
using System.Collections.Generic;
using InventorySystem;

namespace SkinsSystem
{
	public class SS_mdl_Curent_Skin
	{
		public SS_mdl_Curent_Skin()
		{
			this._curent_skin = new Dictionary<eIS_Arrmor_SUBT, int>();
			this._curent_skin.Add(eIS_Arrmor_SUBT.AS_HEAD, -1);
			this._curent_skin.Add(eIS_Arrmor_SUBT.AS_BODY, -1);
			this._curent_skin.Add(eIS_Arrmor_SUBT.AS_LEGS, -1);
		}

		public void AddCurrentSkinType(eIS_Arrmor_SUBT skin_part, int skin_id)
		{
			this._curent_skin[skin_part] = skin_id;
		}

		public Dictionary<eIS_Arrmor_SUBT, int> GetCurrentSkin()
		{
			return this._curent_skin;
		}

		public Dictionary<eIS_Arrmor_SUBT, int> _curent_skin = new Dictionary<eIS_Arrmor_SUBT, int>();
	}
}
