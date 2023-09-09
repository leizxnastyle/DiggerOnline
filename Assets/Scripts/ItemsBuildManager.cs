using System;
using System.Collections.Generic;

public static class ItemsBuildManager
{
	internal static void Init()
	{
		ItemsBuildManager._game_builds = new Dictionary<StorePurchase, mdl_Item_Build>
		{
			{
				StorePurchase.HG_KNIGHT_BUILD_1,
				new mdl_Item_Build("HG_KNIGHT_BUILD_1", new List<int>
				{
					1
				}, new List<int>
				{
					1
				}, 1)
			},
			{
				StorePurchase.HG_KNIGHT_BUILD_2,
				new mdl_Item_Build("HG_KNIGHT_BUILD_2", new List<int>
				{
					1,
					102,
					2005
				}, new List<int>
				{
					1,
					1,
					1
				}, 2)
			},
			{
				StorePurchase.HG_KNIGHT_BUILD_3,
				new mdl_Item_Build("HG_KNIGHT_BUILD_3", new List<int>
				{
					1,
					100,
					102,
					2004,
					2005
				}, new List<int>
				{
					1,
					1,
					1,
					1,
					1
				}, 3)
			},
			{
				StorePurchase.HG_KNIGHT_BUILD_4,
				new mdl_Item_Build("HG_KNIGHT_BUILD_4", new List<int>
				{
					1,
					100,
					102,
					102,
					2003,
					2004,
					2005
				}, new List<int>
				{
					1,
					1,
					1,
					1,
					1,
					1,
					1
				}, 4)
			},
			{
				StorePurchase.HG_KNIGHT_BUILD_5,
				new mdl_Item_Build("HG_KNIGHT_BUILD_5", new List<int>
				{
					2,
					100,
					103,
					2009,
					2004,
					2005
				}, new List<int>
				{
					1,
					1,
					1,
					1,
					1,
					1
				}, 5)
			},
			{
				StorePurchase.HG_ARCHER_BUILD_1,
				new mdl_Item_Build("HG_KNIGHT_BUILD_1", new List<int>
				{
					200,
					102
				}, new List<int>
				{
					1,
					1
				}, 1)
			},
			{
				StorePurchase.HG_ARCHER_BUILD_2,
				new mdl_Item_Build("HG_KNIGHT_BUILD_2", new List<int>
				{
					7,
					200,
					102
				}, new List<int>
				{
					1,
					1,
					1
				}, 2)
			},
			{
				StorePurchase.HG_ARCHER_BUILD_3,
				new mdl_Item_Build("HG_KNIGHT_BUILD_3", new List<int>
				{
					7,
					200,
					201,
					102
				}, new List<int>
				{
					1,
					2,
					1,
					1
				}, 3)
			},
			{
				StorePurchase.HG_ARCHER_BUILD_4,
				new mdl_Item_Build("HG_KNIGHT_BUILD_4", new List<int>
				{
					7,
					200,
					201,
					102
				}, new List<int>
				{
					1,
					3,
					2,
					1
				}, 4)
			},
			{
				StorePurchase.HG_ARCHER_BUILD_5,
				new mdl_Item_Build("HG_KNIGHT_BUILD_5", new List<int>
				{
					8,
					200,
					201,
					202,
					100,
					102
				}, new List<int>
				{
					1,
					3,
					2,
					1,
					1,
					1
				}, 5)
			}
		};
	}

	internal static mdl_Item_Build GetBuild(StorePurchase sp)
	{
		if (ItemsBuildManager._game_builds.ContainsKey(sp))
		{
			return ItemsBuildManager._game_builds[sp];
		}
		return null;
	}

	private static Dictionary<StorePurchase, mdl_Item_Build> _game_builds;
}
