using System;
using UnityEngine;

public class SticScaleWorker : MonoBehaviour
{
	private void Start()
	{
		this.walkStick = this.tc.GetStick(0);
		this.downZone = this.tc.GetZone(0);
		this.upZone = this.tc.GetZone(1);
		if (CameraAspectRation.curentScreenAspect == CameraAspectRation.AR_4_3)
		{
			this.downZone.regionRect = new Rect(0.65f, 0.83f, 0.11f, 0.15f);
			this.upZone.regionRect = new Rect(0.82f, 0.83f, 0.11f, 0.15f);
		}
		else if (CameraAspectRation.curentScreenAspect == CameraAspectRation.AR_3_2)
		{
			this.downZone.regionRect = new Rect(0.65f, 0.83f, 0.11f, 0.15f);
			this.upZone.regionRect = new Rect(0.82f, 0.83f, 0.11f, 0.15f);
		}
		this.downZone.OnLayout();
		this.upZone.OnLayout();
	}

	private void Update()
	{
		if (Screen.width <= 1920)
		{
			if (this.walkStick.sizeCm != 1.7f)
			{
				this.walkStick.sizeCm = 1.7f;
				this.walkStick.OnLayout();
			}
		}
		else if (this.walkStick.sizeCm != 2.3f)
		{
			this.walkStick.sizeCm = 2.3f;
			this.walkStick.OnLayout();
		}
	}

	private const int STICK_WALK = 0;

	public const int ZONE_DOWN = 0;

	public const int ZONE_UP = 1;

	public TouchController tc;

	private TouchStick walkStick;

	private TouchZone downZone;

	private TouchZone upZone;
}
