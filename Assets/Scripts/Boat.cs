using System;
using UnityEngine;

public class Boat : EntityBase
{
	public GameObject Player
	{
		get
		{
			return this._Player;
		}
	}

	public Vector3 AwakePosition
	{
		get
		{
			return this._AwakePosition;
		}
	}

	public static void KickPlayer(GameObject player)
	{
		if (player == null)
		{
			return;
		}
		Boat[] array = UnityEngine.Object.FindObjectsOfType(typeof(Boat)) as Boat[];
		foreach (Boat boat in array)
		{
			if (boat._Player == player)
			{
				boat.PlayerExit(false);
				break;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this._Model = base.transform.FindChild("model").gameObject;
		this._PlayerPosition = base.transform.FindChild("player_position");
		this._PlayerExit = base.transform.FindChild("player_exit");
		this._AwakePosition = base.transform.position;
		this._Position = base.transform.position;
		base.GetComponent<Rigidbody>().isKinematic = true;
	}

	protected override void Creation(object[] data)
	{
		bool flag = WorldData.Instance.GetBlockType(base.BlockX, base.BlockY, (int)(base.transform.position.y - 0.01f)) == BlockType.Water;
		base.GetComponent<Rigidbody>().isKinematic = !flag;
	}

	protected override void PreviewCreation(object[] data)
	{
		UnityEngine.Object.Destroy(base.transform.FindChild("colliders").gameObject);
	}

	protected override void Destruction()
	{
		if (this._Player != null && this._Player == WorldGameObjectX.Instance.MainPlayer)
		{
			this.PlayerExit(false);
		}
	}

	private void FixedUpdate()
	{
		if (base.IsPreview || !WorldGameObjectX.Instance.IsWorldGenerated)
		{
			return;
		}
		if (this._InWater)
		{
			bool flag = WorldData.Instance.GetBlockType(base.BlockX, base.BlockY, (int)base.transform.position.y) == BlockType.Water;
			if (!this._UnderWater && flag)
			{
				this._UnderWater = true;
			}
			else if (this._UnderWater && !flag)
			{
				this._UnderWater = false;
				if (!base.GetComponent<Rigidbody>().isKinematic)
				{
					base.transform.position = new Vector3(base.transform.position.x, (float)((int)base.transform.position.y), base.transform.position.z);
					base.GetComponent<Rigidbody>().position = base.transform.position;
					base.GetComponent<Rigidbody>().velocity = new Vector3(base.GetComponent<Rigidbody>().velocity.x, 0f, base.GetComponent<Rigidbody>().velocity.z);
				}
			}
		}
		bool flag2 = WorldData.Instance.GetBlockType(base.BlockX, base.BlockY, (int)(base.transform.position.y - 0.01f)) == BlockType.Water;
		if (flag2 && !this._InWater)
		{
			this._InWater = true;
			this._UnderWater = false;
			if (!base.GetComponent<Rigidbody>().isKinematic)
			{
				base.GetComponent<Rigidbody>().useGravity = false;
			}
		}
		else if (!flag2 && this._InWater)
		{
			this._InWater = false;
			this._UnderWater = false;
			if (!base.GetComponent<Rigidbody>().isKinematic)
			{
				base.GetComponent<Rigidbody>().useGravity = true;
			}
		}
		if (!base.GetComponent<Rigidbody>().isKinematic && this._InWater)
		{
			bool flag3 = false;
			if (this._Player != null && this._Player == WorldGameObjectX.Instance.MainPlayer)
			{
				if (UnityEngine.Input.GetKey(KeyCode.W))
				{
					this._CurSpeed += 1f * Time.fixedDeltaTime;
					flag3 = true;
				}
				if (UnityEngine.Input.GetKey(KeyCode.S))
				{
					if (this._CurSpeed > 0f)
					{
						this._CurSpeed = Mathf.Lerp(this._CurSpeed, 0f, Time.fixedDeltaTime * 2f);
						if (this._CurSpeed < 0.1f)
						{
							this._CurSpeed = 0f;
						}
					}
					else
					{
						this._CurSpeed -= 0.5f * Time.fixedDeltaTime;
					}
					flag3 = true;
				}
				this._CurSpeed = Mathf.Clamp(this._CurSpeed, -1f, 6f);
			}
			if (!flag3)
			{
				this._CurSpeed = Mathf.MoveTowards(this._CurSpeed, 0f, 0.7f * Time.fixedDeltaTime);
				if (this._CurSpeed < 0.1f && this._CurSpeed > -0.1f)
				{
					this._CurSpeed = 0f;
				}
			}
			if (this._CurSpeed != 0f)
			{
				base.GetComponent<Rigidbody>().velocity = base.transform.forward * this._CurSpeed;
			}
			else if (base.GetComponent<Rigidbody>().velocity != Vector3.zero)
			{
				base.GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
			if (base.GetComponent<Rigidbody>().angularVelocity != Vector3.zero)
			{
				base.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			}
			if (this._UnderWater)
			{
				base.GetComponent<Rigidbody>().velocity = new Vector3(base.GetComponent<Rigidbody>().velocity.x, 1.5f, base.GetComponent<Rigidbody>().velocity.z);
			}
		}
		if (!base.GetComponent<Rigidbody>().isKinematic && this._Player != null && this._Player == WorldGameObjectX.Instance.MainPlayer)
		{
			float num = 0f;
			if (UnityEngine.Input.GetKey(KeyCode.A))
			{
				num += -90f * Time.fixedDeltaTime;
			}
			if (UnityEngine.Input.GetKey(KeyCode.D))
			{
				num += 90f * Time.fixedDeltaTime;
			}
			if (num != 0f)
			{
				base.transform.Rotate(0f, num, 0f, Space.Self);
				if (this._PlayerCameraController.IsThirdPerson)
				{
					this._PlayerMouseLookHor.RotationX += num;
				}
			}
		}
		base.FixWorldBounds();
		if (this._Player != null)
		{
			this._Player.transform.position = this._PlayerPosition.position;
		}
	}

	private void OnCollisionStay()
	{
		this._CurSpeed = base.GetComponent<Rigidbody>().velocity.magnitude;
	}

	protected override void Updating()
	{
		if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, this._Position, Time.deltaTime * 5f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this._Rotation, Time.deltaTime * 5f);
		}
		if (this._InWater && !this._UnderWater)
		{
			float num = this._Model.transform.localEulerAngles.z;
			bool flag = this._Player != null && this._Player == WorldGameObjectX.Instance.MainPlayer;
			if (flag && UnityEngine.Input.GetKey(KeyCode.A))
			{
				num = Mathf.MoveTowardsAngle(num, 15f, Time.deltaTime * 50f);
			}
			else if (flag && UnityEngine.Input.GetKey(KeyCode.D))
			{
				num = Mathf.MoveTowardsAngle(num, -15f, Time.deltaTime * 50f);
			}
			else
			{
				num = Mathf.MoveTowardsAngle(num, Mathf.PingPong(Time.time * 6f, 20f) - 10f, Time.deltaTime * 30f);
			}
			this._Model.transform.localRotation = Quaternion.Euler(0f, 0f, num);
		}
		else
		{
			this._Model.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public override void OnButtonF(string playerName)
	{
		GameObject mainPlayer = WorldGameObjectX.Instance.MainPlayer;
		if (!(this._Player != null))
		{
			Boat.KickPlayer(mainPlayer);
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
		this._PlayerCameraController = this._Player.GetComponent<CameraController>();
		this._PlayerMouseLookHor = this._Player.GetComponent<MouseLook>();
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
		this._Player.transform.position = this._PlayerPosition.position;
		GameObject gameObject = new GameObject("collider");
		gameObject.transform.parent = base.transform;
		CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.height = this._Player.GetComponent<CharacterController>().height;
		capsuleCollider.radius = this._Player.GetComponent<CharacterController>().radius;
		capsuleCollider.center = this._Player.GetComponent<CharacterController>().center;
		gameObject.transform.position = this._Player.transform.position;
		gameObject.transform.localScale = this._Player.transform.localScale;
		if (!others)
		{
			base.GetComponent<Rigidbody>().isKinematic = false;
			base.GetComponent<Rigidbody>().useGravity = (!this._InWater && !this._UnderWater);
			base.photonView.isForcedSending = 1;
			base.photonView.RPC("PlayerEnterOthers", PhotonTargets.Others, new object[]
			{
				PhotonNetwork.player
			});
			base.photonView.RPC("SetNewOwner", PhotonTargets.All, new object[]
			{
				PhotonNetwork.player.ID
			});
		}
	}

	[PunRPC]
	protected void PlayerEnterOthers(PhotonPlayer owner)
	{
		base.GetComponent<Rigidbody>().isKinematic = true;
		base.GetComponent<Rigidbody>().useGravity = false;
		base.photonView.isForcedSending = -1;
		this.PlayerEnter(WorldGameObjectX.Instance.FindPlayer(owner).Avatar, true);
		UnityEngine.Debug.Log("PlayerEnterOthers " + owner.name);
	}

	[PunRPC]
	protected void SetNewOwner(int owner)
	{
		base.photonView.ownerId = owner;
	}

	public void PlayerExitHG(bool others = false)
	{
		this.PlayerExit(others);
	}

	private void PlayerExit(bool others = false)
	{
		this._Player.transform.position = this._PlayerExit.transform.position;
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
		Transform transform = base.transform.FindChild("collider");
		UnityEngine.Object.Destroy(transform.gameObject);
		this._Player.GetComponent<PlayerNetwork>().InVehicle = false;
		this._PlayerCameraController = null;
		this._PlayerMouseLookHor = null;
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
			this._Position = base.transform.position;
			this._Rotation = base.transform.rotation;
		}
		else
		{
			this._Position = (Vector3)stream.ReceiveNext();
			this._Rotation = (Quaternion)stream.ReceiveNext();
		}
	}

	public const float MaxSpeed = 6f;

	public const float MaxBackSpeed = 1f;

	public const float AccelerateSpeed = 1f;

	public const float BackSpeed = 0.5f;

	public const float RotationAngleSpeed = 90f;

	public const float FrictionSpeed = 0.7f;

	public const float EmersionSpeed = 1.5f;

	private bool _InWater;

	private bool _UnderWater;

	private GameObject _Model;

	private Transform _PlayerPosition;

	private Transform _PlayerExit;

	private float _CurSpeed;

	private GameObject _Player;

	private CameraController _PlayerCameraController;

	private MouseLook _PlayerMouseLookHor;

	private Vector3 _AwakePosition = Vector3.zero;

	private Vector3 _Position = Vector3.zero;

	private Quaternion _Rotation = Quaternion.identity;
}
