using System;
using com.playGenesis.VkUnityPlugin;
using UnityEngine;

public class StarterSceneController : MonoBehaviour
{
	public void Start()
	{
		if (VkApi.VkApiInstance.IsUserLoggedIn)
		{
			return;
		}
		VkApi.VkApiInstance.Login();
	}

	public void TestCaptcha()
	{
		VKRequest httprequest = new VKRequest
		{
			url = "captcha.force",
			CallBackFunction = new Action<VKRequest>(this.OnCaptchaForse)
		};
		VkApi.VkApiInstance.Call(httprequest);
	}

	private void OnCaptchaForse(VKRequest r)
	{
		if (r.error != null)
		{
			UnityEngine.Object.FindObjectOfType<GlobalErrorHandler>().Notification.Notify(r);
			return;
		}
		UnityEngine.Debug.Log(r.response);
	}

	public void SendNotificationToAdmin()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("NotificationToAdmin");
	}

	public void FriendsGet()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Friends");
	}

	public void ShareScreenShot()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("ScreenShotShareDemo");
	}

	public void Logout()
	{
		VkApi.VkApiInstance.Logout();
		UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
	}
}
