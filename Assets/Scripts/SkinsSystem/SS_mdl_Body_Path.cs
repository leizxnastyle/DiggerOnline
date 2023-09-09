using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;

namespace SkinsSystem
{
	public class SS_mdl_Body_Path
	{
		public SS_mdl_Body_Path()
		{
		}

		public SS_mdl_Body_Path(eIS_Arrmor_SUBT bpid, int sid, string bppn, int iid)
		{
			this.bp_id = bpid;
			this.skin_id = sid;
			this.bp_part_Name = (from s in bppn.Split(new char[]
			{
				','
			})
			select s).ToList<string>();
			this.item_id = iid;
		}

		public eIS_Arrmor_SUBT bp_id;

		public int skin_id;

		public int item_id;

		public List<string> bp_part_Name = new List<string>();
	}
}
