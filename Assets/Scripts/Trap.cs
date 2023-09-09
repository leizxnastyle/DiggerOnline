using System;
using System.Collections.Generic;
using UnityEngine;

public class Trap : EntityBase
{
	protected override void Updating()
	{
		if (this._State == Trap.State.None)
		{
			return;
		}
		float num = (this._StakesStartPosition - this.Stakes.transform.position).magnitude + 0.001f;
		int layerMask = 1 << LayerMask.NameToLayer("Players");
		foreach (Transform transform in this.StakesPeak)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(transform.position - base.transform.up * num, base.transform.up, out raycastHit, num, layerMask) && !this._DamagedPlayers.Contains(raycastHit.collider.gameObject))
			{
				GameObject gameObject = raycastHit.collider.gameObject;
				this._DamagedPlayers.Add(gameObject);
				if (gameObject == WorldGameObjectX.Instance.MainPlayer)
				{
					WorldGameObjectX.Instance.MainPlayerNode.DoDamage(100f, true);
				}
				break;
			}
		}
		if (this._State == Trap.State.LiftUp || this._State == Trap.State.LiftDown)
		{
			float num2 = Mathf.Clamp01((this._NextStateTime - Time.time) / ((this._State != Trap.State.LiftUp) ? this.LiftDownTime : this.LiftUpTime));
			if (this._State == Trap.State.LiftUp)
			{
				num2 = 1f - num2;
			}
			this.Stakes.transform.position = this._StakesStartPosition + base.transform.up * (num2 * this.LiftValue);
		}
		if (Time.time >= this._NextStateTime)
		{
			this.NextState();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!base.IsPreview && this._State == Trap.State.None && other.gameObject.layer == LayerMask.NameToLayer("Players"))
		{
			base.photonView.RPC("StartWork", PhotonTargets.All, new object[0]);
		}
	}

	[PunRPC]
	private void StartWork()
	{
		if (this._State == Trap.State.None)
		{
			this.NextState();
		}
	}

	private void NextState()
	{
		switch (this._State)
		{
		case Trap.State.None:
			this._State = Trap.State.Delay;
			this._NextStateTime = Time.time + this.DelayTime;
			this._StakesStartPosition = this.Stakes.transform.position;
			break;
		case Trap.State.Delay:
			this._State = Trap.State.LiftUp;
			this._NextStateTime = Time.time + this.LiftUpTime;
			break;
		case Trap.State.LiftUp:
			this._State = Trap.State.Stay;
			this._NextStateTime = Time.time + this.StayTime;
			break;
		case Trap.State.Stay:
			this._State = Trap.State.LiftDown;
			this._NextStateTime = Time.time + this.LiftDownTime;
			this.Stakes.transform.position = this._StakesStartPosition + base.transform.up * this.LiftValue;
			break;
		case Trap.State.LiftDown:
			this._State = Trap.State.None;
			this.Stakes.transform.position = this._StakesStartPosition;
			this._DamagedPlayers.Clear();
			break;
		}
	}

	public float DelayTime;

	public float LiftUpTime;

	public float StayTime;

	public float LiftDownTime;

	public float LiftValue;

	public GameObject Stakes;

	public Transform[] StakesPeak;

	private Trap.State _State;

	private float _NextStateTime;

	private List<GameObject> _DamagedPlayers = new List<GameObject>();

	private Vector3 _StakesStartPosition = Vector3.zero;

	private enum State
	{
		None,
		Delay,
		LiftUp,
		Stay,
		LiftDown
	}
}
