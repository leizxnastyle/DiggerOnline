using System;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{
	public float RotationX
	{
		get
		{
			return this.rotationX;
		}
		set
		{
			this.rotationX = value;
			bool flag = this.disableInput;
			this.disableInput = true;
			this.Update();
			this.disableInput = flag;
		}
	}

	public float RotationY
	{
		get
		{
			return this.rotationY;
		}
		set
		{
			this.rotationY = value;
			bool flag = this.disableInput;
			this.disableInput = true;
			this.Update();
			this.disableInput = flag;
		}
	}

	public Quaternion OriginalRotation
	{
		get
		{
			return this.originalRotation;
		}
		set
		{
			this.originalRotation = value;
			this.Update();
		}
	}

	public float ScopeFactor
	{
		get
		{
			CameraController component = base.transform.root.GetComponent<CameraController>();
			return (!component.Scope || !component.CurGun.sniper) ? 1f : 0.3f;
		}
	}

	private void Update()
	{
	}

	private void Start()
	{
		if (base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().freezeRotation = true;
		}
		this.originalRotation = base.transform.localRotation;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public bool disableInput;

	public RotationAxes axes;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	public float sinematicLerp = 1f;

	public bool invertX;

	public bool invertY;

	private float rotationX;

	private float rotationY;

	private Quaternion originalRotation;
}
