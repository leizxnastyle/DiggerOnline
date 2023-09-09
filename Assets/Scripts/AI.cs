using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class AI : Photon.MonoBehaviour
{
	private bool IsStunned
	{
		get
		{
			return Time.time < this._StunTime;
		}
	}

	private void Awake()
	{
		this._CharacterController = base.GetComponent<CharacterController>();
		this._NetworkAnim = base.GetComponent<NetworkSyncAnimation>();
		base.transform.Translate(0f, 0.1f, 0f, Space.World);
	}

	private void Start()
	{
		this._AggressiveHomePosition = base.transform.position;
		this._PatrollingCenter = base.transform.position;
	}

	public void Punch(Vector3 force, float duration)
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		this.ClearPath();
		this._StunTime = Time.time + duration;
		this._IdleTime = 0f;
		this._PunchForce = force;
		this._PendingPath = null;
		int num = UnityEngine.Random.Range(3, 6);
		for (int i = 0; i < num; i++)
		{
			if (this._PendingPath == null)
			{
				List<Vector3> list = Pathfinder.FindRandom(base.transform.position, this._PatrollingCenter, this.PatrollingDistance);
				if (list == null)
				{
					break;
				}
				this._PendingPath = new List<Vector3>(list);
			}
			else
			{
				List<Vector3> list2 = Pathfinder.FindRandom(this._PendingPath[this._PendingPath.Count - 1], this._PatrollingCenter, this.PatrollingDistance);
				if (list2 == null)
				{
					break;
				}
				this._PendingPath.AddRange(list2);
			}
		}
	}

	private void Update()
	{
		if (!base.photonView.isMine || !WorldGameObjectX.Instance.IsWorldGenerated)
		{
			return;
		}
		if (base.transform.position.y < 0f)
		{
			base.transform.position = WorldGameObjectX.Instance.GenerateSpawnPoint();
		}
		if (this._Path != null && this._CharacterController.isGrounded)
		{
			this.ValidatePath();
		}
		if (Time.time < this._IdleTime)
		{
			if (this._NetworkAnim != null && this._NetworkAnim.Anim && !this.IsIdlePlaying() && !this._NetworkAnim.Anim.IsPlaying("fidget"))
			{
				if (this._NetworkAnim.Anim["fidget"] != null && Time.time >= this._FidgetNextTime)
				{
					this._FidgetNextTime = Time.time + this._NetworkAnim.Anim["fidget"].length + UnityEngine.Random.Range(2f, 4f);
					this._NetworkAnim.PlayAnimation(NetworkSyncAnimation.AnimState.fidget, true);
				}
				else
				{
					this.PlayIdle();
				}
			}
		}
		else if (this._Path == null && this._PendingPath != null && this._CharacterController.isGrounded && !this.IsStunned)
		{
			this._Path = this._PendingPath;
			this._PendingPath = null;
		}
		else if (this._Path == null && this._CharacterController.isGrounded && !this.IsStunned)
		{
			if (this.Aggressive && this._AggressiveTarget == null)
			{
				Collider[] array = Physics.OverlapSphere(base.transform.position, this.AggressiveDistance);
				List<GameObject> list = new List<GameObject>();
				foreach (Collider collider in array)
				{
					if (!(collider.gameObject == base.gameObject))
					{
						if (collider.GetComponent<PlayerNetwork>() != null)
						{
							list.Add(collider.gameObject);
						}
					}
				}
				if (list.Count > 0)
				{
					GameObject gameObject = list[UnityEngine.Random.Range(0, list.Count)];
					this._AggressiveHomePosition = gameObject.transform.position;
					this._AggressiveTarget = gameObject;
				}
			}
			if (this.Follow && this._FollowTarget == null)
			{
				this._FollowTarget = WorldGameObjectX.Instance.MainPlayer;
			}
			if (this.Aggressive && this._AggressiveTarget != null)
			{
				float num = Vector3.Distance(base.transform.position, this._AggressiveTarget.transform.position);
				if (num <= 2f)
				{
					this.RotateTo(this._AggressiveTarget.transform.position);
					if (this._NetworkAnim != null && this._NetworkAnim.Anim != null && !this._NetworkAnim.Anim.IsPlaying("dig"))
					{
						if (this._NetworkAnim.Anim["dig"] != null)
						{
							this._NetworkAnim.PlayAnimation(NetworkSyncAnimation.AnimState.dig, true);
						}
						EntityBase component = this._AggressiveTarget.GetComponent<EntityBase>();
						if (component != null)
						{
							for (int j = 0; j < this.Damage; j++)
							{
								component.OnLeftMouseHit(string.Empty);
							}
							if (component.Life <= 0)
							{
								this._AggressiveTarget = null;
							}
						}
						if (this._AggressiveTarget == WorldGameObjectX.Instance.MainPlayer)
						{
							WorldGameObjectX.Instance.PunchPlayer(PhotonNetwork.player, base.transform.forward, 0.5f, -1, null, false);
							if (WorldGameObjectX.Instance.MainPlayerNode.Life <= 0f)
							{
								this._AggressiveTarget = null;
							}
						}
					}
				}
				else if (num > this.AggressiveDistance + 1f || Vector3.Distance(base.transform.position, this._AggressiveHomePosition) > this.AggressiveBackDistance)
				{
					this._AggressiveTarget = null;
					this.MoveToPosition(null, this._AggressiveHomePosition, 1f, 0f);
				}
				else
				{
					this.MoveToPosition(this._AggressiveTarget, Vector3.zero, 1f, 1f);
				}
			}
			else if (this.Follow && this._FollowTarget != null)
			{
				float num2 = Vector3.Distance(base.transform.position, this._FollowTarget.transform.position);
				if (num2 <= 3f)
				{
					this.RotateTo(this._FollowTarget.transform.position);
					this.PlayIdle();
				}
				else
				{
					float num3 = (num2 <= 15f) ? 0f : (num2 - 15f);
					this.MoveToPosition(this._FollowTarget, Vector3.zero, 1f + num3, 3f);
				}
			}
			else if (this.Patrolling)
			{
				if (!this._PatrollingIdleActive)
				{
					this._PatrollingIdleActive = true;
					this._IdleTime = Time.time + UnityEngine.Random.Range(this.PatrollingIdle, this.PatrollingIdle * 2f);
				}
				else
				{
					this._PatrollingIdleActive = false;
					this._Path = Pathfinder.FindRandom(base.transform.position, this._PatrollingCenter, this.PatrollingDistance);
					if (this._Path == null)
					{
						this._IdleTime = Time.time + 1f;
					}
				}
			}
			else
			{
				if (this.PlayersMonitoring)
				{
					if (this._PlayersMonitoringTarget != null && Vector3.Distance(this._PlayersMonitoringTarget.transform.position, base.transform.position) > 9f)
					{
						this._PlayersMonitoringTarget = null;
					}
					if (this._PlayersMonitoringTarget == null)
					{
						Collider[] array3 = Physics.OverlapSphere(base.transform.position, 8f);
						foreach (Collider collider2 in array3)
						{
							if (collider2.gameObject != base.gameObject && collider2.GetComponent<PlayerNetwork>() != null)
							{
								this._PlayersMonitoringTarget = collider2.gameObject;
								break;
							}
						}
					}
					if (this._PlayersMonitoringTarget != null)
					{
						this.RotateTo(this._PlayersMonitoringTarget.transform.position);
					}
					else
					{
						this._IdleTime = Time.time + 1f;
					}
				}
				else
				{
					this._IdleTime = Time.time + 1f;
				}
				this.PlayIdle();
			}
		}
		if (this._JumpForce > Physics.gravity.y)
		{
			this._JumpForce = Mathf.MoveTowards(this._JumpForce, Physics.gravity.y, -Physics.gravity.y * Time.deltaTime);
		}
		if (this._Path != null)
		{
			for (int l = 0; l < this._Path.Count; l++)
			{
				UnityEngine.Debug.DrawLine((l != 0) ? this._Path[l - 1] : base.transform.position, this._Path[l], Color.red);
			}
			float num4 = Vector3.Distance(base.transform.position, this._Path[0]);
			if (this._CharacterController.isGrounded)
			{
				if (this._Path[0].y > base.transform.position.y + 0.2f || this._PathNeedJump)
				{
					this._JumpForce = 4.5f;
					this._PathNeedJump = false;
				}
				else
				{
					this._JumpForce = -0.1f;
				}
			}
			Vector3 a = this._Path[0] - base.transform.position;
			a.Normalize();
			a.y = this._JumpForce;
			a.x *= this.Speed * this._SpeedMultiplier;
			a.z *= this.Speed * this._SpeedMultiplier;
			this._CharacterController.Move(a * Time.deltaTime);
			this.RotateTo(this._Path[0]);
			string name = (!this._CharacterController.isGrounded) ? "jump" : "run";
			NetworkSyncAnimation.AnimState anim = (!this._CharacterController.isGrounded) ? NetworkSyncAnimation.AnimState.jump : NetworkSyncAnimation.AnimState.run;
			if (this._NetworkAnim != null && this._NetworkAnim.Anim != null && this._NetworkAnim.Anim[name] != null && !this._NetworkAnim.Anim.IsPlaying(name))
			{
				this._NetworkAnim.PlayAnimation(anim, true);
			}
			if (this._CharacterController.isGrounded)
			{
				if (Vector3.Distance(base.transform.position, this._Path[0]) < 0.1f)
				{
					this._PathFailCounter = 0;
					this._PathNeedJump = (this._Path.Count >= 2 && Vector3.Distance(this._Path[0], this._Path[1]) > 1.9f);
					this._Path.RemoveAt(0);
					if (this._Path.Count == 0)
					{
						this.ClearPath();
					}
				}
				else if (Vector3.Distance(base.transform.position, this._Path[0]) >= num4 && ++this._PathFailCounter >= 10)
				{
					this.ClearPath();
					this._IdleTime = Time.time + 1f;
				}
			}
		}
		else
		{
			if (this._CharacterController.isGrounded)
			{
				this._JumpForce = -0.1f;
			}
			if (this.IsStunned && this._PunchForce != Vector3.zero)
			{
				this._CharacterController.Move(this._PunchForce * Time.deltaTime);
			}
			else
			{
				this._CharacterController.Move(new Vector3(0f, this._JumpForce, 0f) * Time.deltaTime);
			}
		}
	}

	private void MoveToPosition(GameObject targetObject, Vector3 targetPosition, float speedMultiplier, float cutPath)
	{
		this._Path = Pathfinder.Find(base.transform.position, (!(targetObject != null)) ? targetPosition : targetObject.transform.position);
		int num = (int)cutPath;
		if (this._Path != null && num > 0)
		{
			if (this._Path.Count > num)
			{
				this._Path.RemoveRange(this._Path.Count - num, num);
			}
			else
			{
				this.ClearPath();
			}
		}
		if (this._Path == null)
		{
			this._IdleTime = Time.time + 1f;
		}
		this._SpeedMultiplier = speedMultiplier;
		this._PathObject = targetObject;
	}

	private void ValidatePath()
	{
		if (this._PathObject != null && Vector3.Distance(this._Path[this._Path.Count - 1], this._PathObject.transform.position) > 0.1f)
		{
			this.ClearPath();
		}
	}

	private void RotateTo(Vector3 position)
	{
		Vector3 vector = new Vector3(position.x, base.transform.position.y, position.z) - base.transform.position;
		if (vector != Vector3.zero)
		{
			Quaternion b = Quaternion.LookRotation(vector);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 6f);
		}
	}

	private void ClearPath()
	{
		this._Path = null;
		this._PathNeedJump = false;
		this._PathFailCounter = 0;
	}

	private void PlayIdle()
	{
		if (this._NetworkAnim == null || this._NetworkAnim.Anim == null)
		{
			return;
		}
		if (this.IsIdlePlaying())
		{
			return;
		}
		if (this._NetworkAnim.Anim["idle2"] != null && UnityEngine.Random.value > 0.5f)
		{
			this._NetworkAnim.PlayAnimation(NetworkSyncAnimation.AnimState.idle2, true);
		}
		else if (this._NetworkAnim.Anim["idle"] != null)
		{
			this._NetworkAnim.PlayAnimation(NetworkSyncAnimation.AnimState.idle, true);
		}
	}

	private bool IsIdlePlaying()
	{
		return this._NetworkAnim.Anim.IsPlaying("idle") || this._NetworkAnim.Anim.IsPlaying("idle2");
	}

	public const float FollowDistance = 3f;

	public const float FollowSpeedUpDistance = 15f;

	public const float RotationSmoothDamping = 6f;

	public const float PlayersMonitoringDistance = 8f;

	public const float FidgetInterval = 2f;

	public float Speed = 4f;

	public int Damage = 1;

	public bool Aggressive;

	public float AggressiveDistance = 10f;

	public float AggressiveBackDistance = 20f;

	private GameObject _AggressiveTarget;

	private Vector3 _AggressiveHomePosition = Vector3.zero;

	public bool Patrolling;

	public float PatrollingDistance;

	public float PatrollingIdle = 5f;

	private Vector3 _PatrollingCenter = Vector3.zero;

	private bool _PatrollingIdleActive;

	public bool Follow;

	private GameObject _FollowTarget;

	public bool PlayersMonitoring = true;

	private GameObject _PlayersMonitoringTarget;

	private CharacterController _CharacterController;

	private NetworkSyncAnimation _NetworkAnim;

	private float _IdleTime;

	private float _FidgetNextTime;

	private float _StunTime;

	private List<Vector3> _Path;

	private GameObject _PathObject;

	private int _PathFailCounter;

	private bool _PathNeedJump;

	private List<Vector3> _PendingPath;

	private float _JumpForce;

	private float _SpeedMultiplier = 1f;

	private Vector3 _PunchForce = Vector3.zero;
}
