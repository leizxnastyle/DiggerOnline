using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OptMeshGenerator : IMeshGenerator
{
	public OptMeshGenerator(IBatchProcessor<Chunk> batchProcessor, WorldData worldData)
	{
		this.batchProcessor = batchProcessor;
		this.worldData = worldData;
		this.m_gmd.Init();
	}

	public void GenerateLandMeshes(PChunkList chunks)
	{
		this.batchProcessor.Process(1, chunks, new Action<Chunk>(this.GenerateLandMesh), false);
	}

	public void GenerateWaterMeshes(PChunkList chunks)
	{
		this.batchProcessor.Process(1, chunks, new Action<Chunk>(this.GenerateWaterMesh), false);
	}

	public void GenerateGlassMeshes(PChunkList chunks)
	{
		this.batchProcessor.Process(1, chunks, new Action<Chunk>(this.GenerateGlassMesh), false);
	}

	public Chunk.MeshData[] GenerateMeshData(int x1, int x2, int y1, int y2, int z1, int z2, BlockGetter blockGetter, BlockKindGetter kindGetter = null)
	{
		Chunk.MeshData[] result = new Chunk.MeshData[]
		{
			new Chunk.MeshData(),
			new Chunk.MeshData(),
			new Chunk.MeshData()
		};
		int[] array = new int[3];
		for (int i = x1; i <= x2; i++)
		{
			for (int j = y1; j <= y2; j++)
			{
				for (int k = z1; k <= z2; k++)
				{
					IntVect intVect = new IntVect(i, j, k);
					for (int l = 0; l < 3; l++)
					{
					}
				}
			}
		}
		return result;
	}

	private void GenerateMesh(Chunk.MeshData meshData, Chunk chunk, OptMeshGenerator.GenerateType typeData)
	{
		this.m_gmd.index = 0;
		this.ClearData(meshData);
		this.m_gmd.chunk = chunk;
		this.m_gmd.meshData = meshData;
		this.m_gmd.generateType = typeData;
		IntVect intVect = chunk.Position.MultiplyComponent(this.worldData.ChunkBlockVolume);
		for (int i = 0; i < this.worldData.ChunkBlockWidth; i++)
		{
			for (int j = 0; j < this.worldData.ChunkBlockHeight; j++)
			{
				for (int k = 0; k < this.worldData.ChunkBlockDepth; k++)
				{
					this.m_gmd.chunkBlockX = i;
					this.m_gmd.chunkBlockY = j;
					this.m_gmd.chunkBlockZ = k;
					this.m_gmd.blockX = this.m_gmd.chunkBlockX + intVect.X;
					this.m_gmd.blockY = this.m_gmd.chunkBlockY + intVect.Y;
					this.m_gmd.blockZ = this.m_gmd.chunkBlockZ + intVect.Z;
					this.m_gmd.Reset();
					this.CreateMeshDataOpt();
				}
			}
		}
	}

	public void GenerateLandMesh(Chunk chunk)
	{
		object @object = OptMeshGenerator._object;
		lock (@object)
		{
			this.GenerateMesh(chunk.LandData, chunk, OptMeshGenerator.GenerateType.Land);
			this.worldData.AddFinishedLandChunk(chunk);
		}
	}

	public void GenerateWaterMesh(Chunk chunk)
	{
		object @object = OptMeshGenerator._object;
		lock (@object)
		{
			this.GenerateMesh(chunk.WaterData, chunk, OptMeshGenerator.GenerateType.Water);
			this.worldData.AddFinishedWaterChunk(chunk);
		}
	}

	public void GenerateGlassMesh(Chunk chunk)
	{
		object @object = OptMeshGenerator._object;
		lock (@object)
		{
			this.GenerateMesh(chunk.GlassData, chunk, OptMeshGenerator.GenerateType.Glass);
			this.worldData.AddFinishedGlassChunk(chunk);
		}
	}

	private void ClearData(Chunk.MeshData mData)
	{
		mData.m_Vertices.Clear();
		mData.m_Indices.Clear();
		mData.m_Uvs.Clear();
		mData.m_Colors.Clear();
	}

	private void CreateMeshDataOpt()
	{
		IntVect intVect = this.worldData.ChunkBlockVolume - IntVect.One;
		this.m_gmd.isEdge = (this.m_gmd.chunkBlockX == 0 || this.m_gmd.chunkBlockY == 0 || this.m_gmd.chunkBlockZ == 0 || this.m_gmd.chunkBlockX == intVect.X || this.m_gmd.chunkBlockY == intVect.Y || this.m_gmd.chunkBlockZ == intVect.Z);
		this.m_gmd.water = (this.m_gmd.generateType == OptMeshGenerator.GenerateType.Water);
		BlockType blockType;
		BlockKind blockKind;
		if (this.m_gmd.water)
		{
			blockType = ((this.m_gmd.chunk != null) ? this.m_gmd.chunk.GetBlockType(this.m_gmd.chunkBlockX, this.m_gmd.chunkBlockY, this.m_gmd.chunkBlockZ) : this.worldData.GetBlockType(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ));
			blockKind = ((this.m_gmd.chunk != null) ? this.m_gmd.chunk.GetBlockKind(this.m_gmd.chunkBlockX, this.m_gmd.chunkBlockY, this.m_gmd.chunkBlockZ) : this.worldData.GetBlockKind(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ));
			if (!blockType.IsAir() && !blockType.IsGlass() && blockKind.IsDefault())
			{
				return;
			}
		}
		else
		{
			blockType = this.m_gmd.GetBlockType(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			blockKind = this.m_gmd.GetBlockKind(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			if (blockType.IsLand() && blockKind.IsDefault())
			{
				return;
			}
		}
		this.m_gmd.condition = this.generateTypeAssociate[(int)this.m_gmd.generateType];
		this.m_gmd.lightAmount = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
		this.m_gmd.blockFace = BlockFace.Side;
		this.m_gmd.sideIndex = 0;
		this.CreateMeshSideOpt(this.m_gmd.blockX - 1, this.m_gmd.blockY, this.m_gmd.blockZ);
		this.CreateMeshSideOpt(this.m_gmd.blockX + 1, this.m_gmd.blockY, this.m_gmd.blockZ);
		this.CreateMeshSideOpt(this.m_gmd.blockX, this.m_gmd.blockY - 1, this.m_gmd.blockZ);
		this.CreateMeshSideOpt(this.m_gmd.blockX, this.m_gmd.blockY + 1, this.m_gmd.blockZ);
		this.m_gmd.blockFace = BlockFace.Top;
		this.CreateMeshSideOpt(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ - 1);
		this.m_gmd.blockFace = BlockFace.Bottom;
		this.CreateMeshSideOpt(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ + 1);
		if (!blockKind.IsDefault())
		{
			this.m_gmd.flip = blockKind.IsFlip();
			if (this.m_gmd.flip)
			{
				blockKind = blockKind.DoFlip();
			}
			if (blockKind == BlockKind.Default)
			{
				return;
			}
			this.m_gmd.blockKind = blockKind;
			this.m_gmd.blockType = blockType;
			this.m_gmd.blockFace = BlockFace.Side;
			this.m_gmd.sideIndex = 0;
			this.CreateMeshSideFromOpt(this.m_gmd.blockX - 1, this.m_gmd.blockY, this.m_gmd.blockZ);
			this.CreateMeshSideFromOpt(this.m_gmd.blockX + 1, this.m_gmd.blockY, this.m_gmd.blockZ);
			this.CreateMeshSideFromOpt(this.m_gmd.blockX, this.m_gmd.blockY - 1, this.m_gmd.blockZ);
			this.CreateMeshSideFromOpt(this.m_gmd.blockX, this.m_gmd.blockY + 1, this.m_gmd.blockZ);
			this.m_gmd.blockFace = BlockFace.Top;
			this.CreateMeshSideFromOpt(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ + 1);
			this.m_gmd.blockFace = BlockFace.Bottom;
			this.CreateMeshSideFromOpt(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ - 1);
		}
	}

	private void CreateMeshSideOpt(int posX, int posY, int posZ)
	{
		this.m_gmd.blockType = this.m_gmd.GetBlockType(posX, posY, posZ);
		BlockKind blockKind = this.m_gmd.GetBlockKind(posX, posY, posZ);
		if (this.m_gmd.condition(this.m_gmd.blockType))
		{
			this.m_gmd.flip = blockKind.IsFlip();
			if (this.m_gmd.flip)
			{
				blockKind = blockKind.DoFlip();
			}
			if (blockKind != BlockKind.Default)
			{
				this.m_gmd.sideIndex = this.m_gmd.sideIndex + 1;
				return;
			}
			BlockType blockType = this.m_gmd.GetBlockType(posX, posY, posZ + 1);
			BlockType blockType2 = this.m_gmd.GetBlockType(posX, posY, posZ - 1);
			bool flag = !blockType.IsWater() && !blockType.IsAir();
			bool flag2 = !blockType2.IsWater() && !blockType2.IsAir();
			float num = 0f;
			float num2 = 0f;
			if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.water)
			{
				num2 = ((flag || (!blockType.IsWater() && !blockType.IsAir())) ? 0.1f : 0f);
				num = ((flag2 || (!blockType2.IsWater() && !blockType2.IsAir())) ? 0.1f : 0f);
			}
			for (int i = 0; i < 4; i++)
			{
				Vector3 a = Vector3.zero;
				Vector3 vector = this.offsets[this.m_gmd.sideIndex][i];
				if (this.m_gmd.water)
				{
					if (this.m_gmd.sideIndex < 4)
					{
						a += ((i == 0 || i == 3) ? new Vector3(0f, 0f, num - 0.1f) : new Vector3(0f, 0f, num2 - 0.1f));
					}
					else
					{
						a += new Vector3(0f, 0f, -0.1f);
					}
				}
				this.m_gmd.vert[i].x = a.x + (float)this.m_gmd.chunkBlockX + vector.x;
				this.m_gmd.vert[i].y = a.y + (float)this.m_gmd.chunkBlockY + vector.y;
				this.m_gmd.vert[i].z = a.z + (float)this.m_gmd.chunkBlockZ + vector.z;
			}
			this.AddBlockSideOpt();
			this.m_gmd.index = this.m_gmd.index + 4;
		}
		this.m_gmd.sideIndex = this.m_gmd.sideIndex + 1;
	}

	private void CreateMeshSideFromOpt(int posX, int posY, int posZ)
	{
		this.m_gmd.Reset();
		if (this.m_gmd.condition(this.m_gmd.blockType))
		{
			this.m_gmd.lightAmount = World.Instance.Lighting.GetBlockLight(posX, posY, posZ);
			if (this.m_gmd.blockKind.IsStair())
			{
				this.AddStair();
			}
			else if (this.m_gmd.blockKind.IsCornerStair())
			{
				switch (this.m_gmd.blockKind)
				{
				case BlockKind.CornerStairEast:
					this.AddCornerStairEast();
					goto IL_D3;
				case BlockKind.CornerStairSouth:
					this.AddCornerStairSouth();
					goto IL_D3;
				case BlockKind.CornerStairNorth:
					this.AddCornerStairNorth();
					goto IL_D3;
				}
				this.AddCornerStairWest();
				IL_D3:;
			}
			else
			{
				if (this.m_gmd.blockKind.IsFence())
				{
				}
				if (this.m_gmd.blockKind.IsQuarter())
				{
					this.AddQuarterUV();
				}
				if (this.m_gmd.blockKind.IsHalf())
				{
					this.AddHalfUV();
				}
				if (this.m_gmd.blockKind.IsThird())
				{
					this.AddThridUV();
				}
				if (this.m_gmd.blockKind.IsDiagonal())
				{
					this.AddDiagonalUV();
				}
				if (this.m_gmd.blockKind.IsCorner())
				{
					this.AddCornerUV();
				}
				for (int i = 0; i < 4; i++)
				{
					Vector3 zero = Vector3.zero;
					Vector3 vector = this.formOffsets[this.m_gmd.sideIndex][i];
					if (this.m_gmd.blockKind.IsFence())
					{
						if (this.m_gmd.blockKind == BlockKind.Fence)
						{
							vector = this.offsetsFence[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.FenceOnWallEastWest)
						{
							vector = this.offsetsFenceEastWest[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.FenceOnWallSouthNorth)
						{
							vector = this.offsetsFenceSouthNorth[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.FenceEast)
						{
							vector = this.offsetsFenceTwo[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.FenceWest)
						{
							vector = this.offsetsFenceFirst[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.FenceNorth)
						{
							vector = this.offsetsFenceFour[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.FenceSouth)
						{
							vector = this.offsetsFenceThree[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.EastFenceEast)
						{
							vector = this.offsetsEastFenceEast[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.EastFenceNorth)
						{
							vector = this.offsetsEastFenceNorth[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.EastFenceSouth)
						{
							vector = this.offsetsEastFenceSouth[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.EastFenceWest)
						{
							vector = this.offsetsEastFenceWest[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.WestFenceEast)
						{
							vector = this.offsetsWestFenceEast[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.WestFenceNorth)
						{
							vector = this.offsetsWestFenceNorth[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.WestFenceSouth)
						{
							vector = this.offsetsWestFenceSouth[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.WestFenceWest)
						{
							vector = this.offsetsWestFenceWest[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.NorthFenceEast)
						{
							vector = this.offsetsNorthFenceEast[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.NorthFenceNorth)
						{
							vector = this.offsetsNorthFenceNorth[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.NorthFenceSouth)
						{
							vector = this.offsetsNorthFenceSouth[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.NorthFenceWest)
						{
							vector = this.offsetsNorthFenceWest[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.SouthFenceEast)
						{
							vector = this.offsetsSouthFenceEast[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.SouthFenceNorth)
						{
							vector = this.offsetsSouthFenceNorth[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.SouthFenceSouth)
						{
							vector = this.offsetsSouthFenceSouth[this.m_gmd.sideIndex][i];
						}
						else if (this.m_gmd.blockKind == BlockKind.SouthFenceWest)
						{
							vector = this.offsetsSouthFenceWest[this.m_gmd.sideIndex][i];
						}
					}
					if (this.m_gmd.blockKind.IsQuarter())
					{
						this.AddQuarter(i, ref zero);
					}
					if (this.m_gmd.blockKind.IsHalf())
					{
						this.AddHalf(i, ref zero);
					}
					if (this.m_gmd.blockKind.IsThird())
					{
						this.AddThrid(i, ref zero);
					}
					if (this.m_gmd.blockKind.IsDiagonal())
					{
						this.AddDiagonal(i, ref zero);
					}
					if (this.m_gmd.blockKind.IsCorner())
					{
						this.AddCorner(i, ref zero);
					}
					this.m_gmd.vert[i].x = zero.x + (float)this.m_gmd.chunkBlockX + vector.x;
					this.m_gmd.vert[i].y = zero.y + (float)this.m_gmd.chunkBlockY + vector.y;
					this.m_gmd.vert[i].z = zero.z + (float)this.m_gmd.chunkBlockZ + vector.z;
				}
				this.AddBlockSideOpt();
				this.m_gmd.index = this.m_gmd.index + 4;
			}
		}
		this.m_gmd.sideIndex = this.m_gmd.sideIndex + 1;
	}

	private void AddBlockSideOpt()
	{
		float num = (float)World.Instance.Lighting.LightingSteps;
		float r = (float)(this.m_gmd.lightAmount >> 4) / num;
		float g = (float)(this.m_gmd.lightAmount & 15) / num;
		Color item = new Color(r, g, 0f, 0f);
		this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
		this.m_gmd.meshData.m_Colors.Add(item);
		this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
		this.m_gmd.meshData.m_Colors.Add(item);
		this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
		this.m_gmd.meshData.m_Colors.Add(item);
		this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
		this.m_gmd.meshData.m_Colors.Add(item);
		this.m_gmd.meshData.m_Indices.Add(this.m_gmd.index);
		this.m_gmd.meshData.m_Indices.Add(this.m_gmd.index + 1);
		this.m_gmd.meshData.m_Indices.Add(this.m_gmd.index + 2);
		this.m_gmd.meshData.m_Indices.Add(this.m_gmd.index + 2);
		this.m_gmd.meshData.m_Indices.Add(this.m_gmd.index + 3);
		this.m_gmd.meshData.m_Indices.Add(this.m_gmd.index);
		if (this.m_gmd.water)
		{
			this.m_gmd.meshData.m_Uvs.Add(new Vector2(0f, 0f));
			this.m_gmd.meshData.m_Uvs.Add(new Vector2(1f, 0f));
			this.m_gmd.meshData.m_Uvs.Add(new Vector2(1f, 1f));
			this.m_gmd.meshData.m_Uvs.Add(new Vector2(0f, 1f));
		}
		else
		{
			Rect rect;
			if (this.m_gmd.flip && this.m_gmd.blockFace != BlockFace.Side)
			{
				int num2 = (this.m_gmd.blockFace != BlockFace.Bottom) ? 2 : 0;
				rect = this.worldData.BlockUvCoordinates[this.m_gmd.blockType].BlockFaceUvCoordinates[num2];
			}
			else
			{
				rect = this.worldData.BlockUvCoordinates[this.m_gmd.blockType].BlockFaceUvCoordinates[(int)this.m_gmd.blockFace];
			}
			if ((this.m_gmd.blockKind == BlockKind.HalfWallSouth && this.m_gmd.sideIndex < 2) || (this.m_gmd.blockKind == BlockKind.HalfWallNorth && this.m_gmd.sideIndex < 2) || (this.m_gmd.blockKind == BlockKind.HalfWallEast && ((this.m_gmd.sideIndex > 1 && this.m_gmd.sideIndex < 4) || this.m_gmd.blockFace != BlockFace.Side)) || (this.m_gmd.blockKind == BlockKind.HalfWallWest && ((this.m_gmd.sideIndex > 1 && this.m_gmd.sideIndex < 4) || this.m_gmd.blockFace != BlockFace.Side)) || ((this.m_gmd.blockKind == BlockKind.QuarterOnWallSouth && this.m_gmd.sideIndex < 2) || (this.m_gmd.blockKind == BlockKind.QuarterOnWallNorth && this.m_gmd.sideIndex < 2) || (this.m_gmd.blockKind == BlockKind.QuarterOnWallEast && ((this.m_gmd.sideIndex > 1 && this.m_gmd.sideIndex < 4) || this.m_gmd.blockFace != BlockFace.Side))) || (this.m_gmd.blockKind == BlockKind.QuarterOnWallWest && ((this.m_gmd.sideIndex > 1 && this.m_gmd.sideIndex < 4) || this.m_gmd.blockFace != BlockFace.Side)) || ((this.m_gmd.blockKind == BlockKind.ThirdOnWallSouth && this.m_gmd.sideIndex < 2) || (this.m_gmd.blockKind == BlockKind.ThirdOnWallNorth && this.m_gmd.sideIndex < 2) || (this.m_gmd.blockKind == BlockKind.ThirdOnWallEast && ((this.m_gmd.sideIndex > 1 && this.m_gmd.sideIndex < 4) || this.m_gmd.blockFace != BlockFace.Side))) || (this.m_gmd.blockKind == BlockKind.ThirdOnWallWest && ((this.m_gmd.sideIndex > 1 && this.m_gmd.sideIndex < 4) || this.m_gmd.blockFace != BlockFace.Side)))
			{
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvY[0], rect.y + rect.height * this.m_gmd.uvX[0]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[1], rect.y + rect.height * this.m_gmd.uvY[1]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvY[2], rect.y + rect.height * this.m_gmd.uvX[2]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[3], rect.y + rect.height * this.m_gmd.uvY[3]));
			}
			else if (this.m_gmd.blockKind == BlockKind.FenceOnWallEastWest || this.m_gmd.blockKind == BlockKind.EastFenceNorth || this.m_gmd.blockKind == BlockKind.EastFenceSouth || this.m_gmd.blockKind == BlockKind.WestFenceNorth || this.m_gmd.blockKind == BlockKind.WestFenceSouth || this.m_gmd.blockKind == BlockKind.EastFenceEast || this.m_gmd.blockKind == BlockKind.EastFenceWest || this.m_gmd.blockKind == BlockKind.WestFenceEast || this.m_gmd.blockKind == BlockKind.WestFenceWest)
			{
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[0], rect.y + rect.height * this.m_gmd.uvY[0]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvY[1], rect.y + rect.height * this.m_gmd.uvX[1]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[2], rect.y + rect.height * this.m_gmd.uvY[2]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvY[3], rect.y + rect.height * this.m_gmd.uvX[3]));
			}
			else if ((this.m_gmd.blockKind == BlockKind.FenceOnWallSouthNorth && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (this.m_gmd.blockKind == BlockKind.NorthFenceNorth && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (this.m_gmd.blockKind == BlockKind.NorthFenceSouth && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (this.m_gmd.blockKind == BlockKind.NorthFenceWest && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (this.m_gmd.blockKind == BlockKind.NorthFenceEast && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (this.m_gmd.blockKind == BlockKind.SouthFenceNorth && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (this.m_gmd.blockKind == BlockKind.SouthFenceSouth && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (this.m_gmd.blockKind == BlockKind.SouthFenceWest && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (this.m_gmd.blockKind == BlockKind.SouthFenceEast && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)))
			{
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[0], rect.y + rect.height * this.m_gmd.uvY[0]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvY[1], rect.y + rect.height * this.m_gmd.uvX[1]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[2], rect.y + rect.height * this.m_gmd.uvY[2]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvY[3], rect.y + rect.height * this.m_gmd.uvX[3]));
			}
			else
			{
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[0], rect.y + rect.height * this.m_gmd.uvY[0]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[1], rect.y + rect.height * this.m_gmd.uvY[1]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[2], rect.y + rect.height * this.m_gmd.uvY[2]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[3], rect.y + rect.height * this.m_gmd.uvY[3]));
			}
		}
	}

	private void AddStair()
	{
		StairAssociation association = StairAssociation.GetAssociation(this.m_gmd.blockKind);
		BlockFaceStair blockFaceStair = association[this.m_gmd.sideIndex];
		if (blockFaceStair == BlockFaceStair.Back || (blockFaceStair == BlockFaceStair.Bottom && !this.m_gmd.flip) || (blockFaceStair == BlockFaceStair.Top && this.m_gmd.flip))
		{
			this.m_gmd.vert[0].x = (float)(this.m_gmd.chunkBlockX + this.formOffsets[this.m_gmd.sideIndex][0].X);
			this.m_gmd.vert[0].y = (float)(this.m_gmd.chunkBlockY + this.formOffsets[this.m_gmd.sideIndex][0].Y);
			this.m_gmd.vert[0].z = (float)(this.m_gmd.chunkBlockZ + this.formOffsets[this.m_gmd.sideIndex][0].Z);
			this.m_gmd.vert[1].x = (float)(this.m_gmd.chunkBlockX + this.formOffsets[this.m_gmd.sideIndex][1].X);
			this.m_gmd.vert[1].y = (float)(this.m_gmd.chunkBlockY + this.formOffsets[this.m_gmd.sideIndex][1].Y);
			this.m_gmd.vert[1].z = (float)(this.m_gmd.chunkBlockZ + this.formOffsets[this.m_gmd.sideIndex][1].Z);
			this.m_gmd.vert[2].x = (float)(this.m_gmd.chunkBlockX + this.formOffsets[this.m_gmd.sideIndex][2].X);
			this.m_gmd.vert[2].y = (float)(this.m_gmd.chunkBlockY + this.formOffsets[this.m_gmd.sideIndex][2].Y);
			this.m_gmd.vert[2].z = (float)(this.m_gmd.chunkBlockZ + this.formOffsets[this.m_gmd.sideIndex][2].Z);
			this.m_gmd.vert[3].x = (float)(this.m_gmd.chunkBlockX + this.formOffsets[this.m_gmd.sideIndex][3].X);
			this.m_gmd.vert[3].y = (float)(this.m_gmd.chunkBlockY + this.formOffsets[this.m_gmd.sideIndex][3].Y);
			this.m_gmd.vert[3].z = (float)(this.m_gmd.chunkBlockZ + this.formOffsets[this.m_gmd.sideIndex][3].Z);
			if (this.m_gmd.flip && blockFaceStair == BlockFaceStair.Back)
			{
				this.m_gmd.uvY[0] = 1f;
				this.m_gmd.uvY[1] = 0f;
				this.m_gmd.uvY[2] = 0f;
				this.m_gmd.uvY[3] = 1f;
			}
			this.AddBlockSideOpt();
			this.m_gmd.index = this.m_gmd.index + 4;
		}
		else if (blockFaceStair == BlockFaceStair.Forward || (blockFaceStair == BlockFaceStair.Top && !this.m_gmd.flip) || (blockFaceStair == BlockFaceStair.Bottom && this.m_gmd.flip))
		{
			Vector3 v = association.Top.v1;
			Vector3 v2 = association.Top.v2;
			int[] num = association.Top.num;
			float[][] uvY = association.Top.uvY;
			float[][] uvX = association.Top.uvX;
			if (blockFaceStair == BlockFaceStair.Forward)
			{
				uvY = association.Forward.uvY;
				uvX = association.Forward.uvX;
				v = association.Forward.v1;
				v2 = association.Forward.v2;
				num = association.Forward.num;
			}
			for (int i = 0; i < 2; i++)
			{
				this.m_gmd.uvX[0] = uvX[i][0];
				this.m_gmd.uvX[1] = uvX[i][1];
				this.m_gmd.uvX[2] = uvX[i][2];
				this.m_gmd.uvX[3] = uvX[i][3];
				this.m_gmd.uvY[0] = uvY[i][0];
				this.m_gmd.uvY[1] = uvY[i][1];
				this.m_gmd.uvY[2] = uvY[i][2];
				this.m_gmd.uvY[3] = uvY[i][3];
				for (int j = 0; j < 4; j++)
				{
					Vector3 a = this.formOffsets[this.m_gmd.sideIndex][j];
					if (!this.m_gmd.flip)
					{
						if (i == 0)
						{
							a += v;
							if (j == num[0] || j == num[1])
							{
								a += v2;
							}
						}
						else if (j == num[2] || j == num[3])
						{
							a += v2 * -1f;
						}
					}
					else if (i == 0)
					{
						if (blockFaceStair == BlockFaceStair.Forward)
						{
							this.m_gmd.uvY[0] = 0.5f;
							this.m_gmd.uvY[1] = 0f;
							this.m_gmd.uvY[2] = 0f;
							this.m_gmd.uvY[3] = 0.5f;
						}
						if (this.m_gmd.blockKind == BlockKind.StairNorth || this.m_gmd.blockKind == BlockKind.StairSouth)
						{
							if (blockFaceStair == BlockFaceStair.Bottom)
							{
								a -= v;
							}
							if (j == num[0] || j == num[1])
							{
								a += v2;
							}
						}
						else if (this.m_gmd.blockKind == BlockKind.StairWest || this.m_gmd.blockKind == BlockKind.StairEast)
						{
							if (blockFaceStair == BlockFaceStair.Bottom)
							{
								if (j == num[0] || j == num[1])
								{
									a -= v2;
								}
							}
							else if (blockFaceStair == BlockFaceStair.Forward && (j == num[0] || j == num[1]))
							{
								a += v2;
							}
						}
					}
					else
					{
						if (blockFaceStair == BlockFaceStair.Forward)
						{
							this.m_gmd.uvY[0] = 1f;
							this.m_gmd.uvY[1] = 0.5f;
							this.m_gmd.uvY[2] = 0.5f;
							this.m_gmd.uvY[3] = 1f;
						}
						if (this.m_gmd.blockKind == BlockKind.StairWest || this.m_gmd.blockKind == BlockKind.StairEast)
						{
							if (blockFaceStair == BlockFaceStair.Bottom)
							{
								if (j == num[2] || j == num[3])
								{
									a += v2;
								}
								a -= v;
							}
							else if (blockFaceStair == BlockFaceStair.Forward && (j == num[2] || j == num[3]))
							{
								a += v2 * -1f;
							}
						}
						else if (j == num[2] || j == num[3])
						{
							a += v2 * -1f;
						}
						if (blockFaceStair == BlockFaceStair.Forward)
						{
							a += v;
						}
					}
					this.m_gmd.vert[j].x = (float)this.m_gmd.chunkBlockX + a.x;
					this.m_gmd.vert[j].y = (float)this.m_gmd.chunkBlockY + a.y;
					this.m_gmd.vert[j].z = (float)this.m_gmd.chunkBlockZ + a.z;
				}
				this.AddBlockSideOpt();
				this.m_gmd.index = this.m_gmd.index + 4;
			}
		}
		else if (blockFaceStair == BlockFaceStair.Right || blockFaceStair == BlockFaceStair.Left)
		{
			Vector3 v3 = association.Right.v1;
			Vector3 v4 = association.Right.v2;
			int[] num2 = association.Right.num;
			float[][] uvY = association.Right.uvY;
			float[][] uvX = association.Right.uvX;
			if (blockFaceStair == BlockFaceStair.Left)
			{
				uvY = association.Left.uvY;
				uvX = association.Left.uvX;
				num2 = association.Left.num;
			}
			for (int k = 0; k < 2; k++)
			{
				this.m_gmd.uvX = uvX[k];
				this.m_gmd.uvY = uvY[k];
				for (int l = 0; l < 4; l++)
				{
					Vector3 a2 = this.formOffsets[this.m_gmd.sideIndex][l];
					if (!this.m_gmd.flip)
					{
						if (k == 0)
						{
							if (l == num2[0] || l == num2[1])
							{
								a2 += v3;
							}
							if (l == num2[0] || l == num2[2])
							{
								a2 += v4;
							}
						}
						else if (l == num2[3] || l == num2[2])
						{
							a2 += v3 * -1f;
						}
					}
					else if (k == 0)
					{
						if (l == num2[0] || l == num2[1])
						{
							a2 += v3;
						}
						if (l == num2[1] || l == num2[3])
						{
							a2 -= v4;
						}
						this.m_gmd.uvY = new float[]
						{
							0.5f,
							0f,
							0f,
							0.5f
						};
					}
					else
					{
						if (l == num2[3] || l == num2[2])
						{
							a2 += v3 * -1f;
						}
						this.m_gmd.uvY = new float[]
						{
							1f,
							0f,
							0f,
							1f
						};
					}
					this.m_gmd.vert[l].x = (float)this.m_gmd.chunkBlockX + a2.x;
					this.m_gmd.vert[l].y = (float)this.m_gmd.chunkBlockY + a2.y;
					this.m_gmd.vert[l].z = (float)this.m_gmd.chunkBlockZ + a2.z;
				}
				this.AddBlockSideOpt();
				this.m_gmd.index = this.m_gmd.index + 4;
			}
		}
	}

	private void AddCornerStairEast()
	{
		this.m_gmd.h = 0;
		while (this.m_gmd.h <= 1)
		{
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector = this.formOffsets[this.m_gmd.sideIndex][i];
				Vector3 a = Vector3.zero;
				if (!this.m_gmd.flip)
				{
					if (this.m_gmd.blockFace == BlockFace.Side)
					{
						float[] array = new float[4];
						array[1] = 0.5f;
						array[2] = 0.5f;
						this.m_gmd.uvY = array;
					}
					if (this.m_gmd.h == 0)
					{
						if ((this.m_gmd.sideIndex < 4 && (i == 1 || i == 2)) || this.m_gmd.sideIndex == 4)
						{
							a -= new Vector3(0f, 0f, 0.5f);
						}
					}
					else if (this.m_gmd.h == 1)
					{
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 0)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 1)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 2)
						{
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 3)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						this.m_gmd.uvY = new float[]
						{
							0.5f,
							1f,
							1f,
							0.5f
						};
						if (this.m_gmd.blockFace == BlockFace.Top)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if ((this.m_gmd.sideIndex < 4 && (i == 0 || i == 3)) || this.m_gmd.sideIndex == 5)
						{
							a += new Vector3(0f, 0f, 0.5f);
						}
						if (this.m_gmd.sideIndex == 1 || (this.m_gmd.sideIndex == 3 && i < 2))
						{
							a -= new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 3 || (this.m_gmd.sideIndex == 1 && i > 1))
						{
							a -= new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 0 && i < 2)
						{
							a -= new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 2 && i > 1)
						{
							a -= new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 1)
						{
							a -= new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 2)
						{
							a -= new Vector3(0.5f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 3)
						{
							a -= new Vector3(0.5f, 0f, 0f);
						}
					}
				}
				else if (this.m_gmd.h == 0)
				{
					if ((this.m_gmd.sideIndex < 4 && (i == 1 || i == 2)) || this.m_gmd.sideIndex == 4)
					{
						a -= new Vector3(0f, 0f, 0.5f);
					}
					if (this.m_gmd.sideIndex == 1 || (this.m_gmd.sideIndex == 3 && i < 2))
					{
						a -= new Vector3(0.5f, 0f, 0f);
					}
					if (this.m_gmd.sideIndex == 3 || (this.m_gmd.sideIndex == 1 && i > 1))
					{
						a -= new Vector3(0f, 0.5f, 0f);
					}
					if (this.m_gmd.sideIndex == 0 && i < 2)
					{
						a -= new Vector3(0f, 0.5f, 0f);
					}
					if (this.m_gmd.sideIndex == 2 && i > 1)
					{
						a -= new Vector3(0.5f, 0f, 0f);
					}
					if (this.m_gmd.sideIndex == 5 && i == 2)
					{
						a -= new Vector3(0f, 0.5f, 0f);
					}
					if (this.m_gmd.sideIndex == 5 && i == 1)
					{
						a -= new Vector3(0.5f, 0.5f, 0f);
					}
					if (this.m_gmd.sideIndex == 5 && i == 0)
					{
						a -= new Vector3(0.5f, 0f, 0f);
					}
					if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 0)
					{
						this.m_gmd.uvX = new float[]
						{
							0.5f,
							0.5f,
							1f,
							1f
						};
					}
					if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 1)
					{
						this.m_gmd.uvX = new float[]
						{
							1f,
							1f,
							0.5f,
							0.5f
						};
					}
					if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 2)
					{
						this.m_gmd.uvX = new float[]
						{
							0f,
							0f,
							0.5f,
							0.5f
						};
					}
					if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 3)
					{
						this.m_gmd.uvX = new float[]
						{
							0.5f,
							0.5f,
							1f,
							1f
						};
					}
					float[] array2 = new float[4];
					array2[1] = 0.5f;
					array2[2] = 0.5f;
					this.m_gmd.uvY = array2;
					if (this.m_gmd.blockFace == BlockFace.Bottom)
					{
						this.m_gmd.uvX = new float[]
						{
							0.5f,
							0.5f,
							1f,
							1f
						};
					}
				}
				else if (this.m_gmd.h == 1)
				{
					if (this.m_gmd.blockFace == BlockFace.Side)
					{
						this.m_gmd.uvY = new float[]
						{
							0.5f,
							1f,
							1f,
							0.5f
						};
						this.m_gmd.uvX = new float[]
						{
							0f,
							0f,
							1f,
							1f
						};
					}
					if (this.m_gmd.blockFace == BlockFace.Bottom)
					{
						this.m_gmd.uvX = new float[]
						{
							0f,
							0f,
							1f,
							1f
						};
						float[] array3 = new float[4];
						array3[1] = 1f;
						array3[2] = 1f;
						this.m_gmd.uvY = array3;
					}
					if ((this.m_gmd.sideIndex < 4 && (i == 0 || i == 3)) || this.m_gmd.sideIndex == 5)
					{
						a += new Vector3(0f, 0f, 0.5f);
					}
				}
				this.m_gmd.vert[i].x = a.x + (float)this.m_gmd.chunkBlockX + vector.x;
				this.m_gmd.vert[i].y = a.y + (float)this.m_gmd.chunkBlockY + vector.y;
				this.m_gmd.vert[i].z = a.z + (float)this.m_gmd.chunkBlockZ + vector.z;
			}
			this.AddBlockSideOpt();
			this.m_gmd.index = this.m_gmd.index + 4;
			this.m_gmd.h = this.m_gmd.h + 1;
		}
	}

	private void AddCornerStairWest()
	{
		this.m_gmd.h = 0;
		while (this.m_gmd.h <= 1)
		{
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector = this.formOffsets[this.m_gmd.sideIndex][i];
				Vector3 a = Vector3.zero;
				if (!this.m_gmd.flip)
				{
					if (this.m_gmd.blockFace == BlockFace.Side)
					{
						float[] array = new float[4];
						array[1] = 0.5f;
						array[2] = 0.5f;
						this.m_gmd.uvY = array;
					}
					if (this.m_gmd.h == 0)
					{
						if ((this.m_gmd.sideIndex < 4 && (i == 1 || i == 2)) || this.m_gmd.sideIndex == 4)
						{
							a -= new Vector3(0f, 0f, 0.5f);
						}
					}
					else if (this.m_gmd.h == 1)
					{
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 0)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 1)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 3)
						{
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 2)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						this.m_gmd.uvY = new float[]
						{
							0.5f,
							1f,
							1f,
							0.5f
						};
						if (this.m_gmd.blockFace == BlockFace.Top)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						if ((this.m_gmd.sideIndex < 4 && (i == 0 || i == 3)) || this.m_gmd.sideIndex == 5)
						{
							a += new Vector3(0f, 0f, 0.5f);
						}
						if (this.m_gmd.sideIndex == 0 || (this.m_gmd.sideIndex == 2 && i < 2))
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 2 || (this.m_gmd.sideIndex == 0 && i > 1))
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 1 && i < 2)
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 3 && i > 1)
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 3)
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 0)
						{
							a += new Vector3(0.5f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 1)
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
					}
				}
				else
				{
					if (this.m_gmd.blockFace == BlockFace.Side)
					{
						float[] array2 = new float[4];
						array2[1] = 0.5f;
						array2[2] = 0.5f;
						this.m_gmd.uvY = array2;
					}
					if (this.m_gmd.h == 0)
					{
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 0)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 1)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 3)
						{
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 2)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						float[] array3 = new float[4];
						array3[1] = 0.5f;
						array3[2] = 0.5f;
						this.m_gmd.uvY = array3;
						if (this.m_gmd.blockFace == BlockFace.Bottom)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if ((this.m_gmd.sideIndex < 4 && (i == 1 || i == 2)) || this.m_gmd.sideIndex == 4)
						{
							a -= new Vector3(0f, 0f, 0.5f);
						}
						if (this.m_gmd.sideIndex == 0 || (this.m_gmd.sideIndex == 2 && i < 2))
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 2 || (this.m_gmd.sideIndex == 0 && i > 1))
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 1 && i < 2)
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 3 && i > 1)
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 5 && i == 0)
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 5 && i == 3)
						{
							a += new Vector3(0.5f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 5 && i == 2)
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
					}
					else if (this.m_gmd.h == 1)
					{
						if (this.m_gmd.blockFace == BlockFace.Side)
						{
							this.m_gmd.uvY = new float[]
							{
								0.5f,
								1f,
								1f,
								0.5f
							};
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								1f,
								1f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Bottom)
						{
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								1f,
								1f
							};
							float[] array4 = new float[4];
							array4[1] = 1f;
							array4[2] = 1f;
							this.m_gmd.uvY = array4;
						}
						if ((this.m_gmd.sideIndex < 4 && (i == 0 || i == 3)) || this.m_gmd.sideIndex == 5)
						{
							a += new Vector3(0f, 0f, 0.5f);
						}
					}
				}
				this.m_gmd.vert[i].x = a.x + (float)this.m_gmd.chunkBlockX + vector.x;
				this.m_gmd.vert[i].y = a.y + (float)this.m_gmd.chunkBlockY + vector.y;
				this.m_gmd.vert[i].z = a.z + (float)this.m_gmd.chunkBlockZ + vector.z;
			}
			this.AddBlockSideOpt();
			this.m_gmd.index = this.m_gmd.index + 4;
			this.m_gmd.h = this.m_gmd.h + 1;
		}
	}

	private void AddCornerStairSouth()
	{
		this.m_gmd.h = 0;
		while (this.m_gmd.h <= 1)
		{
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector = this.formOffsets[this.m_gmd.sideIndex][i];
				Vector3 a = Vector3.zero;
				if (!this.m_gmd.flip)
				{
					if (this.m_gmd.blockFace == BlockFace.Side)
					{
						float[] array = new float[4];
						array[1] = 0.5f;
						array[2] = 0.5f;
						this.m_gmd.uvY = array;
					}
					if (this.m_gmd.h == 0)
					{
						if ((this.m_gmd.sideIndex < 4 && (i == 1 || i == 2)) || this.m_gmd.sideIndex == 4)
						{
							a -= new Vector3(0f, 0f, 0.5f);
						}
					}
					else if (this.m_gmd.h == 1)
					{
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 1)
						{
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 0)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 3)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 2)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						this.m_gmd.uvY = new float[]
						{
							0.5f,
							1f,
							1f,
							0.5f
						};
						if (this.m_gmd.blockFace == BlockFace.Top)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						if ((this.m_gmd.sideIndex < 4 && (i == 0 || i == 3)) || this.m_gmd.sideIndex == 5)
						{
							a += new Vector3(0f, 0f, 0.5f);
						}
						if (this.m_gmd.sideIndex == 0 || (this.m_gmd.sideIndex == 2 && i < 2))
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 3 || (this.m_gmd.sideIndex == 0 && i < 2))
						{
							a -= new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 1 && i > 1)
						{
							a -= new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 3 && i > 1)
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 2)
						{
							a -= new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 1)
						{
							a += new Vector3(0.5f, -0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 0)
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
					}
				}
				else
				{
					if (this.m_gmd.blockFace == BlockFace.Side)
					{
						this.m_gmd.uvY = new float[]
						{
							0.5f,
							1f,
							1f,
							0.5f
						};
					}
					if (this.m_gmd.h == 0)
					{
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 1)
						{
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 0)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 3)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 2)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						float[] array2 = new float[4];
						array2[1] = 0.5f;
						array2[2] = 0.5f;
						this.m_gmd.uvY = array2;
						if (this.m_gmd.blockFace == BlockFace.Bottom)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if ((this.m_gmd.sideIndex < 4 && (i == 1 || i == 2)) || this.m_gmd.sideIndex == 4)
						{
							a -= new Vector3(0f, 0f, 0.5f);
						}
						if (this.m_gmd.sideIndex == 0 || (this.m_gmd.sideIndex == 2 && i < 2))
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 3 || (this.m_gmd.sideIndex == 0 && i < 2))
						{
							a -= new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 1 && i > 1)
						{
							a -= new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 3 && i > 1)
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 5 && i == 1)
						{
							a -= new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 5 && i == 2)
						{
							a += new Vector3(0.5f, -0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 5 && i == 3)
						{
							a += new Vector3(0.5f, 0f, 0f);
						}
					}
					else if (this.m_gmd.h == 1)
					{
						if (this.m_gmd.blockFace == BlockFace.Side)
						{
							this.m_gmd.uvY = new float[]
							{
								0.5f,
								1f,
								1f,
								0.5f
							};
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								1f,
								1f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Bottom)
						{
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								1f,
								1f
							};
							float[] array3 = new float[4];
							array3[1] = 1f;
							array3[2] = 1f;
							this.m_gmd.uvY = array3;
						}
						if ((this.m_gmd.sideIndex < 4 && (i == 0 || i == 3)) || this.m_gmd.sideIndex == 5)
						{
							a += new Vector3(0f, 0f, 0.5f);
						}
					}
				}
				this.m_gmd.vert[i].x = a.x + (float)this.m_gmd.chunkBlockX + vector.x;
				this.m_gmd.vert[i].y = a.y + (float)this.m_gmd.chunkBlockY + vector.y;
				this.m_gmd.vert[i].z = a.z + (float)this.m_gmd.chunkBlockZ + vector.z;
			}
			this.AddBlockSideOpt();
			this.m_gmd.index = this.m_gmd.index + 4;
			this.m_gmd.h = this.m_gmd.h + 1;
		}
	}

	private void AddCornerStairNorth()
	{
		this.m_gmd.h = 0;
		while (this.m_gmd.h <= 1)
		{
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector = this.formOffsets[this.m_gmd.sideIndex][i];
				Vector3 a = Vector3.zero;
				if (!this.m_gmd.flip)
				{
					if (this.m_gmd.blockFace == BlockFace.Side)
					{
						float[] array = new float[4];
						array[1] = 0.5f;
						array[2] = 0.5f;
						this.m_gmd.uvY = array;
					}
					if (this.m_gmd.h == 0)
					{
						if ((this.m_gmd.sideIndex < 4 && (i == 1 || i == 2)) || this.m_gmd.sideIndex == 4)
						{
							a -= new Vector3(0f, 0f, 0.5f);
						}
					}
					else if (this.m_gmd.h == 1)
					{
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 0)
						{
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 1)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 2)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 3)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						this.m_gmd.uvY = new float[]
						{
							0.5f,
							1f,
							1f,
							0.5f
						};
						if (this.m_gmd.blockFace == BlockFace.Top)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if ((this.m_gmd.sideIndex < 4 && (i == 0 || i == 3)) || this.m_gmd.sideIndex == 5)
						{
							a += new Vector3(0f, 0f, 0.5f);
						}
						if (this.m_gmd.sideIndex == 1 || (this.m_gmd.sideIndex == 2 && i > 1))
						{
							a -= new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 2 || (this.m_gmd.sideIndex == 1 && i < 2))
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 0 && i > 1)
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 3 && i < 2)
						{
							a -= new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 0)
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 3)
						{
							a += new Vector3(-0.5f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 4 && i == 2)
						{
							a -= new Vector3(0.5f, 0f, 0f);
						}
					}
				}
				else
				{
					if (this.m_gmd.blockFace == BlockFace.Side)
					{
						this.m_gmd.uvY = new float[]
						{
							0.5f,
							1f,
							1f,
							0.5f
						};
					}
					if (this.m_gmd.h == 0)
					{
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 0)
						{
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 1)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 2)
						{
							this.m_gmd.uvX = new float[]
							{
								1f,
								1f,
								0.5f,
								0.5f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex == 3)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						float[] array2 = new float[4];
						array2[1] = 0.5f;
						array2[2] = 0.5f;
						this.m_gmd.uvY = array2;
						if (this.m_gmd.blockFace == BlockFace.Bottom)
						{
							this.m_gmd.uvX = new float[]
							{
								0.5f,
								0.5f,
								1f,
								1f
							};
						}
						if ((this.m_gmd.sideIndex < 4 && (i == 1 || i == 2)) || this.m_gmd.sideIndex == 4)
						{
							a -= new Vector3(0f, 0f, 0.5f);
						}
						if (this.m_gmd.sideIndex == 1 || (this.m_gmd.sideIndex == 2 && i > 1))
						{
							a -= new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 2 || (this.m_gmd.sideIndex == 1 && i < 2))
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 0 && i > 1)
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 3 && i < 2)
						{
							a -= new Vector3(0.5f, 0f, 0f);
						}
						if (this.m_gmd.sideIndex == 5 && i == 3)
						{
							a += new Vector3(0f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 5 && i == 0)
						{
							a += new Vector3(-0.5f, 0.5f, 0f);
						}
						if (this.m_gmd.sideIndex == 5 && i == 1)
						{
							a -= new Vector3(0.5f, 0f, 0f);
						}
					}
					else if (this.m_gmd.h == 1)
					{
						if (this.m_gmd.blockFace == BlockFace.Side)
						{
							this.m_gmd.uvY = new float[]
							{
								0.5f,
								1f,
								1f,
								0.5f
							};
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								1f,
								1f
							};
						}
						if (this.m_gmd.blockFace == BlockFace.Bottom)
						{
							this.m_gmd.uvX = new float[]
							{
								0f,
								0f,
								1f,
								1f
							};
							float[] array3 = new float[4];
							array3[1] = 1f;
							array3[2] = 1f;
							this.m_gmd.uvY = array3;
						}
						if ((this.m_gmd.sideIndex < 4 && (i == 0 || i == 3)) || this.m_gmd.sideIndex == 5)
						{
							a += new Vector3(0f, 0f, 0.5f);
						}
					}
				}
				this.m_gmd.vert[i].x = a.x + (float)this.m_gmd.chunkBlockX + vector.x;
				this.m_gmd.vert[i].y = a.y + (float)this.m_gmd.chunkBlockY + vector.y;
				this.m_gmd.vert[i].z = a.z + (float)this.m_gmd.chunkBlockZ + vector.z;
			}
			this.AddBlockSideOpt();
			this.m_gmd.index = this.m_gmd.index + 4;
			this.m_gmd.h = this.m_gmd.h + 1;
		}
	}

	private void AddHalfUV()
	{
		if (this.m_gmd.blockKind == BlockKind.HalfWallSouth || this.m_gmd.blockKind == BlockKind.HalfWallNorth)
		{
			if (this.m_gmd.blockFace == BlockFace.Top || this.m_gmd.blockFace == BlockFace.Bottom)
			{
				float[] array = new float[4];
				array[1] = 0.5f;
				array[2] = 0.5f;
				this.m_gmd.uvY = array;
			}
			if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex < 2)
			{
				this.m_gmd.uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.5f
				};
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					0.5f,
					1f
				};
			}
		}
		if (this.m_gmd.blockKind == BlockKind.HalfWallWest || this.m_gmd.blockKind == BlockKind.HalfWallEast)
		{
			if (this.m_gmd.blockFace == BlockFace.Top || this.m_gmd.blockFace == BlockFace.Bottom)
			{
				this.m_gmd.uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.5f
				};
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					0.5f,
					1f
				};
			}
			if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex > 1 && this.m_gmd.sideIndex < 4)
			{
				this.m_gmd.uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.5f
				};
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					0.5f,
					1f
				};
			}
		}
		if (this.m_gmd.blockKind == BlockKind.Half && this.m_gmd.blockFace == BlockFace.Side)
		{
			float[] uvY;
			if (!this.m_gmd.flip)
			{
				float[] array2 = new float[4];
				array2[1] = 0.5f;
				uvY = array2;
				array2[2] = 0.5f;
			}
			else
			{
				RuntimeHelpers.InitializeArray(uvY = new float[4], fieldof(_003CPrivateImplementationDetails_003E._0024_0024field-141).FieldHandle);
			}
			this.m_gmd.uvY = uvY;
		}
	}

	private void AddHalf(int j, ref Vector3 vec)
	{
		if (this.m_gmd.blockKind == BlockKind.HalfWallSouth)
		{
			if (this.m_gmd.sideIndex == 0 && j > 1)
			{
				vec += new Vector3(0f, 0.5f, 0f);
			}
			if (this.m_gmd.sideIndex == 1 && j < 2)
			{
				vec += new Vector3(0f, 0.5f, 0f);
			}
			if ((this.m_gmd.sideIndex == 4 && j == 3) || (this.m_gmd.sideIndex == 4 && j == 0))
			{
				vec += new Vector3(0f, 0.5f, 0f);
			}
			if (this.m_gmd.sideIndex == 2)
			{
				vec += new Vector3(0f, 0.5f, 0f);
			}
			if ((this.m_gmd.sideIndex == 5 && j == 3) || (this.m_gmd.sideIndex == 5 && j == 0))
			{
				vec += new Vector3(0f, 0.5f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.HalfWallNorth)
		{
			if (this.m_gmd.sideIndex == 0 && j < 2)
			{
				vec -= new Vector3(0f, 0.5f, 0f);
			}
			if (this.m_gmd.sideIndex == 3)
			{
				vec -= new Vector3(0f, 0.5f, 0f);
			}
			if (this.m_gmd.sideIndex == 5 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.5f, 0f);
			}
			if (this.m_gmd.sideIndex == 4 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.5f, 0f);
			}
			if (this.m_gmd.sideIndex == 1 && j > 1)
			{
				vec -= new Vector3(0f, 0.5f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.HalfWallEast)
		{
			if (this.m_gmd.sideIndex == 2 && j < 2)
			{
				vec += new Vector3(0.5f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 0)
			{
				vec += new Vector3(0.5f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 3 && j > 1)
			{
				vec += new Vector3(0.5f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 5 && j > 1)
			{
				vec += new Vector3(0.5f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 4 && j < 2)
			{
				vec += new Vector3(0.5f, 0f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.HalfWallWest)
		{
			if (this.m_gmd.sideIndex == 2 && j > 1)
			{
				vec -= new Vector3(0.5f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 1)
			{
				vec -= new Vector3(0.5f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 3 && j < 2)
			{
				vec -= new Vector3(0.5f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 5 && j < 2)
			{
				vec -= new Vector3(0.5f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 4 && j > 1)
			{
				vec -= new Vector3(0.5f, 0f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.Half)
		{
			if (!this.m_gmd.flip)
			{
				if ((this.m_gmd.sideIndex < 4 && (j == 1 || j == 2)) || this.m_gmd.sideIndex == 4)
				{
					vec -= new Vector3(0f, 0f, 0.5f);
				}
			}
			else if ((this.m_gmd.sideIndex < 4 && (j == 0 || j == 3)) || this.m_gmd.sideIndex == 5)
			{
				vec += new Vector3(0f, 0f, 0.5f);
			}
		}
	}

	private void AddQuarterUV()
	{
		if (this.m_gmd.blockKind == BlockKind.QuarterOnWallSouth || this.m_gmd.blockKind == BlockKind.QuarterOnWallNorth)
		{
			if (this.m_gmd.blockFace == BlockFace.Top || this.m_gmd.blockFace == BlockFace.Bottom)
			{
				float[] array = new float[4];
				array[1] = 0.25f;
				array[2] = 0.25f;
				this.m_gmd.uvY = array;
			}
			if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex < 2)
			{
				this.m_gmd.uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.25f
				};
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					0.25f,
					1f
				};
			}
		}
		if (this.m_gmd.blockKind == BlockKind.QuarterOnWallWest || this.m_gmd.blockKind == BlockKind.QuarterOnWallEast)
		{
			if (this.m_gmd.blockFace == BlockFace.Top || this.m_gmd.blockFace == BlockFace.Bottom)
			{
				this.m_gmd.uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.25f
				};
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					0.25f,
					1f
				};
			}
			if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex > 1 && this.m_gmd.sideIndex < 4)
			{
				this.m_gmd.uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.25f
				};
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					0.25f,
					1f
				};
			}
		}
		if (this.m_gmd.blockKind == BlockKind.Quarter && this.m_gmd.blockFace == BlockFace.Side)
		{
			float[] uvY;
			if (!this.m_gmd.flip)
			{
				float[] array2 = new float[4];
				array2[1] = 0.25f;
				uvY = array2;
				array2[2] = 0.25f;
			}
			else
			{
				float[] array3 = new float[4];
				array3[0] = 0.25f;
				uvY = array3;
				array3[3] = 0.25f;
			}
			this.m_gmd.uvY = uvY;
		}
	}

	private void AddQuarter(int j, ref Vector3 vec)
	{
		if (this.m_gmd.blockKind == BlockKind.Quarter)
		{
			if (!this.m_gmd.flip)
			{
				if ((this.m_gmd.sideIndex < 4 && (j == 1 || j == 2)) || this.m_gmd.sideIndex == 4)
				{
					vec -= new Vector3(0f, 0f, 0.75f);
				}
			}
			else if ((this.m_gmd.sideIndex < 4 && (j == 0 || j == 3)) || this.m_gmd.sideIndex == 5)
			{
				vec += new Vector3(0f, 0f, 0.75f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.QuarterOnWallSouth)
		{
			if (this.m_gmd.sideIndex == 0 && j > 1)
			{
				vec += new Vector3(0f, 0.75f, 0f);
			}
			if (this.m_gmd.sideIndex == 1 && j < 2)
			{
				vec += new Vector3(0f, 0.75f, 0f);
			}
			if ((this.m_gmd.sideIndex == 4 && j == 3) || (this.m_gmd.sideIndex == 4 && j == 0))
			{
				vec += new Vector3(0f, 0.75f, 0f);
			}
			if (this.m_gmd.sideIndex == 2)
			{
				vec += new Vector3(0f, 0.75f, 0f);
			}
			if ((this.m_gmd.sideIndex == 5 && j == 3) || (this.m_gmd.sideIndex == 5 && j == 0))
			{
				vec += new Vector3(0f, 0.75f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.QuarterOnWallNorth)
		{
			if (this.m_gmd.sideIndex == 0 && j < 2)
			{
				vec -= new Vector3(0f, 0.75f, 0f);
			}
			if (this.m_gmd.sideIndex == 3)
			{
				vec -= new Vector3(0f, 0.75f, 0f);
			}
			if (this.m_gmd.sideIndex == 5 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.75f, 0f);
			}
			if (this.m_gmd.sideIndex == 4 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.75f, 0f);
			}
			if (this.m_gmd.sideIndex == 1 && j > 1)
			{
				vec -= new Vector3(0f, 0.75f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.QuarterOnWallEast)
		{
			if (this.m_gmd.sideIndex == 2 && j < 2)
			{
				vec += new Vector3(0.75f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 0)
			{
				vec += new Vector3(0.75f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 3 && j > 1)
			{
				vec += new Vector3(0.75f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 5 && j > 1)
			{
				vec += new Vector3(0.75f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 4 && j < 2)
			{
				vec += new Vector3(0.75f, 0f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.QuarterOnWallWest)
		{
			if (this.m_gmd.sideIndex == 2 && j > 1)
			{
				vec -= new Vector3(0.75f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 1)
			{
				vec -= new Vector3(0.75f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 3 && j < 2)
			{
				vec -= new Vector3(0.75f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 5 && j < 2)
			{
				vec -= new Vector3(0.75f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 4 && j > 1)
			{
				vec -= new Vector3(0.75f, 0f, 0f);
			}
		}
	}

	private void AddThridUV()
	{
		if (this.m_gmd.blockKind == BlockKind.ThirdOnWallSouth || this.m_gmd.blockKind == BlockKind.ThirdOnWallNorth)
		{
			if (this.m_gmd.blockFace == BlockFace.Top || this.m_gmd.blockFace == BlockFace.Bottom)
			{
				float[] array = new float[4];
				array[1] = 0.75f;
				array[2] = 0.75f;
				this.m_gmd.uvY = array;
			}
			if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex < 2)
			{
				this.m_gmd.uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.75f
				};
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					0.75f,
					1f
				};
			}
		}
		if (this.m_gmd.blockKind == BlockKind.ThirdOnWallWest || this.m_gmd.blockKind == BlockKind.ThirdOnWallEast)
		{
			if (this.m_gmd.blockFace == BlockFace.Top || this.m_gmd.blockFace == BlockFace.Bottom)
			{
				this.m_gmd.uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.75f
				};
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					0.75f,
					1f
				};
			}
			if (this.m_gmd.blockFace == BlockFace.Side && this.m_gmd.sideIndex > 1 && this.m_gmd.sideIndex < 4)
			{
				this.m_gmd.uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.75f
				};
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					0.75f,
					1f
				};
			}
		}
		if (this.m_gmd.blockKind == BlockKind.Third && this.m_gmd.blockFace == BlockFace.Side)
		{
			float[] uvY;
			if (!this.m_gmd.flip)
			{
				float[] array2 = new float[4];
				array2[1] = 0.75f;
				uvY = array2;
				array2[2] = 0.75f;
			}
			else
			{
				float[] array3 = new float[4];
				array3[0] = 0.75f;
				uvY = array3;
				array3[3] = 0.75f;
			}
			this.m_gmd.uvY = uvY;
		}
	}

	private void AddThrid(int j, ref Vector3 vec)
	{
		if (this.m_gmd.blockKind == BlockKind.Third)
		{
			if (!this.m_gmd.flip)
			{
				if ((this.m_gmd.sideIndex < 4 && (j == 1 || j == 2)) || this.m_gmd.sideIndex == 4)
				{
					vec -= new Vector3(0f, 0f, 0.25f);
				}
			}
			else if ((this.m_gmd.sideIndex < 4 && (j == 0 || j == 3)) || this.m_gmd.sideIndex == 5)
			{
				vec += new Vector3(0f, 0f, 0.25f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.ThirdOnWallSouth)
		{
			if (this.m_gmd.sideIndex == 0 && j > 1)
			{
				vec += new Vector3(0f, 0.25f, 0f);
			}
			if (this.m_gmd.sideIndex == 1 && j < 2)
			{
				vec += new Vector3(0f, 0.25f, 0f);
			}
			if ((this.m_gmd.sideIndex == 4 && j == 3) || (this.m_gmd.sideIndex == 4 && j == 0))
			{
				vec += new Vector3(0f, 0.25f, 0f);
			}
			if (this.m_gmd.sideIndex == 2)
			{
				vec += new Vector3(0f, 0.25f, 0f);
			}
			if ((this.m_gmd.sideIndex == 5 && j == 3) || (this.m_gmd.sideIndex == 5 && j == 0))
			{
				vec += new Vector3(0f, 0.25f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.ThirdOnWallNorth)
		{
			if (this.m_gmd.sideIndex == 0 && j < 2)
			{
				vec -= new Vector3(0f, 0.25f, 0f);
			}
			if (this.m_gmd.sideIndex == 3)
			{
				vec -= new Vector3(0f, 0.25f, 0f);
			}
			if (this.m_gmd.sideIndex == 5 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.25f, 0f);
			}
			if (this.m_gmd.sideIndex == 4 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.25f, 0f);
			}
			if (this.m_gmd.sideIndex == 1 && j > 1)
			{
				vec -= new Vector3(0f, 0.25f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.ThirdOnWallEast)
		{
			if (this.m_gmd.sideIndex == 2 && j < 2)
			{
				vec += new Vector3(0.25f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 0)
			{
				vec += new Vector3(0.25f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 3 && j > 1)
			{
				vec += new Vector3(0.25f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 5 && j > 1)
			{
				vec += new Vector3(0.25f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 4 && j < 2)
			{
				vec += new Vector3(0.25f, 0f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.ThirdOnWallWest)
		{
			if (this.m_gmd.sideIndex == 2 && j > 1)
			{
				vec -= new Vector3(0.25f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 1)
			{
				vec -= new Vector3(0.25f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 3 && j < 2)
			{
				vec -= new Vector3(0.25f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 5 && j < 2)
			{
				vec -= new Vector3(0.25f, 0f, 0f);
			}
			if (this.m_gmd.sideIndex == 4 && j > 1)
			{
				vec -= new Vector3(0.25f, 0f, 0f);
			}
		}
	}

	private void AddDiagonalUV()
	{
		if (this.m_gmd.blockKind.IsDiagonal() && this.m_gmd.blockFace == BlockFace.Side)
		{
			int[] uvnumber = DiagonalAssociation.GetUVNumber(this.m_gmd.blockKind);
			if ((this.m_gmd.blockKind == BlockKind.DiagonalEast || this.m_gmd.blockKind == BlockKind.DiagonalNorth || this.m_gmd.blockKind == BlockKind.DiagonalSouth || this.m_gmd.blockKind == BlockKind.DiagonalWest || this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthBottom || this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthBottom || this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastBottom || this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestBottom) && !this.m_gmd.flip && this.m_gmd.blockKind != BlockKind.DiagonalOnWallSouthBottom && this.m_gmd.blockKind != BlockKind.DiagonalOnWallNorthBottom && this.m_gmd.blockKind != BlockKind.DiagonalOnWallEastBottom && this.m_gmd.blockKind != BlockKind.DiagonalOnWallWestBottom && this.m_gmd.sideIndex == uvnumber[0])
			{
				float[] array = new float[4];
				array[1] = 1f;
				array[2] = 1f;
				this.m_gmd.uvY = array;
			}
		}
		else if ((this.m_gmd.blockFace == BlockFace.Top || this.m_gmd.blockFace == BlockFace.Bottom) && (this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestRight || this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestLeft || this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastLeft || this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastRight || this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthRight))
		{
			if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestRight && this.m_gmd.blockFace == BlockFace.Top)
			{
				this.m_gmd.uvY = new float[]
				{
					0f,
					0f,
					0f,
					1f
				};
			}
			if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestLeft && this.m_gmd.blockFace == BlockFace.Bottom)
			{
				float[] array2 = new float[4];
				array2[1] = 1f;
				this.m_gmd.uvY = array2;
			}
			if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastLeft && this.m_gmd.blockFace == BlockFace.Top)
			{
				float[] array3 = new float[4];
				array3[0] = 1f;
				this.m_gmd.uvY = array3;
			}
			if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastLeft && this.m_gmd.blockFace == BlockFace.Bottom)
			{
				this.m_gmd.uvY = new float[]
				{
					0f,
					0f,
					0f,
					1f
				};
			}
			if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastRight && this.m_gmd.blockFace == BlockFace.Top)
			{
				float[] array4 = new float[4];
				array4[1] = 1f;
				this.m_gmd.uvY = array4;
			}
			if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthRight && this.m_gmd.blockFace == BlockFace.Bottom)
			{
				this.m_gmd.uvY = new float[]
				{
					0f,
					0f,
					0f,
					1f
				};
			}
		}
	}

	private void AddDiagonal(int j, ref Vector3 vec)
	{
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthTop)
		{
			this.m_gmd.flip = false;
		}
		DiagonalAssociation association = DiagonalAssociation.GetAssociation(this.m_gmd.blockKind);
		if (this.m_gmd.blockKind == BlockKind.DiagonalWest || this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastBottom || this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastTop)
		{
			if (!this.m_gmd.flip && this.m_gmd.blockKind != BlockKind.DiagonalOnWallEastBottom)
			{
				if (this.m_gmd.sideIndex == association.I[0] && (j == association.J[0] || j == association.J[1]))
				{
					vec += Vector3.left;
				}
				else if (this.m_gmd.sideIndex == association.I[1] && j == association.J[1])
				{
					vec += Vector3.left;
				}
				else if (this.m_gmd.sideIndex == association.I[2] && j == association.J[0])
				{
					vec += Vector3.left;
				}
				else if (this.m_gmd.sideIndex == 4 && (j == association.J[2] || j == association.J[3]))
				{
					vec += Vector3.left;
				}
			}
			else if (this.m_gmd.sideIndex == association.I[0] && (j == association.FlipJ[0] || j == association.FlipJ[1]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
			else if (this.m_gmd.sideIndex == association.I[1] && j == association.FlipJ[1])
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[2] && j == association.FlipJ[0])
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 5 && (j == association.FlipJ[2] || j == association.FlipJ[3]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalEast || this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestBottom || this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestTop)
		{
			if (!this.m_gmd.flip && this.m_gmd.blockKind != BlockKind.DiagonalOnWallWestBottom)
			{
				if (this.m_gmd.sideIndex == association.I[0] && (j == association.J[0] || j == association.J[1]))
				{
					vec += Vector3.right;
				}
				else if (this.m_gmd.sideIndex == association.I[1] && j == association.J[0])
				{
					vec += Vector3.right;
				}
				else if (this.m_gmd.sideIndex == association.I[2] && j == association.J[1])
				{
					vec += Vector3.right;
				}
				else if (this.m_gmd.sideIndex == 4 && (j == association.J[2] || j == association.J[3]))
				{
					vec += Vector3.right;
				}
			}
			else if (this.m_gmd.sideIndex == association.I[0] && (j == association.FlipJ[0] || j == association.FlipJ[1]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
			else if (this.m_gmd.sideIndex == association.I[1] && j == association.FlipJ[1])
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[2] && j == association.FlipJ[0])
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 5 && (j == association.FlipJ[2] || j == association.FlipJ[3]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalSouth || this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthBottom || this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthTop)
		{
			if (!this.m_gmd.flip && this.m_gmd.blockKind != BlockKind.DiagonalOnWallSouthBottom)
			{
				if (this.m_gmd.sideIndex == association.I[0] && (j == association.J[0] || j == association.J[1]))
				{
					vec += Vector3.up;
				}
				else if (this.m_gmd.sideIndex == association.I[1] && j == association.J[0])
				{
					vec += Vector3.up;
				}
				else if (this.m_gmd.sideIndex == association.I[2] && j == association.J[1])
				{
					vec += Vector3.up;
				}
				else if (this.m_gmd.sideIndex == 4 && (j == association.J[2] || j == association.J[3]))
				{
					vec += Vector3.up;
				}
			}
			else if (this.m_gmd.sideIndex == association.I[0] && (j == association.FlipJ[0] || j == association.FlipJ[1]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
			else if (this.m_gmd.sideIndex == association.I[1] && j == association.FlipJ[1])
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[2] && j == association.FlipJ[0])
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == 5 && (j == association.FlipJ[2] || j == association.FlipJ[3]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalNorth || this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthBottom || this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthTop)
		{
			if (!this.m_gmd.flip && this.m_gmd.blockKind != BlockKind.DiagonalOnWallNorthBottom)
			{
				if (this.m_gmd.sideIndex == association.I[0] && (j == association.J[0] || j == association.J[1]))
				{
					vec += Vector3.down;
				}
				else if (this.m_gmd.sideIndex == association.I[1] && j == association.J[0])
				{
					vec += Vector3.down;
				}
				else if (this.m_gmd.sideIndex == association.I[2] && j == association.J[1])
				{
					vec += Vector3.down;
				}
				else if (this.m_gmd.sideIndex == 4 && (j == association.J[2] || j == association.J[3]))
				{
					vec += Vector3.down;
				}
			}
			else if (this.m_gmd.sideIndex == association.I[0] && (j == association.FlipJ[0] || j == association.FlipJ[1]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
			else if (this.m_gmd.sideIndex == association.I[1] && j == association.FlipJ[1])
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[2] && j == association.FlipJ[0])
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (this.m_gmd.sideIndex == 5 && (j == association.FlipJ[2] || j == association.FlipJ[3]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastLeft)
		{
			if (this.m_gmd.sideIndex == association.I[0] && (j == association.J[0] || j == association.J[1]))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[1] && j == 3)
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[2] && j == association.J[0])
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == 2 && (j == association.J[2] || j == association.J[3]))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestLeft)
		{
			if (this.m_gmd.sideIndex == association.I[0] && (j == association.J[0] || j == association.J[1]))
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[1] && j == association.J[0])
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[2] && j == 2)
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (this.m_gmd.sideIndex == 3 && (j == association.J[2] || j == association.J[3]))
			{
				vec += new Vector3(0f, -1f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthLeft)
		{
			if (this.m_gmd.sideIndex == 2 && (j == 0 || j == 1))
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 4 && j == 0)
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 5 && j == 3)
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 0 && (j == 2 || j == 3))
			{
				vec += new Vector3(1f, 0f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthLeft)
		{
			if (this.m_gmd.sideIndex == 3 && (j == 0 || j == 1))
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 4 && j == 2)
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 5 && j == 1)
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 1 && (j == 2 || j == 3))
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastRight)
		{
			if (this.m_gmd.sideIndex == association.I[0] && (j == association.J[0] || j == association.J[1]))
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[1] && j == association.J[0])
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[2] && j == 1)
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (this.m_gmd.sideIndex == 3 && (j == association.J[2] || j == association.J[3]))
			{
				vec += new Vector3(0f, -1f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestRight)
		{
			if (this.m_gmd.sideIndex == association.I[0] && (j == association.J[0] || j == association.J[1]))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[1] && j == 0)
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == association.I[2] && j == association.J[1])
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == 2 && (j == association.J[2] || j == association.J[3]))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthRight)
		{
			if (this.m_gmd.sideIndex == 2 && (j == 2 || j == 3))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == 4 && j == 3)
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == 5 && j == 0)
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (this.m_gmd.sideIndex == 1 && (j == 0 || j == 1))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthRight)
		{
			if (this.m_gmd.sideIndex == 3 && (j == 2 || j == 3))
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 4 && j == 1)
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 5 && j == 2)
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (this.m_gmd.sideIndex == 0 && (j == 0 || j == 1))
			{
				vec += new Vector3(1f, 0f, 0f);
			}
		}
	}

	private void AddCornerUV()
	{
		this.m_gmd.uvX = new float[]
		{
			0f,
			0f,
			1f,
			1f
		};
		float[] array = new float[4];
		array[1] = 1f;
		array[2] = 1f;
		this.m_gmd.uvY = array;
		switch (this.m_gmd.blockKind)
		{
		case BlockKind.CornerEast:
			if (!this.m_gmd.flip)
			{
				if (this.m_gmd.sideIndex == 2 || this.m_gmd.sideIndex == 0)
				{
					float[] array2 = new float[4];
					array2[0] = 1f;
					array2[1] = 1f;
					this.m_gmd.uvX = array2;
					float[] array3 = new float[4];
					array3[1] = 1f;
					this.m_gmd.uvY = array3;
				}
			}
			else if (this.m_gmd.sideIndex == 3 || this.m_gmd.sideIndex == 1)
			{
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					1f,
					1f
				};
				this.m_gmd.uvY = new float[]
				{
					0f,
					0f,
					0f,
					1f
				};
			}
			break;
		case BlockKind.CornerWest:
			if (!this.m_gmd.flip)
			{
				if (this.m_gmd.sideIndex == 3 || this.m_gmd.sideIndex == 0)
				{
					float[] array4 = new float[4];
					array4[0] = 1f;
					array4[1] = 1f;
					this.m_gmd.uvX = array4;
					float[] array5 = new float[4];
					array5[1] = 1f;
					this.m_gmd.uvY = array5;
				}
			}
			else if (this.m_gmd.sideIndex == 2 || this.m_gmd.sideIndex == 1)
			{
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					1f,
					1f
				};
				this.m_gmd.uvY = new float[]
				{
					0f,
					0f,
					0f,
					1f
				};
			}
			break;
		case BlockKind.CornerNorth:
			if (!this.m_gmd.flip)
			{
				if (this.m_gmd.sideIndex == 2 || this.m_gmd.sideIndex == 1)
				{
					float[] array6 = new float[4];
					array6[0] = 1f;
					array6[1] = 1f;
					this.m_gmd.uvX = array6;
					float[] array7 = new float[4];
					array7[1] = 1f;
					this.m_gmd.uvY = array7;
				}
			}
			else if (this.m_gmd.sideIndex == 3 || this.m_gmd.sideIndex == 0)
			{
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					1f,
					1f
				};
				this.m_gmd.uvY = new float[]
				{
					0f,
					0f,
					0f,
					1f
				};
			}
			break;
		case BlockKind.CornerSouth:
			if (!this.m_gmd.flip)
			{
				if (this.m_gmd.sideIndex == 3 || this.m_gmd.sideIndex == 1)
				{
					float[] array8 = new float[4];
					array8[0] = 1f;
					array8[1] = 1f;
					this.m_gmd.uvX = array8;
					float[] array9 = new float[4];
					array9[1] = 1f;
					this.m_gmd.uvY = array9;
				}
			}
			else if (this.m_gmd.sideIndex == 2 || this.m_gmd.sideIndex == 0)
			{
				this.m_gmd.uvX = new float[]
				{
					0f,
					0f,
					1f,
					1f
				};
				this.m_gmd.uvY = new float[]
				{
					0f,
					0f,
					0f,
					1f
				};
			}
			break;
		}
	}

	private void AddCorner(int j, ref Vector3 vec)
	{
		switch (this.m_gmd.blockKind)
		{
		case BlockKind.CornerEast:
			if (!this.m_gmd.flip)
			{
				if (this.m_gmd.blockFace == BlockFace.Top)
				{
					if (j == 0)
					{
						vec = new Vector3(0f, 1f, 0f);
					}
					if (j == 2)
					{
						vec = new Vector3(-1f, 0f, 0f);
					}
					if (j == 3)
					{
						vec = new Vector3(-1f, 1f, 0f);
					}
				}
				else if (this.m_gmd.sideIndex == 1)
				{
					if (j == 2)
					{
						vec = new Vector3(-1f, 0f, 0f);
					}
					if (j == 1)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 2)
				{
					if (j == 1)
					{
						vec = new Vector3(0f, 1f, 0f);
					}
					if (j == 2)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 0)
				{
					if (j == 2)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 3 && j == 1)
				{
					vec = new Vector3(0f, 0f, -1f);
				}
			}
			else if (this.m_gmd.blockFace == BlockFace.Bottom)
			{
				if (j == 0)
				{
					vec = new Vector3(0f, 1f, 0f);
				}
				if (j == 2)
				{
					vec = new Vector3(-1f, 0f, 0f);
				}
				if (j == 3)
				{
					vec = new Vector3(-1f, 1f, 0f);
				}
			}
			else if (this.m_gmd.sideIndex == 1)
			{
				if (j == 3)
				{
					vec = new Vector3(-1f, 0f, 0f);
				}
				if (j == 0)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 2)
			{
				if (j == 0)
				{
					vec = new Vector3(0f, 1f, 0f);
				}
				if (j == 3)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 0)
			{
				if (j == 3)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 3 && j == 0)
			{
				vec = new Vector3(0f, 0f, 1f);
			}
			break;
		case BlockKind.CornerWest:
			if (!this.m_gmd.flip)
			{
				if (this.m_gmd.blockFace == BlockFace.Top)
				{
					if (j == 1)
					{
						vec = new Vector3(1f, 0f, 0f);
					}
					if (j == 0)
					{
						vec = new Vector3(1f, 1f, 0f);
					}
					if (j == 3)
					{
						vec = new Vector3(0f, 1f, 0f);
					}
				}
				else if (this.m_gmd.sideIndex == 1)
				{
					if (j == 1)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 3)
				{
					if (j == 2)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 2)
				{
					if (j == 2)
					{
						vec = new Vector3(0f, 1f, 0f);
					}
					if (j == 1)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 0)
				{
					if (j == 1)
					{
						vec = new Vector3(1f, 0f, 0f);
					}
					if (j == 2)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
			}
			else if (this.m_gmd.blockFace == BlockFace.Bottom)
			{
				if (j == 1)
				{
					vec = new Vector3(1f, 0f, 0f);
				}
				if (j == 0)
				{
					vec = new Vector3(1f, 1f, 0f);
				}
				if (j == 3)
				{
					vec = new Vector3(0f, 1f, 0f);
				}
			}
			else if (this.m_gmd.sideIndex == 1)
			{
				if (j == 0)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 3)
			{
				if (j == 3)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 2)
			{
				if (j == 3)
				{
					vec = new Vector3(0f, 1f, 0f);
				}
				if (j == 0)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 0)
			{
				if (j == 0)
				{
					vec = new Vector3(1f, 0f, 0f);
				}
				if (j == 3)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			break;
		case BlockKind.CornerNorth:
			if (!this.m_gmd.flip)
			{
				if (this.m_gmd.blockFace == BlockFace.Top)
				{
					if (j == 1)
					{
						vec = new Vector3(0f, -1f, 0f);
					}
					if (j == 2)
					{
						vec = new Vector3(-1f, -1f, 0f);
					}
					if (j == 3)
					{
						vec = new Vector3(-1f, 0f, 0f);
					}
				}
				else if (this.m_gmd.sideIndex == 0)
				{
					if (j == 1)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 2)
				{
					if (j == 2)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 1)
				{
					if (j == 1)
					{
						vec = new Vector3(-1f, 0f, 0f);
					}
					if (j == 2)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 3)
				{
					if (j == 2)
					{
						vec = new Vector3(0f, -1f, 0f);
					}
					if (j == 1)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
			}
			else if (this.m_gmd.blockFace == BlockFace.Bottom)
			{
				if (j == 1)
				{
					vec = new Vector3(0f, -1f, 0f);
				}
				if (j == 2)
				{
					vec = new Vector3(-1f, -1f, 0f);
				}
				if (j == 3)
				{
					vec = new Vector3(-1f, 0f, 0f);
				}
			}
			else if (this.m_gmd.sideIndex == 0)
			{
				if (j == 0)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 2)
			{
				if (j == 3)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 1)
			{
				if (j == 0)
				{
					vec = new Vector3(-1f, 0f, 0f);
				}
				if (j == 3)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 3)
			{
				if (j == 3)
				{
					vec = new Vector3(0f, -1f, 0f);
				}
				if (j == 0)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			break;
		case BlockKind.CornerSouth:
			if (!this.m_gmd.flip)
			{
				if (this.m_gmd.blockFace == BlockFace.Top)
				{
					if (j == 0)
					{
						vec = new Vector3(1f, 0f, 0f);
					}
					if (j == 1)
					{
						vec = new Vector3(1f, -1f, 0f);
					}
					if (j == 2)
					{
						vec = new Vector3(0f, -1f, 0f);
					}
				}
				else if (this.m_gmd.sideIndex == 1)
				{
					if (j == 2)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 2)
				{
					if (j == 1)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 0)
				{
					if (j == 2)
					{
						vec = new Vector3(1f, 0f, 0f);
					}
					if (j == 1)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
				else if (this.m_gmd.sideIndex == 3)
				{
					if (j == 1)
					{
						vec = new Vector3(0f, -1f, 0f);
					}
					if (j == 2)
					{
						vec = new Vector3(0f, 0f, -1f);
					}
				}
			}
			else if (this.m_gmd.blockFace == BlockFace.Bottom)
			{
				if (j == 0)
				{
					vec = new Vector3(1f, 0f, 0f);
				}
				if (j == 1)
				{
					vec = new Vector3(1f, -1f, 0f);
				}
				if (j == 2)
				{
					vec = new Vector3(0f, -1f, 0f);
				}
			}
			else if (this.m_gmd.sideIndex == 1)
			{
				if (j == 3)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 2)
			{
				if (j == 0)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 0)
			{
				if (j == 3)
				{
					vec = new Vector3(1f, 0f, 0f);
				}
				if (j == 0)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			else if (this.m_gmd.sideIndex == 3)
			{
				if (j == 0)
				{
					vec = new Vector3(0f, -1f, 0f);
				}
				if (j == 3)
				{
					vec = new Vector3(0f, 0f, 1f);
				}
			}
			break;
		}
	}

	private static readonly object _object = new object();

	private readonly IBatchProcessor<Chunk> batchProcessor;

	private readonly WorldData worldData;

	private readonly IntVect[][] offsets = new IntVect[][]
	{
		new IntVect[]
		{
			new IntVect(0, 0, 0),
			new IntVect(0, 0, 1),
			new IntVect(0, 1, 1),
			new IntVect(0, 1, 0)
		},
		new IntVect[]
		{
			new IntVect(1, 1, 0),
			new IntVect(1, 1, 1),
			new IntVect(1, 0, 1),
			new IntVect(1, 0, 0)
		},
		new IntVect[]
		{
			new IntVect(1, 0, 0),
			new IntVect(1, 0, 1),
			new IntVect(0, 0, 1),
			new IntVect(0, 0, 0)
		},
		new IntVect[]
		{
			new IntVect(0, 1, 0),
			new IntVect(0, 1, 1),
			new IntVect(1, 1, 1),
			new IntVect(1, 1, 0)
		},
		new IntVect[]
		{
			new IntVect(0, 0, 0),
			new IntVect(0, 1, 0),
			new IntVect(1, 1, 0),
			new IntVect(1, 0, 0)
		},
		new IntVect[]
		{
			new IntVect(1, 0, 1),
			new IntVect(1, 1, 1),
			new IntVect(0, 1, 1),
			new IntVect(0, 0, 1)
		}
	};

	private readonly IntVect[][] formOffsets = new IntVect[][]
	{
		new IntVect[]
		{
			new IntVect(0, 1, 0),
			new IntVect(0, 1, 1),
			new IntVect(0, 0, 1),
			new IntVect(0, 0, 0)
		},
		new IntVect[]
		{
			new IntVect(1, 0, 0),
			new IntVect(1, 0, 1),
			new IntVect(1, 1, 1),
			new IntVect(1, 1, 0)
		},
		new IntVect[]
		{
			new IntVect(0, 0, 0),
			new IntVect(0, 0, 1),
			new IntVect(1, 0, 1),
			new IntVect(1, 0, 0)
		},
		new IntVect[]
		{
			new IntVect(1, 1, 0),
			new IntVect(1, 1, 1),
			new IntVect(0, 1, 1),
			new IntVect(0, 1, 0)
		},
		new IntVect[]
		{
			new IntVect(0, 0, 1),
			new IntVect(0, 1, 1),
			new IntVect(1, 1, 1),
			new IntVect(1, 0, 1)
		},
		new IntVect[]
		{
			new IntVect(1, 0, 0),
			new IntVect(1, 1, 0),
			new IntVect(0, 1, 0),
			new IntVect(0, 0, 0)
		}
	};

	private readonly Vector3[][] offsetsFence = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0.35f, 0.65f, 0f),
			new Vector3(0.35f, 0.65f, 1f),
			new Vector3(0.35f, 0.35f, 1f),
			new Vector3(0.35f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0.35f, 0f),
			new Vector3(0.65f, 0.35f, 1f),
			new Vector3(0.65f, 0.65f, 1f),
			new Vector3(0.65f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0.35f, 0f),
			new Vector3(0.35f, 0.35f, 1f),
			new Vector3(0.65f, 0.35f, 1f),
			new Vector3(0.65f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0.65f, 0f),
			new Vector3(0.65f, 0.65f, 1f),
			new Vector3(0.35f, 0.65f, 1f),
			new Vector3(0.35f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0.35f, 0f),
			new Vector3(0.65f, 0.65f, 0f),
			new Vector3(0.35f, 0.65f, 0f),
			new Vector3(0.35f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0.35f, 1f),
			new Vector3(0.35f, 0.65f, 1f),
			new Vector3(0.65f, 0.65f, 1f),
			new Vector3(0.65f, 0.35f, 1f)
		}
	};

	private readonly Vector3[][] offsetsFenceFirst = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0.35f, 0f, 1f),
			new Vector3(0.35f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0f, 0f),
			new Vector3(0.35f, 0f, 1f),
			new Vector3(0.35f, 0.35f, 1f),
			new Vector3(0.35f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0.35f, 0f),
			new Vector3(0.35f, 0.35f, 1f),
			new Vector3(0f, 0.35f, 1f),
			new Vector3(0f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0f),
			new Vector3(0f, 0.35f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0f, 0f),
			new Vector3(0.35f, 0.35f, 0f),
			new Vector3(0f, 0.35f, 0f),
			new Vector3(0f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0.35f, 1f),
			new Vector3(0.35f, 0.35f, 1f),
			new Vector3(0.35f, 0f, 1f)
		}
	};

	private readonly Vector3[][] offsetsFenceTwo = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 0f),
			new Vector3(1f, 0.35f, 1f),
			new Vector3(0.65f, 0.35f, 1f),
			new Vector3(0.65f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 0.35f, 1f),
			new Vector3(1f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0f, 0f),
			new Vector3(0.65f, 0f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0.35f, 0f),
			new Vector3(0.65f, 0.35f, 1f),
			new Vector3(0.65f, 0f, 1f),
			new Vector3(0.65f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0.35f, 0f),
			new Vector3(0.65f, 0.35f, 0f),
			new Vector3(0.65f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0f, 1f),
			new Vector3(0.65f, 0.35f, 1f),
			new Vector3(1f, 0.35f, 1f),
			new Vector3(1f, 0f, 1f)
		}
	};

	private readonly Vector3[][] offsetsFenceThree = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, 1f, 1f),
			new Vector3(0.65f, 1f, 1f),
			new Vector3(0.65f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0.65f, 0f),
			new Vector3(0.65f, 0.65f, 1f),
			new Vector3(1f, 0.65f, 1f),
			new Vector3(1f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.65f, 0f),
			new Vector3(1f, 0.65f, 1f),
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 1f, 0f),
			new Vector3(0.65f, 1f, 1f),
			new Vector3(0.65f, 0.65f, 1f),
			new Vector3(0.65f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.65f, 0f),
			new Vector3(1f, 1f, 0f),
			new Vector3(0.65f, 1f, 0f),
			new Vector3(0.65f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0.65f, 1f),
			new Vector3(0.65f, 1f, 1f),
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 0.65f, 1f)
		}
	};

	private readonly Vector3[][] offsetsFenceFour = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0f, 0.65f, 1f),
			new Vector3(0f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0.65f, 0f),
			new Vector3(0.35f, 0.65f, 1f),
			new Vector3(0.35f, 1f, 1f),
			new Vector3(0.35f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 1f, 0f),
			new Vector3(0.35f, 1f, 1f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.65f, 0f),
			new Vector3(0f, 0.65f, 1f),
			new Vector3(0.35f, 0.65f, 1f),
			new Vector3(0.35f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0.65f, 0f),
			new Vector3(0.35f, 1f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.65f, 1f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0.35f, 1f, 1f),
			new Vector3(0.35f, 0.65f, 1f)
		}
	};

	private readonly Vector3[][] offsetsFenceEastWest = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 0.65f, 0.35f),
			new Vector3(0f, 0.65f, 0.65f),
			new Vector3(0f, 0.35f, 0.65f),
			new Vector3(0f, 0.35f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 0.35f),
			new Vector3(1f, 0.35f, 0.65f),
			new Vector3(1f, 0.65f, 0.65f),
			new Vector3(1f, 0.65f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0.35f),
			new Vector3(0f, 0.35f, 0.65f),
			new Vector3(1f, 0.35f, 0.65f),
			new Vector3(1f, 0.35f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.65f, 0.35f),
			new Vector3(1f, 0.65f, 0.65f),
			new Vector3(0f, 0.65f, 0.65f),
			new Vector3(0f, 0.65f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0.65f),
			new Vector3(0f, 0.65f, 0.65f),
			new Vector3(1f, 0.65f, 0.65f),
			new Vector3(1f, 0.35f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 0.35f),
			new Vector3(1f, 0.65f, 0.35f),
			new Vector3(0f, 0.65f, 0.35f),
			new Vector3(0f, 0.35f, 0.35f)
		}
	};

	private readonly Vector3[][] offsetsFenceSouthNorth = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0.35f, 1f, 0.35f),
			new Vector3(0.35f, 1f, 0.65f),
			new Vector3(0.35f, 0f, 0.65f),
			new Vector3(0.35f, 0f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0f, 0.35f),
			new Vector3(0.65f, 0f, 0.65f),
			new Vector3(0.65f, 1f, 0.65f),
			new Vector3(0.65f, 1f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0f, 0.35f),
			new Vector3(0.35f, 0f, 0.65f),
			new Vector3(0.65f, 0f, 0.65f),
			new Vector3(0.65f, 0f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 1f, 0.35f),
			new Vector3(0.65f, 1f, 0.65f),
			new Vector3(0.35f, 1f, 0.65f),
			new Vector3(0.35f, 1f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0f, 0.65f),
			new Vector3(0.35f, 1f, 0.65f),
			new Vector3(0.65f, 1f, 0.65f),
			new Vector3(0.65f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0f, 0.35f),
			new Vector3(0.65f, 1f, 0.35f),
			new Vector3(0.35f, 1f, 0.35f),
			new Vector3(0.35f, 0f, 0.35f)
		}
	};

	private readonly Vector3[][] offsetsEastFenceEast = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0f),
			new Vector3(0f, 0.35f, 0.35f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(0f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(1f, 0.35f, 0.35f),
			new Vector3(1f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(1f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 0f),
			new Vector3(1f, 0.35f, 0.35f),
			new Vector3(0f, 0.35f, 0.35f),
			new Vector3(0f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 0.35f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(0f, 0.35f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0f),
			new Vector3(0f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0.35f, 0f)
		}
	};

	private readonly Vector3[][] offsetsEastFenceWest = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0.35f),
			new Vector3(0f, 0.65f, 0.35f),
			new Vector3(0f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.65f, 0f),
			new Vector3(1f, 0.65f, 0.35f),
			new Vector3(1f, 1f, 0.35f),
			new Vector3(1f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.65f, 0f),
			new Vector3(0f, 0.65f, 0.35f),
			new Vector3(1f, 0.65f, 0.35f),
			new Vector3(1f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, 1f, 0.35f),
			new Vector3(0f, 1f, 0.35f),
			new Vector3(0f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0.35f),
			new Vector3(1f, 0.65f, 0.35f),
			new Vector3(0f, 0.65f, 0.35f),
			new Vector3(0f, 1f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 0.65f, 0f),
			new Vector3(1f, 0.65f, 0f),
			new Vector3(1f, 1f, 0f)
		}
	};

	private readonly Vector3[][] offsetsEastFenceNorth = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 1f, 0.65f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0f, 0.65f, 1f),
			new Vector3(0f, 0.65f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.65f, 0.65f),
			new Vector3(1f, 0.65f, 1f),
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.65f, 0.65f),
			new Vector3(0f, 0.65f, 1f),
			new Vector3(1f, 0.65f, 1f),
			new Vector3(1f, 0.65f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0.65f),
			new Vector3(1f, 1f, 1f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 0.65f, 1f),
			new Vector3(0f, 0.65f, 1f),
			new Vector3(0f, 1f, 1f)
		},
		new Vector3[]
		{
			new Vector3(0f, 1f, 0.65f),
			new Vector3(0f, 0.65f, 0.65f),
			new Vector3(1f, 0.65f, 0.65f),
			new Vector3(1f, 1f, 0.65f)
		}
	};

	private readonly Vector3[][] offsetsEastFenceSouth = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0.65f),
			new Vector3(0f, 0.35f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0f, 0.65f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 0.35f, 1f),
			new Vector3(1f, 0.35f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0f, 0.65f),
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 0.65f),
			new Vector3(1f, 0.35f, 1f),
			new Vector3(0f, 0.35f, 1f),
			new Vector3(0f, 0.35f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0.35f, 1f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0.65f),
			new Vector3(0f, 0f, 0.65f),
			new Vector3(1f, 0f, 0.65f),
			new Vector3(1f, 0.35f, 0.65f)
		}
	};

	private readonly Vector3[][] offsetsWestFenceEast = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0f),
			new Vector3(0f, 0.35f, 0.35f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(0f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(1f, 0.35f, 0.35f),
			new Vector3(1f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(1f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 0f),
			new Vector3(1f, 0.35f, 0.35f),
			new Vector3(0f, 0.35f, 0.35f),
			new Vector3(0f, 0.35f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 0.35f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(0f, 0.35f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0f),
			new Vector3(0f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0.35f, 0f)
		}
	};

	private readonly Vector3[][] offsetsWestFenceWest = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0.35f),
			new Vector3(0f, 0.65f, 0.35f),
			new Vector3(0f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.65f, 0f),
			new Vector3(1f, 0.65f, 0.35f),
			new Vector3(1f, 1f, 0.35f),
			new Vector3(1f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.65f, 0f),
			new Vector3(0f, 0.65f, 0.35f),
			new Vector3(1f, 0.65f, 0.35f),
			new Vector3(1f, 0.65f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, 1f, 0.35f),
			new Vector3(0f, 1f, 0.35f),
			new Vector3(0f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0.35f),
			new Vector3(1f, 0.65f, 0.35f),
			new Vector3(0f, 0.65f, 0.35f),
			new Vector3(0f, 1f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 0.65f, 0f),
			new Vector3(1f, 0.65f, 0f),
			new Vector3(1f, 1f, 0f)
		}
	};

	private readonly Vector3[][] offsetsWestFenceNorth = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 1f, 0.65f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0f, 0.65f, 1f),
			new Vector3(0f, 0.65f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.65f, 0.65f),
			new Vector3(1f, 0.65f, 1f),
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.65f, 0.65f),
			new Vector3(0f, 0.65f, 1f),
			new Vector3(1f, 0.65f, 1f),
			new Vector3(1f, 0.65f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0.65f),
			new Vector3(1f, 1f, 1f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 0.65f, 1f),
			new Vector3(0f, 0.65f, 1f),
			new Vector3(0f, 1f, 1f)
		},
		new Vector3[]
		{
			new Vector3(0f, 1f, 0.65f),
			new Vector3(0f, 0.65f, 0.65f),
			new Vector3(1f, 0.65f, 0.65f),
			new Vector3(1f, 1f, 0.65f)
		}
	};

	private readonly Vector3[][] offsetsWestFenceSouth = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0.65f),
			new Vector3(0f, 0.35f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0f, 0.65f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 0.35f, 1f),
			new Vector3(1f, 0.35f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0f, 0.65f),
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 0.65f),
			new Vector3(1f, 0.35f, 1f),
			new Vector3(0f, 0.35f, 1f),
			new Vector3(0f, 0.35f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0.35f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0.35f, 1f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0.35f, 0.65f),
			new Vector3(0f, 0f, 0.65f),
			new Vector3(1f, 0f, 0.65f),
			new Vector3(1f, 0.35f, 0.65f)
		}
	};

	private readonly Vector3[][] offsetsNorthFenceEast = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0.65f, 1f, 0f),
			new Vector3(0.65f, 1f, 0.35f),
			new Vector3(0.65f, 0f, 0.35f),
			new Vector3(0.65f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(1f, 1f, 0.35f),
			new Vector3(1f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0f, 0f),
			new Vector3(0.65f, 0f, 0.35f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(1f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, 1f, 0.35f),
			new Vector3(0.65f, 1f, 0.35f),
			new Vector3(0.65f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0.35f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(0.65f, 0f, 0.35f),
			new Vector3(0.65f, 1f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 1f, 0f),
			new Vector3(0.65f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 1f, 0f)
		}
	};

	private readonly Vector3[][] offsetsNorthFenceWest = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0.35f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(0f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0f, 0f),
			new Vector3(0.35f, 0f, 0.35f),
			new Vector3(0.35f, 1f, 0.35f),
			new Vector3(0.35f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(0.35f, 0f, 0.35f),
			new Vector3(0.35f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 1f, 0f),
			new Vector3(0.35f, 1f, 0.35f),
			new Vector3(0f, 1f, 0.35f),
			new Vector3(0f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 1f, 0.35f),
			new Vector3(0.35f, 0f, 0.35f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(0f, 1f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 0f, 0f),
			new Vector3(0.35f, 0f, 0f),
			new Vector3(0.35f, 1f, 0f)
		}
	};

	private readonly Vector3[][] offsetsNorthFenceNorth = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 1f, 0.65f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0f, 0.65f),
			new Vector3(0.35f, 0f, 1f),
			new Vector3(0.35f, 1f, 1f),
			new Vector3(0.35f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0f, 0.65f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0.35f, 0f, 1f),
			new Vector3(0.35f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 1f, 0.65f),
			new Vector3(0.35f, 1f, 1f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 1f, 1f),
			new Vector3(0.35f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 1f, 1f)
		},
		new Vector3[]
		{
			new Vector3(0f, 1f, 0.65f),
			new Vector3(0f, 0f, 0.65f),
			new Vector3(0.35f, 0f, 0.65f),
			new Vector3(0.35f, 1f, 0.65f)
		}
	};

	private readonly Vector3[][] offsetsNorthFenceSouth = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0.65f, 1f, 0.65f),
			new Vector3(0.65f, 1f, 1f),
			new Vector3(0.65f, 0f, 1f),
			new Vector3(0.65f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0f, 0.65f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0f, 0.65f),
			new Vector3(0.65f, 0f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0.65f),
			new Vector3(1f, 1f, 1f),
			new Vector3(0.65f, 1f, 1f),
			new Vector3(0.65f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(0.65f, 0f, 1f),
			new Vector3(0.65f, 1f, 1f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 1f, 0.65f),
			new Vector3(0.65f, 0f, 0.65f),
			new Vector3(1f, 0f, 0.65f),
			new Vector3(1f, 1f, 0.65f)
		}
	};

	private readonly Vector3[][] offsetsSouthFenceEast = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0.65f, 1f, 0f),
			new Vector3(0.65f, 1f, 0.35f),
			new Vector3(0.65f, 0f, 0.35f),
			new Vector3(0.65f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(1f, 1f, 0.35f),
			new Vector3(1f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0f, 0f),
			new Vector3(0.65f, 0f, 0.35f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(1f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, 1f, 0.35f),
			new Vector3(0.65f, 1f, 0.35f),
			new Vector3(0.65f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0.35f),
			new Vector3(1f, 0f, 0.35f),
			new Vector3(0.65f, 0f, 0.35f),
			new Vector3(0.65f, 1f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 1f, 0f),
			new Vector3(0.65f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(1f, 1f, 0f)
		}
	};

	private readonly Vector3[][] offsetsSouthFenceWest = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 1f, 0.35f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(0f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0f, 0f),
			new Vector3(0.35f, 0f, 0.35f),
			new Vector3(0.35f, 1f, 0.35f),
			new Vector3(0.35f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(0.35f, 0f, 0.35f),
			new Vector3(0.35f, 0f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 1f, 0f),
			new Vector3(0.35f, 1f, 0.35f),
			new Vector3(0f, 1f, 0.35f),
			new Vector3(0f, 1f, 0f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 1f, 0.35f),
			new Vector3(0.35f, 0f, 0.35f),
			new Vector3(0f, 0f, 0.35f),
			new Vector3(0f, 1f, 0.35f)
		},
		new Vector3[]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, 0f, 0f),
			new Vector3(0.35f, 0f, 0f),
			new Vector3(0.35f, 1f, 0f)
		}
	};

	private readonly Vector3[][] offsetsSouthFenceNorth = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0f, 1f, 0.65f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 0f, 0.65f),
			new Vector3(0.35f, 0f, 1f),
			new Vector3(0.35f, 1f, 1f),
			new Vector3(0.35f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0f, 0f, 0.65f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0.35f, 0f, 1f),
			new Vector3(0.35f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 1f, 0.65f),
			new Vector3(0.35f, 1f, 1f),
			new Vector3(0f, 1f, 1f),
			new Vector3(0f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0.35f, 1f, 1f),
			new Vector3(0.35f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 1f, 1f)
		},
		new Vector3[]
		{
			new Vector3(0f, 1f, 0.65f),
			new Vector3(0f, 0f, 0.65f),
			new Vector3(0.35f, 0f, 0.65f),
			new Vector3(0.35f, 1f, 0.65f)
		}
	};

	private readonly Vector3[][] offsetsSouthFenceSouth = new Vector3[][]
	{
		new Vector3[]
		{
			new Vector3(0.65f, 1f, 0.65f),
			new Vector3(0.65f, 1f, 1f),
			new Vector3(0.65f, 0f, 1f),
			new Vector3(0.65f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 0f, 0.65f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 0f, 0.65f),
			new Vector3(0.65f, 0f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(1f, 0f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 0.65f),
			new Vector3(1f, 1f, 1f),
			new Vector3(0.65f, 1f, 1f),
			new Vector3(0.65f, 1f, 0.65f)
		},
		new Vector3[]
		{
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(0.65f, 0f, 1f),
			new Vector3(0.65f, 1f, 1f)
		},
		new Vector3[]
		{
			new Vector3(0.65f, 1f, 0.65f),
			new Vector3(0.65f, 0f, 0.65f),
			new Vector3(1f, 0f, 0.65f),
			new Vector3(1f, 1f, 0.65f)
		}
	};

	private readonly OptMeshGenerator.GenerateCondition[] generateTypeAssociate = new OptMeshGenerator.GenerateCondition[]
	{
		new OptMeshGenerator.GenerateCondition(BlockComparer.IsLand),
		new OptMeshGenerator.GenerateCondition(BlockComparer.IsWater),
		new OptMeshGenerator.GenerateCondition(BlockComparer.IsGlass)
	};

	private OptMeshGenerator.GenMeshData m_gmd = default(OptMeshGenerator.GenMeshData);

	private enum GenerateType
	{
		Land,
		Water,
		Glass
	}

	private struct GenMeshData
	{
		public BlockType GetBlockType(int x, int y, int z)
		{
			return (!this.isEdge) ? this.chunk.GetBlockType(x - (this.blockX - this.chunkBlockX), y - (this.blockY - this.chunkBlockY), z - (this.blockZ - this.chunkBlockZ)) : WorldData.Instance.GetBlockType(x, y, z);
		}

		public BlockKind GetBlockKind(int x, int y, int z)
		{
			return (!this.isEdge) ? this.chunk.GetBlockKind(x - (this.blockX - this.chunkBlockX), y - (this.blockY - this.chunkBlockY), z - (this.blockZ - this.chunkBlockZ)) : WorldData.Instance.GetBlockKind(x, y, z);
		}

		public void Init()
		{
			this.vert = new Vector3[]
			{
				default(Vector3),
				default(Vector3),
				default(Vector3),
				default(Vector3)
			};
			this.uvX = new float[]
			{
				0f,
				0f,
				1f,
				1f
			};
			float[] array = new float[4];
			array[1] = 1f;
			array[2] = 1f;
			this.uvY = array;
		}

		public void Reset()
		{
			this.uvX[0] = 0f;
			this.uvX[1] = 0f;
			this.uvX[2] = 1f;
			this.uvX[3] = 1f;
			this.uvY[0] = 0f;
			this.uvY[1] = 1f;
			this.uvY[2] = 1f;
			this.uvY[3] = 0f;
		}

		public Chunk chunk;

		public Chunk.MeshData meshData;

		public OptMeshGenerator.GenerateType generateType;

		public OptMeshGenerator.GenerateCondition condition;

		public int blockX;

		public int blockY;

		public int blockZ;

		public int chunkBlockX;

		public int chunkBlockY;

		public int chunkBlockZ;

		public int index;

		public int h;

		public bool isEdge;

		public bool water;

		public bool flip;

		public byte lightAmount;

		public int sideIndex;

		public BlockFace blockFace;

		public BlockType blockType;

		public BlockKind blockKind;

		public Vector3[] vert;

		public float[] uvX;

		public float[] uvY;
	}

	private delegate bool GenerateCondition(BlockType block);
}
