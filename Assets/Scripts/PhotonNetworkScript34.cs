using System;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PhotonNetworkScript34 : Photon.MonoBehaviour
{
	private void OnPhotonInstantiate(PhotonMessageInfo msg)
	{
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
		}
		else
		{
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}

	private void Update()
	{
		if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this.correctPlayerPos, Time.deltaTime * 5f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.correctPlayerRot, Time.deltaTime * 5f);
		}
	}

	private Vector3 correctPlayerPos = Vector3.zero;

	private Quaternion correctPlayerRot = Quaternion.identity;
}
