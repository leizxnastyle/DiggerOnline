using System;
using com.playGenesis.VkUnityPlugin;
using UnityEngine;

public class LoginController : MonoBehaviour
{
	public void Start()
	{
		this.vkapi = VkApi.VkApiInstance;
		if (this.vkapi.IsUserLoggedIn)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("StarterScene");
		}
	}

	public void LoginToVK()
	{
		VkApi.VkSetts.forceOAuth = false;
		this.vkapi.LoggedIn += this.onVKLogin;
		this.vkapi.Login();
	}

	public void LoginVKOauth()
	{
		this.vkapi.LoggedIn += this.onLogin;
		this.vkapi.LoggedOut += this.onLogout;
		VkApi.VkSetts.forceOAuth = true;
		this.vkapi.Logout();
	}

	public void onLogin()
	{
		this.vkapi.LoggedIn -= this.onLogin;
		UnityEngine.SceneManagement.SceneManager.LoadScene("StarterScene");
	}

	public void onVKLogin()
	{
		this.vkapi.LoggedIn -= this.onVKLogin;
		UnityEngine.SceneManagement.SceneManager.LoadScene("StarterScene");
	}

	public void onLogout()
	{
		this.vkapi.LoggedOut -= this.onLogout;
		this.vkapi.Login();
	}

	private VkApi vkapi;
}
