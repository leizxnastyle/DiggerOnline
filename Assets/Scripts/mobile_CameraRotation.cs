using System;
using UnityEngine;

public class mobile_CameraRotation : MonoBehaviour
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
			return (!component.Scope || !component.CurGun.sniper) ? 7f : 0.3f;
		}
	}

	private void Start()
	{
		if (this.ctrl != null)
		{
			this.aimZone = this.ctrl.GetZone(3);
			this.originalRotation = base.transform.localRotation;
		}
	}

	private void Update()
	{
		if (!this.disableInput)
		{
			if (!this.isSit)
			{
				if (this.aimZone.UniPressed())
				{
					if (TutorialManager.IsStarted && TutorialManager.curentTutorialStep == 2 && Mathf.Abs(this.rotationX) + Mathf.Abs(this.rotationY) > 45f)
					{
						TutorialManager.Inst.ToNext();
					}
					if (Mathf.Abs(this.rotationX - this.lastX) + Mathf.Abs(this.rotationY - this.lastY) > 5f && !CameraController.Instance.isTouchHit)
					{
						CameraController.Instance.isRotateCamera = true;
					}
					Vector2 uniDragDelta = this.aimZone.GetUniDragDelta(TouchCoordSys.SCREEN_CM, true);
					this.RotateCamera(uniDragDelta);
				}
				else
				{
					if (CameraController.Instance != null)
					{
						CameraController.Instance.isRotateCamera = false;
						CameraController.Instance.isTouchHit = false;
					}
					this.lastX = this.rotationX;
					this.lastY = this.rotationY;
				}
				if (TutorialManager.IsStarted && TutorialManager.hintIsShow)
				{
					this.aimZone.OnReset();
				}
			}
		}
		else
		{
			Quaternion rhs = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			Quaternion rhs2 = Quaternion.AngleAxis(this.rotationY, Vector3.left);
			if (MainMenu.Instance.CinematicCamera)
			{
				base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, this.originalRotation * rhs * rhs2, Time.deltaTime * this.sinematicLerp);
			}
			else
			{
				base.transform.localRotation = this.originalRotation * rhs * rhs2;
			}
		}
	}

	private void RotateCamera(Vector2 rotate_to)
	{
		Input.GetKey(KeyCode.Alpha2);
		if (this.axes == RotationAxes.MouseXAndY)
		{
			if (!this.disableInput)
			{
				this.rotationX += rotate_to.x * ProfileINI.mouse_sens * ((!this.invertX) ? 1f : -1f) * this.ScopeFactor;
				this.rotationY += -rotate_to.y * ProfileINI.mouse_sens * ((!this.invertY) ? 1f : -1f) * this.ScopeFactor;
				this.rotationX = this.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
				this.rotationY = this.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
			}
			Quaternion rhs = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			Quaternion rhs2 = Quaternion.AngleAxis(this.rotationY, Vector3.left);
			if (MainMenu.Instance.CinematicCamera)
			{
				base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, this.originalRotation * rhs * rhs2, Time.deltaTime * this.sinematicLerp);
			}
			else
			{
				base.transform.localRotation = this.originalRotation * rhs * rhs2;
			}
		}
		else if (this.axes == RotationAxes.MouseX)
		{
			if (!this.disableInput)
			{
				this.rotationX += rotate_to.x * ProfileINI.mouse_sens * ((!this.invertX) ? 1f : -1f) * this.ScopeFactor;
				this.rotationX = this.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
			}
			Quaternion rhs3 = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			if (MainMenu.Instance.CinematicCamera)
			{
				base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, this.originalRotation * rhs3, Time.deltaTime * this.sinematicLerp);
			}
			else
			{
				base.transform.localRotation = this.originalRotation * rhs3;
			}
		}
		else
		{
			if (!this.disableInput)
			{
				this.rotationY += -rotate_to.y * ProfileINI.mouse_sens * ((!this.invertY) ? 1f : -1f) * this.ScopeFactor;
				this.rotationY = this.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
			}
			Quaternion rhs4 = Quaternion.AngleAxis(this.rotationY, Vector3.left);
			if (MainMenu.Instance.CinematicCamera)
			{
				base.transform.localRotation = Quaternion.Lerp(base.transform.localRotation, this.originalRotation * rhs4, Time.deltaTime * this.sinematicLerp);
			}
			else
			{
				base.transform.localRotation = this.originalRotation * rhs4;
			}
		}
	}

	private float ClampAngle(float angle, float min, float max)
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

	public const int ZONE_AIM = 3;

	public TouchController ctrl;

	private TouchZone aimZone;

	public PlayerInput _Input;

	public RotationAxes axes;

	public bool disableInput;

	public float minimumX = -360f;

	public float maximumX = 360f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	private float rotationX;

	private float rotationY;

	private Quaternion originalRotation;

	public bool invertX;

	public bool invertY;

	public float sinematicLerp = 1f;

	public bool isSit;

	private float lastX;

	private float lastY;
}
