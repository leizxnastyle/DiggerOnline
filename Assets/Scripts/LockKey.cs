using System;
using UnityEngine;

public class LockKey : EntityBase
{
	private void OnTriggerEnter(Collider other)
	{
		if (base.IsPreview)
		{
			return;
		}
		if (!WorldGameObjectX.Instance.IsWorldGenerated)
		{
			return;
		}
		if (other.gameObject != WorldGameObjectX.Instance.MainPlayer)
		{
			return;
		}
		if (!App.Instance.Settings.isWatch && (Level.Instance.IsAdmin(null) || Level.Instance.IsModerator(null)))
		{
			return;
		}
		this.TakeKey();
	}

	public override void OnButtonE(string playerName)
	{
		if (!App.Instance.Settings.isWatch && (this.Creator == playerName || Level.Instance.IsAdmin(null) || Level.Instance.IsModerator(null)))
		{
			WorldGameObjectX.Instance.TakeEnity(base.gameObject);
		}
		else
		{
			this.TakeKey();
		}
	}

	private void TakeKey()
	{
		UnityEngine.Object.Destroy(this.KeyModel);
		UnityEngine.Object.Destroy(base.GetComponent<Collider>());
		Chat.SendInfoF(PhotonNetwork.playerName + Localize.GetText("LOCK_FIND_KEY", null) + Localize.GetText("PURCHASE_" + this.Name, null), true);
	}

	public int LockID;

	public GameObject KeyModel;
}
