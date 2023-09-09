using System;
using UnityEngine;

public class Door : EntityBase
{
	public override void OnButtonF(string playerName)
	{
		if (!this._Opened)
		{
			base.photonView.RPC("OpenDoor", PhotonTargets.All, new object[0]);
		}
		else
		{
			base.photonView.RPC("CloseDoor", PhotonTargets.All, new object[0]);
		}
	}

	[PunRPC]
	private void OpenDoor()
	{
		iTween.RotateTo(this.InnerModel, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(this.InnerModel.transform.localEulerAngles.x, this.OpenY, 0f),
			"time",
			1f,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeOutExpo
		}));
		this._Opened = true;
		SoundManager.Instance.Play(SoundManager.Sound.DoorOpen, base.GetComponent<AudioSource>());
	}

	[PunRPC]
	private void CloseDoor()
	{
		iTween.RotateTo(this.InnerModel, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(this.InnerModel.transform.localEulerAngles.x, this.CloseY, 0f),
			"time",
			1f,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeOutExpo
		}));
		this._Opened = false;
		SoundManager.Instance.Play(SoundManager.Sound.DoorClose, base.GetComponent<AudioSource>());
	}

	public GameObject InnerModel;

	public float CloseY;

	public float OpenY = 90f;

	private bool _Opened;
}
