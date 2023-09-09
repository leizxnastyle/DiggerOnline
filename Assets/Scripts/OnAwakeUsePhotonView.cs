using System;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnAwakeUsePhotonView : Photon.MonoBehaviour
{
	private void Awake()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		base.photonView.RPC("OnAwakeRPC", PhotonTargets.All, new object[0]);
	}

	private void Start()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		base.photonView.RPC("OnAwakeRPC", PhotonTargets.All, new object[]
		{
			1
		});
	}

	[PunRPC]
	public void OnAwakeRPC()
	{
		UnityEngine.Debug.Log("RPC: 'OnAwakeRPC' PhotonView: " + base.photonView);
	}

	[PunRPC]
	public void OnAwakeRPC(byte myParameter)
	{
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			"RPC: 'OnAwakeRPC' Parameter: ",
			myParameter,
			" PhotonView: ",
			base.photonView
		}));
	}
}
