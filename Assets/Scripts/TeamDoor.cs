using System;
using UnityEngine;

public class TeamDoor : EntityBase
{
	public void OpenDoorR()
	{
		base.photonView.RPC("OpenDoor", PhotonTargets.All, new object[0]);
	}

	public void CloseDoorR()
	{
		base.photonView.RPC("CloseDoor", PhotonTargets.All, new object[0]);
	}

	public override void OnLeftMouseHit(string playerName)
	{
		if (TeamBattle.Instance == null)
		{
			base.OnLeftMouseHit(playerName);
		}
	}

	[PunRPC]
	public void OpenDoor()
	{
		iTween.RotateAdd(this.GearModel1, iTween.Hash(new object[]
		{
			"x",
			360,
			"time",
			0.25f,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeInQuad
		}));
		iTween.RotateAdd(this.GearModel2, iTween.Hash(new object[]
		{
			"x",
			360,
			"time",
			0.25f,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeInQuad
		}));
		iTween.MoveTo(this.LeftDoor, iTween.Hash(new object[]
		{
			"position",
			new Vector3(0f, 0f, 1f),
			"time",
			0.25f,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeInQuad
		}));
		iTween.MoveTo(this.RightDoor, iTween.Hash(new object[]
		{
			"position",
			new Vector3(0f, 0f, -1f),
			"time",
			0.25f,
			"islocal",
			true,
			"easetype",
			iTween.EaseType.easeInQuad
		}));
		this._Opened = true;
		SoundManager.Instance.Play(SoundManager.Sound.TeamDoorOpen, base.GetComponent<AudioSource>());
	}

	[PunRPC]
	public void CloseDoor()
	{
		if (this._Opened)
		{
			iTween.RotateAdd(this.GearModel1, iTween.Hash(new object[]
			{
				"x",
				-360,
				"time",
				0.25f,
				"islocal",
				true,
				"easetype",
				iTween.EaseType.easeInQuad
			}));
			iTween.RotateAdd(this.GearModel2, iTween.Hash(new object[]
			{
				"x",
				-360,
				"time",
				0.25f,
				"islocal",
				true,
				"easetype",
				iTween.EaseType.easeInQuad
			}));
			if (this.team == 1)
			{
				iTween.MoveTo(this.LeftDoor, iTween.Hash(new object[]
				{
					"position",
					new Vector3(0f, 0f, 0.0329404f),
					"time",
					0.25f,
					"islocal",
					true,
					"easetype",
					iTween.EaseType.easeInQuad
				}));
				iTween.MoveTo(this.RightDoor, iTween.Hash(new object[]
				{
					"position",
					new Vector3(0f, 0f, 0.0329404f),
					"time",
					0.25f,
					"islocal",
					true,
					"easetype",
					iTween.EaseType.easeInQuad
				}));
			}
			if (this.team == 2)
			{
				iTween.MoveTo(this.LeftDoor, iTween.Hash(new object[]
				{
					"position",
					new Vector3(0f, 0f, 0.1363504f),
					"time",
					0.25f,
					"islocal",
					true,
					"easetype",
					iTween.EaseType.easeInQuad
				}));
				iTween.MoveTo(this.RightDoor, iTween.Hash(new object[]
				{
					"position",
					new Vector3(0f, 0f, 0.1363504f),
					"time",
					0.25f,
					"islocal",
					true,
					"easetype",
					iTween.EaseType.easeInQuad
				}));
			}
			this._Opened = false;
			SoundManager.Instance.Play(SoundManager.Sound.TeamDoorClose, base.GetComponent<AudioSource>());
		}
	}

	public GameObject LeftDoor;

	public GameObject RightDoor;

	public GameObject GearModel1;

	public GameObject GearModel2;

	public GameObject trigger;

	public int team;

	private bool _Opened;
}
