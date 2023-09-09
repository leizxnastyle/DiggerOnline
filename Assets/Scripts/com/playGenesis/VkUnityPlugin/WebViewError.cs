using System;

namespace com.playGenesis.VkUnityPlugin
{
	public class WebViewError
	{
		public WebViewError(string url, string error)
		{
			this.FailedUrl = url;
			if (error.Contains("Canceled by user"))
			{
				this.ErrorType = WebViewErrorType.CanceledByUser;
			}
			else if (error.Contains("Network error"))
			{
				this.ErrorType = WebViewErrorType.NetworkError;
			}
		}

		public string FailedUrl { get; set; }

		public WebViewErrorType ErrorType = WebViewErrorType.UknownError;
	}
}
