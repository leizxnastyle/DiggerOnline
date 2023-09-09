using System;
using UnityEngine;

namespace com.playGenesis.VkUnityPlugin
{
	public class WebView : QueueWorker<WebViewRequest>
	{
		public event Action<string> WebViewDoneEvent;

		protected void OpenWebView(string openurl, string closeurl)
		{
			this.jo = new AndroidJavaObject("com.playgenesis.vkunityplugin.Initializer", new object[0]);
			this.jo.Call("OpenWebView", new object[]
			{
				openurl,
				closeurl
			});
		}

		private void Awake()
		{
			if (WebView.Instance == null)
			{
				WebView.Instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		protected override void StartProcessing()
		{
			this.OpenWebView(this._current.Element.NavigateToUrl, this._current.Element.CloseWhenNavigatedToUrl);
		}

		public string parseErrorFormUrl(string url)
		{
			if (url.Contains("cancel=1") || url.Contains("fail=1") || url.Contains("error=access_denied"))
			{
				return "Canceled by user";
			}
			if (url.Contains("network_error=1"))
			{
				return "Network error";
			}
			return null;
		}

		private void OnWebViewDoneIntrnal(string url)
		{
			UnityEngine.Debug.Log("InternalWebView");
			this._current.Element.LastUrlWithParams = url;
			string text = this.parseErrorFormUrl(url);
			if (!string.IsNullOrEmpty(text))
			{
				this._current.Element.Error = new WebViewError(url, text);
			}
			this._current.Element.CallbackAction(this._current.Element);
			base.ProccessNext();
		}

		public void WebViewDone(string url)
		{
			UnityEngine.Debug.Log("webview done with url " + url);
			if (this.WebViewDoneEvent != null)
			{
				this.WebViewDoneEvent(url);
			}
			this.OnWebViewDoneIntrnal(url);
		}

		private AndroidJavaObject jo;

		public static WebView Instance;
	}
}
