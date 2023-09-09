using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MeshGeneratorSmothLight : IMeshGenerator
{
	public MeshGeneratorSmothLight(IBatchProcessor<Chunk> batchProcessor, WorldData worldData)
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

	private void GenerateMesh(Chunk.MeshData meshData, Chunk chunk, MeshGeneratorSmothLight.GenerateType typeData)
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
		object @object = MeshGeneratorSmothLight._object;
		lock (@object)
		{
			this.GenerateMesh(chunk.LandData, chunk, MeshGeneratorSmothLight.GenerateType.Land);
			this.worldData.AddFinishedLandChunk(chunk);
		}
	}

	public void GenerateWaterMesh(Chunk chunk)
	{
		object @object = MeshGeneratorSmothLight._object;
		lock (@object)
		{
			this.GenerateMesh(chunk.WaterData, chunk, MeshGeneratorSmothLight.GenerateType.Water);
			this.worldData.AddFinishedWaterChunk(chunk);
		}
	}

	public void GenerateGlassMesh(Chunk chunk)
	{
		object @object = MeshGeneratorSmothLight._object;
		lock (@object)
		{
			this.GenerateMesh(chunk.GlassData, chunk, MeshGeneratorSmothLight.GenerateType.Glass);
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
		this.m_gmd.water = (this.m_gmd.generateType == MeshGeneratorSmothLight.GenerateType.Water);
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
			this.GetFromVertexLight();
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
			this.GetVertexLight();
			this.AddBlockSideOpt(ref blockKind);
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
				this.AddBlockSideOpt(ref this.m_gmd.blockKind);
				this.m_gmd.index = this.m_gmd.index + 4;
			}
		}
		this.m_gmd.sideIndex = this.m_gmd.sideIndex + 1;
	}

	private void AddBlockSideOpt(ref BlockKind kind)
	{
		if (!kind.IsDefault())
		{
			if (kind.IsStair())
			{
				Color white = Color.white;
				Color white2 = Color.white;
				if (kind == BlockKind.StairSouth)
				{
					if (this.m_gmd.faceStair == BlockFaceStair.Left)
					{
						float r = this.m_gmd.formLeft[2].r;
						float r2 = this.m_gmd.formLeft[1].r;
						float g = this.m_gmd.formLeft[2].g;
						float g2 = this.m_gmd.formLeft[1].g;
						float r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						float g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white = new Color(r3, g3, 0f);
						r = this.m_gmd.formLeft[3].r;
						r2 = this.m_gmd.formLeft[0].r;
						g = this.m_gmd.formLeft[3].g;
						g2 = this.m_gmd.formLeft[0].g;
						r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white2 = new Color(r3, g3, 0f);
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(white2);
							r = white.r;
							r2 = white2.r;
							g = white.g;
							g2 = white2.g;
							r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
							g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
							white = new Color(r3, g3, 0f);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[1]);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white2);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Right)
					{
						float r = this.m_gmd.formRight[2].r;
						float r2 = this.m_gmd.formRight[1].r;
						float g = this.m_gmd.formRight[2].g;
						float g2 = this.m_gmd.formRight[1].g;
						float r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						float g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white = new Color(r3, g3, 0f);
						r = this.m_gmd.formRight[3].r;
						r2 = this.m_gmd.formRight[0].r;
						g = this.m_gmd.formRight[3].g;
						g2 = this.m_gmd.formRight[0].g;
						r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white2 = new Color(r3, g3, 0f);
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
							r = white.r;
							r2 = white2.r;
							g = white.g;
							g2 = white2.g;
							r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
							g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
							white = new Color(r3, g3, 0f);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white2);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(white2);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Back)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Forward)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Top)
					{
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Bottom)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
					}
				}
				else if (kind == BlockKind.StairNorth)
				{
					if (this.m_gmd.faceStair == BlockFaceStair.Left)
					{
						float r = this.m_gmd.formRight[2].r;
						float r2 = this.m_gmd.formRight[1].r;
						float g = this.m_gmd.formRight[2].g;
						float g2 = this.m_gmd.formRight[1].g;
						float r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						float g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white = new Color(r3, g3, 0f);
						r = this.m_gmd.formRight[3].r;
						r2 = this.m_gmd.formRight[0].r;
						g = this.m_gmd.formRight[3].g;
						g2 = this.m_gmd.formRight[0].g;
						r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white2 = new Color(r3, g3, 0f);
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(white2);
							r = white.r;
							r2 = white2.r;
							g = white.g;
							g2 = white2.g;
							r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
							g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
							white = new Color(r3, g3, 0f);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[1]);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white2);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Right)
					{
						float r = this.m_gmd.formLeft[2].r;
						float r2 = this.m_gmd.formLeft[1].r;
						float g = this.m_gmd.formLeft[2].g;
						float g2 = this.m_gmd.formLeft[1].g;
						float r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						float g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white = new Color(r3, g3, 0f);
						r = this.m_gmd.formLeft[3].r;
						r2 = this.m_gmd.formLeft[0].r;
						g = this.m_gmd.formLeft[3].g;
						g2 = this.m_gmd.formLeft[0].g;
						r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white2 = new Color(r3, g3, 0f);
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
							r = white.r;
							r2 = white2.r;
							g = white.g;
							g2 = white2.g;
							r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
							g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
							white = new Color(r3, g3, 0f);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white2);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(white2);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Back)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Forward)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Top)
					{
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Bottom)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
					}
				}
				else if (kind == BlockKind.StairEast)
				{
					if (this.m_gmd.faceStair == BlockFaceStair.Left)
					{
						float r = this.m_gmd.formFront[2].r;
						float r2 = this.m_gmd.formFront[1].r;
						float g = this.m_gmd.formFront[2].g;
						float g2 = this.m_gmd.formFront[1].g;
						float r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						float g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white = new Color(r3, g3, 0f);
						r = this.m_gmd.formFront[3].r;
						r2 = this.m_gmd.formFront[0].r;
						g = this.m_gmd.formFront[3].g;
						g2 = this.m_gmd.formFront[0].g;
						r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white2 = new Color(r3, g3, 0f);
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
							r = white.r;
							r2 = white2.r;
							g = white.g;
							g2 = white2.g;
							r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
							g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
							white = new Color(r3, g3, 0f);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white2);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(white2);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Right)
					{
						float r = this.m_gmd.formBack[2].r;
						float r2 = this.m_gmd.formBack[1].r;
						float g = this.m_gmd.formBack[2].g;
						float g2 = this.m_gmd.formLeft[1].g;
						float r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						float g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white = new Color(r3, g3, 0f);
						r = this.m_gmd.formBack[3].r;
						r2 = this.m_gmd.formBack[0].r;
						g = this.m_gmd.formBack[3].g;
						g2 = this.m_gmd.formBack[0].g;
						r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white2 = new Color(r3, g3, 0f);
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(white2);
							r = white.r;
							r2 = white2.r;
							g = white.g;
							g2 = white2.g;
							r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
							g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
							white = new Color(r3, g3, 0f);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[1]);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white2);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Back)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Forward)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Top)
					{
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Bottom)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
					}
				}
				else if (kind == BlockKind.StairWest)
				{
					if (this.m_gmd.faceStair == BlockFaceStair.Left)
					{
						float r = this.m_gmd.formBack[2].r;
						float r2 = this.m_gmd.formBack[1].r;
						float g = this.m_gmd.formBack[2].g;
						float g2 = this.m_gmd.formBack[1].g;
						float r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						float g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white = new Color(r3, g3, 0f);
						r = this.m_gmd.formBack[3].r;
						r2 = this.m_gmd.formBack[0].r;
						g = this.m_gmd.formBack[3].g;
						g2 = this.m_gmd.formBack[0].g;
						r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white2 = new Color(r3, g3, 0f);
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
							r = white.r;
							r2 = white2.r;
							g = white.g;
							g2 = white2.g;
							r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
							g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
							white = new Color(r3, g3, 0f);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white2);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(white2);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Right)
					{
						float r = this.m_gmd.formFront[2].r;
						float r2 = this.m_gmd.formFront[1].r;
						float g = this.m_gmd.formFront[2].g;
						float g2 = this.m_gmd.formFront[1].g;
						float r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						float g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white = new Color(r3, g3, 0f);
						r = this.m_gmd.formFront[3].r;
						r2 = this.m_gmd.formFront[0].r;
						g = this.m_gmd.formFront[3].g;
						g2 = this.m_gmd.formFront[0].g;
						r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
						g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
						white2 = new Color(r3, g3, 0f);
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(white2);
							r = white.r;
							r2 = white2.r;
							g = white.g;
							g2 = white2.g;
							r3 = this.Interpolation(0f, 1f, r, r2, 0.5f);
							g3 = this.Interpolation(0f, 1f, g, g2, 0.5f);
							white = new Color(r3, g3, 0f);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[1]);
							this.m_gmd.meshData.m_Colors.Add(white);
							this.m_gmd.meshData.m_Colors.Add(white2);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Back)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Forward)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Top)
					{
						if (this.m_gmd.h < 1)
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
						}
						else
						{
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
						}
					}
					else if (this.m_gmd.faceStair == BlockFaceStair.Bottom)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
					}
				}
			}
			else if (kind.IsFence())
			{
				if (this.m_gmd.sideIndex == 1)
				{
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[1]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[2]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
				}
				else if (this.m_gmd.sideIndex == 0)
				{
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[1]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[2]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
				}
				else if (this.m_gmd.sideIndex == 3)
				{
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[1]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[2]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
				}
				else if (this.m_gmd.sideIndex == 2)
				{
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[1]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[2]);
					this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
				}
				else if (this.m_gmd.sideIndex == 4)
				{
					if (kind == BlockKind.SouthFenceEast || kind == BlockKind.SouthFenceNorth || kind == BlockKind.SouthFenceSouth || kind == BlockKind.SouthFenceWest || kind == BlockKind.NorthFenceEast || kind == BlockKind.NorthFenceNorth || kind == BlockKind.NorthFenceSouth || kind == BlockKind.NorthFenceWest)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
					}
					else if (kind == BlockKind.EastFenceEast || kind == BlockKind.EastFenceNorth || kind == BlockKind.EastFenceSouth || kind == BlockKind.EastFenceWest || kind == BlockKind.WestFenceEast || kind == BlockKind.WestFenceNorth || kind == BlockKind.WestFenceSouth || kind == BlockKind.WestFenceWest)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
					}
					else if (kind == BlockKind.FenceOnWallSouthNorth)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
					}
					else if (kind == BlockKind.Fence)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
					}
					else
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
					}
				}
				else if (this.m_gmd.sideIndex == 5)
				{
					if (kind == BlockKind.SouthFenceEast || kind == BlockKind.SouthFenceNorth || kind == BlockKind.SouthFenceSouth || kind == BlockKind.SouthFenceWest || kind == BlockKind.NorthFenceEast || kind == BlockKind.NorthFenceNorth || kind == BlockKind.NorthFenceSouth || kind == BlockKind.NorthFenceWest)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
					}
					else if (kind == BlockKind.EastFenceEast || kind == BlockKind.EastFenceNorth || kind == BlockKind.EastFenceSouth || kind == BlockKind.EastFenceWest || kind == BlockKind.WestFenceEast || kind == BlockKind.WestFenceNorth || kind == BlockKind.WestFenceSouth || kind == BlockKind.WestFenceWest)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
					}
					else if (kind == BlockKind.FenceOnWallSouthNorth)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
					}
					else if (kind == BlockKind.Fence)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
					}
					else
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
					}
				}
			}
			else if (kind.IsCornerStair())
			{
				Color white3 = Color.white;
				Color white4 = Color.white;
				if (kind == BlockKind.CornerStairSouth)
				{
					if (this.m_gmd.sideIndex == 0)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formLeft[0].r;
							float r5 = this.m_gmd.formLeft[1].r;
							float g4 = this.m_gmd.formLeft[0].g;
							float g5 = this.m_gmd.formLeft[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formLeft[3].r;
							r5 = this.m_gmd.formLeft[2].r;
							g4 = this.m_gmd.formLeft[3].g;
							g5 = this.m_gmd.formLeft[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
						}
						else
						{
							float r4 = this.m_gmd.formLeft[1].r;
							float r5 = this.m_gmd.formLeft[2].r;
							float g4 = this.m_gmd.formLeft[1].g;
							float g5 = this.m_gmd.formLeft[2].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = this.m_gmd.formLeft[0].r;
							r5 = this.m_gmd.formLeft[3].r;
							g4 = this.m_gmd.formLeft[0].g;
							g5 = this.m_gmd.formLeft[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formLeft[2].r;
							r5 = this.m_gmd.formLeft[3].r;
							g4 = this.m_gmd.formLeft[2].g;
							g5 = this.m_gmd.formLeft[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[2]);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 1)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formRight[0].r;
							float r5 = this.m_gmd.formRight[1].r;
							float g4 = this.m_gmd.formRight[0].g;
							float g5 = this.m_gmd.formRight[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formRight[3].r;
							r5 = this.m_gmd.formRight[2].r;
							g4 = this.m_gmd.formRight[3].g;
							g5 = this.m_gmd.formRight[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
						}
						else
						{
							float r4 = this.m_gmd.formRight[0].r;
							float r5 = this.m_gmd.formRight[1].r;
							float g4 = this.m_gmd.formRight[0].g;
							float g5 = this.m_gmd.formRight[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[1]);
							r4 = this.m_gmd.formRight[1].r;
							r5 = this.m_gmd.formRight[2].r;
							g4 = this.m_gmd.formRight[1].g;
							g5 = this.m_gmd.formRight[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							r4 = this.m_gmd.formRight[0].r;
							r5 = this.m_gmd.formRight[3].r;
							g4 = this.m_gmd.formRight[0].g;
							g5 = this.m_gmd.formRight[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 3)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formBack[0].r;
							float r5 = this.m_gmd.formBack[1].r;
							float g4 = this.m_gmd.formBack[0].g;
							float g5 = this.m_gmd.formBack[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formBack[3].r;
							r5 = this.m_gmd.formBack[2].r;
							g4 = this.m_gmd.formBack[3].g;
							g5 = this.m_gmd.formBack[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
						}
						else
						{
							float r4 = this.m_gmd.formBack[0].r;
							float r5 = this.m_gmd.formBack[1].r;
							float g4 = this.m_gmd.formBack[0].g;
							float g5 = this.m_gmd.formBack[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[1]);
							r4 = this.m_gmd.formBack[1].r;
							r5 = this.m_gmd.formBack[2].r;
							g4 = this.m_gmd.formBack[1].g;
							g5 = this.m_gmd.formBack[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							r4 = this.m_gmd.formBack[0].r;
							r5 = this.m_gmd.formBack[3].r;
							g4 = this.m_gmd.formBack[0].g;
							g5 = this.m_gmd.formBack[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 2)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formFront[0].r;
							float r5 = this.m_gmd.formFront[1].r;
							float g4 = this.m_gmd.formFront[0].g;
							float g5 = this.m_gmd.formFront[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formFront[3].r;
							r5 = this.m_gmd.formFront[2].r;
							g4 = this.m_gmd.formFront[3].g;
							g5 = this.m_gmd.formFront[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
						}
						else
						{
							float r4 = this.m_gmd.formFront[1].r;
							float r5 = this.m_gmd.formFront[2].r;
							float g4 = this.m_gmd.formFront[1].g;
							float g5 = this.m_gmd.formFront[2].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = this.m_gmd.formFront[0].r;
							r5 = this.m_gmd.formFront[3].r;
							g4 = this.m_gmd.formFront[0].g;
							g5 = this.m_gmd.formFront[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formFront[2].r;
							r5 = this.m_gmd.formFront[3].r;
							g4 = this.m_gmd.formFront[2].g;
							g5 = this.m_gmd.formFront[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[2]);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 4)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
					}
					else if (this.m_gmd.sideIndex == 5)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
					}
				}
				else if (kind == BlockKind.CornerStairNorth)
				{
					if (this.m_gmd.sideIndex == 3)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formBack[0].r;
							float r5 = this.m_gmd.formBack[1].r;
							float g4 = this.m_gmd.formBack[0].g;
							float g5 = this.m_gmd.formBack[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formBack[3].r;
							r5 = this.m_gmd.formBack[2].r;
							g4 = this.m_gmd.formBack[3].g;
							g5 = this.m_gmd.formBack[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
						}
						else
						{
							float r4 = this.m_gmd.formBack[1].r;
							float r5 = this.m_gmd.formBack[2].r;
							float g4 = this.m_gmd.formBack[1].g;
							float g5 = this.m_gmd.formBack[2].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = this.m_gmd.formBack[0].r;
							r5 = this.m_gmd.formBack[3].r;
							g4 = this.m_gmd.formBack[0].g;
							g5 = this.m_gmd.formBack[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formBack[2].r;
							r5 = this.m_gmd.formBack[3].r;
							g4 = this.m_gmd.formBack[2].g;
							g5 = this.m_gmd.formBack[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[2]);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 2)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formFront[0].r;
							float r5 = this.m_gmd.formFront[1].r;
							float g4 = this.m_gmd.formFront[0].g;
							float g5 = this.m_gmd.formFront[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formFront[3].r;
							r5 = this.m_gmd.formFront[2].r;
							g4 = this.m_gmd.formFront[3].g;
							g5 = this.m_gmd.formFront[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
						}
						else
						{
							float r4 = this.m_gmd.formFront[0].r;
							float r5 = this.m_gmd.formFront[1].r;
							float g4 = this.m_gmd.formFront[0].g;
							float g5 = this.m_gmd.formFront[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[1]);
							r4 = this.m_gmd.formFront[1].r;
							r5 = this.m_gmd.formFront[2].r;
							g4 = this.m_gmd.formFront[1].g;
							g5 = this.m_gmd.formFront[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							r4 = this.m_gmd.formFront[0].r;
							r5 = this.m_gmd.formFront[3].r;
							g4 = this.m_gmd.formFront[0].g;
							g5 = this.m_gmd.formFront[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 0)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formLeft[0].r;
							float r5 = this.m_gmd.formLeft[1].r;
							float g4 = this.m_gmd.formLeft[0].g;
							float g5 = this.m_gmd.formLeft[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formLeft[3].r;
							r5 = this.m_gmd.formLeft[2].r;
							g4 = this.m_gmd.formLeft[3].g;
							g5 = this.m_gmd.formLeft[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
						}
						else
						{
							float r4 = this.m_gmd.formLeft[0].r;
							float r5 = this.m_gmd.formLeft[1].r;
							float g4 = this.m_gmd.formLeft[0].g;
							float g5 = this.m_gmd.formLeft[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[1]);
							r4 = this.m_gmd.formLeft[1].r;
							r5 = this.m_gmd.formLeft[2].r;
							g4 = this.m_gmd.formLeft[1].g;
							g5 = this.m_gmd.formLeft[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							r4 = this.m_gmd.formLeft[0].r;
							r5 = this.m_gmd.formLeft[3].r;
							g4 = this.m_gmd.formLeft[0].g;
							g5 = this.m_gmd.formLeft[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 1)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formRight[0].r;
							float r5 = this.m_gmd.formRight[1].r;
							float g4 = this.m_gmd.formRight[0].g;
							float g5 = this.m_gmd.formRight[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formRight[3].r;
							r5 = this.m_gmd.formRight[2].r;
							g4 = this.m_gmd.formRight[3].g;
							g5 = this.m_gmd.formRight[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
						}
						else
						{
							float r4 = this.m_gmd.formRight[1].r;
							float r5 = this.m_gmd.formRight[2].r;
							float g4 = this.m_gmd.formRight[1].g;
							float g5 = this.m_gmd.formRight[2].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = this.m_gmd.formRight[0].r;
							r5 = this.m_gmd.formRight[3].r;
							g4 = this.m_gmd.formRight[0].g;
							g5 = this.m_gmd.formRight[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formRight[2].r;
							r5 = this.m_gmd.formRight[3].r;
							g4 = this.m_gmd.formRight[2].g;
							g5 = this.m_gmd.formRight[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[2]);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 4)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
					}
					else if (this.m_gmd.sideIndex == 5)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
					}
				}
				else if (kind == BlockKind.CornerStairEast)
				{
					if (this.m_gmd.sideIndex == 0)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formLeft[0].r;
							float r5 = this.m_gmd.formLeft[1].r;
							float g4 = this.m_gmd.formLeft[0].g;
							float g5 = this.m_gmd.formLeft[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formLeft[3].r;
							r5 = this.m_gmd.formLeft[2].r;
							g4 = this.m_gmd.formLeft[3].g;
							g5 = this.m_gmd.formLeft[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
						}
						else
						{
							float r4 = this.m_gmd.formLeft[1].r;
							float r5 = this.m_gmd.formLeft[2].r;
							float g4 = this.m_gmd.formLeft[1].g;
							float g5 = this.m_gmd.formLeft[2].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = this.m_gmd.formLeft[0].r;
							r5 = this.m_gmd.formLeft[3].r;
							g4 = this.m_gmd.formLeft[0].g;
							g5 = this.m_gmd.formLeft[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formLeft[2].r;
							r5 = this.m_gmd.formLeft[3].r;
							g4 = this.m_gmd.formLeft[2].g;
							g5 = this.m_gmd.formLeft[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[2]);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 1)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formRight[0].r;
							float r5 = this.m_gmd.formRight[1].r;
							float g4 = this.m_gmd.formRight[0].g;
							float g5 = this.m_gmd.formRight[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formRight[3].r;
							r5 = this.m_gmd.formRight[2].r;
							g4 = this.m_gmd.formRight[3].g;
							g5 = this.m_gmd.formRight[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
						}
						else
						{
							float r4 = this.m_gmd.formRight[0].r;
							float r5 = this.m_gmd.formRight[1].r;
							float g4 = this.m_gmd.formRight[0].g;
							float g5 = this.m_gmd.formRight[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[1]);
							r4 = this.m_gmd.formRight[1].r;
							r5 = this.m_gmd.formRight[2].r;
							g4 = this.m_gmd.formRight[1].g;
							g5 = this.m_gmd.formRight[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							r4 = this.m_gmd.formRight[0].r;
							r5 = this.m_gmd.formRight[3].r;
							g4 = this.m_gmd.formRight[0].g;
							g5 = this.m_gmd.formRight[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 3)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formBack[0].r;
							float r5 = this.m_gmd.formBack[1].r;
							float g4 = this.m_gmd.formBack[0].g;
							float g5 = this.m_gmd.formBack[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formBack[3].r;
							r5 = this.m_gmd.formBack[2].r;
							g4 = this.m_gmd.formBack[3].g;
							g5 = this.m_gmd.formBack[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
						}
						else
						{
							float r4 = this.m_gmd.formBack[0].r;
							float r5 = this.m_gmd.formBack[1].r;
							float g4 = this.m_gmd.formBack[0].g;
							float g5 = this.m_gmd.formBack[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[1]);
							r4 = this.m_gmd.formBack[1].r;
							r5 = this.m_gmd.formBack[2].r;
							g4 = this.m_gmd.formBack[1].g;
							g5 = this.m_gmd.formBack[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							r4 = this.m_gmd.formBack[0].r;
							r5 = this.m_gmd.formBack[3].r;
							g4 = this.m_gmd.formBack[0].g;
							g5 = this.m_gmd.formBack[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 2)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formFront[0].r;
							float r5 = this.m_gmd.formFront[1].r;
							float g4 = this.m_gmd.formFront[0].g;
							float g5 = this.m_gmd.formFront[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formFront[3].r;
							r5 = this.m_gmd.formFront[2].r;
							g4 = this.m_gmd.formFront[3].g;
							g5 = this.m_gmd.formFront[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
						}
						else
						{
							float r4 = this.m_gmd.formFront[0].r;
							float r5 = this.m_gmd.formFront[1].r;
							float g4 = this.m_gmd.formFront[0].g;
							float g5 = this.m_gmd.formFront[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[1]);
							r4 = this.m_gmd.formFront[1].r;
							r5 = this.m_gmd.formFront[2].r;
							g4 = this.m_gmd.formFront[1].g;
							g5 = this.m_gmd.formFront[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							r4 = this.m_gmd.formFront[0].r;
							r5 = this.m_gmd.formFront[3].r;
							g4 = this.m_gmd.formFront[0].g;
							g5 = this.m_gmd.formFront[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 4)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
					}
					else if (this.m_gmd.sideIndex == 5)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
					}
				}
				else if (kind == BlockKind.CornerStairWest)
				{
					if (this.m_gmd.sideIndex == 0)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formLeft[0].r;
							float r5 = this.m_gmd.formLeft[1].r;
							float g4 = this.m_gmd.formLeft[0].g;
							float g5 = this.m_gmd.formLeft[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formLeft[3].r;
							r5 = this.m_gmd.formLeft[2].r;
							g4 = this.m_gmd.formLeft[3].g;
							g5 = this.m_gmd.formLeft[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
						}
						else
						{
							float r4 = this.m_gmd.formLeft[0].r;
							float r5 = this.m_gmd.formLeft[1].r;
							float g4 = this.m_gmd.formLeft[0].g;
							float g5 = this.m_gmd.formLeft[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[1]);
							r4 = this.m_gmd.formLeft[1].r;
							r5 = this.m_gmd.formLeft[2].r;
							g4 = this.m_gmd.formLeft[1].g;
							g5 = this.m_gmd.formLeft[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							r4 = this.m_gmd.formLeft[0].r;
							r5 = this.m_gmd.formLeft[3].r;
							g4 = this.m_gmd.formLeft[0].g;
							g5 = this.m_gmd.formLeft[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 1)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formRight[0].r;
							float r5 = this.m_gmd.formRight[1].r;
							float g4 = this.m_gmd.formRight[0].g;
							float g5 = this.m_gmd.formRight[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formRight[3].r;
							r5 = this.m_gmd.formRight[2].r;
							g4 = this.m_gmd.formRight[3].g;
							g5 = this.m_gmd.formRight[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
						}
						else
						{
							float r4 = this.m_gmd.formRight[1].r;
							float r5 = this.m_gmd.formRight[2].r;
							float g4 = this.m_gmd.formRight[1].g;
							float g5 = this.m_gmd.formRight[2].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = this.m_gmd.formRight[0].r;
							r5 = this.m_gmd.formRight[3].r;
							g4 = this.m_gmd.formRight[0].g;
							g5 = this.m_gmd.formRight[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formRight[2].r;
							r5 = this.m_gmd.formRight[3].r;
							g4 = this.m_gmd.formRight[2].g;
							g5 = this.m_gmd.formRight[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[2]);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 3)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formBack[0].r;
							float r5 = this.m_gmd.formBack[1].r;
							float g4 = this.m_gmd.formBack[0].g;
							float g5 = this.m_gmd.formBack[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formBack[3].r;
							r5 = this.m_gmd.formBack[2].r;
							g4 = this.m_gmd.formBack[3].g;
							g5 = this.m_gmd.formBack[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
						}
						else
						{
							float r4 = this.m_gmd.formBack[0].r;
							float r5 = this.m_gmd.formBack[1].r;
							float g4 = this.m_gmd.formBack[0].g;
							float g5 = this.m_gmd.formBack[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[1]);
							r4 = this.m_gmd.formBack[1].r;
							r5 = this.m_gmd.formBack[2].r;
							g4 = this.m_gmd.formBack[1].g;
							g5 = this.m_gmd.formBack[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							r4 = this.m_gmd.formBack[0].r;
							r5 = this.m_gmd.formBack[3].r;
							g4 = this.m_gmd.formBack[0].g;
							g5 = this.m_gmd.formBack[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 2)
					{
						if (this.m_gmd.h == 0)
						{
							float r4 = this.m_gmd.formFront[0].r;
							float r5 = this.m_gmd.formFront[1].r;
							float g4 = this.m_gmd.formFront[0].g;
							float g5 = this.m_gmd.formFront[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
							this.m_gmd.meshData.m_Colors.Add(white3);
							r4 = this.m_gmd.formFront[3].r;
							r5 = this.m_gmd.formFront[2].r;
							g4 = this.m_gmd.formFront[3].g;
							g5 = this.m_gmd.formFront[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
						}
						else
						{
							float r4 = this.m_gmd.formFront[0].r;
							float r5 = this.m_gmd.formFront[1].r;
							float g4 = this.m_gmd.formFront[0].g;
							float g5 = this.m_gmd.formFront[1].g;
							float r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							float g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white3);
							this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[1]);
							r4 = this.m_gmd.formFront[1].r;
							r5 = this.m_gmd.formFront[2].r;
							g4 = this.m_gmd.formFront[1].g;
							g5 = this.m_gmd.formFront[2].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
							r4 = this.m_gmd.formFront[0].r;
							r5 = this.m_gmd.formFront[3].r;
							g4 = this.m_gmd.formFront[0].g;
							g5 = this.m_gmd.formFront[3].g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white3 = new Color(r6, g6, 0f);
							r4 = white3.r;
							r5 = white4.r;
							g4 = white3.g;
							g5 = white4.g;
							r6 = this.Interpolation(0f, 1f, r4, r5, 0.5f);
							g6 = this.Interpolation(0f, 1f, g4, g5, 0.5f);
							white4 = new Color(r6, g6, 0f);
							this.m_gmd.meshData.m_Colors.Add(white4);
						}
					}
					else if (this.m_gmd.sideIndex == 4)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
					}
					else if (this.m_gmd.sideIndex == 5)
					{
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
						this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
					}
				}
			}
			else if (this.m_gmd.sideIndex == 1)
			{
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[0]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[1]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[2]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formRight[3]);
			}
			else if (this.m_gmd.sideIndex == 0)
			{
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[0]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[1]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[2]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formLeft[3]);
			}
			else if (this.m_gmd.sideIndex == 3)
			{
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[0]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[1]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[2]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBack[3]);
			}
			else if (this.m_gmd.sideIndex == 2)
			{
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[0]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[1]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[2]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formFront[3]);
			}
			else if (this.m_gmd.sideIndex == 4)
			{
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[0]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[1]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[2]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formTop[3]);
			}
			else if (this.m_gmd.sideIndex == 5)
			{
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[0]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[1]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[2]);
				this.m_gmd.meshData.m_Colors.Add(this.m_gmd.formBottom[3]);
			}
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
		}
		else if (this.m_gmd.sideIndex == 1)
		{
			this.LeftSide();
		}
		else if (this.m_gmd.sideIndex == 0)
		{
			this.RightSide();
		}
		else if (this.m_gmd.sideIndex == 3)
		{
			this.FrontSide();
		}
		else if (this.m_gmd.sideIndex == 2)
		{
			this.BackSide();
		}
		else if (this.m_gmd.sideIndex == 4)
		{
			this.TopSide();
		}
		else if (this.m_gmd.sideIndex == 5)
		{
			this.BottomSide();
		}
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
				int num = (this.m_gmd.blockFace != BlockFace.Bottom) ? 2 : 0;
				rect = this.worldData.BlockUvCoordinates[this.m_gmd.blockType].BlockFaceUvCoordinates[num];
			}
			else
			{
				rect = this.worldData.BlockUvCoordinates[this.m_gmd.blockType].BlockFaceUvCoordinates[(int)this.m_gmd.blockFace];
			}
			if (kind == BlockKind.FenceOnWallEastWest || kind == BlockKind.EastFenceNorth || kind == BlockKind.EastFenceSouth || kind == BlockKind.WestFenceNorth || kind == BlockKind.WestFenceSouth || kind == BlockKind.EastFenceEast || kind == BlockKind.EastFenceWest || kind == BlockKind.WestFenceEast || kind == BlockKind.WestFenceWest)
			{
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[0], rect.y + rect.height * this.m_gmd.uvY[0]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvY[1], rect.y + rect.height * this.m_gmd.uvX[1]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[2], rect.y + rect.height * this.m_gmd.uvY[2]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvY[3], rect.y + rect.height * this.m_gmd.uvX[3]));
			}
			else if ((kind == BlockKind.FenceOnWallSouthNorth && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (kind == BlockKind.NorthFenceNorth && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (kind == BlockKind.NorthFenceSouth && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (kind == BlockKind.NorthFenceWest && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (kind == BlockKind.NorthFenceEast && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (kind == BlockKind.SouthFenceNorth && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (kind == BlockKind.SouthFenceSouth && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (kind == BlockKind.SouthFenceWest && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)) || (kind == BlockKind.SouthFenceEast && (this.m_gmd.sideIndex == 0 || this.m_gmd.sideIndex == 1)))
			{
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[0], rect.y + rect.height * this.m_gmd.uvY[0]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvY[1], rect.y + rect.height * this.m_gmd.uvX[1]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[2], rect.y + rect.height * this.m_gmd.uvY[2]));
				this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvY[3], rect.y + rect.height * this.m_gmd.uvX[3]));
			}
			else if (kind.IsDefault())
			{
				if (this.m_gmd.fliped)
				{
					this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[3], rect.y + rect.height * this.m_gmd.uvY[3]));
					this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[0], rect.y + rect.height * this.m_gmd.uvY[0]));
					this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[1], rect.y + rect.height * this.m_gmd.uvY[1]));
					this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[2], rect.y + rect.height * this.m_gmd.uvY[2]));
				}
				else
				{
					this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[0], rect.y + rect.height * this.m_gmd.uvY[0]));
					this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[1], rect.y + rect.height * this.m_gmd.uvY[1]));
					this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[2], rect.y + rect.height * this.m_gmd.uvY[2]));
					this.m_gmd.meshData.m_Uvs.Add(new Vector2(rect.x + rect.width * this.m_gmd.uvX[3], rect.y + rect.height * this.m_gmd.uvY[3]));
				}
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

	private void GetVertexLight()
	{
		this.m_gmd.defaultLight = new Color[]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		};
		float num = (float)World.Instance.Lighting.LightingSteps;
		if (this.m_gmd.sideIndex == 1)
		{
			int num2 = this.m_gmd.blockX;
			int num3 = this.m_gmd.blockY + 1;
			int num4 = this.m_gmd.blockZ - 1;
			int num5 = this.m_gmd.blockX;
			int num6 = this.m_gmd.blockY + 1;
			int num7 = this.m_gmd.blockZ + 1;
			int num8 = this.m_gmd.blockX;
			int num9 = this.m_gmd.blockY - 1;
			int num10 = this.m_gmd.blockZ + 1;
			int num11 = this.m_gmd.blockX;
			int num12 = this.m_gmd.blockY - 1;
			int num13 = this.m_gmd.blockZ - 1;
			byte blockLight = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
			float num14 = (float)(blockLight >> 4) / num;
			float num15 = (float)(blockLight & 15) / num;
			byte blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3, num4 + 1);
			float num16 = (float)(blockLight2 >> 4) / num;
			float num17 = (float)(blockLight2 & 15) / num;
			byte blockLight3 = World.Instance.Lighting.GetBlockLight(num2, num3 - 1, num4);
			float num18 = (float)(blockLight3 >> 4) / num;
			float num19 = (float)(blockLight3 & 15) / num;
			byte blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			float num20 = (float)(blockLight4 >> 4) / num;
			float num21 = (float)(blockLight4 & 15) / num;
			float num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float g = (num15 + num17 + num19 + num21) / num22;
			Color color = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num5, num6, num7 - 1);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num5, num6 - 1, num7);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color2 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num8, num9 + 1, num10);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9, num10 - 1);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color3 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num11, num12, num13 + 1);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num11, num12 + 1, num13);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color4 = new Color(r, g, 0f, 0f);
			this.m_gmd.defaultLight[0] = color;
			this.m_gmd.defaultLight[1] = color2;
			this.m_gmd.defaultLight[2] = color3;
			this.m_gmd.defaultLight[3] = color4;
		}
		else if (this.m_gmd.sideIndex == 0)
		{
			int num2 = this.m_gmd.blockX;
			int num3 = this.m_gmd.blockY + 1;
			int num4 = this.m_gmd.blockZ - 1;
			int num5 = this.m_gmd.blockX;
			int num6 = this.m_gmd.blockY + 1;
			int num7 = this.m_gmd.blockZ + 1;
			int num8 = this.m_gmd.blockX;
			int num9 = this.m_gmd.blockY - 1;
			int num10 = this.m_gmd.blockZ + 1;
			int num11 = this.m_gmd.blockX;
			int num12 = this.m_gmd.blockY - 1;
			int num13 = this.m_gmd.blockZ - 1;
			byte blockLight = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
			float num14 = (float)(blockLight >> 4) / num;
			float num15 = (float)(blockLight & 15) / num;
			byte blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3, num4 + 1);
			float num16 = (float)(blockLight2 >> 4) / num;
			float num17 = (float)(blockLight2 & 15) / num;
			byte blockLight3 = World.Instance.Lighting.GetBlockLight(num2, num3 - 1, num4);
			float num18 = (float)(blockLight3 >> 4) / num;
			float num19 = (float)(blockLight3 & 15) / num;
			byte blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			float num20 = (float)(blockLight4 >> 4) / num;
			float num21 = (float)(blockLight4 & 15) / num;
			float num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float g = (num15 + num17 + num19 + num21) / num22;
			Color color = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num5, num6, num7 - 1);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num5, num6 - 1, num7);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color2 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num8, num9 + 1, num10);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9, num10 - 1);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color3 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num11, num12, num13 + 1);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num11, num12 + 1, num13);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color4 = new Color(r, g, 0f, 0f);
			this.m_gmd.defaultLight[0] = color4;
			this.m_gmd.defaultLight[1] = color3;
			this.m_gmd.defaultLight[2] = color2;
			this.m_gmd.defaultLight[3] = color;
		}
		else if (this.m_gmd.sideIndex == 3)
		{
			int num2 = this.m_gmd.blockX - 1;
			int num3 = this.m_gmd.blockY;
			int num4 = this.m_gmd.blockZ - 1;
			int num5 = this.m_gmd.blockX - 1;
			int num6 = this.m_gmd.blockY;
			int num7 = this.m_gmd.blockZ + 1;
			int num8 = this.m_gmd.blockX + 1;
			int num9 = this.m_gmd.blockY;
			int num10 = this.m_gmd.blockZ + 1;
			int num11 = this.m_gmd.blockX + 1;
			int num12 = this.m_gmd.blockY;
			int num13 = this.m_gmd.blockZ - 1;
			byte blockLight = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
			float num14 = (float)(blockLight >> 4) / num;
			float num15 = (float)(blockLight & 15) / num;
			byte blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3, num4 + 1);
			float num16 = (float)(blockLight2 >> 4) / num;
			float num17 = (float)(blockLight2 & 15) / num;
			byte blockLight3 = World.Instance.Lighting.GetBlockLight(num2 + 1, num3, num4);
			float num18 = (float)(blockLight3 >> 4) / num;
			float num19 = (float)(blockLight3 & 15) / num;
			byte blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			float num20 = (float)(blockLight4 >> 4) / num;
			float num21 = (float)(blockLight4 & 15) / num;
			float num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float g = (num15 + num17 + num19 + num21) / num22;
			Color color = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num5, num6, num7 - 1);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num5 + 1, num6, num7);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color2 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num8 - 1, num9, num10);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9, num10 - 1);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color3 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num11, num12, num13 + 1);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num11 - 1, num12, num13);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color4 = new Color(r, g, 0f, 0f);
			this.m_gmd.defaultLight[0] = color;
			this.m_gmd.defaultLight[1] = color2;
			this.m_gmd.defaultLight[2] = color3;
			this.m_gmd.defaultLight[3] = color4;
		}
		else if (this.m_gmd.sideIndex == 2)
		{
			int num2 = this.m_gmd.blockX - 1;
			int num3 = this.m_gmd.blockY;
			int num4 = this.m_gmd.blockZ - 1;
			int num5 = this.m_gmd.blockX - 1;
			int num6 = this.m_gmd.blockY;
			int num7 = this.m_gmd.blockZ + 1;
			int num8 = this.m_gmd.blockX + 1;
			int num9 = this.m_gmd.blockY;
			int num10 = this.m_gmd.blockZ + 1;
			int num11 = this.m_gmd.blockX + 1;
			int num12 = this.m_gmd.blockY;
			int num13 = this.m_gmd.blockZ - 1;
			byte blockLight = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
			float num14 = (float)(blockLight >> 4) / num;
			float num15 = (float)(blockLight & 15) / num;
			byte blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3, num4 + 1);
			float num16 = (float)(blockLight2 >> 4) / num;
			float num17 = (float)(blockLight2 & 15) / num;
			byte blockLight3 = World.Instance.Lighting.GetBlockLight(num2 + 1, num3, num4);
			float num18 = (float)(blockLight3 >> 4) / num;
			float num19 = (float)(blockLight3 & 15) / num;
			byte blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			float num20 = (float)(blockLight4 >> 4) / num;
			float num21 = (float)(blockLight4 & 15) / num;
			float num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float g = (num15 + num17 + num19 + num21) / num22;
			Color color = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num5, num6, num7 - 1);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num5 + 1, num6, num7);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color2 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num8 - 1, num9, num10);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9, num10 - 1);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color3 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num11, num12, num13 + 1);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num11 - 1, num12, num13);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color4 = new Color(r, g, 0f, 0f);
			this.m_gmd.defaultLight[0] = color4;
			this.m_gmd.defaultLight[1] = color3;
			this.m_gmd.defaultLight[2] = color2;
			this.m_gmd.defaultLight[3] = color;
		}
		else if (this.m_gmd.sideIndex == 4)
		{
			int num2 = this.m_gmd.blockX - 1;
			int num3 = this.m_gmd.blockY - 1;
			int num4 = this.m_gmd.blockZ;
			int num5 = this.m_gmd.blockX - 1;
			int num6 = this.m_gmd.blockY + 1;
			int num7 = this.m_gmd.blockZ;
			int num8 = this.m_gmd.blockX + 1;
			int num9 = this.m_gmd.blockY + 1;
			int num10 = this.m_gmd.blockZ;
			int num11 = this.m_gmd.blockX + 1;
			int num12 = this.m_gmd.blockY - 1;
			int num13 = this.m_gmd.blockZ;
			byte blockLight = World.Instance.Lighting.GetBlockLight(num2, num3 + 1, num4);
			float num14 = (float)(blockLight >> 4) / num;
			float num15 = (float)(blockLight & 15) / num;
			byte blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
			float num16 = (float)(blockLight2 >> 4) / num;
			float num17 = (float)(blockLight2 & 15) / num;
			byte blockLight3 = World.Instance.Lighting.GetBlockLight(num2 + 1, num3, num4);
			float num18 = (float)(blockLight3 >> 4) / num;
			float num19 = (float)(blockLight3 & 15) / num;
			byte blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			float num20 = (float)(blockLight4 >> 4) / num;
			float num21 = (float)(blockLight4 & 15) / num;
			float num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float g = (num15 + num17 + num19 + num21) / num22;
			Color color = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num5 + 1, num6, num7);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num5, num6 - 1, num7);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color2 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num8 - 1, num9, num10);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9 - 1, num10);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color3 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num11 - 1, num12, num13);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num11, num12 + 1, num13);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color4 = new Color(r, g, 0f, 0f);
			this.m_gmd.defaultLight[0] = color;
			this.m_gmd.defaultLight[1] = color2;
			this.m_gmd.defaultLight[2] = color3;
			this.m_gmd.defaultLight[3] = color4;
		}
		else if (this.m_gmd.sideIndex == 5)
		{
			int num2 = this.m_gmd.blockX + 1;
			int num3 = this.m_gmd.blockY + 1;
			int num4 = this.m_gmd.blockZ;
			int num5 = this.m_gmd.blockX + 1;
			int num6 = this.m_gmd.blockY - 1;
			int num7 = this.m_gmd.blockZ;
			int num8 = this.m_gmd.blockX - 1;
			int num9 = this.m_gmd.blockY - 1;
			int num10 = this.m_gmd.blockZ;
			int num11 = this.m_gmd.blockX - 1;
			int num12 = this.m_gmd.blockY + 1;
			int num13 = this.m_gmd.blockZ;
			byte blockLight = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
			float num14 = (float)(blockLight >> 4) / num;
			float num15 = (float)(blockLight & 15) / num;
			byte blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3 - 1, num4);
			float num16 = (float)(blockLight2 >> 4) / num;
			float num17 = (float)(blockLight2 & 15) / num;
			byte blockLight3 = World.Instance.Lighting.GetBlockLight(num2 - 1, num3, num4);
			float num18 = (float)(blockLight3 >> 4) / num;
			float num19 = (float)(blockLight3 & 15) / num;
			byte blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			float num20 = (float)(blockLight4 >> 4) / num;
			float num21 = (float)(blockLight4 & 15) / num;
			float num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			float g = (num15 + num17 + num19 + num21) / num22;
			Color color = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num5, num6 + 1, num7);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num5 - 1, num6, num7);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color2 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num8 + 1, num9, num10);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9 + 1, num10);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color3 = new Color(r, g, 0f, 0f);
			blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
			num14 = (float)(blockLight >> 4) / num;
			num15 = (float)(blockLight & 15) / num;
			blockLight2 = World.Instance.Lighting.GetBlockLight(num11, num12 - 1, num13);
			num16 = (float)(blockLight2 >> 4) / num;
			num17 = (float)(blockLight2 & 15) / num;
			blockLight3 = World.Instance.Lighting.GetBlockLight(num11 + 1, num12, num13);
			num18 = (float)(blockLight3 >> 4) / num;
			num19 = (float)(blockLight3 & 15) / num;
			blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ);
			num20 = (float)(blockLight4 >> 4) / num;
			num21 = (float)(blockLight4 & 15) / num;
			num22 = 0f;
			if (num14 > 0f)
			{
				num22 += 1f;
			}
			if (num16 > 0f)
			{
				num22 += 1f;
			}
			if (num18 > 0f)
			{
				num22 += 1f;
			}
			if (num20 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			r = (num14 + num16 + num18 + num20) / num22;
			num22 = 0f;
			if (num15 > 0f)
			{
				num22 += 1f;
			}
			if (num17 > 0f)
			{
				num22 += 1f;
			}
			if (num19 > 0f)
			{
				num22 += 1f;
			}
			if (num21 > 0f)
			{
				num22 += 1f;
			}
			num22 = (num22 + 1f) / 1.125f;
			g = (num15 + num17 + num19 + num21) / num22;
			Color color4 = new Color(r, g, 0f, 0f);
			this.m_gmd.defaultLight[0] = color2;
			this.m_gmd.defaultLight[1] = color;
			this.m_gmd.defaultLight[2] = color4;
			this.m_gmd.defaultLight[3] = color3;
		}
	}

	private void FrontSide()
	{
		int num = (int)(this.m_gmd.defaultLight[3].r * 255f + 0.5f);
		int num2 = (int)(this.m_gmd.defaultLight[2].r * 255f + 0.5f);
		int num3 = (int)(this.m_gmd.defaultLight[1].r * 255f + 0.5f);
		int num4 = (int)(this.m_gmd.defaultLight[0].r * 255f + 0.5f);
		if (num + num3 > num2 + num4)
		{
			this.m_gmd.fliped = true;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
		}
		else
		{
			this.m_gmd.fliped = false;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
		}
	}

	private void TopSide()
	{
		int num = (int)(this.m_gmd.defaultLight[3].r * 255f + 0.5f);
		int num2 = (int)(this.m_gmd.defaultLight[2].r * 255f + 0.5f);
		int num3 = (int)(this.m_gmd.defaultLight[1].r * 255f + 0.5f);
		int num4 = (int)(this.m_gmd.defaultLight[0].r * 255f + 0.5f);
		if (num + num3 > num2 + num4)
		{
			this.m_gmd.fliped = true;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
		}
		else
		{
			this.m_gmd.fliped = false;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
		}
	}

	private void BottomSide()
	{
		int num = (int)(this.m_gmd.defaultLight[3].r * 255f + 0.5f);
		int num2 = (int)(this.m_gmd.defaultLight[2].r * 255f + 0.5f);
		int num3 = (int)(this.m_gmd.defaultLight[1].r * 255f + 0.5f);
		int num4 = (int)(this.m_gmd.defaultLight[0].r * 255f + 0.5f);
		if (num + num3 > num2 + num4)
		{
			this.m_gmd.fliped = true;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
		}
		else
		{
			this.m_gmd.fliped = false;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
		}
	}

	private void BackSide()
	{
		int num = (int)(this.m_gmd.defaultLight[3].r * 255f + 0.5f);
		int num2 = (int)(this.m_gmd.defaultLight[2].r * 255f + 0.5f);
		int num3 = (int)(this.m_gmd.defaultLight[1].r * 255f + 0.5f);
		int num4 = (int)(this.m_gmd.defaultLight[0].r * 255f + 0.5f);
		if (num + num3 > num2 + num4)
		{
			this.m_gmd.fliped = true;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
		}
		else
		{
			this.m_gmd.fliped = false;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
		}
	}

	private void RightSide()
	{
		int num = (int)(this.m_gmd.defaultLight[3].r * 255f + 0.5f);
		int num2 = (int)(this.m_gmd.defaultLight[2].r * 255f + 0.5f);
		int num3 = (int)(this.m_gmd.defaultLight[1].r * 255f + 0.5f);
		int num4 = (int)(this.m_gmd.defaultLight[0].r * 255f + 0.5f);
		if (num + num3 > num2 + num4)
		{
			this.m_gmd.fliped = true;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
		}
		else
		{
			this.m_gmd.fliped = false;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
		}
	}

	private void LeftSide()
	{
		int num = (int)(this.m_gmd.defaultLight[3].r * 255f + 0.5f);
		int num2 = (int)(this.m_gmd.defaultLight[2].r * 255f + 0.5f);
		int num3 = (int)(this.m_gmd.defaultLight[1].r * 255f + 0.5f);
		int num4 = (int)(this.m_gmd.defaultLight[0].r * 255f + 0.5f);
		if (num + num3 > num2 + num4)
		{
			this.m_gmd.fliped = true;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
		}
		else
		{
			this.m_gmd.fliped = false;
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[0].x, this.m_gmd.vert[0].z, this.m_gmd.vert[0].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[1].x, this.m_gmd.vert[1].z, this.m_gmd.vert[1].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[2].x, this.m_gmd.vert[2].z, this.m_gmd.vert[2].y));
			this.m_gmd.meshData.m_Vertices.Add(new Vector3(this.m_gmd.vert[3].x, this.m_gmd.vert[3].z, this.m_gmd.vert[3].y));
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[0]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[1]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[2]);
			this.m_gmd.meshData.m_Colors.Add(this.m_gmd.defaultLight[3]);
		}
	}

	private void GetFromVertexLight()
	{
		this.m_gmd.formLeft = new Color[]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		};
		this.m_gmd.formRight = new Color[]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		};
		this.m_gmd.formBack = new Color[]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		};
		this.m_gmd.formFront = new Color[]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		};
		this.m_gmd.formTop = new Color[]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		};
		this.m_gmd.formBottom = new Color[]
		{
			Color.white,
			Color.white,
			Color.white,
			Color.white
		};
		float num = (float)World.Instance.Lighting.LightingSteps;
		int num2 = this.m_gmd.blockX - 1;
		int num3 = this.m_gmd.blockY + 1;
		int num4 = this.m_gmd.blockZ - 1;
		int num5 = this.m_gmd.blockX - 1;
		int num6 = this.m_gmd.blockY + 1;
		int num7 = this.m_gmd.blockZ + 1;
		int num8 = this.m_gmd.blockX - 1;
		int num9 = this.m_gmd.blockY - 1;
		int num10 = this.m_gmd.blockZ + 1;
		int num11 = this.m_gmd.blockX - 1;
		int num12 = this.m_gmd.blockY - 1;
		int num13 = this.m_gmd.blockZ - 1;
		byte blockLight = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
		float num14 = (float)(blockLight >> 4) / num;
		float num15 = (float)(blockLight & 15) / num;
		byte blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3, num4 + 1);
		float num16 = (float)(blockLight2 >> 4) / num;
		float num17 = (float)(blockLight2 & 15) / num;
		byte blockLight3 = World.Instance.Lighting.GetBlockLight(num2, num3 - 1, num4);
		float num18 = (float)(blockLight3 >> 4) / num;
		float num19 = (float)(blockLight3 & 15) / num;
		byte blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX - 1, this.m_gmd.blockY, this.m_gmd.blockZ);
		float num20 = (float)(blockLight4 >> 4) / num;
		float num21 = (float)(blockLight4 & 15) / num;
		float num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		float r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		float g = (num15 + num17 + num19 + num21) / num22;
		Color color = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num5, num6, num7 - 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num5, num6 - 1, num7);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX - 1, this.m_gmd.blockY, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		Color color2 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num8, num9 + 1, num10);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9, num10 - 1);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX - 1, this.m_gmd.blockY, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		Color color3 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num11, num12, num13 + 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num11, num12 + 1, num13);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX - 1, this.m_gmd.blockY, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		Color color4 = new Color(r, g, 0f, 0f);
		this.m_gmd.formLeft[0] = color;
		this.m_gmd.formLeft[1] = color2;
		this.m_gmd.formLeft[2] = color3;
		this.m_gmd.formLeft[3] = color4;
		num2 = this.m_gmd.blockX + 1;
		num3 = this.m_gmd.blockY + 1;
		num4 = this.m_gmd.blockZ - 1;
		num5 = this.m_gmd.blockX + 1;
		num6 = this.m_gmd.blockY + 1;
		num7 = this.m_gmd.blockZ + 1;
		num8 = this.m_gmd.blockX + 1;
		num9 = this.m_gmd.blockY - 1;
		num10 = this.m_gmd.blockZ + 1;
		num11 = this.m_gmd.blockX + 1;
		num12 = this.m_gmd.blockY - 1;
		num13 = this.m_gmd.blockZ - 1;
		blockLight = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3, num4 + 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num2, num3 - 1, num4);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX + 1, this.m_gmd.blockY, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num5, num6, num7 - 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num5, num6 - 1, num7);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX + 1, this.m_gmd.blockY, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color2 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num8, num9 + 1, num10);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9, num10 - 1);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX + 1, this.m_gmd.blockY, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color3 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num11, num12, num13 + 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num11, num12 + 1, num13);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX + 1, this.m_gmd.blockY, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color4 = new Color(r, g, 0f, 0f);
		this.m_gmd.formRight[0] = color4;
		this.m_gmd.formRight[1] = color3;
		this.m_gmd.formRight[2] = color2;
		this.m_gmd.formRight[3] = color;
		num2 = this.m_gmd.blockX - 1;
		num3 = this.m_gmd.blockY + 1;
		num4 = this.m_gmd.blockZ - 1;
		num5 = this.m_gmd.blockX - 1;
		num6 = this.m_gmd.blockY + 1;
		num7 = this.m_gmd.blockZ + 1;
		num8 = this.m_gmd.blockX + 1;
		num9 = this.m_gmd.blockY + 1;
		num10 = this.m_gmd.blockZ + 1;
		num11 = this.m_gmd.blockX + 1;
		num12 = this.m_gmd.blockY + 1;
		num13 = this.m_gmd.blockZ - 1;
		blockLight = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3, num4 + 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num2 + 1, num3, num4);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY + 1, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num5, num6, num7 - 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num5 + 1, num6, num7);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY + 1, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color2 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num8 - 1, num9, num10);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9, num10 - 1);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY + 1, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color3 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num11, num12, num13 + 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num11 - 1, num12, num13);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY + 1, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color4 = new Color(r, g, 0f, 0f);
		this.m_gmd.formBack[0] = color4;
		this.m_gmd.formBack[1] = color3;
		this.m_gmd.formBack[2] = color2;
		this.m_gmd.formBack[3] = color;
		num2 = this.m_gmd.blockX - 1;
		num3 = this.m_gmd.blockY - 1;
		num4 = this.m_gmd.blockZ - 1;
		num5 = this.m_gmd.blockX - 1;
		num6 = this.m_gmd.blockY - 1;
		num7 = this.m_gmd.blockZ + 1;
		num8 = this.m_gmd.blockX + 1;
		num9 = this.m_gmd.blockY - 1;
		num10 = this.m_gmd.blockZ + 1;
		num11 = this.m_gmd.blockX + 1;
		num12 = this.m_gmd.blockY - 1;
		num13 = this.m_gmd.blockZ - 1;
		blockLight = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3, num4 + 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num2 + 1, num3, num4);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY - 1, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num5, num6, num7 - 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num5 + 1, num6, num7);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY - 1, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color2 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num8 - 1, num9, num10);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9, num10 - 1);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY - 1, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color3 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num11, num12, num13 + 1);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num11 - 1, num12, num13);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY - 1, this.m_gmd.blockZ);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color4 = new Color(r, g, 0f, 0f);
		this.m_gmd.formFront[0] = color;
		this.m_gmd.formFront[1] = color2;
		this.m_gmd.formFront[2] = color3;
		this.m_gmd.formFront[3] = color4;
		num2 = this.m_gmd.blockX - 1;
		num3 = this.m_gmd.blockY - 1;
		num4 = this.m_gmd.blockZ + 1;
		num5 = this.m_gmd.blockX - 1;
		num6 = this.m_gmd.blockY + 1;
		num7 = this.m_gmd.blockZ + 1;
		num8 = this.m_gmd.blockX + 1;
		num9 = this.m_gmd.blockY + 1;
		num10 = this.m_gmd.blockZ + 1;
		num11 = this.m_gmd.blockX + 1;
		num12 = this.m_gmd.blockY - 1;
		num13 = this.m_gmd.blockZ + 1;
		blockLight = World.Instance.Lighting.GetBlockLight(num2, num3 + 1, num4);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num2 + 1, num3, num4);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ + 1);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num5 + 1, num6, num7);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num5, num6 - 1, num7);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ + 1);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color2 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num8 - 1, num9, num10);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9 - 1, num10);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ + 1);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color3 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num11 - 1, num12, num13);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num11, num12 + 1, num13);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ + 1);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color4 = new Color(r, g, 0f, 0f);
		this.m_gmd.formTop[0] = color;
		this.m_gmd.formTop[1] = color2;
		this.m_gmd.formTop[2] = color3;
		this.m_gmd.formTop[3] = color4;
		num2 = this.m_gmd.blockX + 1;
		num3 = this.m_gmd.blockY + 1;
		num4 = this.m_gmd.blockZ - 1;
		num5 = this.m_gmd.blockX + 1;
		num6 = this.m_gmd.blockY - 1;
		num7 = this.m_gmd.blockZ - 1;
		num8 = this.m_gmd.blockX - 1;
		num9 = this.m_gmd.blockY - 1;
		num10 = this.m_gmd.blockZ - 1;
		num11 = this.m_gmd.blockX - 1;
		num12 = this.m_gmd.blockY + 1;
		num13 = this.m_gmd.blockZ - 1;
		blockLight = World.Instance.Lighting.GetBlockLight(num2, num3, num4);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num2, num3 - 1, num4);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num2 - 1, num3, num4);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ - 1);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num5, num6, num7);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num5, num6 + 1, num7);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num5 - 1, num6, num7);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ - 1);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color2 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num8, num9, num10);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num8 + 1, num9, num10);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num8, num9 + 1, num10);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ - 1);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color3 = new Color(r, g, 0f, 0f);
		blockLight = World.Instance.Lighting.GetBlockLight(num11, num12, num13);
		num14 = (float)(blockLight >> 4) / num;
		num15 = (float)(blockLight & 15) / num;
		blockLight2 = World.Instance.Lighting.GetBlockLight(num11, num12 - 1, num13);
		num16 = (float)(blockLight2 >> 4) / num;
		num17 = (float)(blockLight2 & 15) / num;
		blockLight3 = World.Instance.Lighting.GetBlockLight(num11 + 1, num12, num13);
		num18 = (float)(blockLight3 >> 4) / num;
		num19 = (float)(blockLight3 & 15) / num;
		blockLight4 = World.Instance.Lighting.GetBlockLight(this.m_gmd.blockX, this.m_gmd.blockY, this.m_gmd.blockZ - 1);
		num20 = (float)(blockLight4 >> 4) / num;
		num21 = (float)(blockLight4 & 15) / num;
		num22 = 0f;
		if (num14 > 0f)
		{
			num22 += 1f;
		}
		if (num16 > 0f)
		{
			num22 += 1f;
		}
		if (num18 > 0f)
		{
			num22 += 1f;
		}
		if (num20 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		r = (num14 + num16 + num18 + num20) / num22;
		num22 = 0f;
		if (num15 > 0f)
		{
			num22 += 1f;
		}
		if (num17 > 0f)
		{
			num22 += 1f;
		}
		if (num19 > 0f)
		{
			num22 += 1f;
		}
		if (num21 > 0f)
		{
			num22 += 1f;
		}
		num22 = (num22 + 1f) / 1.125f;
		g = (num15 + num17 + num19 + num21) / num22;
		color4 = new Color(r, g, 0f, 0f);
		this.m_gmd.formBottom[0] = color2;
		this.m_gmd.formBottom[1] = color;
		this.m_gmd.formBottom[2] = color4;
		this.m_gmd.formBottom[3] = color3;
	}

	private void AddStair()
	{
		StairAssociation association = StairAssociation.GetAssociation(this.m_gmd.blockKind);
		this.m_gmd.faceStair = association[this.m_gmd.sideIndex];
		if (this.m_gmd.faceStair == BlockFaceStair.Back || (this.m_gmd.faceStair == BlockFaceStair.Bottom && !this.m_gmd.flip) || (this.m_gmd.faceStair == BlockFaceStair.Top && this.m_gmd.flip))
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
			if (this.m_gmd.flip && this.m_gmd.faceStair == BlockFaceStair.Back)
			{
				this.m_gmd.uvY[0] = 1f;
				this.m_gmd.uvY[1] = 0f;
				this.m_gmd.uvY[2] = 0f;
				this.m_gmd.uvY[3] = 1f;
			}
			this.AddBlockSideOpt(ref this.m_gmd.blockKind);
			this.m_gmd.index = this.m_gmd.index + 4;
		}
		else if (this.m_gmd.faceStair == BlockFaceStair.Forward || (this.m_gmd.faceStair == BlockFaceStair.Top && !this.m_gmd.flip) || (this.m_gmd.faceStair == BlockFaceStair.Bottom && this.m_gmd.flip))
		{
			Vector3 v = association.Top.v1;
			Vector3 v2 = association.Top.v2;
			int[] num = association.Top.num;
			float[][] uvY = association.Top.uvY;
			float[][] uvX = association.Top.uvX;
			if (this.m_gmd.faceStair == BlockFaceStair.Forward)
			{
				uvY = association.Forward.uvY;
				uvX = association.Forward.uvX;
				v = association.Forward.v1;
				v2 = association.Forward.v2;
				num = association.Forward.num;
			}
			this.m_gmd.h = 0;
			while (this.m_gmd.h < 2)
			{
				this.m_gmd.uvX[0] = uvX[this.m_gmd.h][0];
				this.m_gmd.uvX[1] = uvX[this.m_gmd.h][1];
				this.m_gmd.uvX[2] = uvX[this.m_gmd.h][2];
				this.m_gmd.uvX[3] = uvX[this.m_gmd.h][3];
				this.m_gmd.uvY[0] = uvY[this.m_gmd.h][0];
				this.m_gmd.uvY[1] = uvY[this.m_gmd.h][1];
				this.m_gmd.uvY[2] = uvY[this.m_gmd.h][2];
				this.m_gmd.uvY[3] = uvY[this.m_gmd.h][3];
				for (int i = 0; i < 4; i++)
				{
					Vector3 a = this.formOffsets[this.m_gmd.sideIndex][i];
					if (!this.m_gmd.flip)
					{
						if (this.m_gmd.h == 0)
						{
							a += v;
							if (i == num[0] || i == num[1])
							{
								a += v2;
							}
						}
						else if (i == num[2] || i == num[3])
						{
							a += v2 * -1f;
						}
					}
					else if (this.m_gmd.h == 0)
					{
						if (this.m_gmd.faceStair == BlockFaceStair.Forward)
						{
							this.m_gmd.uvY[0] = 0.5f;
							this.m_gmd.uvY[1] = 0f;
							this.m_gmd.uvY[2] = 0f;
							this.m_gmd.uvY[3] = 0.5f;
						}
						if (this.m_gmd.blockKind == BlockKind.StairNorth || this.m_gmd.blockKind == BlockKind.StairSouth)
						{
							if (this.m_gmd.faceStair == BlockFaceStair.Bottom)
							{
								a -= v;
							}
							if (i == num[0] || i == num[1])
							{
								a += v2;
							}
						}
						else if (this.m_gmd.blockKind == BlockKind.StairWest || this.m_gmd.blockKind == BlockKind.StairEast)
						{
							if (this.m_gmd.faceStair == BlockFaceStair.Bottom)
							{
								if (i == num[0] || i == num[1])
								{
									a -= v2;
								}
							}
							else if (this.m_gmd.faceStair == BlockFaceStair.Forward && (i == num[0] || i == num[1]))
							{
								a += v2;
							}
						}
					}
					else
					{
						if (this.m_gmd.faceStair == BlockFaceStair.Forward)
						{
							this.m_gmd.uvY[0] = 1f;
							this.m_gmd.uvY[1] = 0.5f;
							this.m_gmd.uvY[2] = 0.5f;
							this.m_gmd.uvY[3] = 1f;
						}
						if (this.m_gmd.blockKind == BlockKind.StairWest || this.m_gmd.blockKind == BlockKind.StairEast)
						{
							if (this.m_gmd.faceStair == BlockFaceStair.Bottom)
							{
								if (i == num[2] || i == num[3])
								{
									a += v2;
								}
								a -= v;
							}
							else if (this.m_gmd.faceStair == BlockFaceStair.Forward && (i == num[2] || i == num[3]))
							{
								a += v2 * -1f;
							}
						}
						else if (i == num[2] || i == num[3])
						{
							a += v2 * -1f;
						}
						if (this.m_gmd.faceStair == BlockFaceStair.Forward)
						{
							a += v;
						}
					}
					this.m_gmd.vert[i].x = (float)this.m_gmd.chunkBlockX + a.x;
					this.m_gmd.vert[i].y = (float)this.m_gmd.chunkBlockY + a.y;
					this.m_gmd.vert[i].z = (float)this.m_gmd.chunkBlockZ + a.z;
				}
				this.AddBlockSideOpt(ref this.m_gmd.blockKind);
				this.m_gmd.index = this.m_gmd.index + 4;
				this.m_gmd.h = this.m_gmd.h + 1;
			}
		}
		else if (this.m_gmd.faceStair == BlockFaceStair.Right || this.m_gmd.faceStair == BlockFaceStair.Left)
		{
			Vector3 v3 = association.Right.v1;
			Vector3 v4 = association.Right.v2;
			int[] num2 = association.Right.num;
			float[][] uvY = association.Right.uvY;
			float[][] uvX = association.Right.uvX;
			if (this.m_gmd.faceStair == BlockFaceStair.Left)
			{
				uvY = association.Left.uvY;
				uvX = association.Left.uvX;
				num2 = association.Left.num;
			}
			this.m_gmd.h = 0;
			while (this.m_gmd.h < 2)
			{
				this.m_gmd.uvX[0] = uvX[this.m_gmd.h][0];
				this.m_gmd.uvX[1] = uvX[this.m_gmd.h][1];
				this.m_gmd.uvX[2] = uvX[this.m_gmd.h][2];
				this.m_gmd.uvX[3] = uvX[this.m_gmd.h][3];
				this.m_gmd.uvY[0] = uvY[this.m_gmd.h][0];
				this.m_gmd.uvY[1] = uvY[this.m_gmd.h][1];
				this.m_gmd.uvY[2] = uvY[this.m_gmd.h][2];
				this.m_gmd.uvY[3] = uvY[this.m_gmd.h][3];
				for (int j = 0; j < 4; j++)
				{
					Vector3 a2 = this.formOffsets[this.m_gmd.sideIndex][j];
					if (!this.m_gmd.flip)
					{
						if (this.m_gmd.h == 0)
						{
							if (j == num2[0] || j == num2[1])
							{
								a2 += v3;
							}
							if (j == num2[0] || j == num2[2])
							{
								a2 += v4;
							}
						}
						else if (j == num2[3] || j == num2[2])
						{
							a2 += v3 * -1f;
						}
					}
					else if (this.m_gmd.h == 0)
					{
						if (j == num2[0] || j == num2[1])
						{
							a2 += v3;
						}
						if (j == num2[1] || j == num2[3])
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
						if (j == num2[3] || j == num2[2])
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
					this.m_gmd.vert[j].x = (float)this.m_gmd.chunkBlockX + a2.x;
					this.m_gmd.vert[j].y = (float)this.m_gmd.chunkBlockY + a2.y;
					this.m_gmd.vert[j].z = (float)this.m_gmd.chunkBlockZ + a2.z;
				}
				this.AddBlockSideOpt(ref this.m_gmd.blockKind);
				this.m_gmd.index = this.m_gmd.index + 4;
				this.m_gmd.h = this.m_gmd.h + 1;
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
			this.AddBlockSideOpt(ref this.m_gmd.blockKind);
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
			this.AddBlockSideOpt(ref this.m_gmd.blockKind);
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
			this.AddBlockSideOpt(ref this.m_gmd.blockKind);
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
			this.AddBlockSideOpt(ref this.m_gmd.blockKind);
			this.m_gmd.index = this.m_gmd.index + 4;
			this.m_gmd.h = this.m_gmd.h + 1;
		}
	}

	private void AddHalfUV()
	{
		if ((this.m_gmd.blockKind == BlockKind.HalfWallSouth || this.m_gmd.blockKind == BlockKind.HalfWallNorth) && (this.m_gmd.blockFace == BlockFace.Top || this.m_gmd.blockFace == BlockFace.Bottom))
		{
			float[] array = new float[4];
			array[1] = 0.5f;
			array[2] = 0.5f;
			this.m_gmd.uvY = array;
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
				RuntimeHelpers.InitializeArray(uvY = new float[4], fieldof(_003CPrivateImplementationDetails_003E._0024_0024field-80).FieldHandle);
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
		if ((this.m_gmd.blockKind == BlockKind.QuarterOnWallSouth || this.m_gmd.blockKind == BlockKind.QuarterOnWallNorth) && (this.m_gmd.blockFace == BlockFace.Top || this.m_gmd.blockFace == BlockFace.Bottom))
		{
			float[] array = new float[4];
			array[1] = 0.25f;
			array[2] = 0.25f;
			this.m_gmd.uvY = array;
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
		if ((this.m_gmd.blockKind == BlockKind.ThirdOnWallSouth || this.m_gmd.blockKind == BlockKind.ThirdOnWallNorth) && (this.m_gmd.blockFace == BlockFace.Top || this.m_gmd.blockFace == BlockFace.Bottom))
		{
			float[] array = new float[4];
			array[1] = 0.75f;
			array[2] = 0.75f;
			this.m_gmd.uvY = array;
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
			if (this.m_gmd.blockKind == BlockKind.DiagonalEast || this.m_gmd.blockKind == BlockKind.DiagonalNorth || this.m_gmd.blockKind == BlockKind.DiagonalSouth || this.m_gmd.blockKind == BlockKind.DiagonalWest || this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestTop || this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthBottom || this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthBottom || this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastBottom || this.m_gmd.blockKind == BlockKind.DiagonalOnWallWestBottom)
			{
				if (!this.m_gmd.flip && this.m_gmd.blockKind != BlockKind.DiagonalOnWallSouthBottom && this.m_gmd.blockKind != BlockKind.DiagonalOnWallNorthBottom && this.m_gmd.blockKind != BlockKind.DiagonalOnWallEastBottom && this.m_gmd.blockKind != BlockKind.DiagonalOnWallWestBottom)
				{
					if (this.m_gmd.sideIndex == uvnumber[0])
					{
						float[] array = new float[4];
						array[1] = 1f;
						array[2] = 1f;
						this.m_gmd.uvY = array;
					}
					else if (this.m_gmd.sideIndex == uvnumber[1])
					{
						this.m_gmd.uvX = new float[]
						{
							0f,
							0f,
							0f,
							1f
						};
					}
				}
				else if (this.m_gmd.sideIndex == uvnumber[0])
				{
					float[] array2 = new float[4];
					array2[0] = 1f;
					array2[2] = 1f;
					this.m_gmd.uvX = array2;
				}
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
				float[] array3 = new float[4];
				array3[1] = 1f;
				this.m_gmd.uvY = array3;
			}
			if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallEastLeft && this.m_gmd.blockFace == BlockFace.Top)
			{
				float[] array4 = new float[4];
				array4[0] = 1f;
				this.m_gmd.uvY = array4;
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
				float[] array5 = new float[4];
				array5[1] = 1f;
				this.m_gmd.uvY = array5;
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
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallSouthLeft && this.m_gmd.blockFace == BlockFace.Top)
		{
			float[] array6 = new float[4];
			array6[0] = 1f;
			array6[2] = 1f;
			this.m_gmd.uvX = array6;
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthRight && this.m_gmd.blockFace == BlockFace.Bottom)
		{
			float[] array7 = new float[4];
			array7[0] = 1f;
			array7[2] = 1f;
			this.m_gmd.uvX = array7;
		}
		if (this.m_gmd.blockKind == BlockKind.DiagonalOnWallNorthLeft && this.m_gmd.blockFace == BlockFace.Top)
		{
			float[] array8 = new float[4];
			array8[0] = 1f;
			array8[2] = 1f;
			this.m_gmd.uvX = array8;
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

	private float Interpolation(float arg1, float arg2, float func1, float func2, float arg)
	{
		return (func2 - func1) * (arg - arg1) / (arg2 - arg1) + func1;
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

	private readonly MeshGeneratorSmothLight.GenerateCondition[] generateTypeAssociate = new MeshGeneratorSmothLight.GenerateCondition[]
	{
		new MeshGeneratorSmothLight.GenerateCondition(BlockComparer.IsLand),
		new MeshGeneratorSmothLight.GenerateCondition(BlockComparer.IsWater),
		new MeshGeneratorSmothLight.GenerateCondition(BlockComparer.IsGlass)
	};

	private MeshGeneratorSmothLight.GenMeshData m_gmd = default(MeshGeneratorSmothLight.GenMeshData);

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
			this.fliped = false;
			this.defaultLight = new Color[4];
			this.formBack = new Color[4];
			this.formBottom = new Color[4];
			this.formFront = new Color[4];
			this.formLeft = new Color[4];
			this.formRight = new Color[4];
			this.formTop = new Color[4];
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

		public MeshGeneratorSmothLight.GenerateType generateType;

		public MeshGeneratorSmothLight.GenerateCondition condition;

		public int blockX;

		public int blockY;

		public int blockZ;

		public int chunkBlockX;

		public int chunkBlockY;

		public int chunkBlockZ;

		public int index;

		public bool isEdge;

		public bool water;

		public bool flip;

		public bool fliped;

		public byte lightAmount;

		public Color[] defaultLight;

		public Color[] formLeft;

		public Color[] formRight;

		public Color[] formFront;

		public Color[] formBack;

		public Color[] formTop;

		public Color[] formBottom;

		public int h;

		public int sideIndex;

		public BlockFace blockFace;

		public BlockType blockType;

		public BlockKind blockKind;

		public BlockFaceStair faceStair;

		public Vector3[] vert;

		public float[] uvX;

		public float[] uvY;
	}

	private delegate bool GenerateCondition(BlockType block);
}
