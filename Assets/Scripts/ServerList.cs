using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerList : MonoBehaviour
{
	private void Awake()
	{
		if (ServerList.Instance == null)
		{
			ServerList.Instance = this;
		}
	}

	private void Update()
	{
		if (ManagerServerList.ShowWindow)
		{
			this.Mask[0].GetComponent<Image>().enabled = true;
			this.Mask[1].GetComponent<Button>().enabled = false;
		}
		else
		{
			this.Mask[0].GetComponent<Image>().enabled = false;
			this.Mask[1].GetComponent<Button>().enabled = true;
		}
	}

	public void OfflineStart()
	{
		KGUI.SetNodes("main_menu.txt_title", false, false);
		KGUI.SetNodes("main_menu.background1", false, false);
		KGUI.SetNodes("main_menu.bottom_menu", false, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.Build.SlotOn", this.Category == ServerList.CategoryList.Build, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.TeamBattle.SlotOn", this.Category == ServerList.CategoryList.TeamBattle, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.Run.SlotOn", this.Category == ServerList.CategoryList.Run, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.HungerGames.SlotOn", this.Category == ServerList.CategoryList.HungerGames, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.HideSeek.SlotOn", this.Category == ServerList.CategoryList.HideSeek, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.ZombieVirus.SlotOn", this.Category == ServerList.CategoryList.ZombieVirus, false);
		this.RefreshServersList();
	}

	public void SetCategory(int Number)
	{
		SoundManager.Instance.Play(SoundManager.Sound.MenuClick, this.Audio.GetComponent<AudioSource>());
		this.Category = (ServerList.CategoryList)Number;
		KGUI.SetNodes("MapsList.Canvas.Categories.Build.SlotOn", this.Category == ServerList.CategoryList.Build, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.TeamBattle.SlotOn", this.Category == ServerList.CategoryList.TeamBattle, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.Run.SlotOn", this.Category == ServerList.CategoryList.Run, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.HungerGames.SlotOn", this.Category == ServerList.CategoryList.HungerGames, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.HideSeek.SlotOn", this.Category == ServerList.CategoryList.HideSeek, false);
		KGUI.SetNodes("MapsList.Canvas.Categories.ZombieVirus.SlotOn", this.Category == ServerList.CategoryList.ZombieVirus, false);
		this.RefreshServersList();
	}

	public void NoMaps()
	{
		SoundManager.Instance.Play(SoundManager.Sound.MenuClick, this.Audio.GetComponent<AudioSource>());
		MainMenu.Instance._FastGameType = -1;
		if (this.Category == ServerList.CategoryList.TeamBattle)
		{
			MainMenu.Instance.RefreshStartFastGame(2);
		}
		if (this.Category == ServerList.CategoryList.HungerGames)
		{
			MainMenu.Instance.RefreshStartFastGame(3);
		}
		if (this.Category == ServerList.CategoryList.Run)
		{
			MainMenu.Instance.RefreshStartFastGame(4);
		}
		if (this.Category == ServerList.CategoryList.HideSeek)
		{
			MainMenu.Instance.RefreshStartFastGame(5);
		}
		if (this.Category == ServerList.CategoryList.ZombieVirus)
		{
			MainMenu.Instance.RefreshStartFastGame(6);
		}
		MainMenu.Instance.SwitchMenu(Menu.FastGame, null, null);
	}

	public void RefreshServersList()
	{
		base.CancelInvoke("RefreshServersList");
		if (!KGUI.FindNode("MapsList", false).gameObject.activeInHierarchy)
		{
			return;
		}
		base.Invoke("RefreshServersList", 1f);
		this._ServerRooms.Clear();
		if (this.Search.GetComponent<Text>().text.Length == 0)
		{
			KGUI.SetNodes("MapsList.Canvas.Panels.NoMaps", true, false);
		}
		this.LocalizN[0].GetComponent<Text>().text = Localize.GetText("key1200", null);
		this.LocalizN[1].GetComponent<Text>().text = Localize.GetText("key1201", null);
		this.LocalizN[2].GetComponent<Text>().text = Localize.GetText("key1202", null);
		this.LocalizN[3].GetComponent<Text>().text = Localize.GetText("key1203", null);
		if (this.Search.GetComponent<Text>().text.Length > 0)
		{
			Color color = this.SearchGL[0].GetComponent<Image>().color;
			color.a = 0.784f;
			this.SearchGL[0].GetComponent<Image>().color = color;
			Color color2 = this.SearchGL[1].GetComponent<Text>().color;
			color2.a = 1f;
			this.SearchGL[1].GetComponent<Text>().color = color2;
			Color caretColor = this.SearchGL[2].GetComponent<InputField>().caretColor;
			caretColor.a = 0.784f;
			this.SearchGL[2].GetComponent<InputField>().caretColor = caretColor;
			Color color3 = this.SearchGL[3].GetComponent<Image>().color;
			color3.a = 1f;
			this.SearchGL[3].GetComponent<Image>().color = color3;
		}
		else
		{
			Color color4 = this.SearchGL[0].GetComponent<Image>().color;
			color4.a = 0f;
			this.SearchGL[0].GetComponent<Image>().color = color4;
			Color color5 = this.SearchGL[1].GetComponent<Text>().color;
			color5.a = 0f;
			this.SearchGL[1].GetComponent<Text>().color = color5;
			Color caretColor2 = this.SearchGL[2].GetComponent<InputField>().caretColor;
			caretColor2.a = 0f;
			this.SearchGL[2].GetComponent<InputField>().caretColor = caretColor2;
			Color color6 = this.SearchGL[3].GetComponent<Image>().color;
			color6.a = 0f;
			this.SearchGL[3].GetComponent<Image>().color = color6;
		}
		foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
		{
			if (!roomInfo.customProperties.ContainsKey("is_watch") || !(bool)roomInfo.customProperties["is_watch"])
			{
				if (this.Search.GetComponent<Text>().text.Length > 0)
				{
					string text = (string)roomInfo.customProperties["map_name"];
					if (Localize.GetText(text.Split(new char[]
					{
						' '
					})[0], null).IndexOf(this.Search.GetComponent<Text>().text, StringComparison.CurrentCultureIgnoreCase) == -1)
					{
						goto IL_59D;
					}
				}
				GameINI.GameType gameType = (GameINI.GameType)((int)roomInfo.customProperties["game_type"]);
				if (this.Category != ServerList.CategoryList.Build || gameType == GameINI.GameType.BUILDING)
				{
					if (this.Category != ServerList.CategoryList.TeamBattle || gameType == GameINI.GameType.TEAM_BATTLE)
					{
						if (this.Category != ServerList.CategoryList.Run || gameType == GameINI.GameType.ARCADE)
						{
							if (this.Category != ServerList.CategoryList.HungerGames || gameType == GameINI.GameType.HUNGER_GAMES)
							{
								if (this.Category != ServerList.CategoryList.HideSeek || gameType == GameINI.GameType.HIDE_SEEK)
								{
									if (this.Category != ServerList.CategoryList.ZombieVirus || gameType == GameINI.GameType.ZOMBIE_VIRUS)
									{
										if (gameType != GameINI.GameType.DEATHMATCH)
										{
											if (gameType != GameINI.GameType.CTF)
											{
												if (roomInfo.playerCount < (int)roomInfo.maxPlayers)
												{
													if (roomInfo.maxPlayers < 13)
													{
														if ((gameType == GameINI.GameType.HUNGER_GAMES || gameType == GameINI.GameType.ARCADE) && roomInfo.customProperties.ContainsKey("game_status"))
														{
															GameStatus gameStatus = (GameStatus)int.Parse(roomInfo.customProperties["game_status"].ToString());
															if (gameStatus == GameStatus.GS_LOAD)
															{
																goto IL_59D;
															}
															if (gameStatus != GameStatus.GS_WAIT)
															{
																goto IL_59D;
															}
														}
														this._ServerRooms.Add(roomInfo);
														if (this._ServerRooms.Count >= 180)
														{
															break;
														}
														KGUI.SetNodes("MapsList.Canvas.Panels.NoMaps", false, false);
														Color color7 = this.SearchGL[0].GetComponent<Image>().color;
														color7.a = 0.784f;
														this.SearchGL[0].GetComponent<Image>().color = color7;
														Color color8 = this.SearchGL[1].GetComponent<Text>().color;
														color8.a = 1f;
														this.SearchGL[1].GetComponent<Text>().color = color8;
														Color caretColor3 = this.SearchGL[2].GetComponent<InputField>().caretColor;
														caretColor3.a = 0.784f;
														this.SearchGL[2].GetComponent<InputField>().caretColor = caretColor3;
														Color color9 = this.SearchGL[3].GetComponent<Image>().color;
														color9.a = 1f;
														this.SearchGL[3].GetComponent<Image>().color = color9;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			IL_59D:;
		}
		KGUI.ResizeGrid("MapsList.servers_grid", this._ServerRooms.Count, delegate(GameObject slot, int index)
		{
			RoomInfo roomInfo2 = this._ServerRooms[index];
			string text2 = (string)roomInfo2.customProperties["map_name"];
			string text3 = (string)roomInfo2.customProperties["map_name"];
			string text4 = Localize.GetText(text3, text3);
			string text5 = roomInfo2.playerCount + "/" + roomInfo2.maxPlayers;
			slot.transform.Find("txt_title").GetComponent<UILabel>().text = text2.Replace(text3, text4);
			slot.transform.Find("txt_players").GetComponent<UILabel>().text = text5;
			slot.transform.Find("icon_lock").gameObject.SetActive(roomInfo2.customProperties.ContainsKey("password") && ((string)roomInfo2.customProperties["password"]).Length > 0);
		}, "MapsList");
	}

	public static ServerList Instance;

	public ServerList.CategoryList Category;

	public List<RoomInfo> _ServerRooms = new List<RoomInfo>();

	public RoomInfo _SelectedRoom;

	public AudioSource Audio;

	public Text Search;

	public Text[] LocalizN;

	public GameObject[] Mask;

	public GameObject[] SearchGL;

	public enum CategoryList
	{
		Build,
		TeamBattle,
		Run,
		HungerGames,
		HideSeek,
		ZombieVirus
	}
}
