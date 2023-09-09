using System;
using System.Collections.Generic;
using PreviewModels;
using UnityEngine;

public class KindTest : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Update()
	{
		if ((UnityEngine.Input.GetKeyDown(KeyCode.Z) || UnityEngine.Input.GetKeyDown(KeyCode.X)) && !GameType.IsArcadeMode && !GameType.IsHungerGamesMode && !GameType.BattleMode())
		{
			if (Chat.IsEnabled() || MainMenu.Instance.MenuActive)
			{
				return;
			}
			if (WorldGameObjectX.Instance.CurrentBlockEntity == EntityType.RAIL)
			{
				return;
			}
			List<CommonBlockKind> list = new List<CommonBlockKind>();
			if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_FENCE) > 0)
			{
				list.Add(CommonBlockKind.Fence);
			}
			if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_HALF) > 0)
			{
				list.Add(CommonBlockKind.Half);
			}
			if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_QUARTER) > 0)
			{
				list.Add(CommonBlockKind.Quarter);
			}
			if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_DIAGONAL) > 0)
			{
				list.Add(CommonBlockKind.Diagonal);
			}
			if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR) > 0)
			{
				list.Add(CommonBlockKind.Stair);
			}
			if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_CORNER) > 0)
			{
				list.Add(CommonBlockKind.Corner);
			}
			if (ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR_CORNER) > 0)
			{
				list.Add(CommonBlockKind.StairCorner);
			}
			list.Add(CommonBlockKind.Default);
			int num = list.IndexOf(WorldGameObjectX.Instance.CurrentCommonKind);
			if (num != -1)
			{
				if (UnityEngine.Input.GetKeyDown(KeyCode.Z) && num > 0)
				{
					num--;
				}
				else if (UnityEngine.Input.GetKeyDown(KeyCode.X) && num < list.Count - 1)
				{
					num++;
				}
			}
			else
			{
				num = list.Count - 1;
			}
			WorldGameObjectX.Instance.CurrentCommonKind = list[num];
			KGUI.SetNodes("hud.custom_blocks.icon_01_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Fence, false);
			KGUI.SetNodes("hud.custom_blocks.icon_02_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Half, false);
			KGUI.SetNodes("hud.custom_blocks.icon_03_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Quarter, false);
			KGUI.SetNodes("hud.custom_blocks.icon_04_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Diagonal, false);
			KGUI.SetNodes("hud.custom_blocks.icon_05_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Stair, false);
			KGUI.SetNodes("hud.custom_blocks.icon_08_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.StairCorner, false);
			KGUI.SetNodes("hud.custom_blocks.icon_07_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Corner, false);
			KGUI.SetNodes("hud.custom_blocks.icon_06_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Default, false);
		}
		try
		{
			if (this.simplePreview == null)
			{
				this.simplePreview = this.previewFactory.CreatePreview(WorldGameObjectX.Instance.CurrentCommonKind);
				WorldGameObjectX.Instance.Preview = this.simplePreview;
			}
			if (this.simplePreview.CommonBlock != WorldGameObjectX.Instance.CurrentCommonKind)
			{
				throw new ArgumentException(string.Format("Diferent common block kind {0} : {1}", WorldGameObjectX.Instance.CurrentCommonKind, this.simplePreview.CommonBlock));
			}
			this.simplePreview.ShowPreview();
		}
		catch (ArgumentException)
		{
			if (this.simplePreview != null)
			{
				this.simplePreview.DestroyPreview();
				this.simplePreview = null;
				WorldGameObjectX.Instance.Preview = null;
			}
		}
	}

	private PreviewFactory previewFactory = new SimplePreview();

	private SimplePreviewModel simplePreview;
}
