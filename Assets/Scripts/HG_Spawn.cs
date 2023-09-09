using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HG_Spawn : EntityBase
{
	private PhotonView p_View
	{
		get
		{
			return base.GetComponent<PhotonView>();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.gameObject.AddComponent<Rigidbody>();
		base.GetComponent<Rigidbody>().drag = 1f;
		base.GetComponent<Rigidbody>().isKinematic = true;
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

	public override void OnButtonE(string playerName)
	{
		if (!GameType.IsHungerGamesMode || WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>().PlayerTeam == 0 || WorldGameObjectX.Instance.MainPlayer.GetComponent<PlayerNetwork>().CurentLife <= 0)
		{
			base.OnButtonE(playerName);
		}
	}

	[PunRPC]
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	[PunRPC]
	public void Show()
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

	public virtual void AddItem(List<int> items)
	{
	}

	public const float RespawnTime = 60f;

	public int chest_game_id = -1;

	public ChestType chest_type;

	protected bool _CanRespawn = true;

	public List<int> in_item;
}
