using System;

[Serializable]
public class PhotonTransformViewScaleModel
{
	public bool SynchronizeEnabled;

	public PhotonTransformViewScaleModel.InterpolateOptions InterpolateOption;

	public float InterpolateMoveTowardsSpeed = 1f;

	public float InterpolateLerpSpeed;

	public enum InterpolateOptions
	{
		Disabled,
		MoveTowards,
		Lerp
	}
}
