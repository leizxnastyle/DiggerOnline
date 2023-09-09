using System;
using System.Collections;
using UnityEngine;

public class Community : MonoBehaviour
{
	private void Awake()
	{
		if (Community.Instance == null)
		{
			Community.Instance = this;
		}
	}

	public void OfflineStart()
	{
		KGUI.SetNodes("main_menu.txt_title", false, false);
		KGUI.SetNodes("main_menu.background1", false, false);
		KGUI.SetNodes("main_menu.bottom_menu", false, false);
		KGUI.SetNodes("Community.Canvas.Categories.News.SlotOn", this.Category == Community.CategoryList.News, false);
		KGUI.SetNodes("Community.Canvas.Categories.BuildTeam.SlotOn", this.Category == Community.CategoryList.BuildTeam, false);
		KGUI.SetNodes("Community.Canvas.Categories.Video.SlotOn", this.Category == Community.CategoryList.Video, false);
		KGUI.SetNodes("Community.Canvas.Panels.News", this.Category == Community.CategoryList.News, false);
		KGUI.SetNodes("Community.Canvas.Panels.BuildTeam", this.Category == Community.CategoryList.BuildTeam, false);
		KGUI.SetNodes("Community.Canvas.Panels.Video", this.Category == Community.CategoryList.Video, false);
		KGUI.SetNodes("Community.Canvas.Panels.BuildTeam.Slot4", false, false);
		KGUI.SetNodes("Community.Canvas.Panels.Video.Slot2", false, false);
		KGUI.SetNodes("Community.Canvas.Panels.Video.Slot3", false, false);
		KGUI.SetNodes("Community.Canvas.Panels.Video.Slot4", false, false);
	}

	public void SetCategory(int Number)
	{
		SoundManager.Instance.Play(SoundManager.Sound.MenuClick, this.Audio.GetComponent<AudioSource>());
		this.Category = (Community.CategoryList)Number;
		KGUI.SetNodes("Community.Canvas.Categories.News.SlotOn", this.Category == Community.CategoryList.News, false);
		KGUI.SetNodes("Community.Canvas.Categories.BuildTeam.SlotOn", this.Category == Community.CategoryList.BuildTeam, false);
		KGUI.SetNodes("Community.Canvas.Categories.Video.SlotOn", this.Category == Community.CategoryList.Video, false);
		KGUI.SetNodes("Community.Canvas.Panels.News", this.Category == Community.CategoryList.News, false);
		KGUI.SetNodes("Community.Canvas.Panels.BuildTeam", this.Category == Community.CategoryList.BuildTeam, false);
		KGUI.SetNodes("Community.Canvas.Panels.Video", this.Category == Community.CategoryList.Video, false);
		KGUI.SetNodes("Community.Canvas.Panels.BuildTeam.Slot4", false, false);
		KGUI.SetNodes("Community.Canvas.Panels.Video.Slot2", false, false);
		KGUI.SetNodes("Community.Canvas.Panels.Video.Slot3", false, false);
		KGUI.SetNodes("Community.Canvas.Panels.Video.Slot4", false, false);
	}

	public void SetURL(string URL)
	{
		SoundManager.Instance.Play(SoundManager.Sound.MenuClick, this.Audio.GetComponent<AudioSource>());
		Application.ExternalCall("window.open", new object[]
		{
			"https://vk.com/" + URL
		});
	}

	public void CheckMemberCommunity()
	{
		base.StartCoroutine(this.GetMemberCommunity(VKAPI.INSTANCE._viewerId));
	}

	public IEnumerator GetMemberCommunity(string PlayerID)
	{
		WWWForm Form = new WWWForm();
		Form.AddField("PlayerID", PlayerID);
		WWW phpLoad = new WWW(SettingsManager.ServerURL[2] + "GetMemberCommunity.php", Form);
		yield return phpLoad;
		KGUI.SetNodes("Profile.Signs.CommunityMember", phpLoad.text == "1", false);
		yield break;
	}

	public static Community Instance;

	public AudioSource Audio;

	public Community.CategoryList Category;

	public enum CategoryList
	{
		News,
		BuildTeam,
		Video
	}
}
