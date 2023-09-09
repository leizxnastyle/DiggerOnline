using System;
using System.Collections.Generic;
using UnityEngine;

public class Rail : EntityBase
{
	public int PlaneIndex
	{
		get
		{
			return this._PlaneIndex;
		}
	}

	public static Vector3[] PlanePosition
	{
		get
		{
			return Rail._PlanePosition;
		}
	}

	public static Vector3[] PlaneRotation
	{
		get
		{
			return Rail._PlaneRotation;
		}
	}

	public bool IsTransition
	{
		get
		{
			return this._IsTransition;
		}
	}

	public static void ClearAttachedRails()
	{
		Rail._AttachedRails.Clear();
	}

	public static Rail Find(int blockX, int blockY, int blockZ)
	{
		foreach (Rail rail in Rail._AttachedRails)
		{
			if (rail.BlockX == blockX && rail.BlockY == blockY && rail.BlockZ == blockZ)
			{
				return rail;
			}
		}
		return null;
	}

	public static Rail Find(Collider collider)
	{
		foreach (Rail rail in Rail._AttachedRails)
		{
			if (rail._ModelCollider.bounds.Intersects(collider.bounds))
			{
				return rail;
			}
		}
		return null;
	}

	public float GetRotationToRail(Rail rail)
	{
		float result;
		if (this._PlaneIndex == rail._PlaneIndex)
		{
			if (this._PlaneBlockX == rail._PlaneBlockX)
			{
				result = (float)((this._PlaneBlockY >= rail._PlaneBlockY) ? 0 : 180);
			}
			else
			{
				result = (float)((this._PlaneBlockX >= rail._PlaneBlockX) ? 90 : 270);
			}
		}
		else
		{
			int num = Rail._PlaneDirections[this._PlaneIndex, rail._PlaneIndex];
			int blockX = rail.BlockX;
			int blockY = rail.BlockY;
			int blockZ = rail.BlockZ;
			Rail.SetPlaneBlock(this._PlaneIndex, ref blockX, ref blockY, ref blockZ);
			if (blockZ < this._PlaneBlockZ)
			{
				num = (num + 2) % 4;
			}
			result = (float)((num != 0) ? ((num != 1) ? ((num != 2) ? 180 : 270) : 0) : 90);
		}
		return result;
	}

	private void Attach()
	{
		if (base.transform.position.y % 1f < 0.1f)
		{
			this._PlaneIndex = 0;
		}
		else if (base.transform.position.y % 1f > 0.9f)
		{
			this._PlaneIndex = 1;
		}
		else if (base.transform.position.x % 1f < 0.1f)
		{
			this._PlaneIndex = 2;
		}
		else if (base.transform.position.x % 1f > 0.9f)
		{
			this._PlaneIndex = 3;
		}
		else if (base.transform.position.z % 1f < 0.1f)
		{
			this._PlaneIndex = 4;
		}
		else
		{
			if (base.transform.position.z % 1f <= 0.1f)
			{
				UnityEngine.Debug.Log("Wrong rail position! " + base.transform.position);
				return;
			}
			this._PlaneIndex = 5;
		}
		base.transform.position = new Vector3((float)base.BlockX + 0.5f, (float)base.BlockZ, (float)base.BlockY + 0.5f);
		base.transform.rotation = Quaternion.identity;
		base.transform.Translate(Rail._PlanePosition[this._PlaneIndex], Space.World);
		base.transform.Rotate(Rail._PlaneRotation[this._PlaneIndex], Space.World);
		this._PlaneBlockX = base.BlockX;
		this._PlaneBlockY = base.BlockY;
		this._PlaneBlockZ = base.BlockZ;
		Rail.SetPlaneBlock(this._PlaneIndex, ref this._PlaneBlockX, ref this._PlaneBlockY, ref this._PlaneBlockZ);
		foreach (Rail rail in Rail._AttachedRails)
		{
			if (this.IsSibling(rail))
			{
				this.SetSibling(rail, true);
				rail.SetSibling(this, true);
			}
		}
		if (this._Model == null)
		{
			this.SetModel(this.RailTwoStrong);
		}
		Rail._AttachedRails.Add(this);
		if (Rail._AttachedRailsGroup == null)
		{
			Rail._AttachedRailsGroup = new GameObject("Rails").transform;
		}
		base.transform.parent = Rail._AttachedRailsGroup;
		Trolley.OnRailAttached(this);
	}

	[PunRPC]
	private void Detach()
	{
		if (this._Siblings != null)
		{
			foreach (Rail rail in this._Siblings)
			{
				rail.SetSibling(this, false);
			}
			this._Siblings = null;
		}
		Rail._AttachedRails.Remove(this);
		base.transform.parent = null;
		Trolley.OnRailDetached(this);
	}

	public override void SelfDelete()
	{
		base.photonView.RPC("Detach", PhotonTargets.All, new object[0]);
		base.SelfDelete();
	}

	public Rail GetNext(Vector3 direction, float closestAngle)
	{
		if (this._Siblings != null)
		{
			Rail rail = null;
			float num = 0f;
			foreach (Rail rail2 in this._Siblings)
			{
				Vector3 vector = rail2.transform.position - base.transform.position;
				float num2 = Vector3.Angle(vector, direction);
				if (Vector3.Dot(Vector3.Cross(vector, direction), base.transform.up) < 0f)
				{
					num2 = 360f - num2;
				}
				if (num2 > 225f || num2 < 135f)
				{
					float num3 = Mathf.Abs(Mathf.DeltaAngle(num2, closestAngle));
					if (rail == null || num3 < num)
					{
						rail = rail2;
						num = num3;
					}
				}
			}
			return rail;
		}
		return null;
	}

	public Rail GetDown()
	{
		if (this._Siblings != null)
		{
			foreach (Rail rail in this._Siblings)
			{
				if (rail.BlockZ < base.BlockZ || (rail.BlockZ == base.BlockZ && rail._IsTransition))
				{
					return rail;
				}
			}
		}
		return null;
	}

	public static void SetPlaneBlock(int planeIndex, ref int blockX, ref int blockY, ref int blockZ)
	{
		int num = blockX;
		int num2 = blockY;
		int num3 = blockZ;
		switch (planeIndex)
		{
		case 1:
			blockX = WorldData.Instance.WidthInBlocks - num;
			blockZ = WorldData.Instance.DepthInBlocks - num3;
			break;
		case 2:
			blockX = WorldData.Instance.DepthInBlocks - num3;
			blockZ = num;
			break;
		case 3:
			blockX = num3;
			blockZ = WorldData.Instance.WidthInBlocks - num;
			break;
		case 4:
			blockY = WorldData.Instance.DepthInBlocks - num3;
			blockZ = num2;
			break;
		case 5:
			blockY = num3;
			blockZ = WorldData.Instance.HeightInBlocks - num2;
			break;
		}
	}

	protected override void Creation(object[] data)
	{
		this.Attach();
	}

	protected override void Updating()
	{
		if (this._Siblings != null)
		{
			foreach (Rail rail in this._Siblings)
			{
				UnityEngine.Debug.DrawLine(base.transform.position, rail.transform.position, Color.yellow);
			}
		}
	}

	private bool IsSibling(Rail rail)
	{
		if (this._PlaneIndex == rail._PlaneIndex)
		{
			int num = rail._PlaneBlockX - this._PlaneBlockX;
			int num2 = rail._PlaneBlockY - this._PlaneBlockY;
			int num3 = rail._PlaneBlockZ - this._PlaneBlockZ;
			int num4 = Mathf.Abs(num);
			int num5 = Mathf.Abs(num2);
			int num6 = Mathf.Abs(num3);
			if (num4 > 1 || num5 > 1 || num6 > 1 || num4 + num5 != 1)
			{
				return false;
			}
			int num7 = ((num == 0) ? ((num2 != -1) ? 3 : 1) : ((num != -1) ? 2 : 0)) + (num3 + 1) * 4;
			int num8 = 11 - num7 - ((num7 % 2 != 0) ? -1 : 1);
			return (this._ValidSiblingBits >> num7 & 1) != 0 && (rail._ValidSiblingBits >> num8 & 1) != 0;
		}
		else
		{
			int blockX = rail.BlockX;
			int blockY = rail.BlockY;
			int blockZ = rail.BlockZ;
			Rail.SetPlaneBlock(this._PlaneIndex, ref blockX, ref blockY, ref blockZ);
			int num9 = blockX - this._PlaneBlockX;
			int num10 = blockY - this._PlaneBlockY;
			int num11 = blockZ - this._PlaneBlockZ;
			if (num9 == 0 && num10 == 0 && num11 == 1)
			{
				int num12 = Rail._PlaneDirections[this._PlaneIndex, rail._PlaneIndex];
				if (num12 == -1)
				{
					return false;
				}
				num12 += 8;
				int num13 = Rail._PlaneDirections[rail._PlaneIndex, this._PlaneIndex] + 8;
				return (this._ValidSiblingBits >> num12 & 1) != 0 && (rail._ValidSiblingBits >> num13 & 1) != 0;
			}
			else
			{
				if (Mathf.Abs(num9) + Mathf.Abs(num10) != 1 || num11 != -1)
				{
					return false;
				}
				int num14 = Rail._PlaneDirections[this._PlaneIndex, rail._PlaneIndex];
				if (num14 == -1)
				{
					return false;
				}
				num14 = (num14 + 2) % 4;
				int num15 = (Rail._PlaneDirections[rail._PlaneIndex, this._PlaneIndex] + 2) % 4;
				return (this._ValidSiblingBits >> num14 & 1) != 0 && (rail._ValidSiblingBits >> num15 & 1) != 0;
			}
		}
	}

	private void SetSibling(Rail rail, bool add)
	{
		if (add)
		{
			if (this._Siblings == null)
			{
				this._Siblings = new List<Rail>();
			}
			this._Siblings.Add(rail);
		}
		else
		{
			this._Siblings.Remove(rail);
			if (this._Siblings.Count == 0)
			{
				this._Siblings = null;
			}
		}
		int num = 0;
		int num2 = 0;
		if (this._Siblings != null)
		{
			foreach (Rail rail2 in this._Siblings)
			{
				int num5;
				bool flag;
				if (rail2._PlaneIndex == this._PlaneIndex)
				{
					int num3 = rail2._PlaneBlockX - this._PlaneBlockX;
					int num4 = rail2._PlaneBlockY - this._PlaneBlockY;
					num5 = ((num3 == 0) ? ((num4 != -1) ? 3 : 1) : ((num3 != -1) ? 2 : 0));
					flag = (this._PlaneBlockZ < rail2._PlaneBlockZ);
				}
				else
				{
					num5 = Rail._PlaneDirections[this._PlaneIndex, rail2._PlaneIndex];
					int blockX = rail2.BlockX;
					int blockY = rail2.BlockY;
					int blockZ = rail2.BlockZ;
					Rail.SetPlaneBlock(this._PlaneIndex, ref blockX, ref blockY, ref blockZ);
					flag = (this._PlaneBlockZ < blockZ);
					if (blockZ < this._PlaneBlockZ)
					{
						num5 = (num5 + 2) % 4;
					}
				}
				int num6 = 0;
				if (flag)
				{
					num6 |= 1 << (num5 + 2) % 4;
					num6 |= 1 << (num5 + 2) % 4 + 4;
				}
				else
				{
					num6 |= 1 << (num5 + 2) % 4;
					num6 |= 1 << (num5 + 2) % 4 + 4;
					num6 |= 1 << (num5 + 2) % 4 + 8;
					num6 |= 1 << (num5 + 1) % 4;
					num6 |= 1 << (num5 + 3) % 4;
					num6 |= 1 << (num5 + 1) % 4 + 4;
					num6 |= 1 << (num5 + 3) % 4 + 4;
				}
				num |= (num6 ^ 4095);
				num2 |= 1 << num5;
				if (flag)
				{
					num2 |= 1 << num5 + 4;
				}
			}
		}
		this._ValidSiblingBits = (num ^ 4095);
		if (this._Siblings != null)
		{
			Vector3 zero = Vector3.zero;
			GameObject model;
			if (this._Siblings.Count == 1)
			{
				model = this.RailTwoStrong;
				zero = new Vector3(0f, (float)(((num2 & 1) == 0) ? (((num2 & 2) == 0) ? (((num2 & 4) == 0) ? 180 : 270) : 0) : 90), 0f);
				if ((num2 & 240) != 0)
				{
					model = this.RailElevation;
					this._IsTransition = true;
				}
			}
			else if (this._Siblings.Count == 2)
			{
				if ((num2 & 5) == 5 || (num2 & 10) == 10)
				{
					if ((num2 & 240) != 0)
					{
						num2 = (num2 >> 4 | (num2 & 240));
					}
					model = this.RailTwoStrong;
					zero = new Vector3(0f, (float)(((num2 & 1) == 0) ? (((num2 & 2) == 0) ? (((num2 & 4) == 0) ? 180 : 270) : 0) : 90), 0f);
					if ((num2 & 240) != 0)
					{
						model = this.RailElevation;
						this._IsTransition = true;
					}
				}
				else
				{
					model = this.RailTwoCurve;
					zero = new Vector3(0f, (float)(((num2 & 3) != 3) ? (((num2 & 6) != 6) ? (((num2 & 12) != 12) ? 90 : 180) : 270) : 0), 0f);
				}
			}
			else if (this._Siblings.Count == 3)
			{
				int num7 = 0;
				if ((num2 & 1) == 0)
				{
					num7 = 270;
				}
				else if ((num2 & 2) == 0)
				{
					num7 = 180;
				}
				else if ((num2 & 4) == 0)
				{
					num7 = 90;
				}
				model = this.RailThree;
				zero = new Vector3(0f, (float)num7, 0f);
			}
			else
			{
				model = this.RailFour;
			}
			this.SetModel(model);
			zero.y -= 90f;
			this._Model.transform.localRotation = Quaternion.Euler(zero);
		}
		else
		{
			this.SetModel(this.RailTwoStrong);
		}
	}

	private void SetModel(GameObject model)
	{
		if (this._Model != null)
		{
			UnityEngine.Object.Destroy(this._Model);
		}
		this._Model = (UnityEngine.Object.Instantiate(model, base.transform.position, base.transform.rotation) as GameObject);
		this._Model.transform.parent = base.transform;
		this._Model.GetComponentInChildren<EntityBaseChild>().Parent = this;
		this._ModelCollider = this._Model.GetComponentInChildren<Collider>();
		base.UpdateTextures();
		TimeOfDay.Affect(this._Model);
	}

	public override void OnButtonE(string playerName)
	{
		if (this.Creator == playerName || Level.Instance.IsAdmin(null))
		{
			this.SelfDelete();
		}
		else
		{
			Chat.SendInfoF(ProfileINI.nickname + Localize.GetText("CANT_TAKE_AND_DESTROY_ITEMS", null), false);
		}
	}

	public GameObject RailTwoStrong;

	public GameObject RailTwoCurve;

	public GameObject RailThree;

	public GameObject RailFour;

	public GameObject RailElevation;

	private static List<Rail> _AttachedRails = new List<Rail>();

	private static Transform _AttachedRailsGroup = null;

	private GameObject _Model;

	private Collider _ModelCollider;

	private List<Rail> _Siblings;

	private int _ValidSiblingBits = 4095;

	private int _PlaneIndex = -1;

	private int _PlaneBlockX;

	private int _PlaneBlockY;

	private int _PlaneBlockZ;

	private static Vector3[] _PlanePosition = new Vector3[]
	{
		new Vector3(0f, 0.01f, 0f),
		new Vector3(0f, 0.99f, 0f),
		new Vector3(-0.49f, 0.49f, 0f),
		new Vector3(0.49f, 0.49f, 0f),
		new Vector3(0f, 0.49f, -0.49f),
		new Vector3(0f, 0.49f, 0.49f)
	};

	private static Vector3[] _PlaneRotation = new Vector3[]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 180f),
		new Vector3(0f, 0f, -90f),
		new Vector3(0f, 0f, 90f),
		new Vector3(90f, 0f, 0f),
		new Vector3(-90f, 0f, 0f)
	};

	private static int[,] _PlaneDirections = new int[,]
	{
		{
			-1,
			-1,
			0,
			2,
			1,
			3
		},
		{
			-1,
			-1,
			2,
			0,
			1,
			3
		},
		{
			2,
			0,
			-1,
			-1,
			1,
			3
		},
		{
			0,
			2,
			-1,
			-1,
			1,
			3
		},
		{
			3,
			1,
			0,
			2,
			-1,
			-1
		},
		{
			1,
			3,
			0,
			2,
			-1,
			-1
		}
	};

	private bool _IsTransition;
}
