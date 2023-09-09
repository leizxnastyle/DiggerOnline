using System;
using UnityEngine;

public class BlockUVCoordinates
{
	public BlockUVCoordinates(Rect topUvCoordinates, Rect sideUvCoordinates, Rect bottomUvCoordinates)
	{
		this.BlockFaceUvCoordinates[0] = topUvCoordinates;
		this.BlockFaceUvCoordinates[1] = sideUvCoordinates;
		this.BlockFaceUvCoordinates[2] = bottomUvCoordinates;
	}

	public Rect[] BlockFaceUvCoordinates
	{
		get
		{
			return this.m_BlockFaceUvCoordinates;
		}
	}

	private readonly Rect[] m_BlockFaceUvCoordinates = new Rect[3];
}
