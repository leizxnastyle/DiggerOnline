using System;
using PreviewModels;

public abstract class PreviewFactory
{
	public abstract SimplePreviewModel CreatePreview(CommonBlockKind commonKindBlock);
}
