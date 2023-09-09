using System;
using UnityEngine;

public class Firework : EntityBase
{
	public override void OnButtonE(string player_name)
	{
		if (Info.Instance.GameMode != "BUILDING")
		{
			return;
		}
		if (this.Creator == player_name || Level.Instance.IsAdmin(null))
		{
			if (!this.FireworkOn)
			{
				WorldGameObjectX.Instance.TakeEnity(base.gameObject);
			}
		}
		else
		{
			Chat.SendInfoF(ProfileINI.nickname + Localize.GetText("CANT_TAKE_AND_DESTROY_ITEMS", null), false);
		}
	}

	public override void OnButtonF(string player_name)
	{
		if (Info.Instance.GameMode != "BUILDING")
		{
			return;
		}
		if (Level.Instance.IsAdmin(null) || Level.Instance.IsModerator(null))
		{
			if (this.FireworkOn)
			{
				return;
			}
			this.FireworkOn = true;
			this.Spark.GetComponent<ParticleEmitter>().enabled = true;
			base.Invoke("StartFirework", 3f);
		}
		else
		{
			Chat.SendWarning(Localize.GetText("key4004", null), false);
		}
	}

	public void StartFirework()
	{
		base.photonView.RPC("StartFirework", PhotonTargets.All, new object[]
		{
			this.Color
		});
		this.SelfDelete();
	}

	[PunRPC]
	public void StartFirework(int id)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(WorldGameObjectX.Instance.fireworkList[id], base.transform.position, Quaternion.identity);
	}

	private bool FireworkOn;

	public GameObject Spark;

	public GameObject instFirework;

	public int Color;
}
