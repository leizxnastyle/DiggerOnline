using System;
using System.Collections;
using UnityEngine;

public class Gift : EntityBase
{
	protected override void Awake()
	{
		base.Awake();
		base.gameObject.AddComponent<Rigidbody>();
		base.GetComponent<Rigidbody>().drag = 1f;
		base.GetComponent<Rigidbody>().isKinematic = true;
	}

	protected override void Creation(object[] data)
	{
		base.GetComponent<Rigidbody>().isKinematic = false;
		this.SelfDelete();
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
		if (!GameType.BattleMode() || GameType.IsObserving())
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
			SoundManager.Instance.PlayAtPoint(SoundManager.Sound.TakeGift, base.transform.position);
			if (this.Type == EntityType.GIFT_ARMOR)
			{
				WorldGameObjectX.Instance.MainPlayerNode.Shield = WorldGameObjectX.Instance.MainPlayerNode.MaxShield;
			}
			else if (this.Type == EntityType.GIFT_LIFE)
			{
				WorldGameObjectX.Instance.MainPlayerNode.Life = WorldGameObjectX.Instance.MainPlayerNode.MaxLife;
			}
			else if (this.Type == EntityType.GIFT_AMMO)
			{
				component.CurGun.bulletsLeft = component.CurGun.bulletsMax;
			}
			else if (this.Type == EntityType.GIFT_GRENADE)
			{
				component.Grenade.bullets += 3;
			}
			if (this._CanRespawn)
			{
				WorldGameObjectX.Instance.StartCoroutine(this.RespawnGift());
			}
			else
			{
				this.SelfDelete();
			}
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

	private bool _CanRespawn = true;
}
