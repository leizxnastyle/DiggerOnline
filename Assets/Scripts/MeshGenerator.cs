using System;
using UnityEngine;

public class MeshGenerator : IMeshGenerator
{
	public MeshGenerator(IBatchProcessor<Chunk> batchProcessor, WorldData worldData)
	{
		this._BatchProcessor = batchProcessor;
		this._WorldData = worldData;
	}

	public void GenerateLandMeshes(PChunkList chunks)
	{
		this._BatchProcessor.Process(1, chunks, new Action<Chunk>(this.GenerateLandMesh), false);
	}

	public void GenerateLandMesh(Chunk chunk)
	{
		this.GenerateLandMeshData(chunk);
		this._WorldData.AddFinishedLandChunk(chunk);
	}

	public void GenerateWaterMeshes(PChunkList chunks)
	{
		this._BatchProcessor.Process(1, chunks, new Action<Chunk>(this.GenerateWaterMesh), false);
	}

	public void GenerateWaterMesh(Chunk chunk)
	{
		this.GenerateWaterMeshData(chunk);
		this._WorldData.AddFinishedWaterChunk(chunk);
	}

	public void GenerateGlassMeshes(PChunkList chunks)
	{
		this._BatchProcessor.Process(1, chunks, new Action<Chunk>(this.GenerateGlassMesh), false);
	}

	public void GenerateGlassMesh(Chunk chunk)
	{
		this.GenerateGlassMeshData(chunk);
		this._WorldData.AddFinishedGlassChunk(chunk);
	}

	public Chunk.MeshData[] GenerateMeshData(int x1, int x2, int y1, int y2, int z1, int z2, BlockGetter blockGetter, BlockKindGetter kindGetter)
	{
		Chunk.MeshData[] array = new Chunk.MeshData[]
		{
			new Chunk.MeshData(),
			new Chunk.MeshData(),
			new Chunk.MeshData()
		};
		int[] array2 = new int[3];
		for (int i = x1; i <= x2; i++)
		{
			for (int j = y1; j <= y2; j++)
			{
				for (int k = z1; k <= z2; k++)
				{
					array2[0] = this.CreateDataMeshForLand(i, j, k, null, array[0], i, j, k, array2[0], blockGetter);
					array2[1] = this.CreateDataMeshForWater(i, j, k, null, array[1], i, j, k, array2[1], blockGetter);
					array2[2] = this.CreateDataMeshForGlass(i, j, k, null, array[2], i, j, k, array2[2], blockGetter);
				}
			}
		}
		return array;
	}

	private void GenerateLandMeshData(Chunk chunk)
	{
		int index = 0;
		chunk.LandData.m_Vertices.Clear();
		chunk.LandData.m_Indices.Clear();
		chunk.LandData.m_Uvs.Clear();
		chunk.LandData.m_Colors.Clear();
		for (int i = 0; i < this._WorldData.ChunkBlockWidth; i++)
		{
			int blockX = chunk.X * this._WorldData.ChunkBlockWidth + i;
			for (int j = 0; j < this._WorldData.ChunkBlockHeight; j++)
			{
				int blockY = j + chunk.Y * this._WorldData.ChunkBlockHeight;
				for (int k = 0; k < this._WorldData.ChunkBlockDepth; k++)
				{
					int blockZ = k + chunk.Z * this._WorldData.ChunkBlockDepth;
					index = this.CreateDataMeshForLand(blockX, blockY, blockZ, chunk, chunk.LandData, i, j, k, index, null);
				}
			}
		}
	}

	private void GenerateWaterMeshData(Chunk chunk)
	{
		int index = 0;
		chunk.WaterData.m_Vertices.Clear();
		chunk.WaterData.m_Indices.Clear();
		chunk.WaterData.m_Uvs.Clear();
		chunk.WaterData.m_Colors.Clear();
		for (int i = 0; i < this._WorldData.ChunkBlockWidth; i++)
		{
			int blockX = chunk.X * this._WorldData.ChunkBlockWidth + i;
			for (int j = 0; j < this._WorldData.ChunkBlockHeight; j++)
			{
				int blockY = j + chunk.Y * this._WorldData.ChunkBlockHeight;
				for (int k = 0; k < this._WorldData.ChunkBlockDepth; k++)
				{
					int blockZ = k + chunk.Z * this._WorldData.ChunkBlockDepth;
					index = this.CreateDataMeshForWater(blockX, blockY, blockZ, chunk, chunk.WaterData, i, j, k, index, null);
				}
			}
		}
	}

	private void GenerateGlassMeshData(Chunk chunk)
	{
		int index = 0;
		chunk.GlassData.m_Vertices.Clear();
		chunk.GlassData.m_Indices.Clear();
		chunk.GlassData.m_Uvs.Clear();
		chunk.GlassData.m_Colors.Clear();
		for (int i = 0; i < this._WorldData.ChunkBlockWidth; i++)
		{
			int blockX = chunk.X * this._WorldData.ChunkBlockWidth + i;
			for (int j = 0; j < this._WorldData.ChunkBlockHeight; j++)
			{
				int blockY = j + chunk.Y * this._WorldData.ChunkBlockHeight;
				for (int k = 0; k < this._WorldData.ChunkBlockDepth; k++)
				{
					int blockZ = k + chunk.Z * this._WorldData.ChunkBlockDepth;
					index = this.CreateDataMeshForGlass(blockX, blockY, blockZ, chunk, chunk.GlassData, i, j, k, index, null);
				}
			}
		}
	}

	private int CreateDataMeshForLand(int blockX, int blockY, int blockZ, Chunk chunk, Chunk.MeshData data, int chunkBlockX, int chunkBlockY, int chunkBlockZ, int index, BlockGetter blockGetter)
	{
		if (blockGetter == null)
		{
			bool isEdge = chunkBlockX == 0 || chunkBlockY == 0 || chunkBlockZ == 0 || chunkBlockX == this._WorldData.ChunkBlockWidth - 1 || chunkBlockY == this._WorldData.ChunkBlockHeight - 1 || chunkBlockZ == this._WorldData.ChunkBlockDepth - 1;
			int ox = blockX - chunkBlockX;
			int oy = blockY - chunkBlockY;
			int oz = blockZ - chunkBlockZ;
			blockGetter = ((int x, int y, int z) => (!isEdge) ? chunk.GetBlockType(x - ox, y - oy, z - oz) : this._WorldData.GetBlockType(x, y, z));
		}
		BlockType blockType = blockGetter(blockX, blockY, blockZ);
		if (blockType != BlockType.Air && blockType != BlockType.Water && !WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass)
		{
			return index;
		}
		byte blockLight = World.Instance.Lighting.GetBlockLight(blockX, blockY, blockZ);
		BlockType blockType2 = blockGetter(blockX, blockY - 1, blockZ);
		if (blockType2 != BlockType.Air && blockType2 != BlockType.Water && !WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)(chunkBlockX + 1), (float)chunkBlockY, (float)chunkBlockZ, (float)(chunkBlockX + 1), (float)chunkBlockY, (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)chunkBlockY, (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)chunkBlockY, (float)chunkBlockZ, 0.5f, data, index, blockType2, BlockFace.Side, blockLight, false);
			index += 4;
		}
		blockType2 = blockGetter(blockX - 1, blockY, blockZ);
		if (blockType2 != BlockType.Air && blockType2 != BlockType.Water && !WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)chunkBlockX, (float)chunkBlockY, (float)chunkBlockZ, (float)chunkBlockX, (float)chunkBlockY, (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)(chunkBlockY + 1), (float)chunkBlockZ, 0.8f, data, index, blockType2, BlockFace.Side, blockLight, false);
			index += 4;
		}
		blockType2 = blockGetter(blockX, blockY + 1, blockZ);
		if (blockType2 != BlockType.Air && blockType2 != BlockType.Water && !WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)chunkBlockX, (float)(chunkBlockY + 1), (float)chunkBlockZ, (float)chunkBlockX, (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)chunkBlockZ, 0.9f, data, index, blockType2, BlockFace.Side, blockLight, false);
			index += 4;
		}
		blockType2 = blockGetter(blockX + 1, blockY, blockZ);
		if (blockType2 != BlockType.Air && blockType2 != BlockType.Water && !WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)chunkBlockZ, (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)(chunkBlockX + 1), (float)chunkBlockY, (float)(chunkBlockZ + 1), (float)(chunkBlockX + 1), (float)chunkBlockY, (float)chunkBlockZ, 0.7f, data, index, blockType2, BlockFace.Side, blockLight, false);
			index += 4;
		}
		blockType2 = blockGetter(blockX, blockY, blockZ + 1);
		if (blockType2 != BlockType.Air && blockType2 != BlockType.Water && !WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)(chunkBlockX + 1), (float)chunkBlockY, (float)(chunkBlockZ + 1), (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)chunkBlockY, (float)(chunkBlockZ + 1), 0.4f, data, index, blockType2, BlockFace.Bottom, blockLight, false);
			index += 4;
		}
		blockType2 = blockGetter(blockX, blockY, blockZ - 1);
		if (blockType2 != BlockType.Air && blockType2 != BlockType.Water && !WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)chunkBlockX, (float)chunkBlockY, (float)chunkBlockZ, (float)chunkBlockX, (float)(chunkBlockY + 1), (float)chunkBlockZ, (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)chunkBlockZ, (float)(chunkBlockX + 1), (float)chunkBlockY, (float)chunkBlockZ, 1f, data, index, blockType2, BlockFace.Top, blockLight, false);
			index += 4;
		}
		return index;
	}

	private int CreateDataMeshForWater(int blockX, int blockY, int blockZ, Chunk chunk, Chunk.MeshData data, int chunkBlockX, int chunkBlockY, int chunkBlockZ, int index, BlockGetter blockGetter)
	{
		if (blockGetter == null)
		{
			bool isEdge = chunkBlockX == 0 || chunkBlockY == 0 || chunkBlockZ == 0 || chunkBlockX == this._WorldData.ChunkBlockWidth - 1 || chunkBlockY == this._WorldData.ChunkBlockHeight - 1 || chunkBlockZ == this._WorldData.ChunkBlockDepth - 1;
			int ox = blockX - chunkBlockX;
			int oy = blockY - chunkBlockY;
			int oz = blockZ - chunkBlockZ;
			blockGetter = ((int x, int y, int z) => (!isEdge) ? chunk.GetBlockType(x - ox, y - oy, z - oz) : this._WorldData.GetBlockType(x, y, z));
		}
		BlockType blockType = (chunk == null) ? this._WorldData.GetBlockType(blockX, blockY, blockZ) : chunk.GetBlockType(chunkBlockX, chunkBlockY, chunkBlockZ);
		if (blockType != BlockType.Air && !WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass)
		{
			return index;
		}
		byte blockLight = World.Instance.Lighting.GetBlockLight(blockX, blockY, blockZ);
		BlockType blockType2 = blockGetter(blockX, blockY, blockZ + 1);
		bool flag = blockType2 != BlockType.Water && blockType2 != BlockType.Air;
		BlockType blockType3 = blockGetter(blockX, blockY, blockZ - 1);
		bool flag2 = blockType3 != BlockType.Water && blockType3 != BlockType.Air;
		BlockType blockType4 = blockGetter(blockX, blockY - 1, blockZ);
		if (blockType4 == BlockType.Water)
		{
			blockType2 = blockGetter(blockX, blockY - 1, blockZ + 1);
			blockType3 = blockGetter(blockX, blockY - 1, blockZ - 1);
			float num = (!flag && (blockType2 == BlockType.Water || blockType2 == BlockType.Air)) ? 0f : 0.1f;
			float num2 = (!flag2 && (blockType3 == BlockType.Water || blockType3 == BlockType.Air)) ? 0f : 0.1f;
			this.AddBlockSide((float)(chunkBlockX + 1), (float)chunkBlockY, (float)chunkBlockZ + num2 - 0.1f, (float)(chunkBlockX + 1), (float)chunkBlockY, (float)(chunkBlockZ + 1) + num - 0.1f, (float)chunkBlockX, (float)chunkBlockY, (float)(chunkBlockZ + 1) + num - 0.1f, (float)chunkBlockX, (float)chunkBlockY, (float)chunkBlockZ + num2 - 0.1f, 0.5f, data, index, blockType4, BlockFace.Side, blockLight, true);
			index += 4;
		}
		blockType4 = blockGetter(blockX - 1, blockY, blockZ);
		if (blockType4 == BlockType.Water)
		{
			blockType2 = blockGetter(blockX - 1, blockY, blockZ + 1);
			blockType3 = blockGetter(blockX - 1, blockY, blockZ - 1);
			float num = (!flag && (blockType2 == BlockType.Water || blockType2 == BlockType.Air)) ? 0f : 0.1f;
			float num2 = (!flag2 && (blockType3 == BlockType.Water || blockType3 == BlockType.Air)) ? 0f : 0.1f;
			this.AddBlockSide((float)chunkBlockX, (float)chunkBlockY, (float)chunkBlockZ + num2 - 0.1f, (float)chunkBlockX, (float)chunkBlockY, (float)(chunkBlockZ + 1) + num - 0.1f, (float)chunkBlockX, (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1) + num - 0.1f, (float)chunkBlockX, (float)(chunkBlockY + 1), (float)chunkBlockZ + num2 - 0.1f, 0.8f, data, index, blockType4, BlockFace.Side, blockLight, true);
			index += 4;
		}
		blockType4 = blockGetter(blockX, blockY + 1, blockZ);
		if (blockType4 == BlockType.Water)
		{
			blockType2 = blockGetter(blockX, blockY + 1, blockZ + 1);
			blockType3 = blockGetter(blockX, blockY + 1, blockZ - 1);
			float num = (!flag && (blockType2 == BlockType.Water || blockType2 == BlockType.Air)) ? 0f : 0.1f;
			float num2 = (!flag2 && (blockType3 == BlockType.Water || blockType3 == BlockType.Air)) ? 0f : 0.1f;
			this.AddBlockSide((float)chunkBlockX, (float)(chunkBlockY + 1), (float)chunkBlockZ + num2 - 0.1f, (float)chunkBlockX, (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1) + num - 0.1f, (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1) + num - 0.1f, (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)chunkBlockZ + num2 - 0.1f, 0.9f, data, index, blockType4, BlockFace.Side, blockLight, true);
			index += 4;
		}
		blockType4 = blockGetter(blockX + 1, blockY, blockZ);
		if (blockType4 == BlockType.Water)
		{
			blockType2 = blockGetter(blockX + 1, blockY, blockZ + 1);
			blockType3 = blockGetter(blockX + 1, blockY, blockZ - 1);
			float num = (!flag && (blockType2 == BlockType.Water || blockType2 == BlockType.Air)) ? 0f : 0.1f;
			float num2 = (!flag2 && (blockType3 == BlockType.Water || blockType3 == BlockType.Air)) ? 0f : 0.1f;
			this.AddBlockSide((float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)chunkBlockZ + num2 - 0.1f, (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1) + num - 0.1f, (float)(chunkBlockX + 1), (float)chunkBlockY, (float)(chunkBlockZ + 1) + num - 0.1f, (float)(chunkBlockX + 1), (float)chunkBlockY, (float)chunkBlockZ + num2 - 0.1f, 0.7f, data, index, blockType4, BlockFace.Side, blockLight, true);
			index += 4;
		}
		blockType4 = blockGetter(blockX, blockY, blockZ + 1);
		if (blockType4 == BlockType.Water)
		{
			this.AddBlockSide((float)(chunkBlockX + 1), (float)chunkBlockY, (float)(chunkBlockZ + 1) - 0.1f, (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1) - 0.1f, (float)chunkBlockX, (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1) - 0.1f, (float)chunkBlockX, (float)chunkBlockY, (float)(chunkBlockZ + 1) - 0.1f, 0.4f, data, index, blockType4, BlockFace.Bottom, blockLight, true);
			index += 4;
		}
		blockType4 = blockGetter(blockX, blockY, blockZ - 1);
		if (blockType4 == BlockType.Water)
		{
			this.AddBlockSide((float)chunkBlockX, (float)chunkBlockY, (float)chunkBlockZ - 0.1f, (float)chunkBlockX, (float)(chunkBlockY + 1), (float)chunkBlockZ - 0.1f, (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)chunkBlockZ - 0.1f, (float)(chunkBlockX + 1), (float)chunkBlockY, (float)chunkBlockZ - 0.1f, 1f, data, index, blockType4, BlockFace.Top, blockLight, true);
			index += 4;
		}
		return index;
	}

	private int CreateDataMeshForGlass(int blockX, int blockY, int blockZ, Chunk chunk, Chunk.MeshData data, int chunkBlockX, int chunkBlockY, int chunkBlockZ, int index, BlockGetter blockGetter)
	{
		if (blockGetter == null)
		{
			bool isEdge = chunkBlockX == 0 || chunkBlockY == 0 || chunkBlockZ == 0 || chunkBlockX == this._WorldData.ChunkBlockWidth - 1 || chunkBlockY == this._WorldData.ChunkBlockHeight - 1 || chunkBlockZ == this._WorldData.ChunkBlockDepth - 1;
			int ox = blockX - chunkBlockX;
			int oy = blockY - chunkBlockY;
			int oz = blockZ - chunkBlockZ;
			blockGetter = ((int x, int y, int z) => (!isEdge) ? chunk.GetBlockType(x - ox, y - oy, z - oz) : this._WorldData.GetBlockType(x, y, z));
		}
		BlockType blockType = blockGetter(blockX, blockY, blockZ);
		if (blockType != BlockType.Air && blockType != BlockType.Water)
		{
			return index;
		}
		byte blockLight = World.Instance.Lighting.GetBlockLight(blockX, blockY, blockZ);
		BlockType blockType2 = blockGetter(blockX, blockY - 1, blockZ);
		if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)(chunkBlockX + 1), (float)chunkBlockY, (float)chunkBlockZ, (float)(chunkBlockX + 1), (float)chunkBlockY, (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)chunkBlockY, (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)chunkBlockY, (float)chunkBlockZ, 0.5f, data, index, blockType2, BlockFace.Side, blockLight, false);
			index += 4;
		}
		blockType2 = blockGetter(blockX - 1, blockY, blockZ);
		if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)chunkBlockX, (float)chunkBlockY, (float)chunkBlockZ, (float)chunkBlockX, (float)chunkBlockY, (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)(chunkBlockY + 1), (float)chunkBlockZ, 0.8f, data, index, blockType2, BlockFace.Side, blockLight, false);
			index += 4;
		}
		blockType2 = blockGetter(blockX, blockY + 1, blockZ);
		if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)chunkBlockX, (float)(chunkBlockY + 1), (float)chunkBlockZ, (float)chunkBlockX, (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)chunkBlockZ, 0.9f, data, index, blockType2, BlockFace.Side, blockLight, false);
			index += 4;
		}
		blockType2 = blockGetter(blockX + 1, blockY, blockZ);
		if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)chunkBlockZ, (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)(chunkBlockX + 1), (float)chunkBlockY, (float)(chunkBlockZ + 1), (float)(chunkBlockX + 1), (float)chunkBlockY, (float)chunkBlockZ, 0.7f, data, index, blockType2, BlockFace.Side, blockLight, false);
			index += 4;
		}
		blockType2 = blockGetter(blockX, blockY, blockZ + 1);
		if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)(chunkBlockX + 1), (float)chunkBlockY, (float)(chunkBlockZ + 1), (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)(chunkBlockY + 1), (float)(chunkBlockZ + 1), (float)chunkBlockX, (float)chunkBlockY, (float)(chunkBlockZ + 1), 0.4f, data, index, blockType2, BlockFace.Bottom, blockLight, false);
			index += 4;
		}
		blockType2 = blockGetter(blockX, blockY, blockZ - 1);
		if (WorldGameObjectX.Instance.BlockParametrs[(int)blockType2].is_glass)
		{
			this.AddBlockSide((float)chunkBlockX, (float)chunkBlockY, (float)chunkBlockZ, (float)chunkBlockX, (float)(chunkBlockY + 1), (float)chunkBlockZ, (float)(chunkBlockX + 1), (float)(chunkBlockY + 1), (float)chunkBlockZ, (float)(chunkBlockX + 1), (float)chunkBlockY, (float)chunkBlockZ, 1f, data, index, blockType2, BlockFace.Top, blockLight, false);
			index += 4;
		}
		return index;
	}

	private void AddBlockSide(float x1, float z1, float y1, float x2, float z2, float y2, float x3, float z3, float y3, float x4, float z4, float y4, float color, Chunk.MeshData data, int index, BlockType blockType, BlockFace blockFace, byte blockLight, bool fullUV)
	{
		float num = 0.001f;
		if (blockType == BlockType.Leaves)
		{
			num = 0f;
		}
		data.m_Vertices.Add(new Vector3(x1, y1, z1));
		data.m_Vertices.Add(new Vector3(x2, y2, z2));
		data.m_Vertices.Add(new Vector3(x3, y3, z3));
		data.m_Vertices.Add(new Vector3(x4, y4, z4));
		float num2 = (float)World.Instance.Lighting.LightingSteps;
		float r = (float)(blockLight >> 4) / num2;
		float g = (float)(blockLight & 15) / num2;
		Color item = new Color(r, g, 0f, 0f);
		data.m_Colors.Add(item);
		data.m_Colors.Add(item);
		data.m_Colors.Add(item);
		data.m_Colors.Add(item);
		data.m_Indices.Add(index);
		data.m_Indices.Add(index + 1);
		data.m_Indices.Add(index + 2);
		data.m_Indices.Add(index + 2);
		data.m_Indices.Add(index + 3);
		data.m_Indices.Add(index);
		if (fullUV)
		{
			data.m_Uvs.Add(new Vector2(0f, 0f));
			data.m_Uvs.Add(new Vector2(1f, 0f));
			data.m_Uvs.Add(new Vector2(1f, 1f));
			data.m_Uvs.Add(new Vector2(0f, 1f));
		}
		else
		{
			Rect rect = this._WorldData.BlockUvCoordinates[blockType].BlockFaceUvCoordinates[(int)blockFace];
			data.m_Uvs.Add(new Vector2(rect.x + num, rect.y + num));
			data.m_Uvs.Add(new Vector2(rect.x + num, rect.y + rect.height - num));
			data.m_Uvs.Add(new Vector2(rect.x + rect.width - num, rect.y + rect.height - num));
			data.m_Uvs.Add(new Vector2(rect.x + rect.width - num, rect.y + num));
		}
	}

	private readonly IBatchProcessor<Chunk> _BatchProcessor;

	private readonly WorldData _WorldData;
}
