using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : EntityBase
{
	protected override void Awake()
	{
		base.Awake();
		this._BusyAttachPoints = new bool[this.AttachPoints.Length];
	}

	public override void OnButtonF(string playerName)
	{
		if (Holder.Active == null)
		{
			this.PlayerEnter();
		}
	}

	protected override void Destruction()
	{
		if (this == Holder.Active)
		{
			this.PlayerExit(true);
		}
	}

	private void PlayerEnter()
	{
		GameObject mainPlayer = WorldGameObjectX.Instance.MainPlayer;
		Transform attachPoint = this.GetAttachPoint(mainPlayer.transform.position);
		if (attachPoint == null)
		{
			return;
		}
		this._IsCameraThirdPerson = CameraController.Instance.IsThirdPerson;
		this._IsCameraFreeLook = CameraController.Instance.IsFreeLook;
		if (!this._IsCameraThirdPerson || !this._IsCameraFreeLook)
		{
			CameraController.Instance.SetThirdPerson(true, 0f, false);
		}
		CameraController.Instance.DisableCameraSwitching = true;
		if (mainPlayer.GetComponent<PlayerMotor>() != null)
		{
			mainPlayer.GetComponent<PlayerMotor>().enabled = false;
		}
		if (mainPlayer.GetComponent<PlayerInput>() != null)
		{
			mainPlayer.GetComponent<PlayerInput>().SetMovement(false);
		}
		mainPlayer.transform.position = attachPoint.position;
		mainPlayer.GetComponent<MouseLook>().RotationX = attachPoint.eulerAngles.y;
		mainPlayer.GetComponent<NetworkSyncAnimation>().PlayAnimation(this.StayAnimation, true);
		Holder.Active = this;
		base.StartCoroutine(this.PlayerMonitoringProcess(attachPoint.eulerAngles.y));
	}

	private void PlayerExit(bool move)
	{
		CameraController.Instance.DisableCameraSwitching = false;
		if (this._IsCameraThirdPerson)
		{
			CameraController.Instance.SetThirdPerson(this._IsCameraFreeLook, 0f, true);
		}
		else
		{
			CameraController.Instance.SetFirstPerson(true);
		}
		GameObject mainPlayer = WorldGameObjectX.Instance.MainPlayer;
		if (mainPlayer.GetComponent<PlayerMotor>() != null)
		{
			mainPlayer.GetComponent<PlayerMotor>().enabled = true;
		}
		if (mainPlayer.GetComponent<PlayerInput>() != null)
		{
			mainPlayer.GetComponent<PlayerInput>().SetMovement(true);
		}
		if (move && base.GetComponent<Renderer>() != null)
		{
			mainPlayer.transform.position = base.GetComponent<Renderer>().bounds.center;
		}
		this._BusyAttachPoints[this._BusyAttachPointIndex] = false;
		base.photonView.RPC("SetBusyPoint", PhotonTargets.Others, new object[]
		{
			this._BusyAttachPointIndex,
			false
		});
		Holder.Active = null;
		base.StopAllCoroutines();
	}

	private IEnumerator PlayerMonitoringProcess(float attachPointY)
	{
		Vector3 pos = WorldGameObjectX.Instance.MainPlayer.transform.position;
		for (;;)
		{
			WorldGameObjectX.Instance.MainPlayer.GetComponent<MouseLook>().RotationX = attachPointY;
			if (WorldGameObjectX.Instance.MainPlayer == null)
			{
				break;
			}
			if (WorldGameObjectX.Instance.MainPlayer.GetComponent<NetworkSyncAnimation>().CurrentAnimation != this.StayAnimation)
			{
				break;
			}
			if (pos != WorldGameObjectX.Instance.MainPlayer.transform.position)
			{
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		this.PlayerExit(pos == WorldGameObjectX.Instance.MainPlayer.transform.position);
		yield break;
	}

	private Transform GetAttachPoint(Vector3 pos)
	{
		List<Transform> list = new List<Transform>(this.AttachPoints);
		List<Transform> list2 = new List<Transform>(this.AttachPoints);
		list2.Sort((Transform a, Transform b) => (int)((Vector3.Distance(a.position, pos) - Vector3.Distance(b.position, pos)) * 1000f));
		for (int i = 0; i < list2.Count; i++)
		{
			Transform transform = list2[i];
			int num = list.IndexOf(transform);
			if (!this._BusyAttachPoints[num])
			{
				this._BusyAttachPointIndex = num;
				this._BusyAttachPoints[this._BusyAttachPointIndex] = true;
				base.photonView.RPC("SetBusyPoint", PhotonTargets.Others, new object[]
				{
					this._BusyAttachPointIndex,
					true
				});
				return transform;
			}
		}
		return null;
	}

	[PunRPC]
	private void SetBusyPoint(int index, bool busy)
	{
		this._BusyAttachPoints[index] = busy;
	}

	public static Holder Active;

	public Transform[] AttachPoints;

	public NetworkSyncAnimation.AnimState StayAnimation;

	private bool _IsCameraThirdPerson;

	private bool _IsCameraFreeLook;

	private bool[] _BusyAttachPoints;

	private int _BusyAttachPointIndex;
}
