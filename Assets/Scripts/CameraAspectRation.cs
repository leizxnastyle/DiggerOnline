using System;
using UnityEngine;

public class CameraAspectRation : MonoBehaviour
{
	public static float GetScreenCoff
	{
		get
		{
			return (float)Screen.height / 600f;
		}
	}

	private void Awake()
	{
		UnityEngine.Debug.Log("GetScreenCoff " + CameraAspectRation.GetScreenCoff);
		Vector2 aspectRatio = AspectRatioWorker.GetAspectRatio(Screen.width, Screen.height);
		if (aspectRatio == CameraAspectRation.AR_4_3)
		{
			this.root.manualHeight = 750;
		}
		else if (aspectRatio == CameraAspectRation.AR_3_2)
		{
			this.root.manualHeight = 665;
		}
		else if (aspectRatio == CameraAspectRation.AR_3_2 || aspectRatio == CameraAspectRation.AR_16_10)
		{
			this.root.manualHeight = 622;
		}
		CameraAspectRation.curentScreenAspect = aspectRatio;
	}

	public static readonly Vector2 AR_4_3 = new Vector2(4f, 3f);

	public static readonly Vector2 AR_3_2 = new Vector2(3f, 2f);

	public static readonly Vector2 AR_8_5 = new Vector2(8f, 5f);

	public static readonly Vector2 AR_5_3 = new Vector2(5f, 3f);

	public static readonly Vector2 AR_16_9 = new Vector2(16f, 9f);

	public static readonly Vector2 AR_16_10 = new Vector2(16f, 10f);

	public UIRoot root;

	public static Vector2 curentScreenAspect = Vector2.zero;
}
