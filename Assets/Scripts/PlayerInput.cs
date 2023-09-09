using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public bool EnableMovement
	{
		get
		{
			return this._EnableMovement.Value;
		}
	}

	public bool Ladder
	{
		get
		{
			return this._Ladder;
		}
	}

	public int LaderSpeed
	{
		get
		{
			return this._LaderSpeed;
		}
	}

	public bool DirectMovement
	{
		get
		{
			return this._DirectMovement;
		}
	}

	public bool isLadder
	{
		get
		{
			return this._Ladder;
		}
	}

	public bool UnderWater
	{
		get
		{
			return this._UnderWater;
		}
	}

	public float Gravity
	{
		get
		{
			return this._Motor.Movement.Gravity;
		}
	}

	public float JumpModifier
	{
		get
		{
			return this._Motor.JumpModifier;
		}
	}

	public bool Grounded
	{
		get
		{
			return this._Motor.Grounded;
		}
	}

	private void Awake()
	{
		this._Motor = base.GetComponent<PlayerMotor>();
		this._Controller = base.GetComponent<CharacterController>();
		this._EnableMovement = true;
		this._UnderWater = false;
		this._WaterTime = 0f;
		this._WaterSpeed = 3;
		this._WaterVelocity = 0f;
		this._Ladder = false;
		this._LaderSpeed = 4;
		this._OnIce = false;
		this._DirectMovement = false;
		this._CurVelocity = Vector3.zero;
		this._SpeedUp = false;
		this._Modifier = 1f;
	}

	private void SetDirectMovement(bool enabled)
	{
		this._DirectMovement = enabled;
	}

	public void SetMovement(bool move)
	{
		this._EnableMovement = move;
		if (!move)
		{
			this.StopMovement();
		}
	}

	public void StopMovement()
	{
		this._Motor.InputMoveDirection = new Vector3(0f, 0f, 0f);
		this._Motor.InputJump = false;
	}

	public void SetSpeedUp(bool speedUp)
	{
		this._SpeedUp = speedUp;
		if (this._SpeedUp)
		{
			this._Motor.Movement.MaxForwardSpeed = 13f * this._Modifier;
			this._Motor.Movement.MaxSidewaysSpeed = 13f * this._Modifier;
			this._Motor.Movement.MaxBackwardsSpeed = 13f * this._Modifier;
			this._WaterSpeed = Mathf.RoundToInt(8f * this._Modifier);
			this._LaderSpeed = Mathf.RoundToInt(12f * this._Modifier);
		}
		else
		{
			this._Motor.Movement.MaxForwardSpeed = 5f * this._Modifier;
			this._Motor.Movement.MaxSidewaysSpeed = 5f * this._Modifier;
			this._Motor.Movement.MaxBackwardsSpeed = 5f * this._Modifier;
			this._WaterSpeed = Mathf.RoundToInt(3f * this._Modifier);
			this._LaderSpeed = Mathf.RoundToInt(5f * this._Modifier);
		}
	}

	public void SetSpeedModifier(float modifier)
	{
		this._Modifier = modifier;
		this.SetSpeedUp(this._SpeedUp);
	}

	private void SetIceBody(bool ice)
	{
		if (ice && !this._OnIce)
		{
			this._CurVelocity = this._Motor.Movement.VelocityHide * 0.2f;
		}
		this._OnIce = ice;
	}

	private void SetUnderWaterBody(bool[] water_data)
	{
		if (water_data[0])
		{
			if (!this._UnderWater)
			{
				this._Motor.Movement.Gravity = 0f;
				this._Motor.Movement.VelocityHide = Vector3.zero;
				this._Motor.Movement.FrameVelocity = Vector3.zero;
				this._Motor.InputMoveDirection = Vector3.zero;
				this._UnderWater = water_data[0];
				this._WaterTime = Time.time + 1f;
			}
			else if (water_data[1])
			{
				this._WaterTime = Time.time + 0.5f;
			}
		}
		else if (!water_data[0] && this._UnderWater && !this._Ladder)
		{
			this._Motor.Movement.Gravity = 40f;
			this._UnderWater = water_data[0];
		}
	}

	private void SetLadderBody(bool _ladder)
	{
		this._Ladder = _ladder;
		if (this._Ladder)
		{
			this._UnderWater = false;
			this._Motor.Movement.Gravity = 0f;
			this._Motor.Movement.VelocityHide = Vector3.zero;
			this._Motor.Movement.FrameVelocity = Vector3.zero;
			this._Motor.InputMoveDirection = Vector3.zero;
		}
		else
		{
			this._Motor.Movement.Gravity = 40f;
		}
	}

	private void UpdateWater()
	{
		Vector3 zero = Vector3.zero;
		if (this._WaterVelocity < 0f)
		{
			zero.y += this._WaterVelocity;
		}
		this._Controller.Move(base.transform.rotation * zero * Time.deltaTime);
	}

	private void Update()
	{
		if ((!this._EnableMovement || Chat.IsEnabled()) && !this.UnderWater)
		{
			return;
		}
		if ((!this._EnableMovement || Chat.IsEnabled()) && this.UnderWater && !MainMenu.Instance.Flying)
		{
			this.UpdateWater();
			return;
		}
		Vector3 vector;
		if (!this._DirectMovement)
		{
			vector = new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0f, UnityEngine.Input.GetAxis("Vertical"));
		}
		else
		{
			vector = new Vector3(0f, 0f, Mathf.Max(Mathf.Abs(UnityEngine.Input.GetAxis("Horizontal")), Mathf.Abs(UnityEngine.Input.GetAxis("Vertical"))));
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
		if (this._Ladder)
		{
			vector.x *= (float)this._LaderSpeed;
			vector.z *= (float)this._LaderSpeed;
			if (UnityEngine.Input.GetKey("space"))
			{
				num2 = 1f;
			}
			if (UnityEngine.Input.GetKey("left shift"))
			{
				num2 = -1f;
			}
			vector.y = (float)this._LaderSpeed * num2;
			this._Motor.InputJump = Input.GetButton("Jump");
			this._Controller.Move(base.transform.rotation * vector * Time.deltaTime);
		}
		else if (this._UnderWater)
		{
			this._Motor.Movement.Gravity = 0f;
			vector.x *= (float)this._WaterSpeed;
			vector.z *= (float)this._WaterSpeed;
			if (UnityEngine.Input.GetKey("space") && Time.time >= this._WaterTime)
			{
				this._WaterVelocity = Mathf.MoveTowards(this._WaterVelocity, 1f, Time.deltaTime * 15f);
				if (this._WaterVelocity > 0f)
				{
					num2 = this._WaterVelocity;
				}
			}
			else
			{
				this._WaterVelocity = Mathf.MoveTowards(this._WaterVelocity, -1f, Time.deltaTime * 15f);
			}
			if (UnityEngine.Input.GetKey("left shift"))
			{
				num2 = -1f;
			}
			vector.y = (float)this._WaterSpeed * num2;
			this._Motor.InputJump = Input.GetButton("Jump");
			if (this._WaterVelocity < 0f)
			{
				vector.y += this._WaterVelocity;
			}
			this._Controller.Move(base.transform.rotation * vector * Time.deltaTime);
		}
		else if (this._OnIce)
		{
			Vector3 b = base.transform.TransformDirection(vector);
			this._CurVelocity = Vector3.Lerp(this._CurVelocity, b, 1.25f * Time.deltaTime);
			this._Motor.InputMoveDirection = this._CurVelocity;
			this._Motor.InputJump = Input.GetButton("Jump");
		}
		else
		{
			this._Motor.InputMoveDirection = base.transform.rotation * vector;
			this._Motor.InputJump = Input.GetButton("Jump");
		}
	}

	private PlayerMotor _Motor;

	private CharacterController _Controller;

	private SecuredValue<bool> _EnableMovement;

	private SecuredValue<bool> _UnderWater;

	private SecuredValue<float> _WaterTime;

	private SecuredValue<int> _WaterSpeed;

	private SecuredValue<float> _WaterVelocity;

	private SecuredValue<bool> _Ladder;

	private SecuredValue<int> _LaderSpeed;

	private SecuredValue<bool> _OnIce;

	private SecuredValue<bool> _DirectMovement;

	private SecuredValue<Vector3> _CurVelocity;

	private SecuredValue<bool> _SpeedUp;

	private SecuredValue<float> _Modifier;
}
