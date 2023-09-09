using System;

[Serializable]
public class PhotonTransformViewRotationModel
{
	public bool SynchronizeEnabled;

	public PhotonTransformViewRotationModel.InterpolateOptions InterpolateOption = PhotonTransformViewRotationModel.InterpolateOptions.RotateTowards;

	public float InterpolateRotateTowardsSpeed = 180f;

	public float InterpolateLerpSpeed = 5f;

	public enum InterpolateOptions
	{
		Disabled,
		RotateTowards,
		Lerp
	}
}
