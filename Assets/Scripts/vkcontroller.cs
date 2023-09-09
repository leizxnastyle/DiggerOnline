using System;
using System.Collections.Generic;
using com.playGenesis.VkUnityPlugin;
using com.playGenesis.VkUnityPlugin.MiniJSON;
using UnityEngine;

public class vkcontroller : MonoBehaviour
{
	private void Start()
	{
		if (VkApi.VkApiInstance.IsUserLoggedIn)
		{
			this.onLoggedIn();
		}
		else
		{
			VkApi.VkApiInstance.LoggedIn += this.onLoggedIn;
			VkApi.VkApiInstance.Login();
		}
	}

	public void onLoggedIn()
	{
		try
		{
			VkApi.VkApiInstance.LoggedIn -= this.onLoggedIn;
		}
		catch (Exception ex)
		{
		}
		VKRequest httprequest = new VKRequest
		{
			url = "users.get?user_ids=205387401&photo_50",
			CallBackFunction = new Action<VKRequest>(this.OnGotUserInfo)
		};
		VkApi.VkApiInstance.Call(httprequest);
	}

	public void OnGotUserInfo(VKRequest r)
	{
		if (r.error != null)
		{
			UnityEngine.Debug.Log(r.error.error_msg);
			return;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(r.response) as Dictionary<string, object>;
		List<object> list = (List<object>)dictionary["response"];
		VKUser vkuser = VKUser.Deserialize(list[0]);
		UnityEngine.Debug.Log("user id is " + vkuser.id);
		UnityEngine.Debug.Log("user name is " + vkuser.first_name);
		UnityEngine.Debug.Log("user last name is " + vkuser.last_name);
	}
}
