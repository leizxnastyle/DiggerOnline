using System;

public enum BlockKind : byte
{
	Default,
	Half,
	Fence,
	DiagonalWest,
	DiagonalEast,
	DiagonalSouth,
	DiagonalNorth,
	StairNorth,
	StairSouth,
	StairWest,
	StairEast,
	HalfWallNorth,
	HalfWallSouth,
	HalfWallEast,
	HalfWallWest,
	Quarter = 16,
	QuarterOnWallEast,
	QuarterOnWallNorth,
	QuarterOnWallSouth,
	QuarterOnWallWest,
	FenceOnWallSouthNorth,
	FenceOnWallEastWest,
	Third = 25,
	ThirdOnWallEast,
	ThirdOnWallWest,
	ThirdOnWallSouth,
	ThirdOnWallNorth,
	DiagonalOnWallWestRight,
	DiagonalOnWallWestLeft,
	DiagonalOnWallWestTop,
	DiagonalOnWallWestBottom,
	DiagonalOnWallEastRight,
	DiagonalOnWallEastLeft,
	DiagonalOnWallEastTop,
	DiagonalOnWallEastBottom,
	DiagonalOnWallSouthRight,
	DiagonalOnWallSouthLeft,
	DiagonalOnWallSouthTop,
	DiagonalOnWallSouthBottom,
	DiagonalOnWallNorthRight,
	DiagonalOnWallNorthLeft,
	DiagonalOnWallNorthTop,
	DiagonalOnWallNorthBottom,
	StairOnWallNorthRight,
	StairOnWallNorthLeft,
	StairOnWallNorthTop,
	StairOnWallNorthBottom,
	StairOnWallSouthRight,
	StairOnWallSouthLeft,
	StairOnWallSouthTop,
	StairOnWallSouthBottom,
	StairOnWallWestRight,
	StairOnWallWestLeft,
	StairOnWallWestTop,
	StairOnWallWestBottom,
	StairOnWallEastRight,
	StairOnWallEastLeft,
	StairOnWallEastTop,
	StairOnWallEastBottom,
	CornerEast,
	CornerWest,
	CornerNorth,
	CornerSouth,
	FenceWest,
	FenceEast,
	FenceNorth,
	FenceSouth,
	EastFenceWest,
	EastFenceEast,
	EastFenceNorth,
	EastFenceSouth,
	WestFenceWest,
	WestFenceEast,
	WestFenceNorth,
	WestFenceSouth,
	NorthFenceWest,
	NorthFenceEast,
	NorthFenceNorth,
	NorthFenceSouth,
	SouthFenceWest,
	SouthFenceEast,
	SouthFenceNorth,
	SouthFenceSouth,
	CornerStairEast,
	CornerStairWest,
	CornerStairSouth,
	CornerStairNorth,
	Flip = 128
}
