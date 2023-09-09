using System;
using System.Collections.Generic;
using com.playGenesis.VkUnityPlugin;
using com.playGenesis.VkUnityPlugin.MiniJSON;
using UnityEngine;

public class Controller : MonoBehaviour
{
	private void Start()
	{
		this.sets = VkApi.VkSetts;
		this.vkapi = VkApi.VkApiInstance;
		this.d = this.vkapi.gameObject.GetComponent<Downloader>();
		if (this.vkapi.IsUserLoggedIn)
		{
			this.startWorkingWithVk();
		}
		else
		{
			this.vkapi.LoggedIn += this.startWorkingWithVk;
			this.vkapi.Login();
		}
	}

	public void Login()
	{
		this.vkapi.Login();
	}

	public void LogOut()
	{
		this.vkapi.Logout();
		this.sets.forceOAuth = true;
		this.sets.revoke = true;
	}

	public void startWorkingWithVk()
	{
		if (VKToken.TokenValidFor() < 120)
		{
			this.Login();
		}
		this.Get3FriendsDataFromVk();
	}

	public void Get3FriendsDataFromVk()
	{
		VKRequest httprequest = new VKRequest
		{
			url = "friends.get?user_id=" + VkApi.CurrentToken.user_id + "&count=3&fields=photo_200",
			CallBackFunction = new Action<VKRequest>(this.OnGet5FriendsCompleted)
		};
		this.vkapi.Call(httprequest);
	}

	private void OnGet5FriendsCompleted(VKRequest arg1)
	{
		if (arg1.error != null)
		{
			UnityEngine.Object.FindObjectOfType<GlobalErrorHandler>().Notification.Notify(arg1);
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(arg1.response) as Dictionary<string, object>;
		Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["response"];
		List<object> list = (List<object>)dictionary2["items"];
		foreach (object user in list)
		{
			this.friends.Add(VKUser.Deserialize(user));
		}
		for (int i = 0; i < this.friends.Count; i++)
		{
			FriendManager[] array = UnityEngine.Object.FindObjectsOfType<FriendManager>();
			Action<DownloadRequest> onFinished = delegate(DownloadRequest downloadRequest)
			{
				FriendManager friendManager = (FriendManager)downloadRequest.CustomData[0];
				friendManager.setUpImage(downloadRequest.DownloadResult.texture);
			};
			array[i].t.text = this.friends[i].first_name + " " + this.friends[i].last_name;
			array[i].friend = this.friends[i];
			DownloadRequest downloadRequest2 = new DownloadRequest
			{
				url = this.friends[i].photo_200,
				onFinished = onFinished,
				CustomData = new object[]
				{
					array[i]
				}
			};
			this.d.download(downloadRequest2);
		}
	}

	public void Back()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("StarterScene");
	}

	public VkApi vkapi;

	public Downloader d;

	public List<VKUser> friends = new List<VKUser>();

	public VkSettings sets;
}
