using System;
using System.Collections;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnClickDestroy : Photon.MonoBehaviour
{
	public void OnClick()
	{
		if (!this.DestroyByRpc)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
		else
		{
			base.photonView.RPC("DestroyRpc", PhotonTargets.AllBuffered, new object[0]);
		}
	}

	[PunRPC]
	public IEnumerator DestroyRpc()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		yield return 0;
		PhotonNetwork.UnAllocateViewID(base.photonView.viewID);
		yield break;
	}

	public bool DestroyByRpc;
}
