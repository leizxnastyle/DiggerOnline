using System;
using System.Collections;
using UnityEngine;

public class HG_Point_Start_Battle : EntityBase
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
		WorldGameObjectX.Instance.photonView.RPC("BattleStartPoint", PhotonTargets.All, new object[]
		{
			string.Concat(new object[]
			{
				base.transform.position.x,
				";",
				base.transform.position.y,
				";",
				base.transform.position.z
			})
		});
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
		if (!GameType.IsHungerGamesMode && !GameType.IsArcadeMode)
		{
			return;
		}
		if (GameType.IsArcadeMode && HG_WorkController.hgstatus == GameStatus.GS_START)
		{
			UnityEngine.Object.Destroy(base.gameObject);
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
		if (!GameType.IsHungerGamesMode && !GameType.IsArcadeMode)
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

	public const float RespawnTime = 60f;

	protected bool _CanRespawn = true;
}
