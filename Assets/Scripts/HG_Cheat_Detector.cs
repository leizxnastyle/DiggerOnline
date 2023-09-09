using System;
using System.Collections;
using UnityEngine;

public class HG_Cheat_Detector : MonoBehaviour
{
	public int Player_ID { get; private set; }

	public string View_ID { get; private set; }

	private IEnumerator Start()
	{
		this.input = base.transform.GetComponent<PlayerInput>();
		yield return new WaitForSeconds(5f);
		base.InvokeRepeating("CheatCheck", this._update_time, this._update_time);
		yield break;
	}

	public void OnDisable()
	{
		if (this.wall != null)
		{
			this.wall.GetComponent<HG_Cheat_Wall_Detector>().isTriggerWallOn -= this.HG_Cheat_Detector_isTriggerWallOn;
		}
	}

	public void Init(int id, float update_count, string vid)
	{
		this.Player_ID = id;
		this.View_ID = vid;
		this._playerNode = WorldGameObjectX.Instance.FindPlayerByViewerID(this.View_ID);
		this._update_time = update_count;
		GameObject original = (GameObject)Resources.Load("DWork/Prefabs/CheatWall", typeof(GameObject));
		this.wall = UnityEngine.Object.Instantiate<GameObject>(original);
		this.wall.transform.parent = base.transform;
		this.wall.transform.localPosition = new Vector3(0f, 0.3f, 0f);
		this.wall.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		this.wall.GetComponent<HG_Cheat_Wall_Detector>().isTriggerWallOn += this.HG_Cheat_Detector_isTriggerWallOn;
		base.InvokeRepeating("CheckLava", 0f, 2f);
	}

	private void Update()
	{
		this.CalculateLocalVelocity();
		if (GameType.IsArcadeMode)
		{
			this.CheckFly();
		}
	}

	private void CalculateLocalVelocity()
	{
		this.localeVelocity = (base.transform.position - this.previous) / Time.deltaTime;
		this.previous = base.transform.position;
	}

	private void HG_Cheat_Detector_isTriggerWallOn()
	{
		if (!GameType.IsObserving(this._playerNode.Name) && Mathf.Abs(this.localeVelocity.x) < this._max_available_speed && Mathf.Abs(this.localeVelocity.z) < this._max_available_speed)
		{
			UnityEngine.Debug.Log("WALL CRACK ENABLE");
		}
	}

	private void CheatCheck()
	{
	}

	private void CheckFly()
	{
		if (!this._playerNode.PlayerOnLadder && !GameType.IsObserving(this._playerNode.Name))
		{
			Ray ray = new Ray(base.transform.position, -base.transform.up);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, 100f))
			{
				float num = Vector3.Distance(ray.origin, raycastHit.point);
				if (num >= 2f && this.localeVelocity.y > 0f)
				{
					if (!this._checkFly)
					{
						this.start_fly_time = Time.time;
					}
					this._checkFly = true;
				}
				else if (num < 2f || this.localeVelocity.y <= 0f)
				{
					if (this._checkFly)
					{
						float num2 = Time.time - this.start_fly_time;
						UnityEngine.Debug.Log("---fly_time = " + num2);
						if (num2 > 1f)
						{
							CheatFinderManager.ReportCheat(this.View_ID, CheatFinderManager.CheatType.CT_ALL_FLY_CHEAT);
						}
					}
					this._checkFly = false;
				}
			}
		}
		else
		{
			this._checkFly = false;
		}
	}

	private void CheckSpeed()
	{
		if (Mathf.Abs(this.localeVelocity.x) > this._max_available_speed || Mathf.Abs(this.localeVelocity.z) > this._max_available_speed)
		{
			if (!this._speedCrackDetect)
			{
				this._speedCrackDetect = true;
			}
			else
			{
				CheatFinderManager.ReportCheat(this.View_ID, CheatFinderManager.CheatType.CT_ALL_SPEED_CHEAT);
				UnityEngine.Debug.Log("SPEED CRACK ENABLE");
			}
		}
		else
		{
			this._speedCrackDetect = false;
		}
	}

	private void CheckLava()
	{
		if (this._playerNode.Life <= 0f)
		{
			return;
		}
		Vector3 origin = base.transform.position + base.transform.up * 0.5f;
		Vector3 direction = -base.transform.up * 0.5f;
		RaycastHit raycastHit;
		if (Physics.Raycast(origin, direction, out raycastHit, direction.magnitude, 1 << LayerMask.NameToLayer("Terrain")) && raycastHit.collider.tag != "Water")
		{
			Vector3 vector = raycastHit.point - raycastHit.normal * 0.1f;
			IntVect intVect = new IntVect(vector.x, vector.z, vector.y);
			BlockType blockType = WorldData.Instance.GetBlockType(intVect.X, intVect.Y, intVect.Z);
			if (blockType == BlockType.Lava)
			{
				if (this.isLavaStay)
				{
					CheatFinderManager.ReportCheat(this.View_ID, CheatFinderManager.CheatType.CT_LAVA_STAY);
				}
				else
				{
					this.isLavaStay = true;
				}
			}
			else
			{
				this.isLavaStay = false;
			}
		}
	}

	private PlayerNode _playerNode;

	private float _update_time = 0.3f;

	private float _max_available_speed = 7f;

	private Transform main_parent;

	private GameObject wall;

	private PlayerInput input;

	private bool _checkFly;

	private bool _speedCrackDetect;

	private float start_fly_time;

	private Vector3 localeVelocity = Vector3.zero;

	private Vector3 previous = Vector3.zero;

	public bool isLavaStay;
}
