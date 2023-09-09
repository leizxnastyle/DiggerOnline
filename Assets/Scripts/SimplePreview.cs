using System;
using PreviewModels;

public class SimplePreview : PreviewFactory
{
	public override SimplePreviewModel CreatePreview(CommonBlockKind commonKindBlock)
	{
		switch (commonKindBlock)
		{
		case CommonBlockKind.Fence:
			return new FencePreview();
		case CommonBlockKind.Diagonal:
			return new DiagonalPreview();
		case CommonBlockKind.Corner:
			return new CornerPreview();
		case CommonBlockKind.StairCorner:
			return new CornerStairPreview();
		}
		throw new ArgumentException("An invalid CommonKindBlock: " + commonKindBlock.ToString());
	}
}
