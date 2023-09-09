using System;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[AddComponentMenu("Photon Networking/Photon Transform View")]
public class PhotonTransformView : MonoBehaviour, IPunObservable
{
	private void Awake()
	{
		this.m_PhotonView = base.GetComponent<PhotonView>();
		this.m_PositionControl = new PhotonTransformViewPositionControl(this.m_PositionModel);
		this.m_RotationControl = new PhotonTransformViewRotationControl(this.m_RotationModel);
		this.m_ScaleControl = new PhotonTransformViewScaleControl(this.m_ScaleModel);
	}

	private void Update()
	{
		if (this.m_PhotonView == null || this.m_PhotonView.isMine || !PhotonNetwork.connected)
		{
			return;
		}
		this.UpdatePosition();
		this.UpdateRotation();
		this.UpdateScale();
	}

	private void UpdatePosition()
	{
		if (!this.m_PositionModel.SynchronizeEnabled || !this.m_ReceivedNetworkUpdate)
		{
			return;
		}
		base.transform.localPosition = this.m_PositionControl.UpdatePosition(base.transform.localPosition);
	}

	private void UpdateRotation()
	{
		if (!this.m_RotationModel.SynchronizeEnabled || !this.m_ReceivedNetworkUpdate)
		{
			return;
		}
		base.transform.localRotation = this.m_RotationControl.GetRotation(base.transform.localRotation);
	}

	private void UpdateScale()
	{
		if (!this.m_ScaleModel.SynchronizeEnabled || !this.m_ReceivedNetworkUpdate)
		{
			return;
		}
		base.transform.localScale = this.m_ScaleControl.GetScale(base.transform.localScale);
	}

	public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
	{
		this.m_PositionControl.SetSynchronizedValues(speed, turnSpeed);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		this.m_PositionControl.OnPhotonSerializeView(base.transform.localPosition, stream, info);
		this.m_RotationControl.OnPhotonSerializeView(base.transform.localRotation, stream, info);
		this.m_ScaleControl.OnPhotonSerializeView(base.transform.localScale, stream, info);
		if (!this.m_PhotonView.isMine && this.m_PositionModel.DrawErrorGizmo)
		{
			this.DoDrawEstimatedPositionError();
		}
		if (stream.isReading)
		{
			this.m_ReceivedNetworkUpdate = true;
		}
	}

	private void DoDrawEstimatedPositionError()
	{
		Vector3 networkPosition = this.m_PositionControl.GetNetworkPosition();
		UnityEngine.Debug.DrawLine(networkPosition, base.transform.position, Color.red, 2f);
		UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + Vector3.up, Color.green, 2f);
		UnityEngine.Debug.DrawLine(networkPosition, networkPosition + Vector3.up, Color.red, 2f);
	}

	[SerializeField]
	private PhotonTransformViewPositionModel m_PositionModel = new PhotonTransformViewPositionModel();

	[SerializeField]
	private PhotonTransformViewRotationModel m_RotationModel = new PhotonTransformViewRotationModel();

	[SerializeField]
	private PhotonTransformViewScaleModel m_ScaleModel = new PhotonTransformViewScaleModel();

	private PhotonTransformViewPositionControl m_PositionControl;

	private PhotonTransformViewRotationControl m_RotationControl;

	private PhotonTransformViewScaleControl m_ScaleControl;

	private PhotonView m_PhotonView;

	private bool m_ReceivedNetworkUpdate;
}
