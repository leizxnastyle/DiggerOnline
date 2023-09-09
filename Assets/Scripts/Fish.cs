using System;
using UnityEngine;

public class Fish : EntityBase
{
	protected override void Creation(object[] data)
	{
		BlockType blockType = WorldData.Instance.GetBlockType(base.BlockX, base.BlockY, base.BlockZ - 1);
		BlockType blockType2 = WorldData.Instance.GetBlockType(base.BlockX, base.BlockY, base.BlockZ);
		if (!base.photonView.isMine)
		{
			if (blockType2 != BlockType.Water)
			{
				this.FishObj.GetComponent<Animation>().Play("breathing");
			}
			base.Invoke("TestCurentCube", 1f);
			return;
		}
		if (blockType2 == BlockType.Water)
		{
			this.OnFinishDiving();
			base.Invoke("TestCurentCube", 1f);
		}
		else if (blockType == BlockType.Water)
		{
			Vector3 vector = new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y - 0.6f, base.gameObject.transform.position.z);
			iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
			{
				"easetype",
				iTween.EaseType.easeInOutQuad,
				"position",
				vector,
				"time",
				2f
			}));
			base.Invoke("OnFinishDiving", 2f);
			base.Invoke("TestCurentCube", 1f);
		}
		else
		{
			this.FishObj.GetComponent<Animation>().Play("breathing");
		}
	}

	protected override void PreviewCreation(object[] data)
	{
		this.FishObj.GetComponent<Animation>().Play("breathing");
	}

	public override void OnLeftMouseHit(string playerName)
	{
		if (base.photonView.isMine)
		{
			if (this._CurCube != BlockType.Water)
			{
				base.OnLeftMouseHit(playerName);
			}
			else
			{
				iTween.Stop(base.gameObject);
				base.CancelInvoke("OnFinishDiving");
				this.OnScared(WorldGameObjectX.Instance.MainPlayer.transform.position);
			}
		}
		else
		{
			base.photonView.RPC("OnScared", PhotonTargets.MasterClient, new object[]
			{
				WorldGameObjectX.Instance.MainPlayer.transform.position
			});
		}
	}

	public void TestCurentCube()
	{
		this._CurCube = WorldData.Instance.GetBlockType(base.BlockX, base.BlockY, base.BlockZ);
		if (!base.photonView.isMine)
		{
			return;
		}
		if (this._CurCube != BlockType.Water)
		{
			iTween.Stop(base.gameObject);
			base.CancelInvoke("OnFinishDiving");
			this._NewPos = Vector3.zero;
			int num = (int)base.gameObject.transform.position.x;
			int num2 = (int)base.gameObject.transform.position.y;
			int num3 = (int)base.gameObject.transform.position.z;
			if (WorldData.Instance.GetBlockType(num, num3 - 1, num2) == BlockType.Water)
			{
				this._NewPos = new Vector3((float)num + 0.5f, (float)num2, (float)(num3 - 1) + 0.5f);
				this._LastPos = base.gameObject.transform.position;
			}
			else if (WorldData.Instance.GetBlockType(num, num3 + 1, num2) == BlockType.Water)
			{
				this._NewPos = new Vector3((float)num + 0.5f, (float)num2, (float)(num3 + 1) + 0.5f);
				this._LastPos = base.gameObject.transform.position;
			}
			else if (WorldData.Instance.GetBlockType(num - 1, num3, num2) == BlockType.Water)
			{
				this._NewPos = new Vector3((float)(num - 1) + 0.5f, (float)num2, (float)num3 + 0.5f);
				this._LastPos = base.gameObject.transform.position;
			}
			else if (WorldData.Instance.GetBlockType(num + 1, num3, num2) == BlockType.Water)
			{
				this._NewPos = new Vector3((float)(num + 1) + 0.5f, (float)num2, (float)num3 + 0.5f);
				this._LastPos = base.gameObject.transform.position;
			}
			else if (WorldData.Instance.GetBlockType(num, num3, num2 - 1) == BlockType.Water)
			{
				this._NewPos = new Vector3((float)num + 0.5f, (float)(num2 - 1), (float)num3 + 0.5f);
				this._LastPos = base.gameObject.transform.position;
			}
			else if (WorldData.Instance.GetBlockType(num, num3, num2 + 1) == BlockType.Water)
			{
				this._NewPos = new Vector3((float)num + 0.5f, (float)(num2 + 1), (float)num3 + 0.5f);
				this._LastPos = base.gameObject.transform.position;
			}
			if (this._NewPos != Vector3.zero)
			{
				base.photonView.RPC("MoveTo", PhotonTargets.All, new object[]
				{
					this._NewPos,
					1f,
					1f,
					true
				});
				base.Invoke("OnFinishDiving", 1f);
			}
			else
			{
				base.Invoke("OnFinishDiving", 2f);
			}
		}
		base.Invoke("TestCurentCube", 1f);
	}

	public void OnFinishDiving()
	{
		this._CurCube = WorldData.Instance.GetBlockType((int)base.gameObject.transform.position.x, (int)base.gameObject.transform.position.z, (int)base.gameObject.transform.position.y);
		if (this._CurCube != BlockType.Water)
		{
			return;
		}
		Vector3 positionToMove = this.GetPositionToMove();
		if (positionToMove == Vector3.zero)
		{
			base.Invoke("OnFinishDiving", 2f);
			return;
		}
		float magnitude = (positionToMove - base.gameObject.transform.position).magnitude;
		base.photonView.RPC("MoveTo", PhotonTargets.All, new object[]
		{
			positionToMove,
			3f,
			magnitude * 4f,
			false
		});
		base.Invoke("OnFinishDiving", magnitude * 4f);
	}

	[PunRPC]
	public void MoveTo(Vector3 moveTo, float lookTime, float time, bool easeTypeOut)
	{
		iTween.Stop(base.gameObject);
		base.CancelInvoke("OnFinishDiving");
		iTween.EaseType easeType;
		if (easeTypeOut)
		{
			easeType = iTween.EaseType.easeOutExpo;
		}
		else
		{
			easeType = iTween.EaseType.easeInOutQuart;
		}
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"looktarget",
			moveTo,
			"axis",
			"y",
			"looktime",
			lookTime,
			"easetype",
			easeType,
			"position",
			moveTo,
			"time",
			time
		}));
	}

	[PunRPC]
	public void OnScared(Vector3 myPos)
	{
		if (this._CurCube != BlockType.Water)
		{
			return;
		}
		iTween.Stop(base.gameObject);
		base.CancelInvoke("OnFinishDiving");
		Vector3 positionToMove = this.GetPositionToMove();
		Vector3 positionToMove2 = this.GetPositionToMove();
		Vector3 positionToMove3 = this.GetPositionToMove();
		float num = (!(positionToMove != Vector3.zero)) ? 0f : (positionToMove - myPos).magnitude;
		float num2 = (!(positionToMove2 != Vector3.zero)) ? 0f : (positionToMove2 - myPos).magnitude;
		float num3 = (!(positionToMove3 != Vector3.zero)) ? 0f : (positionToMove3 - myPos).magnitude;
		Vector3 vector = Vector3.zero;
		if (num > num2 && num > num3)
		{
			vector = positionToMove;
		}
		else if (num2 > num && num2 > num3)
		{
			vector = positionToMove2;
		}
		else if (num3 > num && num3 > num2)
		{
			vector = positionToMove3;
		}
		if (vector != Vector3.zero)
		{
			float num4 = Mathf.Max(Mathf.Max(num, num2), num3);
			base.photonView.RPC("MoveTo", PhotonTargets.All, new object[]
			{
				vector,
				1f,
				num4,
				true
			});
			base.Invoke("OnFinishDiving", num4);
		}
		else
		{
			base.Invoke("OnFinishDiving", 2f);
		}
	}

	private bool IsPointInMap(Vector3 pos)
	{
		return pos.y >= 1f && pos.y <= 125f && pos.x >= 16f && pos.z >= 16f && pos.x <= (float)WorldData.Instance.GetMaxBlockX() && pos.z <= (float)WorldData.Instance.GetMaxBlockY();
	}

	private Vector3 GetPositionToMove()
	{
		for (int i = 0; i < 10; i++)
		{
			int num = this._Random.Next((int)base.gameObject.transform.position.x - 3, (int)base.gameObject.transform.position.x + 3);
			int num2 = this._Random.Next((int)base.gameObject.transform.position.y - 3, (int)base.gameObject.transform.position.y + 3);
			int num3 = this._Random.Next((int)base.gameObject.transform.position.z - 3, (int)base.gameObject.transform.position.z + 3);
			if (this.IsPointInMap(new Vector3((float)num, (float)num2, (float)num3)))
			{
				if (WorldData.Instance.GetBlockType(num, num3, num2) == BlockType.Water)
				{
					this._NewPos = new Vector3((float)num + 0.5f, (float)num2, (float)num3 + 0.5f);
					if (this._LastPos != this._NewPos)
					{
						this._LastPos = base.gameObject.transform.position;
						return this._NewPos;
					}
				}
			}
		}
		int num4 = (int)base.gameObject.transform.position.x;
		int num5 = (int)base.gameObject.transform.position.y;
		int num6 = (int)base.gameObject.transform.position.z;
		if (WorldData.Instance.GetBlockType(num4, num6 - 1, num5) == BlockType.Water)
		{
			this._NewPos = new Vector3((float)num4 + 0.5f, (float)num5, (float)(num6 - 1) + 0.5f);
			if (this._LastPos != this._NewPos)
			{
				this._LastPos = base.gameObject.transform.position;
				return this._NewPos;
			}
		}
		if (WorldData.Instance.GetBlockType(num4, num6 + 1, num5) == BlockType.Water)
		{
			this._NewPos = new Vector3((float)num4 + 0.5f, (float)num5, (float)(num6 + 1) + 0.5f);
			if (this._LastPos != this._NewPos)
			{
				this._LastPos = base.gameObject.transform.position;
				return this._NewPos;
			}
		}
		if (WorldData.Instance.GetBlockType(num4 - 1, num6, num5) == BlockType.Water)
		{
			this._NewPos = new Vector3((float)(num4 - 1) + 0.5f, (float)num5, (float)num6 + 0.5f);
			if (this._LastPos != this._NewPos)
			{
				this._LastPos = base.gameObject.transform.position;
				return this._NewPos;
			}
		}
		if (WorldData.Instance.GetBlockType(num4 + 1, num6, num5) == BlockType.Water)
		{
			this._NewPos = new Vector3((float)(num4 + 1) + 0.5f, (float)num5, (float)num6 + 0.5f);
			if (this._LastPos != this._NewPos)
			{
				this._LastPos = base.gameObject.transform.position;
				return this._NewPos;
			}
		}
		if (WorldData.Instance.GetBlockType(num4, num6, num5 - 1) == BlockType.Water)
		{
			this._NewPos = new Vector3((float)num4 + 0.5f, (float)(num5 - 1), (float)num6 + 0.5f);
			if (this._LastPos != this._NewPos)
			{
				this._LastPos = base.gameObject.transform.position;
				return this._NewPos;
			}
		}
		if (WorldData.Instance.GetBlockType(num4, num6, num5 + 1) == BlockType.Water)
		{
			this._NewPos = new Vector3((float)num4 + 0.5f, (float)(num5 + 1), (float)num6 + 0.5f);
			if (this._LastPos != this._NewPos)
			{
				this._LastPos = base.gameObject.transform.position;
				return this._NewPos;
			}
		}
		return Vector3.zero;
	}

	public const int MaxFindPosIteration = 10;

	public const int FindPosRadius = 3;

	public GameObject FishObj;

	private Vector3 _LastPos = Vector3.zero;

	private Vector3 _NewPos;

	private BlockType _CurCube;

	private System.Random _Random = new System.Random();
}
