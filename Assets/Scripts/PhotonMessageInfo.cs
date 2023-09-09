using System;

public class PhotonMessageInfo
{
	public PhotonMessageInfo()
	{
		this.sender = PhotonNetwork.player;
		this.timeInt = (int)(PhotonNetwork.time * 1000.0);
		this.photonView = null;
	}

	public PhotonMessageInfo(PhotonPlayer player, int timestamp, PhotonView view)
	{
		this.sender = player;
		this.timeInt = timestamp;
		this.photonView = view;
	}

	public double timestamp
	{
		get
		{
			uint num = (uint)this.timeInt;
			double num2 = num;
			return num2 / 1000.0;
		}
	}

	public override string ToString()
	{
		return string.Format("[PhotonMessageInfo: Sender='{1}' Senttime={0}]", this.timestamp, this.sender);
	}

	private int timeInt;

	public PhotonPlayer sender;

	public PhotonView photonView;
}
