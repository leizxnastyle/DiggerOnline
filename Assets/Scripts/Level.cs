using System;
using System.Collections.Generic;
using Photon;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
	public string[] Banned
	{
		get
		{
			return this._Banned.ToArray();
		}
	}

	public bool CanBuild
	{
		get
		{
			return this._CanBuild || GameType.BattleMode();
		}
	}

	public bool CanBuildZombieGame
	{
		get
		{
			return this._CanBuild;
		}
	}

	private void Awake()
	{
		Level.Instance = this;
	}

	private void Start()
	{
		ProfileINI.Save();
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		GC.WaitForPendingFinalizers();
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		EntityBase.Entities.Clear();
		Rail.ClearAttachedRails();
		if (App.Instance.Settings.isServer)
		{
			WorldGameObjectX.Instance.StartServer();
			if (App.Instance.Settings.isServerAdministrator)
			{
				this._AdministratorsZ.Add(PhotonNetwork.playerName);
			}
			this._Builders.Add(PhotonNetwork.playerName);
			this._CanBuild = true;
		}
		else
		{
			PhotonNetwork.isMessageQueueRunning = true;
			bool flag = false;
			foreach (PhotonPlayer photonPlayer in PhotonNetwork.otherPlayers)
			{
				if (photonPlayer.name == PhotonNetwork.playerName && (string)photonPlayer.customProperties["player_id"] == VKAPI.INSTANCE._viewerId)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				PhotonNetwork.LeaveRoom();
				MainMenu.Instance.ShowLoading("LOADING_LEAVING", string.Empty);
				SceneManager.LoadScene("Menu");
				return;
			}
			if ((int)PhotonNetwork.room.customProperties["game_type"] == 6 || (int)PhotonNetwork.room.customProperties["game_type"] == 8)
			{
				WorldGameObjectX.Instance.photonView.RPC("SendPlayerProperties", PhotonTargets.AllBuffered, new object[]
				{
					PhotonNetwork.playerName,
					ProfileINI.GetActualSkin(),
					VKAPI.INSTANCE._viewerId,
					false,
					PhotonNetwork.player,
					ProfileINI.level,
					ProfileINI.WeaponSkinData.Save()
				});
				WorldGameObjectX.Instance.StartSelfLoadMap((string)PhotonNetwork.room.customProperties["player_id"]);
			}
			else
			{
				WorldGameObjectX.Instance.photonView.RPC("SendPlayerProperties", PhotonTargets.AllBuffered, new object[]
				{
					PhotonNetwork.playerName,
					ProfileINI.GetActualSkin(),
					VKAPI.INSTANCE._viewerId,
					true,
					PhotonNetwork.player,
					ProfileINI.level,
					ProfileINI.WeaponSkinData.Save()
				});
			}
			base.photonView.RPC("GetActualData", PhotonTargets.MasterClient, new object[0]);
		}
	}

	[PunRPC]
	private void GetActualData(PhotonMessageInfo info)
	{
		base.photonView.RPC("SetActualData", info.sender, new object[]
		{
			this._AdministratorsZ.ToArray(),
			this._ModeratorsZ.ToArray(),
			this._Builders.ToArray(),
			this._Banned.ToArray()
		});
	}

	[PunRPC]
	private void SetActualData(string[] administrators, string[] moderators, string[] builders, string[] banned)
	{
		this._AdministratorsZ = new List<string>(administrators);
		this._ModeratorsZ = new List<string>(moderators);
		this._Builders = new List<string>(builders);
		this._Banned = new List<string>(banned);
		this._CanBuild = false;
		foreach (string a in this._Builders)
		{
			if (a == PhotonNetwork.playerName)
			{
				this._CanBuild = true;
				break;
			}
		}
		if (this.IsAdmin(null))
		{
			MainMenu.Instance.SetSelectedMapSlot(App.Instance.Settings.slotID);
		}
	}

	private bool IsTrustCall(PhotonMessageInfo info)
	{
		return info == null || this.IsAdmin(info.sender.name) || this.IsModerator(info.sender.name);
	}

	public bool IsAdmin(string playerName = null)
	{
		return this._AdministratorsZ.Contains((playerName == null) ? PhotonNetwork.playerName : playerName);
	}

	public void AddModeratorSafe(string playerName)
	{
		if (this.IsModerator(PhotonNetwork.playerName) || this.IsAdmin(PhotonNetwork.playerName))
		{
			this.AddModerator(playerName, null);
		}
	}

	[PunRPC]
	private void AddModerator(string playerName, PhotonMessageInfo info = null)
	{
		if (!this.IsTrustCall(info))
		{
			return;
		}
		this._ModeratorsZ.Add(playerName);
		if (info == null)
		{
			base.photonView.RPC("AddModerator", PhotonTargets.Others, new object[]
			{
				playerName
			});
		}
	}

	[PunRPC]
	public void RemoveModerator(string playerName, PhotonMessageInfo info = null)
	{
		if (!this.IsTrustCall(info))
		{
			return;
		}
		this._ModeratorsZ.Remove(playerName);
		if (info == null)
		{
			base.photonView.RPC("RemoveModerator", PhotonTargets.Others, new object[]
			{
				playerName
			});
		}
	}

	public bool IsModerator(string playerName = null)
	{
		return this._ModeratorsZ.Contains((playerName == null) ? PhotonNetwork.playerName : playerName);
	}

	public void AddBuilderSafe(string playerName)
	{
		if (this.IsModerator(PhotonNetwork.playerName) || this.IsAdmin(PhotonNetwork.playerName))
		{
			this.AddBuilder(playerName, null);
		}
	}

	[PunRPC]
	private void AddBuilder(string playerName, PhotonMessageInfo info = null)
	{
		if (!this.IsTrustCall(info))
		{
			return;
		}
		this._Builders.Add(playerName);
		if (info == null)
		{
			base.photonView.RPC("AddBuilder", PhotonTargets.Others, new object[]
			{
				playerName
			});
		}
		if (playerName == PhotonNetwork.playerName)
		{
			this._CanBuild = true;
		}
	}

	[PunRPC]
	public void RemoveBuilder(string playerName, PhotonMessageInfo info = null)
	{
		if (!this.IsTrustCall(info))
		{
			return;
		}
		this._Builders.Remove(playerName);
		if (info == null)
		{
			base.photonView.RPC("RemoveBuilder", PhotonTargets.Others, new object[]
			{
				playerName
			});
		}
		if (playerName == PhotonNetwork.playerName)
		{
			this._CanBuild = false;
		}
	}

	public bool IsBuilder(string playerName = null)
	{
		return this._Builders.Contains((playerName == null) ? PhotonNetwork.playerName : playerName);
	}

	[PunRPC]
	public void AddBanned(string playerName, PhotonMessageInfo info = null)
	{
		if (!this.IsTrustCall(info))
		{
			return;
		}
		this._Banned.Add(playerName);
		if (info == null)
		{
			base.photonView.RPC("AddBanned", PhotonTargets.Others, new object[]
			{
				playerName
			});
		}
	}

	[PunRPC]
	public void RemoveBanned(string playerName, PhotonMessageInfo info = null)
	{
		if (!this.IsTrustCall(info))
		{
			return;
		}
		this._Banned.Remove(playerName);
		if (info == null)
		{
			base.photonView.RPC("RemoveBanned", PhotonTargets.Others, new object[]
			{
				playerName
			});
		}
	}

	public bool IsBanned(string playerName = null)
	{
		return this._Banned.Contains((playerName == null) ? PhotonNetwork.playerName : playerName);
	}

	public static Level Instance;

	private List<string> _AdministratorsZ = new List<string>();

	private List<string> _ModeratorsZ = new List<string>();

	private List<string> _Builders = new List<string>();

	private List<string> _Banned = new List<string>();

	private bool _CanBuild;
}
