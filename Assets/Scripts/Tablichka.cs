using System;
using UnityEngine;

public class Tablichka : EntityBase
{
	protected override void PreviewCreation(object[] data)
	{
		if (data != null)
		{
			this.SetText((string)data[0], (string)data[1], (string)data[2], (string)data[3]);
		}
	}

	protected override void Creation(object[] data)
	{
		if (data != null)
		{
			this.SetText((string)data[0], (string)data[1], (string)data[2], (string)data[3]);
		}
		if (!PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("GetActualText", PhotonTargets.MasterClient, new object[0]);
		}
	}

	public override object[] GetData()
	{
		return new object[]
		{
			this.Line1.GetComponent<TextMesh>().text,
			this.Line2.GetComponent<TextMesh>().text,
			this.Line3.GetComponent<TextMesh>().text,
			this.Line4.GetComponent<TextMesh>().text
		};
	}

	[PunRPC]
	private void GetActualText(PhotonMessageInfo info)
	{
		base.photonView.RPC("SetText", info.sender, new object[]
		{
			this.Line1.GetComponent<TextMesh>().text,
			this.Line2.GetComponent<TextMesh>().text,
			this.Line3.GetComponent<TextMesh>().text,
			this.Line4.GetComponent<TextMesh>().text
		});
	}

	public override void OnButtonF(string playerName)
	{
		if (GameType.IsHungerGamesMode)
		{
			return;
		}
		if (GameType.IsArcadeMode)
		{
			return;
		}
		if (GameType.IsHideSeek)
		{
			return;
		}
		if (App.Instance.Settings.isWatch)
		{
			Chat.SendInfoF(Localize.GetText("CANT_EDIT_PLATE", null), false);
			return;
		}
		if (playerName == this.Creator)
		{
			MainMenu.Instance.SwitchMenu(Menu.Plate, this, new string[]
			{
				this.Line1.GetComponent<TextMesh>().text,
				this.Line2.GetComponent<TextMesh>().text,
				this.Line3.GetComponent<TextMesh>().text,
				this.Line4.GetComponent<TextMesh>().text
			});
		}
		else
		{
			Chat.SendInfoF(Localize.GetText("CANT_EDIT_PLATE", null), false);
		}
	}

	[PunRPC]
	private void SetText(string s1, string s2, string s3, string s4)
	{
		s1 = Localize.GetText(s1, null);
		s2 = Localize.GetText(s2, null);
		s3 = Localize.GetText(s3, null);
		s4 = Localize.GetText(s4, null);
		this.Line1.GetComponent<TextMesh>().characterSize = 0.4f;
		this.Line2.GetComponent<TextMesh>().characterSize = 0.4f;
		this.Line3.GetComponent<TextMesh>().characterSize = 0.4f;
		this.Line4.GetComponent<TextMesh>().characterSize = 0.4f;
		this.Line1.GetComponent<TextMesh>().text = s1;
		this.Line2.GetComponent<TextMesh>().text = s2;
		this.Line3.GetComponent<TextMesh>().text = s3;
		this.Line4.GetComponent<TextMesh>().text = s4;
		while (this.Line1.GetComponent<MeshRenderer>().bounds.extents.z > 0.3f || this.Line1.GetComponent<MeshRenderer>().bounds.extents.x > 0.3f)
		{
			this.Line1.GetComponent<TextMesh>().characterSize *= 0.9f;
		}
		while (this.Line2.GetComponent<MeshRenderer>().bounds.extents.z > 0.3f || this.Line2.GetComponent<MeshRenderer>().bounds.extents.x > 0.3f)
		{
			this.Line2.GetComponent<TextMesh>().characterSize *= 0.9f;
		}
		while (this.Line3.GetComponent<MeshRenderer>().bounds.extents.z > 0.3f || this.Line3.GetComponent<MeshRenderer>().bounds.extents.x > 0.3f)
		{
			this.Line3.GetComponent<TextMesh>().characterSize *= 0.9f;
		}
		while (this.Line4.GetComponent<MeshRenderer>().bounds.extents.z > 0.3f || this.Line4.GetComponent<MeshRenderer>().bounds.extents.x > 0.3f)
		{
			this.Line4.GetComponent<TextMesh>().characterSize *= 0.9f;
		}
	}

	public void UpdateText(string s1, string s2, string s3, string s4)
	{
		base.photonView.RPC("SetText", PhotonTargets.All, new object[]
		{
			s1,
			s2,
			s3,
			s4
		});
	}

	public GameObject Line1;

	public GameObject Line2;

	public GameObject Line3;

	public GameObject Line4;
}
