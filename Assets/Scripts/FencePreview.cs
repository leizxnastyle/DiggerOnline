using System;
using PreviewModels;
using UnityEngine;

public class FencePreview : SimplePreviewModel
{
	public override CommonBlockKind CommonBlock
	{
		get
		{
			return CommonBlockKind.Fence;
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
		if (this._fencePreview == null)
		{
			this.obj = Resources.Load("fence", typeof(GameObject));
			if (this.obj != null)
			{
				this._fencePreview = (GameObject)UnityEngine.Object.Instantiate(this.obj, Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));
			}
			this.obj = null;
			this._fencePreview.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
		}
		else
		{
			this.Update();
		}
	}

	public override void DestroyPreview()
	{
		UnityEngine.Object.Destroy(this._fencePreview);
		this._fencePreview = null;
		this._isPreview = false;
	}

	private void Update()
	{
		this.IsFence();
		this._fencePreview.SetActive(this._isPreview);
		if (this._fencePreview.activeInHierarchy)
		{
			this.PositionPreview();
			this.RotationPreview();
			this._currentKind = this.GetCurrentKind();
			this.SetMaterial();
			MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.Wheel);
		}
	}

	private void IsFence()
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

	private BlockKind GetCurrentKind()
	{
		if (this._lastSide == Side.Azimut)
		{
			switch (this.vector[this.i])
			{
			case FancePosition.East:
				return BlockKind.FenceEast;
			case FancePosition.West:
				return BlockKind.FenceWest;
			case FancePosition.South:
				return BlockKind.FenceSouth;
			case FancePosition.North:
				return BlockKind.FenceNorth;
			}
			return BlockKind.Fence;
		}
		if (this._lastSide == Side.East)
		{
			switch (this.vector[this.i])
			{
			case FancePosition.East:
				return BlockKind.EastFenceEast;
			case FancePosition.West:
				return BlockKind.EastFenceWest;
			case FancePosition.South:
				return BlockKind.EastFenceSouth;
			case FancePosition.North:
				return BlockKind.EastFenceNorth;
			}
			return BlockKind.FenceOnWallEastWest;
		}
		if (this._lastSide == Side.North)
		{
			switch (this.vector[this.i])
			{
			case FancePosition.East:
				return BlockKind.NorthFenceEast;
			case FancePosition.West:
				return BlockKind.NorthFenceWest;
			case FancePosition.South:
				return BlockKind.NorthFenceSouth;
			case FancePosition.North:
				return BlockKind.NorthFenceNorth;
			}
			return BlockKind.FenceOnWallSouthNorth;
		}
		if (this._lastSide == Side.South)
		{
			switch (this.vector[this.i])
			{
			case FancePosition.East:
				return BlockKind.SouthFenceEast;
			case FancePosition.West:
				return BlockKind.SouthFenceWest;
			case FancePosition.South:
				return BlockKind.SouthFenceSouth;
			case FancePosition.North:
				return BlockKind.SouthFenceNorth;
			}
			return BlockKind.FenceOnWallSouthNorth;
		}
		if (this._lastSide == Side.West)
		{
			switch (this.vector[this.i])
			{
			case FancePosition.East:
				return BlockKind.WestFenceEast;
			case FancePosition.West:
				return BlockKind.WestFenceWest;
			case FancePosition.South:
				return BlockKind.WestFenceSouth;
			case FancePosition.North:
				return BlockKind.WestFenceNorth;
			}
			return BlockKind.FenceOnWallEastWest;
		}
		return BlockKind.Default;
	}

	private void SetMaterial()
	{
		Texture2D mainTexture = WorldData.Instance.BlockTextures[WorldGameObjectX.Instance.CurrentBlock];
		if (this._fencePreview != null)
		{
			this._fencePreview.GetComponent<Renderer>().material.mainTexture = mainTexture;
		}
	}

	private void PositionPreview()
	{
		if (Vector3.Angle(this._hit.normal, this._side) != 0f)
		{
			this._fencePreview.transform.rotation = Quaternion.identity;
			if (Vector3.Angle(this._hit.normal, new Vector3(0.1f, 0f, 0f)) == 0f)
			{
				this._lastSide = Side.East;
				this._fencePreview.transform.rotation = this.SetLastPosition(Side.East);
			}
			if (Vector3.Angle(this._hit.normal, new Vector3(-0.1f, 0f, 0f)) == 0f)
			{
				this._lastSide = Side.West;
				this._fencePreview.transform.rotation = this.SetLastPosition(Side.West);
			}
			if (Vector3.Angle(this._hit.normal, new Vector3(0f, 0f, 0.1f)) == 0f)
			{
				this._lastSide = Side.North;
				this._fencePreview.transform.rotation = this.SetLastPosition(Side.North);
			}
			if (Vector3.Angle(this._hit.normal, new Vector3(0f, 0f, -0.1f)) == 0f)
			{
				this._lastSide = Side.South;
				this._fencePreview.transform.rotation = this.SetLastPosition(Side.South);
			}
			if (Vector3.Angle(this._hit.normal, new Vector3(0f, 0.1f, 0f)) == 0f)
			{
				this._lastSide = Side.Azimut;
				this._fencePreview.transform.rotation = this.SetLastPosition(Side.Azimut);
			}
			if (Vector3.Angle(this._hit.normal, new Vector3(0f, -0.1f, 0f)) == 0f)
			{
				this._lastSide = Side.Azimut;
				this._fencePreview.transform.rotation = this.SetLastPosition(Side.Azimut);
			}
			this._side = this._hit.normal;
		}
	}

	private Quaternion SetLastPosition(Side side)
	{
		switch (side)
		{
		case Side.East:
			return Quaternion.Euler(0f, 90f, 0f);
		case Side.West:
			return Quaternion.Euler(0f, 90f, 0f);
		case Side.South:
			return Quaternion.Euler(0f, 0f, 0f);
		case Side.North:
			return Quaternion.Euler(0f, 0f, 0f);
		}
		return Quaternion.Euler(-90f, 0f, 0f);
	}

	private void RotationPreview()
	{
		this._fencePreview.transform.position = this.GetFenceRotation(UnityEngine.Input.GetAxis("Mouse ScrollWheel"));
	}

	private Vector3 GetFenceRotation(float wheel)
	{
		Vector3 vector = this._hit.point + this._hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)vector.x, (int)vector.z, (int)vector.y);
		if (wheel > 0f)
		{
			this.i++;
			if (this.i > this.vector.Length - 1)
			{
				this.i = 0;
			}
		}
		else if (wheel < 0f)
		{
			this.i--;
			if (this.i < 0)
			{
				this.i = this.vector.Length - 1;
			}
		}
		if (this._lastSide == Side.Azimut)
		{
			switch (this.vector[this.i])
			{
			case FancePosition.East:
				return new Vector3((float)intVect.X + 0.83f, (float)intVect.Z, (float)intVect.Y + 0.17f);
			case FancePosition.West:
				return new Vector3((float)intVect.X + 0.17f, (float)intVect.Z, (float)intVect.Y + 0.17f);
			case FancePosition.South:
				return new Vector3((float)intVect.X + 0.83f, (float)intVect.Z, (float)intVect.Y + 0.83f);
			case FancePosition.North:
				return new Vector3((float)intVect.X + 0.17f, (float)intVect.Z, (float)intVect.Y + 0.83f);
			}
			return new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
		}
		if (this._lastSide == Side.East)
		{
			switch (this.vector[this.i])
			{
			case FancePosition.East:
				return new Vector3((float)intVect.X, (float)intVect.Z + 0.17f, (float)intVect.Y + 0.17f);
			case FancePosition.West:
				return new Vector3((float)intVect.X, (float)intVect.Z + 0.17f, (float)intVect.Y + 0.83f);
			case FancePosition.South:
				return new Vector3((float)intVect.X, (float)intVect.Z + 0.83f, (float)intVect.Y + 0.17f);
			case FancePosition.North:
				return new Vector3((float)intVect.X, (float)intVect.Z + 0.83f, (float)intVect.Y + 0.83f);
			}
			return new Vector3((float)intVect.X, (float)intVect.Z + 0.5f, (float)intVect.Y + 0.5f);
		}
		if (this._lastSide == Side.North)
		{
			switch (this.vector[this.i])
			{
			case FancePosition.East:
				return new Vector3((float)intVect.X + 0.83f, (float)intVect.Z + 0.17f, (float)intVect.Y);
			case FancePosition.West:
				return new Vector3((float)intVect.X + 0.17f, (float)intVect.Z + 0.17f, (float)intVect.Y);
			case FancePosition.South:
				return new Vector3((float)intVect.X + 0.83f, (float)intVect.Z + 0.83f, (float)intVect.Y);
			case FancePosition.North:
				return new Vector3((float)intVect.X + 0.17f, (float)intVect.Z + 0.83f, (float)intVect.Y);
			}
			return new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 0.5f, (float)intVect.Y);
		}
		if (this._lastSide == Side.South)
		{
			switch (this.vector[this.i])
			{
			case FancePosition.East:
				return new Vector3((float)intVect.X + 0.83f, (float)intVect.Z + 0.17f, (float)intVect.Y);
			case FancePosition.West:
				return new Vector3((float)intVect.X + 0.17f, (float)intVect.Z + 0.17f, (float)intVect.Y);
			case FancePosition.South:
				return new Vector3((float)intVect.X + 0.83f, (float)intVect.Z + 0.83f, (float)intVect.Y);
			case FancePosition.North:
				return new Vector3((float)intVect.X + 0.17f, (float)intVect.Z + 0.83f, (float)intVect.Y);
			}
			return new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 0.5f, (float)intVect.Y);
		}
		if (this._lastSide == Side.West)
		{
			switch (this.vector[this.i])
			{
			case FancePosition.East:
				return new Vector3((float)intVect.X, (float)intVect.Z + 0.17f, (float)intVect.Y + 0.17f);
			case FancePosition.West:
				return new Vector3((float)intVect.X, (float)intVect.Z + 0.17f, (float)intVect.Y + 0.83f);
			case FancePosition.South:
				return new Vector3((float)intVect.X, (float)intVect.Z + 0.83f, (float)intVect.Y + 0.17f);
			case FancePosition.North:
				return new Vector3((float)intVect.X, (float)intVect.Z + 0.83f, (float)intVect.Y + 0.83f);
			}
			return new Vector3((float)intVect.X, (float)intVect.Z + 0.5f, (float)intVect.Y + 0.5f);
		}
		return new Vector3((float)intVect.X, (float)intVect.Z, (float)intVect.Y);
	}

	private GameObject _fencePreview;

	private BlockKind _currentKind;

	private UnityEngine.Object obj;

	private bool _isPreview = true;

	private Vector3 _side = Vector3.zero;

	private RaycastHit _hit;

	private Ray _ray;

	private FancePosition[] vector = new FancePosition[]
	{
		FancePosition.East,
		FancePosition.North,
		FancePosition.South,
		FancePosition.West,
		FancePosition.Center
	};

	private Side _lastSide;

	private int i;
}
