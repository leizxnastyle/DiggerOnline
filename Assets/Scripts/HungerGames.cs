using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using UnityEngine;

public class HungerGames : Deathmatch
{
	private void Start()
	{
		this._isSpawn = false;
		this.ServerStartGame();
	}

	private new void Update()
	{
		WorldGameObjectX.Instance.IEShowPlayerCount();
		if (HG_WorkController.hgstatus == GameStatus.GS_WAIT)
		{
			if (Time.time > this._lastcheck + (float)this._wait_time)
			{
				int num = 0;
				foreach (PlayerNode playerNode in WorldGameObjectX.Instance.PlayerList)
				{
					if (playerNode.Avatar != null)
					{
						num++;
					}
				}
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("WAIT_PLAYER_TO_START", null) + num + "/8");
				this._lastcheck = Time.time;
			}
		}
		else if (HG_WorkController.hgstatus == GameStatus.GS_PRE_START)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		}
		else if (HG_WorkController.hgstatus == GameStatus.GS_PRE_ARENA)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		}
	}

	public override void SpawnToGame()
	{
		Chat.SendInfoF(Localize.GetText("PlayerConnect1", null) + PhotonNetwork.playerName + Localize.GetText("PlayerConnect2", null), true);
		MainMenu.Instance.SelectedBuildItem = StorePurchase.NONE;
		MainMenu.Instance.SwitchMenu(Menu.SelectPack, true, null);
		UnityEngine.Debug.Log("------------------- SpawnToGame");
		this.OnChestStartWork();
		base.StartCoroutine(this.WaitPressPlayOrKick());
	}

	private IEnumerator WaitPressPlayOrKick()
	{
		yield return new WaitForSeconds(10f);
		if (!this._isSpawn && Info.Instance.GameMode == "HUNGER_GAMES")
		{
			MainMenu.Instance.StarHG();
		}
		yield break;
	}

	protected override void ServerStartGame()
	{
		this._GameTime = 5000f;
		this._GameEnded = false;
	}

	public override void RefreshTabMenu(bool open)
	{
		KGUI.SetNodes("deathmatch.button_observe", false, false);
		if (open && MainMenu.Instance.CurMenu != Menu.Deathmatch)
		{
			MainMenu.Instance.SwitchMenu(Menu.Deathmatch, null, null);
			return;
		}
		TeamBattle.Team[] array = (this._TeamsResult == null) ? this._Teams : this._TeamsResult;
		this._TeamsTabMenu = new TeamBattle.Team[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			this._TeamsTabMenu[i] = array[i].GetCopy();
		}
		KGUI.ResizeGrid("deathmatch.grid_players", this._TeamsTabMenu[1].Players.Count, delegate(GameObject slot, int index)
		{
			TeamBattle.Player player = this._TeamsTabMenu[1].Players[index];
			slot.transform.Find("txt_nickname").GetComponent<UILabel>().text = string.Empty + MainMenu.FixCollorName(player.Name);
			slot.transform.Find("txt_score").GetComponent<UILabel>().text = string.Empty + player.Score;
			KGUI.SetControlSprite(slot.transform.Find("level_icon"), "level_" + player.Level, 0f);
			slot.transform.FindChild("level_icon").gameObject.SetActive(false);
		}, "deathmatch");
	}

	public override bool IsTeammate(string playerName)
	{
		return this._GameEnded;
	}

	public override bool IsOpposite(string playerName)
	{
		return !this._GameEnded;
	}

	public override Vector3 GenerateSpawnPoint()
	{
		if (SpawnArrow.CurSpawn != null)
		{
			return new Vector3((float)((int)SpawnArrow.CurSpawn.transform.position.x), (float)((int)SpawnArrow.CurSpawn.transform.position.y), (float)((int)SpawnArrow.CurSpawn.transform.position.z));
		}
		WorldData worldData = bs._WorldGameObjectX.WorldData;
		Vector3 vector = new Vector3((float)UnityEngine.Random.Range(0, worldData.WidthInBlocks - 1), (float)worldData.DepthInBlocks, (float)UnityEngine.Random.Range(0, worldData.HeightInBlocks - 1)) + Vector3.one / 2f;
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, Vector3.down, out raycastHit, 1000f, 1 << LayerMask.NameToLayer("Terrain")))
		{
			return raycastHit.point + Vector3.up;
		}
		return vector;
	}

	[PunRPC]
	public override void AddToTeam(int teamIndex, string playerName, object[] playerData, PhotonMessageInfo info = null)
	{
		base.AddToTeam(teamIndex, playerName, playerData, info);
	}

	[PunRPC]
	protected override void EndGame(int winner)
	{
		this._GameEnded = true;
		this._GameTime = 20f;
	}

	[PunRPC]
	protected override void RestartGame()
	{
		base.RestartGame();
	}

	public override void OnDead(string killer, string victim, int score)
	{
		UnityEngine.Debug.Log("OnPlayerDead HGs");
		base.photonView.RPC("OnPlayerDead", PhotonTargets.MasterClient, new object[]
		{
			killer,
			victim,
			score
		});
	}

	public override void MainPlayerDeadProcess()
	{
		this.TEST_WinerHG();
	}

	[PunRPC]
	protected override void OnPlayerDead(string killer, string victim, int score)
	{
		base.OnPlayerDead(killer, victim, score);
	}

	public override void AddKill(string killer)
	{
		UnityEngine.Debug.Log("AddKill HGs");
		if (killer == HG_WorkController._player.PlayerName)
		{
			HG_WorkController.kills++;
			HG_WorkController.golds += 2;
			Chat.SendInfoF("+2" + Localize.GetText("HG_ADD_GOLD_GAME", null), false);
		}
	}

	[PunRPC]
	protected override void RemoveFromTeam(int teamIndex, string playerName, PhotonMessageInfo info = null)
	{
		base.RemoveFromTeam(teamIndex, playerName, info);
	}

	[PunRPC]
	public override void SetGameTime(float time)
	{
		base.SetGameTime(time);
	}

	[PunRPC]
	protected override void SetTeamData(int teamIndex, int teamScore)
	{
		base.SetTeamData(teamIndex, teamScore);
	}

	[PunRPC]
	public override void StartPlay(int teamIndex)
	{
		base.StartPlay(teamIndex);
	}

	[PunRPC]
	public override void UpdatePlayerData(int teamIndex, string playerName, object[] playerData)
	{
		base.UpdatePlayerData(teamIndex, playerName, playerData);
	}

	public override void PhotonEventWork(byte eventCode, object content, int senderId)
	{
		if (eventCode == 4)
		{
			string[] array = content.ToString().Split(new char[]
			{
				'_'
			});
			if (array[0] == "gi")
			{
				this.AddItemToChest((string)content);
			}
			else if (array[0] == "ai")
			{
				this.AddItemToInventory((string)content);
			}
			else if (array[0] == "gcifp")
			{
				this.AddOneItemToPlayer((string)content);
			}
			else if (array[0] == "ibag")
			{
				this.InstBag((string)content);
			}
			else if (array[0] == "start")
			{
				this._isSpawn = true;
			}
		}
		else if (eventCode == 5)
		{
			string[] array2 = content.ToString().Split(new char[]
			{
				'_'
			});
			if (array2[0] == "hgtime")
			{
				this.ShowTimeMsg((string)content);
			}
			else if (array2[0] == "spawn")
			{
				this.SpawnBattleTo((string)content);
			}
			else if (array2[0] == "goplay")
			{
				base.StartCoroutine("StartPlayBattle", (string)content);
			}
			else if (array2[0] == "winer")
			{
				base.StartCoroutine("WinerHG", (string)content);
			}
			else if (array2[0] == "toarena")
			{
				this.SpawnArenaTo((string)content);
			}
			else if (array2[0] == "goarena")
			{
				base.StartCoroutine("ArenaStart", (string)content);
			}
		}
		else
		{
			base.PhotonEventWork(eventCode, content, senderId);
		}
	}

	private void AddItemToChest(string str)
	{
		if (str != string.Empty)
		{
			string[] array = str.Split(new char[]
			{
				'_'
			});
			int num = int.Parse(array[1]);
			if (array[0] == "gi" && WorldGameObjectX.LevelChest.ContainsKey(num))
			{
				if (array[2] != string.Empty)
				{
					WorldGameObjectX.LevelChest[num].AddItem((from s in array[2].Split(new char[]
					{
						';'
					})
					select int.Parse(s)).ToList<int>());
				}
				else
				{
					WorldGameObjectX.LevelChest[num].AddItem(new List<int>());
				}
				if (HG_WorkController.curent_chest_open != -1 && HG_WorkController.curent_chest_open == num)
				{
					pnl_Inventory.pInventory.ShowChest(WorldGameObjectX.LevelChest[num].in_item);
				}
			}
		}
	}

	private void AddItemToInventory(string get_items)
	{
		string[] array = get_items.Split(new char[]
		{
			'_'
		});
		if (array[0] == "ai")
		{
			Chat.SendInfoF(HG_WorkController._player.PlayerName + " " + Localize.GetText("HG_GET_GUILD_SET_MSG", null) + Localize.GetText("PURCHASE_" + Store.Purchases[MainMenu.Instance.SelectedBuildItem].Name, null), false);
			List<int> list = (from s in array[2].Split(new char[]
			{
				';'
			})
			select int.Parse(s)).ToList<int>();
			foreach (int inv_id in list)
			{
				InventaryObjManager.AddItemToInventary(inv_id);
			}
		}
	}

	private void AddOneItemToPlayer(string get_items)
	{
		string[] array = get_items.Split(new char[]
		{
			'_'
		});
		if (array[0] == "gcifp")
		{
			int item_id = int.Parse(array[2]);
			int cell_id = int.Parse(array[3]);
			InventaryObjManager.inventary.PutItemToInventory(IS_Manager.GetItemById(item_id), cell_id);
		}
	}

	private void InstBag(string get_items)
	{
		string[] array = get_items.Split(new char[]
		{
			'_'
		});
		if (array[0] == "ibag")
		{
			this.DropBag(int.Parse(array[1]), int.Parse(array[2]), array[3], array[4]);
		}
	}

	private void DropBag(int chest_id, int chest_type, string item_in_id, string pos)
	{
		UnityEngine.Debug.Log("-------------- DropBag ");
		GameObject gameObject = (GameObject)Resources.Load("drop_beg", typeof(GameObject));
		if (gameObject != null)
		{
			List<int> list = (from s in pos.Split(new char[]
			{
				','
			})
			select int.Parse(s)).ToList<int>();
			Vector3 position = new Vector3((float)list[0], (float)list[1], (float)list[2]);
			GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(gameObject, position, base.transform.rotation);
			List<int> i = (from s in item_in_id.Split(new char[]
			{
				';'
			})
			select int.Parse(s)).ToList<int>();
			gameObject2.GetComponent<HG_Spawn_Beg>().Inst(chest_type, chest_id, i);
			WorldGameObjectX.LevelChest.Add(chest_id, gameObject2.GetComponent<HG_Spawn>());
		}
	}

	public void OnChestStartWork()
	{
		WorldGameObjectX.LevelChest = new Dictionary<int, HG_Spawn>();
		int num = 0;
		foreach (EntityBase entityBase in EntityBase.Entities)
		{
			if (entityBase.Type == EntityType.CHEST_STONE || entityBase.Type == EntityType.CHEST || entityBase.Type == EntityType.DARK_CASTLE_CHEST)
			{
				HG_Spawn_Chest hg_Spawn_Chest = (HG_Spawn_Chest)entityBase;
				hg_Spawn_Chest.chest_game_id = num;
				if (!WorldGameObjectX.LevelChest.ContainsKey(num))
				{
					WorldGameObjectX.LevelChest.Add(num, hg_Spawn_Chest);
				}
				else
				{
					WorldGameObjectX.LevelChest.Remove(num);
					WorldGameObjectX.LevelChest.Add(num, hg_Spawn_Chest);
				}
				hg_Spawn_Chest.SendInstToserver();
				num++;
			}
		}
	}

	private void ShowTimeMsg(string time)
	{
		string[] array = time.Split(new char[]
		{
			'_'
		});
		if (array[0] == "hgtime")
		{
			HG_WorkController.hgstatus = (GameStatus)int.Parse(array[2].ToString());
			if (array[2] == "1")
			{
				WorldGameObjectX.SetCustomPropertiesToHG();
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_Left_to_spawn", null) + array[1]);
			}
			else if (array[2] == "2")
			{
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_start_battle_after", null) + array[1]);
				CheatFinderManager.CheckStayPosition();
			}
			else if (array[2] == "3" && int.Parse(array[1]) <= 60)
			{
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_fight_in_the_arena", null) + array[1]);
			}
			else if (array[2] == "4")
			{
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_start_the_arena", null) + array[1]);
			}
		}
	}

	private void SpawnBattleTo(string tospawn)
	{
		this._isSpawn = true;
		string[] array = tospawn.Split(new char[]
		{
			'_'
		});
		if (array[0] == "spawn")
		{
			List<float> list = (from s in array[1].Split(new char[]
			{
				';'
			})
			select float.Parse(s)).ToList<float>();
			Vector3 pos = new Vector3(list[0], list[1] + 0.5f, list[2]);
			base.TeleportPlayerBattlePoint(pos);
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		}
	}

	private IEnumerator StartPlayBattle(string msg)
	{
		HG_WorkController.hgstatus = GameStatus.GS_START;
		KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_in_the_attack", null));
		WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(true);
		yield return new WaitForSeconds(3f);
		KGUI.SetControlText("hud.battle.txt_generic_tip", string.Empty);
		if (MainMenu.Instance.SelectedBuildItem != StorePurchase.NONE)
		{
			Chat.SendInfoF(HG_WorkController._player.PlayerName + " " + Localize.GetText("HG_GET_GUILD_MSG", null) + Localize.GetText("PURCHASE_" + Store.Purchases[MainMenu.Instance.SelectedBuildItem].Name, null), false);
		}
		Chat.SendInfoF("+2" + Localize.GetText("HG_ADD_GOLD_GAME", null), false);
		HG_WorkController.golds += 2;
		yield break;
	}

	private void SpawnArenaTo(string msg)
	{
		string[] array = msg.Split(new char[]
		{
			'_'
		});
		if (array[0] == "toarena")
		{
			float[] array2 = (from s in array[1].Split(new char[]
			{
				','
			})
			select float.Parse(s)).ToArray<float>();
			HG_WorkController.hgstatus = GameStatus.GS_PRE_ARENA;
			WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
			Boat[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(Boat)) as Boat[];
			foreach (Boat boat in array3)
			{
				if (boat.Player == WorldGameObjectX.Instance.MainPlayer)
				{
					boat.PlayerExitHG(false);
					break;
				}
			}
			foreach (EntityBase entityBase in EntityBase.Entities)
			{
				if (entityBase.Type == EntityType.HG_ARENA_SPAWN_POINT)
				{
					Vector3 hgspawnPosition = base.GetHGSpawnPosition(entityBase.transform.position);
					base.TeleportPlayerBattlePoint(hgspawnPosition);
					base.LookAtTarrget(entityBase.transform.position);
					break;
				}
			}
		}
	}

	private IEnumerator ArenaStart(string msg)
	{
		HG_WorkController.hgstatus = GameStatus.GS_ARENA;
		KGUI.SetControlText("hud.battle.txt_generic_tip", "Arena Is Start!!! Kill All!!!");
		WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(true);
		yield return new WaitForSeconds(3f);
		KGUI.SetControlText("hud.battle.txt_generic_tip", string.Empty);
		yield break;
	}

	private IEnumerator WinerHG(string msg)
	{
		yield return new WaitForSeconds(2f);
		string[] data = msg.Split(new char[]
		{
			'_'
		});
		if (data[0] == "winer")
		{
			try
			{
				int kills = HG_WorkController.kills * 5;
				KGUI.SetControlText("hud.battle.txt_generic_tip", Localize.GetText("hg_winner_is", null) + WorldGameObjectX.Instance.FindPlayerByViewerID(data[1]).Name);
				bool isWiner = WorldGameObjectX.Instance.FindPlayerByViewerID(data[1]).Name == HG_WorkController._player.PlayerName;
				if (isWiner)
				{
					HG_WorkController.golds += 15;
				}
				MainMenu.Instance.SwitchMenu(Menu.BattleResult, isWiner, (!isWiner) ? kills : (15 + kills));
				WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerInput>().SetMovement(false);
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				UnityEngine.Debug.Log("ERROR CHECK HG WINNER " + ex.StackTrace);
			}
			yield return new WaitForSeconds(3f);
		}
		yield break;
	}

	public void TEST_WinerHG()
	{
		TeamBattle.Instance.FastEndOn(1);
		int num = HG_WorkController.kills * 5;
		MainMenu.Instance.SwitchMenu(Menu.BattleResult, false, num);
	}

	private float _lastcheck;

	private int _wait_time = 2;

	private bool _isSpawn;

	public bool isCheckWeapon;
}
