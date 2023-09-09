using System;

namespace PreviewModels
{
	public abstract class SimplePreviewModel
	{
		public abstract CommonBlockKind CommonBlock { get; }

		public abstract bool IsPreview { get; }

		public abstract BlockKind Kind { get; }

		public abstract void ShowPreview();

		public abstract void DestroyPreview();
	}
}
