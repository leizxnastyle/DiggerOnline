using System;

namespace com.playGenesis.VkUnityPlugin
{
	public sealed class WebViewErrorType
	{
		private WebViewErrorType(int value, string name)
		{
			this.name = name;
			this.value = value;
		}

		public static explicit operator string(WebViewErrorType h)
		{
			return h.name;
		}

		public static readonly WebViewErrorType CanceledByUser = new WebViewErrorType(1, "Canceled by user");

		public static readonly WebViewErrorType NetworkError = new WebViewErrorType(3, "Network error");

		public static readonly WebViewErrorType UknownError = new WebViewErrorType(4, "Undefined error");

		private readonly string name;

		private readonly int value;
	}
}
