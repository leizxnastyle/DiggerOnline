using System;
using UnityEngine;

public class PhotonTransformViewRotationControl
{
	public PhotonTransformViewRotationControl(PhotonTransformViewRotationModel model)
	{
		this.m_Model = model;
	}

	public Quaternion GetRotation(Quaternion currentRotation)
	{
		switch (this.m_Model.InterpolateOption)
		{
		default:
			return this.m_NetworkRotation;
		case PhotonTransformViewRotationModel.InterpolateOptions.RotateTowards:
			return Quaternion.RotateTowards(currentRotation, this.m_NetworkRotation, this.m_Model.InterpolateRotateTowardsSpeed * Time.deltaTime);
		case PhotonTransformViewRotationModel.InterpolateOptions.Lerp:
			return Quaternion.Lerp(currentRotation, this.m_NetworkRotation, this.m_Model.InterpolateLerpSpeed * Time.deltaTime);
		}
	}

	public void OnPhotonSerializeView(Quaternion currentRotation, PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.m_Model.SynchronizeEnabled)
		{
			return;
		}
		if (stream.isWriting)
		{
			stream.SendNext(currentRotation);
			this.m_NetworkRotation = currentRotation;
		}
		else
		{
			this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();
		}
	}

	private PhotonTransformViewRotationModel m_Model;

	private Quaternion m_NetworkRotation;
}
