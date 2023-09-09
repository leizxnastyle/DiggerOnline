using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Photon Networking/Photon Rigidbody 2D View")]
[RequireComponent(typeof(PhotonView))]
public class PhotonRigidbody2DView : MonoBehaviour
{
	private void Awake()
	{
		this.m_Body = base.GetComponent<Rigidbody2D>();
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			if (this.m_SynchronizeVelocity)
			{
				stream.SendNext(this.m_Body.velocity);
			}
			if (this.m_SynchronizeAngularVelocity)
			{
				stream.SendNext(this.m_Body.angularVelocity);
			}
		}
		else
		{
			if (this.m_SynchronizeVelocity)
			{
				this.m_Body.velocity = (Vector2)stream.ReceiveNext();
			}
			if (this.m_SynchronizeAngularVelocity)
			{
				this.m_Body.angularVelocity = (float)stream.ReceiveNext();
			}
		}
	}

	[SerializeField]
	private bool m_SynchronizeVelocity = true;

	[SerializeField]
	private bool m_SynchronizeAngularVelocity = true;

	private Rigidbody2D m_Body;
}
