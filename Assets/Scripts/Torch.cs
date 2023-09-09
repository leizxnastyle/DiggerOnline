using System;
using UnityEngine;

public class Torch : EntityBase
{
	protected override void Creation(object[] data)
	{
		if (base.name.Contains("Camping2"))
		{
			base.GetComponent<AudioSource>().volume = ProfileINI.sound_volume * ProfileINI.sound_scale;
		}
		this.SetLight();
	}

	protected override void PreviewCreation(object[] data)
	{
		if (base.name.Contains("Camping2"))
		{
			base.GetComponent<AudioSource>().volume = ProfileINI.sound_volume * ProfileINI.sound_scale;
		}
		base.photonView.RPC("RemoveLight", PhotonTargets.All, new object[0]);
	}

	public override void ObjectIsMoved()
	{
		if (base.name.Contains("Camping2"))
		{
			base.GetComponent<AudioSource>().volume = ProfileINI.sound_volume * ProfileINI.sound_scale;
		}
		this.SetLight();
	}

	private void SetLight()
	{
		World.Instance.Lighting.AddLight(base.BlockX, base.BlockY, base.BlockZ, true);
	}

	[PunRPC]
	private void RemoveLight()
	{
		World.Instance.Lighting.RemoveLight(base.BlockX, base.BlockY, base.BlockZ, true);
	}

	public override void SelfDelete()
	{
		base.photonView.RPC("RemoveLight", PhotonTargets.All, new object[0]);
		base.SelfDelete();
	}
}
