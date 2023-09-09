using System;
using PreviewModels;
using UnityEngine;

public class DiagonalPreview : SimplePreviewModel
{
	public override CommonBlockKind CommonBlock
	{
		get
		{
			return CommonBlockKind.Diagonal;
		}
	}

	public override BlockKind Kind
	{
		get
		{
			return this._currentKind;
		}
	}

	public override bool IsPreview
	{
		get
		{
			return this._isPreview;
		}
	}

	public override void ShowPreview()
	{
		if (this._diagonalPreview == null)
		{
			this.obj = Resources.Load("diagonal", typeof(GameObject));
			if (this.obj != null)
			{
				this._diagonalPreview = (GameObject)UnityEngine.Object.Instantiate(this.obj, Vector3.zero, Quaternion.identity);
			}
			this.obj = null;
		}
		else
		{
			this.Update();
		}
	}

	public override void DestroyPreview()
	{
		UnityEngine.Object.Destroy(this._diagonalPreview);
		this._diagonalPreview = null;
	}

	private void PreviewRotation()
	{
		if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			this._diagonalPreview.transform.Rotate(Vector3.right, 90f);
		}
		else if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			this._diagonalPreview.transform.Rotate(Vector3.right, -90f);
		}
	}

	private void PreviewPosition()
	{
		Vector3 vector = this._hit.point + this._hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)vector.x, (int)vector.z, (int)vector.y);
		this._diagonalPreview.transform.position = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 0.5f, (float)intVect.Y + 0.5f);
		BlockKind blockKind = WorldData.Instance.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
		if (blockKind.IsHalf() || blockKind.IsQuarter() || blockKind.IsThird())
		{
			this._diagonalPreview.transform.position = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 1.5f, (float)intVect.Y + 0.5f);
		}
		if (Vector3.Angle(this._hit.normal, this._side) != 0f)
		{
			if (Vector3.Angle(this._hit.normal, new Vector3(0.1f, 0f, 0f)) == 0f)
			{
				this._diagonalPreview.transform.rotation = this.SetLastPosition(Side.East);
			}
			if (Vector3.Angle(this._hit.normal, new Vector3(-0.1f, 0f, 0f)) == 0f)
			{
				this._diagonalPreview.transform.rotation = this.SetLastPosition(Side.West);
			}
			if (Vector3.Angle(this._hit.normal, new Vector3(0f, 0f, 0.1f)) == 0f)
			{
				this._diagonalPreview.transform.rotation = this.SetLastPosition(Side.North);
			}
			if (Vector3.Angle(this._hit.normal, new Vector3(0f, 0f, -0.1f)) == 0f)
			{
				this._diagonalPreview.transform.rotation = this.SetLastPosition(Side.South);
			}
			this._side = this._hit.normal;
		}
	}

	private Quaternion SetLastPosition(Side side)
	{
		Quaternion result = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		switch (side)
		{
		case Side.East:
			if (this._lastPosition == Position.bottom)
			{
				return Quaternion.Euler(new Vector3(90f, 180f, 0f));
			}
			if (this._lastPosition == Position.top)
			{
				return Quaternion.Euler(new Vector3(270f, 180f, 0f));
			}
			if (this._lastPosition == Position.left)
			{
				return Quaternion.Euler(new Vector3(0f, 180f, 0f));
			}
			if (this._lastPosition == Position.right)
			{
				return Quaternion.Euler(new Vector3(0f, 0f, 180f));
			}
			return result;
		case Side.West:
			if (this._lastPosition == Position.bottom)
			{
				return Quaternion.Euler(new Vector3(90f, 0f, 0f));
			}
			if (this._lastPosition == Position.top)
			{
				return Quaternion.Euler(new Vector3(270f, 0f, 0f));
			}
			if (this._lastPosition == Position.left)
			{
				return Quaternion.Euler(new Vector3(0f, 0f, 0f));
			}
			if (this._lastPosition == Position.right)
			{
				return Quaternion.Euler(new Vector3(0f, 180f, 180f));
			}
			return result;
		case Side.South:
			if (this._lastPosition == Position.bottom)
			{
				return Quaternion.Euler(new Vector3(90f, 270f, 0f));
			}
			if (this._lastPosition == Position.top)
			{
				return Quaternion.Euler(new Vector3(270f, 270f, 0f));
			}
			if (this._lastPosition == Position.left)
			{
				return Quaternion.Euler(new Vector3(0f, 270f, 0f));
			}
			if (this._lastPosition == Position.right)
			{
				return Quaternion.Euler(new Vector3(0f, 90f, 180f));
			}
			return result;
		case Side.North:
			if (this._lastPosition == Position.bottom)
			{
				return Quaternion.Euler(new Vector3(90f, 90f, 0f));
			}
			if (this._lastPosition == Position.top)
			{
				return Quaternion.Euler(new Vector3(270f, 90f, 0f));
			}
			if (this._lastPosition == Position.left)
			{
				return Quaternion.Euler(new Vector3(0f, 90f, 0f));
			}
			if (this._lastPosition == Position.right)
			{
				return Quaternion.Euler(new Vector3(0f, 270f, 180f));
			}
			return result;
		default:
			return result;
		}
	}

	private void Update()
	{
		this.IsDiagonal();
		this._diagonalPreview.SetActive(this._isPreview);
		if (this._diagonalPreview.activeInHierarchy)
		{
			this.PreviewRotation();
			this.PreviewPosition();
			this._currentKind = this.GetCurrentKind();
			this.SetMaterial();
			MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.Wheel);
		}
	}

	private BlockKind GetCurrentKind()
	{
		Vector3 vector = this.NormalizedRotation(this._diagonalPreview.transform.rotation.eulerAngles);
		BlockKind currentKind = this._currentKind;
		if (Vector3.Angle(this._hit.normal, new Vector3(-0.1f, 0f, 0f)) == 0f)
		{
			if (vector.Equals(new Vector3(90f, 0f, 0f)))
			{
				this._lastPosition = Position.bottom;
				return BlockKind.DiagonalOnWallWestBottom;
			}
			if (vector.Equals(new Vector3(270f, 0f, 0f)))
			{
				this._lastPosition = Position.top;
				return BlockKind.DiagonalOnWallWestTop;
			}
			if (vector.Equals(new Vector3(0f, 0f, 0f)))
			{
				this._lastPosition = Position.left;
				return BlockKind.DiagonalOnWallWestLeft;
			}
			if (vector.Equals(new Vector3(0f, 180f, 180f)))
			{
				this._lastPosition = Position.right;
				return BlockKind.DiagonalOnWallWestRight;
			}
		}
		if (Vector3.Angle(this._hit.normal, new Vector3(0.1f, 0f, 0f)) == 0f)
		{
			if (vector.Equals(new Vector3(90f, 180f, 0f)))
			{
				this._lastPosition = Position.bottom;
				return BlockKind.DiagonalOnWallEastBottom;
			}
			if (vector.Equals(new Vector3(270f, 180f, 0f)))
			{
				this._lastPosition = Position.top;
				return BlockKind.DiagonalOnWallEastTop;
			}
			if (vector.Equals(new Vector3(0f, 180f, 0f)))
			{
				this._lastPosition = Position.left;
				return BlockKind.DiagonalOnWallEastLeft;
			}
			if (vector.Equals(new Vector3(0f, 0f, 180f)))
			{
				this._lastPosition = Position.right;
				return BlockKind.DiagonalOnWallEastRight;
			}
		}
		if (Vector3.Angle(this._hit.normal, new Vector3(0f, 0f, 0.1f)) == 0f)
		{
			if (vector.Equals(new Vector3(90f, 90f, 0f)))
			{
				this._lastPosition = Position.bottom;
				return BlockKind.DiagonalOnWallNorthBottom;
			}
			if (vector.Equals(new Vector3(270f, 90f, 0f)))
			{
				this._lastPosition = Position.top;
				return BlockKind.DiagonalOnWallNorthTop;
			}
			if (vector.Equals(new Vector3(0f, 90f, 0f)))
			{
				this._lastPosition = Position.left;
				return BlockKind.DiagonalOnWallNorthLeft;
			}
			if (vector.Equals(new Vector3(0f, 270f, 180f)))
			{
				this._lastPosition = Position.right;
				return BlockKind.DiagonalOnWallNorthRight;
			}
		}
		if (Vector3.Angle(this._hit.normal, new Vector3(0f, 0f, -0.1f)) == 0f)
		{
			if (vector.Equals(new Vector3(90f, 270f, 0f)))
			{
				this._lastPosition = Position.bottom;
				return BlockKind.DiagonalOnWallSouthBottom;
			}
			if (vector.Equals(new Vector3(270f, 270f, 0f)))
			{
				this._lastPosition = Position.top;
				return BlockKind.DiagonalOnWallSouthTop;
			}
			if (vector.Equals(new Vector3(0f, 270f, 0f)))
			{
				this._lastPosition = Position.left;
				return BlockKind.DiagonalOnWallSouthLeft;
			}
			if (vector.Equals(new Vector3(0f, 90f, 180f)))
			{
				this._lastPosition = Position.right;
				return BlockKind.DiagonalOnWallSouthRight;
			}
		}
		UnityEngine.Debug.LogWarning("Не удалось распознать позицию блока.");
		return currentKind;
	}

	private void SetMaterial()
	{
		Texture2D mainTexture = WorldData.Instance.BlockTextures[WorldGameObjectX.Instance.CurrentBlock];
		if (this._diagonalPreview != null)
		{
			this._diagonalPreview.GetComponent<Renderer>().material.mainTexture = mainTexture;
		}
	}

	private void IsDiagonal()
	{
		this._ray = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		int layerMask = 1 << LayerMask.NameToLayer("Terrain");
		if (Physics.Raycast(this._ray, out this._hit, CameraController.RaycastDistance, layerMask))
		{
			if (Vector3.Angle(this._hit.normal, new Vector3(-0.1f, 0f, 0f)) == 0f || Vector3.Angle(this._hit.normal, new Vector3(0.1f, 0f, 0f)) == 0f || Vector3.Angle(this._hit.normal, new Vector3(0f, 0f, -0.1f)) == 0f || Vector3.Angle(this._hit.normal, new Vector3(0f, 0f, 0.1f)) == 0f)
			{
				this._isPreview = true;
			}
			else
			{
				this._isPreview = false;
			}
		}
		else
		{
			this._isPreview = false;
		}
	}

	private Vector3 NormalizedRotation(Vector3 rotation)
	{
		Vector3 result = new Vector3((float)((int)rotation.x), (float)((int)rotation.y), (float)((int)rotation.z));
		if (result.x > 0f && result.x <= 90f)
		{
			result.x = 90f;
		}
		if (result.x > 90f && result.x <= 180f)
		{
			result.x = 180f;
		}
		if (result.x > 180f && result.x <= 270f)
		{
			result.x = 270f;
		}
		if (result.y > 0f && result.y <= 90f)
		{
			result.y = 90f;
		}
		if (result.y > 90f && result.y <= 180f)
		{
			result.y = 180f;
		}
		if (result.y > 180f && result.y <= 270f)
		{
			result.y = 270f;
		}
		if (result.z > 0f && result.z <= 90f)
		{
			result.z = 90f;
		}
		if (result.z > 90f && result.z <= 180f)
		{
			result.z = 180f;
		}
		if (result.z > 180f && result.z <= 270f)
		{
			result.z = 270f;
		}
		return result;
	}

	private GameObject _diagonalPreview;

	private BlockKind _currentKind;

	private UnityEngine.Object obj;

	private bool _isPreview = true;

	private Position _lastPosition;

	private Vector3 _side = Vector3.zero;

	private RaycastHit _hit;

	private Ray _ray;
}
