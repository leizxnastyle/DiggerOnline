using System;

public class DiagonalAssociation
{
	// Note: this type is marked as 'beforefieldinit'.
	static DiagonalAssociation()
	{
		DiagonalAssociation diagonalAssociation = new DiagonalAssociation();
		diagonalAssociation.I = new int[]
		{
			1,
			2,
			3
		};
		diagonalAssociation.J = new int[]
		{
			1,
			2,
			2,
			3
		};
		DiagonalAssociation diagonalAssociation2 = diagonalAssociation;
		int[] array = new int[4];
		array[1] = 3;
		array[2] = 1;
		diagonalAssociation2.FlipJ = array;
		DiagonalAssociation.West = diagonalAssociation;
		DiagonalAssociation.East = new DiagonalAssociation
		{
			I = new int[]
			{
				0,
				2,
				3
			},
			J = new int[]
			{
				1,
				2,
				0,
				1
			},
			FlipJ = new int[]
			{
				3,
				0,
				3,
				2
			}
		};
		DiagonalAssociation.South = new DiagonalAssociation
		{
			I = new int[]
			{
				2,
				0,
				1
			},
			J = new int[]
			{
				2,
				1,
				0,
				3
			},
			FlipJ = new int[]
			{
				0,
				3,
				0,
				3
			}
		};
		DiagonalAssociation.North = new DiagonalAssociation
		{
			I = new int[]
			{
				3,
				0,
				1
			},
			J = new int[]
			{
				1,
				2,
				1,
				2
			},
			FlipJ = new int[]
			{
				3,
				0,
				1,
				2
			}
		};
		DiagonalAssociation.WestRight = new DiagonalAssociation
		{
			I = new int[]
			{
				0,
				4,
				5
			},
			J = new int[]
			{
				2,
				3,
				0,
				1
			},
			FlipJ = new int[4]
		};
		DiagonalAssociation.EastRight = new DiagonalAssociation
		{
			I = new int[]
			{
				1,
				4,
				5
			},
			J = new int[]
			{
				2,
				3,
				0,
				1
			},
			FlipJ = new int[4]
		};
		DiagonalAssociation.SouthRight = new DiagonalAssociation
		{
			I = new int[]
			{
				2,
				4,
				5
			},
			J = new int[]
			{
				2,
				3,
				2,
				3
			},
			FlipJ = new int[4]
		};
		DiagonalAssociation.NorthRight = new DiagonalAssociation
		{
			I = new int[]
			{
				2,
				0,
				1
			},
			J = new int[]
			{
				2,
				1,
				1,
				2
			},
			FlipJ = new int[4]
		};
		DiagonalAssociation.WestLeft = new DiagonalAssociation
		{
			I = new int[]
			{
				0,
				4,
				5
			},
			J = new int[]
			{
				1,
				0,
				2,
				3
			},
			FlipJ = new int[4]
		};
		DiagonalAssociation.EastLeft = new DiagonalAssociation
		{
			I = new int[]
			{
				1,
				4,
				5
			},
			J = new int[]
			{
				0,
				1,
				2,
				3
			},
			FlipJ = new int[4]
		};
		DiagonalAssociation.SouthLeft = new DiagonalAssociation
		{
			I = new int[]
			{
				3,
				0,
				1
			},
			J = new int[]
			{
				1,
				2,
				0,
				3
			},
			FlipJ = new int[4]
		};
		DiagonalAssociation.NorthLeft = new DiagonalAssociation
		{
			I = new int[]
			{
				2,
				0,
				1
			},
			J = new int[]
			{
				2,
				1,
				1,
				2
			},
			FlipJ = new int[4]
		};
	}

	public static DiagonalAssociation GetAssociation(BlockKind kind)
	{
		switch (kind)
		{
		case BlockKind.DiagonalOnWallWestRight:
			return DiagonalAssociation.WestRight;
		case BlockKind.DiagonalOnWallWestLeft:
			return DiagonalAssociation.WestLeft;
		case BlockKind.DiagonalOnWallWestTop:
			return DiagonalAssociation.East;
		case BlockKind.DiagonalOnWallWestBottom:
			return DiagonalAssociation.East;
		case BlockKind.DiagonalOnWallEastRight:
			return DiagonalAssociation.EastRight;
		case BlockKind.DiagonalOnWallEastLeft:
			return DiagonalAssociation.EastLeft;
		case BlockKind.DiagonalOnWallEastTop:
			return DiagonalAssociation.West;
		case BlockKind.DiagonalOnWallEastBottom:
			return DiagonalAssociation.West;
		case BlockKind.DiagonalOnWallSouthRight:
			return DiagonalAssociation.SouthRight;
		case BlockKind.DiagonalOnWallSouthLeft:
			return DiagonalAssociation.SouthLeft;
		case BlockKind.DiagonalOnWallSouthTop:
			return DiagonalAssociation.South;
		case BlockKind.DiagonalOnWallSouthBottom:
			return DiagonalAssociation.South;
		case BlockKind.DiagonalOnWallNorthRight:
			return DiagonalAssociation.NorthRight;
		case BlockKind.DiagonalOnWallNorthLeft:
			return DiagonalAssociation.NorthLeft;
		case BlockKind.DiagonalOnWallNorthTop:
			return DiagonalAssociation.North;
		case BlockKind.DiagonalOnWallNorthBottom:
			return DiagonalAssociation.North;
		default:
			switch (kind)
			{
			case BlockKind.DiagonalWest:
				return DiagonalAssociation.West;
			case BlockKind.DiagonalEast:
				return DiagonalAssociation.East;
			case BlockKind.DiagonalSouth:
				return DiagonalAssociation.South;
			case BlockKind.DiagonalNorth:
				return DiagonalAssociation.North;
			default:
				return null;
			}
			break;
		}
	}

	public static int[] GetUVNumber(BlockKind kind)
	{
		switch (kind)
		{
		case BlockKind.DiagonalOnWallWestTop:
			break;
		case BlockKind.DiagonalOnWallWestBottom:
			return new int[]
			{
				2,
				3
			};
		default:
			switch (kind)
			{
			case BlockKind.DiagonalWest:
				goto IL_6F;
			case BlockKind.DiagonalEast:
				break;
			case BlockKind.DiagonalSouth:
				goto IL_7E;
			case BlockKind.DiagonalNorth:
				goto IL_89;
			default:
				return new int[]
				{
					0,
					1
				};
			}
			break;
		case BlockKind.DiagonalOnWallEastTop:
			goto IL_6F;
		case BlockKind.DiagonalOnWallEastBottom:
			return new int[]
			{
				3,
				2
			};
		case BlockKind.DiagonalOnWallSouthTop:
			goto IL_7E;
		case BlockKind.DiagonalOnWallSouthBottom:
		{
			int[] array = new int[2];
			array[0] = 1;
			return array;
		}
		case BlockKind.DiagonalOnWallNorthTop:
			goto IL_89;
		case BlockKind.DiagonalOnWallNorthBottom:
			return new int[]
			{
				0,
				1
			};
		}
		return new int[]
		{
			2,
			3
		};
		IL_6F:
		return new int[]
		{
			3,
			2
		};
		IL_7E:
		int[] array2 = new int[2];
		array2[0] = 1;
		return array2;
		IL_89:
		return new int[]
		{
			0,
			1
		};
	}

	public int[] I = new int[3];

	public int[] J = new int[4];

	public int[] FlipJ = new int[4];

	public static DiagonalAssociation West;

	public static DiagonalAssociation East;

	public static DiagonalAssociation South;

	public static DiagonalAssociation North;

	public static DiagonalAssociation WestRight;

	public static DiagonalAssociation EastRight;

	public static DiagonalAssociation SouthRight;

	public static DiagonalAssociation NorthRight;

	public static DiagonalAssociation WestLeft;

	public static DiagonalAssociation EastLeft;

	public static DiagonalAssociation SouthLeft;

	public static DiagonalAssociation NorthLeft;
}
