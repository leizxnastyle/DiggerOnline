using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using InventorySystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public static event Action isInventoryMenuOpen;

	public static event Action isInventoryMenuClose;

	public static event Action<CommonBlockKind> isSetBlockKind;

	public DateTime BonusGemsEndTime
	{
		set
		{
			this._BonusGemsEndTime = value;
		}
	}

	public Menu CurMenu
	{
		get
		{
			return this._CurMenu;
		}
	}

	public bool MenuActive
	{
		get
		{
			return this._CurMenu != Menu.None;
		}
	}

	public void RegisterMenus()
	{
		this._NonModalMenus = new Menu[]
		{
			Menu.None,
			Menu.Hud
		};
		this._CanToggleMenus = new Menu[]
		{
			Menu.None,
			Menu.Hud,
			Menu.Start,
			Menu.MyMaps,
			Menu.Settings,
			Menu.Shop,
			Menu.ItemsPack,
			Menu.Servers,
			Menu.TopMaps,
			Menu.Bank,
			Menu.Community,
			Menu.FastGame,
			Menu.Inventory,
			Menu.TabMenu,
			Menu.Book,
			Menu.GameMenu,
			Menu.Emotions,
			Menu.Profile,
			Menu.SelectWeapon,
			Menu.TeamBattle,
			Menu.Deathmatch,
			Menu.Ratings,
			Menu.Achievements,
			Menu.Ban,
			Menu.Authorizathion
		};
		this.RegisterMenu(Enum.GetValues(typeof(Menu)) as Menu[], null, delegate(object data, object dataEx)
		{
			KGUI.SetNodes("main_menu_background", SceneManager.GetActiveScene().name == "Menu", false);
			if (this._CurMenu != Menu.Book && Book.Current != null)
			{
				Book.Current.SaveCurTexts();
				Book.Current = null;
			}
		}, delegate()
		{
		});
		this.RegisterMenu(new Menu[]
		{
			Menu.Start,
			Menu.MyMaps,
			Menu.Settings,
			Menu.Shop,
			Menu.Servers,
			Menu.TopMaps,
			Menu.Bank,
			Menu.Ban,
			Menu.Community,
			Menu.FastGame,
			Menu.Profile,
			Menu.Ratings,
			Menu.Achievements
		}, "main_menu", delegate(object data, object dataEx)
		{
			if (ProtectManager.PlayerBanned)
			{
				PlayerBanned.Instance.ShowBan();
				return;
			}
			KGUI.SetNodes("main_menu", true, false);
			KGUI.SetNodes("main_menu.slider_page1", false, false);
			KGUI.SetNodes("main_menu.slider_page2", false, false);
			KGUI.SetNodes("main_menu.top_menu", SceneManager.GetActiveScene().name == "Menu", false);
			KGUI.SetNodes("main_menu.bottom_menu", SceneManager.GetActiveScene().name == "Menu", false);
			KGUI.SetNodes("main_menu.money", false, false);
			KGUI.SetNodes("main_menu.button_close", false, false);
			KGUI.SetNodes("main_menu.background1", true, false);
			KGUI.SetNodes("main_menu.background2", true, false);
			if ((this._BonusGemsEndTime - DateTime.Now).TotalSeconds > 0.0)
			{
				ManagerBank.BonusX2 = true;
			}
			KGUI.SetNodes("main_menu.bonus_gems.txt_time", false, false);
			KGUI.SetNodes("main_menu.bonus_gems", false, false);
			if (this._CurMenu == Menu.Start || this._CurMenu == Menu.TopMaps || this._CurMenu == Menu.Community || this._CurMenu == Menu.Servers || this._CurMenu == Menu.Settings || this._CurMenu == Menu.Achievements || this._CurMenu == Menu.Profile || this._CurMenu == Menu.Ratings || (this._CurMenu == Menu.Shop && SceneManager.GetActiveScene().name == "Menu"))
			{
				KGUI.SetNodes("main_menu.button_back", false, false);
			}
			if (this._CurMenu == Menu.Bank)
			{
				if (SceneManager.GetActiveScene().name == "Game")
				{
					KGUI.SetNodes("main_menu.button_back", true, false);
				}
				if (SceneManager.GetActiveScene().name == "Menu")
				{
					KGUI.SetNodes("main_menu.button_back", false, false);
				}
			}
			if (this._CurMenu == Menu.Settings)
			{
				if (SceneManager.GetActiveScene().name == "Game")
				{
					KGUI.SetNodes("main_menu.button_close", true, false);
				}
				if (SceneManager.GetActiveScene().name == "Menu")
				{
					KGUI.SetNodes("main_menu.button_close", false, false);
				}
			}
			if (SceneManager.GetActiveScene().name == "Menu")
			{
				int num = ProfileINI.experience;
				int num2 = ProfileINI.LevelExp(num);
				int num3 = ProfileINI.ExpForLevel(num2);
				int num4 = ProfileINI.ExpForLevel(num2 + 1);
				KGUI.FindNode("main_menu.exp_bar_cur", false).GetComponent<UISprite>().fillAmount = (float)(num - num3) / (float)(num4 - num3);
			}
			Bar.Instance.SetCoins();
			KGUI.SetNodes("main_menu.top_menu.Canvas.Bank.Label", ManagerBank.BonusX2, false);
			KGUI.SetControlText("main_menu.bottom_menu.txt_level", ProfileINI.level + "  lvl");
			KGUI.SetControlSprite(KGUI.FindNode("main_menu.bottom_menu.level_icon", false), "level_" + ProfileINI.level, 0f);
			KGUI.SetControlText("main_menu.bottom_menu.txt_nickname", ProfileINI.nickname);
			this.CheckKickReason();
		}, delegate()
		{
			KGUI.SetButtonCallback("main_menu.ibtn_back", delegate()
			{
				this.UseBeckButton();
			});
			KGUI.SetButtonCallback("main_menu.ibtn_close", delegate()
			{
				if (SceneManager.GetActiveScene().name == "Game")
				{
					this.HideMenu();
				}
			});
			KGUI.SetTooltipCallback("main_menu.bottom_menu.version", () => Localize.GetText("VERSION_TEXT", null));
			KGUI.SetTooltipCallback("main_menu.bottom_menu.background2", delegate
			{
				int num = ProfileINI.LevelExp(ProfileINI.experience);
				int num2 = ProfileINI.ExpForLevel(num + 1);
				return ProfileINI.experience + " / " + num2;
			});
			KGUI.SetTooltipCallback("main_menu.bottom_menu.level_icon.sprite", () => Localize.GetText("LEVEL_" + ProfileINI.level, null));
		});
		this.RegisterMenu(Menu.Hud, this.hud_tag, delegate(object data, object dataEx)
		{
			this.SetCrosshairInfo(null, MainMenu.CrosshairAction.None);
			KGUI.SetNodes(this.hud_tag + ".txt_watch_mode_tip", App.Instance.Settings.isWatch, false);
			KGUI.SetNodes(this.hud_tag + ".txt_respawn_tip", false, false);
			KGUI.SetNodes(this.hud_tag + ".battle.txt_flag_captured_tip", false, false);
			KGUI.SetNodes(this.hud_tag + ".gold_cup", false, false);
			KGUI.SetNodes(this.hud_tag + ".chat.grid_messages.slot_prototype", false, false);
			KGUI.SetNodes(this.hud_tag + ".kills_list.grid_messages.slot_prototype", false, false);
			KGUI.SetNodes(this.hud_tag + ".nickname", false, false);
			KGUI.SetNodes(this.hud_tag + ".speech_bubble", false, false);
			KGUI.SetNodes(this.hud_tag + ".emotion_image", false, false);
			KGUI.SetNodes(this.hud_tag + ".custom_blocks", false, false);
			KGUI.SetNodes(this.hud_tag + ".ibtn_audioсhat", true, false);
			KGUI.EnableNodes(this.hud_tag + ".Entity", false, false);
			KGUI.ResizeGrid(this.hud_tag + ".boobles", 0, delegate(GameObject slot, int index)
			{
			}, null);
			KGUI.SetControlText(this.hud_tag + ".battle.txt_generic_tip_arcade", string.Empty);
			KGUI.EnableNodes(this.hud_tag + ".rig_widget", false, false);
			KGUI.EnableNodes(this.hud_tag + ".ibtn_inventar", !App.Instance.Settings.isWatch, false);
			KGUI.EnableNodes(this.hud_tag + ".cube", !App.Instance.Settings.isWatch, false);
			if (GameType.BattleMode())
			{
				KGUI.SetNodes(this.hud_tag + ".hide_seek", false, false);
				KGUI.SetNodes(this.hud_tag + ".custom_blocks", false, false);
				KGUI.SetNodes(this.hud_tag + ".txt_battle_more_players_tip", PhotonNetwork.playerList.Length < 4 && GameType.IsNeedCheckTips(), false);
				if (!GameType.IsObserving())
				{
					KGUI.SetNodes(this.hud_tag + ".crosshair", false, false);
					this.RefreshBattleWeapon(0);
					CameraController.Instance.HandsGunAnimations.gameObject.SetActive(true);
				}
				else
				{
					KGUI.SetNodes(this.hud_tag + ".battle", false, false);
					CameraController.Instance.HandsGunAnimations.gameObject.SetActive(false);
				}
				KGUI.SetNodes(this.hud_tag + ".cube", false, false);
				KGUI.SetNodes(this.hud_tag + ".battle.anchor_bottom_left", true, false);
				if (TeamBattle.Instance != null)
				{
					if (TeamBattle.Instance is Deathmatch || TeamBattle.Instance is HungerGames)
					{
						KGUI.SetNodes(this.hud_tag + ".battle.flag_blue", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.flag_red", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_blue_score", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_red_score", false, false);
					}
					else
					{
						KGUI.SetControlText(this.hud_tag + ".battle.txt_red_score", TeamBattle.Instance.RedTeamScore + string.Empty);
						KGUI.SetControlText(this.hud_tag + ".battle.txt_blue_score", TeamBattle.Instance.BlueTeamScore + string.Empty);
					}
					if (!(TeamBattle.Instance is Ctf))
					{
						KGUI.SetNodes(this.hud_tag + ".battle.flags", false, false);
					}
					if (TeamBattle.Instance is ZombieVirus)
					{
						KGUI.SetNodes(this.hud_tag + ".battle.flag_red", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.flag_blue", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_red_score", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_blue_score", false, false);
					}
					else
					{
						KGUI.SetNodes(this.hud_tag + ".battle.zombie_virus", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_survivors_count", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_zombies_count", false, false);
					}
					if (TeamBattle.Instance is HungerGames)
					{
						KGUI.SetNodes(this.hud_tag + ".battle.flag_red", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.flag_blue", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_red_score", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_blue_score", false, false);
					}
					else if (TeamBattle.Instance is HideSeek)
					{
						KGUI.SetNodes(this.hud_tag + ".battle.flag_red", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.flag_blue", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_red_score", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_blue_score", false, false);
					}
					else if (TeamBattle.Instance is Arcade)
					{
						KGUI.SetNodes(this.hud_tag + ".battle.anchor_bottom_left", false, false);
					}
					else
					{
						KGUI.SetNodes(this.hud_tag + ".battle.zombie_virus", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_survivors_count", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.txt_zombies_count", false, false);
					}
				}
			}
			else
			{
				KGUI.SetNodes(this.hud_tag + ".battle", false, false);
				KGUI.SetNodes(this.hud_tag + ".hide_seek", false, false);
				CameraController.Instance.HandsGunAnimations.gameObject.SetActive(true);
				if (App.Instance.Settings.isWatch)
				{
					KGUI.SetNodes(this.hud_tag + ".cube", false, false);
				}
				if ((ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_FENCE) > 0 || ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_HALF) > 0 || ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_DIAGONAL) > 0 || ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR) > 0 || ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_QUARTER) > 0 || ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_CORNER) > 0 || ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR_CORNER) > 0) && !App.Instance.Settings.isWatch)
				{
					KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_01", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_FENCE) > 0, false);
					KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_02", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_HALF) > 0, false);
					KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_03", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_QUARTER) > 0, false);
					KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_04", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_DIAGONAL) > 0, false);
					KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_05", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR) > 0, false);
					KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_07", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_CORNER) > 0, false);
					KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_08", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR_CORNER) > 0, false);
					KGUI.FindNode(this.hud_tag + ".custom_blocks", false).GetComponent<UITable>().Reposition();
					this.CheckSelectBlock();
				}
				else
				{
					KGUI.SetNodes(this.hud_tag + ".custom_blocks", false, false);
				}
			}
			KGUI.SetNodes(this.hud_tag + ".bonuses", false, true);
			this.RefreshFlying();
			this.RefreshAcceleration();
			this.RefreshDayNight();
			if (MainMenu.CleanScreen)
			{
				KGUI.FindNode("hud", false).gameObject.SetActive(false);
			}
		}, delegate()
		{
			KGUI.SetButtonCallback(this.hud_tag + ".bonuses.fly", delegate()
			{
				this._FlyEnabled = !this._FlyEnabled;
				this.RefreshFlying();
				this.StartCoroutine(this.DisableInputTimedFrame());
			});
			KGUI.SetButtonCallback(this.hud_tag + ".bonuses.speed", delegate()
			{
				this._SpeedEnabled = !this._SpeedEnabled;
				this.RefreshAcceleration();
			});
			KGUI.SetButtonCallback(this.hud_tag + ".ibtn_menu", delegate()
			{
				MainMenu.Instance.SwitchMenu(Menu.GameMenu, null, null);
			});
			KGUI.SetButtonCallback(this.hud_tag + ".ibtn_chat", delegate()
			{
				Chat._Instance.OpenChat();
			});
			KGUI.SetButtonCallback(this.hud_tag + ".ibtn_audioсhat", delegate()
			{
				this.SwitchMenu(Menu.TabMenu, null, null);
			});
			KGUI.SetButtonCallback(this.hud_tag + ".ibtn_inventar", delegate()
			{
				this.SwitchMenu(Menu.Inventory, this._InventoryTab, null);
			});
			KGUI.SetButtonCallback(this.hud_tag + ".Entity.iBtnSetEntity", delegate()
			{
				if (WorldGameObjectX.Instance.EntityPreview != null)
				{
					WorldGameObjectX.Instance.SetEntityFromPreview();
					this.StartCoroutine(this.DisableInputTimedFrame());
				}
			});
			KGUI.SetButtonCallback(this.hud_tag + ".Entity.iBtnSetLeft", delegate()
			{
				if (WorldGameObjectX.Instance.EntityPreview != null)
				{
					WorldGameObjectX.Instance.UpdateStorePreview(-1);
				}
			});
			KGUI.SetButtonCallback(this.hud_tag + ".Entity.iBtnSetRight", delegate()
			{
				if (WorldGameObjectX.Instance.EntityPreview != null)
				{
					WorldGameObjectX.Instance.UpdateStorePreview(1);
				}
			});
		});
		this.RegisterMenu(Menu.Start, "Start", delegate(object data, object dataEx)
		{
			Info.Instance.Location = string.Empty;
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("START_MENU", null));
			KGUI.SetNodes("Start.page1_fast_game", false, false);
			KGUI.SetNodes("main_menu.slider_page1", false, false);
			KGUI.SetNodes("Start.page1_fast_game.TV", false, false);
			KGUI.SetNodes("Start.Canvas", true, false);
			Offers.Instance.SetOffers();
		}, delegate()
		{
			KGUI.SetButtonCallback("Start.ibtn_fast_game", delegate()
			{
				KGUI.SetNodes("Start.page1_start", false, false);
				KGUI.SetNodes("Start.page1_fast_game", true, false);
				KGUI.SetNodes("main_menu.button_back", true, false);
				KGUI.SetNodes("Start.Canvas", false, false);
				KGUI.SetNodes("Start.page1_fast_game.TV", true, false);
				KGUI.SetControlText("main_menu.txt_title", Localize.GetText("START_FAST_GAME", null));
				this.RefreshStartFastGame(0);
			});
			KGUI.SetButtonCallback("Start.page1_fast_game.ibtn_next_game", delegate()
			{
				this.RefreshStartFastGame(1);
			});
			KGUI.SetButtonCallback("Start.page1_fast_game.ibtn_prev_game", delegate()
			{
				this.RefreshStartFastGame(-1);
			});
			KGUI.SetButtonCallback("Start.page1_fast_game.ibtn_play", delegate()
			{
				if (this._FastGameType == -1)
				{
					this.SwitchMenu(Menu.MyMaps, null, null);
				}
				else if (this._FastGameMaps.Count > 0)
				{
					this.SwitchMenu(Menu.FastGame, null, null);
				}
				else
				{
					this.StartCoroutine(this.FastConnectToGame(this._MapGameTypes[this._FastGameType]));
				}
			});
			KGUI.SetButtonCallback("Start.ibtn_my_maps", delegate()
			{
				this.SwitchMenu(Menu.MyMaps, null, null);
				Protect.Instance.CheckDLL(UnityEngine.Random.Range(0, 14));
			});
			KGUI.SetButtonCallback("Start.ibtn_servers_list", delegate()
			{
				this.SwitchMenu(Menu.Servers, null, null);
				Protect.Instance.CheckDLL(UnityEngine.Random.Range(0, 14));
			});
			KGUI.SetButtonCallback("Start.ibtn_call_friends", delegate()
			{
				Application.ExternalCall("InviteFrends", new object[0]);
			});
		});
		this.RegisterMenu(Menu.MyMaps, "MyMaps", delegate(object data, object dataEx)
		{
			Info.Instance.Location = "MyMaps";
			this._MapGameType = 0;
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("MY_MAPS_TITLE1", null));
			KGUI.SetNodes("MyMaps.page2", false, true);
			KGUI.ResizeGrid("MyMaps.my_maps_grid", 1 + ProfileINI.GetPurchaseValue(StorePurchase.SLOTS), delegate(GameObject slot, int index)
			{
				slot.transform.Find("txt_title").GetComponent<UILabel>().text = ProfileINI.GetSlotMapName(index + 1);
			}, "MyMaps");
		}, delegate()
		{
			KGUI.SetButtonCallback("MyMaps.my_maps_grid.ibtn_select", delegate()
			{
				this.StartCoroutine(this.LoadSlotMapInfo(VKAPI.INSTANCE._viewerId, KGUI.SlotIndex + 1));
			});
			KGUI.SetButtonCallback("MyMaps.my_maps_grid.ibtn_edit", delegate()
			{
				int slotID = KGUI.SlotIndex + 1;
				this.ShowInputText("ENTER_SLOT_NAME", "ENTER_SLOT_NAME_DESC", delegate(string newName)
				{
					if (newName != ProfileINI.GetSlotMapName(slotID))
					{
						this.StartCoroutine(this.ModifyMapName(slotID, newName));
					}
				}, ProfileINI.GetSlotMapName(slotID));
			});
			KGUI.SetButtonCallback("MyMaps.ibtn_buy_slot", delegate()
			{
				this.StartCoroutine(this.PurchaseBuy(StorePurchase.SLOTS, 0));
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.map_size.ibtn_next", delegate()
			{
				this.ChangeMapSize(true);
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.map_size.ibtn_prev", delegate()
			{
				this.ChangeMapSize(false);
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.map_type.ibtn_next", delegate()
			{
				this.ChangeMapType(true);
				if (MapsGen.MapBiomNumber != 0)
				{
					MapsGen.MapBiomNumber = 0;
					KGUI.SetControlText("MyMaps.page2.info.map_biom.txt_type", Localize.GetText("key110" + MapsGen.MapBiomNumber, null));
				}
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.map_type.ibtn_prev", delegate()
			{
				this.ChangeMapType(false);
				if (MapsGen.MapBiomNumber != 0)
				{
					MapsGen.MapBiomNumber = 0;
					KGUI.SetControlText("MyMaps.page2.info.map_biom.txt_type", Localize.GetText("key110" + MapsGen.MapBiomNumber, null));
				}
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.map_biom.ibtn_next", delegate()
			{
				this.ChangeBiomType(true);
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.map_biom.ibtn_prev", delegate()
			{
				this.ChangeBiomType(false);
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.map_time.ibtn_next", delegate()
			{
				this.ChangeMapTime(true);
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.map_time.ibtn_prev", delegate()
			{
				this.ChangeMapTime(false);
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.ibtn_create_map", delegate()
			{
				if (MapsGen.MapTypeNumber == 0 && MapsGen.MapBiomNumber == 0)
				{
					this._MapType = 0;
				}
				else if (MapsGen.MapTypeNumber == 0 && MapsGen.MapBiomNumber == 1)
				{
					this._MapType = 8;
				}
				else if (MapsGen.MapTypeNumber == 0 && MapsGen.MapBiomNumber == 2)
				{
					this._MapType = 5;
				}
				else if (MapsGen.MapTypeNumber == 0 && MapsGen.MapBiomNumber == 3)
				{
					this._MapType = 3;
				}
				else if (MapsGen.MapTypeNumber == 0 && MapsGen.MapBiomNumber == 4)
				{
					this._MapType = 4;
				}
				else if (MapsGen.MapTypeNumber == 0 && MapsGen.MapBiomNumber == 5)
				{
					this._MapType = 2;
				}
				else if (MapsGen.MapTypeNumber == 1 && MapsGen.MapBiomNumber == 0)
				{
					this._MapType = 0;
				}
				else if (MapsGen.MapTypeNumber == 1 && MapsGen.MapBiomNumber == 1)
				{
					this._MapType = 8;
				}
				else if (MapsGen.MapTypeNumber == 1 && MapsGen.MapBiomNumber == 2)
				{
					this._MapType = 5;
				}
				else if (MapsGen.MapTypeNumber == 2)
				{
					this._MapType = 7;
				}
				else if (MapsGen.MapTypeNumber == 3)
				{
					this._MapType = 6;
				}
				ProfileINI.server_name = KGUI.GetControlText("MyMaps.page2.info.name.inp_name");
				ProfileINI.server_about = KGUI.GetControlText("MyMaps.page2.info.description.inp_description");
				string controlText = KGUI.GetControlText("MyMaps.page2.info.password.inp_password");
				int num = KGUI.GetControlText("MyMaps.page2.info.name.inp_name").Count((char c) => !char.IsWhiteSpace(c));
				if (KGUI.GetControlText("MyMaps.page2.info.name.inp_name").Trim() == string.Empty || num < 3)
				{
					this.ShowHint(Localize.GetText("key1300", null), false);
					return;
				}
				bool flag = false;
				foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
				{
					if ((string)roomInfo.customProperties["player_id"] == VKAPI.INSTANCE._viewerId && (int)roomInfo.customProperties["slot_id"] == this._SelectedMapSlot)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					this.ShowHint(Localize.GetText("LBL_ALERT_NO_REGENERATE", null), false);
					return;
				}
				bool flag2 = false;
				foreach (RoomInfo roomInfo2 in PhotonNetwork.GetRoomList())
				{
					if ((string)roomInfo2.customProperties["map_name"] == ProfileINI.server_name)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					this.ShowHint(Localize.GetText("LBL_ALERT_NAME_EXIST", null), false);
					return;
				}
				using (MD5 md = MD5.Create())
				{
					if (controlText.Length > 0)
					{
						this.ProtectPassword = ProtectHash.GetHash(md, controlText);
					}
					else
					{
						this.ProtectPassword = string.Empty;
					}
				}
				this.InitializeGameINI();
				App.Instance.Settings.server_name = ProfileINI.server_name;
				App.Instance.Settings.server_about = ProfileINI.server_about;
				App.Instance.Settings.gameType = GameINI.GameType.BUILDING;
				App.Instance.Settings.mapSize = GameINI.MapSize.SMALL;
				App.Instance.Settings.mapPopulation = 12;
				App.Instance.Settings.isOnline = true;
				App.Instance.Settings.isServer = true;
				App.Instance.Settings.isServerAdministrator = true;
				App.Instance.Settings.isWatch = false;
				App.Instance.Settings.loadingSavedMap = false;
				App.Instance.Settings.playerID = VKAPI.INSTANCE._viewerId;
				App.Instance.Settings.slotID = this._SelectedMapSlot;
				App.Instance.Settings.publicMapID = 0;
				App.Instance.Settings.Password = this.ProtectPassword;
				App.Instance.Settings.mapSize = this._MapSizes[this._MapSize];
				App.Instance.Settings.mapType = this._MapTypes[this._MapType];
				App.Instance.Settings.mapTime = this._MapTimes[this._MapTime];
				this.HideMenu();
				this.ShowLoading("LOADING_LEVEL", string.Empty);
				SceneManager.LoadScene("Game");
			});
			KGUI.SetButtonCallback("MyMaps.page2.base.ibtn_load", delegate()
			{
				this.ShowMyMap(false, false);
			});
			KGUI.SetButtonCallback("MyMaps.page2.base.ibtn_generate", delegate()
			{
				this.ShowHint(Localize.GetText("PeregenerateMap", null), false);
				this.ShowMyMap(true, false);
			});
			KGUI.SetButtonCallback("MyMaps.page2.base.ibtn_public", delegate()
			{
				this.ShowMyMap(false, true);
			});
			KGUI.SetButtonCallback("MyMaps.page2.base.ibtn_look_in_public", delegate()
			{
				this.SwitchMenu(Menu.TopMaps, this._SelectedMapID, null);
			});
			KGUI.SetButtonCallback("MyMaps.page2.base.ibtn_remove_from_public", delegate()
			{
				this.StartCoroutine(this.ClosePublicMap(VKAPI.INSTANCE._viewerId, this._SelectedMapSlot, this._SelectedMapID));
			});
			KGUI.SetButtonCallback("MyMaps.page2.base.ibtn_save", delegate()
			{
				if (this._SelectedMapSlot != 0)
				{
					this.StartCoroutine(World.Instance.SaveMapToServer(this._SelectedMapSlot, null));
				}
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.ibtn_load_map", delegate()
			{
				int num = KGUI.GetControlText("MyMaps.page2.info.name.inp_name").Count((char c) => !char.IsWhiteSpace(c));
				if (KGUI.GetControlText("MyMaps.page2.info.name.inp_name").Trim() == string.Empty || num < 3)
				{
					this.ShowHint(Localize.GetText("key1300", null), false);
					return;
				}
				if (this._MapGameTypes[this._MapGameType] == GameINI.GameType.ARCADE || this._MapGameTypes[this._MapGameType] == GameINI.GameType.HUNGER_GAMES || this._MapGameTypes[this._MapGameType] == GameINI.GameType.HIDE_SEEK)
				{
					this._MapGameTypes[this._MapGameType] = GameINI.GameType.BUILDING;
				}
				this.JoinOrCreateGame(VKAPI.INSTANCE._viewerId, this._SelectedMapSlot, false, KGUI.GetControlText("MyMaps.page2.info.inp_name"), KGUI.GetControlText("MyMaps.page2.info.inp_description"), KGUI.GetControlText("MyMaps.page2.info.inp_password"));
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.game_type.ibtn_next", delegate()
			{
				if (this._MapGameType >= this._MapGameTypes.Length - 1)
				{
					return;
				}
				this._MapGameType++;
				while (this._IgnoreMapGameTypes.Contains(this._MapGameTypes[this._MapGameType]))
				{
					this._MapGameType++;
				}
				if (this._MapGameType == this._MapGameTypes.Length)
				{
					this._MapGameType = 0;
				}
				KGUI.SetControlText("MyMaps.page2.info.game_type.txt_type", Localize.GetText("GAME_TYPE_" + this._MapGameTypes[this._MapGameType], null));
				KGUI.FindNode("MyMaps.page2.info.grid_map_info", false).GetComponent<UITable>().Reposition();
				KGUI.ResetScrollBar("MyMaps.page2.info.clip_map_info", null);
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.game_type.ibtn_prev", delegate()
			{
				if (this._MapGameType <= 0)
				{
					return;
				}
				this._MapGameType--;
				while (this._IgnoreMapGameTypes.Contains(this._MapGameTypes[this._MapGameType]))
				{
					this._MapGameType--;
				}
				if (this._MapGameType < 0)
				{
					this._MapGameType = 0;
				}
				KGUI.SetControlText("MyMaps.page2.info.game_type.txt_type", Localize.GetText("GAME_TYPE_" + this._MapGameTypes[this._MapGameType], null));
				KGUI.FindNode("MyMaps.page2.info.grid_map_info", false).GetComponent<UITable>().Reposition();
				KGUI.ResetScrollBar("MyMaps.page2.info.clip_map_info", null);
			});
			KGUI.SetButtonCallback("MyMaps.page2.info.ibtn_public_map", delegate()
			{
				int num = KGUI.GetControlText("MyMaps.page2.info.inp_name").Count((char c) => !char.IsWhiteSpace(c));
				if (KGUI.GetControlText("MyMaps.page2.info.inp_name").Trim() == string.Empty || num < 3)
				{
					this.ShowHint(Localize.GetText("key1300", null), false);
					return;
				}
				this.StartCoroutine(this.PublicMapToServer(KGUI.GetControlText("MyMaps.page2.info.inp_name"), KGUI.GetControlText("MyMaps.page2.info.inp_description"), this._CustomMapImageURL));
			});
			KGUI.SetButtonCallback(new string[]
			{
				"MyMaps.page2.info.screen",
				"MyMaps.page2.info.screen_custom"
			}, delegate()
			{
				this.ShowInputText("INPUT_TEXT_PICTURE_URL1", "INPUT_TEXT_PICTURE_URL2", delegate(string text)
				{
					this.StartCoroutine(this.GetCustomMapImage(this._SelectedMapID, "MyMaps.page2.info.screen", text));
				}, string.Empty);
			});
		});
		this.RegisterMenu(Menu.Settings, "main_menu_settings", delegate(object data, object dataEx)
		{
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("SETTINGS_TITLE1", null));
			UISlider component = KGUI.FindNode("main_menu_settings.sld_hor_mouse_sensivity", false).GetComponent<UISlider>();
			UISlider component2 = KGUI.FindNode("main_menu_settings.sld_hor_sound_volume", false).GetComponent<UISlider>();
			UISlider component3 = KGUI.FindNode("main_menu_settings.sld_hor_ambient_volume", false).GetComponent<UISlider>();
			component.sliderValue = (ProfileINI.mouse_sens - 1f) / 29f;
			component2.sliderValue = ProfileINI.sound_volume;
			component3.sliderValue = ProfileINI.ambient_volume;
			KGUI.SetControlText("main_menu_settings.mouse_sensivity.txt_percent", (int)(component.sliderValue * 100f) + "%");
			KGUI.SetControlText("main_menu_settings.sound_volume.txt_percent", (int)(component2.sliderValue * 100f) + "%");
			KGUI.SetControlText("main_menu_settings.ambient_volume.txt_percent", (int)(component3.sliderValue * 100f) + "%");
			int locale = (int)Localize.Locale;
			string text = Localize.GetText("LANGUAGE_" + ((locale != 0) ? "EN" : "RU"), null);
			KGUI.SetControlText("main_menu_settings.language.text_type", text);
			if (App.Instance.CurPlatform == App.Platform.STEAM)
			{
				KGUI.SetControlCheckbox("main_menu_settings.ckb_show_fs", ProfileINI.full_screen);
			}
			else
			{
				KGUI.SetNodes("main_menu_settings.full_screen", false, false);
			}
			KGUI.SetControlCheckbox("main_menu_settings.ckb_show_bubbles", ProfileINI.showBaloons);
			if (SceneManager.GetActiveScene().name == "Game")
			{
				KGUI.SetNodes("main_menu_settings.ambient_occlusion", false, false);
				KGUI.SetNodes("main_menu_settings.bloom", false, false);
				KGUI.SetNodes("main_menu_settings.slider_settings", false, false);
			}
			else
			{
				KGUI.SetControlCheckbox("main_menu_settings.ckb_ambient_occlusion", ProfileINI.ambientOcclusion);
				KGUI.SetControlCheckbox("main_menu_settings.ckb_bloom", ProfileINI.bloom);
			}
		}, delegate()
		{
			KGUI.SetSliderCallback("main_menu_settings.sld_hor_mouse_sensivity", delegate
			{
				UISlider component = KGUI.CallbackSender.GetComponent<UISlider>();
				ProfileINI.mouse_sens = component.sliderValue * 29f + 1f;
				KGUI.SetControlText("main_menu_settings.mouse_sensivity.txt_percent", (int)(component.sliderValue * 100f) + "%");
			});
			KGUI.SetSliderCallback("main_menu_settings.sld_hor_sound_volume", delegate
			{
				UISlider component = KGUI.CallbackSender.GetComponent<UISlider>();
				ProfileINI.sound_volume = component.sliderValue;
				KGUI.SetControlText("main_menu_settings.sound_volume.txt_percent", (int)(component.sliderValue * 100f) + "%");
			});
			KGUI.SetSliderCallback("main_menu_settings.sld_hor_ambient_volume", delegate
			{
				UISlider component = KGUI.CallbackSender.GetComponent<UISlider>();
				ProfileINI.ambient_volume = component.sliderValue;
				KGUI.SetControlText("main_menu_settings.ambient_volume.txt_percent", (int)(component.sliderValue * 100f) + "%");
			});
			KGUI.SetButtonCallback("main_menu_settings.language.btn_prev", delegate()
			{
				if (ProfileINI.ilang <= 0)
				{
					return;
				}
				ProfileINI.ilang--;
				KGUI.SetLocale(Localize.LocaleType.RU);
				KGUI.SetControlText("main_menu_settings.language.text_type", Localize.GetText("LANGUAGE_RU", null));
				KGUI.SetControlText("main_menu.txt_title", Localize.GetText("SETTINGS_TITLE1", null));
			});
			KGUI.SetButtonCallback("main_menu_settings.language.btn_next", delegate()
			{
				if (ProfileINI.ilang >= 1)
				{
					return;
				}
				ProfileINI.ilang++;
				KGUI.SetLocale(Localize.LocaleType.EN);
				KGUI.SetControlText("main_menu_settings.language.text_type", Localize.GetText("LANGUAGE_EN", null));
				KGUI.SetControlText("main_menu.txt_title", Localize.GetText("SETTINGS_TITLE1", null));
			});
			KGUI.SetCheckboxCallback("main_menu_settings.ckb_show_bubbles", delegate
			{
				UICheckbox component = KGUI.CallbackSender.GetComponent<UICheckbox>();
				ProfileINI.showBaloons = component.isChecked;
				ProfileINI.Save();
			});
			KGUI.SetCheckboxCallback("main_menu_settings.ckb_show_fs", delegate
			{
				UICheckbox component = KGUI.CallbackSender.GetComponent<UICheckbox>();
				ProfileINI.SetFullScreen(component.isChecked);
			});
			KGUI.SetCheckboxCallback("main_menu_settings.ckb_ambient_occlusion", delegate
			{
				UICheckbox component = KGUI.CallbackSender.GetComponent<UICheckbox>();
				ProfileINI.ambientOcclusion = component.isChecked;
				ProfileINI.Save();
			});
			KGUI.SetCheckboxCallback("main_menu_settings.ckb_bloom", delegate
			{
				UICheckbox component = KGUI.CallbackSender.GetComponent<UICheckbox>();
				ProfileINI.bloom = component.isChecked;
				ProfileINI.Save();
			});
			KGUI.SetButtonCallback("main_menu_settings.button_save_setting.ibtn_save_setting", delegate()
			{
				this.StartCoroutine(this.SaveSettingToBase());
				if (SceneManager.GetActiveScene().name == "Game")
				{
					WorldGameObjectX.Instance.SetAmbientSoundOnLevel();
				}
			});
		});
		this.RegisterMenu(Menu.Shop, "Shop", delegate(object data, object dataEx)
		{
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("SHOP_TITLE1", null));
			KGUI.SetNodes("Shop.Canvas", false, false);
			KGUI.SetNodes("Shop.page2_shop", false, false);
			if (data != null)
			{
				this._ShopTab = (Store.TabType)((int)data);
			}
			this.RefreshShop();
			if (dataEx != null)
			{
				this.ShowPurchaseItem((StorePurchase)((int)dataEx));
			}
		}, delegate()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < Store.TabTypeButtonNames.Length; i++)
			{
				string text = "Shop" + Store.TabTypeButtonNames[i];
				if (KGUI.FindNode(text, true) != null)
				{
					list.Add(text);
				}
			}
			KGUI.SetButtonCallback(list.ToArray(), delegate()
			{
				Store.TabType shopTab = this._ShopTab;
				for (int j = 0; j < Store.TabTypeButtonNames.Length; j++)
				{
					if (Store.TabTypeButtonNames[j].Contains(KGUI.CallbackSender.name))
					{
						this._ShopTab = (Store.TabType)j;
						break;
					}
				}
				if (this._ShopTab != shopTab)
				{
					this.RefreshShop();
				}
			});
			KGUI.SetButtonCallback("Shop.items.ibtn_select", delegate()
			{
				this.ShowPurchaseItem(this._CurShopPurchases[this._CurShopPurchases.Keys.ElementAt(KGUI.SlotIndex2)][KGUI.SlotIndex]);
			});
			KGUI.SetButtonCallback("Shop.icons.ibtn_next_items", delegate()
			{
				KGUI.FindNode("Shop.clip_icons", false).GetComponent<UIDraggablePanel>().Scroll(0.4f);
			});
			KGUI.SetButtonCallback("Shop.icons.ibtn_prev_items", delegate()
			{
				KGUI.FindNode("Shop.clip_icons", false).GetComponent<UIDraggablePanel>().Scroll(-0.4f);
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_item_look", delegate()
			{
				bool flag = false;
				List<EntityType> data;
				List<BlockType> dataEx;
				mdl_Item_Build dataEx2;
				this.GetPurchaseContent(out data, out dataEx, out flag, out dataEx2);
				if (flag)
				{
					this.SetMenu(Menu.ItemsPack, true, flag, dataEx2);
				}
				else
				{
					this.SetMenu(Menu.ItemsPack, true, data, dataEx);
				}
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_item_info", delegate()
			{
				this._ShopPurchaseItemInfo = true;
				this.ShowPurchaseItem(this._CurPurchase);
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_item_back", delegate()
			{
				this._ShopPurchaseItemInfo = false;
				this.ShowPurchaseItem(this._CurPurchase);
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_rotate_left", delegate()
			{
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_rotate_right", delegate()
			{
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_buy_one", delegate()
			{
				this.StartCoroutine(this.PurchaseBuy(this._CurPurchase, 0));
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_buy_day", delegate()
			{
				this.StartCoroutine(this.PurchaseBuy(this._CurPurchase, 1));
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_buy_week", delegate()
			{
				this.StartCoroutine(this.PurchaseBuy(this._CurPurchase, 2));
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_buy_forever", delegate()
			{
				this.StartCoroutine(this.PurchaseBuy(this._CurPurchase, 3));
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_buy_month", delegate()
			{
				this.StartCoroutine(this.PurchaseBuy(this._CurPurchase, 4));
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_equip", delegate()
			{
				this.StartCoroutine(ProfileINI.SetSkin(Store.Purchases[this._CurPurchase].Skin));
				this.ShowPurchaseItem(this._CurPurchase);
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_equip_wskin", delegate()
			{
				Store.PurchaseInfoWeaponSkin purchaseInfoWeaponSkin = (Store.PurchaseInfoWeaponSkin)Store.Purchases[this._CurPurchase];
				ProfileINI.WeaponSkinData.SelectSkin(purchaseInfoWeaponSkin.SkinType, purchaseInfoWeaponSkin.SkinId);
				this.StartCoroutine(ProfileINI.SaveWeaponSkin());
				this.ShowPurchaseItem(this._CurPurchase);
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_takeoff_wskin", delegate()
			{
				Store.PurchaseInfoWeaponSkin purchaseInfoWeaponSkin = (Store.PurchaseInfoWeaponSkin)Store.Purchases[this._CurPurchase];
				ProfileINI.WeaponSkinData.SelectSkin(purchaseInfoWeaponSkin.SkinType, 0);
				this.StartCoroutine(ProfileINI.SaveWeaponSkin());
				this.ShowPurchaseItem(this._CurPurchase);
			});
			KGUI.SetButtonCallback("Shop.page2_shop.ibtn_use", delegate()
			{
			});
			KGUI.SetTooltipCallback("Shop.page2_shop.weapon_stats.stat1_icon", () => Localize.GetText("WEAPON_STAT_DAMAGE", null));
			KGUI.SetTooltipCallback("Shop.page2_shop.weapon_stats.stat2_icon", () => Localize.GetText("WEAPON_STAT_RATE", null));
			KGUI.SetTooltipCallback("Shop.page2_shop.weapon_stats.stat3_icon", () => Localize.GetText("WEAPON_STAT_ACCURACY", null));
			KGUI.SetTooltipCallback("Shop.page2_shop.weapon_stats.stat4_icon", () => Localize.GetText("WEAPON_STAT_VOLUME", null));
		});
		this.RegisterMenu(Menu.ItemsPack, "items_pack", delegate(object data, object dataEx)
		{
			bool isShowBuilsd = false;
			if (data is bool)
			{
				isShowBuilsd = true;
				mdl_Item_Build itemsBuildId = (mdl_Item_Build)dataEx;
				this._TempBuildItemId = new List<int>();
				KGUI.ResizeGrid("items_pack.grid_items", itemsBuildId.build_items_id.Count, delegate(GameObject slot, int index)
				{
					IS_mdl_Item item = IS_Manager.GetItemById(itemsBuildId.build_items_id[index]);
					slot.transform.Find("icon").GetComponent<UISprite>().spriteName = item.IconName;
					slot.transform.Find("count").GetComponent<UILabel>().text = ((item.Count <= 1) ? string.Empty : (item.Count * itemsBuildId.build_counts[index]).ToString());
					this._TempBuildItemId.Add(item.Id);
					KGUI.SetButtonHoverOnCallback("items_pack.grid_items." + index, delegate
					{
						if (!KGUI.FindNode("items_pack.tooltip", false).GetComponent<pnl_Tooltip>().IsShowNow() && isShowBuilsd)
						{
							KGUI.FindNode("items_pack.tooltip", false).GetComponent<pnl_Tooltip>().Show(Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition), item, 64);
						}
						else
						{
							KGUI.FindNode("items_pack.tooltip", false).GetComponent<pnl_Tooltip>().HideShop();
						}
					});
				}, "emotions");
			}
			else
			{
				isShowBuilsd = false;
				List<EntityType> entities = (List<EntityType>)data;
				List<BlockType> blocks = (List<BlockType>)dataEx;
				KGUI.ResizeGrid("items_pack.grid_items", entities.Count + blocks.Count, delegate(GameObject slot, int index)
				{
					string value;
					if (index < entities.Count)
					{
						value = Store.Entities[entities[index]].SpriteName.Value;
					}
					else
					{
						value = Store.Blocks[blocks[index - entities.Count]].SpriteName.Value;
					}
					slot.transform.Find("icon").GetComponent<UISprite>().spriteName = value;
					KGUI.RemoveButtonHoverOnCallback("items_pack.grid_items." + index);
					try
					{
						if (Store.Entities[entities[index]].count.Value > 0)
						{
							slot.transform.Find("count").GetComponent<UILabel>().text = Store.Entities[entities[index]].count.Value.ToString();
						}
						else
						{
							slot.transform.Find("count").GetComponent<UILabel>().text = string.Empty;
						}
					}
					catch
					{
						slot.transform.Find("count").GetComponent<UILabel>().text = string.Empty;
					}
					slot.transform.FindChild("icon").GetComponent<UISprite>().MakePixelPerfect();
				}, "emotions");
			}
		}, delegate()
		{
			KGUI.SetButtonCallback("items_pack.ibtn_close", delegate()
			{
				this.SetMenu(Menu.ItemsPack, false, null, null);
			});
		});
		this.RegisterMenu(Menu.Inventory, "inventory", delegate(object data, object dataEx)
		{
			if (data != null)
			{
				this._InventoryTab = (Store.TabType)((int)data);
			}
			this.RefreshInventory();
			KGUI.EnableNodes("inventory.Left.block.grid.ibtn_blocks_f1", true, false);
			KGUI.EnableNodes("inventory.Left.block.grid.ibtn_blocks_f2", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_CORNER) > 0, false);
			KGUI.EnableNodes("inventory.Left.block.grid.ibtn_blocks_f3", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR) > 0, false);
			KGUI.EnableNodes("inventory.Left.block.grid.ibtn_blocks_f4", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_QUARTER) > 0, false);
			KGUI.EnableNodes("inventory.Left.block.grid.ibtn_blocks_f5", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_HALF) > 0, false);
			KGUI.EnableNodes("inventory.Left.block.grid.ibtn_blocks_f6", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_FENCE) > 0, false);
			KGUI.EnableNodes("inventory.Left.block.grid.ibtn_blocks_f7", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_DIAGONAL) > 0, false);
			KGUI.EnableNodes("inventory.Left.block.grid.ibtn_blocks_f8", ProfileINI.GetPurchaseValue(StorePurchase.BLOCK_KIND_STAIR_CORNER) > 0, false);
		}, delegate()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < Store.TabTypeButtonNames.Length; i++)
			{
				string text = "inventory" + Store.TabTypeButtonNames[i];
				if (KGUI.FindNode(text, true) != null)
				{
					list.Add(text);
				}
			}
			KGUI.SetButtonCallback(list.ToArray(), delegate()
			{
				Store.TabType inventoryTab = this._InventoryTab;
				for (int j = 0; j < Store.TabTypeButtonNames.Length; j++)
				{
					if (Store.TabTypeButtonNames[j].Contains(KGUI.CallbackSender.name))
					{
						this._InventoryTab = (Store.TabType)j;
						if (this._InventoryTab == Store.TabType.Blocks)
						{
							this._InventoryTab = Store.TabType.Blocks_Wood;
						}
						break;
					}
				}
				if (this._InventoryTab != inventoryTab)
				{
					this.RefreshInventory();
				}
			});
			KGUI.SetButtonCallback("inventory.items.ibtn_select", delegate()
			{
				if (GameType.BuildDisabled())
				{
					return;
				}
				if (!this._InventoryTab.ToString().Contains("Blocks"))
				{
					EntityType entityType = this._CurInventoryItems[this._CurInventoryItems.Keys.ElementAt(KGUI.SlotIndex2)][KGUI.SlotIndex];
					if (entityType != EntityType.RAIL)
					{
						WorldGameObjectX.Instance.AddPreview(entityType);
						StorePurchase storePurchase = Store.Entities[entityType].Purchase;
						if (storePurchase != StorePurchase.NONE)
						{
							Store.Pay cost = Store.Purchases[storePurchase].Cost;
							if (cost is Store.OnePay && !((Store.OnePay)cost).Once && ProfileINI.CheckPurchaseValue(storePurchase))
							{
								int purchaseValue = ProfileINI.GetPurchaseValue(storePurchase);
								if (purchaseValue > 0)
								{
									this.PurchaseUse(storePurchase, false);
								}
							}
						}
					}
					else
					{
						WorldGameObjectX.Instance.CurrentBlock = BlockType.Air;
						WorldGameObjectX.Instance.CurrentBlockEntity = EntityType.RAIL;
						PreviewCubes.Instance.DrawRail();
					}
				}
				else
				{
					BlockType blockType = this._CurInventoryBlocks[this._CurInventoryBlocks.Keys.ElementAt(KGUI.SlotIndex2)][KGUI.SlotIndex];
					WorldGameObjectX.Instance.CurrentBlock = blockType;
					WorldGameObjectX.Instance.CurrentBlockEntity = EntityType.AIR;
					PreviewCubes.Instance.DrawCube();
					PreviewCubes.Instance.Cube.GetComponent<Renderer>().material.mainTexture = WorldData.Instance.BlockTextures[blockType];
				}
				this.HideMenu();
			});
			KGUI.SetButtonCallback("inventory.icons.ibtn_next_items", delegate()
			{
				KGUI.FindNode("inventory.clip_icons", false).GetComponent<UIDraggablePanel>().Scroll(0.4f);
			});
			KGUI.SetButtonCallback("inventory.icons.ibtn_prev_items", delegate()
			{
				KGUI.FindNode("inventory.clip_icons", false).GetComponent<UIDraggablePanel>().Scroll(-0.4f);
			});
			KGUI.SetButtonCallback("inventory.ibtn_back", delegate()
			{
				if (this._InventoryTab.ToString().Contains("Blocks_"))
				{
					this._InventoryTab = Store.TabType.Blocks;
					this.RefreshInventory();
				}
				else
				{
					this.HideMenu();
				}
			});
			KGUI.SetButtonCallback("inventory.ibtn_shop", delegate()
			{
				if (this._InventoryTab.ToString().Contains("Blocks_"))
				{
					this.SwitchMenu(Menu.Shop, Store.TabType.Blocks, null);
				}
				else
				{
					this.SwitchMenu(Menu.Shop, this._InventoryTab, null);
				}
			});
			KGUI.SetButtonCallback("inventory.Left.block.grid.ibtn_blocks_f1", delegate()
			{
				if (MainMenu.isSetBlockKind != null)
				{
					MainMenu.isSetBlockKind(CommonBlockKind.Default);
				}
				this.HideMenu();
			});
			KGUI.SetButtonCallback("inventory.Left.block.grid.ibtn_blocks_f2", delegate()
			{
				if (MainMenu.isSetBlockKind != null)
				{
					MainMenu.isSetBlockKind(CommonBlockKind.Diagonal);
				}
				if (MainMenu.isSetBlockKind != null)
				{
					MainMenu.isSetBlockKind(CommonBlockKind.Diagonal);
				}
				this.HideMenu();
				UnityEngine.Debug.Log("inventory.Left.block.grid.ibtn_blocks_f2");
			});
			KGUI.SetButtonCallback("inventory.Left.block.grid.ibtn_blocks_f3", delegate()
			{
				if (MainMenu.isSetBlockKind != null)
				{
					MainMenu.isSetBlockKind(CommonBlockKind.Stair);
				}
				if (MainMenu.isSetBlockKind != null)
				{
					MainMenu.isSetBlockKind(CommonBlockKind.Stair);
				}
				this.HideMenu();
			});
			KGUI.SetButtonCallback("inventory.Left.block.grid.ibtn_blocks_f4", delegate()
			{
				if (MainMenu.isSetBlockKind != null)
				{
					MainMenu.isSetBlockKind(CommonBlockKind.Quarter);
				}
				this.HideMenu();
			});
			KGUI.SetButtonCallback("inventory.Left.block.grid.ibtn_blocks_f5", delegate()
			{
				if (MainMenu.isSetBlockKind != null)
				{
					MainMenu.isSetBlockKind(CommonBlockKind.Half);
				}
				this.HideMenu();
			});
			KGUI.SetButtonCallback("inventory.Left.block.grid.ibtn_blocks_f6", delegate()
			{
				if (MainMenu.isSetBlockKind != null)
				{
					MainMenu.isSetBlockKind(CommonBlockKind.Fence);
				}
				if (MainMenu.isSetBlockKind != null)
				{
					MainMenu.isSetBlockKind(CommonBlockKind.Fence);
				}
				this.HideMenu();
			});
		});
		this.RegisterMenu(Menu.Servers, "MapsList", delegate(object data, object dataEx)
		{
			ServerList.Instance.OfflineStart();
			KGUI.SetNodes("MapsList.TeamBattle", false, false);
			KGUI.SetNodes("MapsList.Run", false, false);
			KGUI.SetNodes("MapsList.HungerGames", false, false);
			KGUI.SetNodes("MapsList.HideSeek", false, false);
			KGUI.SetNodes("MapsList.ZombieVirus", false, false);
			Info.Instance.Location = "MapsList";
		}, delegate()
		{
			KGUI.SetNodes("main_menu.slider_page1", false, false);
			KGUI.SetButtonCallback("MapsList.servers_grid.ibtn_select", delegate()
			{
				RoomInfo selectedRoom = ServerList.Instance._ServerRooms[KGUI.SlotIndex];
				ServerList.Instance._SelectedRoom = selectedRoom;
				if (ServerList.Instance._SelectedRoom.customProperties.ContainsKey("password") && ((string)ServerList.Instance._SelectedRoom.customProperties["password"]).Length > 0)
				{
					this.MapPassword = (string)ServerList.Instance._SelectedRoom.customProperties["password"];
					this.ShowInputText2("key1002", "key1001", delegate(string text)
					{
						this.InputPassword = text;
						this.ChekMapData();
					}, string.Empty);
				}
				else if (this.CheckMapCanLoad())
				{
					this.JoinGame(ServerList.Instance._SelectedRoom);
				}
			});
		});
		this.RegisterMenu(Menu.TopMaps, "main_menu_top_maps", delegate(object data, object dataEx)
		{
			Info.Instance.Location = "TopMaps";
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("TOP_MAPS_TITLE1", null));
			KGUI.SetNodes("main_menu_top_maps.page2", false, false);
			KGUI.ResizeGrid("main_menu_top_maps.top_maps_grid", 0, delegate(GameObject slot, int index)
			{
			}, null);
			KGUI.SetNodes("main_menu_top_maps.buttons_gag", false, true);
			if (data == null)
			{
				KGUI.SetNodes("main_menu_top_maps.buttons_gag.new_maps", true, false);
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_NEW", null));
				this.StartCoroutine(this.RefreshListOfMapsNew());
			}
			else
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", string.Empty);
				this.StartCoroutine(this.LoadTopMapInfo((int)data));
			}
		}, delegate()
		{
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_new_maps", delegate()
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_NEW", null));
				KGUI.SetNodes("main_menu_top_maps.buttons_gag", false, true);
				KGUI.SetNodes("main_menu_top_maps.buttons_gag.new_maps", true, false);
				this.StartCoroutine(this.RefreshListOfMapsNew());
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_best_maps", delegate()
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_ALL", null));
				KGUI.SetNodes("main_menu_top_maps.buttons_gag", false, true);
				KGUI.SetNodes("main_menu_top_maps.buttons_gag.best_maps", true, false);
				this.StartCoroutine(this.RefreshListOfMapsBestAllTime());
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_best_maps_for_day", delegate()
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_DAY", null));
				KGUI.SetNodes("main_menu_top_maps.buttons_gag", false, true);
				KGUI.SetNodes("main_menu_top_maps.buttons_gag.best_maps_for_day", true, false);
				this.StartCoroutine(this.RefreshListOfDay());
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_best_maps_for_week", delegate()
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_WEEK", null));
				KGUI.SetNodes("main_menu_top_maps.buttons_gag", false, true);
				KGUI.SetNodes("main_menu_top_maps.buttons_gag.best_maps_for_week", true, false);
				this.StartCoroutine(this.RefreshListOfMapsWeekly());
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_best_maps_for_month", delegate()
			{
				KGUI.SetControlText("main_menu_top_maps.txt_best_period", Localize.GetText("TOP_MAPS_MONTH", null));
				KGUI.SetNodes("main_menu_top_maps.buttons_gag", false, true);
				KGUI.SetNodes("main_menu_top_maps.buttons_gag.best_maps_for_month", true, false);
				this.StartCoroutine(this.RefreshListOfMapsMonthly());
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_select", delegate()
			{
				if (KGUI.SlotIndex < this._Maps.Count)
				{
					this.StartCoroutine(this.LoadTopMapInfo(int.Parse(this._Maps[KGUI.SlotIndex].MapID)));
				}
				else
				{
					this.StartCoroutine(this.LoadMoreMaps());
				}
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_look", delegate()
			{
				this.JoinOrCreateGame(this._SelectedMapOwner, this._SelectedMapSlot, true, KGUI.GetControlText("main_menu_top_maps.page2.txt_name"), KGUI.GetControlText("main_menu_top_maps.page2.txt_description"), string.Empty);
			});
			KGUI.SetButtonCallback("main_menu_top_maps.ibtn_copy_url", delegate()
			{
				string text = "https://vk.com/diggeronline#map_id_" + this._SelectedMapID;
				if (text != null)
				{
					NGUITools.clipboard = text;
					this.ShowHint(Localize.GetText("TOP_MAPS_COPY_URL_DONE", null) + "\n" + text, false);
				}
				else
				{
					this.ShowHint("TOP_MAPS_COPY_URL_UNSUPPORTED", false);
				}
			});
			KGUI.SetButtonCallback("main_menu_top_maps.score.ibtn_vote_yes", delegate()
			{
				KGUI.SetNodes("main_menu_top_maps.score.background_large", false, false);
				KGUI.SetNodes("main_menu_top_maps.score.background_small", true, false);
				KGUI.SetNodes("main_menu_top_maps.score.button_vote_yes", false, false);
				int num = int.Parse(KGUI.GetControlText("main_menu_top_maps.txt_score"));
				KGUI.SetControlText("main_menu_top_maps.txt_score", string.Empty + (num + 1));
				this._SelectedMapNotVoted = false;
				this.StartCoroutine(this.VoteYes(this._SelectedMapID));
			});
		});
		this.RegisterMenu(Menu.Loading, "loading", delegate(object data, object dataEx)
		{
			KGUI.SetNodes("loading.button_fight", false, false);
		}, delegate()
		{
			KGUI.SetButtonCallback("loading.button_fight.ibtn_exit", delegate()
			{
				WorldGameObjectX.Instance.CloseServerAndExit();
				WorldGameObjectX.Instance.MapExit();
			});
		});
		this.RegisterMenu(Menu.Hint, "hint", delegate(object data, object dataEx)
		{
			KGUI.SetControlText("hint.txt_info", Localize.GetText((string)data, null));
		}, delegate()
		{
			KGUI.SetButtonCallback("hint.ibtn_ok", delegate()
			{
				this.HideHint();
			});
		});
		this.RegisterMenu(Menu.Premium, "Premium", delegate(object data, object dataEx)
		{
		}, delegate()
		{
			KGUI.SetButtonCallback("Premium.ButtonBuy", delegate()
			{
				UnityEngine.Debug.Log("Подписка на премиум");
				Application.ExternalCall("BuyPremium", new object[0]);
			});
			KGUI.SetButtonCallback("Premium.ButtonBack", delegate()
			{
				UnityEngine.Debug.Log("Кнопка назад");
				this.SwitchMenu(Menu.Shop, Store.TabType.Weapons, null);
				KGUI.SetNodes("Premium", false, false);
			});
		});
		this.RegisterMenu(Menu.NotEnoughMoney, "not_enough_money", delegate(object data, object dataEx)
		{
			Currency currency = (Currency)((int)((object[])data)[0]);
			int num = (int)((object[])data)[1];
			int num2 = num - ProfileINI.money[(int)currency];
			KGUI.SetNodes("not_enough_money.icon_gold", currency == Currency.Gold, false);
			this.SetMoneyValue("not_enough_money.txt_count", currency, num2.ToString());
			if (currency != Currency.Gold)
			{
				KGUI.SetNodes("not_enough_money.button_buy", false, false);
			}
		}, delegate()
		{
			KGUI.SetButtonCallback("not_enough_money.ibtn_ok", delegate()
			{
				this.SetMenu(Menu.NotEnoughMoney, false, null, null);
			});
			KGUI.SetButtonCallback("not_enough_money.ibtn_buy", delegate()
			{
				this.SetMenu(Menu.NotEnoughMoney, false, null, null);
				this.SwitchMenu(Menu.Bank, null, null);
			});
		});
		this.RegisterMenu(Menu.Bonus, "bonus", delegate(object data, object dataEx)
		{
			int num = ((int[])data)[0];
			int num2 = ((int[])data)[1];
			int num3 = ((int[])data)[2];
			for (int i = 5; i > num; i--)
			{
				KGUI.SetNodes("bonus.stages.stage" + i + "_on", false, false);
			}
			KGUI.SetControlText("bonus.txt_days", num + "/5 " + Localize.GetText("BONUS_DAYS", null));
			KGUI.SetNodes("bonus.stages.stage4_q", false, false);
			if (num2 > 0)
			{
				KGUI.SetControlText("bonus.txt_count", string.Empty + num2);
				KGUI.SetControlText("bonus.txt_count2", "+ 5exp");
				KGUI.SetNodes("bonus.gems", false, false);
			}
			if (num3 > 0)
			{
				KGUI.SetControlText("bonus.txt_count", string.Empty + num3);
				KGUI.SetControlText("bonus.txt_count2", "+ 5exp");
				KGUI.SetNodes("bonus.gold", false, false);
			}
			SoundManager.Instance.Play(SoundManager.Sound.EverydayBonus, this.GetComponent<AudioSource>());
		}, delegate()
		{
			KGUI.SetButtonCallback("bonus.ibtn_ok", delegate()
			{
				this.HideMenu();
				this.ShowLoading("LOADING_PROFILE", string.Empty);
				this.StartCoroutine(App.Instance.LoadProfile2(true));
			});
		});
		this.RegisterMenu(Menu.BonusGems, "bonus_gems", delegate(object data, object dataEx)
		{
			this._BonusData = (int[])data;
			SoundManager.Instance.Play(SoundManager.Sound.EverydayBonus, this.GetComponent<AudioSource>());
		}, delegate()
		{
			KGUI.SetButtonCallback("bonus_gems.ibtn_ok", delegate()
			{
				if (this._BonusData[0] != -1)
				{
					this.SwitchMenu(Menu.Bonus, this._BonusData, null);
				}
				else
				{
					this.StartCoroutine(App.Instance.LoadProfile2(true));
				}
			});
		});
		this.RegisterMenu(Menu.ServerSelect, "server_select", delegate(object data, object dataEx)
		{
			int num = App.Instance.PlatformShardCount[(int)App.Instance.CurPlatform];
			int bestServerIndex = -1;
			int bestServerLoad = 0;
			KGUI.ResizeGrid("server_select.grid_servers_list", num, delegate(GameObject slot, int index)
			{
				int num3 = (int)(100f * (float)App.Instance.ShardPopulation[index] / (float)App.Instance.MaxShardPopulation);
				if (num3 >= 100)
				{
					slot.GetComponentInChildren<UILabel>().text = string.Concat(new object[]
					{
						Localize.GetText("SERVER_SELECT_SERVER_NAME", null),
						" ",
						index + 1,
						" ",
						Localize.GetText("SERVER_SELECT_SERVER_FULL", null)
					});
				}
				else
				{
					slot.GetComponentInChildren<UILabel>().text = Localize.GetText("SERVER_SELECT_SERVER_NAME", null) + " " + (index + 1);
				}
				KGUI.SetNodes("server_select." + index + ".utilization_1", num3 >= 25, false);
				KGUI.SetNodes("server_select." + index + ".utilization_2", num3 >= 50, false);
				KGUI.SetNodes("server_select." + index + ".utilization_3", num3 >= 75, false);
				KGUI.SetNodes("server_select." + index + ".utilization_4", num3 >= 100, false);
				if (bestServerIndex == -1 || num3 < bestServerLoad)
				{
					bestServerIndex = index;
					bestServerLoad = num3;
				}
			}, null);
			if (bestServerLoad < 100 && num > 1)
			{
				this._RecommendedShard = bestServerIndex;
				GameObject gameObject = KGUI.FindNode("server_select.recommended_server", false).gameObject;
				int num2 = (int)(100f * (float)App.Instance.ShardPopulation[bestServerIndex] / (float)App.Instance.MaxShardPopulation);
				gameObject.GetComponentInChildren<UILabel>().text = Localize.GetText("RECOMENDED_WORLD", null) + (bestServerIndex + 1);
				KGUI.SetNodes("server_select.recommended_server.utilization_1", num2 >= 25, false);
				KGUI.SetNodes("server_select.recommended_server.utilization_2", num2 >= 50, false);
				KGUI.SetNodes("server_select.recommended_server.utilization_3", num2 >= 75, false);
				KGUI.SetNodes("server_select.recommended_server.utilization_4", num2 >= 100, false);
			}
			else
			{
				KGUI.SetNodes("server_select.recommended_server", false, false);
			}
		}, delegate()
		{
			KGUI.SetButtonCallback(new string[]
			{
				"server_select.ibtn_some_server",
				"server_select.ibtn_recommended_server"
			}, delegate()
			{
				int num = (!(KGUI.CallbackSender.name == "ibtn_some_server")) ? this._RecommendedShard : KGUI.SlotIndex;
				int num2 = (int)(100f * (float)App.Instance.ShardPopulation[num] / (float)App.Instance.MaxShardPopulation);
				if (num2 < 100)
				{
					this.HideMenu();
					this.ShowLoading("LOADING_SERVER", string.Empty);
					App.Instance.CurrentShard = num + 1;
					PhotonNetwork.autoJoinLobby = true;
					PhotonNetwork.ConnectUsingSettings(string.Concat(new object[]
					{
						App.Instance.CurPlatform.ToString(),
						App.Instance.GameVersion,
						"Qshard_",
						App.Instance.CurrentShard
					}));
				}
			});
		});
		this.RegisterMenu(Menu.FastGame, "FastGame", delegate(object data, object dataEx)
		{
			Info.Instance.Location = "FastGame";
			this._FastGameMap = 0;
			this.RefreshStartSelectMap(0);
			KGUI.SetNodes("FastGame.ibtn_next_map", true, false);
			KGUI.SetNodes("FastGame.ibtn_prev_map", true, false);
			KGUI.SetNodes("FastGame.txt_map_name", true, false);
			KGUI.SetNodes("FastGame.txt_map_description2", true, false);
			KGUI.SetNodes("FastGame.button_play2", true, false);
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("START_FAST_GAME", null));
		}, delegate()
		{
			KGUI.SetButtonCallback("FastGame.ibtn_next_map", delegate()
			{
				this.RefreshStartSelectMap(1);
			});
			KGUI.SetButtonCallback("FastGame.ibtn_prev_map", delegate()
			{
				this.RefreshStartSelectMap(-1);
			});
			KGUI.SetButtonCallback("FastGame.ibtn_play2", delegate()
			{
				Maps.StandartMap standartMap = this._FastGameMaps[this._FastGameMap];
				this._MapGameType = this._MapGameTypes.ToList<GameINI.GameType>().IndexOf(standartMap.Game);
				this._MapDestroyable = standartMap.Destroyable;
				UnityEngine.Debug.Log(App.Instance.GetProtocol() + standartMap.MapURL);
				this.JoinOrCreateGame(App.Instance.GetProtocol() + standartMap.MapURL, -1, false, standartMap.Name, standartMap.Description, string.Empty);
			});
		});
		this.RegisterMenu(Menu.Community, "Community", delegate(object data, object dataEx)
		{
			Community.Instance.OfflineStart();
		}, delegate()
		{
		});
		this.RegisterMenu(Menu.Achievements, "Achievements", delegate(object data, object dataEx)
		{
		}, delegate()
		{
		});
		this.RegisterMenu(Menu.Bank, "Bank", delegate(object data, object dataEx)
		{
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("BANK_TITLE", null));
			KGUI.SetNodes("Bank.Canvas", true, false);
			Bank.Instance.SetPrice();
		}, delegate()
		{
		});
		this.RegisterMenu(Menu.Ban, "Ban", delegate(object data, object dataEx)
		{
			KGUI.SetNodes("Ban.Canvas", true, false);
			KGUI.SetNodes("main_menu", false, false);
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}, delegate()
		{
		});
		this.RegisterMenu(Menu.Authorizathion, "Authorizathion", delegate(object data, object dataEx)
		{
			KGUI.SetNodes("Authorizathion.Canvas", true, false);
			KGUI.SetNodes("Authorizathion.AuthLabel", false, false);
			KGUI.SetNodes("main_menu", false, false);
		}, delegate()
		{
		});
		this.RegisterMenu(Menu.TabMenu, "tab_menu", delegate(object data, object dataEx)
		{
			this.RefreshTabMenu();
		}, delegate()
		{
			KGUI.SetButtonCallback("tab_menu.ibtn_close", delegate()
			{
				this.HideMenu();
			});
			KGUI.SetCheckboxCallback("tab_menu.ckb_new_players_building", delegate
			{
				ProfileINI.newgamersislook = KGUI.GetControlCheckbox("tab_menu.ckb_new_players_building");
			});
			KGUI.SetCheckboxCallback("tab_menu.ckb_builder", delegate
			{
				PlayerNode playerNode = WorldGameObjectX.Instance.PlayerList[KGUI.SlotIndex];
				if (!Level.Instance.IsBuilder(playerNode.Name))
				{
					Level.Instance.AddBuilderSafe(playerNode.Name);
					Chat.SendInfoF(Localize.GetText("key4002", null) + playerNode.Name, true);
				}
				else
				{
					Level.Instance.RemoveBuilder(playerNode.Name, null);
					Chat.SendWarning(Localize.GetText("key4003", null) + playerNode.Name, true);
				}
			});
			KGUI.SetCheckboxCallback("tab_menu.ckb_moderator", delegate
			{
				PlayerNode playerNode = WorldGameObjectX.Instance.PlayerList[KGUI.SlotIndex];
				if (!Level.Instance.IsModerator(playerNode.Name))
				{
					Level.Instance.AddModeratorSafe(playerNode.Name);
				}
				else
				{
					Level.Instance.RemoveModerator(playerNode.Name, null);
				}
			});
			KGUI.SetButtonCallback("tab_menu.ibtn_ban", delegate()
			{
				PlayerNode playerNode = WorldGameObjectX.Instance.PlayerList[KGUI.SlotIndex];
				if ((Level.Instance.IsAdmin(null) || Level.Instance.IsModerator(null)) && !Level.Instance.IsBanned(playerNode.NetPlayer.name))
				{
					WorldGameObjectX.Instance.BanPlayer(playerNode);
				}
			});
			KGUI.SetButtonCallback("tab_menu.ibtn_unban", delegate()
			{
				string[] banned = Level.Instance.Banned;
				string playerName = banned[KGUI.SlotIndex - WorldGameObjectX.Instance.PlayerList.Count];
				if ((Level.Instance.IsAdmin(null) || Level.Instance.IsModerator(null)) && Level.Instance.IsBanned(playerName))
				{
					WorldGameObjectX.Instance.UnbanPlayer(playerName);
				}
			});
			KGUI.SetButtonCallback("tab_menu.top_map_info.score.ibtn_vote_yes", delegate()
			{
				KGUI.SetNodes("tab_menu.top_map_info.score.background_large", false, false);
				KGUI.SetNodes("tab_menu.top_map_info.score.background_small", true, false);
				KGUI.SetNodes("tab_menu.top_map_info.score.button_vote_yes", false, false);
				int num = int.Parse(KGUI.GetControlText("tab_menu.top_map_info.txt_score"));
				KGUI.SetControlText("tab_menu.top_map_info.txt_score", string.Empty + (num + 1));
				this._SelectedMapNotVoted = false;
				this.StartCoroutine(this.VoteYes(this._SelectedMapID));
			});
			KGUI.SetButtonCallback("tab_menu.profile", delegate()
			{
				if (KGUI.SlotIndex < WorldGameObjectX.Instance.PlayerList.Count)
				{
					PlayerNode playerNode = WorldGameObjectX.Instance.PlayerList[KGUI.SlotIndex];
					this.SwitchMenu(Menu.Profile, playerNode.ViewerID, null);
				}
			});
			KGUI.SetButtonCallback("tab_menu.grid_players.ibtn_voice", delegate()
			{
				if (KGUI.SlotIndex < WorldGameObjectX.Instance.PlayerList.Count)
				{
					PlayerNode playerNode = WorldGameObjectX.Instance.PlayerList[KGUI.SlotIndex];
					playerNode.Voice = !playerNode.Voice;
					this.RefreshVoiceButton(playerNode, KGUI.FindNode("tab_menu.grid_players." + KGUI.SlotIndex + ".ibtn_voice", false));
				}
			});
			KGUI.SetButtonCallback("tab_menu.grid_players.ibtn_teleport", delegate()
			{
				if (KGUI.SlotIndex < WorldGameObjectX.Instance.PlayerList.Count && ProfileINI.GetPurchaseValue(StorePurchase.TELEPORT) == 1)
				{
					PlayerNode playerNode = WorldGameObjectX.Instance.PlayerList[KGUI.SlotIndex];
					Vector3 position = playerNode.Avatar.transform.position;
					WorldGameObjectX.Instance.MainPlayer.transform.position = position;
					WorldGameObjectX.Instance.photonView.RPC("TeleportPlayer", PhotonTargets.Others, new object[]
					{
						position
					});
				}
			});
		});
		this.RegisterMenu(Menu.Book, "book", delegate(object data, object dataEx)
		{
			Book.Current.RefreshControls();
		}, delegate()
		{
			KGUI.SetButtonCallback("book.ibtn_next", delegate()
			{
				Book.Current.NextButton();
			});
			KGUI.SetButtonCallback("book.ibtn_prev", delegate()
			{
				Book.Current.PrevButton();
			});
			KGUI.SetButtonCallback("book.ibtn_close", delegate()
			{
				Book.Current.CloseButton();
			});
		});
		this.RegisterMenu(Menu.GameMenu, "game_menu", delegate(object data, object dataEx)
		{
			if (!GameType.BattleMode() || TeamBattle.Instance is ZombieVirus || TeamBattle.Instance is HungerGames || TeamBattle.Instance is Arcade || TeamBattle.Instance is HideSeek)
			{
				KGUI.SetNodes("game_menu.choose_weapon", false, false);
			}
			if (GameType.IsHungerGamesMode)
			{
				KGUI.SetNodes("game_menu.inventory_system", true, false);
			}
			else
			{
				KGUI.SetNodes("game_menu.inventory_system", false, false);
			}
			if (GameType.BattleMode())
			{
				KGUI.SetNodes("game_menu.respawn", false, false);
			}
			if (App.Instance.Settings.isWatch || GameType.BuildDisabled())
			{
				KGUI.SetNodes("game_menu.inventory", false, false);
			}
			if (!Level.Instance.IsAdmin(null) || App.Instance.Settings.isWatch || !GameType.CanSaveMap())
			{
				KGUI.SetNodes("game_menu.save", false, false);
			}
			if (!App.Instance.Settings.isWatch)
			{
				KGUI.SetNodes("game_menu.estimate", false, false);
			}
			if (App.Instance.CurPlatform == App.Platform.STEAM || GameType.BattleMode())
			{
				KGUI.SetNodes("game_menu.help", false, false);
			}
			KGUI.FindNode("game_menu.buttons", false).GetComponent<UITable>().Reposition();
		}, delegate()
		{
			KGUI.SetButtonCallback("game_menu.ibtn_resume", delegate()
			{
				this.HideMenu();
			});
			KGUI.SetButtonCallback("game_menu.ibtn_choose_weapon", delegate()
			{
				this.SwitchMenu(Menu.SelectWeapon, true, null);
			});
			KGUI.SetButtonCallback("game_menu.ibtn_inventory_system", delegate()
			{
				this.HideMenu();
				this.OpenCloseInventory();
			});
			KGUI.SetButtonCallback("game_menu.ibtn_respawn", delegate()
			{
				WorldGameObjectX.Instance.Respawn();
				this.HideMenu();
			});
			KGUI.SetButtonCallback("game_menu.ibtn_settings", delegate()
			{
				this.SwitchMenu(Menu.Settings, null, null);
			});
			KGUI.SetButtonCallback("game_menu.ibtn_inventory", delegate()
			{
				WorldGameObjectX.Instance.ToggleInventory();
			});
			KGUI.SetButtonCallback("game_menu.ibtn_save", delegate()
			{
				this.SaveMap(null);
			});
			KGUI.SetButtonCallback("game_menu.ibtn_estimate", delegate()
			{
			});
			KGUI.SetButtonCallback("game_menu.ibtn_get_game_url", delegate()
			{
				string text = null;
				string str = App.Instance.CurrentShard.ToString("D2") + PhotonNetwork.room.name;
				if (App.Instance.CurPlatform == App.Platform.VK)
				{
					text = "https://vk.com/diggeronline#game_id_" + str;
				}
				if (text != null)
				{
					NGUITools.clipboard = text;
					this.ShowHint(Localize.GetText("GAME_URL_URL_DONE", null) + "\n" + text, false);
				}
				else
				{
					this.ShowHint("GAME_URL_URL_UNSUPPORTED", false);
				}
			});
			KGUI.SetButtonCallback("game_menu.ibtn_help", delegate()
			{
				this.SwitchMenu(Menu.Tutorial, null, null);
			});
			KGUI.SetButtonCallback("game_menu.ibtn_exit", delegate()
			{
				if (Level.Instance.IsAdmin(null) && PhotonNetwork.room.playerCount > 1 && !App.Instance.Settings.isWatch)
				{
					MainMenu _003C_003Ef__this = this;
					string title = "EXIT_TEXT";
					string text = "EXIT_DESCRIPTION";
					string[] buttonNames = new string[]
					{
						"EXIT_SERVER_OFF",
						"EXIT_STILL_SERVER",
						"EXIT_RESUME"
					};
					Action[] array = new Action[3];
					array[0] = delegate()
					{
						this.SaveMap(delegate
						{
							WorldGameObjectX.Instance.CloseServerAndExit();
						});
					};
					array[1] = delegate()
					{
						this.SaveMap(delegate
						{
							WorldGameObjectX.Instance.ExitGame(string.Empty);
						});
					};
					array[2] = delegate()
					{
					};
					_003C_003Ef__this.ShowAskMenu(title, text, buttonNames, array, true);
				}
				else
				{
					this.SaveMap(delegate
					{
						WorldGameObjectX.Instance.ExitGame(string.Empty);
					});
				}
			});
			KGUI.SetButtonCallback("game_menu.ibtn_close", delegate()
			{
				this.HideMenu();
			});
		});
		this.RegisterMenu(Menu.AskMenu, "ask_menu", delegate(object data, object dataEx)
		{
			this._AskMenuActions = (Action[])dataEx;
			string[] strData = (string[])data;
			KGUI.SetControlText("ask_menu.txt_title", Localize.GetText(strData[0], null));
			KGUI.SetControlText("ask_menu.txt_info", Localize.GetText(strData[1], null));
			KGUI.ResizeGrid("ask_menu.buttons", this._AskMenuActions.Length, delegate(GameObject slot, int index)
			{
				slot.GetComponentInChildren<UILabel>().text = Localize.GetText(strData[index + 2], null);
			}, null);
			if (strData[strData.Length - 1] != "X")
			{
				KGUI.SetNodes("ask_menu.button_close", false, false);
			}
		}, delegate()
		{
			KGUI.SetButtonCallback("ask_menu.ibtn_select", delegate()
			{
				this.SetMenu(Menu.AskMenu, false, null, null);
				this._AskMenuActions[KGUI.SlotIndex]();
			});
			KGUI.SetButtonCallback("ask_menu.ibtn_close", delegate()
			{
				this.SetMenu(Menu.AskMenu, false, null, null);
			});
		});
		this.RegisterMenu(Menu.InputText, "input_text", delegate(object data, object dataEx)
		{
			this._InputTextAction = (Action<string>)dataEx;
			string[] array = (string[])data;
			KGUI.SetControlText("input_text.txt_title", array[0]);
			KGUI.SetControlText("input_text.txt_info", array[1]);
			KGUI.SetControlText("input_text.inp_input", array[2]);
			KGUI.FindNode("input_text.inp_input", false).GetComponent<UIInput>().selected = true;
		}, delegate()
		{
			KGUI.SetButtonCallback("input_text.ibtn_ok", delegate()
			{
				string controlText = KGUI.GetControlText("input_text.inp_input");
				if (controlText.Length == 0)
				{
					return;
				}
				if (controlText.Length > 15 && !controlText.Contains("http"))
				{
					this.ShowHint(Localize.GetText("MAP_NAME_ERROR", null), false);
					return;
				}
				this.SetMenu(Menu.InputText, false, null, null);
				this._InputTextAction(controlText);
			});
			KGUI.SetButtonCallback("input_text.ibtn_close", delegate()
			{
				this.SetMenu(Menu.InputText, false, null, null);
			});
		});
		this.RegisterMenu(Menu.InputText2, "input_text2", delegate(object data, object dataEx)
		{
			ManagerServerList.ShowWindow = true;
			this._InputTextAction = (Action<string>)dataEx;
			string[] array = (string[])data;
			KGUI.SetControlText("input_text2.txt_title", array[0]);
			KGUI.SetControlText("input_text2.inp_input", array[2]);
			KGUI.FindNode("input_text2.inp_input", false).GetComponent<UIInput>().selected = true;
			KGUI.SetNodes("input_text2.error", false, false);
		}, delegate()
		{
			KGUI.SetButtonCallback("input_text2.ibtn_ok", delegate()
			{
				string controlText = KGUI.GetControlText("input_text2.inp_input");
				if (controlText.Length == 0)
				{
					return;
				}
				this._InputTextAction(controlText);
			});
			KGUI.SetButtonCallback("input_text2.ibtn_close", delegate()
			{
				this.SetMenu(Menu.InputText2, false, null, null);
				ManagerServerList.ShowWindow = false;
			});
		});
		this.RegisterMenu(Menu.Emotions, "emotions", delegate(object data, object dataEx)
		{
			if (ProfileINI.GetPurchaseValue(StorePurchase.EMOTIONS) != 0 && ProfileINI.GetPurchaseValue(StorePurchase.MEM_SMILES) != 0)
			{
				KGUI.SetNodes("emotions.button_more_emotions", false, false);
			}
			SpeechBubbles.Emotion[] activeEmotions = SpeechBubbles.GetActiveEmotions();
			KGUI.ResizeGrid("emotions.grid_emotions", activeEmotions.Length, delegate(GameObject slot, int index)
			{
				slot.transform.Find("icon").GetComponent<UISprite>().spriteName = activeEmotions[index].Picture.name;
			}, "emotions");
		}, delegate()
		{
			KGUI.SetButtonCallback("emotions.ibtn_select", delegate()
			{
				SpeechBubbles.Emotion emotion = SpeechBubbles.GetActiveEmotions()[KGUI.SlotIndex];
				Chat.SendEmotion(emotion.Picture.name, true);
				this.HideMenu();
			});
			KGUI.SetButtonCallback("emotions.ibtn_more_emotions", delegate()
			{
				this.SwitchMenu(Menu.Shop, Store.TabType.Tools, null);
				if (ProfileINI.GetPurchaseValue(StorePurchase.EMOTIONS) == 0)
				{
					this.ShowPurchaseItem(StorePurchase.EMOTIONS);
				}
				else
				{
					this.ShowPurchaseItem(StorePurchase.MEM_SMILES);
				}
			});
			KGUI.SetButtonCallback("emotions.ibtn_close", delegate()
			{
				this.HideMenu();
			});
		});
		this.RegisterMenu(Menu.ChangeName, "change_name", delegate(object data, object dataEx)
		{
			KGUI.SetNodes("change_name.button_close", (bool)data, false);
			KGUI.SetNodes("change_name.txt_name_busy", false, false);
			KGUI.SetControlText("change_name.inp_name", ProfileINI.nickname);
		}, delegate()
		{
			KGUI.SetButtonCallback("change_name.ibtn_ok", delegate()
			{
				string controlText = KGUI.GetControlText("change_name.inp_name");
				if (controlText.Length < 3)
				{
					this.StartCoroutine(this.ShowNameEnterError("NAME_TOO_SHORT"));
				}
				else if (controlText.Length > 12)
				{
					this.StartCoroutine(this.ShowNameEnterError("NAME_TOO_LONG"));
				}
				else if (!string.IsNullOrEmpty(controlText) && Regex.IsMatch(controlText, "^[a-zA-Zа-яА-Я0-9_-]{3,12}$"))
				{
					this.StartCoroutine(this.ChangeNameProcess(controlText));
				}
				else
				{
					this.StartCoroutine(this.ShowNameEnterError("ERROR_NAME_CHAR"));
				}
			});
			KGUI.SetButtonCallback("change_name.ibtn_close", delegate()
			{
				this.SetMenu(Menu.ChangeName, false, null, null);
			});
		});
		Action endxCallback = null;
		this.RegisterMenu(Menu.SaveMapQuestion, "hint2", delegate(object data, object dataEx)
		{
			endxCallback = (Action)dataEx;
			KGUI.SetControlText("hint2.txt_info", Localize.GetText("MY_MAPS_SAVE_Q", null));
		}, delegate()
		{
			KGUI.SetButtonCallback("hint2.ibtn_ok", delegate()
			{
				this.HideMenu();
				this.StartCoroutine(World.Instance.SaveMapToServer(this._SelectedMapSlot, endxCallback));
			});
			KGUI.SetButtonCallback("hint2.ibtn_no", delegate()
			{
				this.HideMenu();
				if (endxCallback != null)
				{
					endxCallback();
				}
			});
		});
		this.RegisterMenu(Menu.Tutorial, "tutorial", delegate(object data, object dataEx)
		{
			KGUI.SetNodes("tutorial.tutorial1", false, true);
			KGUI.SetNodes("tutorial.tutorial1.1", true, false);
		}, delegate()
		{
			KGUI.SetButtonCallback("tutorial.ibtn_next", delegate()
			{
				int num = 0;
				if (KGUI.FindNode("tutorial.tutorial1.1", false).gameObject.activeInHierarchy)
				{
					num = 1;
				}
				else if (KGUI.FindNode("tutorial.tutorial1.2", false).gameObject.activeInHierarchy)
				{
					num = 2;
				}
				else if (KGUI.FindNode("tutorial.tutorial1.3", false).gameObject.activeInHierarchy)
				{
					num = 3;
				}
				else if (KGUI.FindNode("tutorial.tutorial1.4", false).gameObject.activeInHierarchy)
				{
					num = 4;
				}
				if (num < 4)
				{
					KGUI.SetNodes("tutorial.tutorial1." + num, false, false);
					KGUI.SetNodes("tutorial.tutorial1." + (num + 1), true, false);
				}
				else
				{
					this.HideMenu();
				}
			});
		});
		this.RegisterMenu(Menu.SelectWeapon, "select_weapon", delegate(object data, object dataEx)
		{
			if (WorldGameObjectX.Instance.MainPlayer == null)
			{
				this.HideMenu();
				return;
			}
			PlayerNetwork player = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>();
			if ((bool)data)
			{
				if (!WorldGameObjectX.Instance.MainPlayerDead)
				{
					WorldGameObjectX.Instance.KillPlayer();
				}
				player.SelectWeapon(0);
				for (int i = 0; i < player.MainWeapon.Length; i++)
				{
					player.MainWeapon[i] = null;
				}
			}
			this._SelectWeapons.Clear();
			this._SelectWeapons.Add(player.Pistol);
			foreach (Gun gun in player.Guns)
			{
				if ((gun.storePurchase != StorePurchase.NONE && ProfileINI.GetPurchaseValue(gun.storePurchase) != 0 && !this._SelectWeapons.Contains(gun)) || gun.storePurchase == StorePurchase.WEAPON_MP5 || gun.storePurchase == StorePurchase.WEAPON_SAWN_OFF || gun.storePurchase == StorePurchase.WEAPON_GLOK)
				{
					this._SelectWeapons.Add(gun);
				}
			}
			KGUI.ResizeGrid("select_weapon.grid_weapons", this._SelectWeapons.Count, delegate(GameObject slot, int index)
			{
				slot.transform.Find("txt_title").GetComponent<UILabel>().text = string.Empty;
				KGUI.SetControlSprite(slot.transform.Find("icon"), "big_" + this._SelectWeapons[index].name, 100f);
				slot.transform.Find("selected").gameObject.SetActive(player.MainWeapon.Contains(this._SelectWeapons[index]));
			}, "select_weapon");
			this.RefreshBattleWeapon(0);
		}, delegate()
		{
			KGUI.SetButtonCallback("select_weapon.ibtn_buy", delegate()
			{
				this.SwitchMenu(Menu.Shop, Store.TabType.Weapons, null);
			});
			KGUI.SetButtonCallback("select_weapon.ibtn_fight", delegate()
			{
				this.HideMenu();
				if (WorldGameObjectX.Instance.MainPlayerDead)
				{
					WorldGameObjectX.Instance.Respawn();
				}
			});
			KGUI.SetButtonCallback("select_weapon.ibtn_close", delegate()
			{
				this.HideMenu();
			});
			KGUI.SetButtonCallback("select_weapon.ibtn_select", delegate()
			{
				PlayerNetwork component = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>();
				Gun gun = this._SelectWeapons[KGUI.SlotIndex];
				if (!component.MainWeapon.Contains(gun))
				{
					for (int i = 0; i < component.MainWeapon.Length; i++)
					{
						if (component.MainWeapon[i] == null)
						{
							bs._WorldGameObjectX.MainPlayer.GetComponent<AudioSource>().PlayOneShot(Igor.Instance.takeSound, ProfileINI.sound_volume * ProfileINI.sound_scale);
							component.MainWeapon[i] = gun;
							KGUI.SetNodes("select_weapon.grid_weapons." + KGUI.SlotIndex + ".selected", true, false);
							break;
						}
					}
				}
				else
				{
					component.MainWeapon[component.MainWeapon.ToList<Gun>().IndexOf(gun)] = null;
					KGUI.SetNodes("select_weapon.grid_weapons." + KGUI.SlotIndex + ".selected", false, false);
				}
				this.RefreshBattleWeapon(0);
			});
		});
		this.RegisterMenu(Menu.SelectPack, "select_hg_pack", delegate(object data, object dataEx)
		{
			if (WorldGameObjectX.Instance.MainPlayer == null)
			{
				this.HideMenu();
				return;
			}
			this.RefreshSelectBuildIcon();
			if ((bool)data && !WorldGameObjectX.Instance.MainPlayerDead)
			{
				WorldGameObjectX.Instance.KillPlayer();
			}
		}, delegate()
		{
			KGUI.SetButtonCallback("select_hg_pack.ibtn_fight", delegate()
			{
				this.StarHG();
			});
			KGUI.SetButtonCallback("select_hg_pack.ibtn_close", delegate()
			{
				this.StarHG();
			});
			KGUI.SetButtonCallback("select_hg_pack.ibtn_select", delegate()
			{
				this.SelectedBuildItem = WorldGameObjectX.Instance.GetPurchasePack()[KGUI.SlotIndex];
				this.RefreshSelectBuildIcon();
			});
		});
		this.RegisterMenu(Menu.SelectHSTeam, "select_hs_team", delegate(object data, object dataEx)
		{
			if (WorldGameObjectX.Instance.MainPlayer == null)
			{
				this.HideMenu();
				return;
			}
		}, delegate()
		{
			KGUI.SetButtonCallback("select_hs_team.button_hide.ibtn_select", delegate()
			{
				MainMenu.Instance.SwitchMenu(Menu.SelectHSItem, true, null);
			});
			KGUI.SetButtonCallback("select_hs_team.button_seek.ibtn_select", delegate()
			{
				UnityEngine.Debug.Log("select_hs_team.button_seek");
				TeamBattle.Instance.SpawnToGame();
				TeamBattle.Instance.StartPlay(2);
				this.HideMenu();
			});
		});
		this.RegisterMenu(Menu.SelectHSItem, "select_hs_item", delegate(object data, object dataEx)
		{
			if (WorldGameObjectX.Instance.MainPlayer == null)
			{
				this.HideMenu();
				return;
			}
		}, delegate()
		{
			KGUI.SetButtonCallback("select_hs_item.button_box.ibtn_select", delegate()
			{
				UnityEngine.Debug.Log("select_hs_item.button_box");
				HideSeek.hide_item_id = 1;
				TeamBattle.Instance.SpawnToGame();
				TeamBattle.Instance.StartPlay(1);
				this.HideMenu();
			});
			KGUI.SetButtonCallback("select_hs_item.button_item.ibtn_select", delegate()
			{
				UnityEngine.Debug.Log("select_hs_item.button_item");
				HideSeek.hide_item_id = 2;
				TeamBattle.Instance.SpawnToGame();
				TeamBattle.Instance.StartPlay(1);
				this.HideMenu();
			});
		});
		this.RegisterMenu(Menu.Profile, "Profile", delegate(object data, object dataEx)
		{
			KGUI.SetNodes("Profile.ibtn_edit", SceneManager.GetActiveScene().name == "Menu", false);
			string text = (string)data;
			if (text == null)
			{
				text = VKAPI.INSTANCE._viewerId;
			}
			if (text == string.Empty)
			{
				this.HideMenu();
				return;
			}
			this.StartCoroutine(this.ShowPlayerProfileProcess(text));
		}, delegate()
		{
			KGUI.SetButtonCallback("Profile.ibtn_edit", delegate()
			{
				if (this._CurMenu == Menu.Profile)
				{
					this.SetMenu(Menu.ChangeName, true, true, null);
				}
			});
			KGUI.SetTooltipCallback("Profile.ibtn_edit", () => Localize.GetText("ChangeNick", null));
			KGUI.SetTooltipCallback("Profile.Signs.Premium", () => Localize.GetText("key8000", null));
			KGUI.SetTooltipCallback("Profile.Signs.BadPlayer", () => Localize.GetText("key8001", null));
			KGUI.SetTooltipCallback("Profile.Signs.CommunityMember", () => Localize.GetText("key8002", null));
			KGUI.SetTooltipCallback("Profile.experience.cur_level", delegate
			{
				int num = int.Parse(KGUI.FindNode("Profile.experience.cur_level", false).GetComponentInChildren<UISprite>().spriteName.Substring(6));
				return Localize.GetText("LEVEL_" + num, null);
			});
			KGUI.SetTooltipCallback("Profile.experience.next_level", delegate
			{
				int num = int.Parse(KGUI.FindNode("Profile.experience.next_level", false).GetComponentInChildren<UISprite>().spriteName.Substring(6));
				return Localize.GetText("LEVEL_" + num, null);
			});
			KGUI.SetTooltipCallback("Profile.stat_1_1", () => Localize.GetText("PROFILE_STAT_1_1", null));
			KGUI.SetTooltipCallback("Profile.stat_1_2", () => Localize.GetText("PROFILE_STAT_1_2", null));
			KGUI.SetTooltipCallback("Profile.stat_1_3", () => Localize.GetText("PROFILE_STAT_1_3", null));
			KGUI.SetTooltipCallback("Profile.stat_1_4", () => Localize.GetText("PROFILE_STAT_1_4", null));
			KGUI.SetTooltipCallback("Profile.stat_2_1", () => Localize.GetText("PROFILE_STAT_2_1", null));
			KGUI.SetTooltipCallback("Profile.stat_2_2", () => Localize.GetText("PROFILE_STAT_2_2", null));
			KGUI.SetTooltipCallback("Profile.stat_2_3", () => Localize.GetText("PROFILE_STAT_2_3", null));
			KGUI.SetTooltipCallback("Profile.stat_2_4", () => Localize.GetText("PROFILE_STAT_2_4", null));
			KGUI.SetTooltipCallback("Profile.stat_3_1", () => Localize.GetText("PROFILE_STAT_3_1", null));
			KGUI.SetTooltipCallback("Profile.stat_3_2", () => Localize.GetText("PROFILE_STAT_3_2", null));
			KGUI.SetTooltipCallback("Profile.stat_3_3", () => Localize.GetText("PROFILE_STAT_3_3", null));
			KGUI.SetTooltipCallback("Profile.stat_4_1", () => Localize.GetText("PROFILE_STAT_4_1", null));
			KGUI.SetTooltipCallback("Profile.stat_4_2", () => Localize.GetText("PROFILE_STAT_4_2", null));
			KGUI.SetTooltipCallback("Profile.stat_4_3", () => Localize.GetText("PROFILE_STAT_4_3", null));
		});
		this.RegisterMenu(Menu.Plate, "plate", delegate(object data, object dataEx)
		{
			this._CurPlate = (Tablichka)data;
			string[] array = (string[])dataEx;
			KGUI.SetControlText("plate.inp_line1", array[0]);
			KGUI.SetControlText("plate.inp_line2", array[1]);
			KGUI.SetControlText("plate.inp_line3", array[2]);
			KGUI.SetControlText("plate.inp_line4", array[3]);
		}, delegate()
		{
			KGUI.SetButtonCallback("plate.ibtn_ok", delegate()
			{
				this._CurPlate.UpdateText(KGUI.GetControlText("plate.inp_line1"), KGUI.GetControlText("plate.inp_line2"), KGUI.GetControlText("plate.inp_line3"), KGUI.GetControlText("plate.inp_line4"));
				this.HideMenu();
			});
		});
		this.RegisterMenu(Menu.TeamBattle, "team_battle", delegate(object data, object dataEx)
		{
			KGUI.SetNodes("team_battle.button_back", false, false);
			if (TeamBattle.Instance is ZombieVirus)
			{
				KGUI.SetControlText("team_battle.button_red_team.txt_title", Localize.GetText("TEAM_BATTLE_FOR_ZOMBIES", null));
				KGUI.SetControlText("team_battle.button_blue_team.txt_title", Localize.GetText("TEAM_BATTLE_FOR_SURVIVORS", null));
				KGUI.SetControlText("team_battle.page1.txt_team", Localize.GetText("TEAM_BATTLE_ZOMBIES", null));
				KGUI.SetControlText("team_battle.page2.txt_team", Localize.GetText("TEAM_BATTLE_SURVIVORS", null));
			}
			else if (TeamBattle.Instance is HideSeek)
			{
				KGUI.SetControlText("team_battle.button_red_team.txt_title", Localize.GetText("SELECT_HIDE", null));
				KGUI.SetControlText("team_battle.button_blue_team.txt_title", Localize.GetText("SELECT_SEEK", null));
				KGUI.SetControlText("team_battle.page1.txt_team", Localize.GetText("SELECT_HIDE", null));
				KGUI.SetControlText("team_battle.page2.txt_team", Localize.GetText("SELECT_SEEK", null));
			}
			else
			{
				KGUI.SetControlText("team_battle.button_red_team.txt_title", Localize.GetText("TEAM_BATTLE_FOR_RED", null));
				KGUI.SetControlText("team_battle.button_blue_team.txt_title", Localize.GetText("TEAM_BATTLE_FOR_BLUE", null));
				KGUI.SetControlText("team_battle.page1.txt_team", Localize.GetText("TEAM_BATTLE_RED", null));
				KGUI.SetControlText("team_battle.page2.txt_team", Localize.GetText("TEAM_BATTLE_BLUE", null));
			}
			TeamBattle.Instance.RefreshTabMenu(false);
		}, delegate()
		{
			KGUI.SetButtonCallback("team_battle.ibtn_observe", delegate()
			{
				TeamBattle.Instance.StartPlay(0);
				this.HideMenu();
			});
			KGUI.SetButtonCallback("team_battle.ibtn_red_team", delegate()
			{
				if (TeamBattle.Instance is HideSeek)
				{
					HideSeek.SetRandomItem();
					TeamBattle.Instance.SpawnToGame();
					TeamBattle.Instance.StartPlay(1);
					this.HideMenu();
				}
				else
				{
					TeamBattle.Instance.StartPlay(1);
					if (!(TeamBattle.Instance is ZombieVirus))
					{
						this.SwitchMenu(Menu.SelectWeapon, false, null);
					}
				}
			});
			KGUI.SetButtonCallback("team_battle.ibtn_blue_team", delegate()
			{
				if (TeamBattle.Instance is HideSeek)
				{
					TeamBattle.Instance.SpawnToGame();
					TeamBattle.Instance.StartPlay(2);
					this.HideMenu();
				}
				else
				{
					TeamBattle.Instance.StartPlay(2);
					this.SwitchMenu(Menu.SelectWeapon, false, null);
				}
			});
			KGUI.SetButtonCallback("team_battle.ibtn_back", delegate()
			{
				this.HideMenu();
			});
			KGUI.SetButtonCallback("team_battle.ibtn_close", delegate()
			{
				this.HideMenu();
			});
			KGUI.SetButtonCallback("team_battle.grid_blue_team.ibtn_cheater", delegate()
			{
				TeamBattle.Instance.CheckPlayerCheating(2, KGUI.SlotIndex);
			});
			KGUI.SetButtonCallback("team_battle.grid_red_team.ibtn_cheater", delegate()
			{
				TeamBattle.Instance.CheckPlayerCheating(1, KGUI.SlotIndex);
			});
			KGUI.SetButtonCallback("team_battle.grid_blue_team.ibtn_voice", delegate()
			{
				PlayerNode playerNode = TeamBattle.Instance.GetPlayerNode(2, KGUI.SlotIndex);
				if (playerNode != null)
				{
					playerNode.Voice = !playerNode.Voice;
					this.RefreshVoiceButton(playerNode, KGUI.FindNode("team_battle.grid_blue_team." + KGUI.SlotIndex + ".ibtn_voice", false));
				}
			});
			KGUI.SetButtonCallback("team_battle.grid_red_team.ibtn_voice", delegate()
			{
				PlayerNode playerNode = TeamBattle.Instance.GetPlayerNode(1, KGUI.SlotIndex);
				if (playerNode != null)
				{
					playerNode.Voice = !playerNode.Voice;
					this.RefreshVoiceButton(playerNode, KGUI.FindNode("team_battle.grid_red_team." + KGUI.SlotIndex + ".ibtn_voice", false));
				}
			});
			KGUI.SetTooltipCallback("team_battle.grid_blue_team.level_icon", () => Localize.GetText("LEVEL_" + TeamBattle.Instance.GetPlayerLevel(2, KGUI.SlotIndex), null));
			KGUI.SetTooltipCallback("team_battle.grid_red_team.level_icon", () => Localize.GetText("LEVEL_" + TeamBattle.Instance.GetPlayerLevel(1, KGUI.SlotIndex), null));
			KGUI.SetButtonCallback("team_battle.grid_blue_team.profile", delegate()
			{
				this.ToggleMenu(Menu.Profile, TeamBattle.Instance.GetPlayerViewerID(2, KGUI.SlotIndex), null);
			});
			KGUI.SetButtonCallback("team_battle.grid_red_team.profile", delegate()
			{
				this.ToggleMenu(Menu.Profile, TeamBattle.Instance.GetPlayerViewerID(1, KGUI.SlotIndex), null);
			});
		});
		this.RegisterMenu(Menu.Deathmatch, "deathmatch", delegate(object data, object dataEx)
		{
			KGUI.SetNodes("deathmatch.button_close", false, false);
			KGUI.SetNodes("deathmatch.checkbox_chat", false, false);
			KGUI.SetNodes("deathmatch.txt_score_title", true, false);
			if (GameType.IsHungerGamesMode)
			{
				KGUI.SetControlText("deathmatch.txt_title", Localize.GetText(App.Instance.Settings.server_name.Split(new char[]
				{
					' '
				})[0], null) + "  ");
				if ((HG_WorkController.hgstatus != GameStatus.GS_WAIT && HG_WorkController.hgstatus != GameStatus.GS_PREPEA) || WorldGameObjectX.Instance.MainPlayer != null)
				{
					KGUI.SetNodes("deathmatch.button_play", false, false);
				}
				else
				{
					KGUI.SetNodes("deathmatch.button_play", true, false);
				}
			}
			else if (GameType.IsArcadeMode)
			{
				KGUI.SetControlText("deathmatch.txt_title", Localize.GetText(App.Instance.Settings.server_name.Split(new char[]
				{
					' '
				})[0], null) + "  ");
				KGUI.SetNodes("deathmatch.txt_score_title", false, false);
				KGUI.SetNodes("deathmatch.button_play", false, false);
			}
			TeamBattle.Instance.RefreshTabMenu(false);
		}, delegate()
		{
			KGUI.SetButtonCallback("deathmatch.ibtn_play", delegate()
			{
				if (!GameType.IsArcadeMode)
				{
					TeamBattle.Instance.StartPlay(1);
					this.SwitchMenu(Menu.SelectWeapon, false, null);
				}
			});
			KGUI.SetButtonCallback("deathmatch.ibtn_observe", delegate()
			{
				TeamBattle.Instance.StartPlay(0);
				this.HideMenu();
			});
			KGUI.SetButtonCallback("deathmatch.ibtn_close", delegate()
			{
				this.HideMenu();
			});
			KGUI.SetCheckboxCallback("deathmatch.ckb_chat", delegate
			{
			});
			KGUI.SetButtonCallback("deathmatch.grid_players.ibtn_cheater", delegate()
			{
				TeamBattle.Instance.CheckPlayerCheating(1, KGUI.SlotIndex);
			});
			KGUI.SetButtonCallback("deathmatch.grid_players.ibtn_voice", delegate()
			{
				PlayerNode playerNode = TeamBattle.Instance.GetPlayerNode(1, KGUI.SlotIndex);
				if (playerNode != null)
				{
					playerNode.Voice = !playerNode.Voice;
					this.RefreshVoiceButton(playerNode, KGUI.FindNode("deathmatch.grid_players." + KGUI.SlotIndex + ".ibtn_voice", false));
				}
			});
			KGUI.SetTooltipCallback("deathmatch.grid_players.level_icon", () => Localize.GetText("LEVEL_" + TeamBattle.Instance.GetPlayerLevel(1, KGUI.SlotIndex), null));
			KGUI.SetButtonCallback("deathmatch.grid_players.profile", delegate()
			{
				this.ToggleMenu(Menu.Profile, TeamBattle.Instance.GetPlayerViewerID(1, KGUI.SlotIndex), null);
			});
		});
		this.RegisterMenu(Menu.BattleResult, "battle_result", delegate(object data, object dataEx)
		{
			bool flag = (bool)data;
			int score = (int)dataEx;
			KGUI.SetNodes("battle_result.button_close", false, false);
			if (flag)
			{
				KGUI.SetNodes("battle_result.lose_screen", false, false);
			}
			else
			{
				KGUI.SetNodes("battle_result.win_screen", false, false);
				KGUI.SetNodes("battle_result.win_title", false, false);
			}
			KGUI.SetNodes("battle_result.exp_premium_disabled", false, false);
			KGUI.SetNodes("battle_result.war_points_premium_disabled", false, false);
			KGUI.SetNodes("battle_result.txt_exp", false, false);
			KGUI.SetNodes("battle_result.txt_war_points", false, false);
			KGUI.SetNodes("battle_result.exp_disabled", false, false);
			KGUI.SetNodes("battle_result.war_points_disabled", false, false);
			bool flag2 = ProfileINI.GetPurchaseValue(StorePurchase.MORE_EXPERIENCE) != 0;
			KGUI.SetNodes("battle_result.exp_premium_enabled", flag2, false);
			KGUI.SetNodes("battle_result.war_points_premium_enabled", flag2, false);
			if (!this.IsAllInclusive)
			{
				KGUI.SetNodes("battle_result.gold_premium_enabled", true, false);
			}
			else
			{
				KGUI.SetNodes("battle_result.war_points_premium_enabled", false, false);
				KGUI.SetNodes("battle_result.txt_war_points_premium", false, false);
				KGUI.SetNodes("battle_result.gold_premium_enabled", false, false);
			}
			KGUI.SetNodes("battle_result.war_points_enabled", false, false);
			if (GameType.IsArcadeMode)
			{
				string text = Localize.GetText("ARCADE_PLACE_TXT", null);
				if (flag)
				{
					text = text.Replace("%PLACE%", "1");
				}
				else
				{
					text = text.Replace("%PLACE%", Arcade.Place.ToString());
				}
				KGUI.SetControlText("battle_result.txt_arcade_stage", text);
			}
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
			Action<string, bool> action = delegate(string nodeName, bool normal)
			{
				Color color = new Color(0.3529412f, 0.3529412f, 0.274509817f);
				Color color2 = new Color(0.807843149f, 0.807843149f, 0.5647059f);
				Color white = Color.white;
				Color black = Color.black;
				UILabel component = KGUI.FindNode(nodeName, false).GetComponent<UILabel>();
				component.color = ((!normal) ? white : color);
				component.effectColor = ((!normal) ? black : color2);
			};
			action("battle_result.txt_exp_premium", flag2);
			action("battle_result.txt_war_points_premium", flag2);
			this.StartCoroutine(this.BattleResultProcess(score));
		}, delegate()
		{
			KGUI.SetButtonCallback("battle_result.ibtn_ok", delegate()
			{
				if (GameType.IsHungerGamesMode || GameType.IsArcadeMode || GameType.IsHideSeek)
				{
					WorldGameObjectX.Instance.CloseServerAndExit();
				}
				else
				{
					TeamBattle.Instance.RefreshTabMenu(true);
				}
			});
			KGUI.SetButtonCallback("battle_result.ibtn_close", delegate()
			{
				if (GameType.IsHungerGamesMode || GameType.IsArcadeMode || GameType.IsHideSeek)
				{
					WorldGameObjectX.Instance.CloseServerAndExit();
				}
				else
				{
					this.HideMenu();
				}
			});
		});
		this.RegisterMenu(Menu.LevelUp, "level_up", delegate(object data, object dataEx)
		{
			int level = ProfileINI.level;
			KGUI.SetControlText("level_up.txt_title", Localize.GetText("LEVEL_TITLE", null) + " " + level);
			KGUI.SetControlText("level_up.txt_level_name", Localize.GetText("LEVEL_" + level, null));
			KGUI.SetControlSprite(KGUI.FindNode("level_up.level_icon", false), "level_" + level, 0f);
			WorldGameObjectX.Instance.photonView.RPC("SetPlayerLevel", PhotonTargets.OthersBuffered, new object[]
			{
				PhotonNetwork.player,
				level
			});
		}, delegate()
		{
			KGUI.SetButtonCallback("level_up.ibtn_ok", delegate()
			{
				this.SetMenu(Menu.LevelUp, false, null, null);
			});
		});
		this.RegisterMenu(Menu.Ratings, "main_menu_ratings", delegate(object data, object dataEx)
		{
			this.StartCoroutine(this.ShowRatingsProcess(1));
		}, delegate()
		{
			KGUI.SetButtonCallback("main_menu_ratings.ibtn_icon_03", delegate()
			{
				this.StartCoroutine(this.ShowRatingsProcess(0));
			});
			KGUI.SetButtonCallback("main_menu_ratings.ibtn_icon_02", delegate()
			{
				this.StartCoroutine(this.ShowRatingsProcess(1));
			});
			KGUI.SetButtonCallback("main_menu_ratings.ibtn_icon_01", delegate()
			{
				this.StartCoroutine(this.ShowRatingsProcess(2));
			});
			KGUI.SetButtonCallback("main_menu_ratings.ibtn_icon_04", delegate()
			{
				this.StartCoroutine(this.ShowRatingsProcess(3));
			});
		});
	}

	public void SetSelectedMapSlot(int mapSlot)
	{
		this._SelectedMapSlot = mapSlot;
	}

	public void RegisterMenu(Menu menu, string nodeName, Action<object, object> initializer, Action callbacks)
	{
		if (!this._MenuNodeNames.ContainsKey(menu))
		{
			this._MenuNodeNames[menu] = new List<string>();
		}
		if (nodeName != null)
		{
			this._MenuNodeNames[menu].Add(nodeName);
		}
		if (!this._MenuInitializers.ContainsKey(menu))
		{
			this._MenuInitializers[menu] = new List<Action<object, object>>();
		}
		this._MenuInitializers[menu].Add(initializer);
		if (callbacks != null)
		{
			callbacks();
		}
	}

	public void RegisterMenu(Menu[] menus, string nodeName, Action<object, object> initializer, Action callbacks)
	{
		foreach (Menu menu in menus)
		{
			this.RegisterMenu(menu, nodeName, initializer, null);
		}
		callbacks();
	}

	public void SwitchMenu(Menu menu, object data = null, object dataEx = null)
	{
		if (KGUI.FindNode("loading", false).gameObject.activeSelf)
		{
			return;
		}
		if (menu == this._CurMenu)
		{
			return;
		}
		this.SetMenu(this._CurMenu, false, data, dataEx);
		this._CurMenu = menu;
		this.SetMenu(this._CurMenu, true, data, dataEx);
	}

	public void SwitchMenuSet(Menu menu, object data = null, object dataEx = null)
	{
		this.SetMenu(this._CurMenu, false, data, dataEx);
		this._CurMenu = menu;
		this.SetMenu(this._CurMenu, true, data, dataEx);
	}

	public void ToggleMenu(Menu menu, object data = null, object dataEx = null)
	{
		if (KGUI.FindNode("loading", false).gameObject.activeSelf)
		{
			return;
		}
		if (this._CurMenu != menu && this._CanToggleMenus.Contains(this._CurMenu))
		{
			this.SwitchMenu(menu, data, dataEx);
		}
		else if (this._CurMenu == menu)
		{
			this.HideMenu();
		}
	}

	public void SetMenu(Menu menu, bool show, object data = null, object dataEx = null)
	{
		UITooltip.ShowText(null);
		foreach (string nodeName in this._MenuNodeNames[menu])
		{
			KGUI.FindNode(nodeName, false).gameObject.SetActive(!show);
			KGUI.SetNodes(nodeName, show, false);
		}
		if (!this._NonModalMenus.Contains(menu))
		{
			KGUI.SetModal(this._MenuNodeNames[menu], show);
		}
		if (show)
		{
			foreach (Action<object, object> action in this._MenuInitializers[menu])
			{
				action(data, dataEx);
			}
			if (SceneManager.GetActiveScene().name == "Game")
			{
				if (this._CurMenu != Menu.None)
				{
					MainMenu.EnableMouseWork();
				}
				else
				{
					MainMenu.DisableMouseWork();
				}
			}
		}
	}

	public static void EnableMouseWork()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		WorldGameObjectX.Instance.DisableMouseLook();
		if (WorldGameObjectX.Instance.MainPlayer != null)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		}
	}

	public static void DisableMouseWork()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		WorldGameObjectX.Instance.EnableMouseLook();
		if (WorldGameObjectX.Instance.MainPlayer != null)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(true);
		}
	}

	public void HideMenu()
	{
		this.SwitchMenu(Menu.None, null, null);
	}

	public void ShowHint(string text, bool asNewMenu = false)
	{
		if (asNewMenu)
		{
			this.SwitchMenu(Menu.Hint, text, null);
		}
		else
		{
			this.SetMenu(Menu.Hint, true, text, null);
		}
		this.IsShowHint = true;
	}

	public void HideHint()
	{
		if (this.CurMenu == Menu.Hint)
		{
			this.HideMenu();
		}
		else
		{
			this.SetMenu(Menu.Hint, false, null, null);
		}
		this.IsShowHint = false;
		if (MainMenu.IsCheatOn)
		{
			MainMenu.IsCheatOn = false;
			WorldGameObjectX.Instance.ExitGame(string.Empty);
		}
	}

	public void ShowNotEnoughMoney(Currency currency, int cost)
	{
		this.SetMenu(Menu.NotEnoughMoney, true, new object[]
		{
			currency,
			cost
		}, null);
	}

	public void ShowLoading(string text, string rawText = "")
	{
		this.SetMenu(Menu.Loading, true, null, null);
		this.SetLoadingText(text, rawText);
	}

	public void HideLoading()
	{
		this.SetMenu(Menu.Loading, false, string.Empty, null);
	}

	public void SetLoadingText(string text, string rawText = "")
	{
		KGUI.SetControlText("loading.txt_text", Localize.GetText(text, null) + rawText);
	}

	public void ShowAskMenu(string title, string text, string[] buttonNames, Action[] buttonCallbacks, bool showCloseButton)
	{
		List<string> list = new List<string>();
		list.Add(title);
		list.Add(text);
		list.AddRange(buttonNames);
		list.Add((!showCloseButton) ? string.Empty : "X");
		this.SetMenu(Menu.AskMenu, true, list.ToArray(), buttonCallbacks);
	}

	public void ShowInputText(string title, string text, Action<string> okCallback, string defaultText = "")
	{
		this.SetMenu(Menu.InputText, true, new List<string>
		{
			Localize.GetText(title, null),
			Localize.GetText(text, null),
			defaultText
		}.ToArray(), okCallback);
	}

	public void ShowInputText2(string title, string text, Action<string> okCallback, string defaultText = "")
	{
		this.SetMenu(Menu.InputText2, true, new List<string>
		{
			Localize.GetText(title, null),
			Localize.GetText(text, null),
			defaultText
		}.ToArray(), okCallback);
	}

	private void Awake()
	{
		MainMenu.isShowAllInMsg = false;
		if (MainMenu.Instance != null)
		{
			MainMenu.Instance.SetMenu(Menu.Hud, false, null, null);
			return;
		}
		MainMenu.Instance = this;
		this.RegisterMenus();
		IS_Manager.Init();
		ItemsBuildManager.Init();
		this.ContentLoadingText = KGUI.FindNode(this.hud_tag + ".anchor_bottom.asset_loading", false);
		base.StartCoroutine(this.VoiceHighlightProcess());
	}

	public void HideLoadContentText()
	{
		KGUI.SetNodes(this.hud_tag + ".anchor_bottom.asset_loading", false, false);
	}

	private void OnLevelWasLoaded()
	{
		KGUI.SetNodes("main_menu_background", SceneManager.GetActiveScene().name == "Menu", false);
	}

	public void RefreshAcceleration()
	{
		bool flag = ProfileINI.GetPurchaseValue(StorePurchase.SPEED) != 0 && !GameType.BattleMode();
		bool flag2 = this._SpeedEnabled && flag && !GameType.BattleMode();
		if (WorldGameObjectX.Instance.MainPlayer != null)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetSpeedUp(flag2);
		}
		if (flag)
		{
			KGUI.SetNodes(this.hud_tag + ".bonuses.speed", true, false);
			KGUI.SetNodes(this.hud_tag + ".bonuses.speed.on", flag2, false);
			KGUI.SetNodes(this.hud_tag + ".bonuses.speed.off", !flag2, false);
		}
		else
		{
			KGUI.SetNodes(this.hud_tag + ".bonuses.speed", false, false);
		}
	}

	public void RefreshFlying()
	{
		bool flag = ProfileINI.GetPurchaseValue(StorePurchase.FLY) != 0 && !GameType.BattleMode();
		if (!flag)
		{
			flag = App.Instance.Settings.isWatch;
		}
		this.Flying = ((this._FlyEnabled && flag) || GameType.IsObserving());
		if (WorldGameObjectX.Instance.MainPlayer != null)
		{
			WorldGameObjectX.Instance.MainPlayer.SendMessage("SetLadderBody", this.Flying);
		}
		if (flag && !App.Instance.Settings.isWatch)
		{
			KGUI.SetNodes(this.hud_tag + ".bonuses.fly", true, false);
			KGUI.SetNodes(this.hud_tag + ".bonuses.fly.on", this.Flying, false);
			KGUI.SetNodes(this.hud_tag + ".bonuses.fly.off", !this.Flying, false);
		}
		else
		{
			KGUI.SetNodes(this.hud_tag + ".bonuses.fly", false, false);
		}
	}

	public void RefreshDayNight()
	{
		bool flag = false;
		if (flag)
		{
			KGUI.SetNodes(this.hud_tag + ".bonuses.day_night", true, false);
			TimeOfDay.RefreshBonusIcon();
		}
		else
		{
			KGUI.SetNodes(this.hud_tag + ".bonuses.day_night", false, false);
		}
	}

	private void UpdateSpecialBonus()
	{
		if (GameType.BattleMode() || Chat.IsEnabled() || this.MenuActive)
		{
			return;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1) && ProfileINI.GetPurchaseValue(StorePurchase.SPEED) != 0)
		{
			this._SpeedEnabled = !this._SpeedEnabled;
			this.RefreshAcceleration();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2) && (ProfileINI.GetPurchaseValue(StorePurchase.FLY) != 0 || App.Instance.Settings.isWatch))
		{
			this._FlyEnabled = !this._FlyEnabled;
			this.RefreshFlying();
		}
	}

	public void SetCrosshairInfo(string title, MainMenu.CrosshairAction crosshairAction)
	{
		KGUI.SetNodes(this.hud_tag + ".crosshair.actions", false, true);
		if ((crosshairAction & MainMenu.CrosshairAction.Take) != MainMenu.CrosshairAction.None)
		{
			if (!GameType.IsHungerGamesMode)
			{
				KGUI.SetNodes(this.hud_tag + ".crosshair.actions.take", true, false);
			}
			else if (string.IsNullOrEmpty(title))
			{
				KGUI.SetNodes(this.hud_tag + ".crosshair.actions.take", true, false);
			}
			else if (title != "PLATE" && !title.Contains("PAINTING"))
			{
				KGUI.SetNodes(this.hud_tag + ".crosshair.actions.take", true, false);
			}
		}
		if ((crosshairAction & MainMenu.CrosshairAction.Sit) != MainMenu.CrosshairAction.None && !GameType.IsHungerGamesMode)
		{
			KGUI.SetNodes(this.hud_tag + ".crosshair.actions.sit", true, false);
		}
		if ((crosshairAction & MainMenu.CrosshairAction.Open) != MainMenu.CrosshairAction.None)
		{
			KGUI.SetNodes(this.hud_tag + ".crosshair.actions.open", true, false);
		}
		if ((crosshairAction & MainMenu.CrosshairAction.Activate) != MainMenu.CrosshairAction.None && !GameType.IsHungerGamesMode)
		{
			KGUI.SetNodes(this.hud_tag + ".crosshair.actions.activate", true, false);
		}
		if ((crosshairAction & MainMenu.CrosshairAction.Write) != MainMenu.CrosshairAction.None && !GameType.IsHungerGamesMode)
		{
			KGUI.SetNodes(this.hud_tag + ".crosshair.actions.write", true, false);
		}
		if ((crosshairAction & MainMenu.CrosshairAction.Draw) != MainMenu.CrosshairAction.None && !GameType.IsHungerGamesMode)
		{
			KGUI.SetNodes(this.hud_tag + ".crosshair.actions.draw", true, false);
		}
		if ((crosshairAction & MainMenu.CrosshairAction.Wheel) != MainMenu.CrosshairAction.None && !GameType.IsHungerGamesMode)
		{
			KGUI.SetNodes(this.hud_tag + ".crosshair.actions.wheel", true, false);
		}
		if ((crosshairAction & MainMenu.CrosshairAction.Put) != MainMenu.CrosshairAction.None && !GameType.IsHungerGamesMode)
		{
			KGUI.SetNodes(this.hud_tag + ".crosshair.actions.put", true, false);
		}
		KGUI.FindNode(this.hud_tag + ".crosshair.actions", false).GetComponent<UITable>().Reposition();
		if (!string.IsNullOrEmpty(title) && !GameType.IsHungerGamesMode)
		{
			KGUI.SetNodes(this.hud_tag + ".crosshair.txt_title", true, false);
			if (title.Contains(HG_WorkController.SPPOINT_NAME))
			{
				title = HG_WorkController.SPPOINT_NAME;
			}
			KGUI.SetControlText(this.hud_tag + ".crosshair.txt_title", Localize.GetText("PURCHASE_" + title, null));
		}
		else
		{
			KGUI.SetNodes(this.hud_tag + ".crosshair.txt_title", false, false);
		}
	}

	public void AddWCToMenu()
	{
		if (GameType.IsHungerGamesMode)
		{
			if (!base.gameObject.GetComponent<HG_WorkController>())
			{
				this.HgWorkController = base.gameObject.AddComponent<HG_WorkController>();
				this.HgWorkController.Init(this);
			}
			else
			{
				this.HgWorkController = base.gameObject.GetComponent<HG_WorkController>();
				this.HgWorkController.Init(this);
			}
		}
		else if (GameType.IsArcadeMode)
		{
			if (!base.gameObject.GetComponent<HG_WorkController>())
			{
				this.HgWorkController = base.gameObject.AddComponent<HG_WorkController>();
				this.HgWorkController.Init();
			}
			else
			{
				this.HgWorkController = base.gameObject.GetComponent<HG_WorkController>();
				this.HgWorkController.Init();
			}
		}
	}

	private IEnumerator MoveToGameScene()
	{
		this.SetLoadingText("LOADING_LEVEL", string.Empty);
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}
		PhotonNetwork.isMessageQueueRunning = false;
		SceneManager.LoadScene("Game");
		yield break;
	}

	private void InitializeGameINI()
	{
		GameINI gameINI = App.Instance.Settings = new GameINI();
		gameINI.playerID = string.Empty;
		gameINI.slotID = 0;
		gameINI.publicMapID = this._SelectedMapID;
		gameINI.gameType = this._MapGameTypes[this._MapGameType];
		gameINI.destroyable = (this._MapGameTypes[this._MapGameType] == GameINI.GameType.BUILDING || this._MapGameTypes[this._MapGameType] == GameINI.GameType.ARCADE || this._MapDestroyable);
		gameINI.mapSize = GameINI.MapSize.SMALL;
		gameINI.mapTime = this._MapTimes[this._MapTime];
		gameINI.mapPopulation = 12;
		gameINI.isOnline = true;
		gameINI.isServer = false;
		gameINI.isServerAdministrator = false;
		gameINI.isWatch = false;
		gameINI.loadingSavedMap = true;
		gameINI.Password = string.Empty;
		PhotonNetwork.playerName = ProfileINI.nickname;
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("server_url", SettingsManager.ServerURL[0]);
		hashtable.Add("player_id", VKAPI.INSTANCE._viewerId);
		hashtable.Add("gametype", gameINI.gameType);
		hashtable.Add("password", this.InputPassword);
		if (Info.Instance.Location == "MyMaps" && gameINI.gameType == GameINI.GameType.ARCADE)
		{
			UnityEngine.Debug.Log("Error loading SGF");
			PhotonNetwork.Disconnect();
			return;
		}
		if (gameINI.gameType == GameINI.GameType.HUNGER_GAMES)
		{
			gameINI.mapPopulation = 8;
			gameINI.destroyable = false;
			gameINI.gameStatus = GameStatus.GS_WAIT;
			hashtable.Add("pcount", 2);
		}
		else if (gameINI.gameType == GameINI.GameType.ARCADE)
		{
			gameINI.mapPopulation = 8;
			gameINI.gameStatus = GameStatus.GS_WAIT;
		}
		else if (gameINI.gameType == GameINI.GameType.HIDE_SEEK)
		{
			gameINI.destroyable = true;
			gameINI.gameStatus = GameStatus.GS_WAIT;
			gameINI.mapPopulation = 8;
		}
		PhotonNetwork.SetPlayerCustomProperties(hashtable);
	}

	public void JoinGame(RoomInfo room)
	{
		UnityEngine.Debug.Log("JoinGame");
		this.InitializeGameINI();
		App.Instance.Settings.playerID = (string)room.customProperties["player_id"];
		App.Instance.Settings.slotID = (int)room.customProperties["slot_id"];
		App.Instance.Settings.isWatch = (bool)room.customProperties["is_watch"];
		App.Instance.Settings.server_name = (string)room.customProperties["map_name"];
		App.Instance.Settings.server_about = (string)room.customProperties["map_about"];
		PhotonNetwork.JoinRoom(room.name);
		this.HideMenu();
		this.ShowLoading("LOADING_TRY_CONNECT", string.Empty);
	}

	private void OnPhotonJoinRoomFailed()
	{
		UnityEngine.Debug.Log("OnPhotonJoinRoomFailed");
		if (SceneManager.GetActiveScene().name == "Menu")
		{
			MainMenu.Instance.HideLoading();
		}
		else
		{
			MainMenu.Instance.ShowLoading("LOADING_LEAVING", string.Empty);
			SceneManager.LoadScene("Menu");
		}
	}

	private void JoinOrCreateGame(string playerID, int slotID, bool isWatch, string serverName, string serverDesc, string password)
	{
		this.InitializeGameINI();
		using (MD5 md = MD5.Create())
		{
			if (password.Length > 0)
			{
				this.ProtectPassword = ProtectHash.GetHash(md, password);
			}
			else
			{
				this.ProtectPassword = string.Empty;
			}
		}
		App.Instance.Settings.server_name = serverName;
		App.Instance.Settings.server_about = serverDesc;
		App.Instance.Settings.Password = this.ProtectPassword;
		App.Instance.Settings.playerID = playerID;
		App.Instance.Settings.slotID = slotID;
		App.Instance.Settings.isWatch = isWatch;
		App.Instance.Settings.gameStatus = GameStatus.GS_WAIT;
		App.Instance.Settings.mapTime = ((slotID > 0) ? this._MapTimes[this._MapTime] : GameINI.MapTime.DAY);
		PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable
		{
			{
				"player_id",
				playerID
			},
			{
				"slot_id",
				slotID
			},
			{
				"game_status",
				GameStatus.GS_WAIT
			},
			{
				"password",
				string.Empty
			}
		}, 0);
		this.HideMenu();
		this.ShowLoading("LOADING_TRY_CONNECT", string.Empty);
	}

	private void OnPhotonRandomJoinFailed()
	{
		UnityEngine.Debug.Log("OnPhotonRandomJoinFailed");
		App.Instance.Settings.isServer = true;
		App.Instance.Settings.isServerAdministrator = (App.Instance.Settings.slotID != -1);
		SceneManager.LoadScene("Game");
		this.ShowLoading("LOADING_LEVEL", string.Empty);
	}

	private void OnJoinedRoom()
	{
		UnityEngine.Debug.Log("OnJoinedRoom");
		if ((string)PhotonNetwork.room.customProperties["player_id"] == VKAPI.INSTANCE._viewerId)
		{
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("map_name", App.Instance.Settings.server_name);
			hashtable.Add("map_about", App.Instance.Settings.server_about);
			hashtable.Add("password", App.Instance.Settings.Password);
			hashtable.Add("game_type", App.Instance.Settings.gameType);
			PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
		}
		if (!App.Instance.Settings.isServer)
		{
			ProfileINI.server_about = (string)PhotonNetwork.room.customProperties["map_about"];
			base.StartCoroutine(this.MoveToGameScene());
		}
	}

	public void CleanScreenMode(bool mode)
	{
		if (GameType.IsObserving())
		{
			return;
		}
		MainMenu.CleanScreen = mode;
		if (MainMenu.CleanScreen)
		{
			if (!CameraController.Instance.IsThirdPerson)
			{
				WorldGameObjectX.Instance.MainPlayer.GetComponent<SkinManager>().DisableAllHands();
			}
			Nickname.HideAll = true;
		}
		else
		{
			if (!CameraController.Instance.IsThirdPerson)
			{
				WorldGameObjectX.Instance.MainPlayer.GetComponent<SkinManager>().SetHand(ProfileINI.GetActualSkin());
			}
			Nickname.HideAll = false;
		}
		KGUI.FindNode("hud", false).gameObject.SetActive(!MainMenu.CleanScreen);
	}

	private void Update()
	{
		if (ContentUpdater.NumberContent >= 0 && ContentUpdater.NumberContent <= 4)
		{
			if (ContentUpdater.NumberContent == 0)
			{
				this.ContentLoadingText.GetComponent<UILabel>().text = "Загрузка лучших текстур ландшафта " + ContentUpdater.UpdaterProgress + "%";
			}
			if (ContentUpdater.NumberContent == 1)
			{
				this.ContentLoadingText.GetComponent<UILabel>().text = "Загрузка лучших текстур декораций " + ContentUpdater.UpdaterProgress + "%";
			}
			if (ContentUpdater.NumberContent == 2)
			{
				this.ContentLoadingText.GetComponent<UILabel>().text = "Загрузка лучших текстур персонажей " + ContentUpdater.UpdaterProgress + "%";
			}
			if (ContentUpdater.NumberContent == 3)
			{
				this.ContentLoadingText.GetComponent<UILabel>().text = "Загрузка лучших текстур оружия " + ContentUpdater.UpdaterProgress + "%";
			}
			if (ContentUpdater.NumberContent == 4)
			{
				this.ContentLoadingText.GetComponent<UILabel>().text = string.Empty;
			}
		}
		if (App.Instance.CurPlatform != App.Platform.STEAM && UnityEngine.Input.GetKeyDown(KeyCode.F12))
		{
			ProfileINI.ToggleFullScreen();
		}
		if (WorldGameObjectX.Instance != null && UnityEngine.Input.GetKeyDown(KeyCode.R) && !Chat.IsEnabled() && !MainMenu.Instance.MenuActive && !GameType.BattleMode())
		{
			WorldGameObjectX.Instance.Respawn();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.F9))
		{
			this.CinematicCamera = !this.CinematicCamera;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.F11) && SceneManager.GetActiveScene().name == "Game")
		{
			MainMenu.CleanScreen = !MainMenu.CleanScreen;
			this.CleanScreenMode(MainMenu.CleanScreen);
			if (!GameType.BattleMode())
			{
				this.CheckSelectBlock();
			}
		}
		if (SceneManager.GetActiveScene().name == "Game")
		{
			this.UpdateSpecialBonus();
		}
		if (this._BonusGemsSeconds != -1 && (this.CurMenu == Menu.Start || this.CurMenu == Menu.MyMaps || this.CurMenu == Menu.Settings || this.CurMenu == Menu.Shop || this.CurMenu == Menu.Servers || this.CurMenu == Menu.TopMaps || this.CurMenu == Menu.Bank))
		{
			TimeSpan timeSpan = this._BonusGemsEndTime - DateTime.Now;
			if (timeSpan.TotalSeconds > 0.0 && this._BonusGemsSeconds != (int)timeSpan.TotalSeconds)
			{
				KGUI.SetControlText("main_menu.bonus_gems.txt_time", timeSpan.Hours + ((!this._BonusGemsShowSeparator) ? " " : ":") + timeSpan.Minutes);
				this._BonusGemsSeconds = (int)timeSpan.TotalSeconds;
				this._BonusGemsShowSeparator = !this._BonusGemsShowSeparator;
			}
			else if (timeSpan.TotalSeconds <= 0.0)
			{
				this._BonusGemsSeconds = -1;
				this.HideMenu();
				this.SwitchMenu(Menu.Bank, null, null);
			}
		}
	}

	private void CheckSelectBlock()
	{
		KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_01_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Fence, false);
		KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_02_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Half, false);
		KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_03_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Quarter, false);
		KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_04_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Diagonal, false);
		KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_05_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Stair, false);
		KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_07_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Corner, false);
		KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_08_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.StairCorner, false);
		KGUI.SetNodes(this.hud_tag + ".custom_blocks.icon_06_selected", WorldGameObjectX.Instance.CurrentCommonKind == CommonBlockKind.Default, false);
	}

	public void OpenCloseInventory()
	{
		if (!Chat.IsEnabled() && HG_WorkController.CanOpenInventory())
		{
			if (MainMenu.isInventoryMenuOpen != null && this._CurMenu == Menu.None)
			{
				this._CurMenu = Menu.InventarySystem;
				MainMenu.isInventoryMenuOpen();
			}
			else if (MainMenu.isInventoryMenuClose != null && this._CurMenu == Menu.InventarySystem)
			{
				MainMenu.isInventoryMenuClose();
			}
		}
	}

	private IEnumerator FastConnectToGame(GameINI.GameType gameType)
	{
		yield return 0;
		RoomInfo[] allBattlesRoomList = PhotonNetwork.GetRoomList();
		List<RoomInfo> battlesRoomList = new List<RoomInfo>();
		foreach (RoomInfo room in allBattlesRoomList)
		{
			if (room.customProperties.ContainsKey("game_type") && (int)room.customProperties["game_type"] == (int)gameType)
			{
				battlesRoomList.Add(room);
			}
		}
		if (battlesRoomList.Count == 0)
		{
			this.ShowHint("Нет созданных карт, создайте свою нажав на 'Мои карты'", false);
			yield break;
		}
		System.Random r = new System.Random();
		for (int i = 0; i < battlesRoomList.Count; i++)
		{
			int j = r.Next(0, battlesRoomList.Count);
			RoomInfo room2 = battlesRoomList[j];
			if (((string)room2.customProperties["password"]).Length == 0 && room2.playerCount < (int)room2.maxPlayers)
			{
				this.JoinGame(room2);
				yield break;
			}
		}
		foreach (RoomInfo room3 in battlesRoomList)
		{
			if (((string)room3.customProperties["password"]).Length == 0 && room3.playerCount < (int)room3.maxPlayers)
			{
				this.JoinGame(room3);
				yield break;
			}
		}
		this.ShowHint("Все сервера полные или под паролем, создайте свой нажав на 'Мои карты'", false);
		yield break;
	}

	private IEnumerator GetFavoritServers()
	{
		this._FavoritServers = null;
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", VKAPI.INSTANCE._viewerId);
		System.Random r = new System.Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(SettingsManager.ServerURL[0] + "get_favorit_servers.php", form);
		yield return php_load;
		if (php_load.error != null)
		{
			UnityEngine.Debug.LogError(php_load.error);
			yield break;
		}
		string[] param = php_load.text.Split(new char[]
		{
			'\t'
		});
		List<MainMenu.FavoritServer> favoritServers = new List<MainMenu.FavoritServer>();
		for (int i = 1; i < param.Length; i += 3)
		{
			favoritServers.Add(new MainMenu.FavoritServer
			{
				server_name = param[i],
				admin_id = int.Parse(param[i + 1]),
				slot_id = byte.Parse(param[i + 2])
			});
		}
		this._FavoritServers = favoritServers.ToArray();
		yield break;
	}

	public void PurchaseUse(StorePurchase purchase, bool notifyPhotonServer)
	{
		int purchaseValue = ProfileINI.GetPurchaseValue(purchase);
		if (purchaseValue > 0)
		{
			ProfileINI.SetPurchaseValue(purchase, purchaseValue - 1);
		}
		float num = Store.Purchases[purchase].Cooldown;
		if (num > 0f)
		{
			ProfileINI.SetPurchaseCooldown(purchase, num);
		}
		if (WorldGameObjectX.Instance != null && notifyPhotonServer)
		{
			WorldGameObjectX.Instance.photonView.RPC("UsePurchase", PhotonTargets.AllViaServer, new object[]
			{
				(int)purchase
			});
		}
		base.StartCoroutine(this.PurchaseUseProcess(purchase));
	}

	public IEnumerator PurchaseUseProcess(StorePurchase purchase)
	{
		UnityEngine.Debug.Log("purchase: " + purchase);
		WWWForm purchaseUseForm = new WWWForm();
		purchaseUseForm.AddField("viewer_id", VKAPI.INSTANCE._viewerId);
		purchaseUseForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		purchaseUseForm.AddField("p_ind", (int)purchase);
		System.Random r = new System.Random();
		purchaseUseForm.AddField("random", r.Next());
		WWW purchaseUse = new WWW(SettingsManager.ServerURL[0] + "purchase_use.php", purchaseUseForm);
		yield return purchaseUse;
		if (purchaseUse.error != null)
		{
			UnityEngine.Debug.Log("Purchase use error: " + purchaseUse.error);
		}
		else if (purchaseUse.text != "OK")
		{
			UnityEngine.Debug.Log("Purchase use fail: " + purchaseUse.text);
		}
		yield break;
	}

	public void BuyPurchase(StorePurchase purchase, int dateIndex = 0)
	{
		base.StartCoroutine(this.PurchaseBuy(purchase, dateIndex));
	}

	private IEnumerator PurchaseBuy(StorePurchase purchase, int dateIndex = 0)
	{
		string SceneName = SceneManager.GetActiveScene().name;
		if (!ProfileINI.CheckPurchaseValue(purchase))
		{
			yield break;
		}
		StorePurchase requiredPurchase = Store.Purchases[purchase].RequiredPurchase;
		if (requiredPurchase != StorePurchase.NONE && ProfileINI.GetPurchaseValue(requiredPurchase) <= 0)
		{
			this.ShowHint(Localize.GetText("SHOP_LOCKED", null) + " '" + Localize.GetText("PURCHASE_" + Store.Purchases[requiredPurchase].Name, null) + "'", false);
			yield break;
		}
		Store.Pay pay = Store.Purchases[purchase].Cost;
		int count = 0;
		int payCost = 0;
		Currency payCurrency;
		if (pay is Store.OnePay)
		{
			Store.OnePay onePay = (Store.OnePay)pay;
			payCurrency = onePay.Curr;
			payCost = onePay.Cost;
			count = onePay.Count;
		}
		else
		{
			Store.TimedPay timedPay = (Store.TimedPay)pay;
			dateIndex = Mathf.Clamp(dateIndex, 1, 4);
			payCurrency = timedPay.Curr;
			if (dateIndex == 1)
			{
				payCost = timedPay.DayCost;
			}
			else if (dateIndex == 2)
			{
				payCost = timedPay.WeekCost;
			}
			else if (dateIndex == 3)
			{
				payCost = timedPay.ForeverCost;
			}
			else
			{
				payCost = timedPay.MonthCost;
			}
			count = dateIndex;
		}
		if (ProfileINI.money[(int)payCurrency] < payCost && !ProfileINI.all_inclusive)
		{
			this.ShowNotEnoughMoney(payCurrency, payCost);
		}
		else
		{
			WWWForm purchaseBuyForm = new WWWForm();
			purchaseBuyForm.AddField("viewer_id", VKAPI.INSTANCE._viewerId);
			purchaseBuyForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
			purchaseBuyForm.AddField("p_ind", (int)purchase);
			purchaseBuyForm.AddField("count", count);
			purchaseBuyForm.AddField("pay_currency", payCurrency.ToString());
			purchaseBuyForm.AddField("pay_cost", payCost);
			int packetID = App.Instance.Rnd.Next();
			purchaseBuyForm.AddField("random", packetID);
			string yobaKey = App.Instance.GenerateKey(20);
			purchaseBuyForm.AddField("yoba_key", App.Instance.KeyEncrypt(yobaKey));
			WWWForm wwwform = purchaseBuyForm;
			string fieldName = "beta_key";
			string[] array = new string[5];
			int num = 0;
			int num2 = (int)purchase;
			array[num] = num2.ToString();
			array[1] = "_";
			array[2] = packetID.ToString();
			array[3] = "_";
			array[4] = VKAPI.INSTANCE.GetSecret();
			wwwform.AddField(fieldName, VKAPI.MD52(string.Concat(array)));
			this.ShowLoading("SHOP_BUY_IN_PROGRESS", string.Empty);
			string result = null;
			if (WorldGameObjectX.Instance == null || App.Instance.CurPlatform != App.Platform.VK)
			{
				WWW purchaseBuy = new WWW(SettingsManager.ServerURL[0] + "PurchaseBuy.php", purchaseBuyForm);
				yield return purchaseBuy;
				if (purchaseBuy.error != null)
				{
					result = "error" + purchaseBuy.error;
				}
				else
				{
					result = purchaseBuy.text;
				}
			}
			else
			{
				string shopRequest = Encoding.UTF8.GetString(purchaseBuyForm.data);
				yield return base.StartCoroutine(WorldGameObjectX.Instance.PhotonPurchaseBuy(shopRequest, delegate(string r)
				{
					result = r;
				}));
			}
			this.HideLoading();
			if (!result.StartsWith("error"))
			{
				if (result.StartsWith("SUCCESS"))
				{
					string serverText = result.Substring(0, result.IndexOf('\n'));
					string serverKey = result.Substring(result.IndexOf('\n') + 1);
					if (serverKey.Trim() == VKAPI.MD52(string.Concat(new string[]
					{
						serverText,
						"_",
						packetID.ToString(),
						"_",
						yobaKey
					})))
					{
						yobaKey = string.Empty;
						if (serverText.Trim() == "SUCCESS")
						{
							if (!this.IsAllInclusive)
							{
								ProfileINI.money[(int)payCurrency] -= payCost;
								Bar.Instance.SetCoins();
							}
							if (pay is Store.OnePay)
							{
								if (!((Store.OnePay)pay).Once)
								{
									ProfileINI.SetPurchaseValue(purchase, ProfileINI.GetPurchaseValue(purchase) + count);
								}
								else
								{
									ProfileINI.SetPurchaseValue(purchase, count);
								}
							}
							else
							{
								UnityEngine.Debug.Log(purchase);
								UnityEngine.Debug.Log(count);
								ProfileINI.SetPurchaseValue(purchase, count);
								if (count == 1)
								{
									ProfileINI.SetPurchaseTime(purchase, ProfileINI.server_time.AddDays(1.0));
								}
								else if (count == 2)
								{
									ProfileINI.SetPurchaseTime(purchase, ProfileINI.server_time.AddDays(7.0));
								}
								else if (count == 3)
								{
									ProfileINI.SetPurchaseTime(purchase, new DateTime(9999, 12, 30));
								}
								else if (count == 4)
								{
									ProfileINI.SetPurchaseTime(purchase, ProfileINI.server_time.AddMonths(1));
								}
							}
							if (SceneName == "Menu")
							{
								if (purchase == StorePurchase.SLOTS)
								{
									KGUI.ResizeGrid("MyMaps.my_maps_grid", 1 + ProfileINI.GetPurchaseValue(StorePurchase.SLOTS), delegate(GameObject slot, int index)
									{
										slot.transform.Find("txt_title").GetComponent<UILabel>().text = ProfileINI.GetSlotMapName(index + 1);
									}, "MyMaps");
								}
							}
							else if (SceneName == "Game")
							{
								if (purchase == StorePurchase.SPEED)
								{
									this.RefreshAcceleration();
								}
								else if (purchase == StorePurchase.FLY)
								{
									this.RefreshFlying();
								}
								if (purchase == StorePurchase.BLOCK_KIND_FENCE || purchase == StorePurchase.BLOCK_KIND_HALF || purchase == StorePurchase.BLOCK_KIND_DIAGONAL || purchase == StorePurchase.BLOCK_KIND_STAIR || purchase == StorePurchase.BLOCK_KIND_QUARTER || purchase == StorePurchase.BLOCK_KIND_CORNER || purchase == StorePurchase.BLOCK_KIND_STAIR_CORNER)
								{
									MainMenu.Instance.SetMenu(Menu.Hud, true, null, null);
								}
							}
							if (this._CurPurchase == purchase)
							{
								this.ShowPurchaseItem(purchase);
							}
							SoundManager.Instance.Play(SoundManager.Sound.ShopBuy, this.Audio.GetComponent<AudioSource>());
							if (WorldGameObjectX.Instance != null && App.Instance.CurPlatform != App.Platform.VK)
							{
								int value = ProfileINI.GetPurchaseValue(purchase);
								string time = ProfileINI.GetPurchaseTime(purchase).ToString();
								WorldGameObjectX.Instance.photonView.RPC("SetPurchase", PhotonTargets.AllViaServer, new object[]
								{
									(int)purchase,
									value,
									time
								});
							}
							this.RefreshShop();
							Offers.Instance.SetOffers();
						}
						else
						{
							this.ShowHint("Bad request " + serverText, false);
						}
					}
					else
					{
						this.ShowHint("Security error", false);
					}
				}
				else
				{
					this.ShowHint(result, false);
				}
			}
			else
			{
				this.ShowHint(result.Substring("error".Length), false);
			}
		}
		yield break;
	}

	public void ShowPurchaseItem(StorePurchase purchase)
	{
		this._CurPurchase = purchase;
		Store.PurchaseInfo purchaseInfo = Store.Purchases[purchase];
		string text = Localize.GetText("PURCHASE_" + purchaseInfo.Name, null);
		string text2 = Localize.GetText("PURCHASE_DESC_" + purchaseInfo.Name, string.Empty);
		Store.Pay cost = purchaseInfo.Cost;
		bool flag = this._ShopPurchaseItemInfo && text2.Length > 0;
		KGUI.SetNodes("Shop.page2_shop", false, false);
		KGUI.SetNodes("Shop.page2_shop", true, false);
		KGUI.SetNodes("Shop.page2_shop.ibtn_item_upgrade", false, false);
		KGUI.SetControlText("Shop.page2_shop.txt_item_name", text);
		KGUI.SetControlText("Shop.page2_shop.txt_info", string.Empty);
		KGUI.SetNodes("Shop.page2_shop.Sale25", false, false);
		KGUI.SetNodes("Shop.page2_shop.Sale50", false, false);
		KGUI.SetNodes("Shop.page2_shop.New", false, false);
		if (!flag)
		{
			KGUI.SetNodes("Shop.page2_shop.ibtn_item_back", false, false);
			KGUI.SetNodes("Shop.page2_shop.txt_info_ext", false, false);
			if (purchaseInfo.Sales == 0)
			{
				KGUI.SetNodes("Shop.page2_shop.Sale25", false, false);
			}
			else if (purchaseInfo.Sales == 25)
			{
				KGUI.SetNodes("Shop.page2_shop.Sale25", true, false);
			}
			else if (purchaseInfo.Sales == 50)
			{
				KGUI.SetNodes("Shop.page2_shop.Sale50", true, false);
			}
			if (purchaseInfo.New != 0)
			{
				KGUI.SetNodes("Shop.page2_shop.New", true, false);
			}
			else
			{
				KGUI.SetNodes("Shop.page2_shop.New", false, false);
			}
			if (purchaseInfo.LargeIcon != null)
			{
				KGUI.SetControlSprite(KGUI.FindNode("Shop.page2_shop.item_preview", false), purchaseInfo.LargeIcon, 150f);
				KGUI.SetNodes("Shop.page2_shop.item_preview_shadow", false, false);
			}
			else
			{
				KGUI.SetControlSprite(KGUI.FindNode("Shop.page2_shop.item_preview", false), purchaseInfo.Icon, 120f);
			}
			if (text2.Length == 0)
			{
				KGUI.SetNodes("Shop.page2_shop.ibtn_item_info", false, false);
			}
		}
		else
		{
			KGUI.SetNodes("Shop.page2_shop.ibtn_item_info", false, false);
			KGUI.SetNodes("Shop.page2_shop.item_preview", false, false);
			KGUI.SetNodes("Shop.page2_shop.item_preview_shadow", false, false);
			KGUI.SetControlText("Shop.page2_shop.txt_info_ext", text2);
		}
		bool flag2 = false;
		List<EntityType> list;
		List<BlockType> list2;
		mdl_Item_Build mdl_Item_Build;
		this.GetPurchaseContent(out list, out list2, out flag2, out mdl_Item_Build);
		if (list.Count + list2.Count <= 1 && !flag2)
		{
			KGUI.SetNodes("Shop.page2_shop.ibtn_item_look", false, false);
		}
		if (flag2)
		{
			StorePurchase storePurchase = Store.Purchases[purchase].RequiredPurchase;
			if (storePurchase != StorePurchase.NONE && ProfileINI.GetPurchaseValue(storePurchase) <= 0)
			{
				KGUI.SetNodes("Shop.page2_shop.ibtn_item_look", false, false);
			}
		}
		Currency currency;
		if (!this.IsAllInclusive)
		{
			if (cost is Store.TimedPay)
			{
				Store.TimedPay timedPay = (Store.TimedPay)cost;
				currency = timedPay.Curr;
				KGUI.SetNodes("Shop.page2_shop.buy_one", false, false);
				if (ProfileINI.GetPurchaseValue(purchase) > 0 && ProfileINI.GetPurchaseTime(purchase).Year > 2000)
				{
					if (ProfileINI.GetPurchaseTime(purchase).Year == 9999)
					{
						KGUI.SetControlText("Shop.page2_shop.txt_info", Localize.GetText("SHOP_FOREVER_BUY", null));
					}
					else
					{
						KGUI.SetControlText("Shop.page2_shop.txt_info", Localize.GetText("SHOP_ACTIVE_UNTIL", null) + ProfileINI.GetPurchaseTime(purchase));
					}
					KGUI.SetNodes("Shop.page2_shop.buy_day", false, false);
					KGUI.SetNodes("Shop.page2_shop.buy_week", false, false);
					KGUI.SetNodes("Shop.page2_shop.buy_forever", false, false);
					KGUI.SetNodes("Shop.page2_shop.buy_month", false, false);
				}
				else
				{
					if (timedPay.DayCost > 0)
					{
						this.SetMoneyValue("Shop.page2_shop.txt_buy_day_price", timedPay.Curr, string.Empty + timedPay.DayCost);
						KGUI.SetNodes("Shop.page2_shop.buy_day.icon_gold", timedPay.Curr == Currency.Gold, false);
					}
					else
					{
						KGUI.SetNodes("Shop.page2_shop.buy_day", false, false);
					}
					if (timedPay.WeekCost > 0)
					{
						this.SetMoneyValue("Shop.page2_shop.txt_buy_week_price", timedPay.Curr, string.Empty + timedPay.WeekCost);
						KGUI.SetNodes("Shop.page2_shop.buy_week.icon_gold", timedPay.Curr == Currency.Gold, false);
					}
					else
					{
						KGUI.SetNodes("Shop.page2_shop.buy_week", false, false);
					}
					if (timedPay.ForeverCost > 0)
					{
						this.SetMoneyValue("Shop.page2_shop.txt_buy_forever_price", timedPay.Curr, string.Empty + timedPay.ForeverCost);
						KGUI.SetNodes("Shop.page2_shop.buy_forever.icon_gold", timedPay.Curr == Currency.Gold, false);
					}
					else
					{
						KGUI.SetNodes("Shop.page2_shop.buy_forever", false, false);
					}
					if (timedPay.MonthCost > 0)
					{
						this.SetMoneyValue("Shop.page2_shop.txt_buy_month_price", timedPay.Curr, string.Empty + timedPay.MonthCost);
						KGUI.SetNodes("Shop.page2_shop.buy_month.icon_gold", timedPay.Curr == Currency.Gold, false);
					}
					else
					{
						KGUI.SetNodes("Shop.page2_shop.buy_month", false, false);
					}
				}
			}
			else
			{
				Store.OnePay onePay = (Store.OnePay)cost;
				currency = onePay.Curr;
				KGUI.SetNodes("Shop.page2_shop.buy_day", false, false);
				KGUI.SetNodes("Shop.page2_shop.buy_week", false, false);
				KGUI.SetNodes("Shop.page2_shop.buy_forever", false, false);
				KGUI.SetNodes("Shop.page2_shop.buy_month", false, false);
				KGUI.SetNodes("Shop.page2_shop.buy_one.icon_gold", onePay.Curr == Currency.Gold, false);
				if (onePay.Once)
				{
					if (ProfileINI.GetPurchaseValue(purchase) != 0)
					{
						KGUI.SetNodes("Shop.page2_shop.buy_one", false, false);
						KGUI.SetControlText("Shop.page2_shop.txt_info", Localize.GetText("SHOP_BOUGHT", null));
					}
					else
					{
						this.SetMoneyValue("Shop.page2_shop.buy_one.txt_price", onePay.Curr, string.Empty + onePay.Cost);
					}
				}
				else
				{
					KGUI.SetControlText("Shop.page2_shop.txt_info", Localize.GetText("SHOP_YOU_HAVE", null) + " " + ProfileINI.GetPurchaseValue(purchase));
					this.SetMoneyValue("Shop.page2_shop.buy_one.txt_price", onePay.Curr, string.Empty + onePay.Cost);
				}
			}
		}
		else
		{
			currency = cost.Curr;
			KGUI.SetNodes("Shop.page2_shop.buy_day", false, false);
			KGUI.SetNodes("Shop.page2_shop.buy_week", false, false);
			KGUI.SetNodes("Shop.page2_shop.buy_forever", false, false);
			KGUI.SetNodes("Shop.page2_shop.buy_month", false, false);
			KGUI.SetNodes("Shop.page2_shop.buy_one.icon_gold", false, false);
			KGUI.SetNodes("Shop.page2_shop.buy_one.txt_price", false, false);
			if (cost is Store.OnePay)
			{
				Store.OnePay onePay2 = (Store.OnePay)cost;
				if (onePay2.Once)
				{
					if (ProfileINI.GetPurchaseValue(purchase) != 0)
					{
						KGUI.SetNodes("Shop.page2_shop.buy_one", false, false);
						KGUI.SetControlText("Shop.page2_shop.txt_info", (!this.IsAllInclusive) ? Localize.GetText("SHOP_BOUGHT", null) : Localize.GetText("SHOP_GET", null));
					}
					else
					{
						KGUI.SetNodes("Shop.page2_shop.buy_one.txt_price", false, false);
					}
				}
				else
				{
					KGUI.SetControlText("Shop.page2_shop.txt_info", Localize.GetText("SHOP_YOU_HAVE", null) + " " + ProfileINI.GetPurchaseValue(purchase));
				}
			}
			else if (cost is Store.TimedPay)
			{
				KGUI.SetNodes("Shop.page2_shop.buy_one", false, false);
				KGUI.SetControlText("Shop.page2_shop.txt_info", (!this.IsAllInclusive) ? Localize.GetText("SHOP_BOUGHT", null) : Localize.GetText("SHOP_GET", null));
			}
		}
		KGUI.SetNodes("main_menu.money", !this.IsAllInclusive, false);
		KGUI.SetNodes("main_menu.money.gold", currency == Currency.Gold, false);
		this.SetMoneyValue("main_menu.money.txt_count", currency, null);
		if (Store.Purchases[purchase].RequiredPurchase != StorePurchase.NONE && ProfileINI.GetPurchaseValue(Store.Purchases[purchase].RequiredPurchase) <= 0)
		{
			KGUI.SetControlText("Shop.page2_shop.buy_one.txt_title", Localize.GetText("SHOP_BUY_LOCK", null));
		}
		else
		{
			KGUI.SetControlText("Shop.page2_shop.buy_one.txt_title", (!this.IsAllInclusive) ? Localize.GetText("SHOP_BUY_ONE", null) : Localize.GetText("GET_ITEM_FREE", null));
		}
		KGUI.SetNodes("Shop.page2_shop.use", false, false);
		if (Store.Purchases[purchase].Tab == Store.TabType.Skins)
		{
			bool flag3 = ProfileINI.skin == Store.Purchases[purchase].Skin;
			if (ProfileINI.GetPurchaseValue(purchase) == 0 || flag3)
			{
				KGUI.SetNodes("Shop.page2_shop.equip", false, false);
			}
			if (flag3)
			{
				KGUI.SetControlText("Shop.page2_shop.txt_info", Localize.GetText("SHOP_EQUIPPED", null));
			}
		}
		else
		{
			KGUI.SetNodes("Shop.page2_shop.equip", false, false);
		}
		if (Store.Purchases[purchase].Tab == Store.TabType.Weapon_Skin)
		{
			KGUI.SetNodes("Shop.page2_shop.equip_wskin", false, false);
			KGUI.SetNodes("Shop.page2_shop.takeoff_wskin", false, false);
			Store.PurchaseInfoWeaponSkin purchaseInfoWeaponSkin = (Store.PurchaseInfoWeaponSkin)Store.Purchases[purchase];
			bool flag4 = ProfileINI.WeaponSkinData.GetSelectedSkin(purchaseInfoWeaponSkin.SkinType) == purchaseInfoWeaponSkin.SkinId;
			if (ProfileINI.GetPurchaseValue(purchase) > 0)
			{
				KGUI.SetNodes("Shop.page2_shop.equip_wskin", true, false);
			}
			if (flag4)
			{
				KGUI.SetNodes("Shop.page2_shop.takeoff_wskin", true, false);
				KGUI.SetNodes("Shop.page2_shop.equip_wskin", false, false);
				KGUI.SetControlText("Shop.page2_shop.txt_info", Localize.GetText("SHOP_EQUIPPED", null));
			}
		}
		else
		{
			KGUI.SetNodes("Shop.page2_shop.equip_wskin", false, false);
			KGUI.SetNodes("Shop.page2_shop.takeoff_wskin", false, false);
		}
		bool flag5 = false;
		KGUI.SetNodes("Shop.page2_shop.skin_preview", false, false);
		KGUI.SetNodes("Shop.page2_shop.pet_preview", false, false);
		if (!flag)
		{
			if (this._ShopTab == Store.TabType.Skins)
			{
				GameObject gameObject = GameObject.Find("SkinPreview");
				if (gameObject != null)
				{
					gameObject.GetComponentInChildren<SkinManager>().SetSkin(Store.Purchases[purchase].Skin);
					KGUI.SetNodes("Shop.page2_shop.skin_preview", true, false);
					flag5 = true;
				}
			}
			else if (this._ShopTab == Store.TabType.Pets)
			{
				GameObject gameObject2 = GameObject.Find("PetPreview");
				if (gameObject2 != null)
				{
					switch (purchase)
					{
					case StorePurchase.FISH_1:
						gameObject2.GetComponentInChildren<SkinManager>().SetSkin(0);
						break;
					case StorePurchase.FISH_2:
						gameObject2.GetComponentInChildren<SkinManager>().SetSkin(1);
						break;
					case StorePurchase.FISH_3:
						gameObject2.GetComponentInChildren<SkinManager>().SetSkin(2);
						break;
					default:
						if (purchase != StorePurchase.BOAR)
						{
							if (purchase != StorePurchase.CRAB)
							{
								if (purchase != StorePurchase.CAT_BLACK)
								{
									if (purchase == StorePurchase.CAT_STRIPED)
									{
										gameObject2.GetComponentInChildren<SkinManager>().SetSkin(9);
									}
								}
								else
								{
									gameObject2.GetComponentInChildren<SkinManager>().SetSkin(8);
								}
							}
							else
							{
								gameObject2.GetComponentInChildren<SkinManager>().SetSkin(7);
							}
						}
						else
						{
							gameObject2.GetComponentInChildren<SkinManager>().SetSkin(6);
						}
						break;
					case StorePurchase.CAT:
						gameObject2.GetComponentInChildren<SkinManager>().SetSkin(3);
						break;
					case StorePurchase.DOG:
						gameObject2.GetComponentInChildren<SkinManager>().SetSkin(4);
						break;
					case StorePurchase.CHICKEN:
						gameObject2.GetComponentInChildren<SkinManager>().SetSkin(5);
						break;
					}
					KGUI.SetNodes("Shop.page2_shop.pet_preview", true, false);
					flag5 = true;
				}
			}
		}
		if (flag5)
		{
			base.StopCoroutine("PreviewRotationProcess");
			base.StartCoroutine("PreviewRotationProcess");
			KGUI.SetNodes("Shop.page2_shop.item_preview", false, false);
			KGUI.SetNodes("Shop.page2_shop.item_preview_shadow", false, false);
		}
		else
		{
			base.StopCoroutine("PreviewRotationProcess");
			KGUI.SetNodes("Shop.page2_shop.ibtn_rotate_left", false, false);
			KGUI.SetNodes("Shop.page2_shop.ibtn_rotate_right", false, false);
		}
		if (purchaseInfo.WeaponStats != null)
		{
			int[] array = purchaseInfo.WeaponStats;
			KGUI.SetControlText("Shop.page2_shop.weapon_stats.txt_stat1", string.Empty + ((array.Length < 1) ? 0 : array[0]));
			KGUI.SetControlText("Shop.page2_shop.weapon_stats.txt_stat2", string.Empty + ((array.Length < 2) ? 0 : array[1]));
			KGUI.SetControlText("Shop.page2_shop.weapon_stats.txt_stat3", string.Empty + ((array.Length < 3) ? 0 : array[2]));
			KGUI.SetControlText("Shop.page2_shop.weapon_stats.txt_stat4", string.Empty + ((array.Length < 4) ? 0 : array[3]));
		}
		else
		{
			KGUI.SetNodes("Shop.page2_shop.weapon_stats", false, false);
		}
		bool flag6 = ProfileINI.level < purchaseInfo.MinLevel;
		bool flag7 = purchaseInfo.RequiredPurchase != StorePurchase.NONE && ProfileINI.GetPurchaseValue(purchaseInfo.RequiredPurchase) <= 0;
		if (flag6 || flag7)
		{
			if (ProfileINI.GetPurchaseValue(purchase) == 0)
			{
				string text3 = string.Empty;
				if (flag6)
				{
					string text4 = text3;
					text3 = string.Concat(new object[]
					{
						text4,
						Localize.GetText("REQUIRED_LEVEL", null),
						purchaseInfo.MinLevel,
						"\n"
					});
				}
				else
				{
					KGUI.SetNodes("Shop.page2_shop.locked.level", false, false);
				}
				if (flag7)
				{
					text3 = text3 + Localize.GetText("REQUIRED_PURCHASE", null) + Localize.GetText("PURCHASE_" + Store.Purchases[purchaseInfo.RequiredPurchase].Name, null) + "\n";
				}
				KGUI.SetControlText("Shop.page2_shop.locked.txt_info", text3);
				KGUI.SetNodes("Shop.page2_shop.buy_one", false, false);
				KGUI.SetNodes("Shop.page2_shop.buy_day", false, false);
				KGUI.SetNodes("Shop.page2_shop.buy_week", false, false);
				KGUI.SetNodes("Shop.page2_shop.buy_forever", false, false);
				KGUI.SetNodes("Shop.page2_shop.buy_month", false, false);
			}
			else
			{
				KGUI.SetNodes("Shop.page2_shop.locked", false, false);
			}
		}
		else
		{
			KGUI.SetNodes("Shop.page2_shop.locked", false, false);
		}
	}

	private void GetPurchaseContent(out List<EntityType> purchaseEntities, out List<BlockType> purchaseBlocks, out bool isBuild, out mdl_Item_Build item_build)
	{
		purchaseEntities = new List<EntityType>();
		purchaseBlocks = new List<BlockType>();
		item_build = null;
		if (Store.Purchases[this._CurPurchase].Tab != Store.TabType.Hungry_Games)
		{
			foreach (KeyValuePair<EntityType, Store.EntityInfo> keyValuePair in Store.Entities)
			{
				if (keyValuePair.Value.Purchase == this._CurPurchase)
				{
					purchaseEntities.Add(keyValuePair.Key);
				}
			}
			foreach (KeyValuePair<BlockType, Store.BlockInfo> keyValuePair2 in Store.Blocks)
			{
				if (keyValuePair2.Value.Purchase == this._CurPurchase)
				{
					purchaseBlocks.Add(keyValuePair2.Key);
				}
			}
			isBuild = false;
		}
		else
		{
			item_build = ItemsBuildManager.GetBuild(this._CurPurchase);
			isBuild = true;
		}
	}

	private IEnumerator PreviewRotationProcess()
	{
		UIImageButton left = KGUI.FindNode("Shop.page2_shop.ibtn_rotate_left", false).GetComponent<UIImageButton>();
		UIImageButton right = KGUI.FindNode("Shop.page2_shop.ibtn_rotate_right", false).GetComponent<UIImageButton>();
		while (left.gameObject.activeInHierarchy && right.gameObject.activeInHierarchy)
		{
			float rot = 0f;
			if (left.target.spriteName == left.pressedSprite)
			{
				rot = -120f;
			}
			else if (right.target.spriteName == right.pressedSprite)
			{
				rot = 120f;
			}
			if (rot != 0f)
			{
				GameObject preview = null;
				if (this._ShopTab == Store.TabType.Skins)
				{
					preview = GameObject.Find("SkinPreview");
				}
				else if (this._ShopTab == Store.TabType.Pets)
				{
					preview = GameObject.Find("PetPreview");
				}
				if (preview != null)
				{
					preview.GetComponentInChildren<SkinManager>().transform.Rotate(Vector3.up, rot * Time.deltaTime);
				}
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	public string FindOpenRoom(int mapID)
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].customProperties.ContainsKey("public_id"))
			{
				int num = (int)roomList[i].customProperties["public_id"];
				if (num == mapID)
				{
					return (string)roomList[i].customProperties["map_name"];
				}
			}
		}
		return null;
	}

	private IEnumerator SetPregenerateTimer(int slot_id)
	{
		WWWForm form = new WWWForm();
		form.AddField("id", VKAPI.INSTANCE._viewerId);
		form.AddField("map_place", slot_id);
		System.Random r = new System.Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(SettingsManager.ServerURL[0] + "new_map_timer.php", form);
		yield return php_load;
		yield break;
	}

	private IEnumerator PreregenerateSlot(int slot_id)
	{
		this._SelectedMapID = -2;
		WWWForm form = new WWWForm();
		form.AddField("id", VKAPI.INSTANCE._viewerId);
		form.AddField("map_place", slot_id);
		System.Random r = new System.Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(SettingsManager.ServerURL[0] + "is_peregenerate_map.php", form);
		yield return php_load;
		this._SelectedMapID = 1;
		yield return 0;
		yield break;
	}

	private IEnumerator PostComment(string text, int map_id)
	{
		WWWForm form = new WWWForm();
		form.AddField("id", VKAPI.INSTANCE._viewerId);
		form.AddField("post_name", ProfileINI.nickname);
		form.AddField("post_text", text);
		form.AddField("map_id", map_id);
		System.Random r = new System.Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(SettingsManager.ServerURL[0] + "post_comment.php", form);
		yield return php_load;
		base.StartCoroutine(this.GetComments(map_id, this._MapCommentsStartIndex));
		yield break;
	}

	private IEnumerator GetComments(int map_id, int _page_numb)
	{
		this._MapCommentsStartIndex += _page_numb - this._MapCommentsStartIndex;
		this._MapComments.Clear();
		WWWForm form = new WWWForm();
		form.AddField("map_id", map_id);
		form.AddField("start_ind", this._MapCommentsStartIndex);
		UnityEngine.Debug.Log("GetComentStartIndex");
		UnityEngine.Debug.Log(this._MapCommentsStartIndex);
		System.Random r = new System.Random();
		form.AddField("random", r.Next());
		WWW php_load = new WWW(SettingsManager.ServerURL[0] + "get_coments.php", form);
		yield return php_load;
		if (php_load.error != null)
		{
			UnityEngine.Debug.LogError(php_load.error);
			yield break;
		}
		string[] commentsData = php_load.text.Split(new char[]
		{
			'\t'
		});
		for (int i = 1; i < commentsData.Length; i += 3)
		{
			MainMenu.Comment comment = new MainMenu.Comment();
			comment.ID = commentsData[i];
			comment.Name = commentsData[i + 1];
			comment.Text = commentsData[i + 2];
			this._MapComments.Add(comment);
		}
		yield break;
	}

	private IEnumerator PublicMapToServer(string mapName, string mapAbout, string pictureURL)
	{
		KGUI.SetNodes("MyMaps.page2", false, true);
		KGUI.SetNodes("main_menu.slider_page2", false, false);
		WWWForm publicMapForm = new WWWForm();
		publicMapForm.AddField("user_id", VKAPI.INSTANCE._viewerId);
		publicMapForm.AddField("slot_id", this._SelectedMapSlot);
		publicMapForm.AddField("name", mapName);
		publicMapForm.AddField("about", mapAbout);
		publicMapForm.AddField("url", (pictureURL == null) ? string.Empty : pictureURL);
		publicMapForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		WWW publicMap = new WWW(SettingsManager.ServerURL[0] + "public_map.php", publicMapForm);
		yield return publicMap;
		UnityEngine.Debug.Log("Public map: " + publicMap.text);
		yield return base.StartCoroutine(this.LoadSlotMapInfo(VKAPI.INSTANCE._viewerId, this._SelectedMapSlot));
		yield break;
	}

	private IEnumerator ClosePublicMap(string veiwer_id, int slot_id, int map_id)
	{
		KGUI.SetNodes("MyMaps.page2", false, true);
		KGUI.SetNodes("main_menu.slider_page2", false, false);
		WWWForm closeMapForm = new WWWForm();
		closeMapForm.AddField("user_id", veiwer_id);
		closeMapForm.AddField("slot_id", slot_id);
		closeMapForm.AddField("map_id", map_id);
		closeMapForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		WWW closeMap = new WWW(SettingsManager.ServerURL[0] + "close_map.php", closeMapForm);
		yield return closeMap;
		UnityEngine.Debug.Log("Close map: " + closeMap.text);
		yield return base.StartCoroutine(this.LoadSlotMapInfo(veiwer_id, slot_id));
		yield break;
	}

	private IEnumerator ModifyMapName(int slotID, string mapName)
	{
		this.ShowLoading("LOADING_WAIT", string.Empty);
		WWWForm modifyMapNameForm = new WWWForm();
		modifyMapNameForm.AddField("user_id", VKAPI.INSTANCE._viewerId);
		modifyMapNameForm.AddField("slot_id", slotID);
		modifyMapNameForm.AddField("map_name", mapName);
		modifyMapNameForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		WWW modifyMapName = new WWW(SettingsManager.ServerURL[0] + "modify_map_name.php", modifyMapNameForm);
		yield return modifyMapName;
		this.HideLoading();
		if (modifyMapName.error != null)
		{
			this.ShowHint(modifyMapName.error, false);
		}
		else if (modifyMapName.text != "OK")
		{
			this.ShowHint(modifyMapName.text, false);
		}
		else
		{
			ProfileINI.slotMapNames[slotID] = mapName;
			Transform slotNode = KGUI.FindNode("MyMaps.my_maps_grid." + (slotID - 1) + ".txt_title", true);
			if (slotNode != null)
			{
				slotNode.GetComponent<UILabel>().text = mapName;
			}
			if (slotID == this._SelectedMapSlot)
			{
				KGUI.SetControlText("MyMaps.page2.info.name.inp_name", ProfileINI.GetSlotMapName(this._SelectedMapSlot));
			}
		}
		yield break;
	}

	private IEnumerator LoadSlotMapInfo(string viewerID, int slotID)
	{
		string SceneName = SceneManager.GetActiveScene().name;
		KGUI.SetNodes("MyMaps.page2", false, true);
		KGUI.SetNodes("main_menu.slider_page2", false, false);
		WWWForm getSlotInfoForm = new WWWForm();
		getSlotInfoForm.AddField("id", viewerID);
		System.Random r = new System.Random();
		getSlotInfoForm.AddField("random", r.Next());
		getSlotInfoForm.AddField("slot_id", slotID);
		WWW getSlotInfo = new WWW(SettingsManager.ServerURL[0] + "get_slot_info.php", getSlotInfoForm);
		yield return getSlotInfo;
		this._SelectedMapOwner = viewerID;
		this._SelectedMapSlot = slotID;
		this._SelectedMapID = 0;
		this._SelectedMapNotVoted = false;
		this._SelectedMapRating = 0;
		this._SelectedMapAuthor = string.Empty;
		if (getSlotInfo.error != null)
		{
			this.ShowHint(getSlotInfo.error, false);
			this._SelectedMapSlot = 0;
			yield break;
		}
		if (getSlotInfo.text == "NOT AVAILABLE SLOT")
		{
			this.ShowHint("Слота не существет", false);
			this._SelectedMapSlot = 0;
			yield break;
		}
		if (getSlotInfo.text.StartsWith("OK"))
		{
			this._SelectedMapID = Convert.ToInt32(getSlotInfo.text.Substring(3));
			if (this._SelectedMapID < 0)
			{
				this.ShowHint("Слот заблокирован", false);
				this._SelectedMapSlot = 0;
				this._SelectedMapID = 0;
				yield break;
			}
		}
		else if (getSlotInfo.text != "EMPTY SLOT")
		{
			this.ShowHint(getSlotInfo.text, false);
			this._SelectedMapSlot = 0;
			yield break;
		}
		if (SceneName == "Menu")
		{
			if (this._SelectedMapID != 0)
			{
				KGUI.SetNodes("MyMaps.page2.base", true, false);
				KGUI.SetNodes("MyMaps.page2.base.save", false, false);
				if (this._SelectedMapID == 1)
				{
					KGUI.SetNodes("MyMaps.page2.base.load.button_look_in_public", false, false);
					KGUI.SetNodes("MyMaps.page2.base.load.button_remove_from_public", false, false);
				}
				else
				{
					KGUI.SetNodes("MyMaps.page2.base.load.button_generate", false, false);
					KGUI.SetNodes("MyMaps.page2.base.load.button_public", false, false);
				}
				KGUI.FindNode("MyMaps.page2.base.load", false).GetComponent<UITable>().Reposition();
			}
			else
			{
				this.ShowMyMap(true, false);
			}
		}
		else
		{
			KGUI.SetNodes("MyMaps.page2.base", true, false);
			KGUI.SetNodes("MyMaps.page2.base.load", false, false);
			KGUI.SetNodes("MyMaps.page2.base.save.txt_empty", false, false);
			KGUI.SetNodes("MyMaps.page2.base.save.txt_rewrite", false, false);
			if (this._SelectedMapID >= 1)
			{
				KGUI.SetNodes("MyMaps.page2.base.save.txt_rewrite", true, false);
			}
			else
			{
				KGUI.SetNodes("MyMaps.page2.base.save.txt_empty", true, false);
			}
		}
		yield break;
	}

	private void ShowMyMap(bool isGenerate, bool isToPublic)
	{
		KGUI.SetNodes("MyMaps.page2", false, true);
		KGUI.SetNodes("MyMaps.page2.info", true, false);
		if (isGenerate)
		{
			KGUI.FindNode("MyMaps.page2.info.map_time", false).tag = "Sort03";
			KGUI.SetControlText("MyMaps.page2.info.map_size.txt_type", Localize.GetText("MAP_SIZE_" + this._MapSizes[this._MapSize], null));
			KGUI.SetNodes("MyMaps.page2.info.game_type", false, false);
		}
		else
		{
			KGUI.FindNode("MyMaps.page2.info.map_time", false).tag = "Sort09";
			KGUI.SetNodes("MyMaps.page2.info.map_size", false, false);
			KGUI.SetNodes("MyMaps.page2.info.map_type", false, false);
			KGUI.SetNodes("MyMaps.page2.info.map_biom", false, false);
		}
		if (isToPublic)
		{
			KGUI.SetNodes("MyMaps.page2.info.game_type", false, false);
			KGUI.SetNodes("MyMaps.page2.info.password", false, false);
		}
		else
		{
			KGUI.SetControlText("MyMaps.page2.info.name.inp_name", ProfileINI.GetSlotMapName(this._SelectedMapSlot));
			KGUI.SetNodes("MyMaps.page2.info.description", false, false);
			KGUI.SetControlText("MyMaps.page2.info.map_time.txt_type", Localize.GetText("MAP_TIME_" + this._MapTimes[this._MapTime], null));
			KGUI.SetControlText("MyMaps.page2.info.game_type.txt_type", Localize.GetText("GAME_TYPE_" + this._MapGameTypes[this._MapGameType], null));
			KGUI.SetNodes("MyMaps.page2.info.screen", false, false);
		}
		this._CustomMapImageURL = null;
		KGUI.SetNodes("MyMaps.page2.info.screen_custom", false, false);
		KGUI.SetNodes("MyMaps.page2.info.screen_loading", false, false);
		KGUI.SetNodes("MyMaps.page2.info.button_load_map", !isGenerate && !isToPublic, false);
		KGUI.SetNodes("MyMaps.page2.info.button_public_map", isToPublic, false);
		KGUI.SetNodes("MyMaps.page2.info.button_create_map", isGenerate, false);
		KGUI.FindNode("MyMaps.page2.info.clip_map_info.grid_map_info", false).GetComponent<UITable>().Reposition();
		KGUI.ResetScrollBar("MyMaps.page2.info.clip_map_info", null);
	}

	public IEnumerator LoadTopMapInfo(int mapID)
	{
		KGUI.SetNodes("main_menu_top_maps.page2", false, false);
		this._MapCommentsStartIndex = 0;
		WWWForm getMapInfoForm = new WWWForm();
		getMapInfoForm.AddField("map_id", mapID);
		System.Random r = new System.Random();
		getMapInfoForm.AddField("random", r.Next());
		WWW getMapInfo = new WWW(SettingsManager.ServerURL[0] + "get_map_info.php", getMapInfoForm);
		yield return getMapInfo;
		if (getMapInfo.error != null)
		{
			UnityEngine.Debug.LogError(getMapInfo.error);
			yield break;
		}
		if (!KGUI.FindNode("main_menu_top_maps", false).gameObject.activeInHierarchy)
		{
			yield break;
		}
		string[] info = getMapInfo.text.Split(new char[]
		{
			'\t'
		});
		this._SelectedMapOwner = info[7];
		this._SelectedMapSlot = int.Parse(info[8]);
		this._SelectedMapID = mapID;
		this._SelectedMapNotVoted = false;
		this._SelectedMapRating = int.Parse(info[6]);
		this._SelectedMapAuthor = info[3];
		KGUI.SetNodes("main_menu_top_maps.page2", true, false);
		KGUI.SetNodes("main_menu_top_maps.score.background_large", false, false);
		KGUI.SetNodes("main_menu_top_maps.score.button_vote_yes", false, false);
		KGUI.SetControlText("main_menu_top_maps.txt_author", this._SelectedMapAuthor);
		KGUI.SetControlText("main_menu_top_maps.txt_score", string.Empty + this._SelectedMapRating);
		KGUI.SetControlText("main_menu_top_maps.txt_name", MainMenu.FixCollorName(info[0]));
		KGUI.SetControlText("main_menu_top_maps.txt_description", info[1]);
		KGUI.ResetScrollBar("main_menu_top_maps.page2.clip_map_info", null);
		base.StartCoroutine(this.GetCustomMapImage(mapID, "main_menu_top_maps.screen", info[2]));
		base.StartCoroutine(this.GetVoteData(mapID));
		yield break;
	}

	private IEnumerator GetVoteData(int mapID)
	{
		WWWForm getVoteForm = new WWWForm();
		getVoteForm.AddField("id", VKAPI.INSTANCE._viewerId);
		getVoteForm.AddField("map_id", mapID);
		System.Random r = new System.Random();
		getVoteForm.AddField("random", r.Next());
		WWW getVote = new WWW(SettingsManager.ServerURL[0] + "is_voute_by_map.php", getVoteForm);
		yield return getVote;
		if (mapID != this._SelectedMapID)
		{
			yield break;
		}
		if (getVote.error == null)
		{
			if (getVote.text.Trim() == "NOT_VOTE")
			{
				this._SelectedMapNotVoted = true;
				KGUI.SetNodes("main_menu_top_maps.score.background_large", true, false);
				KGUI.SetNodes("main_menu_top_maps.score.background_small", false, false);
				KGUI.SetNodes("main_menu_top_maps.score.button_vote_yes", true, false);
			}
			else if (getVote.text.Trim() == "YES")
			{
				UnityEngine.Debug.Log("Vote: Yes");
			}
			else if (getVote.text.Trim() == "NO")
			{
				UnityEngine.Debug.Log("Vote: No");
			}
			else
			{
				UnityEngine.Debug.LogError(getVote.text);
			}
		}
		else
		{
			UnityEngine.Debug.LogError(getVote.error);
		}
		yield break;
	}

	private IEnumerator VoteYes(int mapID)
	{
		WWWForm voteYesForm = new WWWForm();
		voteYesForm.AddField("id", VKAPI.INSTANCE._viewerId);
		voteYesForm.AddField("map_id", mapID);
		voteYesForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		System.Random r = new System.Random();
		voteYesForm.AddField("random", r.Next());
		WWW voteYes = new WWW(SettingsManager.ServerURL[0] + "voute_yes.php", voteYesForm);
		yield return voteYes;
		yield break;
	}

	private IEnumerator VoteYesNot(int mapID)
	{
		WWWForm voteYesNotForm = new WWWForm();
		voteYesNotForm.AddField("id", VKAPI.INSTANCE._viewerId);
		voteYesNotForm.AddField("map_id", mapID);
		System.Random r = new System.Random();
		voteYesNotForm.AddField("random", r.Next());
		voteYesNotForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		WWW voteYesNot = new WWW(SettingsManager.ServerURL[0] + "voute_yes_not.php", voteYesNotForm);
		yield return voteYesNot;
		yield break;
	}

	private IEnumerator VoteNoNot(int mapID)
	{
		WWWForm voteNoNotForm = new WWWForm();
		voteNoNotForm.AddField("id", VKAPI.INSTANCE._viewerId);
		voteNoNotForm.AddField("map_id", mapID);
		System.Random r = new System.Random();
		voteNoNotForm.AddField("random", r.Next());
		WWW voteNoNot = new WWW(SettingsManager.ServerURL[0] + "voute_no_not.php", voteNoNotForm);
		yield return voteNoNot;
		yield break;
	}

	private IEnumerator GetCustomMapImage(int mapID, string controlName, string imageURL)
	{
		UITable table = KGUI.FindNode(controlName, false).parent.GetComponent<UITable>();
		UISprite screen = KGUI.FindNode(controlName, false).GetComponent<UISprite>();
		UITexture screenCustom = KGUI.FindNode(controlName + "_custom", false).GetComponent<UITexture>();
		GameObject screenLoading = KGUI.FindNode(controlName + "_loading", false).gameObject;
		if (!screen.gameObject.activeInHierarchy || screenCustom.gameObject.activeInHierarchy || screenLoading.gameObject.activeInHierarchy)
		{
			screen.gameObject.SetActive(true);
			screenCustom.gameObject.SetActive(false);
			screenLoading.SetActive(false);
			if (table != null)
			{
				table.repositionNow = true;
			}
		}
		if (imageURL.Length == 0)
		{
			yield break;
		}
		screen.gameObject.SetActive(false);
		screenLoading.SetActive(true);
		if (table != null)
		{
			table.repositionNow = true;
		}
		WWW www = new WWW(imageURL);
		yield return www;
		if (mapID != this._SelectedMapID)
		{
			yield break;
		}
		screenLoading.SetActive(false);
		if (www.error == null)
		{
			screenCustom.gameObject.SetActive(true);
			screenCustom.mainTexture = www.texture;
			this._CustomMapImageURL = imageURL;
		}
		else
		{
			UnityEngine.Debug.LogError(www.error);
			screen.gameObject.SetActive(true);
		}
		if (table != null)
		{
			table.repositionNow = true;
		}
		yield break;
	}

	private IEnumerator RefreshListOfMapsBestAllTime()
	{
		yield return base.StartCoroutine(this.RefreshListOfMapsEx("get_all_time_map"));
		yield break;
	}

	private IEnumerator RefreshListOfMapsMonthly()
	{
		yield return base.StartCoroutine(this.RefreshListOfMapsEx("get_monthly_map"));
		yield break;
	}

	private IEnumerator RefreshListOfDay()
	{
		yield return base.StartCoroutine(this.RefreshListOfMapsEx("get_day_map"));
		yield break;
	}

	private IEnumerator RefreshListOfMapsWeekly()
	{
		yield return base.StartCoroutine(this.RefreshListOfMapsEx("get_weekly_map"));
		yield break;
	}

	private IEnumerator RefreshListOfMapsNew()
	{
		yield return base.StartCoroutine(this.RefreshListOfMapsEx("get_latest_map"));
		yield break;
	}

	private IEnumerator RefreshListOfMapsEx(string script)
	{
		this._GetMapsScript = script;
		this._GetMapsFormData = new Dictionary<string, string>
		{
			{
				"page",
				"0"
			},
			{
				"random",
				string.Empty + new System.Random().Next()
			}
		};
		WWWForm getMapsForm = new WWWForm();
		foreach (KeyValuePair<string, string> data in this._GetMapsFormData)
		{
			getMapsForm.AddField(data.Key, data.Value);
		}
		WWW getMaps = new WWW(SettingsManager.ServerURL[0] + script + ".php", getMapsForm);
		yield return getMaps;
		if (getMaps.error == null)
		{
			base.StartCoroutine(this.RefreshListOfMaps(getMaps.text, false));
		}
		else
		{
			UnityEngine.Debug.LogError(getMaps.error);
		}
		yield break;
	}

	private IEnumerator SearchMaps(int searchType, bool searchSortType, string searchMapName, string searchMapAuthor)
	{
		this._GetMapsScript = "search_maps";
		this._GetMapsFormData = new Dictionary<string, string>
		{
			{
				"text",
				(searchType != 0) ? searchMapAuthor : searchMapName
			},
			{
				"sortby",
				(!searchSortType) ? "public_date" : "rating"
			},
			{
				"type",
				string.Empty + searchType
			},
			{
				"page",
				"0"
			},
			{
				"random",
				string.Empty + new System.Random().Next()
			}
		};
		WWWForm getMapsForm = new WWWForm();
		foreach (KeyValuePair<string, string> data in this._GetMapsFormData)
		{
			getMapsForm.AddField(data.Key, data.Value);
		}
		WWW getMaps = new WWW(SettingsManager.ServerURL[0] + "search_maps.php", getMapsForm);
		yield return getMaps;
		if (getMaps.error == null)
		{
			base.StartCoroutine(this.RefreshListOfMaps(getMaps.text, false));
		}
		else
		{
			UnityEngine.Debug.LogError(getMaps.error);
		}
		yield break;
	}

	private IEnumerator LoadMoreMaps()
	{
		this._GetMapsFormData["random"] = string.Empty + new System.Random().Next();
		this._GetMapsFormData["page"] = string.Empty + (int.Parse(this._GetMapsFormData["page"]) + 1);
		WWWForm getMapsForm = new WWWForm();
		foreach (KeyValuePair<string, string> data in this._GetMapsFormData)
		{
			getMapsForm.AddField(data.Key, data.Value);
		}
		WWW getMaps = new WWW(SettingsManager.ServerURL[0] + this._GetMapsScript + ".php", getMapsForm);
		yield return getMaps;
		if (getMaps.error == null)
		{
			yield return base.StartCoroutine(this.RefreshListOfMaps(getMaps.text, true));
		}
		else
		{
			UnityEngine.Debug.LogError(getMaps.error);
		}
		yield break;
	}

	public IEnumerator RefreshListOfMaps(string text, bool append)
	{
		if (!append)
		{
			this._Maps.Clear();
		}
		if (text.Length == 0)
		{
			yield break;
		}
		string[] mapsData = text.Split(new char[]
		{
			'\t'
		});
		for (int i = 1; i < mapsData.Length; i += 5)
		{
			MainMenu.Map map = new MainMenu.Map();
			map.MapID = mapsData[i];
			map.AuthorID = mapsData[i + 1];
			map.AuthorSlotID = mapsData[i + 2];
			map.Name = mapsData[i + 3];
			map.Rating = mapsData[i + 4];
			this._Maps.Add(map);
		}
		bool moreMaps = true;
		KGUI.ResizeGrid("main_menu_top_maps.top_maps_grid", this._Maps.Count + ((!moreMaps) ? 0 : 1), delegate(GameObject slot, int index)
		{
			if (index < this._Maps.Count)
			{
				MainMenu.Map map2 = this._Maps[index];
				slot.GetComponentInChildren<UILabel>().text = MainMenu.FixCollorName(map2.Name) + " [* " + map2.Rating + "]";
			}
			else
			{
				slot.GetComponentInChildren<UILabel>().text = Localize.GetText("TOP_MAPS_MORE_MAPS", null);
			}
		}, null);
		if (!append && this._Maps.Count > 0 && this.CurMenu == Menu.TopMaps)
		{
			yield return base.StartCoroutine(this.LoadTopMapInfo(int.Parse(this._Maps[0].MapID)));
		}
		yield break;
	}

	public void ChangeNameProcessOut(string name)
	{
		base.StartCoroutine(this.ChangeNameProcess(name));
	}

	private IEnumerator ChangeNameProcess(string name)
	{
		if (string.IsNullOrEmpty(name) || !Regex.IsMatch(name, "^[a-zA-Zа-яА-Я0-9_-]{3,12}$"))
		{
			yield break;
		}
		this.ShowLoading("LOADING_NAME_CHANGING", string.Empty);
		KGUI.SetNodes("change_name.txt_name_busy", false, false);
		WWWForm setNameForm = new WWWForm();
		System.Random r = new System.Random();
		setNameForm.AddField("random", r.Next());
		setNameForm.AddField("id", VKAPI.INSTANCE._viewerId);
		setNameForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		setNameForm.AddField("name", name);
		WWW setName = new WWW(SettingsManager.ServerURL[0] + "SetName.php", setNameForm);
		yield return setName;
		if (setName.error == null)
		{
			if (setName.text == "SET_NAME")
			{
				ProfileINI.Save();
				if (!PhotonNetwork.connected)
				{
					App.Instance.ConnectToShard(true);
				}
				else if (this._CurMenu == Menu.ChangeName)
				{
					this.SwitchMenu(Menu.Start, null, null);
				}
				else
				{
					this.SetMenu(Menu.ChangeName, false, null, null);
				}
				ProfileINI.nickname = name;
				KGUI.SetControlText("Profile.txt_title", MainMenu.FixCollorName(name));
			}
			else
			{
				KGUI.SetNodes("change_name.txt_name_busy", true, false);
			}
		}
		else
		{
			UnityEngine.Debug.LogError(setName.error);
		}
		this.HideLoading();
		yield break;
	}

	public void RefreshTabMenu()
	{
		if (TeamBattle.Instance != null)
		{
			TeamBattle.Instance.RefreshTabMenu(false);
		}
		if (!KGUI.FindNode("tab_menu", false).gameObject.activeInHierarchy)
		{
			return;
		}
		bool isWatch = App.Instance.Settings.isWatch;
		bool isAdministrator = Level.Instance.IsAdmin(null) && !isWatch;
		bool isModerator = Level.Instance.IsModerator(null) && !isWatch;
		string text = (string)PhotonNetwork.room.customProperties["map_name"];
		KGUI.SetControlText("tab_menu.txt_title", text);
		if (isAdministrator)
		{
			KGUI.SetControlCheckbox("tab_menu.ckb_new_players_building", ProfileINI.newgamersislook);
		}
		else
		{
			KGUI.SetNodes("tab_menu.administration", false, false);
		}
		if (isWatch)
		{
			KGUI.SetControlText("tab_menu.top_map_info.txt_author", this._SelectedMapAuthor);
			KGUI.SetNodes("tab_menu.top_map_info.score.background_large", this._SelectedMapNotVoted, false);
			KGUI.SetNodes("tab_menu.top_map_info.score.background_small", !this._SelectedMapNotVoted, false);
			KGUI.SetNodes("tab_menu.top_map_info.score.button_vote_yes", this._SelectedMapNotVoted, false);
			KGUI.SetControlText("tab_menu.top_map_info.score.txt_score", string.Empty + this._SelectedMapRating);
		}
		else
		{
			KGUI.SetNodes("tab_menu.top_map_info", false, false);
		}
		string[] banned = Level.Instance.Banned;
		KGUI.ResizeGrid("tab_menu.grid_players", WorldGameObjectX.Instance.PlayerList.Count + banned.Length, delegate(GameObject slot, int index)
		{
			Transform transform = slot.transform;
			string text2;
			bool flag;
			if (index < WorldGameObjectX.Instance.PlayerList.Count)
			{
				PlayerNode playerNode = WorldGameObjectX.Instance.PlayerList[index];
				text2 = playerNode.NetPlayer.name;
				flag = (playerNode.NetPlayer == PhotonNetwork.player);
				KGUI.SetControlSprite(transform.Find("level_icon"), "level_" + playerNode.Level, 0f);
				this.AddVoiceHighlight(playerNode, transform.Find("ibtn_voice"));
				transform.Find("level_icon").gameObject.SetActive(false);
			}
			else
			{
				text2 = banned[index - WorldGameObjectX.Instance.PlayerList.Count];
				flag = false;
				transform.Find("level_icon").gameObject.SetActive(false);
			}
			bool flag2 = Level.Instance.IsAdmin(text2) && !isWatch;
			bool flag3 = Level.Instance.IsModerator(text2) && !isWatch;
			bool isChecked = Level.Instance.IsBuilder(text2) && !isWatch;
			bool flag4 = Level.Instance.IsBanned(text2);
			transform.Find("txt_name").GetComponent<UILabel>().text = MainMenu.FixCollorName(text2);
			transform.Find("txt_right").GetComponent<UILabel>().text = ((!flag) ? string.Empty : Localize.GetText("TAB_MENU_YOU", null));
			Transform transform2 = transform.Find("checkbox");
			transform2.gameObject.SetActive(true);
			if (!flag2 && !flag4)
			{
				transform2.Find("administrator").gameObject.SetActive(false);
				transform2.Find("banned").gameObject.SetActive(false);
				UICheckbox componentInChildren = transform2.Find("ckb_moderator").GetComponentInChildren<UICheckbox>();
				UICheckbox componentInChildren2 = transform2.Find("ckb_builder").GetComponentInChildren<UICheckbox>();
				KGUI.SetControlCheckbox("tab_menu.grid_players." + index + ".ckb_moderator", flag3);
				KGUI.SetControlCheckbox("tab_menu.grid_players." + index + ".ckb_builder", isChecked);
				componentInChildren.enabled = isAdministrator;
				componentInChildren2.enabled = (isAdministrator || isModerator);
			}
			else
			{
				transform2.Find("ckb_moderator").gameObject.SetActive(false);
				transform2.Find("ckb_builder").gameObject.SetActive(false);
				transform2.Find("administrator").gameObject.SetActive(flag2);
				transform2.Find("banned").gameObject.SetActive(flag4);
			}
			Transform transform3 = transform.Find("button_ban");
			Transform transform4 = transform.Find("button_unban");
			if (((isAdministrator && !flag2) || (isModerator && !flag2 && !flag3)) && !flag)
			{
				if (flag4)
				{
					transform3.gameObject.SetActive(false);
				}
				else
				{
					transform4.gameObject.SetActive(false);
				}
			}
			else
			{
				transform3.gameObject.SetActive(false);
				transform4.gameObject.SetActive(false);
			}
			transform.Find("level_icon").gameObject.SetActive(false);
			if (!flag4)
			{
				if (ProfileINI.GetPurchaseValue(StorePurchase.TELEPORT) == 1)
				{
					KGUI.SetNodes("tab_menu.grid_players.ibtn_teleport", true, false);
					transform.Find("ibtn_teleport").gameObject.SetActive(true);
				}
				if (ProfileINI.GetPurchaseValue(StorePurchase.TELEPORT) != 1)
				{
					KGUI.SetNodes("tab_menu.grid_players.ibtn_teleport", false, false);
					transform.Find("ibtn_teleport").gameObject.SetActive(false);
				}
			}
			else
			{
				KGUI.SetNodes("tab_menu.grid_players.ibtn_teleport", false, false);
				transform.Find("ibtn_teleport").gameObject.SetActive(false);
			}
		}, "tab_menu");
	}

	public void RefreshShop()
	{
		if (!KGUI.FindNode("Shop", false).gameObject.activeInHierarchy)
		{
			return;
		}
		KGUI.SetControlText("Shop.icons.txt_title", Localize.GetText("STORE_" + this._ShopTab.ToString().ToUpper(), null));
		foreach (string text in Store.TabTypeButtonNames)
		{
			if (KGUI.FindNode("Shop" + text, true) != null)
			{
				KGUI.SetNodes("Shop" + text, !text.Contains("ibtn_blocks_"), false);
			}
		}
		KGUI.FindNode("Shop.grid_icons", false).GetComponent<UITable>().Reposition();
		this._CurShopPurchases = new Dictionary<string, List<StorePurchase>>();
		foreach (KeyValuePair<StorePurchase, Store.PurchaseInfo> keyValuePair in Store.Purchases)
		{
			if (!keyValuePair.Value.Disabled && keyValuePair.Value.Tab == this._ShopTab)
			{
				if (keyValuePair.Key != StorePurchase.ALL_INCLUSIVE && keyValuePair.Key != StorePurchase.NY_OFFER && keyValuePair.Key != StorePurchase.NY_TREE && keyValuePair.Key != StorePurchase.WEAPON_LUGER && keyValuePair.Key != StorePurchase.WEAPON_MP5 && keyValuePair.Key != StorePurchase.WEAPON_SAWN_OFF && keyValuePair.Key != StorePurchase.FLOWER && keyValuePair.Key != StorePurchase.FLOWER2 && keyValuePair.Key != StorePurchase.FLOWER3 && keyValuePair.Key != StorePurchase.FLAG && keyValuePair.Key != StorePurchase.WEAPON_GLOK)
				{
					if (keyValuePair.Key != StorePurchase.SANTA_SKIN || ProfileINI.GetPurchaseValue(StorePurchase.SANTA_SKIN) > 0)
					{
						if (!this._CurShopPurchases.ContainsKey(keyValuePair.Value.Category))
						{
							this._CurShopPurchases.Add(keyValuePair.Value.Category, new List<StorePurchase>());
						}
						this._CurShopPurchases[keyValuePair.Value.Category].Add(keyValuePair.Key);
					}
				}
			}
		}
		KGUI.ResizeGrid("Shop.grid_items_categories", this._CurShopPurchases.Count, delegate(GameObject slot, int index)
		{
			List<StorePurchase> purchases = this._CurShopPurchases[this._CurShopPurchases.Keys.ElementAt(index)];
			KGUI.ResizeGrid("Shop.grid_items_categories." + index + ".grid_items", purchases.Count, delegate(GameObject slot2, int index2)
			{
				Store.PurchaseInfo purchaseInfo = Store.Purchases[purchases[index2]];
				KGUI.SetControlSprite(slot2.transform.Find("item_image"), purchaseInfo.Icon, 60f);
				bool flag = ProfileINI.level < purchaseInfo.MinLevel;
				bool flag2 = purchaseInfo.RequiredPurchase != StorePurchase.NONE && ProfileINI.GetPurchaseValue(purchaseInfo.RequiredPurchase) <= 0;
				if (!flag && !flag2)
				{
					slot2.transform.Find("locked").gameObject.SetActive(false);
				}
				else if (flag)
				{
					KGUI.SetControlSprite(slot2.transform.Find("locked").Find("level"), "level_" + purchaseInfo.MinLevel, 0f);
				}
				else
				{
					slot2.transform.Find("locked").Find("level").gameObject.SetActive(false);
				}
			}, null);
			if (this._CurShopPurchases.Count <= 1)
			{
				slot.transform.Find("bookmark").gameObject.SetActive(false);
			}
			else
			{
				slot.transform.Find("bookmark").Find("txt_title").GetComponent<UILabel>().text = Localize.GetText(Store.Purchases[purchases[0]].Category, null);
			}
		}, "Shop_" + this._ShopTab);
		base.StartCoroutine(this.RefreshCategoriesProcess("Shop.grid_items_categories"));
	}

	public IEnumerator RefreshCategoriesProcess(string nodeName)
	{
		yield return 0;
		KGUI.FindNode(nodeName, false).GetComponent<UITable>().repositionNow = true;
		yield break;
	}

	public void RefreshInventory()
	{
		if (!KGUI.FindNode("inventory", false).gameObject.activeInHierarchy)
		{
			return;
		}
		KGUI.SetControlText("inventory.icons.txt_title", Localize.GetText("STORE_" + this._InventoryTab.ToString().ToUpper(), null));
		foreach (string text in Store.TabTypeButtonNames)
		{
			if (KGUI.FindNode("inventory" + text, true) != null)
			{
				if (this._InventoryTab.ToString().Contains("Blocks_"))
				{
					KGUI.SetNodes("inventory" + text, text.Contains("ibtn_blocks_"), false);
				}
				else
				{
					KGUI.SetNodes("inventory" + text, !text.Contains("ibtn_blocks_") && !text.Contains("ibtn_maps") && !text.Contains("ibtn_skins") && !text.Contains("ibtn_weapons") && !text.Contains("ibtn_bonuses"), false);
				}
			}
		}
		KGUI.FindNode("inventory.grid_icons", false).GetComponent<UITable>().Reposition();
		if (!this._InventoryTab.ToString().Contains("Blocks"))
		{
			this._CurInventoryItems.Clear();
			foreach (KeyValuePair<EntityType, Store.EntityInfo> keyValuePair in Store.Entities)
			{
				if (keyValuePair.Value.Tab == this._InventoryTab && this.GetInventoryItemCount(keyValuePair.Key) != 0 && (keyValuePair.Value.Validator == null || keyValuePair.Value.Validator()))
				{
					if (!this._CurInventoryItems.ContainsKey(keyValuePair.Value.Category))
					{
						this._CurInventoryItems.Add(keyValuePair.Value.Category, new List<EntityType>());
					}
					if (keyValuePair.Key != EntityType.HG_DROP_BAG)
					{
						this._CurInventoryItems[keyValuePair.Value.Category].Add(keyValuePair.Key);
					}
				}
			}
			KGUI.ResizeGrid("inventory.grid_items_categories", this._CurInventoryItems.Count, delegate(GameObject slot, int index)
			{
				List<EntityType> items = new List<EntityType>(this._CurInventoryItems[this._CurInventoryItems.Keys.ElementAt(index)]);
				if (items.Contains(EntityType.HG_DROP_BAG))
				{
					items.Remove(EntityType.HG_DROP_BAG);
				}
				KGUI.ResizeGrid("inventory.grid_items_categories." + index + ".grid_items", items.Count, delegate(GameObject slot2, int index2)
				{
					EntityType entityType = items[index2];
					int inventoryItemCount = this.GetInventoryItemCount(entityType);
					KGUI.SetControlSprite(slot2.transform.Find("item_image"), Store.Entities[entityType].SpriteName, 60f);
					slot2.transform.Find("txt_count").GetComponent<UILabel>().text = string.Empty;
					if (inventoryItemCount != -1)
					{
						Store.Pay cost = Store.Purchases[Store.Entities[entityType].Purchase].Cost;
						if (cost is Store.OnePay && !((Store.OnePay)cost).Once)
						{
							slot2.transform.Find("txt_count").GetComponent<UILabel>().text = string.Empty + inventoryItemCount;
						}
					}
				}, null);
				if (this._CurInventoryItems.Count <= 1)
				{
					slot.transform.Find("bookmark").gameObject.SetActive(false);
				}
				else
				{
					slot.transform.Find("bookmark").Find("txt_title").GetComponent<UILabel>().text = Localize.GetText(Store.Entities[items[0]].Category, null);
				}
			}, "inventory_" + this._InventoryTab);
			base.StartCoroutine(this.RefreshCategoriesProcess("inventory.grid_items_categories"));
		}
		else
		{
			this._CurInventoryBlocks.Clear();
			foreach (KeyValuePair<BlockType, Store.BlockInfo> keyValuePair2 in Store.Blocks)
			{
				if (keyValuePair2.Value.Tab == this._InventoryTab && this.GetInventoryBlockCount(keyValuePair2.Key) != 0 && (keyValuePair2.Value.Validator == null || keyValuePair2.Value.Validator()))
				{
					if (!this._CurInventoryBlocks.ContainsKey(keyValuePair2.Value.Category))
					{
						this._CurInventoryBlocks.Add(keyValuePair2.Value.Category, new List<BlockType>());
					}
					this._CurInventoryBlocks[keyValuePair2.Value.Category].Add(keyValuePair2.Key);
				}
			}
			KGUI.ResizeGrid("inventory.grid_items_categories", this._CurInventoryBlocks.Count, delegate(GameObject slot, int index)
			{
				List<BlockType> blocks = this._CurInventoryBlocks[this._CurInventoryBlocks.Keys.ElementAt(index)];
				KGUI.ResizeGrid("inventory.grid_items_categories." + index + ".grid_items", blocks.Count, delegate(GameObject slot2, int index2)
				{
					BlockType key = blocks[index2];
					KGUI.SetControlSprite(slot2.transform.Find("item_image"), Store.Blocks[key].SpriteName, 60f);
					slot2.transform.Find("txt_count").GetComponent<UILabel>().text = string.Empty;
				}, null);
				if (this._CurInventoryBlocks.Count <= 1)
				{
					slot.transform.Find("bookmark").gameObject.SetActive(false);
				}
				else
				{
					slot.transform.Find("bookmark").Find("txt_title").GetComponent<UILabel>().text = Localize.GetText(Store.Blocks[blocks[0]].Category, null);
				}
			}, "inventory_" + this._InventoryTab);
			base.StartCoroutine(this.RefreshCategoriesProcess("inventory.grid_items_categories"));
		}
	}

	private int GetInventoryItemCount(EntityType entityType)
	{
		Store.EntityInfo entityInfo = Store.Entities[entityType];
		if (entityInfo.Purchase == StorePurchase.NONE)
		{
			return -1;
		}
		return Mathf.Max(ProfileINI.GetPurchaseValue(entityInfo.Purchase), 0);
	}

	private int GetInventoryBlockCount(BlockType blockType)
	{
		Store.BlockInfo blockInfo = Store.Blocks[blockType];
		if (blockInfo.Purchase == StorePurchase.NONE)
		{
			return -1;
		}
		return Mathf.Max(ProfileINI.GetPurchaseValue(blockInfo.Purchase), 0);
	}

	public void AddWeaponFromID(int id, int cell_id)
	{
		if (WorldGameObjectX.Instance != null)
		{
			PlayerNetwork component = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>();
			if (!GameType.IsHungerGamesMode)
			{
				if (id == -1)
				{
					component.MainWeapon[cell_id] = null;
				}
				else
				{
					Gun gun = component.Guns[id];
					component.MainWeapon[cell_id] = gun;
				}
			}
			this.RefreshBattleWeapon(cell_id);
		}
	}

	public void RefreshBattleWeapon(int cur_select = 0)
	{
		PlayerNetwork player = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>();
		List<Gun> weapons = new List<Gun>();
		if (!GameType.IsHungerGamesMode)
		{
			weapons.Add(player.Knife);
		}
		weapons.AddRange(player.MainWeapon);
		weapons.Add(player.Grenade);
		weapons.Add(player.Health);
		weapons.Add(player.Shield);
		KGUI.SetNodes(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Text", false, false);
		KGUI.SetNodes(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Sprite", false, false);
		KGUI.SetNodes(this.hud_tag + ".hide_seek", false, false);
		if (Info.Instance.GameMode == "TEAM_BATTLE" || Info.Instance.GameMode == "ZOMBIE_VIRUS")
		{
			KGUI.SetNodes(this.hud_tag + ".hide_seek", false, false);
			KGUI.ResizeGrid(this.hud_tag + ".battle.grid_weapons", weapons.Count, delegate(GameObject slot, int index)
			{
				Gun gun = weapons[index];
				int num = index + 1;
				KGUI.SetNodes(string.Concat(new object[]
				{
					this.hud_tag,
					".battle.grid_weapons.",
					index,
					".numbers"
				}), false, true);
				KGUI.SetNodes(string.Concat(new object[]
				{
					this.hud_tag,
					".battle.grid_weapons.",
					index,
					".numbers.",
					num
				}), true, false);
				KGUI.SetNodes(string.Concat(new object[]
				{
					this.hud_tag,
					".battle.grid_weapons.",
					index,
					".weapon"
				}), false, true);
				KGUI.SetControlText(string.Concat(new object[]
				{
					this.hud_tag,
					".battle.grid_weapons.",
					index,
					".count"
				}), string.Empty);
				if (gun != null)
				{
					KGUI.SetNodes(string.Concat(new object[]
					{
						this.hud_tag,
						".battle.grid_weapons.",
						index,
						".weapon.normal"
					}), true, false);
					KGUI.SetControlSprite(KGUI.FindNode(string.Concat(new object[]
					{
						this.hud_tag,
						".battle.grid_weapons.",
						index,
						".weapon.normal"
					}), false), "weapon_" + gun.name, 0f);
					if (gun.storePurchaseCount != StorePurchase.NONE)
					{
						KGUI.SetControlText(string.Concat(new object[]
						{
							this.hud_tag,
							".battle.grid_weapons.",
							index,
							".count"
						}), string.Empty + gun.bullets);
						float num2 = (ProfileINI.GetPurchaseValue(gun.storePurchaseCount) > 0) ? ProfileINI.GetPurchaseCooldown01(gun.storePurchaseCount) : 1f;
						if (num2 > 0f)
						{
							KGUI.SetNodes(string.Concat(new object[]
							{
								this.hud_tag,
								".battle.grid_weapons.",
								index,
								".weapon.gray"
							}), true, false);
							KGUI.SetControlSprite(KGUI.FindNode(string.Concat(new object[]
							{
								this.hud_tag,
								".battle.grid_weapons.",
								index,
								".weapon.gray"
							}), false), "weapon_" + gun.name + "_gray", 0f);
							KGUI.FindNode(string.Concat(new object[]
							{
								this.hud_tag,
								".battle.grid_weapons.",
								index,
								".weapon.gray.sprite"
							}), false).GetComponent<UISprite>().fillAmount = num2;
						}
					}
				}
				else
				{
					KGUI.SetNodes(string.Concat(new object[]
					{
						this.hud_tag,
						".battle.grid_weapons.",
						index,
						".weapon"
					}), false, false);
				}
				if (gun != null && player.CurGun == gun)
				{
					if (!gun.melee && !gun.custom)
					{
						KGUI.SetNodes(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Sprite", true, false);
						KGUI.SetNodes(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Text", true, false);
						if (!gun.grenade)
						{
							KGUI.SetControlSprite(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Sprite", "Bullet", 0);
							KGUI.SetControlText(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Text", gun.bullets + "/" + gun.bulletsLeft);
						}
						else
						{
							KGUI.SetControlSprite(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Sprite", "Grenade", 0);
							KGUI.SetControlText(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Text", string.Empty + gun.bullets);
						}
					}
					else
					{
						KGUI.SetNodes(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Sprite", false, false);
						KGUI.SetNodes(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Text", false, false);
					}
				}
				else
				{
					KGUI.SetNodes(string.Concat(new object[]
					{
						this.hud_tag,
						".battle.grid_weapons.",
						index,
						".selected"
					}), false, false);
				}
			}, null);
		}
		else if (GameType.IsHungerGamesMode)
		{
			KGUI.SetNodes(this.hud_tag + ".hide_seek", false, false);
			KGUI.ResizeGrid(this.hud_tag + ".battle.grid_weapons", 6, delegate(GameObject slot, int index)
			{
				if (cur_select != 0)
				{
					this.HgWorkController.select_cell = cur_select;
				}
				if (pnl_Inventory.sweapon_cell != null)
				{
					is_InventoryCell is_InventoryCell = pnl_Inventory.all_weapon_slot[index];
					KGUI.SetNodes(string.Concat(new object[]
					{
						this.hud_tag,
						".battle.grid_weapons.",
						index,
						".SelectorLabel"
					}), false, true);
					KGUI.SetControlText(string.Concat(new object[]
					{
						this.hud_tag,
						".battle.grid_weapons.",
						index,
						".SelectorLabel"
					}), (index + 1).ToString());
					try
					{
						if (index == cur_select && InventaryObjManager.GetItemFromBagId(is_InventoryCell.item_guid) != null)
						{
							this.HgWorkController.SetLastWeapon(InventaryObjManager.GetItemFromBagId(is_InventoryCell.item_guid).Id, this.HgWorkController.select_cell);
						}
					}
					catch (Exception ex)
					{
						UnityEngine.Debug.LogError("Error HgWorkController.SetLastWeapon\n" + ex.StackTrace);
					}
					if (!is_InventoryCell.isEmpty)
					{
						KGUI.SetNodes(string.Concat(new object[]
						{
							this.hud_tag,
							".battle.grid_weapons.",
							index,
							".weapons"
						}), true, false);
						try
						{
							KGUI.SetControlSprite(string.Concat(new object[]
							{
								this.hud_tag,
								".battle.grid_weapons.",
								index,
								".weapons"
							}), InventaryObjManager.GetItemFromBagId(is_InventoryCell.item_guid).IconName, 30);
						}
						catch (Exception ex2)
						{
							UnityEngine.Debug.LogError("Error KGUI.SetControlSprite\n" + ex2.StackTrace);
						}
					}
					else
					{
						KGUI.SetNodes(string.Concat(new object[]
						{
							this.hud_tag,
							".battle.grid_weapons.",
							index,
							".weapons"
						}), false, false);
					}
					if (index == cur_select)
					{
						KGUI.SetNodes(string.Concat(new object[]
						{
							this.hud_tag,
							".battle.grid_weapons.",
							index,
							".selected"
						}), true, false);
					}
					else
					{
						KGUI.SetNodes(string.Concat(new object[]
						{
							this.hud_tag,
							".battle.grid_weapons.",
							index,
							".selected"
						}), false, false);
					}
				}
			}, null);
		}
		else if (GameType.IsArcadeMode)
		{
			KGUI.SetNodes(this.hud_tag + ".battle.grid_weapons", false, false);
		}
		else if (GameType.IsHideSeek)
		{
			KGUI.SetNodes(this.hud_tag + ".battle.grid_weapons", false, false);
			KGUI.SetNodes(this.hud_tag + ".hide_seek", false, false);
		}
		if (WorldGameObjectX.Instance.MainPlayerNode.IsZombie)
		{
			KGUI.SetNodes(this.hud_tag + ".battle.grid_weapons", false, false);
			if (!Level.Instance.CanBuildZombieGame)
			{
				KGUI.SetNodes(this.hud_tag + ".battle.anchor_bottom_right.Bullet.Text", false, false);
			}
		}
	}

	public IEnumerator BattleWeaponCooldownProcess()
	{
		while (ProfileINI.GetPurchaseCooldown(StorePurchase.BATTLE_GRENADES) > 0f || ProfileINI.GetPurchaseCooldown(StorePurchase.BATTLE_HEALTH) > 0f || ProfileINI.GetPurchaseCooldown(StorePurchase.BATTLE_SHIELD) > 0f)
		{
			this.RefreshBattleWeapon(0);
			yield return new WaitForSeconds(0.01f);
		}
		this.RefreshBattleWeapon(0);
		yield break;
	}

	private void ChangeMapSize(bool next)
	{
		int newMapSize = this._MapSize;
		for (;;)
		{
			newMapSize += ((!next) ? -1 : 1);
			if (newMapSize < 0 || newMapSize >= this._MapSizes.Length)
			{
				break;
			}
			KeyValuePair<StorePurchase, Store.PurchaseInfo> keyValuePair = Store.Purchases.FirstOrDefault((KeyValuePair<StorePurchase, Store.PurchaseInfo> p) => p.Value.Data != null && p.Value.Data.Value is GameINI.MapSize && (int)p.Value.Data.Value == (int)this._MapSizes[newMapSize]);
			if (keyValuePair.Value == null || ProfileINI.GetPurchaseValue(keyValuePair.Key) > 0)
			{
				goto IL_8C;
			}
		}
		return;
		IL_8C:
		this._MapSize = newMapSize;
		KGUI.SetControlText("MyMaps.page2.info.map_size.txt_type", Localize.GetText("MAP_SIZE_" + this._MapSizes[this._MapSize], null));
	}

	private void ChangeMapType(bool next)
	{
		if (next)
		{
			if (MapsGen.MapTypeNumber == 0)
			{
				if (ProfileINI.GetPurchaseValue(StorePurchase.FLAT_MAP) == 1)
				{
					MapsGen.MapTypeNumber = 1;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.PLATFORM_MAP) == 1)
				{
					MapsGen.MapTypeNumber = 2;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.RUN_MAP) == 1)
				{
					MapsGen.MapTypeNumber = 3;
				}
			}
			else if (MapsGen.MapTypeNumber == 1)
			{
				if (ProfileINI.GetPurchaseValue(StorePurchase.PLATFORM_MAP) == 1)
				{
					MapsGen.MapTypeNumber = 2;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.RUN_MAP) == 1)
				{
					MapsGen.MapTypeNumber = 3;
				}
			}
			else if (MapsGen.MapTypeNumber == 2 && ProfileINI.GetPurchaseValue(StorePurchase.RUN_MAP) == 1)
			{
				MapsGen.MapTypeNumber = 3;
			}
		}
		else if (MapsGen.MapTypeNumber == 3)
		{
			if (ProfileINI.GetPurchaseValue(StorePurchase.PLATFORM_MAP) == 1)
			{
				MapsGen.MapTypeNumber = 2;
			}
			else if (ProfileINI.GetPurchaseValue(StorePurchase.FLAT_MAP) == 1)
			{
				MapsGen.MapTypeNumber = 1;
			}
		}
		else if (MapsGen.MapTypeNumber == 2)
		{
			if (ProfileINI.GetPurchaseValue(StorePurchase.FLAT_MAP) == 1)
			{
				MapsGen.MapTypeNumber = 1;
			}
			else
			{
				MapsGen.MapTypeNumber = 0;
			}
		}
		else if (MapsGen.MapTypeNumber == 1)
		{
			MapsGen.MapTypeNumber = 0;
		}
		KGUI.SetControlText("MyMaps.page2.info.map_type.txt_type", Localize.GetText("key900" + MapsGen.MapTypeNumber, null));
		if (MapsGen.MapTypeNumber == 2)
		{
			KGUI.SetControlText("MyMaps.page2.info.map_biom.txt_type", "Стандарт");
		}
		else if (MapsGen.MapTypeNumber == 3)
		{
			KGUI.SetControlText("MyMaps.page2.info.map_biom.txt_type", "Стандарт");
		}
		else
		{
			KGUI.SetControlText("MyMaps.page2.info.map_biom.txt_type", Localize.GetText("key110" + MapsGen.MapBiomNumber, null));
		}
	}

	private void ChangeBiomType(bool next)
	{
		if (MapsGen.MapTypeNumber >= 2)
		{
			return;
		}
		if (next)
		{
			if (MapsGen.MapBiomNumber == 0)
			{
				if (ProfileINI.GetPurchaseValue(StorePurchase.BIOM_AUTUMN) == 1)
				{
					MapsGen.MapBiomNumber = 1;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.SNOW_MAP) == 1)
				{
					MapsGen.MapBiomNumber = 2;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.OCEAN_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 3;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.ISLAND_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 4;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.SAND_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 5;
				}
			}
			else if (MapsGen.MapBiomNumber == 1)
			{
				if (ProfileINI.GetPurchaseValue(StorePurchase.SNOW_MAP) == 1)
				{
					MapsGen.MapBiomNumber = 2;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.OCEAN_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 3;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.ISLAND_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 4;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.SAND_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 5;
				}
			}
			else if (MapsGen.MapBiomNumber == 2)
			{
				if (MapsGen.MapTypeNumber >= 2)
				{
					return;
				}
				if (ProfileINI.GetPurchaseValue(StorePurchase.OCEAN_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 3;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.ISLAND_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 4;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.SAND_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 5;
				}
			}
			else if (MapsGen.MapBiomNumber == 3)
			{
				if (ProfileINI.GetPurchaseValue(StorePurchase.ISLAND_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 4;
				}
				else if (ProfileINI.GetPurchaseValue(StorePurchase.SAND_MAP) == 1 && MapsGen.MapTypeNumber == 0)
				{
					MapsGen.MapBiomNumber = 5;
				}
			}
			else if (MapsGen.MapBiomNumber == 4 && ProfileINI.GetPurchaseValue(StorePurchase.SAND_MAP) == 1 && MapsGen.MapTypeNumber == 0)
			{
				MapsGen.MapBiomNumber = 5;
			}
		}
		else if (MapsGen.MapBiomNumber == 5)
		{
			if (ProfileINI.GetPurchaseValue(StorePurchase.ISLAND_MAP) == 1 && MapsGen.MapTypeNumber == 0)
			{
				MapsGen.MapBiomNumber = 4;
			}
			else if (ProfileINI.GetPurchaseValue(StorePurchase.OCEAN_MAP) == 1 && MapsGen.MapTypeNumber == 0)
			{
				MapsGen.MapBiomNumber = 3;
			}
			else if (ProfileINI.GetPurchaseValue(StorePurchase.SNOW_MAP) == 1)
			{
				MapsGen.MapBiomNumber = 2;
			}
			else if (ProfileINI.GetPurchaseValue(StorePurchase.BIOM_AUTUMN) == 1)
			{
				MapsGen.MapBiomNumber = 1;
			}
		}
		else if (MapsGen.MapBiomNumber == 4)
		{
			if (ProfileINI.GetPurchaseValue(StorePurchase.OCEAN_MAP) == 1 && MapsGen.MapTypeNumber == 0)
			{
				MapsGen.MapBiomNumber = 3;
			}
			else if (ProfileINI.GetPurchaseValue(StorePurchase.SNOW_MAP) == 1)
			{
				MapsGen.MapBiomNumber = 2;
			}
			else if (ProfileINI.GetPurchaseValue(StorePurchase.BIOM_AUTUMN) == 1)
			{
				MapsGen.MapBiomNumber = 1;
			}
		}
		else if (MapsGen.MapBiomNumber == 3)
		{
			if (ProfileINI.GetPurchaseValue(StorePurchase.SNOW_MAP) == 1)
			{
				MapsGen.MapBiomNumber = 2;
			}
			else if (ProfileINI.GetPurchaseValue(StorePurchase.BIOM_AUTUMN) == 1)
			{
				MapsGen.MapBiomNumber = 1;
			}
		}
		else if (MapsGen.MapBiomNumber == 2)
		{
			if (ProfileINI.GetPurchaseValue(StorePurchase.BIOM_AUTUMN) == 1)
			{
				MapsGen.MapBiomNumber = 1;
			}
			else
			{
				MapsGen.MapBiomNumber = 0;
			}
		}
		else if (MapsGen.MapBiomNumber == 1)
		{
			MapsGen.MapBiomNumber = 0;
		}
		KGUI.SetControlText("MyMaps.page2.info.map_biom.txt_type", Localize.GetText("key110" + MapsGen.MapBiomNumber, null));
	}

	private void ChangeMapTime(bool next)
	{
		int num = this._MapTime + ((!next) ? -1 : 1);
		if (num >= 0 && num < this._MapTimes.Length)
		{
			this._MapTime = num;
			KGUI.SetControlText("MyMaps.page2.info.map_time.txt_type", Localize.GetText("MAP_TIME_" + this._MapTimes[this._MapTime], null));
		}
	}

	public void RefreshStartFastGame(int scroll)
	{
		this._FastGameType += scroll;
		this._FastGameType = Mathf.Clamp(this._FastGameType, -1, this._MapGameTypes.Length - 1);
		this._FastGameMaps = new List<Maps.StandartMap>();
		if (this._FastGameType != -1)
		{
			foreach (SecuredValue<Maps.StandartMap> securedValue in Maps.StandartMaps)
			{
				if (securedValue.Value.Game == this._MapGameTypes[this._FastGameType])
				{
					this._FastGameMaps.Add(securedValue.Value);
				}
			}
		}
		KGUI.SetNodes("Start.page1_fast_game.button_play", true, false);
		KGUI.SetControlText("Start.page1_fast_game.button_play.txt_title", Localize.GetText((this._FastGameMaps.Count != 0) ? "START_NEXT" : "START_PLAY", null));
		KGUI.SetNodes("Start.page1_fast_game.game_types", false, false);
		KGUI.SetNodes("Start.page1_fast_game.game_types", true, false);
		foreach (object obj in KGUI.FindNode("Start.page1_fast_game.game_types", false))
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(this._FastGameType != -1 && transform.name == this._MapGameTypes[this._FastGameType].ToString().ToLower());
		}
		if (this._FastGameType == -1)
		{
			KGUI.SetNodes("Start.page1_fast_game.game_types.random", true, false);
		}
		if (this._FastGameType != -1)
		{
			KGUI.SetControlText("Start.page1_fast_game.txt_game_type", Localize.GetText("GAME_TYPE_" + this._MapGameTypes[this._FastGameType], null));
			KGUI.SetControlText("Start.page1_fast_game.txt_description", Localize.GetText("START_DESCRIPTION_" + this._MapGameTypes[this._FastGameType], null));
		}
		else
		{
			KGUI.SetControlText("Start.page1_fast_game.txt_game_type", Localize.GetText("GAME_TYPE_RANDOM", null));
			KGUI.SetControlText("Start.page1_fast_game.txt_description", Localize.GetText("START_DESCRIPTION_RANDOM", null));
		}
	}

	private void RefreshStartSelectMap(int scroll)
	{
		this._FastGameMap += scroll;
		this._FastGameMap = Mathf.Clamp(this._FastGameMap, 0, this._FastGameMaps.Count - 1);
		Maps.StandartMap standartMap = this._FastGameMaps[this._FastGameMap];
		KGUI.SetControlText("FastGame.txt_map_name", Localize.GetText(standartMap.Name, null));
		KGUI.SetControlText("FastGame.txt_map_description2", Localize.GetText(standartMap.Description, null));
		KGUI.SetControlSprite(KGUI.FindNode("FastGame.map_image", false), standartMap.SpriteName, 0f);
	}

	private IEnumerator BattleResultProcess(int score)
	{
		UnityEngine.Debug.Log("---- score " + score);
		bool isPremium = ProfileINI.GetPurchaseValue(StorePurchase.MORE_EXPERIENCE) != 0;
		int premiumScore = 0;
		if (isPremium)
		{
			premiumScore = score;
		}
		int curExp = ProfileINI.experience;
		int oldExp = curExp;
		int actualExp = curExp + score + premiumScore;
		int add_gold = 0;
		if (GameType.IsHungerGamesMode)
		{
			add_gold = HG_WorkController.golds * ((!isPremium) ? 1 : 2);
		}
		else if (GameType.IsArcadeMode)
		{
			add_gold = score * ((!isPremium) ? 1 : 2);
		}
		else if (GameType.IsHideSeek)
		{
			add_gold = score / ((!isPremium) ? 2 : 1);
		}
		else
		{
			add_gold = score / ((!isPremium) ? 2 : 1);
		}
		ProfileINI.money[0] += add_gold;
		for (;;)
		{
			int level = ProfileINI.LevelExp(curExp);
			int expForCurLevel = ProfileINI.ExpForLevel(level);
			int expForNextLevel = ProfileINI.ExpForLevel(level + 1);
			KGUI.SetControlText("battle_result.txt_exp_premium", string.Empty + Mathf.Max(curExp - oldExp, score));
			if (GameType.IsHungerGamesMode || GameType.IsArcadeMode)
			{
				KGUI.SetControlText("battle_result.txt_war_points_premium", add_gold.ToString());
			}
			else
			{
				KGUI.SetControlText("battle_result.txt_war_points_premium", string.Empty + add_gold);
			}
			KGUI.SetControlText("battle_result.txt_level_exp", curExp + " / " + expForNextLevel);
			KGUI.SetControlSprite(KGUI.FindNode("battle_result.cur_level", false), "level_" + level, 0f);
			KGUI.SetControlSprite(KGUI.FindNode("battle_result.next_level", false), "level_" + Mathf.Min(level + 1, 30), 0f);
			KGUI.FindNode("battle_result.bar_cur", false).GetComponent<UISprite>().fillAmount = (float)(curExp - expForCurLevel) / (float)(expForNextLevel - expForCurLevel);
			KGUI.FindNode("main_menu.exp_bar_cur", false).GetComponent<UISprite>().fillAmount = (float)(curExp - expForCurLevel) / (float)(expForNextLevel - expForCurLevel);
			if (curExp == actualExp)
			{
				break;
			}
			yield return new WaitForSeconds(0.05f);
			curExp += level;
			if (curExp > actualExp)
			{
				curExp = actualExp;
			}
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		}
		bool isNewLevel = ProfileINI.LevelExp(oldExp) != ProfileINI.LevelExp(actualExp);
		ProfileINI.experience = actualExp;
		if (!isNewLevel)
		{
			yield break;
		}
		this.SetMenu(Menu.LevelUp, true, null, null);
		yield break;
	}

	public void OpenChest(List<int> items, int chest_id)
	{
		this.HideMenu();
		this._CurMenu = Menu.InventarySystem;
		this.HgWorkController.OpenChest(items, chest_id);
	}

	private IEnumerator ShowPlayerProfileProcess(string viewerID)
	{
		KGUI.SetControlText("main_menu.txt_title", Localize.GetText("MAIN_MENU_PROFILE", null));
		KGUI.SetNodes("Profile.Signs.Premium", false, false);
		KGUI.SetNodes("Profile.Signs.BadPlayer", false, false);
		KGUI.SetNodes("Profile.Signs.CommunityMember", false, false);
		KGUI.SetControlText("Profile.txt_title", "...");
		KGUI.SetNodes("Profile.stat_block_2", true, false);
		KGUI.SetControlText("Profile.stat_1_1.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_1_2.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_1_3.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_1_4.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_2_1.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_2_2.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_2_3.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_2_4.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_3_1.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_3_2.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_3_3.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_4_1.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_4_2.txt_value", string.Empty);
		KGUI.SetControlText("Profile.stat_4_3.txt_value", string.Empty);
		KGUI.SetControlText("Profile.txt_info", string.Empty);
		KGUI.SetControlText("Profile.txt_level", string.Empty);
		KGUI.SetControlText("Profile.experience.txt_level_exp", string.Empty);
		KGUI.SetControlSprite(KGUI.FindNode("Profile.icon_level", false), "level_1", 0f);
		KGUI.SetControlSprite(KGUI.FindNode("Profile.experience.cur_level", false), "level_1", 0f);
		KGUI.SetControlSprite(KGUI.FindNode("Profile.experience.next_level", false), "level_1", 0f);
		KGUI.FindNode("Profile.experience.bar_cur", false).GetComponent<UISprite>().fillAmount = 0f;
		Dictionary<string, string> profileData = new Dictionary<string, string>();
		yield return base.StartCoroutine(this.GetSmallProfileProcess(viewerID, profileData));
		if (profileData.Count == 0)
		{
			yield break;
		}
		string playerName = MainMenu.FixCollorName(profileData["name"]);
		int wins = int.Parse(profileData["wins"]);
		int kills = int.Parse(profileData["kills"]);
		int deaths = int.Parse(profileData["deaths"]);
		int blocksPlaced = int.Parse(profileData["blocks_placed"]);
		int blocksDestroyed = int.Parse(profileData["blocks_destroyed"]);
		int buildScore = int.Parse(profileData["build_score"]);
		int exp = int.Parse(profileData["exp"]);
		int hg_kill = int.Parse(profileData["hg_kill"]);
		int hg_death = int.Parse(profileData["hg_death"]);
		int hg_win = int.Parse(profileData["hg_win"]);
		int run_win = int.Parse(profileData["run_win"]);
		int run_death = int.Parse(profileData["run_death"]);
		DateTime time = DateTime.Parse(profileData["reg_date"].ToString());
		string startPlayDate = string.Concat(new object[]
		{
			time.Day,
			" ",
			Localize.GetMonth(time.Month, Localize.MounthCase.ACCUSATIVE),
			" ",
			time.Year
		});
		KGUI.SetControlText("Profile.txt_title", MainMenu.FixCollorName(playerName));
		KGUI.SetControlText("Profile.stat_1_1.txt_value", string.Empty + kills);
		KGUI.SetControlText("Profile.stat_1_2.txt_value", string.Empty + deaths);
		KGUI.SetControlText("Profile.stat_1_3.txt_value", string.Empty + ((float)kills / (float)Mathf.Max(deaths, 1)).ToString("f2"));
		KGUI.SetControlText("Profile.stat_1_4.txt_value", string.Empty + wins);
		KGUI.SetControlText("Profile.stat_2_1.txt_value", string.Empty + hg_kill);
		KGUI.SetControlText("Profile.stat_2_2.txt_value", string.Empty + hg_death);
		KGUI.SetControlText("Profile.stat_2_3.txt_value", string.Empty + ((float)hg_kill / (float)Mathf.Max(hg_death, 1)).ToString("f2"));
		KGUI.SetControlText("Profile.stat_2_4.txt_value", string.Empty + hg_win);
		KGUI.SetControlText("Profile.stat_3_1.txt_value", string.Empty + blocksPlaced);
		KGUI.SetControlText("Profile.stat_3_2.txt_value", string.Empty + blocksDestroyed);
		KGUI.SetControlText("Profile.stat_3_3.txt_value", string.Empty + buildScore);
		KGUI.SetControlText("Profile.txt_info", Localize.GetText("PROFILE_PLAY1", null) + startPlayDate);
		KGUI.SetControlText("Profile.stat_4_1.txt_value", string.Empty + run_win);
		KGUI.SetControlText("Profile.stat_4_2.txt_value", string.Empty + run_death);
		KGUI.SetControlText("Profile.stat_4_3.txt_value", string.Empty + ((float)run_win / (float)Mathf.Max(run_death, 1)).ToString("f2"));
		int level = ProfileINI.LevelExp(exp);
		int expForCurLevel = ProfileINI.ExpForLevel(level);
		int expForNextLevel = ProfileINI.ExpForLevel(level + 1);
		KGUI.SetControlText("Profile.txt_level", Localize.GetText("LEVEL_" + level, null));
		KGUI.SetControlText("Profile.experience.txt_level_exp", exp + " / [A0A083]" + expForNextLevel);
		KGUI.SetControlSprite(KGUI.FindNode("Profile.icon_level", false), "level_" + level, 0f);
		KGUI.SetControlSprite(KGUI.FindNode("Profile.experience.cur_level", false), "level_" + level, 0f);
		KGUI.SetControlSprite(KGUI.FindNode("Profile.experience.next_level", false), "level_" + Mathf.Min(level + 1, 30), 0f);
		KGUI.FindNode("Profile.experience.bar_cur", false).GetComponent<UISprite>().fillAmount = (float)(exp - expForCurLevel) / (float)(expForNextLevel - expForCurLevel);
		yield break;
	}

	private IEnumerator GetPremium(string viewerID)
	{
		WWWForm form = new WWWForm();
		form.AddField("viewer_id", viewerID);
		WWW phpLoad = new WWW(SettingsManager.ServerURL[0] + "GetPremium.php", form);
		yield return phpLoad;
		UnityEngine.Debug.Log(phpLoad.text);
		if (phpLoad.text == "PREMIUM_ON")
		{
			KGUI.SetNodes("Profile.Signs.Premium", true, false);
		}
		else
		{
			KGUI.SetNodes("Profile.Signs.Premium", false, false);
		}
		yield break;
	}

	private IEnumerator GetBanCount(string viewerID)
	{
		WWWForm form = new WWWForm();
		form.AddField("PlayerID", viewerID);
		WWW phpLoad = new WWW(SettingsManager.ServerURL[2] + "GetBanCount.php", form);
		yield return phpLoad;
		UnityEngine.Debug.Log(phpLoad.text);
		if (phpLoad.text != "0")
		{
			KGUI.SetNodes("Profile.Signs.BadPlayer", true, false);
		}
		else
		{
			KGUI.SetNodes("Profile.Signs.BadPlayer", false, false);
		}
		yield break;
	}

	private IEnumerator GetSmallProfileProcess(string viewerID, Dictionary<string, string> profileData)
	{
		WWWForm getSmallProfileForm = new WWWForm();
		getSmallProfileForm.AddField("viewer_id", viewerID);
		WWW getSmallProfile = new WWW(SettingsManager.ServerURL[0] + "GetProfileSmall.php", getSmallProfileForm);
		yield return getSmallProfile;
		UnityEngine.Debug.Log(getSmallProfile.text);
		if (getSmallProfile.error != null || !getSmallProfile.text.StartsWith("OK"))
		{
			UnityEngine.Debug.Log("Error: " + ((getSmallProfile.error == null) ? string.Empty : getSmallProfile.error) + ((getSmallProfile.text == null) ? string.Empty : getSmallProfile.text));
			yield break;
		}
		string[] result = getSmallProfile.text.Substring(3).Split(new char[]
		{
			','
		});
		for (int i = 0; i < result.Length; i += 2)
		{
			string key = result[i];
			string value = result[i + 1];
			UnityEngine.Debug.Log(key);
			profileData[key] = value;
		}
		yield return base.StartCoroutine(this.GetPremium(viewerID));
		yield return base.StartCoroutine(this.GetBanCount(viewerID));
		yield return base.StartCoroutine(Community.Instance.GetMemberCommunity(viewerID));
		yield break;
	}

	private IEnumerator ShowRatingsProcess(int category)
	{
		this.ShowLoading("LOADING_WAIT", string.Empty);
		KGUI.SetControlText("main_menu.txt_title", Localize.GetText("RATINGS_TITLE", null));
		KGUI.ResizeGrid("main_menu_ratings.grid_list", 0, delegate(GameObject slot, int index)
		{
		}, string.Empty);
		KGUI.SetControlText("main_menu_ratings.txt_title2", Localize.GetText("RATINGS_CATEGORY_" + category, null));
		KGUI.FindNode("main_menu_ratings.ibtn_icon_03", false).GetComponent<UIImageButton>().isPressed = (category == 0);
		KGUI.FindNode("main_menu_ratings.ibtn_icon_02", false).GetComponent<UIImageButton>().isPressed = (category == 1);
		KGUI.FindNode("main_menu_ratings.ibtn_icon_01", false).GetComponent<UIImageButton>().isPressed = (category == 2);
		KGUI.FindNode("main_menu_ratings.ibtn_icon_04", false).GetComponent<UIImageButton>().isPressed = (category == 3);
		WWWForm getRatingsForm = new WWWForm();
		getRatingsForm.AddField("user_id", VKAPI.INSTANCE._viewerId);
		getRatingsForm.AddField("category", category);
		WWW getRatings = new WWW(SettingsManager.ServerURL[0] + "GetRating.php", getRatingsForm);
		yield return getRatings;
		if (getRatings.error == null)
		{
			if (getRatings.text.StartsWith("OK"))
			{
				string[] lines = getRatings.text.Substring(2).Split(new char[]
				{
					'\n'
				});
				int playerIndex = -1;
				for (int i = 0; i < lines.Length - 1; i++)
				{
					string[] topData = lines[i].Split(new char[]
					{
						'\r'
					});
					if (topData[1] == VKAPI.INSTANCE._viewerId)
					{
						playerIndex = i;
						break;
					}
				}
				Dictionary<string, string> profileData = new Dictionary<string, string>();
				if (playerIndex == -1)
				{
					yield return base.StartCoroutine(this.GetSmallProfileProcess(VKAPI.INSTANCE._viewerId, profileData));
				}
				KGUI.ResizeGrid("main_menu_ratings.grid_list", lines.Length - 1 + ((playerIndex != -1) ? 0 : 1), delegate(GameObject slot, int index)
				{
					int num = -1;
					string[] array = new string[3];
					string checkName;
					int num2;
					if (index < lines.Length - 1)
					{
						num = index + 1;
						string[] array2 = lines[index].Split(new char[]
						{
							'\r'
						});
						checkName = array2[0];
						array[0] = array2[2];
						array[1] = array2[3];
						array[2] = array2[4];
						num2 = ProfileINI.LevelExp(int.Parse(array2[5]));
					}
					else
					{
						playerIndex = index;
						string[] array3 = lines[lines.Length - 1].Split(new char[]
						{
							'\r'
						});
						if (array3.Length < category)
						{
							num = int.Parse(array3[category]);
							checkName = ProfileINI.nickname;
							num2 = ProfileINI.level;
							array[0] = "N/A";
							array[1] = "N/A";
							array[2] = "N/A";
							if (profileData.Count != 0)
							{
								if (category == 0)
								{
									array[0] = profileData["kills"];
									array[1] = profileData["deaths"];
									array[2] = profileData["wins"];
								}
								else if (category == 1)
								{
									array[0] = profileData["build_score"];
									array[2] = profileData["blocks_placed"];
									array[1] = profileData["blocks_destroyed"];
								}
							}
						}
						else
						{
							checkName = ProfileINI.nickname;
							num2 = ProfileINI.level;
							array[0] = "0";
							array[1] = "0";
							array[2] = "0";
						}
					}
					if (category == 0 || category == 3)
					{
						array[2] = ((float)int.Parse(array[0]) / (float)Mathf.Max(int.Parse(array[1]), 1)).ToString("f2");
					}
					else if (category == 1)
					{
						string text = array[0];
						array[0] = array[2];
						array[2] = text;
					}
					KGUI.SetNodes("main_menu_ratings.grid_list." + index + ".stat_block_1", category == 0, false);
					KGUI.SetNodes("main_menu_ratings.grid_list." + index + ".stat_block_2", category == 1, false);
					KGUI.SetNodes("main_menu_ratings.grid_list." + index + ".stat_block_3", category == 2, false);
					KGUI.SetNodes("main_menu_ratings.grid_list." + index + ".stat_block_4", category == 3, false);
					KGUI.SetControlText("main_menu_ratings.grid_list." + index + ".txt_nickname", MainMenu.FixCollorName(checkName));
					KGUI.SetControlText(string.Concat(new object[]
					{
						"main_menu_ratings.grid_list.",
						index,
						".stat_",
						category + 1,
						"_1.txt_value"
					}), array[0]);
					KGUI.SetControlText(string.Concat(new object[]
					{
						"main_menu_ratings.grid_list.",
						index,
						".stat_",
						category + 1,
						"_2.txt_value"
					}), array[1]);
					KGUI.SetControlText(string.Concat(new object[]
					{
						"main_menu_ratings.grid_list.",
						index,
						".stat_",
						category + 1,
						"_3.txt_value"
					}), array[2]);
					KGUI.SetNodes("main_menu_ratings.grid_list." + index + ".place_1", num == 1, false);
					KGUI.SetNodes("main_menu_ratings.grid_list." + index + ".place_2", num == 2, false);
					KGUI.SetNodes("main_menu_ratings.grid_list." + index + ".place_3", num == 3, false);
					KGUI.SetNodes("main_menu_ratings.grid_list." + index + ".place_other", num > 3, false);
					if (num > 3)
					{
						KGUI.SetControlText("main_menu_ratings.grid_list." + index + ".place_other.txt_value", string.Empty + num);
					}
					KGUI.SetNodes("main_menu_ratings.grid_list." + index + ".selection_border", index == playerIndex, false);
					KGUI.SetControlSprite(slot.transform.Find("level_icon"), "level_" + num2, 0f);
				}, string.Empty);
			}
			else
			{
				UnityEngine.Debug.Log("Get error: " + getRatings.text);
			}
		}
		else
		{
			UnityEngine.Debug.Log("Get error: " + getRatings.error);
		}
		yield return 0;
		this.HideLoading();
		yield break;
	}

	public void AddVoiceHighlight(PlayerNode playerNode, Transform buttonNode)
	{
		if (playerNode == null || buttonNode == null)
		{
			return;
		}
		this.RefreshVoiceButton(playerNode, buttonNode);
		UIImageButton button = buttonNode.GetComponent<UIImageButton>();
		this._VoiceHighlights.RemoveAll((KeyValuePair<PlayerNode, UIImageButton> x) => x.Key == playerNode || x.Value == button);
		this._VoiceHighlights.Add(new KeyValuePair<PlayerNode, UIImageButton>(playerNode, button));
	}

	private IEnumerator VoiceHighlightProcess()
	{
		for (;;)
		{
			for (int i = 0; i < this._VoiceHighlights.Count; i++)
			{
				KeyValuePair<PlayerNode, UIImageButton> kv = this._VoiceHighlights[i];
				if (kv.Key == null || kv.Value == null || !kv.Value.gameObject.activeInHierarchy)
				{
					this._VoiceHighlights.RemoveAt(i);
					i--;
				}
				else
				{
					string spriteName = kv.Key.Voice ? "speak_disable" : "speak_off_(update)";
					if (kv.Key.Voice && DateTime.Now - kv.Key.LastVoice < TimeSpan.FromSeconds(1.0))
					{
						spriteName = "speak_on_over";
					}
					if (kv.Value.normalSprite != spriteName)
					{
						kv.Value.normalSprite = spriteName;
						kv.Value.hoverSprite = spriteName;
						kv.Value.SendMessage("UpdateImage");
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	public void UseBeckButton()
	{
		if (this._CurMenu == Menu.Start)
		{
			if (KGUI.FindNode("FastGame", false).gameObject.activeInHierarchy)
			{
				KGUI.SetNodes("FastGame", false, false);
				KGUI.SetNodes("Start.page1_fast_game", true, false);
				this.RefreshStartFastGame(0);
			}
			else
			{
				KGUI.SetControlText("main_menu.txt_title", Localize.GetText("START_MENU", null));
				KGUI.SetNodes("Start.page1_start", true, false);
				KGUI.SetNodes("Start.page1_fast_game", false, false);
				KGUI.SetNodes("FastGame", false, false);
				KGUI.SetNodes("main_menu.button_back", false, false);
				KGUI.SetNodes("Start.Canvas", true, false);
				Offers.Instance.SetOffers();
			}
		}
		else if (this._CurMenu == Menu.FastGame && SceneManager.GetActiveScene().name == "Menu")
		{
			this.SwitchMenu(Menu.Start, null, null);
			KGUI.SetNodes("Start.page1_start", false, false);
			KGUI.SetNodes("Start.page1_fast_game", true, false);
			KGUI.SetNodes("main_menu.button_back", true, false);
			KGUI.SetControlText("main_menu.txt_title", Localize.GetText("START_FAST_GAME", null));
			KGUI.SetNodes("Start.Canvas", false, false);
			this.RefreshStartFastGame(0);
		}
		else if (this._CurMenu == Menu.Shop && SceneManager.GetActiveScene().name == "Game")
		{
			this.SwitchMenu(Menu.Inventory, this._InventoryTab, null);
		}
		else if (this._CurMenu == Menu.Bank && SceneManager.GetActiveScene().name == "Game")
		{
			this.SwitchMenu(Menu.Shop, this._ShopTab, null);
		}
		else if (SceneManager.GetActiveScene().name == "Menu")
		{
			this.SwitchMenu(Menu.Start, null, null);
		}
		else
		{
			this.HideMenu();
		}
	}

	private void RefreshVoiceButton(PlayerNode playerNode, Transform buttonNode)
	{
		if (playerNode == null || buttonNode == null)
		{
			return;
		}
		UIImageButton component = buttonNode.GetComponent<UIImageButton>();
		component.pressedSprite = ((!playerNode.Voice) ? "speak_disable" : "speak_off_(update)");
		component.hoverSprite = (playerNode.Voice ? "speak_disable" : "speak_off_(update)");
		component.normalSprite = (playerNode.Voice ? "speak_disable" : "speak_off_(update)");
		component.SendMessage("UpdateImage");
	}

	public void SaveMap(Action endCallback = null)
	{
		if (App.Instance.Settings.isWatch || !Level.Instance.IsAdmin(null) || !GameType.CanSaveMap())
		{
			if (endCallback != null)
			{
				endCallback();
			}
			return;
		}
		this.SwitchMenu(Menu.SaveMapQuestion, null, endCallback);
	}

	public void SetMoneyValue(string nodeName, Currency currency, string value = null)
	{
		if (currency == Currency.Gold)
		{
			UILabel component = KGUI.FindNode(nodeName, false).GetComponent<UILabel>();
			component.text = string.Empty + (value ?? ProfileINI.money[(int)currency].ToString());
			component.effectColor = new Color32(0, 0, 0, byte.MaxValue);
			component.effectStyle = UILabel.Effect.Outline;
			component.effectDistance = Vector2.one;
			component.color = new Color32(248, 197, 60, byte.MaxValue);
		}
	}

	private void RefreshSelectBuildIcon()
	{
		List<StorePurchase> inpack = WorldGameObjectX.Instance.GetPurchasePack();
		if (inpack.Count == 0)
		{
			this.StarHG();
			return;
		}
		KGUI.ResizeGrid("select_hg_pack.grid_weapons", inpack.Count, delegate(GameObject slot, int index)
		{
			slot.transform.Find("txt_title").GetComponent<UILabel>().text = Localize.GetText("PURCHASE_" + Store.Purchases[inpack[index]].Name, null);
			KGUI.SetControlSprite(slot.transform.Find("icon"), Store.Purchases[inpack[index]].Icon, 100f);
			slot.transform.Find("selected").gameObject.SetActive(this.SelectedBuildItem == inpack[index]);
		}, "select_hg_pack");
	}

	public bool IsAllInclusive
	{
		get
		{
			return ProfileINI.all_inclusive;
		}
	}

	public void SetKickReason(string kr)
	{
		MainMenu.kickReason = kr;
	}

	public void CheckKickReason()
	{
		if (MainMenu.kickReason != string.Empty)
		{
			this.ShowHint(MainMenu.kickReason, false);
			MainMenu.kickReason = string.Empty;
		}
	}

	public void StarHG()
	{
		this.HideMenu();
		if (this.SelectedBuildItem != StorePurchase.NONE)
		{
			WorldGameObjectX.Instance.photonView.RPC("SetSelectedPack", PhotonTargets.All, new object[]
			{
				(int)this.SelectedBuildItem
			});
		}
		base.StartCoroutine(this.StartHungryGamePlay());
	}

	public IEnumerator StartHungryGamePlay()
	{
		KGUI.FindNode("hud", false).gameObject.SetActive(false);
		KGUI.SetNodes(this.hud_tag + ".crosshair", false, false);
		KGUI.SetControlText(this.hud_tag + ".battle.txt_time", string.Empty);
		base.StartCoroutine(this.HgWorkController.UpdateInventory());
		this._CurMenu = Menu.InventarySystem;
		yield return new WaitForSeconds(1f);
		this._CurMenu = Menu.None;
		this.HideMenu();
		if (MainMenu.Instance.IsShowHint)
		{
			MainMenu.Instance.HideHint();
		}
		TeamBattle.Instance.PhotonEventWork(4, "start", 1);
		KGUI.FindNode("hud", false).gameObject.SetActive(true);
		KGUI.SetNodes(this.hud_tag + ".crosshair", true, false);
		this.SetCrosshairInfo(null, MainMenu.CrosshairAction.None);
		this.HideMenu();
		TeamBattle.Instance.RefreshTabMenu(false);
		MainMenu.DisableMouseWork();
		if (HG_WorkController.hgstatus == GameStatus.GS_WAIT)
		{
			TeamBattle.Instance.StartPlay(1);
		}
		else
		{
			HG_WorkController.isT = true;
			TeamBattle.Instance.StartPlay(0);
		}
		if (WorldGameObjectX.Instance.MainPlayer == null)
		{
			UnityEngine.Debug.Log("MainPlayer is null");
		}
		PlayerNetwork player = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>();
		WorldGameObjectX.Instance.Respawn();
		yield break;
	}

	public static string FixCollorName(string checkName)
	{
		if (checkName.Length < 7)
		{
			return checkName;
		}
		if (checkName[0] == '[' && checkName[7] == ']')
		{
			return checkName.Substring(8);
		}
		return checkName;
	}

	private void ChekMapData()
	{
		using (MD5 md = MD5.Create())
		{
			string hash = ProtectHash.GetHash(md, this.InputPassword);
			if (this.MapPassword == hash)
			{
				this.SetMenu(Menu.InputText2, false, null, null);
				ManagerServerList.ShowWindow = false;
				this.JoinGame(ServerList.Instance._SelectedRoom);
			}
			else
			{
				base.StartCoroutine(this.ShowPasswordError("key1003"));
			}
		}
	}

	private bool CheckMapCanLoad()
	{
		foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
		{
			if (roomInfo.name == ServerList.Instance._SelectedRoom.name)
			{
				return !roomInfo.customProperties.ContainsKey("password") || ((string)roomInfo.customProperties["password"]).Length <= 0;
			}
		}
		return true;
	}

	private IEnumerator SaveSettingToBase()
	{
		UnityEngine.Debug.Log("SaveSettingToBase");
		WWWForm saveSettinfForm = new WWWForm();
		saveSettinfForm.AddField("viewer_id", VKAPI.INSTANCE._viewerId);
		saveSettinfForm.AddField("ply_setting", string.Concat(new object[]
		{
			ProfileINI.mouse_sens,
			";",
			ProfileINI.sound_volume,
			";",
			ProfileINI.ambient_volume,
			";",
			(!ProfileINI.showNonFreeServer) ? 0 : 1
		}));
		saveSettinfForm.AddField("auth_key", VKAPI.INSTANCE._authKey);
		WWW saveSettingRequest = new WWW(SettingsManager.ServerURL[0] + "set_setting.php", saveSettinfForm);
		yield return saveSettingRequest;
		UnityEngine.Debug.Log(saveSettingRequest.text);
		yield break;
	}

	private IEnumerator ShowNameEnterError(string error)
	{
		KGUI.SetNodes("change_name.txt_name_busy", true, false);
		KGUI.SetControlText("change_name.txt_name_busy", Localize.GetText(error, "NULL TEXT"));
		yield return new WaitForSeconds(3f);
		KGUI.SetNodes("change_name.txt_name_busy", false, false);
		yield break;
	}

	private IEnumerator ShowPasswordError(string error)
	{
		KGUI.SetNodes("input_text2.error", true, false);
		KGUI.SetControlText("input_text2.error", Localize.GetText(error, "NULL TEXT"));
		yield return new WaitForSeconds(3f);
		KGUI.SetNodes("input_text2.error", false, false);
		yield break;
	}

	private IEnumerator DisableInputTimedFrame()
	{
		CameraController.Instance.DisableInput = true;
		yield return new WaitForEndOfFrame();
		CameraController.Instance.DisableInput = false;
		yield break;
	}

	public static MainMenu Instance;

	public static bool isShowAllInMsg;

	private string hud_tag = "hud_mobile";

	public AudioSource Audio;

	public int _FastGameType = -1;

	private int _FastGameMap;

	private List<Maps.StandartMap> _FastGameMaps;

	private int _MapGameType;

	private GameINI.GameType[] _MapGameTypes = new GameINI.GameType[1];

	private List<GameINI.GameType> _IgnoreMapGameTypes = new List<GameINI.GameType>
	{
		GameINI.GameType.HUNGER_GAMES,
		GameINI.GameType.ARCADE,
		GameINI.GameType.HIDE_SEEK,
		GameINI.GameType.CTF
	};

	private bool _MapDestroyable;

	private int _MapSize;

	private GameINI.MapSize[] _MapSizes = new GameINI.MapSize[]
	{
		GameINI.MapSize.SMALL,
		GameINI.MapSize.TINY,
		GameINI.MapSize.MEDIUM,
		GameINI.MapSize.LARGE
	};

	private int _MapType;

	private GameINI.MapType[] _MapTypes = new GameINI.MapType[]
	{
		GameINI.MapType.STANDART,
		GameINI.MapType.FLAT,
		GameINI.MapType.SAND,
		GameINI.MapType.OCEAN,
		GameINI.MapType.ISLAND,
		GameINI.MapType.SNOWLAND,
		GameINI.MapType.LAVA,
		GameINI.MapType.PLATFORM,
		GameINI.MapType.AUTUMN
	};

	private int _MapTime;

	private GameINI.MapTime[] _MapTimes = new GameINI.MapTime[]
	{
		GameINI.MapTime.DAY,
		GameINI.MapTime.NIGHT,
		GameINI.MapTime.SWITCH
	};

	private List<MainMenu.Map> _Maps = new List<MainMenu.Map>();

	private string _GetMapsScript;

	private Dictionary<string, string> _GetMapsFormData;

	private ArrayList _MapComments = new ArrayList();

	private int _MapCommentsStartIndex;

	private int _RecommendedShard;

	private string _CustomMapImageURL;

	private DateTime _BonusGemsEndTime = DateTime.MinValue;

	private int _BonusGemsSeconds = -1;

	private bool _BonusGemsShowSeparator;

	private Store.TabType _InventoryTab = Store.TabType.Decorations;

	private Dictionary<string, List<EntityType>> _CurInventoryItems = new Dictionary<string, List<EntityType>>();

	private Dictionary<string, List<BlockType>> _CurInventoryBlocks = new Dictionary<string, List<BlockType>>();

	private Store.TabType _ShopTab = Store.TabType.Decorations_Star;

	private StorePurchase _CurPurchase = StorePurchase.NONE;

	private Dictionary<string, List<StorePurchase>> _CurShopPurchases = new Dictionary<string, List<StorePurchase>>();

	private bool _ShopPurchaseItemInfo;

	private int _SelectedMapID = -2;

	private int _SelectedMapSlot;

	private string _SelectedMapOwner = string.Empty;

	private bool _SelectedMapNotVoted;

	private int _SelectedMapRating;

	private string _SelectedMapAuthor = string.Empty;

	private bool _SpeedEnabled = true;

	private bool _FlyEnabled = true;

	internal bool Flying;

	internal bool CinematicCamera;

	public static bool CleanScreen;

	private Menu _CurMenu;

	private Dictionary<Menu, List<string>> _MenuNodeNames = new Dictionary<Menu, List<string>>();

	private Dictionary<Menu, List<Action<object, object>>> _MenuInitializers = new Dictionary<Menu, List<Action<object, object>>>();

	private Menu[] _NonModalMenus;

	private Menu[] _CanToggleMenus;

	private Action[] _AskMenuActions;

	private Action<string> _InputTextAction;

	private static string kickReason = string.Empty;

	private int[] _BonusData;

	private Tablichka _CurPlate;

	private List<Gun> _SelectWeapons = new List<Gun>();

	public HG_WorkController HgWorkController;

	private List<KeyValuePair<PlayerNode, UIImageButton>> _VoiceHighlights = new List<KeyValuePair<PlayerNode, UIImageButton>>();

	private List<int> _TempBuildItemId = new List<int>();

	public StorePurchase SelectedBuildItem = StorePurchase.NONE;

	private Transform ContentLoadingText;

	public bool IsShowHint;

	public static bool IsCheatOn;

	public string ProtectPassword;

	public string MapPassword;

	public string InputPassword;

	private MainMenu.FavoritServer[] _FavoritServers;

	public MainMenu.BiomList Biom;

	private class Map
	{
		public string AuthorID;

		public string AuthorSlotID;

		public string MapID;

		public string Name;

		public string Rating;
	}

	private class Comment
	{
		public string Name;

		public string Text;

		public string ID;
	}

	public enum CrosshairAction
	{
		None,
		Take,
		Sit,
		Open = 4,
		Activate = 8,
		Write = 16,
		Draw = 32,
		Wheel = 64,
		Put = 128
	}

	private class FavoritServer
	{
		public int admin_id;

		public string server_name;

		public byte slot_id;
	}

	public enum BiomList
	{
		Standart,
		Autumn,
		Winter,
		Ocean,
		Islands,
		Desert
	}
}
