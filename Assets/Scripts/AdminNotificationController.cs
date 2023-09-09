using System;
using com.playGenesis.VkUnityPlugin;
using UnityEngine;
using UnityEngine.UI;

public class AdminNotificationController : MonoBehaviour
{
	public void SendNotificationToAdmin()
	{
		VKRequest httprequest = new VKRequest
		{
			url = "apps.sendRequest?user_id=" + this.input.text + "&text=Новая викторина Вконтакте бросает тебе вызов! Установи игру прямо сейчас!&type=request&name=test1",
			CallBackFunction = new Action<VKRequest>(this.OnAppSendRequest)
		};
		VKRequest vkrequest = new VKRequest();
		vkrequest.url = "apps.sendRequest?user_id=" + this.input.text + "&text=hello_from_vk_plugin2&type=request&name=sayhello2";
		vkrequest.CallBackFunction = new Action<VKRequest>(this.OnAppSendRequest);
		VkApi.VkApiInstance.Call(httprequest);
	}

	private void OnAppSendRequest(VKRequest r)
	{
		string value = string.Empty;
		try
		{
			value = r.error.error_code;
		}
		catch
		{
			value = string.Empty;
		}
		if (!string.IsNullOrEmpty(value))
		{
			GlobalErrorHandler.Instance.Notification.Notify(r);
			return;
		}
		GlobalErrorHandler.Instance.Notification.Notity(r.response);
	}

	public void Back()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("StarterScene");
	}

	public InputField input;
}
