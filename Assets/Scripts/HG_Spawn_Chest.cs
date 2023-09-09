using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HG_Spawn_Chest : HG_Spawn
{
	protected override void Awake()
	{
		base.Awake();
	}

	public void SendInstToserver()
	{
		this.in_item = new List<int>();
		WorldGameObjectX.Instance.photonView.RPC("ChestInst", PhotonTargets.All, new object[]
		{
			this.chest_game_id,
			(int)this.chest_type
		});
	}

	private IEnumerator WaitAndSendChestID()
	{
		yield return new WaitForSeconds((float)this.chest_game_id / 10f);
		yield break;
	}

	public override object[] GetData()
	{
		return new object[]
		{
			this._CanRespawn
		};
	}

	protected override void Updating()
	{
		if (!GameType.IsHungerGamesMode)
		{
			return;
		}
		try
		{
			if (base.photonView.isMine && base.transform.position.y < -200f)
			{
				PhotonNetwork.Destroy(base.gameObject);
			}
			Vector3 position = WorldGameObjectX.Instance.MainPlayer.transform.position;
			PlayerNetwork component = WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>();
			if ((base.transform.position - position).magnitude < 2f)
			{
				if (this.Type == EntityType.HG_CHEST)
				{
				}
				if (!this._CanRespawn)
				{
					this.SelfDelete();
				}
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Entiti Update Error\n" + ex.StackTrace);
		}
	}

	public override void OnButtonE(string playerName)
	{
		if (GameType.IsHungerGamesMode && WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>().PlayerTeam != 0 && WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>().CurentLife > 0)
		{
			SoundManager.Instance.PlayAtPoint(SoundManager.Sound.TakeGift, base.transform.position);
			string text = string.Join(";", (from x in this.in_item
			select x.ToString()).ToArray<string>());
			MainMenu.Instance.OpenChest(this.in_item, this.chest_game_id);
		}
		else
		{
			base.OnButtonE(playerName);
		}
	}

	[PunRPC]
	public new void Hide()
	{
		base.gameObject.SetActive(false);
	}

	[PunRPC]
	public new void Show()
	{
		base.gameObject.SetActive(true);
	}

	private IEnumerator RespawnGift()
	{
		base.photonView.RPC("Hide", PhotonTargets.AllBuffered, new object[0]);
		yield return new WaitForSeconds(60f);
		base.photonView.RPC("Show", PhotonTargets.AllBuffered, new object[0]);
		yield break;
	}

	public override void AddItem(List<int> items)
	{
		this.in_item = new List<int>(items);
	}
}
