using System;
using System.Collections.Generic;
using UnityEngine;

public class Trolley : EntityBase
{
	public GameObject Player
	{
		get
		{
			return this._Player;
		}
	}

	public void OnManualMove()
	{
		this.Stop();
		this.StartFreeFall(Vector3.down, 1f);
	}

	public void TryAttach(Rail toRail = null, Collider toCollider = null)
	{
		if (this._CurRail != null)
		{
			return;
		}
		if (toRail != null)
		{
			this._CurRail = toRail;
		}
		else if (toCollider != null)
		{
			this._CurRail = Rail.Find(toCollider);
		}
		else
		{
			this._CurRail = Rail.Find(base.BlockX, base.BlockY, (int)(base.transform.position.y + 0.2f));
		}
		this._CurRailExt = null;
		this._NextRail = null;
		this.RefreshRotation();
		Trolley._DetachedTrolleys.Remove(this);
		if (this._CurRail != null)
		{
			Trolley._AttachedTrolleys.Add(this);
		}
		else
		{
			Trolley._DetachedTrolleys.Add(this);
		}
	}

	public static void OnRailAttached(Rail rail)
	{
		int blockX = rail.BlockX;
		int blockY = rail.BlockY;
		int blockZ = rail.BlockZ;
		foreach (Trolley trolley in Trolley._DetachedTrolleys.ToArray())
		{
			if (!(trolley == null) && !(trolley.photonView == null) && trolley.photonView.isMine)
			{
				if (!trolley._FreeFall && trolley.BlockX == blockX && trolley.BlockY == blockY && trolley.BlockZ == blockZ)
				{
					Trolley._DetachedTrolleys.Remove(trolley);
					Trolley._AttachedTrolleys.Add(trolley);
					trolley._CurRail = rail;
				}
			}
		}
	}

	public static void OnRailDetached(Rail rail)
	{
		foreach (Trolley trolley in Trolley._AttachedTrolleys.ToArray())
		{
			if (!(trolley == null) && !(trolley.photonView == null) && trolley.photonView.isMine)
			{
				if (trolley._CurRail == rail || trolley._CurRailExt == rail || trolley._NextRail == rail)
				{
					trolley.Stop();
					trolley._CurRail = null;
					trolley._CurRailExt = null;
					Trolley._AttachedTrolleys.Remove(trolley);
					Trolley._DetachedTrolleys.Add(trolley);
				}
			}
		}
	}

	public static void KickPlayer(GameObject player)
	{
		if (player == null)
		{
			return;
		}
		bool flag = false;
		foreach (Trolley trolley in Trolley._AttachedTrolleys)
		{
			if (trolley._Player == player)
			{
				trolley.PlayerExit(false);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			foreach (Trolley trolley2 in Trolley._DetachedTrolleys)
			{
				if (trolley2._Player == player)
				{
					trolley2.PlayerExit(false);
					break;
				}
			}
		}
	}

	protected override void Creation(object[] data)
	{
		this._Position = base.transform.position;
		this._Rotation = base.transform.rotation;
		this.TryAttach(null, null);
	}

	protected override void Destruction()
	{
		Trolley._AttachedTrolleys.Remove(this);
		Trolley._DetachedTrolleys.Remove(this);
		this._CurRail = null;
		this._CurRailExt = null;
		this._NextRail = null;
		if (this._Player != null)
		{
			this.PlayerExit(false);
		}
	}

	private void Push(Vector3 direction, float speed)
	{
		if (this._CurRail == null)
		{
			return;
		}
		if (this._NextRail != null)
		{
			Rail next = this._CurRail.GetNext(direction, 0f);
			if (next == this._NextRail)
			{
				if (this._CurSpeed < speed)
				{
					this._CurSpeed = speed;
				}
				return;
			}
			this.Stop();
		}
		this._NextRail = this._CurRail.GetNext(direction, 0f);
		if (this._NextRail != null)
		{
			if (this._CurRailExt != null && this._CurRailExt != this._NextRail)
			{
				this._NextRail = this._CurRail;
				this._CurRail = this._CurRailExt;
			}
			this._CurRailExt = null;
			this._CurSpeed = speed;
			if (Vector3.Dot(-base.transform.forward, (this._NextRail.transform.position - this._CurRail.transform.position).normalized) < 0f)
			{
				this._MoveDir = -1f;
			}
			else
			{
				this._MoveDir = 1f;
			}
			this.RefreshRotation();
		}
	}

	private void Stop()
	{
		if (this._NextRail == null)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, this._NextRail.transform.position) < Vector3.Distance(base.transform.position, this._CurRail.transform.position))
		{
			Rail curRail = this._CurRail;
			this._CurRail = this._NextRail;
			this._NextRail = curRail;
		}
		this._CurRailExt = ((!(this._CurRail.transform.position != this._NextRail.transform.position)) ? null : this._NextRail);
		this._CurRailExt = this._NextRail;
		this._NextRail = null;
	}

	private void RefreshRotation()
	{
		if (this._NextRail == null)
		{
			return;
		}
		float num = this._CurRail.GetRotationToRail(this._NextRail) - ((this._MoveDir >= 0f) ? 0f : 180f);
		if (this._CurRotation.y != num)
		{
			if (Mathf.Abs(Mathf.DeltaAngle(this._CurRotation.y, num)) < 100f)
			{
				this._CurRotation.y = Mathf.MoveTowardsAngle(this._CurRotation.y, num, 720f * Time.deltaTime);
			}
			else
			{
				this._CurRotation.y = num;
			}
		}
		this.Model.transform.localPosition = Vector3.zero;
		this._RotationSpeedFactor = 1f;
		int blockX = this._CurRail.BlockX;
		int blockY = this._CurRail.BlockY;
		int blockZ = this._CurRail.BlockZ;
		Rail.SetPlaneBlock(this._CurRail.PlaneIndex, ref blockX, ref blockY, ref blockZ);
		int blockX2 = this._NextRail.BlockX;
		int blockY2 = this._NextRail.BlockY;
		int blockZ2 = this._NextRail.BlockZ;
		Rail.SetPlaneBlock(this._CurRail.PlaneIndex, ref blockX2, ref blockY2, ref blockZ2);
		if (this._CurRail.IsTransition && this._NextRail.IsTransition)
		{
			this._CurRotation.x = ((blockZ2 <= blockZ) ? -45f : 45f);
			this.Model.Translate(0f, 0.35f, 0f, Space.Self);
		}
		else if (this._CurRail.IsTransition || this._NextRail.IsTransition)
		{
			float num2 = 1f - Vector3.Distance(this._NextRail.transform.position, base.transform.position) / Vector3.Distance(this._CurRail.transform.position, this._NextRail.transform.position);
			num2 = Mathf.Clamp(num2, 0f, 1f);
			if (blockZ2 > blockZ || (this._NextRail.IsTransition && blockZ == blockZ2) || (!this._NextRail.IsTransition && blockZ2 < blockZ))
			{
				if (!this._CurRail.IsTransition)
				{
					if (this._CurRail.PlaneIndex == this._NextRail.PlaneIndex)
					{
						this._CurRotation.x = 45f * num2;
						this.Model.Translate(0f, 0.35f * num2, 0f, Space.Self);
					}
					else if (num2 < 0.5f)
					{
						num2 *= 2f;
						this._CurRotation.x = 45f * num2;
						this.Model.Translate(0f, 0f, 0f, Space.Self);
					}
					else
					{
						num2 = (num2 - 0.5f) * 2f;
						this._CurRotation.x = 45f;
						this.Model.Translate(0f, 0.35f * num2, 0f, Space.Self);
					}
				}
				else if (!this._NextRail.IsTransition && blockZ2 < blockZ)
				{
					num2 = 1f - num2;
					if (num2 < 0.5f)
					{
						num2 = 1f - num2 * 2f;
						this._CurRotation.x = 0f;
						this.Model.Translate(0f, 0.2f - 0.2f * num2, 0f, Space.Self);
					}
					else if (num2 < 0.75f)
					{
						this._RotationSpeedFactor = 0.5f;
						num2 = (num2 - 0.5f) * 4f;
						this._CurRotation.x = 45f * num2;
						this.Model.Translate(0f, 0.35f - 0.15f * (1f - num2), 0f, Space.Self);
					}
					else
					{
						this._CurRotation.x = 45f;
						this.Model.Translate(0f, 0.35f, 0f, Space.Self);
					}
					this._CurRotation.x = this._CurRotation.x - 90f;
				}
				else if (blockX - blockX2 != 0 || blockY - blockY2 != 0)
				{
					num2 = 1f - num2;
					if (num2 < 0.5f)
					{
						this._CurRotation.x = 0f;
						this.Model.Translate(0f, 0.5f * num2 * 2f, 0f, Space.Self);
					}
					else if (num2 < 0.75f)
					{
						this._RotationSpeedFactor = 0.5f;
						num2 = (num2 - 0.5f) * 4f;
						this._CurRotation.x = 45f * num2;
						this.Model.Translate(0f, 0.5f - 0.15f * num2, 0f, Space.Self);
					}
					else
					{
						this._CurRotation.x = 45f;
						this.Model.Translate(0f, 0.35f, 0f, Space.Self);
					}
				}
				else if (num2 < 0.5f)
				{
					this._CurRotation.x = 45f;
					this.Model.Translate(0f, 0.35f - 0.35f * num2 * 2f, 0f, Space.Self);
				}
				else
				{
					num2 = (num2 - 0.5f) * 2f;
					this._CurRotation.x = 45f + 45f * num2;
				}
			}
			else if (!this._CurRail.IsTransition)
			{
				if (num2 < 0.5f)
				{
					this._CurRotation.x = 0f;
					if (this._CurRail.PlaneIndex == this._NextRail.PlaneIndex)
					{
						this.Model.Translate(0f, 0.5f * num2 * 2f, 0f, Space.Self);
					}
					else
					{
						this.Model.Translate(0f, 0.2f * num2 * 2f, 0f, Space.Self);
					}
				}
				else if (num2 < 0.75f)
				{
					this._RotationSpeedFactor = 0.75f;
					num2 = (num2 - 0.5f) * 4f;
					this._CurRotation.x = 360f - 45f * num2;
					if (this._CurRail.PlaneIndex == this._NextRail.PlaneIndex)
					{
						this.Model.Translate(0f, 0.5f - 0.15f * num2, 0f, Space.Self);
					}
					else
					{
						this.Model.Translate(0f, 0.2f + 0.15f * num2, 0f, Space.Self);
					}
				}
				else
				{
					this._CurRotation.x = -45f;
					this.Model.Translate(0f, 0.35f, 0f, Space.Self);
				}
			}
			else
			{
				num2 = 1f - num2;
				this._CurRotation.x = -45f * num2;
				this.Model.Translate(0f, 0.35f * num2, 0f, Space.Self);
			}
		}
		else
		{
			this._CurRotation.x = 0f;
		}
		this._CurRotation.x = this._CurRotation.x * this._MoveDir;
		base.transform.rotation = Quaternion.Euler(Rail.PlaneRotation[this._CurRail.PlaneIndex]);
		base.transform.Rotate(this._CurRotation, Space.Self);
		if (this._Player != null)
		{
			this.PlayerFixPosition();
		}
	}

	protected override void Updating()
	{
		if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this._Position, Time.deltaTime * 5f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this._Rotation, Time.deltaTime * 5f);
			this.Model.transform.localPosition = Vector3.Lerp(this.Model.transform.localPosition, this._PositionModel, Time.deltaTime * 5f);
			if (this._Player != null)
			{
				this.PlayerFixPosition();
			}
			foreach (Transform transform in this.Wheels)
			{
				transform.Rotate(0f, this._WheelsSpeed * Time.deltaTime, 0f, Space.Self);
			}
			return;
		}
		if (this._FreeFall)
		{
			if (this._Player != null)
			{
				this.PlayerFixPosition();
			}
			if (base.GetComponent<Rigidbody>().velocity == Vector3.zero)
			{
				this.StopFreeFall();
				this.TryAttach(null, this.RailConnectCollider);
			}
			base.FixWorldBounds();
			return;
		}
		if (this._CurRail != null)
		{
			UnityEngine.Debug.DrawLine(base.transform.position + Vector3.up, this._CurRail.transform.position, Color.red);
		}
		if (this._CurRailExt != null)
		{
			UnityEngine.Debug.DrawLine(base.transform.position + Vector3.up, this._CurRailExt.transform.position, Color.green);
		}
		if (this._NextRail != null)
		{
			UnityEngine.Debug.DrawLine(base.transform.position + Vector3.up, this._NextRail.transform.position, Color.blue);
		}
		if (this._Player != null && this._Player == WorldGameObjectX.Instance.MainPlayer && this._CurRail != null)
		{
			if (this._NextRail != null)
			{
				bool flag = ((!this._ReverseMove) ? -1f : 1f) * this._MoveDir > 0f;
				if ((flag && UnityEngine.Input.GetAxis("Vertical") > 0f) || (!flag && UnityEngine.Input.GetAxis("Vertical") < 0f))
				{
					this._CurSpeed = Mathf.MoveTowards(this._CurSpeed, 0f, -1.5f * Time.deltaTime);
				}
				if ((flag && UnityEngine.Input.GetAxis("Vertical") < 0f) || (!flag && UnityEngine.Input.GetAxis("Vertical") > 0f))
				{
					this._CurSpeed = Mathf.MoveTowards(this._CurSpeed, 0f, 2.5f * Time.deltaTime);
				}
			}
			if (this._NextRail == null || this._CurSpeed == 0f)
			{
				if (UnityEngine.Input.GetAxis("Vertical") > 0f)
				{
					this.Push(base.transform.forward * ((!this._ReverseMove) ? 1f : -1f), 1f);
				}
				else if (UnityEngine.Input.GetAxis("Vertical") < 0f)
				{
					this.Push(-base.transform.forward * ((!this._ReverseMove) ? 1f : -1f), 1f);
				}
			}
		}
		if (this._NextRail != null)
		{
			float num = base.transform.rotation.eulerAngles.x;
			if (num != 0f)
			{
				if (this._MoveDir < 0f)
				{
					num = 360f - num;
				}
				if (num < 180f)
				{
					this._CurSpeed = Mathf.MoveTowards(this._CurSpeed, 0f, Mathf.Max(num / 90f * 1.5f, 0.3f) * Time.deltaTime);
				}
				else
				{
					this._CurSpeed = Mathf.MoveTowards(this._CurSpeed, 0f, -((360f - num) / 90f) * 1f * Time.deltaTime);
				}
			}
			else
			{
				this._CurSpeed = Mathf.MoveTowards(this._CurSpeed, 0f, 0.3f * Time.deltaTime);
			}
			if (this._CurSpeed <= 1f && (this._CurRail.PlaneIndex == 1 || (this._CurRailExt != null && this._CurRailExt.PlaneIndex == 1)))
			{
				Vector3 normalized = (this._NextRail.transform.position - this._CurRail.transform.position).normalized;
				this.Stop();
				this.StartFreeFall(normalized, this._CurSpeed * 1f);
			}
			else if (this._CurSpeed == 0f)
			{
				this.Stop();
				Rail down = this._CurRail.GetDown();
				if (down == null && this._CurRailExt != null)
				{
					down = this._CurRailExt.GetDown();
				}
				if (down != null)
				{
					this.Push((down.transform.position - this._CurRail.transform.position).normalized, 0.01f);
				}
			}
			if (this._NextRail != null)
			{
				if (this._CurSpeed > 12f)
				{
					this._CurSpeed = 12f;
				}
				base.transform.position = Vector3.MoveTowards(base.transform.position, this._NextRail.transform.position, this._CurSpeed * Time.deltaTime * this._RotationSpeedFactor);
				foreach (Transform transform2 in this.Wheels)
				{
					transform2.Rotate(0f, -220f * this._CurSpeed * this._MoveDir * Time.deltaTime, 0f, Space.Self);
				}
				if (base.transform.position == this._NextRail.transform.position)
				{
					float closestAngle = 0f;
					if (this._Player != null && this._Player == WorldGameObjectX.Instance.MainPlayer)
					{
						if (UnityEngine.Input.GetKey(KeyCode.A))
						{
							closestAngle = 90f;
						}
						else if (UnityEngine.Input.GetKey(KeyCode.D))
						{
							closestAngle = 270f;
						}
					}
					Vector3 normalized2 = (this._NextRail.transform.position - this._CurRail.transform.position).normalized;
					this._CurRail = this._NextRail;
					this._NextRail = this._NextRail.GetNext(normalized2, closestAngle);
					if (this._NextRail == null && this._CurSpeed >= 1f)
					{
						this.StartFreeFall(normalized2, this._CurSpeed * 1f);
					}
				}
				this.RefreshRotation();
				base.FixWorldBounds();
			}
		}
	}

	protected override void PreviewUpdating()
	{
		Vector3 target = (!(Rail.Find(base.BlockX, base.BlockY, base.BlockZ) != null)) ? new Vector3(0f, -0.1f, 0f) : Vector3.zero;
		this.Model.localPosition = Vector3.MoveTowards(this.Model.localPosition, target, Time.deltaTime * 0.5f);
		base.transform.localRotation = Quaternion.RotateTowards(base.transform.localRotation, Quaternion.identity, Time.deltaTime * 360f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (base.IsPreview || !base.photonView.isMine)
		{
			return;
		}
		if (this._CurRail == null)
		{
			if (this._FreeFall)
			{
				EntityBase entityBase = EntityBase.FindEntity(other.gameObject);
				if (entityBase is Rail && (entityBase != this._FreeFallRail || this._FreeFallTime + 2f < Time.time))
				{
					Vector3 velocity = base.GetComponent<Rigidbody>().velocity;
					this.StopFreeFall();
					this.TryAttach(entityBase as Rail, null);
					this.Push(velocity.normalized, velocity.magnitude / 2f);
				}
			}
			return;
		}
		CharacterController component = other.gameObject.GetComponent<CharacterController>();
		if (component != null)
		{
			this.Push((base.transform.position - component.transform.position).normalized, 3.3f);
		}
		else
		{
			EntityBase entityBase2 = EntityBase.FindEntity(other.gameObject);
			if (entityBase2 != null && !(entityBase2 is Rail))
			{
				if (entityBase2 is Trolley)
				{
					Trolley trolley = entityBase2 as Trolley;
					Vector3 normalized = (base.transform.position - trolley.transform.position).normalized;
					this.Push(normalized, 1f);
				}
				else if (this._CurSpeed < 6f)
				{
					float speed = this._CurSpeed / 2f;
					Vector3 a = (!(this._NextRail != null)) ? base.transform.forward : (this._NextRail.transform.position - this._CurRail.transform.position).normalized;
					this.Stop();
					this.Push(-a, speed);
				}
				else
				{
					entityBase2.SelfDelete();
				}
			}
		}
	}

	public override void OnButtonF(string playerName)
	{
		if (this._CurRail == null && (base.transform.eulerAngles.x != 0f || base.transform.eulerAngles.z != 0f))
		{
			return;
		}
		GameObject mainPlayer = WorldGameObjectX.Instance.MainPlayer;
		if (!(this._Player != null))
		{
			Trolley.KickPlayer(mainPlayer);
			if (this._CurRail == null)
			{
				this.TryAttach(null, null);
			}
			this.PlayerEnter(mainPlayer, false);
			return;
		}
		if (this._Player != mainPlayer)
		{
			return;
		}
		this.PlayerExit(false);
	}

	protected void PlayerEnter(GameObject player, bool others = false)
	{
		this._Player = player;
		this._Player.GetComponent<PlayerNetwork>().InVehicle = true;
		this._Player.GetComponent<PlayerNetwork>().InTrolley = true;
		if (this._Player == WorldGameObjectX.Instance.MainPlayer)
		{
			this._PlayerCamera = this._Player.transform.Find("recoil/Main Camera");
			this._PlayerCameraOffset = this._PlayerCamera.transform.localPosition;
			this._MouseLookHor = this._Player.GetComponent<MouseLook>();
			this._MouseLookVer = this._PlayerCamera.GetComponent<MouseLook>();
			CameraController component = this._Player.GetComponent<CameraController>();
			this._PlayerCameraThirdPerson = component.IsThirdPerson;
			this._PlayerCameraFreeLook = component.IsFreeLook;
			if (!component.IsThirdPerson || !component.IsFreeLook)
			{
				component.SetThirdPerson(true, 0f, true);
			}
			component.DisableCameraSwitching = true;
			this._MouseLookHor.transform.rotation = this.Model.transform.rotation;
			this._MouseLookHor.OriginalRotation = this._MouseLookHor.transform.localRotation;
			this._ReverseMove = (Vector3.Dot(CameraController.RaycastCamera.transform.forward, base.transform.forward) < 0f);
			this._MouseLookHor.RotationX = (float)((!this._ReverseMove) ? 0 : 180);
		}
		if (this._Player.GetComponent<CharacterController>() != null)
		{
			this._Player.GetComponent<CharacterController>().enabled = false;
		}
		if (this._Player.GetComponent<PlayerMotor>() != null)
		{
			this._Player.GetComponent<PlayerMotor>().enabled = false;
		}
		if (this._Player.GetComponent<PlayerInput>() != null)
		{
			this._Player.GetComponent<PlayerInput>().SetMovement(false);
		}
		this.PlayerFixPosition();
		if (this._Player == WorldGameObjectX.Instance.MainPlayer)
		{
			GameObject gameObject = new GameObject("trolley_player");
			TrolleyPlayer trolleyPlayer = gameObject.AddComponent<TrolleyPlayer>();
			trolleyPlayer.Initialize(this, this._Player);
		}
		if (!others)
		{
			base.photonView.isForcedSending = 1;
			base.photonView.RPC("PlayerEnterOthers", PhotonTargets.Others, new object[]
			{
				PhotonNetwork.player
			});
		}
	}

	[PunRPC]
	protected void PlayerEnterOthers(PhotonPlayer owner)
	{
		base.photonView.isForcedSending = -1;
		this.Stop();
		if (this._CurRail != null)
		{
			Trolley._AttachedTrolleys.Remove(this);
		}
		else
		{
			Trolley._DetachedTrolleys.Remove(this);
		}
		this._CurRail = null;
		this._CurRailExt = null;
		this._NextRail = null;
		this.PlayerEnter(WorldGameObjectX.Instance.FindPlayer(owner).Avatar, true);
	}

	public void PlayerExit(bool others = false)
	{
		if (this._Player.GetComponent<CharacterController>() != null)
		{
			this._Player.GetComponent<CharacterController>().enabled = true;
		}
		if (this._Player.GetComponent<PlayerMotor>() != null)
		{
			this._Player.GetComponent<PlayerMotor>().enabled = true;
		}
		if (this._Player.GetComponent<PlayerInput>() != null)
		{
			this._Player.GetComponent<PlayerInput>().SetMovement(true);
		}
		if (this._Player == WorldGameObjectX.Instance.MainPlayer)
		{
			this._MouseLookHor.invertX = false;
			this._MouseLookHor.OriginalRotation = Quaternion.identity;
			Vector3 vector = base.transform.forward * ((!this._ReverseMove) ? 1f : -1f);
			vector = new Vector3(vector.x, 0f, vector.z);
			this._MouseLookHor.RotationX = Quaternion.LookRotation(vector.normalized, Vector3.up).eulerAngles.y;
			if (this._PlayerCamera != null)
			{
				this._MouseLookVer.OriginalRotation = Quaternion.identity;
				this._PlayerCamera.transform.localPosition = this._PlayerCameraOffset;
			}
			this._PlayerCamera = null;
			this._MouseLookHor = null;
			this._MouseLookVer = null;
			CameraController component = this._Player.GetComponent<CameraController>();
			if (!this._PlayerCameraThirdPerson)
			{
				component.SetFirstPerson(true);
			}
			else if (!this._PlayerCameraFreeLook)
			{
				component.SetThirdPerson(false, 0f, true);
			}
			component.DisableCameraSwitching = false;
			UnityEngine.Object.Destroy(this.Model.GetComponentInChildren<TrolleyPlayer>().gameObject);
		}
		if (this._IgnoredCollider != null)
		{
			Physics.IgnoreCollision(this.MainCollider, this._IgnoredCollider, false);
			this._IgnoredCollider = null;
		}
		this._Player.transform.position += base.transform.up * 1.2f;
		if (this._FreeFall)
		{
			Physics.IgnoreCollision(this.MainCollider, this._Player.GetComponent<Collider>(), true);
			this._IgnoredCollider = this._Player.GetComponent<Collider>();
		}
		this._Player.GetComponent<PlayerNetwork>().InVehicle = false;
		this._Player.GetComponent<PlayerNetwork>().InTrolley = false;
		this._Player = null;
		if (!others)
		{
			base.photonView.RPC("PlayerExitOthers", PhotonTargets.Others, new object[0]);
		}
	}

	[PunRPC]
	protected void PlayerExitOthers()
	{
		this.PlayerExit(true);
	}

	protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(this.Model.transform.localPosition);
			stream.SendNext(-220f * this._CurSpeed * this._MoveDir);
		}
		else
		{
			this._Position = (Vector3)stream.ReceiveNext();
			this._Rotation = (Quaternion)stream.ReceiveNext();
			this._PositionModel = (Vector3)stream.ReceiveNext();
			this._WheelsSpeed = (float)stream.ReceiveNext();
		}
	}

	private void PlayerFixPosition()
	{
		this._Player.transform.position = this.PlayerPosition.position;
		if (!base.photonView.isMine)
		{
			return;
		}
		if (this._Player == WorldGameObjectX.Instance.MainPlayer)
		{
			if (CameraController.Instance.IsThirdPerson)
			{
				this._MouseLookHor.transform.rotation = this.Model.transform.rotation;
				this._MouseLookHor.OriginalRotation = this._MouseLookHor.transform.localRotation;
			}
			else if (this._MouseLookHor.OriginalRotation != Quaternion.identity)
			{
				this._MouseLookHor.OriginalRotation = Quaternion.identity;
			}
			int num = (!(this._CurRail != null)) ? this._LastPlaneIndex : this._CurRail.PlaneIndex;
			if (this._CurRailExt != null && this._CurRailExt.PlaneIndex < num)
			{
				num = this._CurRailExt.PlaneIndex;
			}
			if (this._NextRail != null && this._NextRail.PlaneIndex < num)
			{
				num = this._NextRail.PlaneIndex;
			}
			this._LastPlaneIndex = num;
			Vector3 b;
			switch (num)
			{
			default:
				b = Vector3.up;
				break;
			case 1:
				b = Vector3.down;
				break;
			case 2:
				b = Vector3.right;
				break;
			case 3:
				b = Vector3.left;
				break;
			case 4:
				b = Vector3.forward;
				break;
			case 5:
				b = Vector3.back;
				break;
			}
			this._CameraOffsetPosition = Vector3.Lerp(this._CameraOffsetPosition, b, Time.deltaTime * 5f);
			this._PlayerCamera.transform.position = this.PlayerPosition.position + this._CameraOffsetPosition;
			if ((this._CurRail != null && this._CurRail.PlaneIndex == 1) || (this._CurRailExt != null && this._CurRailExt.PlaneIndex == 1) || (this._NextRail != null && this._NextRail.PlaneIndex == 1))
			{
				this._CameraOffsetRotationX = Mathf.MoveTowardsAngle(this._CameraOffsetRotationX, 180f, Time.deltaTime * 360f);
				this._MouseLookHor.invertX = true;
			}
			else
			{
				this._CameraOffsetRotationX = Mathf.MoveTowardsAngle(this._CameraOffsetRotationX, 0f, Time.deltaTime * 360f);
				this._MouseLookHor.invertX = false;
			}
			this._MouseLookVer.OriginalRotation = Quaternion.Euler(-this._CameraOffsetRotationX, 0f, 0f);
		}
	}

	private void StartFreeFall(Vector3 direction, float speed)
	{
		this._FreeFall = true;
		this._FreeFallTime = Time.time;
		this._FreeFallRail = this._CurRail;
		this.MainCollider.gameObject.layer = LayerMask.NameToLayer("Default");
		base.GetComponent<Rigidbody>().useGravity = true;
		base.GetComponent<Rigidbody>().isKinematic = false;
		base.GetComponent<Rigidbody>().velocity = direction * speed;
		this.SwapColliders(base.GetComponent<Collider>() as BoxCollider, this.RailConnectCollider);
		if (this._CurRail != null)
		{
			this._CurRail = null;
			this._CurRailExt = null;
			this._NextRail = null;
			Trolley._AttachedTrolleys.Remove(this);
			Trolley._DetachedTrolleys.Add(this);
		}
	}

	private void StopFreeFall()
	{
		this._FreeFall = false;
		this._FreeFallRail = null;
		this.MainCollider.gameObject.layer = LayerMask.NameToLayer("Trolley");
		base.GetComponent<Rigidbody>().useGravity = false;
		base.GetComponent<Rigidbody>().isKinematic = true;
		this.SwapColliders(base.GetComponent<Collider>() as BoxCollider, this.RailConnectCollider);
		if (this._IgnoredCollider != null)
		{
			Physics.IgnoreCollision(this.MainCollider, this._IgnoredCollider, false);
			this._IgnoredCollider = null;
		}
	}

	private void SwapColliders(BoxCollider collider1, BoxCollider collider2)
	{
		Vector3 center = collider1.center;
		Vector3 size = collider1.size;
		collider1.center = collider2.center;
		collider1.size = collider2.size;
		collider2.center = center;
		collider2.size = size;
	}

	public const float MaxSpeed = 12f;

	public const float InitialPushSpeed = 3.3f;

	public const float AccelerateSpeed = 1.5f;

	public const float BreakSpeed = 2.5f;

	public const float FrictionSpeed = 0.3f;

	public const float DownhillSpeed = 1f;

	public const float UpSlowDownSpeed = 1.5f;

	public const float ItemsCrashSpeed = 6f;

	public const float FreeFallDisruptionSpeed = 1f;

	public const float FreeFallMultiplierSpeed = 1f;

	public const float FreeFallCeilingSpeed = 1f;

	private static List<Trolley> _AttachedTrolleys = new List<Trolley>();

	private static List<Trolley> _DetachedTrolleys = new List<Trolley>();

	public Transform Model;

	public Collider MainCollider;

	public Transform PlayerPosition;

	public BoxCollider RailConnectCollider;

	public Transform[] Wheels;

	private float _CurSpeed;

	private float _MoveDir = 1f;

	private Rail _CurRail;

	private Rail _CurRailExt;

	private Rail _NextRail;

	private bool _ReverseMove;

	private Transform _PlayerCamera;

	private Vector3 _PlayerCameraOffset = Vector3.zero;

	private bool _PlayerCameraThirdPerson;

	private bool _PlayerCameraFreeLook;

	private MouseLook _MouseLookHor;

	private MouseLook _MouseLookVer;

	private float _RotationSpeedFactor = 1f;

	private Vector3 _CurRotation = Vector3.zero;

	private bool _FreeFall;

	private float _FreeFallTime;

	private Rail _FreeFallRail;

	private Vector3 _CameraOffsetPosition = Vector3.up;

	private float _CameraOffsetRotationX;

	private int _LastPlaneIndex;

	private GameObject _Player;

	private Collider _IgnoredCollider;

	private Vector3 _Position = Vector3.zero;

	private Quaternion _Rotation = Quaternion.identity;

	private Vector3 _PositionModel = Vector3.zero;

	private float _WheelsSpeed;
}
