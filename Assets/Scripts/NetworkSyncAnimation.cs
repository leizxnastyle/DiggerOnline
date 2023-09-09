using System;
using Photon;
using UnityEngine;

public class NetworkSyncAnimation : Photon.MonoBehaviour
{
	public Animation Anim
	{
		get
		{
			return (!(this._SkinManager != null)) ? base.GetComponent<Animation>() : this._SkinManager.Anim;
		}
	}

	public NetworkSyncAnimation.AnimState CurrentAnimation
	{
		get
		{
			return this._CurrentAnimation;
		}
	}

	private void Start()
	{
		this._SkinManager = base.GetComponent<SkinManager>();
	}

	public void PlayAnimation(NetworkSyncAnimation.AnimState anim, bool crossFade = true)
	{
		if (this.Anim != null)
		{
			string name = Enum.GetName(typeof(NetworkSyncAnimation.AnimState), anim);
			if (crossFade)
			{
				this.Anim.CrossFade(name);
			}
			else
			{
				this.Anim.Play(name);
			}
		}
		this.SyncAnimation(anim);
	}

	public void SyncAnimation(NetworkSyncAnimation.AnimState anim)
	{
		this._CurrentAnimation = anim;
	}

	private void Update()
	{
		if (!base.photonView.isMine && this.Anim != null && base.GetComponent<PlayerNetwork>() != null)
		{
			string text = Enum.GetName(typeof(NetworkSyncAnimation.AnimState), this._CurrentAnimation);
			text = base.GetComponent<PlayerNetwork>().GetPrefix(this.Anim, text);
			if (!this.Anim.IsPlaying(text) && (this._CurrentAnimation != NetworkSyncAnimation.AnimState.death || this._CurrentAnimation != this._LastAnimation))
			{
				if (this._CurrentAnimation == NetworkSyncAnimation.AnimState.dig || this._CurrentAnimation == NetworkSyncAnimation.AnimState.build)
				{
					this.Anim.Play(text);
				}
				else
				{
					this.Anim.CrossFade(text);
				}
				this._LastAnimation = this._CurrentAnimation;
			}
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext((byte)this._CurrentAnimation);
		}
		else
		{
			this._CurrentAnimation = (NetworkSyncAnimation.AnimState)((byte)stream.ReceiveNext());
		}
	}

	private SkinManager _SkinManager;

	private NetworkSyncAnimation.AnimState _CurrentAnimation;

	private NetworkSyncAnimation.AnimState _LastAnimation;

	public enum AnimState
	{
		idle,
		idle2,
		run,
		jump,
		strafL,
		strafR,
		back,
		dig,
		build,
		death,
		fidget,
		sit,
		lie
	}
}
