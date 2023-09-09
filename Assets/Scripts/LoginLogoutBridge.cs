using System;
using com.playGenesis.VkUnityPlugin;
using UnityEngine;

public class LoginLogoutBridge
{
	public void Login()
	{
		this.jo = new AndroidJavaObject("com.playgenesis.vkunityplugin.Initializer", new object[0]);
		string val = this.FormLoginUrl();
		bool flag = this.jo.CallStatic<bool>("isVkAppPresent", new object[0]);
		if (VkApi.VkSetts.forceOAuth || !flag)
		{
			this.WebViewAuth();
			return;
		}
		this.jo.Set<string>("urlBase64", val);
		this.jo.Call("Init", new object[0]);
	}

	public void Logout()
	{
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.playgenesis.vkunityplugin.Initializer", new object[0]))
		{
			VkApi.VkApiInstance.onLoggedOut();
			androidJavaObject.Call("Logout", new object[]
			{
				VkApi.VkSetts.VkAppId.ToString()
			});
		}
	}

	private void WebViewAuth()
	{
		WebViewRequest webViewRequest = new WebViewRequest();
		webViewRequest.NavigateToUrl = this.FormLoginUrl();
		webViewRequest.CloseWhenNavigatedToUrl = "https://oauth.vk.com/blank.html";
		webViewRequest.CallbackAction = delegate(WebViewRequest w)
		{
			if (w.Error != null)
			{
				VkApi.VkApiInstance.SendMessage("AccessDeniedMessage", "-1#Canceled by user");
			}
			else
			{
				VkApi.VkApiInstance.SendMessage("ReceiveNewTokenMessage", VKToken.ParseFromAuthUrl(w.LastUrlWithParams));
			}
		};
		WebViewRequest element = webViewRequest;
		WebView.Instance.Add(element);
	}

	public string FormLoginUrl()
	{
		VkSettings vkSettings = Resources.Load<VkSettings>("VkSettings");
		string text = string.Join(",", vkSettings.scope.ToArray());
		return string.Concat(new object[]
		{
			"https://oauth.vk.com/authorize?client_id=",
			vkSettings.VkAppId,
			"&scope=",
			text,
			"&redirect_uri=https://oauth.vk.com/blank.html&display=mobile&forceOAuth=",
			vkSettings.forceOAuth.ToString(),
			"&revokeAccess=",
			vkSettings.revoke.ToString(),
			"&v=",
			vkSettings.apiVersion,
			"&response_type=token"
		});
	}

	private AndroidJavaObject jo;
}
