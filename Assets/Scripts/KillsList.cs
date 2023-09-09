using System;
using System.Collections.Generic;
using InventorySystem;
using Photon;
using UnityEngine;

public class KillsList : Photon.MonoBehaviour
{
	private void Awake()
	{
		KillsList._Instance = this;
		this._Grid = KGUI.FindNode("hud.kills_list.grid_messages", false).gameObject;
		KGUI.ResizeGrid("hud.kills_list.grid_messages", 0, delegate(GameObject slot, int index)
		{
		}, null);
	}

	private void Update()
	{
		if (this._Grid.activeInHierarchy)
		{
			this.RefreshAlpha();
		}
	}

	public static void AddKill(string killer, string victim, int gunID)
	{
		if (KillsList._Instance == null)
		{
			return;
		}
		KillsList._Instance.AddKillMessage(killer, victim, gunID);
	}

	private void AddKillMessage(string killer, string victim, int gunID)
	{
		if (!WorldGameObjectX.Instance.IsWorldGenerated || WorldGameObjectX.Instance.MainPlayer == null)
		{
			return;
		}
		Gun[] guns = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>().Guns;
		if (gunID < 0 || gunID >= guns.Length)
		{
			return;
		}
		string str = (TeamBattle.Instance.GetPlayerTeam(killer) != 1) ? "[0000FF]" : "[FF0000]";
		string str2 = (TeamBattle.Instance.GetPlayerTeam(victim) != 1) ? "[0000FF]" : "[FF0000]";
		this._Messages.Add(new KillsList.Message
		{
			StartTime = Time.time,
			Killer = str + killer,
			Victim = str2 + victim,
			GunID = gunID
		});
		while (this._Messages.Count > 5)
		{
			this._Messages.RemoveAt(0);
		}
		this.ActivateGrid(true);
		KGUI.ResizeGrid("hud.kills_list.grid_messages", this._Messages.Count, delegate(GameObject slot, int index)
		{
			KillsList.Message message = this._Messages[this._Messages.Count - index - 1];
			message.Widgets = slot.GetComponentsInChildren<UIWidget>(true);
			foreach (UIWidget uiwidget in message.Widgets)
			{
				if (uiwidget.name == "txt_killer")
				{
					uiwidget.GetComponent<UILabel>().text = message.Killer;
				}
				else if (uiwidget.name == "txt_victim")
				{
					uiwidget.GetComponent<UILabel>().text = message.Victim;
				}
				else if (uiwidget.transform.parent.name == "gun_image")
				{
					if (!GameType.IsHungerGamesMode)
					{
						KGUI.SetControlSprite(uiwidget.transform.parent, "weapon_" + guns[(message.GunID == -1) ? 0 : message.GunID].name, 0f);
					}
					else
					{
						KGUI.SetControlSprite(uiwidget.transform.parent, (gunID == 0) ? "weapon_0" : IS_Manager.GetItemById(gunID).IconName, 0f);
					}
				}
			}
		}, null);
		this.RefreshAlpha();
	}

	private void RefreshAlpha()
	{
		float num = 0f;
		if (this._Messages == null)
		{
			UnityEngine.Debug.Log("RefreshAlpha _Messages isNULL");
		}
		else
		{
			foreach (KillsList.Message message in this._Messages)
			{
				float num2 = Mathf.Clamp01(1f - (Time.time - message.StartTime - 5f) / 1f);
				foreach (UIWidget uiwidget in message.Widgets)
				{
					if (uiwidget != null)
					{
						uiwidget.alpha = num2;
					}
				}
				num = Mathf.Max(num, num2);
			}
		}
		if (num == 0f)
		{
			this.ActivateGrid(false);
		}
	}

	private void ActivateGrid(bool activate)
	{
		this._Grid.SetActive(activate);
		if (activate)
		{
			this._Grid.transform.Find("slot_prototype").gameObject.SetActive(false);
		}
	}

	public const int MaxMessages = 5;

	public const float DisappearDelay = 5f;

	public const float DisappearTime = 1f;

	private static KillsList _Instance;

	private List<KillsList.Message> _Messages = new List<KillsList.Message>();

	private GameObject _Grid;

	private class Message
	{
		public float StartTime;

		public string Killer;

		public string Victim;

		public int GunID;

		public UIWidget[] Widgets;
	}
}
