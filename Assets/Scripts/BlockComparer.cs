using System;

public static class BlockComparer
{
	public static bool IsAir(this BlockType block)
	{
		return block == BlockType.Air;
	}

	public static bool IsGlass(this BlockType block)
	{
		return WorldGameObjectX.Instance.BlockParametrs[(int)block].is_glass;
	}

	public static bool IsLand(this BlockType block)
	{
		return !block.IsAir() && !block.IsWater() && !block.IsGlass();
	}

	public static bool IsWater(this BlockType block)
	{
		return block == BlockType.Water;
	}

	public static bool IsDefault(this BlockKind block)
	{
		return block == BlockKind.Default;
	}

	public static bool IsDiagonal(this BlockKind block)
	{
		return block == BlockKind.DiagonalNorth || block == BlockKind.DiagonalSouth || block == BlockKind.DiagonalWest || block == BlockKind.DiagonalEast || (block >= BlockKind.DiagonalOnWallWestRight && block <= BlockKind.DiagonalOnWallNorthBottom);
	}

	public static bool IsCornerStair(this BlockKind block)
	{
		return block == BlockKind.CornerStairEast || block == BlockKind.CornerStairNorth || block == BlockKind.CornerStairSouth || block == BlockKind.CornerStairWest;
	}

	public static bool IsStair(this BlockKind block)
	{
		return block == BlockKind.StairNorth || block == BlockKind.StairSouth || block == BlockKind.StairWest || block == BlockKind.StairEast;
	}

	public static bool IsHalf(this BlockKind block)
	{
		return block == BlockKind.Half || block == BlockKind.HalfWallEast || block == BlockKind.HalfWallNorth || block == BlockKind.HalfWallSouth || block == BlockKind.HalfWallWest;
	}

	public static bool IsFence(this BlockKind block)
	{
		return block == BlockKind.Fence || block == BlockKind.FenceOnWallEastWest || block == BlockKind.FenceOnWallSouthNorth || (block >= BlockKind.FenceWest && block <= BlockKind.SouthFenceSouth);
	}

	public static bool IsQuarter(this BlockKind block)
	{
		return block == BlockKind.Quarter || block == BlockKind.QuarterOnWallEast || block == BlockKind.QuarterOnWallWest || block == BlockKind.QuarterOnWallNorth || block == BlockKind.QuarterOnWallSouth;
	}

	public static bool IsThird(this BlockKind block)
	{
		return block == BlockKind.Third || block == BlockKind.ThirdOnWallEast || block == BlockKind.ThirdOnWallWest || block == BlockKind.ThirdOnWallNorth || block == BlockKind.ThirdOnWallSouth;
	}

	public static bool IsCorner(this BlockKind block)
	{
		return block == BlockKind.CornerEast || block == BlockKind.CornerWest || block == BlockKind.CornerNorth || block == BlockKind.CornerSouth;
	}

	public static bool IsFlip(this BlockKind block)
	{
		return block >= BlockKind.Flip;
	}

	public static BlockKind DoFlip(this BlockKind block)
	{
		if (block >= BlockKind.Flip)
		{
			return (BlockKind)(block - BlockKind.Flip);
		}
		return block + 128;
	}
}
