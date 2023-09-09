using System;
using UnityEngine;

public class RPCConflict : MonoBehaviour
{
	[PunRPC]
	public void SetLiked()
	{
	}

	[PunRPC]
	private void UsePurchase(int a1)
	{
	}

	[PunRPC]
	private void GetVersionServer()
	{
	}
}
