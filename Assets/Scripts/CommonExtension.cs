using System;
using UnityEngine;

public static class CommonExtension
{
	public static BlockKind GetKind(this CommonBlockKind c, Vector3 dir, Vector3 normal)
	{
		CommonExtension.Side side;
		if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
		{
			side = ((dir.x >= 0f) ? CommonExtension.Side.East : CommonExtension.Side.West);
		}
		else
		{
			side = ((dir.z >= 0f) ? CommonExtension.Side.South : CommonExtension.Side.North);
		}
		if (c != CommonBlockKind.Stair && c != CommonBlockKind.Diagonal && c != CommonBlockKind.StairCorner)
		{
			if (normal.x != 0f)
			{
				side = ((normal.x <= 0f) ? CommonExtension.Side.East : CommonExtension.Side.West);
			}
			else if (normal.z != 0f)
			{
				side = ((normal.z <= 0f) ? CommonExtension.Side.South : CommonExtension.Side.North);
			}
		}
		switch (c)
		{
		case CommonBlockKind.Half:
			switch (side)
			{
			case CommonExtension.Side.West:
				if (normal.y == 0f)
				{
					return BlockKind.HalfWallWest;
				}
				return BlockKind.Half;
			case CommonExtension.Side.East:
				if (normal.y == 0f)
				{
					return BlockKind.HalfWallEast;
				}
				return BlockKind.Half;
			case CommonExtension.Side.North:
				if (normal.y == 0f)
				{
					return BlockKind.HalfWallNorth;
				}
				return BlockKind.Half;
			case CommonExtension.Side.South:
				if (normal.y == 0f)
				{
					return BlockKind.HalfWallSouth;
				}
				return BlockKind.Half;
			default:
				return BlockKind.Half;
			}
			break;
		case CommonBlockKind.Fence:
			switch (side)
			{
			case CommonExtension.Side.West:
				if (normal.y == 0f)
				{
					return BlockKind.FenceOnWallEastWest;
				}
				return BlockKind.Fence;
			case CommonExtension.Side.East:
				if (normal.y == 0f)
				{
					return BlockKind.FenceOnWallEastWest;
				}
				return BlockKind.Fence;
			case CommonExtension.Side.North:
				if (normal.y == 0f)
				{
					return BlockKind.FenceOnWallSouthNorth;
				}
				return BlockKind.Fence;
			case CommonExtension.Side.South:
				if (normal.y == 0f)
				{
					return BlockKind.FenceOnWallSouthNorth;
				}
				return BlockKind.Fence;
			default:
				return BlockKind.Fence;
			}
			break;
		case CommonBlockKind.Diagonal:
			switch (side)
			{
			case CommonExtension.Side.West:
				return BlockKind.DiagonalWest;
			case CommonExtension.Side.East:
				return BlockKind.DiagonalEast;
			case CommonExtension.Side.North:
				return BlockKind.DiagonalNorth;
			}
			return BlockKind.DiagonalSouth;
		case CommonBlockKind.Stair:
			switch (side)
			{
			case CommonExtension.Side.West:
				return BlockKind.StairWest;
			case CommonExtension.Side.East:
				return BlockKind.StairEast;
			case CommonExtension.Side.North:
				return BlockKind.StairNorth;
			}
			return BlockKind.StairSouth;
		case CommonBlockKind.Quarter:
			switch (side)
			{
			case CommonExtension.Side.West:
				if (normal.y == 0f)
				{
					return BlockKind.QuarterOnWallWest;
				}
				return BlockKind.Quarter;
			case CommonExtension.Side.East:
				if (normal.y == 0f)
				{
					return BlockKind.QuarterOnWallEast;
				}
				return BlockKind.Quarter;
			case CommonExtension.Side.North:
				if (normal.y == 0f)
				{
					return BlockKind.QuarterOnWallNorth;
				}
				return BlockKind.Quarter;
			case CommonExtension.Side.South:
				if (normal.y == 0f)
				{
					return BlockKind.QuarterOnWallSouth;
				}
				return BlockKind.Quarter;
			default:
				return BlockKind.Quarter;
			}
			break;
		case CommonBlockKind.Third:
			switch (side)
			{
			case CommonExtension.Side.West:
				if (normal.y == 0f)
				{
					return BlockKind.ThirdOnWallWest;
				}
				return BlockKind.Third;
			case CommonExtension.Side.East:
				if (normal.y == 0f)
				{
					return BlockKind.ThirdOnWallEast;
				}
				return BlockKind.Third;
			case CommonExtension.Side.North:
				if (normal.y == 0f)
				{
					return BlockKind.ThirdOnWallNorth;
				}
				return BlockKind.Third;
			case CommonExtension.Side.South:
				if (normal.y == 0f)
				{
					return BlockKind.ThirdOnWallSouth;
				}
				return BlockKind.Third;
			default:
				return BlockKind.Third;
			}
			break;
		case CommonBlockKind.Corner:
			switch (side)
			{
			case CommonExtension.Side.West:
				return BlockKind.CornerWest;
			case CommonExtension.Side.East:
				return BlockKind.CornerEast;
			case CommonExtension.Side.North:
				return BlockKind.CornerNorth;
			}
			return BlockKind.CornerSouth;
		}
		return BlockKind.Default;
	}

	private enum Side
	{
		West,
		East,
		North,
		South
	}
}
