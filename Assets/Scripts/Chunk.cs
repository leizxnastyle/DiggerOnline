using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
	public Chunk(int x, int y, int z)
	{
		this.m_X = x;
		this.m_Y = y;
		this.m_Z = z;
	}

	public void InitializeBlocks(int chunkBlockWidth, int chunkBlockHeight, int chunkBlockDepth)
	{
		this.m_BlockType = new byte[chunkBlockWidth * chunkBlockHeight * chunkBlockDepth];
		this.m_BlockKind = new byte[chunkBlockWidth * chunkBlockHeight * chunkBlockDepth];
	}

	public int X
	{
		get
		{
			return this.m_X;
		}
	}

	public int Y
	{
		get
		{
			return this.m_Y;
		}
	}

	public int Z
	{
		get
		{
			return this.m_Z;
		}
	}

	public BlockType GetBlockType(int x, int y, int z)
	{
		if (this.m_BlockType == null)
		{
			return BlockType.Air;
		}
		int chunkBlockWidth = WorldData.Instance.ChunkBlockWidth;
		int chunkBlockHeight = WorldData.Instance.ChunkBlockHeight;
		int num = x + y * chunkBlockWidth + z * chunkBlockWidth * chunkBlockHeight;
		if (num < 0 || num > this.m_BlockType.Length - 1)
		{
			return BlockType.Air;
		}
		return (BlockType)this.m_BlockType[num];
	}

	public void SetBlockType(BlockType type, int x, int y, int z)
	{
		if (this.m_BlockType == null)
		{
			return;
		}
		int chunkBlockWidth = WorldData.Instance.ChunkBlockWidth;
		int chunkBlockHeight = WorldData.Instance.ChunkBlockHeight;
		this.m_BlockType[x + y * chunkBlockWidth + z * chunkBlockWidth * chunkBlockHeight] = (byte)type;
	}

	public static int WorldChunkYOffset { get; set; } = 0;

	public void SetBlocksBuffer(byte[] buffer)
	{
		this.m_BlockType = buffer;
	}

	public byte[] GetBlocksBuffer()
	{
		return this.m_BlockType;
	}

	public override string ToString()
	{
		return string.Format("Chunk_{0},{1},{2}", this.m_X, this.m_Y, this.m_Z);
	}

	public BlockKind GetBlockKind(int x, int y, int z)
	{
		if (this.m_BlockKind == null)
		{
			return BlockKind.Default;
		}
		int chunkBlockWidth = WorldData.Instance.ChunkBlockWidth;
		int chunkBlockHeight = WorldData.Instance.ChunkBlockHeight;
		int num = x + y * chunkBlockWidth + z * chunkBlockWidth * chunkBlockHeight;
		if (num < 0 || num > this.m_BlockKind.Length - 1)
		{
			return BlockKind.Default;
		}
		return (BlockKind)this.m_BlockKind[num];
	}

	public void SetBlockKind(BlockKind type, int x, int y, int z)
	{
		if (this.m_BlockKind != null)
		{
			int chunkBlockWidth = WorldData.Instance.ChunkBlockWidth;
			int chunkBlockHeight = WorldData.Instance.ChunkBlockHeight;
			this.m_BlockKind[x + y * chunkBlockWidth + z * chunkBlockWidth * chunkBlockHeight] = (byte)type;
		}
	}

	public void SetBlocksKindBuffer(byte[] buffer)
	{
		this.m_BlockKind = buffer;
	}

	public byte[] GetBlocksKindBuffer()
	{
		return this.m_BlockKind;
	}

	public IntVect Position
	{
		get
		{
			return new IntVect(this.m_X, this.m_Y, this.m_Z);
		}
	}

	private byte[] m_BlockType;

	private int m_X;

	private int m_Y;

	private int m_Z;

	public readonly Chunk.MeshData LandData = new Chunk.MeshData();

	public readonly Chunk.MeshData WaterData = new Chunk.MeshData();

	public readonly Chunk.MeshData GlassData = new Chunk.MeshData();

	public bool WaterRegeneration;

	public bool LandRegeneration;

	public bool GlassRegeneration;

	public Transform LandChunk;

	public Transform WaterChunk;

	public Transform GlassChunk;

	public bool NeedsRegeneration;

	public bool NeedsWaterRegen;

	public bool NeedsGlassRegen;

	public readonly List<IntVect> TopSoilBlocks = new List<IntVect>();

	private byte[] m_BlockKind;

	public class MeshData
	{
		public bool IsEmpty
		{
			get
			{
				return this.m_Vertices.Count == 0;
			}
		}

		public readonly List<int> m_Indices = new List<int>();

		public readonly List<Vector2> m_Uvs = new List<Vector2>();

		public readonly List<Vector3> m_Vertices = new List<Vector3>();

		public readonly List<Color> m_Colors = new List<Color>();
	}
}
