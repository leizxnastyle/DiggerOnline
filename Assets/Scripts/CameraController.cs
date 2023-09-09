using System;
using System.Collections;
using System.Linq;
using InventorySystem;
using Plugins.Android.PermissionPlease;
using UnityEngine;

public class CameraController : bs
{
	public Camera FirstPersonCamera
	{
		get
		{
			return this._FirstPersonCamera;
		}
	}

	public bool IsThirdPerson
	{
		get
		{
			return this._IsThirdPerson;
		}
	}

	public bool IsFreeLook
	{
		get
		{
			return this._IsFreeLook;
		}
	}

	public bool DisableInput
	{
		get
		{
			return this._DisableInput || !Screen.lockCursor;
		}
		set
		{
			this._DisableInput = value;
			bool disableInput = value || (this._IsThirdPerson && this._IsFreeLook);
			this._MouseLookVer.disableInput = (this._MouseLookHor.disableInput = disableInput);
		}
	}

	public bool DisableCameraSwitching
	{
		get
		{
			return this._DisableCameraSwitching;
		}
		set
		{
			this._DisableCameraSwitching = value;
		}
	}

	public static Camera RaycastCamera
	{
		get
		{
			return (!(CameraController.Instance != null)) ? null : (CameraController.Instance._IsThirdPerson ? CameraController.Instance._ThirdPersonCamera : CameraController.Instance._FirstPersonCamera);
		}
	}

	public static float RaycastDistance
	{
		get
		{
			if (!GameType.IsHungerGamesMode)
			{
				return (!(CameraController.Instance != null)) ? 0f : CameraController.Instance._RaycastDistance.Value;
			}
			return (!(CameraController.Instance != null)) ? 0f : 3f;
		}
	}

	public Gun CurGun
	{
		get
		{
			return this._PlayerNetwork.CurGun;
		}
	}

	private Gun[] _AvaibleWeapons
	{
		get
		{
			if (!GameType.IsHungerGamesMode)
			{
				return new Gun[]
				{
					this._PlayerNetwork.Knife,
					this._PlayerNetwork.MainWeapon[0],
					this._PlayerNetwork.MainWeapon[1],
					this._PlayerNetwork.Grenade,
					this._PlayerNetwork.Health,
					this._PlayerNetwork.Shield
				};
			}
			return new Gun[]
			{
				this._PlayerNetwork.MainWeapon[0],
				this._PlayerNetwork.MainWeapon[1],
				this._PlayerNetwork.MainWeapon[2],
				this._PlayerNetwork.Grenade
			};
		}
	}

	public int SelectedWeapon
	{
		get
		{
			return this._CurGunID;
		}
	}

	private void Awake()
	{
		if (!base.photonView.isMine)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		CameraController.Instance = this;
		this._FirstPersonCamera = base.transform.FindChild("recoil/Main Camera").GetComponent<Camera>();
		this._ThirdPersonCamera = this._FirstPersonCamera.transform.FindChild("Camera Third Person").GetComponent<Camera>();
		this._SkinManager = base.GetComponentInChildren<SkinManager>();
		this._MouseLookHor = base.GetComponent<MouseLook>();
		this._MouseLookVer = this._FirstPersonCamera.GetComponent<MouseLook>();
		this._BodyAnimation = base.GetComponent<Animation>();
		this._HandsAnimation = this.GetHandsAnimation();
		this._CharacterController = base.GetComponent<CharacterController>();
		this._NetworkSyncAnimation = base.GetComponent<NetworkSyncAnimation>();
		this._PlayerNetwork = base.GetComponent<PlayerNetwork>();
		this._CrosshairNode = KGUI.FindNode("hud.battle.crosshair", false);
		this._ThirdPersonCamera.enabled = false;
		this._ThirdPersonCameraRange = (this._ThirdPersonCamera.transform.position - this._FirstPersonCamera.transform.position).magnitude;
		this._ThirdPersonCameraInitialPosition = this._ThirdPersonCamera.transform.localPosition;
		this._ThirdPersonCameraInitialRotation = this._ThirdPersonCamera.transform.localRotation;
		this._HandsAnimation["hit"].speed = 2f;
		this._HandsAnimation["build"].speed = 2f;
		this._SkinManager.SetSkin(-1);
		this._SkinManager.SetHand(ProfileINI.GetActualSkin());
		this._StartTime = Time.time;
		if (this.ctrl != null)
		{
			this.downZone = this.ctrl.GetZone(2);
			this.upZone = this.ctrl.GetZone(1);
			this.walkStick = this.ctrl.GetStick(0);
			this.aimZone = this.ctrl.GetZone(4);
		}
	}

	private void Start()
	{
		this._CameraRecoilOldPos = this.CameraRecoil.localPosition;
		this.SetSavedCamera();
		if (!base.photonView.owner.isLocal)
		{
			return;
		}
		this.Reset();
	}

	public void SetSavedCamera()
	{
		if (GameType.BattleMode())
		{
			this.SetFirstPerson(true);
		}
		else
		{
			base.StartCoroutine(this.SetSavedCameraProcess());
		}
	}

	private IEnumerator SetSavedCameraProcess()
	{
		yield return 0;
		if (ProfileINI.camera_type == 1)
		{
			this.SetFirstPerson(true);
		}
		if (ProfileINI.camera_type == 2)
		{
			this.SetThirdPerson(false, 0f, true);
		}
		if (ProfileINI.camera_type == 3)
		{
			this.SetThirdPerson(true, 0f, true);
		}
		yield break;
	}

	public void SetFirstPerson(bool saveSettings = true)
	{
		if (!this._IsThirdPerson)
		{
			return;
		}
		if (saveSettings)
		{
			ProfileINI.camera_type = 1;
			ProfileINI.Save();
		}
		this._IsThirdPerson = false;
		this._SkinManager.SetSkin(-1);
		this._SkinManager.SetHand(ProfileINI.GetActualSkin());
		this._PlayerNetwork.GunModels.gameObject.SetActive(false);
		this._FirstPersonCamera.enabled = true;
		this._ThirdPersonCamera.enabled = false;
		this._MouseLookHor.disableInput = false;
		this._MouseLookVer.disableInput = false;
		this._ThirdPersonCamera.transform.parent = this._FirstPersonCamera.transform;
		this._MouseLookVer.minimumY = -90f;
		this._MouseLookVer.maximumY = 90f;
		this._RaycastDistance = 8f;
		base.gameObject.SendMessage("SetDirectMovement", false);
		this._SkinManager.SetTransparent(1f);
		if (GameType.IsHungerGamesMode)
		{
			HG_WorkController._player.SetFirstPerson(true);
		}
	}

	public void SetThirdPerson(bool freeLook, float customAngle = 0f, bool saveSettings = true)
	{
		if (this._IsThirdPerson && this._IsFreeLook == freeLook)
		{
			return;
		}
		if (saveSettings)
		{
			if (!freeLook)
			{
				ProfileINI.camera_type = 2;
				ProfileINI.Save();
			}
			else
			{
				ProfileINI.camera_type = 3;
				ProfileINI.Save();
			}
		}
		this._IsThirdPerson = true;
		this._IsFreeLook = freeLook;
		this._FirstPersonCamera.enabled = false;
		this._ThirdPersonCamera.enabled = true;
		this._SkinManager.SetSkin(ProfileINI.GetActualSkin());
		this._SkinManager.SetHand(-1);
		this._PlayerNetwork.GunModels.gameObject.SetActive(true);
		this._ThirdPersonCamera.transform.parent = this._FirstPersonCamera.transform;
		this._ThirdPersonCamera.transform.localPosition = this._ThirdPersonCameraInitialPosition;
		if (customAngle == 0f)
		{
			this._ThirdPersonCamera.transform.localRotation = this._ThirdPersonCameraInitialRotation;
		}
		else
		{
			Vector3 eulerAngles = this._ThirdPersonCameraInitialRotation.eulerAngles;
			this._ThirdPersonCamera.transform.localRotation = Quaternion.Euler(customAngle, eulerAngles.y, eulerAngles.z);
		}
		if (this._IsFreeLook)
		{
			this._ThirdPersonCamera.transform.parent = null;
		}
		this._RaycastDistance = 8f + (this._ThirdPersonCamera.transform.position - this._FirstPersonCamera.transform.position).magnitude;
		this._MouseLookHor.disableInput = this._IsFreeLook;
		this._MouseLookVer.disableInput = this._IsFreeLook;
		this._FreeLookDirectMovement = this._IsFreeLook;
		base.gameObject.SendMessage("SetDirectMovement", this._FreeLookDirectMovement);
		if (!this._IsFreeLook)
		{
			this._MouseLookVer.minimumY = -70f;
			this._MouseLookVer.maximumY = 50f;
		}
		else
		{
			this._MouseLookVer.RotationY = 0f;
			this._FreeLookLastPosition = this._FirstPersonCamera.transform.position;
		}
		this._ThirdPersonCamera.transform.position = this._FirstPersonCamera.transform.position + (this._ThirdPersonCamera.transform.position - this._FirstPersonCamera.transform.position).normalized * 0.5f;
		if (GameType.IsHungerGamesMode)
		{
			HG_WorkController._player.SetFirstPerson(false);
		}
	}

	public void SendVoiceOnChat()
	{
		UnityEngine.Debug.Log("SendVoiceOnChat");
		if (!Chat.IsEnabled())
		{
			if (!this._MicInited)
			{
				this.InitMicrophone();
			}
			else if (Time.time - this._LastEmotion > 3f)
			{
				this._LastEmotion = Time.time;
				Chat.SendEmotion("voiceChat", true);
			}
		}
	}

	private void LateUpdate()
	{
		if (this.aimZone.JustTapped())
		{
			this.LastTouchPosition = UnityEngine.Input.mousePosition;
		}
		if (bs.DebugKey(KeyCode.A))
		{
			this._PlayerNetwork.Grenade.bullets = 2;
		}
		if (bs.DebugKey(KeyCode.S))
		{
			this.Scope = false;
		}
		if (bs.DebugKey(KeyCode.W))
		{
			base.transform.position = UnityEngine.Object.FindObjectsOfType(typeof(PlayerNetwork)).Cast<PlayerNetwork>().First((PlayerNetwork a) => a.GetComponent<CameraController>() == null).pos;
		}
		if (bs.DebugKey(KeyCode.Q))
		{
			this._PlayerNetwork.Grenade.bullets = 3;
		}
		if (bs.DebugKey(KeyCode.F))
		{
			bs._WorldGameObjectX.PunchPlayer(base.photonView.owner, Vector3.zero, 100f, this.CurGun.arrayId, null, false);
		}
		if (!this._DisableInput && !this._DisableCameraSwitching && (!GameType.BattleMode() || bs._Igor.debug || GameType.IsHungerGamesMode))
		{
			if (UnityEngine.Input.GetKeyUp(KeyCode.F1))
			{
				this.SetFirstPerson(true);
			}
			else if (UnityEngine.Input.GetKeyUp(KeyCode.F2))
			{
				this.SetThirdPerson(false, 0f, true);
			}
			else if (UnityEngine.Input.GetKeyUp(KeyCode.F3))
			{
				this.SetThirdPerson(true, 0f, true);
			}
		}
		if (this._IsThirdPerson)
		{
			float axis = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
			if (axis != 0f && WorldGameObjectX.Instance.EntityPreview == null && !MainMenu.Instance.MenuActive && WorldGameObjectX.Instance.Preview == null)
			{
				this._ThirdPersonCameraRange += -axis * 10f;
				this._ThirdPersonCameraRange = Mathf.Clamp(this._ThirdPersonCameraRange, 1.5f, 6f);
			}
			bool flag = !this._DisableInput && !Chat.IsEnabled() && !MainMenu.Instance.MenuActive && (UnityEngine.Input.GetAxis("Vertical") != 0f || UnityEngine.Input.GetAxis("Horizontal") != 0f || this.walkStick.GetTilt() > 0f);
			Vector3 vector = (!bs._WorldGameObjectX.MainPlayerDead) ? this._FirstPersonCamera.transform.position : base.GetComponent<PlayerNetwork>().Standart.position;
			if (!this._DisableInput && !MainMenu.Instance.MenuActive && (this.downZone.JustTapped() || this.upZone.JustTapped()))
			{
				bool flag2 = false;
				if (this.downZone.JustTapped() && !this.IsPlaying("dig") && !this.IsPlaying("build"))
				{
					if (!GameType.IsHungerGamesMode || (HG_WorkController.hgstatus != GameStatus.GS_WAIT && HG_WorkController.hgstatus != GameStatus.GS_PREPEA))
					{
						this.CrossFade("dig");
						this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.dig);
						base.Invoke("HitEvent", 0.2f);
					}
				}
				else if (this.upZone.JustTapped() && !this.IsPlaying("build") && !this.IsPlaying("dig"))
				{
					this.CrossFade("build");
					this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.build);
					base.Invoke("BuildEvent", 0.2f);
					UnityEngine.Debug.Log("BuildEvent");
				}
				else
				{
					flag2 = true;
				}
				if (this._IsFreeLook && !flag2)
				{
					Vector3 vector2 = vector - this._ThirdPersonCamera.transform.position;
					vector2 = new Vector3(vector2.x, 0f, vector2.z);
					this._MouseLookHor.RotationX = Quaternion.LookRotation(vector2.normalized, Vector3.up).eulerAngles.y - this._MouseLookHor.OriginalRotation.eulerAngles.y;
				}
			}
			else if (flag && !this._IsFreeLook && this._CharacterController.enabled)
			{
				bool[] sticSide = this.GetSticSide();
				bool flag3 = sticSide[0];
				bool flag4 = sticSide[1];
				bool flag5 = sticSide[2];
				bool flag6 = sticSide[3];
				if (flag3 && flag5)
				{
					if (!this.IsPlaying("runR"))
					{
						this.CrossFade("runR");
						this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
					}
				}
				else if (flag3 && flag6)
				{
					if (!this.IsPlaying("runL"))
					{
						this.CrossFade("runL");
						this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
					}
				}
				else if (flag3)
				{
					if (!this.IsPlaying("run"))
					{
						this.CrossFade("run");
						this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
					}
				}
				else if (flag4)
				{
					if (!this.IsPlaying("back"))
					{
						this.CrossFade("back");
						this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.back);
					}
				}
				else if (flag5)
				{
					if (!this.IsPlaying("strafL"))
					{
						this.CrossFade("strafL");
						this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafL);
					}
				}
				else if (flag6 && !this.IsPlaying("strafR"))
				{
					this.CrossFade("strafR");
					this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafR);
				}
			}
			else if (flag && this._IsFreeLook && this._CharacterController.enabled)
			{
				Vector3 vector3 = vector - this._ThirdPersonCamera.transform.position;
				vector3 = new Vector3(vector3.x, 0f, vector3.z);
				Quaternion lhs = Quaternion.LookRotation(vector3.normalized, base.transform.up);
				bool[] sticSide2 = this.GetSticSide();
				bool flag7 = sticSide2[0];
				bool flag8 = sticSide2[1];
				bool flag9 = sticSide2[2];
				bool flag10 = sticSide2[3];
				if (flag9)
				{
					lhs *= Quaternion.AngleAxis((float)((!flag7) ? ((!flag8) ? -90 : -135) : -45), base.transform.up);
				}
				if (flag10)
				{
					lhs *= Quaternion.AngleAxis((float)((!flag7) ? ((!flag8) ? 90 : 135) : 45), base.transform.up);
				}
				if (!flag7 && flag9 == flag10 && flag8)
				{
					lhs *= Quaternion.AngleAxis(180f, base.transform.up);
				}
				float target = lhs.eulerAngles.y - this._MouseLookHor.OriginalRotation.eulerAngles.y;
				this._MouseLookHor.RotationX = Mathf.MoveTowardsAngle(this._MouseLookHor.RotationX, target, Time.deltaTime * 420f);
				if (!this.IsPlaying("run"))
				{
					this.CrossFade("run");
					this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
				}
			}
			else if (!this.IsPlaying("dig") && !this.IsPlaying("build") && !this._DisableInput)
			{
				if (this._CharacterController.enabled && !this._CharacterController.isGrounded)
				{
					if (!this.IsPlaying("jump"))
					{
						this.CrossFade("jump");
						this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.jump);
					}
				}
				else if (!this.IsPlaying("idle") && Holder.Active == null)
				{
					this.CrossFade("idle");
					this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.idle);
				}
			}
			if (this._IsFreeLook)
			{
				this._ThirdPersonCamera.transform.position += vector - this._FreeLookLastPosition;
				this._FreeLookLastPosition = vector;
				float num = 0f;
				float num2 = 0f;
				if (!MainMenu.Instance.MenuActive)
				{
					Vector2 uniDragDelta = this.aimZone.GetUniDragDelta(TouchCoordSys.SCREEN_CM, true);
					num += uniDragDelta.x * ProfileINI.mouse_sens * 1f * 7f;
					num2 += -uniDragDelta.y * ProfileINI.mouse_sens * 1f * 7f;
				}
				float x = this._ThirdPersonCamera.transform.eulerAngles.x;
				if (x < 80f && num2 < x - 75f)
				{
					num2 = x - 75f;
				}
				else if (x > 295f && num2 > x - 300f)
				{
					num2 = x - 300f;
				}
				this._ThirdPersonCamera.transform.RotateAround(vector, Vector3.up, num);
				this._ThirdPersonCamera.transform.RotateAround(vector, -this._ThirdPersonCamera.transform.right, num2);
				bool flag11 = !this.downZone.JustTapped() && !this.upZone.JustTapped();
				if (flag11 != this._FreeLookDirectMovement)
				{
					this._FreeLookDirectMovement = flag11;
					base.gameObject.SendMessage("SetDirectMovement", this._FreeLookDirectMovement);
				}
			}
			Vector3 vector4 = this._ThirdPersonCamera.transform.position - vector;
			Vector3 normalized = vector4.normalized;
			float num3 = vector4.magnitude;
			int layerMask = 1 << LayerMask.NameToLayer("Entity") | 1 << LayerMask.NameToLayer("SmallDecor") | 1 << LayerMask.NameToLayer("Terrain");
			RaycastHit raycastHit;
			if (Physics.Raycast(vector, normalized, out raycastHit, this._ThirdPersonCameraRange + 0.5f, layerMask))
			{
				num3 = (vector - (raycastHit.point - normalized * 0.5f)).magnitude;
			}
			else
			{
				num3 = Mathf.MoveTowards(num3, this._ThirdPersonCameraRange, Time.deltaTime * 5f);
			}
			this._ThirdPersonCamera.transform.position = vector + normalized * num3;
			this._RaycastDistance = 8f + num3 * 0.9f;
			float num4 = (num3 >= 1.2f) ? 1f : 0.3f;
			if (this._ThirdPersonCurAlpha != num4)
			{
				this._ThirdPersonCurAlpha = Mathf.MoveTowards(this._ThirdPersonCurAlpha, num4, Time.deltaTime * 3f);
				this._SkinManager.SetTransparent(this._ThirdPersonCurAlpha);
			}
		}
		else if (!this._DisableInput && !Chat.IsEnabled() && !MainMenu.Instance.MenuActive && !GameType.IsObserving())
		{
			this._IsMove = false;
			if (this._CharacterController.enabled)
			{
				this._IsMove = true;
				bool[] sticSide3 = this.GetSticSide();
				bool flag12 = sticSide3[0];
				bool flag13 = sticSide3[1];
				bool flag14 = sticSide3[2];
				bool flag15 = sticSide3[3];
				if (flag12)
				{
					this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.run);
				}
				else if (flag13)
				{
					this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.back);
				}
				else if (flag14)
				{
					this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafL);
				}
				else if (flag15)
				{
					this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.strafR);
				}
				else if (this._CharacterController.enabled && !this._CharacterController.isGrounded)
				{
					this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.jump);
				}
				else
				{
					this._IsMove = false;
				}
			}
			if (!this.lat_touch)
			{
				if (GameType.IsHungerGamesMode && (HG_WorkController.hgstatus == GameStatus.GS_WAIT || HG_WorkController.hgstatus == GameStatus.GS_PREPEA || HG_WorkController.hgstatus == GameStatus.GS_PRE_ARENA))
				{
					return;
				}
				if (GameType.IsArcadeMode)
				{
					return;
				}
				this.UpdateMouseButton();
			}
			if (GameType.BattleMode())
			{
				this.UpdateWeapon();
			}
			if (!this.aimZone.JustTapped())
			{
				this.lat_touch = false;
			}
		}
		else
		{
			if (GameType.IsHungerGamesMode && MainMenu.Instance.CurMenu != Menu.None && this.isBowTensioned && InventaryObjManager.inventary.CanShootFromBowMinusArrow(true, ref this.arrow_id))
			{
				this.ShootBow(this.arrow_id);
				return;
			}
			this.lat_touch = true;
		}
		if (this.Scope && this.CurGun.sniper)
		{
			this._FirstPersonCamera.fieldOfView = 20f;
		}
		if (GameType.BattleMode())
		{
			bool flag16 = !this.CurGun.melee;
			if (this._PlayerNetwork.Crouch && !flag16)
			{
				base.photonView.RPC("SetCrouch", PhotonTargets.All, new object[]
				{
					false
				});
			}
			else if (flag16 && (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) || UnityEngine.Input.GetKeyUp(KeyCode.LeftShift)))
			{
				base.photonView.RPC("SetCrouch", PhotonTargets.All, new object[]
				{
					Input.GetKeyDown(KeyCode.LeftShift)
				});
			}
		}
		if (this._CharacterController.isGrounded)
		{
			this._LastTimeGrounded = Time.time;
		}
	}

	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (this._CharacterController.velocity.y < -15f && (GameType.BattleMode() || GameType.IsHungerGamesMode) && Time.time - this._StartTime > 3f && Time.time - this._LastTimeGrounded > 1f)
		{
			Vector3 position = WorldGameObjectX.Instance.MainPlayer.transform.position;
			BlockType blockType = WorldData.Instance.GetBlockType((int)position.x, (int)position.z, (int)(position.y + 0.01f) - 1);
			if (blockType != BlockType.Water && blockType != BlockType.Gum)
			{
				base.PlayOneShot(bs._Igor.fallSound);
				bs._WorldGameObjectX.PunchPlayer(base.photonView.owner, Vector3.zero, 0.1f, -1, null, false);
			}
		}
	}

	private void CrossFade(string runr)
	{
		this._BodyAnimation.CrossFade(this._PlayerNetwork.GetPrefix(this._BodyAnimation, runr));
	}

	private bool IsPlaying(string runr)
	{
		return this._BodyAnimation.IsPlaying(this._PlayerNetwork.GetPrefix(this._BodyAnimation, runr));
	}

	private bool IsAction()
	{
		return this.HandsGunAnimations.IsPlaying("reload") || this.HandsGunAnimations.IsPlaying("draw");
	}

	public void SniperScopeOff()
	{
		this.SniperScope.GetComponent<Renderer>().enabled = false;
	}

	private void UpdateWeapon()
	{
		float axis = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			this.SwitchWeapon(axis);
		}
		for (int i = 0; i < 6; i++)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1 + i))
			{
				this.SelectWeapon(i, true);
			}
		}
		if (this.Scope && !this.CurGun.sniper)
		{
			this._FirstPersonCamera.GetComponent<Animation>().CrossFade("aim");
		}
		else
		{
			this._FirstPersonCamera.GetComponent<Animation>().CrossFade("idle");
		}
		bool flag = Time.time - this._PlayerNetwork.HitTime < 1f;
		this.Damage.GetComponent<Renderer>().enabled = flag;
		if (flag)
		{
			this.Damage.GetComponent<Renderer>().material.color = Color.white * Mathf.Max(0f, this._PlayerNetwork.HitTime + 1f - Time.time);
			this.Damage.parent.localEulerAngles = new Vector3(0f, 0f, this.AngleSigned(base.ZeroY(-base.transform.forward), base.ZeroY(this._PlayerNetwork.HitDir).normalized, Vector3.down));
		}
		if (!this.CurGun.melee && !this.IsAction() && !this.HandsGunAnimations.IsPlaying("shoot") && (UnityEngine.Input.GetAxis("Vertical") != 0f || UnityEngine.Input.GetAxis("Horizontal") != 0f))
		{
			this.HandsGunAnimations.Play("walk");
		}
		if (this.upZone.JustTapped() && !this.CurGun.melee && !this.CurGun.grenade && !this.CurGun.sniper)
		{
			this.Scope = !this.Scope;
		}
		if (this.upZone.JustTapped() && this.CurGun.sniper && !this.CurGun.hands.GetComponent<Animation>().isPlaying)
		{
			AnimationState animationState = this.CurGun.hands.GetComponent<Animation>()["snipe"];
			animationState.time = ((!this.Scope) ? 0f : animationState.length);
			animationState.speed = ((!this.Scope) ? 1.5f : -1.5f);
			this.CurGun.hands.GetComponent<Renderer>().enabled = true;
			this.CurGun.hands.GetComponent<Animation>().Play();
			if (this.Scope)
			{
				this.Scope = !this.Scope;
			}
			else
			{
				base.StartCoroutine(bs.AddMethod(() => !this.CurGun.hands.GetComponent<Animation>().isPlaying, delegate()
				{
					if (this.CurGun.sniper)
					{
						this.Scope = !this.Scope;
					}
					this.CurGun.hands.GetComponent<Renderer>().enabled = false;
				}));
			}
		}
		bool flag2 = this.Scope && this.CurGun.sniper;
		if (this.SniperScope.GetComponent<Renderer>().enabled != flag2)
		{
			this.SniperScope.GetComponent<Renderer>().enabled = flag2;
		}
		if (this._CrosshairNode.gameObject.activeInHierarchy != !this.CurGun.sniper)
		{
			this._CrosshairNode.gameObject.SetActive(!this.CurGun.sniper);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.R) && this.CurGun.bullets != this.CurGun.bulletsMag && this.CurGun.bulletsMax > 0 && !this.HandsGunAnimations.IsPlaying("reload"))
		{
			this.Reload();
		}
		if (this.upZone.JustTapped())
		{
			this._MouseDownTime = Time.time;
		}
		if (this.CurGun.grenade)
		{
			if (this.upZone.JustTapped() && this.CurGun.bullets > 0 && !this.CurGun.isCooldown)
			{
				this.CurGun.hands.transform.localPosition = this.CurGun.handsPos + (Vector3.right + Vector3.back).normalized * Mathf.Clamp01(Time.time - this._MouseDownTime) * 0.5f;
			}
			else
			{
				this.CurGun.hands.transform.localPosition = this.CurGun.handsPos;
			}
		}
		if (this.CurGun.hands != null && (this.CurGun.hands.GetComponent<Animation>() == null || !this.CurGun.hands.GetComponent<Animation>().isPlaying))
		{
			this.CurGun.hands.transform.localPosition = this.CurGun.handsPos;
		}
		if (!this._IsMove && !GameType.IsHungerGamesMode)
		{
			this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.idle);
		}
		this.CameraRecoil.localRotation = Quaternion.LookRotation(Vector3.forward + this._Recoil);
		this.CameraRecoil.localPosition = Vector3.Lerp(this.CameraRecoil.localPosition, this._CameraRecoilOldPos + ((!this._PlayerNetwork.Crouch) ? Vector3.zero : (Vector3.down * 0.3f)), Time.deltaTime * 10f);
		this.SetRecoil(Vector3.Lerp(this._Recoil, Vector3.zero, Time.deltaTime * this.CurGun.RecoilRecover));
	}

	private void ShootBow(int arrow_id)
	{
		if (HG_WorkController.hgstatus == GameStatus.GS_PRE_ARENA)
		{
			base.StartCoroutine(this._PlayerNetwork.HGPlayerNetwork.bow_controller.ReloadBow());
			return;
		}
		this.Play("shoot");
		this._PlayerNetwork.LastTimeShoot = Time.time;
		RaycastHit raycastHit;
		Vector3 vector;
		if (Physics.Raycast(this._FirstPersonCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out raycastHit, 10000f))
		{
			vector = (raycastHit.point - this._PlayerNetwork.HGPlayerNetwork.arrow_point[0].transform.position).normalized;
		}
		else
		{
			vector = this._FirstPersonCamera.transform.forward;
		}
		bool flag = this._PlayerNetwork.QDamageTime > Time.time;
		this._PlayerNetwork.Shoot(this._PlayerNetwork.HGPlayerNetwork.arrow_point[0].transform.position, vector, Time.time - this._MouseDownTime, flag, arrow_id);
		this._PlayerNetwork.HGPlayerNetwork.bow_controller.ShootBow();
		if (this._PlayerNetwork.CurGun.grenade)
		{
			this.Play("draw");
		}
		base.photonView.RPC("Shoot", PhotonTargets.Others, new object[]
		{
			this._PlayerNetwork.HGPlayerNetwork.arrow_point[0].transform.position,
			vector,
			Time.time - this._MouseDownTime,
			flag,
			arrow_id
		});
		this.isBowTensioned = false;
	}

	private void StopRigWidget()
	{
		w_RigWorker.RigWorker.StopPress();
		KGUI.EnableNodes("hud.rig_widget", false, false);
	}

	private void UpdateMouseButton()
	{
		if (this.CurGun.bullets <= 0 && this.CurGun.grenade)
		{
			this.SwitchWeapon(1f);
		}
		if (this.aimZone.GetTouchDuration(0) > 1f && !this.isRotateCamera && !VoicechatButtonWorker.isPressedChat)
		{
			try
			{
				if (w_RigWorker.RigWorker.IsActive)
				{
					this.StopRigWidget();
				}
			}
			catch
			{
				UnityEngine.Debug.Log("NO w_RigWorker");
			}
			this.isTouchHit = true;
			this.LastTouchPosition = new Vector2(this.aimZone.GetUniPos(TouchCoordSys.SCREEN_PX).x, (float)Screen.height - this.aimZone.GetUniPos(TouchCoordSys.SCREEN_PX).y);
			if (this.CurGun.melee)
			{
				if (!this._HandsAnimation.IsPlaying("hit") && !this._HandsAnimation.IsPlaying("build"))
				{
					UnityEngine.Debug.Log("_HandsAnimation");
					this._HandsAnimation.Play("hit");
					this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.dig);
					base.Invoke("HitEvent", 0.2f);
				}
			}
			else if (!this.IsAction() && GameType.BattleMode())
			{
				this.UpdateShooting();
			}
		}
		else if (((!ProfileINI.oneTapSet && this.aimZone.JustDoubleTapped()) || (ProfileINI.oneTapSet && this.aimZone.JustTapped())) && WorldGameObjectX.Instance.EntityPreview == null && !this._HandsAnimation.IsPlaying("hit") && !this._HandsAnimation.IsPlaying("build") && this.CurGun.melee)
		{
			if (WorldGameObjectX.Instance.EntityUnderCursor == null)
			{
				this._HandsAnimation.Play("build");
				this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.build);
				base.Invoke("BuildEvent", 0.2f);
			}
		}
		else if (!this._HandsAnimation.IsPlaying("idle") && Holder.Active == null)
		{
			if (this.CurGun.melee)
			{
				this._HandsAnimation.PlayQueued("idle");
				this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.idle);
			}
		}
		else if (!this._IsMove)
		{
			this._NetworkSyncAnimation.SyncAnimation(NetworkSyncAnimation.AnimState.idle);
		}
	}

	private void UpdateShooting()
	{
		if (((!this.CurGun.pistol && !this.CurGun.grenade && !this.CurGun.custom) || (this.downZone.JustTapped() && !this.CurGun.grenade) || (this.downZone.JustTapped() && this.CurGun.custom) || (this.downZone.JustTapped() && this.CurGun.grenade)) && !this.CurGun.isCooldown)
		{
			SecuredValue<bool> v = Time.time - this._PlayerNetwork.LastTimeShoot > this.CurGun.shootInterval;
			SecuredValue<bool> v2 = this.CurGun.bullets > 0;
			if (v && v2)
			{
				if (!this.CurGun.custom)
				{
					if (Time.time - this._PlayerNetwork.LastTimeShoot <= this.CurGun.shootInterval)
					{
						return;
					}
					this.Play("shoot");
					this.CurGun.bullets--;
					this.SetRecoil(this._Recoil + (UnityEngine.Random.insideUnitSphere * this.CurGun.RecoilRand + Vector3.up * this.CurGun.RecoilUp * ((!this._PlayerNetwork.Crouch) ? 1f : this.CurGun.CrouchRecoilFactor)));
					if (this.CurGun.bullets <= 0 && this.CurGun.bulletsLeft > 0)
					{
						this.Reload();
					}
					this._PlayerNetwork.LastTimeShoot = Time.time;
					Vector3 forward = this._FirstPersonCamera.transform.forward;
					bool flag = this._PlayerNetwork.QDamageTime > Time.time;
					this._PlayerNetwork.Shoot(this._PlayerNetwork.CurGun.hands.transform.position, forward, Time.time - this._MouseDownTime, flag, this._PlayerNetwork.CurGunId);
					if (this._PlayerNetwork.CurGun.grenade)
					{
						this.Play("draw");
					}
					base.photonView.RPC("Shoot", PhotonTargets.Others, new object[]
					{
						this.ShootPoint.position,
						forward,
						Time.time - this._MouseDownTime,
						flag,
						this._PlayerNetwork.CurGunId
					});
				}
				else
				{
					if (!this.CurGun.customUseCheck())
					{
						return;
					}
					this.CurGun.bullets--;
					this.CurGun.customUse();
				}
				if (this.CurGun.storePurchaseCount != StorePurchase.NONE)
				{
					MainMenu.Instance.PurchaseUse(this.CurGun.storePurchaseCount, true);
					MainMenu.Instance.RefreshBattleWeapon(0);
					base.StartCoroutine(MainMenu.Instance.BattleWeaponCooldownProcess());
				}
			}
		}
	}

	private void SwitchWeapon(float d)
	{
		if (!GameType.IsHungerGamesMode)
		{
			for (int i = 0; i < 10; i++)
			{
				this._CurGunID = this.Mod(this._CurGunID + ((d <= 0f) ? 1 : -1), this._AvaibleWeapons.Length);
				if (this.SelectWeapon(this._CurGunID, false))
				{
					break;
				}
			}
		}
		else
		{
			if (d < 0f)
			{
				this._CurGunID++;
			}
			else if (d > 0f)
			{
				this._CurGunID--;
			}
			if (this._CurGunID < 0)
			{
				this._CurGunID = 5;
			}
			else if (this._CurGunID > 5)
			{
				this._CurGunID = 0;
			}
			if (this.SelectWeapon(this._CurGunID, false))
			{
			}
		}
	}

	private bool SelectWeapon(int i, bool allowNoBullets = false)
	{
		this._CurGunID = i;
		if (!GameType.IsHungerGamesMode)
		{
			Gun gun = this._AvaibleWeapons[this._CurGunID];
			if (gun != null)
			{
				this._PlayerNetwork.SelectWeapon(gun.arrayId);
				if (this.CurGun.hands != null && this.CurGun.hands.GetComponent<Animation>() != null)
				{
					this.CurGun.hands.transform.localPosition = this.CurGun.handsPos;
				}
				this.Scope = false;
				if (!this.CurGun.melee)
				{
					this.Play("draw");
				}
				MainMenu.Instance.RefreshBattleWeapon(0);
				return true;
			}
		}
		else if (pnl_Inventory.all_weapon_slot.Count > 0)
		{
			IS_mdl_Item itemFromBagId = InventaryObjManager.GetItemFromBagId(pnl_Inventory.all_weapon_slot[this._CurGunID].item_guid);
			if (itemFromBagId != null)
			{
				this._PlayerNetwork.SelectWeapon(itemFromBagId.Id);
			}
			else
			{
				this._PlayerNetwork.SelectWeapon(-1);
			}
			MainMenu.Instance.RefreshBattleWeapon(this._CurGunID);
			return true;
		}
		return false;
	}

	private void OnPermisionDialog(bool granted)
	{
		if (granted)
		{
			if (VoiceChatRecorder.Instance.AvailableDevices.Length > 0)
			{
				VoiceChatRecorder.Instance.Device = VoiceChatRecorder.Instance.AvailableDevices[0];
			}
			else
			{
				UnityEngine.Debug.Log("No microphone found");
			}
			VoiceChatRecorder.Instance.StartRecording();
			VoiceChatRecorder.Instance.NewSample += this.Instance_NewSample;
		}
		else
		{
			this._MicInited = false;
			UnityEngine.Debug.Log("_MicNotInited");
		}
	}

	private IEnumerator InitMicrophonePC()
	{
		this._MicInited = true;
		VoiceChatRecorder.Instance.NetworkId = base.photonView.owner.ID;
		MainMenu.Instance.ShowHint("HINT_ALLOW_MICROPHONE", true);
		while (MainMenu.Instance.CurMenu == Menu.Hint)
		{
			yield return 0;
		}
		MainMenu.Instance.SwitchMenu(Menu.External, null, null);
		yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
		MainMenu.Instance.SwitchMenu(Menu.None, null, null);
		if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
		{
			this._MicInited = false;
			yield break;
		}
		if (VoiceChatRecorder.Instance.AvailableDevices.Length > 0)
		{
			VoiceChatRecorder.Instance.Device = VoiceChatRecorder.Instance.AvailableDevices[0];
		}
		else
		{
			UnityEngine.Debug.Log("No microphone found");
		}
		VoiceChatRecorder.Instance.StartRecording();
		VoiceChatRecorder.Instance.NewSample += this.Instance_NewSample;
		UnityEngine.Debug.Log("Microphone inited");
		yield break;
	}

	private void InitMicrophone()
	{
		this._MicInited = true;
		VoiceChatRecorder.Instance.NetworkId = base.photonView.owner.ID;
		PermissionPlease.GrantPermission(PermissionPlease.AndroidPermission.RECORD_AUDIO, new Action<bool>(this.OnPermisionDialog), false);
	}

	private void Instance_NewSample(VoiceChatPacket obj)
	{
		base.photonView.RPC("SendAudio", PhotonTargets.Others, new object[]
		{
			obj.Data,
			obj.Length,
			PhotonNetwork.player.ID
		});
	}

	private void Play(string clip)
	{
		this.HandsGunAnimations.Stop();
		this.HandsGunAnimations.Play(clip);
		this.HandsGunAnimations.Sample();
	}

	private void Reload()
	{
		if (!this.CurGun.sniper)
		{
			this.Scope = false;
		}
		this.HandsGunAnimations["reload"].speed = this.CurGun.reloadSpeed;
		this.Play("reload");
		base.PlayOneShot((!(this.CurGun.reloadSound == null)) ? this.CurGun.reloadSound : bs._Igor.reload);
		base.StartCoroutine(bs.AddMethod(() => !this.HandsGunAnimations.IsPlaying("reload"), delegate()
		{
			this.CurGun.bulletsLeft -= this.CurGun.bulletsMag - this.CurGun.bullets;
			this.CurGun.bullets = this.CurGun.bulletsMag;
			if (this.CurGun.bulletsLeft < 0)
			{
				this.CurGun.bullets += this.CurGun.bulletsLeft;
				this.CurGun.bulletsLeft = 0;
			}
		}));
	}

	private Animation GetHandsAnimation()
	{
		return this._FirstPersonCamera.transform.Find("hands").GetComponentInChildren<Animation>();
	}

	private void HitEvent()
	{
		if (GameType.IsArcadeMode)
		{
			return;
		}
		WorldGameObjectX.Instance.OnPlayerHitTerrain();
	}

	private void BuildEvent()
	{
		if (!GameType.BlocksBuildDisabled())
		{
			WorldGameObjectX.Instance.OnBuildTerrain();
		}
	}

	public void JoltScreen()
	{
		Camera camera = (!this.IsThirdPerson) ? this._FirstPersonCamera : this._ThirdPersonCamera;
		float z = (float)((UnityEngine.Random.Range(0, 2) != 0) ? -30 : 30);
		iTween.PunchRotation(camera.gameObject, new Vector3(0f, 0f, z), 1f);
	}

	private int Mod(int a, int n)
	{
		return (a % n + n) % n;
	}

	private float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
	{
		return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
	}

	private void PrintAnims()
	{
		foreach (object obj in base.GetComponent<Animation>())
		{
			AnimationState animationState = (AnimationState)obj;
			bs.Log(new object[]
			{
				animationState.name + " " + animationState.layer,
				animationState.enabled,
				animationState.time
			});
		}
	}

	public void Reset()
	{
		this.SelectWeapon(this._CurGunID, true);
		this.Scope = false;
	}

	private void SetRecoil(Vector3 recoil)
	{
		if (this._Recoil == recoil)
		{
			return;
		}
		this._Recoil = recoil;
		float num = 6f + this._Recoil.magnitude * 100f;
		for (int i = 1; i <= 4; i++)
		{
			float x = ((i != 2) ? ((i != 4) ? 0f : -1f) : 1f) * num;
			float y = ((i != 1) ? ((i != 3) ? 0f : -1f) : 1f) * num;
			KGUI.FindNode("hud.battle.crosshair." + i, false).transform.localPosition = new Vector3(x, y, 0f);
		}
	}

	public bool[] GetSticSide()
	{
		bool[] array = new bool[4];
		TouchStick.StickDir digitalDir = this.walkStick.GetDigitalDir(true);
		if (digitalDir == TouchStick.StickDir.U)
		{
			array[0] = true;
		}
		else if (digitalDir == TouchStick.StickDir.UL)
		{
			array[0] = true;
			array[2] = true;
		}
		else if (digitalDir == TouchStick.StickDir.UR)
		{
			array[0] = true;
			array[3] = true;
		}
		else if (digitalDir == TouchStick.StickDir.L)
		{
			array[2] = true;
		}
		else if (digitalDir == TouchStick.StickDir.R)
		{
			array[3] = true;
		}
		else if (digitalDir == TouchStick.StickDir.DL)
		{
			array[1] = true;
			array[2] = true;
		}
		else if (digitalDir == TouchStick.StickDir.DR)
		{
			array[3] = true;
			array[1] = true;
		}
		else if (digitalDir == TouchStick.StickDir.D)
		{
			array[1] = true;
		}
		return array;
	}

	public const float RaycastDefaultDistance = 8f;

	public const float RaycastHGDistance = 3f;

	public const float ThirdPersonMouseLookMinY = -70f;

	public const float ThirdPersonMouseLookMaxY = 50f;

	public const float ThirdPersonCameraRangeMin = 1.5f;

	public const float ThirdPersonCameraRangeMax = 6f;

	public const int STICK_WALK = 0;

	public const int ZONE_DOWN = 2;

	public const int ZONE_UP = 1;

	public const int ZONE_AIM = 3;

	public const int ZONE_SCREEN = 4;

	public static CameraController Instance;

	private Camera _FirstPersonCamera;

	private Camera _ThirdPersonCamera;

	private SkinManager _SkinManager;

	private Animation _BodyAnimation;

	private Animation _HandsAnimation;

	public Animation HandsGunAnimations;

	private NetworkSyncAnimation _NetworkSyncAnimation;

	private MouseLook _MouseLookHor;

	private MouseLook _MouseLookVer;

	private float _ThirdPersonCameraRange;

	private bool _IsThirdPerson;

	private bool _IsFreeLook = true;

	private SecuredValue<float> _RaycastDistance = new SecuredValue<float>(8f);

	private Vector3 _ThirdPersonCameraInitialPosition = Vector3.zero;

	private Quaternion _ThirdPersonCameraInitialRotation = Quaternion.identity;

	private Vector3 _FreeLookLastPosition = Vector3.zero;

	private bool _FreeLookDirectMovement;

	private float _ThirdPersonCurAlpha = 1f;

	private CharacterController _CharacterController;

	private Transform _CrosshairNode;

	public TouchController ctrl;

	private TouchStick walkStick;

	public TouchZone downZone;

	public TouchZone upZone;

	private TouchZone pauseZone;

	public TouchZone aimZone;

	public Vector2 LastTouchPosition = Vector2.zero;

	public bool isRotateCamera;

	public bool isTouchHit;

	private bool _DisableInput;

	private bool _DisableCameraSwitching;

	public Transform CameraRecoil;

	public Transform SniperScope;

	public Transform ShootPoint;

	public Transform Damage;

	private Vector3 _Recoil;

	public bool Scope;

	private PlayerNetwork _PlayerNetwork;

	private Vector3 _CameraRecoilOldPos;

	private float _MouseDownTime;

	private float _LastEmotion;

	private bool _MicInited;

	private bool _IsMove;

	private float _StartTime;

	private Vector3 _OldVel;

	private int _CurGunID;

	private float _LastTimeGrounded;

	private bool lat_touch;

	private bool isBowTensioned;

	private int arrow_id = -1;
}
