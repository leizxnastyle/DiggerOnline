using System;

namespace com.playGenesis.VkUnityPlugin
{
	public sealed class ImageType
	{
		private ImageType(int value, string name)
		{
			this.name = name;
			this.value = value;
		}

		public override string ToString()
		{
			return this.name;
		}

		public static explicit operator string(ImageType i)
		{
			return i.name;
		}

		private readonly string name;

		private readonly int value;

		public static readonly ImageType Jpeg = new ImageType(1, "image/jpeg");

		public static readonly ImageType Png = new ImageType(2, "image/png");
	}
}
