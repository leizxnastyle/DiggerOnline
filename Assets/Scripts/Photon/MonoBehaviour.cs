using System;
using UnityEngine;

namespace Photon
{
	public class MonoBehaviour : MonoBehaviour
	{
		public PhotonView photonView
		{
			get
			{
				return PhotonView.Get(this);
			}
		}

		public PhotonView networkView
		{
			get
			{
				UnityEngine.Debug.LogWarning("Why are you still using networkView? should be PhotonView?");
				return PhotonView.Get(this);
			}
		}
	}
}
