using System;

namespace com.playGenesis.VkUnityPlugin
{
	public class Eroor_17_Behaviour : QueueWorker<VKRequest>
	{
		private void Start()
		{
			this._vkapi = VkApi.VkApiInstance;
		}

		protected override void StartProcessing()
		{
			WebView.Instance.Add(new WebViewRequest
			{
				CallbackAction = new Action<WebViewRequest>(this.OnRequestFinished),
				NavigateToUrl = Utilities.ParseConfirmationUrl(this._current.Element.response),
				CloseWhenNavigatedToUrl = "https://oauth.vk.com/blank.html"
			});
		}

		private void OnRequestFinished(WebViewRequest e)
		{
			if (e.Error == null)
			{
				this._vkapi.Call(this._current.Element);
				base.ProccessNext();
			}
			else
			{
				this._current.Element.error.error_code = "15";
				this._current.Element.error.error_msg = "Access Denied";
				this._current.Element.CallBackFunction(this._current.Element);
				base.ProccessNext();
			}
		}

		private VkApi _vkapi;
	}
}
