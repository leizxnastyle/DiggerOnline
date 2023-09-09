using System;
using UnityEngine;

public class CustomBlockEntity : MonoBehaviour
{
	public static GameObject CustomBlockPreview { get; set; }

	private static void InitializationBlockPreview(CommonBlockKind kind)
	{
		Vector3 position = WorldGameObjectX.Instance.MainPlayer.transform.position;
		Quaternion identity = Quaternion.identity;
		UnityEngine.Object @object;
		if (kind != CommonBlockKind.Diagonal)
		{
			if (kind != CommonBlockKind.Corner)
			{
				UnityEngine.Debug.LogWarning("Форма куба не поддерживается!");
				return;
			}
			@object = Resources.Load("corner", typeof(GameObject));
			CustomBlockEntity._currentBlock = CommonBlockKind.Corner;
		}
		else
		{
			@object = Resources.Load("diagonal", typeof(GameObject));
			CustomBlockEntity._currentBlock = CommonBlockKind.Diagonal;
		}
		if (@object != null)
		{
			CustomBlockEntity.CustomBlockPreview = (GameObject)UnityEngine.Object.Instantiate(@object, position, CustomBlockEntity.SetLastRotation(CustomBlockEntity._lastCornerRotation, CommonBlockKind.Corner));
		}
	}

	public static void DestroyBlockPreview()
	{
		if (CustomBlockEntity.CustomBlockPreview != null)
		{
			UnityEngine.Object.DestroyObject(CustomBlockEntity.CustomBlockPreview);
			CustomBlockEntity.CustomBlockPreview = null;
			CustomBlockEntity._side = Vector3.zero;
			CustomBlockEntity._hit.normal = Vector3.zero;
			CustomBlockEntity._currentKind = BlockKind.Default;
			CustomBlockEntity._currentBlock = CommonBlockKind.Default;
		}
	}

	public static BlockKind GetKind()
	{
		return CustomBlockEntity._currentKind;
	}

	private static void RotationBlockPreview()
	{
		if (CustomBlockEntity.CustomBlockPreview == null)
		{
			return;
		}
		if (CustomBlockEntity._currentBlock == CommonBlockKind.Diagonal)
		{
			if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				CustomBlockEntity.CustomBlockPreview.transform.Rotate(Vector3.right, 90f);
			}
			else if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				CustomBlockEntity.CustomBlockPreview.transform.Rotate(Vector3.right, -90f);
			}
		}
		else if (CustomBlockEntity._currentBlock == CommonBlockKind.Corner)
		{
			if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				CustomBlockEntity.CustomBlockPreview.transform.Rotate(Vector3.forward, 90f);
			}
			else if (UnityEngine.Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				CustomBlockEntity.CustomBlockPreview.transform.Rotate(Vector3.forward, -90f);
			}
		}
	}

	private static void RepositionPreview()
	{
		Vector3 vector = CustomBlockEntity._hit.point + CustomBlockEntity._hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)vector.x, (int)vector.z, (int)vector.y);
		if (CustomBlockEntity._currentBlock == CommonBlockKind.Diagonal)
		{
			CustomBlockEntity.CustomBlockPreview.transform.position = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 0.5f, (float)intVect.Y + 0.5f);
		}
		else if (CustomBlockEntity._currentBlock == CommonBlockKind.Corner)
		{
			CustomBlockEntity.CustomBlockPreview.transform.position = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z, (float)intVect.Y + 0.5f);
		}
		if (CustomBlockEntity._currentBlock == CommonBlockKind.Corner && Vector3.Angle(CustomBlockEntity._hit.normal, new Vector3(0f, -0.1f, 0f)) == 0f)
		{
			CustomBlockEntity.CustomBlockPreview.transform.position = new Vector3((float)intVect.X + 0.5f, (float)(intVect.Z + 1), (float)intVect.Y + 0.5f);
		}
		BlockKind blockKind = WorldData.Instance.GetBlockKind(intVect.X, intVect.Y, intVect.Z);
		if (blockKind.IsHalf() || blockKind.IsQuarter() || blockKind.IsThird())
		{
			CustomBlockEntity.CustomBlockPreview.transform.position = new Vector3((float)intVect.X + 0.5f, (float)intVect.Z + 1.5f, (float)intVect.Y + 0.5f);
		}
		if (CustomBlockEntity._currentBlock == CommonBlockKind.Diagonal && Vector3.Angle(CustomBlockEntity._hit.normal, CustomBlockEntity._side) != 0f)
		{
			if (Vector3.Angle(CustomBlockEntity._hit.normal, new Vector3(0.1f, 0f, 0f)) == 0f)
			{
				CustomBlockEntity.CustomBlockPreview.transform.rotation = CustomBlockEntity.SetLastRotation(CustomBlockEntity.SideRot.East, CommonBlockKind.Diagonal);
			}
			if (Vector3.Angle(CustomBlockEntity._hit.normal, new Vector3(-0.1f, 0f, 0f)) == 0f)
			{
				CustomBlockEntity.CustomBlockPreview.transform.rotation = CustomBlockEntity.SetLastRotation(CustomBlockEntity.SideRot.West, CommonBlockKind.Diagonal);
			}
			if (Vector3.Angle(CustomBlockEntity._hit.normal, new Vector3(0f, 0f, 0.1f)) == 0f)
			{
				CustomBlockEntity.CustomBlockPreview.transform.rotation = CustomBlockEntity.SetLastRotation(CustomBlockEntity.SideRot.North, CommonBlockKind.Diagonal);
			}
			if (Vector3.Angle(CustomBlockEntity._hit.normal, new Vector3(0f, 0f, -0.1f)) == 0f)
			{
				CustomBlockEntity.CustomBlockPreview.transform.rotation = CustomBlockEntity.SetLastRotation(CustomBlockEntity.SideRot.South, CommonBlockKind.Diagonal);
			}
			CustomBlockEntity._side = CustomBlockEntity._hit.normal;
		}
	}

	private static Vector3 NormalizedRotation(Vector3 rotation)
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

	private static BlockKind GetCurrentKindOnWall()
	{
		Vector3 vector = CustomBlockEntity.NormalizedRotation(CustomBlockEntity.CustomBlockPreview.transform.rotation.eulerAngles);
		BlockKind currentKind = CustomBlockEntity._currentKind;
		if (CustomBlockEntity._currentBlock == CommonBlockKind.Diagonal)
		{
			if (Vector3.Angle(CustomBlockEntity._hit.normal, new Vector3(-0.1f, 0f, 0f)) == 0f)
			{
				if (vector.Equals(new Vector3(90f, 0f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.bottom;
					return BlockKind.DiagonalOnWallWestBottom;
				}
				if (vector.Equals(new Vector3(270f, 0f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.top;
					return BlockKind.DiagonalOnWallWestTop;
				}
				if (vector.Equals(new Vector3(0f, 0f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.left;
					return BlockKind.DiagonalOnWallWestLeft;
				}
				if (vector.Equals(new Vector3(0f, 180f, 180f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.right;
					return BlockKind.DiagonalOnWallWestRight;
				}
			}
			if (Vector3.Angle(CustomBlockEntity._hit.normal, new Vector3(0.1f, 0f, 0f)) == 0f)
			{
				if (vector.Equals(new Vector3(90f, 180f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.bottom;
					return BlockKind.DiagonalOnWallEastBottom;
				}
				if (vector.Equals(new Vector3(270f, 180f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.top;
					return BlockKind.DiagonalOnWallEastTop;
				}
				if (vector.Equals(new Vector3(0f, 180f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.left;
					return BlockKind.DiagonalOnWallEastLeft;
				}
				if (vector.Equals(new Vector3(0f, 0f, 180f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.right;
					return BlockKind.DiagonalOnWallEastRight;
				}
			}
			if (Vector3.Angle(CustomBlockEntity._hit.normal, new Vector3(0f, 0f, 0.1f)) == 0f)
			{
				if (vector.Equals(new Vector3(90f, 90f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.bottom;
					return BlockKind.DiagonalOnWallNorthBottom;
				}
				if (vector.Equals(new Vector3(270f, 90f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.top;
					return BlockKind.DiagonalOnWallNorthTop;
				}
				if (vector.Equals(new Vector3(0f, 90f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.left;
					return BlockKind.DiagonalOnWallNorthLeft;
				}
				if (vector.Equals(new Vector3(0f, 270f, 180f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.right;
					return BlockKind.DiagonalOnWallNorthRight;
				}
			}
			if (Vector3.Angle(CustomBlockEntity._hit.normal, new Vector3(0f, 0f, -0.1f)) == 0f)
			{
				if (vector.Equals(new Vector3(90f, 270f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.bottom;
					return BlockKind.DiagonalOnWallSouthBottom;
				}
				if (vector.Equals(new Vector3(270f, 270f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.top;
					return BlockKind.DiagonalOnWallSouthTop;
				}
				if (vector.Equals(new Vector3(0f, 270f, 0f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.left;
					return BlockKind.DiagonalOnWallSouthLeft;
				}
				if (vector.Equals(new Vector3(0f, 90f, 180f)))
				{
					CustomBlockEntity._lastRotation = CustomBlockEntity.Rotation.right;
					return BlockKind.DiagonalOnWallSouthRight;
				}
			}
		}
		else if (CustomBlockEntity._currentBlock == CommonBlockKind.Corner)
		{
			if (vector.Equals(new Vector3(270f, 0f, 0f)))
			{
				CustomBlockEntity._lastCornerRotation = CustomBlockEntity.SideRot.East;
				return BlockKind.CornerEast;
			}
			if (vector.Equals(new Vector3(270f, 270f, 0f)))
			{
				CustomBlockEntity._lastCornerRotation = CustomBlockEntity.SideRot.North;
				return BlockKind.CornerNorth;
			}
			if (vector.Equals(new Vector3(270f, 90f, 0f)))
			{
				CustomBlockEntity._lastCornerRotation = CustomBlockEntity.SideRot.West;
				return BlockKind.CornerWest;
			}
			if (vector.Equals(new Vector3(270f, 180f, 0f)))
			{
				CustomBlockEntity._lastCornerRotation = CustomBlockEntity.SideRot.South;
				return BlockKind.CornerSouth;
			}
			if (vector.Equals(new Vector3(90f, 0f, 0f)))
			{
				CustomBlockEntity._lastCornerRotation = CustomBlockEntity.SideRot.North;
				return BlockKind.CornerNorth;
			}
			if (vector.Equals(new Vector3(90f, 270f, 0f)))
			{
				CustomBlockEntity._lastCornerRotation = CustomBlockEntity.SideRot.South;
				return BlockKind.CornerSouth;
			}
			if (vector.Equals(new Vector3(90f, 90f, 0f)))
			{
				CustomBlockEntity._lastCornerRotation = CustomBlockEntity.SideRot.East;
				return BlockKind.CornerEast;
			}
			if (vector.Equals(new Vector3(90f, 180f, 0f)))
			{
				CustomBlockEntity._lastCornerRotation = CustomBlockEntity.SideRot.West;
				return BlockKind.CornerWest;
			}
		}
		UnityEngine.Debug.LogWarning("Не удалось распознать позицию блока.");
		return currentKind;
	}

	private static bool IsDiagonal(Vector3 normal)
	{
		return Vector3.Angle(normal, new Vector3(-0.1f, 0f, 0f)) != 0f && Vector3.Angle(normal, new Vector3(0.1f, 0f, 0f)) != 0f && Vector3.Angle(normal, new Vector3(0f, 0f, -0.1f)) != 0f && Vector3.Angle(normal, new Vector3(0f, 0f, 0.1f)) != 0f;
	}

	private static bool IsCorner(Vector3 normal)
	{
		return normal.y != 1f;
	}

	public static bool IsAutoCorner()
	{
		Vector3 vector = CustomBlockEntity._hit.point + CustomBlockEntity._hit.normal * 0.01f;
		IntVect intVect = new IntVect((int)vector.x, (int)vector.z, (int)vector.y);
		CustomBlockEntity.ray = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		int layerMask = 1 << LayerMask.NameToLayer("Terrain");
		if (Physics.Raycast(CustomBlockEntity.ray, out CustomBlockEntity._hit, CameraController.RaycastDistance, layerMask))
		{
			if (Mathf.Abs(CustomBlockEntity.ray.direction.x) > Mathf.Abs(CustomBlockEntity.ray.direction.z))
			{
				CustomBlockEntity.s = ((CustomBlockEntity.ray.direction.x >= 0f) ? CustomBlockEntity.SideRot.East : CustomBlockEntity.SideRot.West);
			}
			else
			{
				CustomBlockEntity.s = ((CustomBlockEntity.ray.direction.z >= 0f) ? CustomBlockEntity.SideRot.South : CustomBlockEntity.SideRot.North);
			}
			CustomBlockEntity.near = WorldData.Instance.GetBlockKind(intVect.X + 1, intVect.Y, intVect.Z);
			if ((CustomBlockEntity.near == BlockKind.DiagonalNorth || CustomBlockEntity.near == BlockKind.DiagonalSouth) && CustomBlockEntity.s == CustomBlockEntity.SideRot.East)
			{
				return true;
			}
			CustomBlockEntity.near = WorldData.Instance.GetBlockKind(intVect.X - 1, intVect.Y, intVect.Z);
			if ((CustomBlockEntity.near == BlockKind.DiagonalNorth || CustomBlockEntity.near == BlockKind.DiagonalSouth) && CustomBlockEntity.s == CustomBlockEntity.SideRot.West)
			{
				return true;
			}
			CustomBlockEntity.near = WorldData.Instance.GetBlockKind(intVect.X, intVect.Y + 1, intVect.Z);
			if ((CustomBlockEntity.near == BlockKind.DiagonalWest || CustomBlockEntity.near == BlockKind.DiagonalEast) && CustomBlockEntity.s == CustomBlockEntity.SideRot.South)
			{
				return true;
			}
			CustomBlockEntity.near = WorldData.Instance.GetBlockKind(intVect.X, intVect.Y - 1, intVect.Z);
			if ((CustomBlockEntity.near == BlockKind.DiagonalWest || CustomBlockEntity.near == BlockKind.DiagonalEast) && CustomBlockEntity.s == CustomBlockEntity.SideRot.North)
			{
				return true;
			}
		}
		return false;
	}

	public static BlockKind GetCorner()
	{
		if (CustomBlockEntity.s == CustomBlockEntity.SideRot.East)
		{
			if (CustomBlockEntity.near == BlockKind.DiagonalNorth)
			{
				return BlockKind.CornerSouth;
			}
			if (CustomBlockEntity.near == BlockKind.DiagonalSouth)
			{
				return BlockKind.CornerWest;
			}
		}
		if (CustomBlockEntity.s == CustomBlockEntity.SideRot.West)
		{
			if (CustomBlockEntity.near == BlockKind.DiagonalNorth)
			{
				return BlockKind.CornerNorth;
			}
			if (CustomBlockEntity.near == BlockKind.DiagonalSouth)
			{
				return BlockKind.CornerEast;
			}
		}
		if (CustomBlockEntity.s == CustomBlockEntity.SideRot.South)
		{
			if (CustomBlockEntity.near == BlockKind.DiagonalEast)
			{
				return BlockKind.CornerWest;
			}
			if (CustomBlockEntity.near == BlockKind.DiagonalWest)
			{
				return BlockKind.CornerEast;
			}
		}
		if (CustomBlockEntity.s == CustomBlockEntity.SideRot.North)
		{
			if (CustomBlockEntity.near == BlockKind.DiagonalEast)
			{
				return BlockKind.CornerSouth;
			}
			if (CustomBlockEntity.near == BlockKind.DiagonalWest)
			{
				return BlockKind.CornerNorth;
			}
		}
		UnityEngine.Debug.LogWarning("Get corner fail...");
		return BlockKind.Default;
	}

	public static void Update(CommonBlockKind kind)
	{
		CustomBlockEntity.ray = CameraController.RaycastCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		int layerMask = 1 << LayerMask.NameToLayer("Terrain");
		if (Physics.Raycast(CustomBlockEntity.ray, out CustomBlockEntity._hit, CameraController.RaycastDistance, layerMask))
		{
			if (kind == CommonBlockKind.Diagonal)
			{
				if (!CustomBlockEntity.IsDiagonal(CustomBlockEntity._hit.normal) && CustomBlockEntity.CustomBlockPreview == null)
				{
					CustomBlockEntity.InitializationBlockPreview(kind);
				}
				else if (CustomBlockEntity.IsDiagonal(CustomBlockEntity._hit.normal))
				{
					CustomBlockEntity.DestroyBlockPreview();
				}
			}
			else if (kind == CommonBlockKind.Corner && CustomBlockEntity.CustomBlockPreview == null)
			{
				CustomBlockEntity.InitializationBlockPreview(kind);
			}
		}
		else
		{
			CustomBlockEntity.DestroyBlockPreview();
		}
		if (CustomBlockEntity.CustomBlockPreview != null)
		{
			CustomBlockEntity.RotationBlockPreview();
			CustomBlockEntity.RepositionPreview();
			CustomBlockEntity._currentKind = CustomBlockEntity.GetCurrentKindOnWall();
			CustomBlockEntity.SetMaterial();
			MainMenu.Instance.SetCrosshairInfo(null, MainMenu.CrosshairAction.Wheel);
		}
	}

	public static void SetMaterial()
	{
		Texture2D mainTexture = WorldData.Instance.BlockTextures[WorldGameObjectX.Instance.CurrentBlock];
		if (CustomBlockEntity.CustomBlockPreview != null)
		{
			CustomBlockEntity.CustomBlockPreview.GetComponent<Renderer>().material.mainTexture = mainTexture;
		}
	}

	private static Quaternion SetLastRotation(CustomBlockEntity.SideRot side, CommonBlockKind kind = CommonBlockKind.Diagonal)
	{
		Quaternion result = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		if (kind == CommonBlockKind.Diagonal)
		{
			switch (side)
			{
			case CustomBlockEntity.SideRot.East:
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.bottom)
				{
					return Quaternion.Euler(new Vector3(90f, 180f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.top)
				{
					return Quaternion.Euler(new Vector3(270f, 180f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.left)
				{
					return Quaternion.Euler(new Vector3(0f, 180f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.right)
				{
					return Quaternion.Euler(new Vector3(0f, 0f, 180f));
				}
				return result;
			case CustomBlockEntity.SideRot.West:
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.bottom)
				{
					return Quaternion.Euler(new Vector3(90f, 0f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.top)
				{
					return Quaternion.Euler(new Vector3(270f, 0f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.left)
				{
					return Quaternion.Euler(new Vector3(0f, 0f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.right)
				{
					return Quaternion.Euler(new Vector3(0f, 180f, 180f));
				}
				return result;
			case CustomBlockEntity.SideRot.South:
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.bottom)
				{
					return Quaternion.Euler(new Vector3(90f, 270f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.top)
				{
					return Quaternion.Euler(new Vector3(270f, 270f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.left)
				{
					return Quaternion.Euler(new Vector3(0f, 270f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.right)
				{
					return Quaternion.Euler(new Vector3(0f, 90f, 180f));
				}
				return result;
			case CustomBlockEntity.SideRot.North:
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.bottom)
				{
					return Quaternion.Euler(new Vector3(90f, 90f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.top)
				{
					return Quaternion.Euler(new Vector3(270f, 90f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.left)
				{
					return Quaternion.Euler(new Vector3(0f, 90f, 0f));
				}
				if (CustomBlockEntity._lastRotation == CustomBlockEntity.Rotation.right)
				{
					return Quaternion.Euler(new Vector3(0f, 270f, 180f));
				}
				return result;
			default:
				return result;
			}
		}
		else
		{
			if (kind != CommonBlockKind.Corner)
			{
				return result;
			}
			if (Vector3.Angle(CustomBlockEntity._hit.normal, new Vector3(0f, -0.1f, 0f)) != 0f)
			{
				switch (side)
				{
				case CustomBlockEntity.SideRot.East:
					return Quaternion.Euler(new Vector3(270f, 0f, 0f));
				case CustomBlockEntity.SideRot.West:
					return Quaternion.Euler(new Vector3(270f, 90f, 0f));
				case CustomBlockEntity.SideRot.South:
					return Quaternion.Euler(new Vector3(270f, 180f, 0f));
				case CustomBlockEntity.SideRot.North:
					return Quaternion.Euler(new Vector3(270f, 270f, 0f));
				default:
					return result;
				}
			}
			else
			{
				switch (side)
				{
				case CustomBlockEntity.SideRot.East:
					return Quaternion.Euler(new Vector3(90f, 0f, 0f));
				case CustomBlockEntity.SideRot.West:
					return Quaternion.Euler(new Vector3(90f, 90f, 0f));
				case CustomBlockEntity.SideRot.South:
					return Quaternion.Euler(new Vector3(90f, 180f, 0f));
				case CustomBlockEntity.SideRot.North:
					return Quaternion.Euler(new Vector3(90f, 270f, 0f));
				default:
					return result;
				}
			}
		}
	}

	private static CommonBlockKind _currentBlock;

	private static BlockKind _currentKind;

	private static Vector3 _side = Vector3.zero;

	private static CustomBlockEntity.Rotation _lastRotation = CustomBlockEntity.Rotation.bottom;

	private static CustomBlockEntity.SideRot _lastCornerRotation = CustomBlockEntity.SideRot.East;

	private static RaycastHit _hit;

	private static Ray ray;

	private static BlockKind near;

	private static CustomBlockEntity.SideRot s;

	private enum Rotation : byte
	{
		bottom,
		top,
		right,
		left
	}

	private enum SideRot : byte
	{
		East,
		West,
		South,
		North
	}
}
