using System;
using PreviewModels;
using UnityEngine;

public class CornerStairPreview : SimplePreviewModel
{
	public override CommonBlockKind CommonBlock
	{
		get
		{
			return CommonBlockKind.StairCorner;
		}
	}

	public override bool IsPreview
	{
		get
		{
			return this._isPreview;
		}
	}

	public override BlockKind Kind
	{
		get
		{
			return this._currentKind;
		}
	}

	public override void ShowPreview()
	{
		if (this._preview == null)
		{
			this.obj = Resources.Load("stairs_corner", typeof(GameObject));
			if (this.obj != null)
			{
				this._preview = (GameObject)UnityEngine.Object.Instantiate(this.obj, Vector3.zero, this.SetLastPosition(this._lastPosition));
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
		UnityEngine.Object.Destroy(this._preview);
		this._preview = null;
	}

	private void Update()
	{
		this.IsCorner();
		this._preview.SetActive(this._isPreview);
		if (this._preview.activeInHierarchy)
		{
			this.RotationPreview();
			this.PositionPreview();
			this._currentKind = this.GetCurrentKind();
			this.SetMaterial();
			MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.Wheel);
		}
	}

	private BlockKind GetCurrentKind()
	{
		Vector3 vector = this.NormalizedRotation(this._preview.transform.rotation.eulerAngles);
		BlockKind currentKind = this._currentKind;
		if (vector.Equals(new Vector3(270f, 0f, 0f)))
		{
			this._lastPosition = Side.East;
			return BlockKind.CornerStairEast;
		}
		if (vector.Equals(new Vector3(270f, 270f, 0f)))
		{
			this._lastPosition = Side.South;
			return BlockKind.CornerStairSouth;
		}
		if (vector.Equals(new Vector3(270f, 90f, 0f)))
		{
			this._lastPosition = Side.North;
			return BlockKind.CornerStairNorth;
		}
		if (vector.Equals(new Vector3(270f, 180f, 0f)))
		{
			this._lastPosition = Side.West;
			return BlockKind.CornerStairWest;
		}
		if (vector.Equals(new Vector3(90f, 0f, 0f)))
		{
			this._lastPosition = Side.North;
			return BlockKind.CornerStairNorth;
		}
		if (vector.Equals(new Vector3(90f, 270f, 0f)))
		{
			this._lastPosition = Side.East;
			return BlockKind.CornerStairEast;
		}
		if (vector.Equals(new Vector3(90f, 90f, 0f)))
		{
			this._lastPosition = Side.West;
			return BlockKind.CornerStairWest;
		}
		if (vector.Equals(new Vector3(90f, 180f, 0f)))
		{
			this._lastPosition = Side.South;
			return BlockKind.CornerStairSouth;
		}
		UnityEngine.Debug.LogWarning("Не удалось распознать позицию блока.");
		return currentKind;
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

	private Quaternion SetLastPosition(Side side)
	{
		Quaternion result = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		if (Vector3.Angle(this._hit.normal, new Vector3(0f, -1f, 0f)) != 0f)
		{
			switch (side)
			{
			case Side.East:
				return Quaternion.Euler(new Vector3(270f, 0f, 0f));
			case Side.West:
				return Quaternion.Euler(new Vector3(270f, 90f, 0f));
			case Side.South:
				return Quaternion.Euler(new Vector3(270f, 180f, 0f));
			case Side.North:
				return Quaternion.Euler(new Vector3(270f, 270f, 0f));
			default:
				return result;
			}
		}
		else
		{
			switch (side)
			{
			case Side.East:
				return Quaternion.Euler(new Vector3(90f, 0f, 0f));
			case Side.West:
				return Quaternion.Euler(new Vector3(90f, 90f, 0f));
			case Side.South:
				return Quaternion.Euler(new Vector3(90f, 180f, 0f));
			case Side.North:
				return Quaternion.Euler(new Vector3(90f, 270f, 0f));
			default:
				return result;
			}
		}
	}

	private void SetMaterial()
	{
		Texture2D mainTexture = WorldData.Instance.BlockTextures[WorldGameObjectX.Instance.CurrentBlock];
		if (this._preview != null)
		{
			this._preview.GetComponent<Renderer>().material.mainTexture = mainTexture;
		}
	}

	private void RotationPreview()
	{
		if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			this._preview.transform.Rotate(Vector3.forward, 90f);
		}
		else if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			this._preview.transform.Rotate(Vector3.forward, -90f);
		}
	}

	private void PositionPreview()
	{
		Vector3 vector = this._hit.point + this._hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)vector.x, (int)vector.z, (int)vector.y);
		this._preview.transform.position = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
		if (Vector3.Angle(this._hit.normal, new Vector3(0f, -1f, 0f)) == 0f)
		{
			this._preview.transform.position = new Vector3((float)intVect.X + 0.5f, (float)(intVect.Z + 1), (float)intVect.Y + 0.5f);
		}
		BlockKind blockKind = WorldData.Instance.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
		if (blockKind.IsHalf() || blockKind.IsQuarter() || blockKind.IsThird())
		{
			this._preview.transform.position = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 1.5f, (float)intVect.Y + 0.5f);
		}
		if (Vector3.Angle(this._hit.normal, this._side) != 0f)
		{
			this._preview.transform.rotation = this.SetLastPosition(this._lastPosition);
			this._side = this._hit.normal;
		}
	}

	private void IsCorner()
	{
		this._ray = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		int layerMask = 1 << LayerMask.NameToLayer("Terrain");
		if (Physics.Raycast(this._ray, out this._hit, CameraController.RaycastDistance, layerMask))
		{
			this._isPreview = true;
		}
		else
		{
			this._isPreview = false;
		}
	}

	private GameObject _preview;

	private BlockKind _currentKind;

	private UnityEngine.Object obj;

	private bool _isPreview = true;

	private Side _lastPosition;

	private RaycastHit _hit;

	private Ray _ray;

	private Vector3 _side;
}
