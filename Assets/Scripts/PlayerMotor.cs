using System;
using System.Collections;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
	private void Awake()
	{
		this._Controller = base.GetComponent<CharacterController>();
		this._Tr = base.transform;
	}

	private void Update()
	{
		Vector3 vector = this.Movement.VelocityHide;
		vector = this.ApplyInputVelocityChange(vector);
		vector = this.ApplyGravityAndJumping(vector);
		Vector3 vector2 = Vector3.zero;
		if (this.MoveWithPlatform())
		{
			Vector3 a = this.MovingPlatform.ActivePlatform.TransformPoint(this.MovingPlatform.ActiveLocalPoint);
			vector2 = a - this.MovingPlatform.ActiveGlobalPoint;
			if (vector2 != Vector3.zero)
			{
				this._Controller.Move(vector2);
			}
			Quaternion lhs = this.MovingPlatform.ActivePlatform.rotation * this.MovingPlatform.ActiveLocalRotation;
			float y = (lhs * Quaternion.Inverse(this.MovingPlatform.ActiveGlobalRotation)).eulerAngles.y;
			if (y != 0f)
			{
				this._Tr.Rotate(0f, y, 0f);
			}
		}
		Vector3 position = this._Tr.position;
		Vector3 vector3 = vector * Time.deltaTime;
		float stepOffset = this._Controller.stepOffset;
		Vector3 vector4 = new Vector3(vector3.x, 0f, vector3.z);
		float d = Mathf.Max(stepOffset, vector4.magnitude);
		if (this.Grounded)
		{
			vector3 -= d * Vector3.up;
		}
		this.MovingPlatform.HitPlatform = null;
		this.GroundNormal = Vector3.zero;
		if (this._Controller.enabled)
		{
			this.Movement.CollisionFlags = this._Controller.Move(vector3);
		}
		this.Movement.LastHitPoint = this.Movement.HitPoint;
		this._LastGroundNormal = this.GroundNormal;
		if (this.MovingPlatform.Enabled && this.MovingPlatform.ActivePlatform != this.MovingPlatform.HitPlatform && this.MovingPlatform.HitPlatform != null)
		{
			this.MovingPlatform.ActivePlatform = this.MovingPlatform.HitPlatform;
			this.MovingPlatform.LastMatrix = this.MovingPlatform.HitPlatform.localToWorldMatrix;
			this.MovingPlatform.NewPlatform = true;
		}
		Vector3 vector5 = new Vector3(vector.x, 0f, vector.z);
		this.Movement.VelocityHide = (this._Tr.position - position) / Time.deltaTime;
		Vector3 lhs2 = new Vector3(this.Movement.VelocityHide.x, 0f, this.Movement.VelocityHide.z);
		if (vector5 == Vector3.zero)
		{
			this.Movement.VelocityHide = new Vector3(0f, this.Movement.VelocityHide.y, 0f);
		}
		else
		{
			float value = Vector3.Dot(lhs2, vector5) / vector5.sqrMagnitude;
			this.Movement.VelocityHide = vector5 * Mathf.Clamp01(value) + this.Movement.VelocityHide.y * Vector3.up;
		}
		if (this.Movement.VelocityHide.y < vector.y - 0.001f)
		{
			if (this.Movement.VelocityHide.y < 0f)
			{
				this.Movement.VelocityHide.y = vector.y;
			}
			else
			{
				this.Jumping.HoldingJumpButton = false;
			}
		}
		if (this.Grounded && !this.IsGroundedTest())
		{
			this.Grounded = false;
			if (this.MovingPlatform.Enabled && (this.MovingPlatform.MovementTransfer == PlayerMotor.MovementTransferOnJump.InitTransfer || this.MovingPlatform.MovementTransfer == PlayerMotor.MovementTransferOnJump.PermaTransfer))
			{
				this.Movement.FrameVelocity = this.MovingPlatform.PlatformVelocity;
				this.Movement.VelocityHide += this.MovingPlatform.PlatformVelocity;
			}
			base.SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
			this._Tr.position += d * Vector3.up;
		}
		else if (!this.Grounded && this.IsGroundedTest())
		{
			this.Grounded = true;
			this.Jumping.Jumping = false;
			if (this.MovingPlatform.Enabled)
			{
				base.StartCoroutine(this.SubtractNewPlatformVelocity());
			}
			base.SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
			Vector3 position2 = WorldGameObjectX.Instance.MainPlayer.transform.position;
			BlockType blockType = WorldData.Instance.GetBlockType((int)position2.x, (int)position2.z, (int)(position2.y + 0.01f) - 1);
			if (blockType == BlockType.Gum)
			{
				this.IsNeedJump = true;
			}
			else
			{
				this.JumpSeria = false;
			}
		}
		if (this.MoveWithPlatform())
		{
			this.MovingPlatform.ActiveGlobalPoint = this._Tr.position + Vector3.up * (this._Controller.center.y - this._Controller.height * 0.5f + this._Controller.radius);
			this.MovingPlatform.ActiveLocalPoint = this.MovingPlatform.ActivePlatform.InverseTransformPoint(this.MovingPlatform.ActiveGlobalPoint);
			this.MovingPlatform.ActiveGlobalRotation = this._Tr.rotation;
			this.MovingPlatform.ActiveLocalRotation = Quaternion.Inverse(this.MovingPlatform.ActivePlatform.rotation) * this.MovingPlatform.ActiveGlobalRotation;
		}
	}

	private void FixedUpdate()
	{
		if (this.MovingPlatform.Enabled)
		{
			if (this.MovingPlatform.ActivePlatform != null)
			{
				if (!this.MovingPlatform.NewPlatform)
				{
					this.MovingPlatform.PlatformVelocity = (this.MovingPlatform.ActivePlatform.localToWorldMatrix.MultiplyPoint3x4(this.MovingPlatform.ActiveLocalPoint) - this.MovingPlatform.LastMatrix.MultiplyPoint3x4(this.MovingPlatform.ActiveLocalPoint)) / Time.deltaTime;
				}
				this.MovingPlatform.LastMatrix = this.MovingPlatform.ActivePlatform.localToWorldMatrix;
				this.MovingPlatform.NewPlatform = false;
			}
			else
			{
				this.MovingPlatform.PlatformVelocity = Vector3.zero;
			}
		}
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity)
	{
		if (!this.CanControl)
		{
			this.InputMoveDirection = Vector3.zero;
		}
		Vector3 vector2;
		if (this.Grounded && this.TooSteep())
		{
			Vector3 vector = new Vector3(this.GroundNormal.x, 0f, this.GroundNormal.z);
			vector2 = vector.normalized;
			Vector3 vector3 = Vector3.Project(this.InputMoveDirection, vector2);
			vector2 = vector2 + vector3 * 0.4f + (this.InputMoveDirection - vector3) * 1f;
			vector2 *= 15f;
		}
		else
		{
			vector2 = this.GetDesiredHorizontalVelocity();
		}
		if (this.MovingPlatform.Enabled && this.MovingPlatform.MovementTransfer == PlayerMotor.MovementTransferOnJump.PermaTransfer)
		{
			vector2 += this.Movement.FrameVelocity;
			vector2.y = 0f;
		}
		if (this.Grounded)
		{
			vector2 = this.AdjustGroundVelocityToNormal(vector2, this.GroundNormal);
		}
		else
		{
			velocity.y = 0f;
		}
		float num = this.GetMaxAcceleration(this.Grounded) * Time.deltaTime;
		Vector3 b = vector2 - velocity;
		if (b.sqrMagnitude > num * num)
		{
			b = b.normalized * num;
		}
		if (this.Grounded || this.CanControl)
		{
			velocity += b;
		}
		if (this.Grounded)
		{
			velocity.y = Mathf.Min(velocity.y, 0f);
		}
		return velocity;
	}

	private Vector3 ApplyGravityAndJumping(Vector3 velocity)
	{
		if (!this.InputJump || !this.CanControl)
		{
			this.Jumping.HoldingJumpButton = false;
			this.Jumping.LastButtonDownTime = -100f;
		}
		if (this.InputJump && this.Jumping.LastButtonDownTime < 0f && this.CanControl)
		{
			this.Jumping.LastButtonDownTime = Time.time;
		}
		if (this.Grounded)
		{
			velocity.y = Mathf.Min(0f, velocity.y) - this.Movement.Gravity * Time.deltaTime;
		}
		else
		{
			velocity.y = this.Movement.VelocityHide.y - this.Movement.Gravity * Time.deltaTime;
			velocity.y = Mathf.Max(velocity.y, -this.Movement.MaxFallSpeed);
		}
		if (this.Grounded)
		{
			if ((this.Jumping.Enabled && this.CanControl && Time.time - this.Jumping.LastButtonDownTime < 0.2f) || this.IsNeedJump)
			{
				this.Grounded = false;
				this.Jumping.Jumping = true;
				this.Jumping.LastStartTime = Time.time;
				this.Jumping.LastButtonDownTime = -100f;
				this.Jumping.HoldingJumpButton = true;
				if (this.TooSteep())
				{
					this.Jumping.JumpDir = Vector3.Slerp(Vector3.up, this.GroundNormal, 0.5f);
				}
				else
				{
					this.Jumping.JumpDir = Vector3.Slerp(Vector3.up, this.GroundNormal, 0f);
				}
				if (this.IsNeedJump)
				{
					float num = Mathf.Abs(velocity.y);
					if (this.JumpSeria)
					{
						num = this.LastBouncVel + 1.5f;
					}
					else
					{
						num = 2f;
					}
					this.IsNeedJump = false;
					velocity.y = 0f;
					velocity += this.Jumping.JumpDir * this.CalculateJumpVerticalSpeed(num);
					this.LastBouncVel = num;
					this.JumpSeria = true;
				}
				else
				{
					velocity.y = 0f;
					velocity += this.Jumping.JumpDir * this.CalculateJumpVerticalSpeed(1f);
				}
				if (this.MovingPlatform.Enabled && (this.MovingPlatform.MovementTransfer == PlayerMotor.MovementTransferOnJump.InitTransfer || this.MovingPlatform.MovementTransfer == PlayerMotor.MovementTransferOnJump.PermaTransfer))
				{
					this.Movement.FrameVelocity = this.MovingPlatform.PlatformVelocity;
					velocity += this.MovingPlatform.PlatformVelocity;
				}
				base.SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				this.Jumping.HoldingJumpButton = false;
			}
		}
		return velocity;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.normal.y > 0f && hit.normal.y > this.GroundNormal.y && hit.moveDirection.y < 0f)
		{
			if ((hit.point - this.Movement.LastHitPoint).sqrMagnitude > 0.001f || this._LastGroundNormal == Vector3.zero)
			{
				this.GroundNormal = hit.normal;
			}
			else
			{
				this.GroundNormal = this._LastGroundNormal;
			}
			this.MovingPlatform.HitPlatform = hit.collider.transform;
			this.Movement.HitPoint = hit.point;
			this.Movement.FrameVelocity = Vector3.zero;
		}
	}

	private IEnumerator SubtractNewPlatformVelocity()
	{
		if (this.MovingPlatform.Enabled && (this.MovingPlatform.MovementTransfer == PlayerMotor.MovementTransferOnJump.InitTransfer || this.MovingPlatform.MovementTransfer == PlayerMotor.MovementTransferOnJump.PermaTransfer))
		{
			if (this.MovingPlatform.NewPlatform)
			{
				Transform platform = this.MovingPlatform.ActivePlatform;
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				if (this.Grounded && platform == this.MovingPlatform.ActivePlatform)
				{
					yield return 1;
				}
			}
			this.Movement.VelocityHide -= this.MovingPlatform.PlatformVelocity;
		}
		yield break;
	}

	private bool MoveWithPlatform()
	{
		return this.MovingPlatform.Enabled && (this.Grounded || this.MovingPlatform.MovementTransfer == PlayerMotor.MovementTransferOnJump.PermaLocked) && this.MovingPlatform.ActivePlatform != null;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		Vector3 vector = this._Tr.InverseTransformDirection(this.InputMoveDirection);
		float num = this.MaxSpeedInDirection(vector);
		if (this.Grounded)
		{
			float time = Mathf.Asin(this.Movement.VelocityHide.normalized.y) * 57.29578f;
			num *= this.Movement.SlopeSpeedMultiplier.Evaluate(time);
		}
		return this._Tr.TransformDirection(vector * num);
	}

	private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
	{
		Vector3 lhs = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(lhs, groundNormal).normalized * hVelocity.magnitude;
	}

	private bool IsGroundedTest()
	{
		return this.GroundNormal.y > 0.01f;
	}

	private float GetMaxAcceleration(bool grounded)
	{
		if (grounded)
		{
			return 30f;
		}
		return 10f;
	}

	private float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * this.Movement.Gravity * this.JumpModifier);
	}

	private bool IsJumping()
	{
		return this.Jumping.Jumping;
	}

	private bool IsSliding()
	{
		return this.Grounded && this.Sliding.Enabled && this.TooSteep();
	}

	private bool IsTouchingCeiling()
	{
		return (this.Movement.CollisionFlags & CollisionFlags.Above) != CollisionFlags.None;
	}

	private bool IsGrounded()
	{
		return this.Grounded;
	}

	private bool TooSteep()
	{
		return this.GroundNormal.y <= Mathf.Cos(this._Controller.slopeLimit * 0.0174532924f);
	}

	private Vector3 GetDirection()
	{
		return this.InputMoveDirection;
	}

	private void SetControllable(bool controllable)
	{
		this.CanControl = controllable;
	}

	private float MaxSpeedInDirection(Vector3 desiredMovementDirection)
	{
		if (desiredMovementDirection == Vector3.zero)
		{
			return 0f;
		}
		float num = (!base.GetComponent<PlayerNetwork>().Crouch) ? 1f : 0.3f;
		float num2 = ((desiredMovementDirection.z <= 0f) ? this.Movement.MaxBackwardsSpeed : this.Movement.MaxForwardSpeed) / this.Movement.MaxSidewaysSpeed;
		Vector3 vector = new Vector3(desiredMovementDirection.x, 0f, desiredMovementDirection.z / num2);
		Vector3 normalized = vector.normalized;
		Vector3 vector2 = new Vector3(normalized.x, 0f, normalized.z * num2);
		float num3 = vector2.magnitude * this.Movement.MaxSidewaysSpeed;
		return num3 * num;
	}

	private void SetVelocity(Vector3 velocity)
	{
	}

	[NonSerialized]
	public bool CanControl = true;

	[NonSerialized]
	public SecuredValue<Vector3> InputMoveDirection = Vector3.zero;

	[NonSerialized]
	public SecuredValue<bool> InputJump = false;

	[NonSerialized]
	public SecuredValue<float> JumpModifier = 1f;

	[NonSerialized]
	public PlayerMotor.PlayerMotorMovement Movement = new PlayerMotor.PlayerMotorMovement();

	[NonSerialized]
	public PlayerMotor.PlayerMotorJumping Jumping = new PlayerMotor.PlayerMotorJumping();

	[NonSerialized]
	public PlayerMotor.PlayerMotorMovingPlatform MovingPlatform = new PlayerMotor.PlayerMotorMovingPlatform();

	[NonSerialized]
	public PlayerMotor.PlayerMotorSliding Sliding = new PlayerMotor.PlayerMotorSliding();

	[NonSerialized]
	public bool Grounded = true;

	[NonSerialized]
	public bool IsNeedJump;

	[NonSerialized]
	public bool JumpSeria;

	[NonSerialized]
	public float LastBouncVel;

	[NonSerialized]
	public Vector3 GroundNormal = Vector3.zero;

	private Vector3 _LastGroundNormal = Vector3.zero;

	private Transform _Tr;

	private CharacterController _Controller;

	internal SecuredValue<float> CrouchSpeed = 0f;

	public class PlayerMotorMovement
	{
		public SecuredValue<float> MaxForwardSpeed = 5f;

		public SecuredValue<float> MaxSidewaysSpeed = 5f;

		public SecuredValue<float> MaxBackwardsSpeed = 5f;

		public AnimationCurve SlopeSpeedMultiplier = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(-90f, 1f),
			new Keyframe(0f, 1f),
			new Keyframe(90f, 0f)
		});

		public SecuredValue<float> Gravity = 40f;

		public SecuredValue<float> MaxFallSpeed = 40f;

		public CollisionFlags CollisionFlags;

		public Vector3 VelocityHide;

		public Vector3 FrameVelocity = Vector3.zero;

		public Vector3 HitPoint = Vector3.zero;

		public Vector3 LastHitPoint = new Vector3(float.PositiveInfinity, 0f, 0f);
	}

	public enum MovementTransferOnJump
	{
		None,
		InitTransfer,
		PermaTransfer,
		PermaLocked
	}

	public class PlayerMotorJumping
	{
		public bool Enabled = true;

		public bool Jumping;

		public bool HoldingJumpButton;

		public float LastStartTime;

		public float LastButtonDownTime = -100f;

		public Vector3 JumpDir = Vector3.up;
	}

	public class PlayerMotorMovingPlatform
	{
		public bool Enabled = true;

		public PlayerMotor.MovementTransferOnJump MovementTransfer = PlayerMotor.MovementTransferOnJump.PermaTransfer;

		public Transform HitPlatform;

		public Transform ActivePlatform;

		public Vector3 ActiveLocalPoint;

		public Vector3 ActiveGlobalPoint;

		public Quaternion ActiveLocalRotation;

		public Quaternion ActiveGlobalRotation;

		public Matrix4x4 LastMatrix;

		public Vector3 PlatformVelocity;

		public bool NewPlatform;
	}

	public class PlayerMotorSliding
	{
		public bool Enabled = true;
	}
}
