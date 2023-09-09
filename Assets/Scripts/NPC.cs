using System;
using System.Collections;
using UnityEngine;

public class NPC : EntityBase
{
	protected override void Creation(object[] data)
	{
		SkinManager component = base.GetComponent<SkinManager>();
		if (component != null)
		{
			component.SetSkin((data == null) ? 0 : ((int)data[0]));
		}
		this._InitialLife = this.Life;
		this._CharacterController = base.GetComponent<CharacterController>();
		this._AI = base.GetComponent<AI>();
		this._Position = base.transform.position;
		this._Rotation = base.transform.rotation;
	}

	protected override void PreviewCreation(object[] data)
	{
		SkinManager component = base.GetComponent<SkinManager>();
		if (component != null)
		{
			component.SetSkin((data == null) ? 0 : ((int)data[0]));
		}
		UnityEngine.Object.Destroy(base.GetComponent<AI>());
	}

	protected override void Updating()
	{
		if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this._Position, Time.deltaTime * 5f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this._Rotation, Time.deltaTime * 5f);
		}
	}

	private void OnGUI()
	{
	}

	public override void OnLeftMouseHit(string playerName)
	{
		if (Level.Instance.IsAdmin(null))
		{
			base.photonView.RPC("Hit", PhotonTargets.All, new object[]
			{
				CameraController.RaycastCamera.transform.forward
			});
		}
		else
		{
			base.photonView.RPC("Punch", PhotonTargets.All, new object[]
			{
				CameraController.RaycastCamera.transform.forward
			});
		}
	}

	[PunRPC]
	private void Punch(Vector3 cameraDir)
	{
		if (PhotonNetwork.isMasterClient)
		{
			cameraDir.y = 0.2f;
			this._AI.Punch(cameraDir.normalized * 10f, 0.2f);
		}
	}

	[PunRPC]
	private void Hit(Vector3 cameraDir, PhotonMessageInfo info)
	{
		if (this.Life <= 0)
		{
			return;
		}
		this.Life -= 1;
		if (PhotonNetwork.isMasterClient)
		{
			if (UnityEngine.Random.Range(0, 100) < 33)
			{
				this._AI.Punch(cameraDir.normalized * 10f, 0.2f);
			}
			if (this.Life <= 0)
			{
				base.photonView.RPC("Dying", PhotonTargets.All, new object[0]);
				if (Survival.Instance != null)
				{
					Survival.Instance.OnKillEnemy(info.sender);
				}
			}
		}
	}

	private void HitRedness()
	{
		SkinManager component = base.GetComponent<SkinManager>();
		if (component != null)
		{
			component.Redness();
		}
	}

	[PunRPC]
	private void Dying()
	{
		base.StartCoroutine(this.DyingProcess());
	}

	private IEnumerator DyingProcess()
	{
		UnityEngine.Object.Destroy(this._AI);
		SkinManager skinManager = base.GetComponent<SkinManager>();
		if (skinManager != null)
		{
			NetworkSyncAnimation networkAnim = base.GetComponent<NetworkSyncAnimation>();
			if (PhotonNetwork.isMasterClient && networkAnim != null && networkAnim.Anim != null && networkAnim.Anim["death"] != null)
			{
				networkAnim.PlayAnimation(NetworkSyncAnimation.AnimState.death, true);
				yield return new WaitForSeconds(networkAnim.Anim["death"].length);
			}
			if (this._CharacterController != null)
			{
				UnityEngine.Object.Destroy(this._CharacterController);
				this._CharacterController = null;
			}
			float startTime = Time.time;
			for (;;)
			{
				float alpha = 1f - (Time.time - startTime) / 2f;
				if (alpha < 0f)
				{
					break;
				}
				skinManager.SetTransparent(alpha);
				yield return new WaitForSeconds(0.01f);
			}
		}
		if (PhotonNetwork.isMasterClient)
		{
			this.SelfDelete();
		}
		yield break;
	}

	protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(this.Life);
		}
		else
		{
			this._Position = (Vector3)stream.ReceiveNext();
			this._Rotation = (Quaternion)stream.ReceiveNext();
			this.Life = (byte)stream.ReceiveNext();
		}
	}

	public const float ShowLifeBarDistance = 10f;

	public const float LifeBarWidth = 150f;

	public const float LifeBarHeight = 15f;

	public const float DeathTransparentTime = 2f;

	public const int PunchChancePercent = 33;

	public Texture RectGreenTexture;

	public Texture RectRedTexture;

	protected byte _InitialLife;

	private float _CurLifeValue = 1f;

	private float _CurLifeAlpha;

	private CharacterController _CharacterController;

	private AI _AI;

	private Vector3 _Position = Vector3.zero;

	private Quaternion _Rotation = Quaternion.identity;
}
