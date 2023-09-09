using System;
using System.Collections.Generic;

public class mdl_Item_Build
{
	public mdl_Item_Build()
	{
	}

	public mdl_Item_Build(string bn, List<int> biid, List<int> bc, int bl)
	{
		this.build_name = bn;
		this.build_items_id = biid;
		this.build_counts = bc;
		this.build_level = bl;
	}

	public string build_name { get; private set; }

	public string build_icon_name { get; private set; }

	public List<int> build_items_id { get; private set; }

	public List<int> build_counts { get; private set; }

	public int build_level { get; private set; }
}
