using System;
using System.Collections;
using UnityEngine;

public class MobileControleWorker : MonoBehaviour
{
	private void Awake()
	{
		MobileControleWorker.inst = this;
		base.GetComponent<TouchController>().enabled = false;
	}

	private void Start()
	{
		this._Motor = base.GetComponent<PlayerMotor>();
		this._Controller = base.GetComponent<CharacterController>();
		this._Input = base.gameObject.GetComponent<PlayerInput>();
		if (base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().freezeRotation = true;
		}
		if (this.ctrl != null)
		{
			this.downZone = this.ctrl.GetZone(0);
			this.upZone = this.ctrl.GetZone(1);
			this.walkStick = this.ctrl.GetStick(0);
			this.pauseZone = this.ctrl.GetZone(4);
			this.upZone.Hide();
			this.walkStick.sizeCm = this.walkStick.sizeCm / 600f * (float)Screen.height;
		}
	}

	private void Update()
	{
		if (MainMenu.Instance.CurMenu != Menu.None)
		{
			base.GetComponent<TouchController>().enabled = false;
		}
		else
		{
			base.GetComponent<TouchController>().enabled = true;
		}
		if (TutorialManager.IsStarted && TutorialManager.curentTutorialStep >= 2 && !TutorialManager.hintIsShow && MainMenu.Instance.CurMenu == Menu.None)
		{
			base.GetComponent<TouchController>().enabled = true;
		}
		else if (TutorialManager.IsStarted)
		{
			base.GetComponent<TouchController>().enabled = false;
			this.walkStick.OnReset();
		}
		this.upZone.initiallyDisabled = false;
		if (TutorialManager.IsStarted && TutorialManager.curentTutorialStep == 2)
		{
			this.walkStick.DisableGUI();
			this.downZone.DisableGUI();
		}
		else if (TutorialManager.IsStarted && TutorialManager.curentTutorialStep == 3)
		{
			this.walkStick.EnableGUI();
		}
		else if (TutorialManager.IsStarted && TutorialManager.curentTutorialStep == 4)
		{
			this.downZone.EnableGUI();
		}
		if (this.walkStick.Pressed())
		{
			Vector2 vec = this.walkStick.GetVec();
			float tilt = this.walkStick.GetTilt();
			float angle = this.walkStick.GetAngle();
			TouchStick.StickDir digitalDir = this.walkStick.GetDigitalDir(true);
			Vector3 vec3d = this.walkStick.GetVec3d(false, 0f);
			this.MovePlayerSide(vec3d);
		}
		if (this.walkStick.JustReleased())
		{
			this.MovePlayerSide(Vector3.zero);
		}
		if (this.upZone.Pressed(0) && !this.walkStick.Pressed())
		{
			Vector3 zero = Vector3.zero;
			zero.x *= (float)this._Input.LaderSpeed;
			zero.z *= (float)this._Input.LaderSpeed;
			float num = -1f;
			zero.y = (float)this._Input.LaderSpeed * num;
			this._Controller.Move(base.transform.rotation * zero * Time.deltaTime);
		}
		if (this.downZone.Pressed(0))
		{
			this._Motor.InputJump = true;
			if (!this.walkStick.Pressed() && (!TutorialManager.IsStarted || (TutorialManager.IsStarted && TutorialManager.curentTutorialStep >= 4)))
			{
				Vector3 zero2 = Vector3.zero;
				zero2.x *= (float)this._Input.LaderSpeed;
				zero2.z *= (float)this._Input.LaderSpeed;
				float num2 = 1f;
				zero2.y = (float)this._Input.LaderSpeed * num2;
				this._Controller.Move(base.transform.rotation * zero2 * Time.deltaTime);
			}
		}
		if (this._Input.Ladder && !MainMenu.Instance.Flying && !this.OnFly && !this.OnWait)
		{
			this.OnFly = true;
			MobileControleWorker.inst.upZone.Show();
			MobileControleWorker.inst.upZone.EnableGUI();
		}
		else if (!this._Input.Ladder && !MainMenu.Instance.Flying && this.OnFly && !this.OnWait)
		{
			this.OnFly = false;
			MobileControleWorker.inst.upZone.Hide(true);
			MobileControleWorker.inst.upZone.DisableGUI();
		}
	}

	private IEnumerator WaitAndOff()
	{
		this.OnWait = true;
		yield return new WaitForSeconds(0.5f);
		if (!this._Input.Ladder)
		{
			MobileControleWorker.inst.upZone.Hide(true);
			MobileControleWorker.inst.upZone.DisableGUI();
		}
		else
		{
			this.OnFly = true;
		}
		this.OnWait = false;
		yield break;
	}

	private void MovePlayerSide(Vector3 move_to)
	{
		if (!this._Input.EnableMovement || Chat.IsEnabled())
		{
			return;
		}
		Vector3 vector;
		if (!this._Input.DirectMovement)
		{
			vector = move_to;
		}
		else
		{
			vector = new Vector3(0f, 0f, Mathf.Max(Mathf.Abs(move_to.x), Mathf.Abs(move_to.z)));
		}
		if (vector != Vector3.zero)
		{
			float num = vector.magnitude;
			vector /= num;
			num = Mathf.Min(1f, num);
			num *= num;
			vector *= num;
		}
		float num2 = 0f;
		if (this._Input.Ladder)
		{
			vector.x *= (float)this._Input.LaderSpeed;
			vector.z *= (float)this._Input.LaderSpeed;
			if (this.upZone.Pressed(0))
			{
				num2 = -1f;
			}
			if (this.downZone.Pressed(0))
			{
				num2 = 1f;
			}
			vector.y = (float)this._Input.LaderSpeed * num2;
			this._Motor.InputJump = this.upZone.Pressed(0);
			this._Controller.Move(base.transform.rotation * vector * Time.deltaTime);
		}
		else
		{
			this._Motor.InputMoveDirection = base.transform.rotation * vector;
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.z);
			Vector3 position = CameraController.RaycastCamera.transform.position;
			position.y -= 0.8f;
			Vector3 position2 = CameraController.RaycastCamera.transform.position;
			if (ProfileINI.autoJump)
			{
				Ray ray = new Ray(position, base.gameObject.transform.forward);
				int layerMask = 1 << LayerMask.NameToLayer("Terrain");
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, 1f, layerMask))
				{
					Ray ray2 = new Ray(position2, base.gameObject.transform.forward);
					if (!Physics.Raycast(ray2, out raycastHit, 1f, layerMask))
					{
						this._Motor.InputJump = true;
					}
					else
					{
						this._Motor.InputJump = false;
					}
				}
				else
				{
					this._Motor.InputJump = false;
				}
			}
		}
	}

	public const int STICK_WALK = 0;

	public const int ZONE_DOWN = 0;

	public const int ZONE_UP = 1;

	public const int ZONE_PAUSE = 4;

	public static MobileControleWorker inst;

	public TouchController ctrl;

	private PlayerMotor _Motor;

	private CharacterController _Controller;

	private PlayerInput _Input;

	public TouchZone downZone;

	public TouchZone upZone;

	private TouchStick walkStick;

	private TouchZone pauseZone;

	public bool OnFly;

	public bool OnWait;
}
