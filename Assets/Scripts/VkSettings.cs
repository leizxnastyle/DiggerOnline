using System;
using System.Collections.Generic;
using com.playGenesis.VkUnityPlugin;
using UnityEngine;

public class VkSettings : ScriptableObject
{
	private void Awake()
	{
		this.generateScope();
	}

	public void generateScope()
	{
		if (this.scope != null)
		{
			this.scope.Clear();
		}
		if (this.notify)
		{
			this.scope.Add("notify");
		}
		if (this.friends)
		{
			this.scope.Add("friends");
		}
		if (this.photos)
		{
			this.scope.Add("photos");
		}
		if (this.audio)
		{
			this.scope.Add("audio");
		}
		if (this.video)
		{
			this.scope.Add("video");
		}
		if (this.docs)
		{
			this.scope.Add("docs");
		}
		if (this.notes)
		{
			this.scope.Add("notes");
		}
		if (this.pages)
		{
			this.scope.Add("pages");
		}
		if (this.status)
		{
			this.scope.Add("status");
		}
		if (this.offers)
		{
			this.scope.Add("offers");
		}
		if (this.questions)
		{
			this.scope.Add("questions");
		}
		if (this.wall)
		{
			this.scope.Add("wall");
		}
		if (this.groups)
		{
			this.scope.Add("groups");
		}
		if (this.messages)
		{
			this.scope.Add("messages");
		}
		if (this.notifications)
		{
			this.scope.Add("notifications");
		}
		if (this.stats)
		{
			this.scope.Add("stats");
		}
		if (this.ads)
		{
			this.scope.Add("ads");
		}
		if (this.offline)
		{
			this.scope.Add("offline");
		}
		if (this.nohttps)
		{
			this.scope.Add("nohttps");
		}
	}

	public void ProcessAuthUrl()
	{
		if (string.IsNullOrEmpty(PlayerPrefs.GetString("auth_url", string.Empty)))
		{
			UnityEngine.Debug.LogError("Please, enter auth url in VKSetting");
			UnityEngine.Debug.Break();
		}
		else
		{
			Dictionary<string, string> dictionary = Utilities.ParseUrlParams(PlayerPrefs.GetString("auth_url", string.Empty));
			VkApi.CurrentToken.access_token = dictionary["access_token"];
			VkApi.CurrentToken.expires_in = int.Parse(dictionary["expires_in"]);
			VkApi.CurrentToken.tokenRecievedTime = DateTime.Now;
			VkApi.CurrentToken.user_id = dictionary["user_id"];
			VkApi.CurrentToken.expires_in = ((VkApi.CurrentToken.expires_in != 0) ? VkApi.CurrentToken.expires_in : 9999999);
			VkApi.CurrentToken.Save();
		}
	}

	public int VkAppId;

	public List<string> scope = new List<string>();

	public bool forceOAuth;

	public bool revoke;

	public string apiVersion;

	public bool notify;

	public bool friends;

	public bool photos;

	public bool audio;

	public bool video;

	public bool docs;

	public bool notes;

	public bool pages;

	public bool status;

	public bool offers;

	public bool questions;

	public bool wall;

	public bool groups;

	public bool messages;

	public bool notifications;

	public bool stats;

	public bool ads;

	public bool offline;

	public bool nohttps;
}
