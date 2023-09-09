using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NewMeshGenerator : IMeshGenerator
{
	public NewMeshGenerator(IBatchProcessor<Chunk> batchProcessor, WorldData worldData)
	{
		this.batchProcessor = batchProcessor;
		this.worldData = worldData;
	}

	public void GenerateLandMeshes(PChunkList chunks)
	{
		this.batchProcessor.Process(1, chunks, new Action<Chunk>(this.GenerateLandMesh), false);
	}

	public void GenerateLandMesh(Chunk chunk)
	{
		this.GenerateMesh(chunk.LandData, chunk, NewMeshGenerator.GenerateType.Land);
		this.worldData.AddFinishedLandChunk(chunk);
	}

	public void GenerateWaterMeshes(PChunkList chunks)
	{
		this.batchProcessor.Process(1, chunks, new Action<Chunk>(this.GenerateWaterMesh), false);
	}

	public void GenerateWaterMesh(Chunk chunk)
	{
		this.GenerateMesh(chunk.WaterData, chunk, NewMeshGenerator.GenerateType.Water);
		this.worldData.AddFinishedWaterChunk(chunk);
	}

	public void GenerateGlassMeshes(PChunkList chunks)
	{
		this.batchProcessor.Process(1, chunks, new Action<Chunk>(this.GenerateGlassMesh), false);
	}

	public void GenerateGlassMesh(Chunk chunk)
	{
		this.GenerateMesh(chunk.GlassData, chunk, NewMeshGenerator.GenerateType.Glass);
		this.worldData.AddFinishedGlassChunk(chunk);
	}

	public Chunk.MeshData[] GenerateMeshData(int x1, int x2, int y1, int y2, int z1, int z2, BlockGetter getter, BlockKindGetter kindGetter)
	{
		UnityEngine.Debug.Log("zahodit...");
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
					IntVect intVect = new IntVect(i, j, k);
					for (int l = 0; l < 3; l++)
					{
						array2[l] = this.CreateMeshData(intVect, intVect, null, array[l], array2[l], getter, kindGetter, (NewMeshGenerator.GenerateType)l);
					}
				}
			}
		}
		return array;
	}

	private void ClearData(Chunk.MeshData mData)
	{
		mData.m_Vertices.Clear();
		mData.m_Indices.Clear();
		mData.m_Uvs.Clear();
		mData.m_Colors.Clear();
	}

	private void GenerateMesh(Chunk.MeshData meshData, Chunk chunk, NewMeshGenerator.GenerateType typeData)
	{
		int index = 0;
		this.ClearData(meshData);
		IntVect right = chunk.Position.MultiplyComponent(this.worldData.ChunkBlockVolume);
		for (int i = 0; i < this.worldData.ChunkBlockWidth; i++)
		{
			for (int j = 0; j < this.worldData.ChunkBlockHeight; j++)
			{
				for (int k = 0; k < this.worldData.ChunkBlockDepth; k++)
				{
					IntVect intVect = new IntVect(i, j, k);
					IntVect block = intVect + right;
					index = this.CreateMeshData(block, intVect, chunk, meshData, index, null, null, typeData);
				}
			}
		}
	}

	private int CreateMeshData(IntVect block, IntVect chunkBlock, Chunk chunk, Chunk.MeshData data, int index, BlockGetter getter, BlockKindGetter kindGetter, NewMeshGenerator.GenerateType generateType)
	{
		if (getter == null)
		{
			IntVect right = this.worldData.ChunkBlockVolume - IntVect.One;
			bool isEdge = chunkBlock.CompareOr(IntVect.Zero) || chunkBlock.CompareOr(right);
			IntVect delta = block - chunkBlock;
			getter = ((int x, int y, int z) => (!isEdge) ? chunk.GetBlockType(x - delta.X, y - delta.Y, z - delta.Z) : this.worldData.GetBlockType(x, y, z));
			kindGetter = ((int x, int y, int z) => (!isEdge) ? chunk.GetBlockKind(x - delta.X, y - delta.Y, z - delta.Z) : this.worldData.GetBlockKind(x, y, z));
		}
		bool flag = generateType == NewMeshGenerator.GenerateType.Water;
		BlockType blockType;
		BlockKind blockKind;
		if (flag)
		{
			blockType = ((chunk != null) ? chunk.GetBlockType(chunkBlock.X, chunkBlock.Y, chunkBlock.Z) : this.worldData.GetBlockType(block.X, block.Y, block.Z));
			blockKind = ((chunk != null) ? chunk.GetBlockKind(chunkBlock.X, chunkBlock.Y, chunkBlock.Z) : this.worldData.GetBlockKind(block.X, block.Y, block.Z));
			if (!blockType.IsAir() && !blockType.IsGlass() && blockKind.IsDefault())
			{
				return index;
			}
		}
		else
		{
			blockType = getter(block.X, block.Y, block.Z);
			blockKind = kindGetter(block.X, block.Y, block.Z);
			if (blockType.IsLand() && blockKind.IsDefault())
			{
				return index;
			}
		}
		NewMeshGenerator.GenerateCondition generateCondition = this.generateTypeAssociate[(int)generateType];
		byte blockLight = World.Instance.Lighting.GetBlockLight(block.X, block.Y, block.Z);
		if (flag)
		{
			BlockType block2 = getter(block.X, block.Y, block.Z + 1);
			bool flag2 = !block2.IsWater() && !block2.IsAir();
			BlockType block3 = getter(block.X, block.Y, block.Z - 1);
			bool flag3 = !block3.IsWater() && !block3.IsAir();
			for (int i = 0; i < 6; i++)
			{
				IntVect a = (i < 2) ? IntVect.Right : ((i < 4) ? IntVect.Forward : IntVect.Up);
				int d = ((i + 1) % 2 == 0) ? 1 : -1;
				IntVect intVect = block + a * d;
				BlockType blockType2 = getter(intVect.X, intVect.Y, intVect.Z);
				BlockKind blockKind2 = kindGetter(intVect.X, intVect.Y, intVect.Z);
				if (generateCondition(blockType2))
				{
					BlockFace blockFace = (i < 4) ? BlockFace.Side : ((i == 4) ? BlockFace.Top : BlockFace.Bottom);
					block2 = getter(intVect.X, intVect.Y, intVect.Z + 1);
					block3 = getter(intVect.X, intVect.Y, intVect.Z - 1);
					bool flag4 = blockKind2.IsFlip();
					if (flag4)
					{
						blockKind2 = blockKind2.DoFlip();
					}
					float num = 0f;
					float num2 = 0f;
					Vector3[] array = new Vector3[4];
					float[] uvY = null;
					float[] uvX = null;
					if (i < 4)
					{
						num2 = ((flag2 || (!block2.IsWater() && !block2.IsAir())) ? 0.1f : 0f);
						num = ((flag3 || (!block3.IsWater() && !block3.IsAir())) ? 0.1f : 0f);
					}
					for (int j = 0; j < 4; j++)
					{
						Vector3 a2 = Vector3.zero;
						Vector3 b = this.offsets[i][j];
						if (i < 4)
						{
							a2 += ((j == 0 || j == 3) ? new Vector3(0f, 0f, num - 0.1f) : new Vector3(0f, 0f, num2 - 0.1f));
						}
						else
						{
							a2 += new Vector3(0f, 0f, -0.1f);
						}
						Vector3 vector = chunkBlock + b;
						array[j] = new Vector3(a2.x + vector.x, a2.y + vector.y, a2.z + vector.z);
					}
					this.AddBlockSide(array, data, index, blockType2, blockFace, blockLight, flag, uvY, uvX, false, blockKind2, i);
					index += 4;
				}
			}
			return index;
		}
		if (!blockKind.IsDefault())
		{
			bool flag5 = blockKind.IsFlip();
			if (flag5)
			{
				blockKind = blockKind.DoFlip();
			}
			for (int k = 0; k < 6; k++)
			{
				if (generateCondition(blockType))
				{
					BlockFace blockFace2 = (k < 4) ? BlockFace.Side : ((k == 4) ? BlockFace.Top : BlockFace.Bottom);
					float[] uvY2 = null;
					float[] uvX2 = null;
					if (blockKind.IsThird())
					{
						this.AddThridUV(ref blockKind, ref blockFace2, ref k, ref uvX2, ref uvY2, ref flag5);
					}
					if (blockKind.IsHalf())
					{
						this.AddHalfUV(ref blockKind, ref blockFace2, ref k, ref uvX2, ref uvY2, ref flag5);
					}
					if (blockKind.IsQuarter())
					{
						this.AddQuarterUV(ref blockKind, ref blockFace2, ref k, ref uvX2, ref uvY2, ref flag5);
					}
					if (blockKind.IsDiagonal())
					{
						this.AddDiagonalUV(ref blockKind, ref blockFace2, ref k, ref uvX2, ref uvY2, ref flag5);
					}
					if (blockKind.IsFence())
					{
						if (blockKind == BlockKind.Fence)
						{
							if (blockFace2 == BlockFace.Top || blockFace2 == BlockFace.Bottom)
							{
								uvY2 = new float[]
								{
									0.25f,
									0.75f,
									0.75f,
									0.25f
								};
								uvX2 = new float[]
								{
									0.25f,
									0.25f,
									0.75f,
									0.75f
								};
							}
							else if (blockFace2 == BlockFace.Side)
							{
								uvX2 = new float[]
								{
									0.25f,
									0.25f,
									0.75f,
									0.75f
								};
							}
						}
						else if (blockKind == BlockKind.FenceOnWallEastWest)
						{
							if (k < 2)
							{
								uvY2 = new float[]
								{
									0.25f,
									0.75f,
									0.75f,
									0.25f
								};
								uvX2 = new float[]
								{
									0.25f,
									0.25f,
									0.75f,
									0.75f
								};
							}
							else if (k > 1)
							{
								uvY2 = new float[]
								{
									0.75f,
									0.25f,
									0.75f,
									0.25f
								};
								uvX2 = new float[]
								{
									0f,
									0f,
									1f,
									1f
								};
							}
						}
						else if (blockKind == BlockKind.FenceOnWallSouthNorth)
						{
							if (k > 1 && k < 4)
							{
								uvY2 = new float[]
								{
									0.25f,
									0.75f,
									0.75f,
									0.25f
								};
								uvX2 = new float[]
								{
									0.25f,
									0.25f,
									0.75f,
									0.75f
								};
							}
							else if (k < 2)
							{
								uvX2 = new float[]
								{
									0.75f,
									0.25f,
									0.75f,
									0.25f
								};
								uvY2 = new float[]
								{
									0f,
									0f,
									1f,
									1f
								};
							}
							else if (k > 3)
							{
								uvX2 = new float[]
								{
									0.25f,
									0.75f,
									0.25f,
									0.75f
								};
								float[] array2 = new float[4];
								array2[1] = 1f;
								array2[2] = 1f;
								uvY2 = array2;
							}
						}
					}
					Vector3 zero = Vector3.zero;
					Vector3[] array3 = new Vector3[4];
					Vector3 vector2 = Vector3.zero;
					if (blockKind.IsStair())
					{
						StairAssociation association = StairAssociation.GetAssociation(blockKind);
						BlockFaceStair blockFaceStair = association[k];
						if (blockFaceStair == BlockFaceStair.Back || (blockFaceStair == BlockFaceStair.Bottom && !flag5) || (blockFaceStair == BlockFaceStair.Top && flag5))
						{
							for (int l = 0; l < 4; l++)
							{
								array3[l] = chunkBlock + this.formOffsets[k][l];
								if (flag5 && blockFaceStair == BlockFaceStair.Back)
								{
									uvY2 = new float[]
									{
										1f,
										0f,
										0f,
										1f
									};
								}
							}
							this.AddBlockSide(array3, data, index, blockType, blockFace2, blockLight, flag, uvY2, uvX2, false, blockKind, k);
							index += 4;
						}
						else if (blockFaceStair == BlockFaceStair.Forward || (blockFaceStair == BlockFaceStair.Top && !flag5) || (blockFaceStair == BlockFaceStair.Bottom && flag5))
						{
							Vector3 v = association.Top.v1;
							Vector3 v2 = association.Top.v2;
							int[] num3 = association.Top.num;
							float[][] uvY3 = association.Top.uvY;
							float[][] uvX3 = association.Top.uvX;
							if (blockFaceStair == BlockFaceStair.Forward)
							{
								uvY3 = association.Forward.uvY;
								uvX3 = association.Forward.uvX;
								v = association.Forward.v1;
								v2 = association.Forward.v2;
								num3 = association.Forward.num;
							}
							for (int m = 0; m < 2; m++)
							{
								uvX2 = uvX3[m];
								uvY2 = uvY3[m];
								for (int n = 0; n < 4; n++)
								{
									vector2 = this.formOffsets[k][n];
									if (!flag5)
									{
										if (m == 0)
										{
											vector2 += v;
											if (n == num3[0] || n == num3[1])
											{
												vector2 += v2;
											}
										}
										else if (n == num3[2] || n == num3[3])
										{
											vector2 += v2 * -1f;
										}
									}
									else if (m == 0)
									{
										if (blockFaceStair == BlockFaceStair.Forward)
										{
											uvY2 = new float[]
											{
												0.5f,
												0f,
												0f,
												0.5f
											};
										}
										if (blockKind == BlockKind.StairNorth || blockKind == BlockKind.StairSouth)
										{
											if (blockFaceStair == BlockFaceStair.Bottom)
											{
												vector2 -= v;
											}
											if (n == num3[0] || n == num3[1])
											{
												vector2 += v2;
											}
										}
										else if (blockKind == BlockKind.StairWest || blockKind == BlockKind.StairEast)
										{
											if (blockFaceStair == BlockFaceStair.Bottom)
											{
												if (n == num3[0] || n == num3[1])
												{
													vector2 -= v2;
												}
											}
											else if (blockFaceStair == BlockFaceStair.Forward && (n == num3[0] || n == num3[1]))
											{
												vector2 += v2;
											}
										}
									}
									else
									{
										if (blockFaceStair == BlockFaceStair.Forward)
										{
											uvY2 = new float[]
											{
												1f,
												0.5f,
												0.5f,
												1f
											};
										}
										if (blockKind == BlockKind.StairWest || blockKind == BlockKind.StairEast)
										{
											if (blockFaceStair == BlockFaceStair.Bottom)
											{
												if (n == num3[2] || n == num3[3])
												{
													vector2 += v2;
												}
												vector2 -= v;
											}
											else if (blockFaceStair == BlockFaceStair.Forward && (n == num3[2] || n == num3[3]))
											{
												vector2 += v2 * -1f;
											}
										}
										else if (n == num3[2] || n == num3[3])
										{
											vector2 += v2 * -1f;
										}
										if (blockFaceStair == BlockFaceStair.Forward)
										{
											vector2 += v;
										}
									}
									array3[n] = chunkBlock + vector2;
								}
								this.AddBlockSide(array3, data, index, blockType, blockFace2, blockLight, flag, uvY2, uvX2, false, blockKind, k);
								index += 4;
							}
						}
						else if (blockFaceStair == BlockFaceStair.Right || blockFaceStair == BlockFaceStair.Left)
						{
							Vector3 v3 = association.Right.v1;
							Vector3 v4 = association.Right.v2;
							int[] num4 = association.Right.num;
							float[][] uvY3 = association.Right.uvY;
							float[][] uvX3 = association.Right.uvX;
							if (blockFaceStair == BlockFaceStair.Left)
							{
								uvY3 = association.Left.uvY;
								uvX3 = association.Left.uvX;
								num4 = association.Left.num;
							}
							for (int num5 = 0; num5 < 2; num5++)
							{
								uvX2 = uvX3[num5];
								uvY2 = uvY3[num5];
								for (int num6 = 0; num6 < 4; num6++)
								{
									vector2 = this.formOffsets[k][num6];
									if (!flag5)
									{
										if (num5 == 0)
										{
											if (num6 == num4[0] || num6 == num4[1])
											{
												vector2 += v3;
											}
											if (num6 == num4[0] || num6 == num4[2])
											{
												vector2 += v4;
											}
										}
										else if (num6 == num4[3] || num6 == num4[2])
										{
											vector2 += v3 * -1f;
										}
									}
									else if (num5 == 0)
									{
										if (num6 == num4[0] || num6 == num4[1])
										{
											vector2 += v3;
										}
										if (num6 == num4[1] || num6 == num4[3])
										{
											vector2 -= v4;
										}
										uvY2 = new float[]
										{
											0.5f,
											0f,
											0f,
											0.5f
										};
									}
									else
									{
										if (num6 == num4[3] || num6 == num4[2])
										{
											vector2 += v3 * -1f;
										}
										uvY2 = new float[]
										{
											1f,
											0f,
											0f,
											1f
										};
									}
									array3[num6] = chunkBlock + vector2;
								}
								this.AddBlockSide(array3, data, index, blockType, blockFace2, blockLight, flag, uvY2, uvX2, false, blockKind, k);
								index += 4;
							}
						}
					}
					for (int num7 = 0; num7 < 4; num7++)
					{
						vector2 = this.formOffsets[k][num7];
						if (blockKind.IsThird())
						{
							this.AddThrid(ref blockKind, ref k, ref num7, ref zero, ref flag5);
						}
						if (blockKind.IsHalf())
						{
							this.AddHalf(ref blockKind, ref k, ref num7, ref zero, ref flag5);
						}
						if (blockKind.IsQuarter())
						{
							this.AddQuarter(ref blockKind, ref k, ref num7, ref zero, ref flag5);
						}
						if (blockKind.IsDiagonal())
						{
							this.AddDiagonal(ref blockKind, ref k, ref num7, ref zero, ref flag5);
						}
						if (blockKind.IsFence())
						{
							if (blockKind == BlockKind.Fence)
							{
								vector2 = this.offsetsFence[k][num7];
							}
							else if (blockKind == BlockKind.FenceOnWallEastWest)
							{
								vector2 = this.offsetsFenceEastWest[k][num7];
							}
							else if (blockKind == BlockKind.FenceOnWallSouthNorth)
							{
								vector2 = this.offsetsFenceSouthNorth[k][num7];
							}
						}
						Vector3 vector3 = chunkBlock + vector2;
						array3[num7] = new Vector3(zero.x + vector3.x, zero.y + vector3.y, zero.z + vector3.z);
						zero = Vector3.zero;
					}
					if (!blockKind.IsStair())
					{
						this.AddBlockSide(array3, data, index, blockType, blockFace2, blockLight, flag, uvY2, uvX2, false, blockKind, k);
						index += 4;
					}
				}
			}
		}
		for (int num8 = 0; num8 < 6; num8++)
		{
			IntVect a3 = (num8 < 2) ? IntVect.Right : ((num8 < 4) ? IntVect.Forward : IntVect.Up);
			int d2 = ((num8 + 1) % 2 == 0) ? 1 : -1;
			IntVect intVect2 = block + a3 * d2;
			BlockType blockType3 = getter(intVect2.X, intVect2.Y, intVect2.Z);
			BlockKind blockKind3 = kindGetter(intVect2.X, intVect2.Y, intVect2.Z);
			if (blockKind3.IsDefault())
			{
				bool flag6 = blockKind3.IsFlip();
				if (flag6)
				{
					blockKind3 = blockKind3.DoFlip();
				}
				if (generateCondition(blockType3))
				{
					BlockFace blockFace3 = (num8 < 4) ? BlockFace.Side : ((num8 == 4) ? BlockFace.Top : BlockFace.Bottom);
					float[] uvY4 = null;
					float[] uvX4 = null;
					Vector3[] array4 = new Vector3[4];
					for (int num9 = 0; num9 < 4; num9++)
					{
						Vector3 zero2 = Vector3.zero;
						Vector3 b2 = this.offsets[num8][num9];
						Vector3 vector4 = chunkBlock + b2;
						array4[num9] = new Vector3(zero2.x + vector4.x, zero2.y + vector4.y, zero2.z + vector4.z);
					}
					this.AddBlockSide(array4, data, index, blockType3, blockFace3, blockLight, flag, uvY4, uvX4, false, blockKind3, num8);
					index += 4;
				}
			}
		}
		return index;
	}

	private void AddHalf(ref BlockKind blockKind, ref int i, ref int j, ref Vector3 vec, ref bool flip)
	{
		if (blockKind == BlockKind.HalfWallSouth)
		{
			if (i == 0 && j > 1)
			{
				vec += new Vector3(0f, 0.5f, 0f);
			}
			if (i == 1 && j < 2)
			{
				vec += new Vector3(0f, 0.5f, 0f);
			}
			if ((i == 4 && j == 3) || (i == 4 && j == 0))
			{
				vec += new Vector3(0f, 0.5f, 0f);
			}
			if (i == 2)
			{
				vec += new Vector3(0f, 0.5f, 0f);
			}
			if ((i == 5 && j == 3) || (i == 5 && j == 0))
			{
				vec += new Vector3(0f, 0.5f, 0f);
			}
		}
		if (blockKind == BlockKind.HalfWallNorth)
		{
			if (i == 0 && j < 2)
			{
				vec -= new Vector3(0f, 0.5f, 0f);
			}
			if (i == 3)
			{
				vec -= new Vector3(0f, 0.5f, 0f);
			}
			if (i == 5 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.5f, 0f);
			}
			if (i == 4 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.5f, 0f);
			}
			if (i == 1 && j > 1)
			{
				vec -= new Vector3(0f, 0.5f, 0f);
			}
		}
		if (blockKind == BlockKind.HalfWallEast)
		{
			if (i == 2 && j < 2)
			{
				vec += new Vector3(0.5f, 0f, 0f);
			}
			if (i == 0)
			{
				vec += new Vector3(0.5f, 0f, 0f);
			}
			if (i == 3 && j > 1)
			{
				vec += new Vector3(0.5f, 0f, 0f);
			}
			if (i == 5 && j > 1)
			{
				vec += new Vector3(0.5f, 0f, 0f);
			}
			if (i == 4 && j < 2)
			{
				vec += new Vector3(0.5f, 0f, 0f);
			}
		}
		if (blockKind == BlockKind.HalfWallWest)
		{
			if (i == 2 && j > 1)
			{
				vec -= new Vector3(0.5f, 0f, 0f);
			}
			if (i == 1)
			{
				vec -= new Vector3(0.5f, 0f, 0f);
			}
			if (i == 3 && j < 2)
			{
				vec -= new Vector3(0.5f, 0f, 0f);
			}
			if (i == 5 && j < 2)
			{
				vec -= new Vector3(0.5f, 0f, 0f);
			}
			if (i == 4 && j > 1)
			{
				vec -= new Vector3(0.5f, 0f, 0f);
			}
		}
		if (blockKind == BlockKind.Half)
		{
			if (!flip)
			{
				if ((i < 4 && (j == 1 || j == 2)) || i == 4)
				{
					vec -= new Vector3(0f, 0f, 0.5f);
				}
			}
			else if ((i < 4 && (j == 0 || j == 3)) || i == 5)
			{
				vec += new Vector3(0f, 0f, 0.5f);
			}
		}
	}

	private void AddHalfUV(ref BlockKind curKind, ref BlockFace face, ref int i, ref float[] uvX, ref float[] uvY, ref bool flip)
	{
		if (curKind == BlockKind.HalfWallSouth || curKind == BlockKind.HalfWallNorth)
		{
			if (face == BlockFace.Top || face == BlockFace.Bottom)
			{
				float[] array = new float[4];
				array[1] = 0.5f;
				array[2] = 0.5f;
				uvY = array;
			}
			if (face == BlockFace.Side && i < 2)
			{
				uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.5f
				};
				uvX = new float[]
				{
					0f,
					0f,
					0.5f,
					1f
				};
			}
		}
		if (curKind == BlockKind.HalfWallWest || curKind == BlockKind.HalfWallEast)
		{
			if (face == BlockFace.Top || face == BlockFace.Bottom)
			{
				uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.5f
				};
				uvX = new float[]
				{
					0f,
					0f,
					0.5f,
					1f
				};
			}
			if (face == BlockFace.Side && i > 1 && i < 4)
			{
				uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.5f
				};
				uvX = new float[]
				{
					0f,
					0f,
					0.5f,
					1f
				};
			}
		}
		if (curKind == BlockKind.Half && face == BlockFace.Side)
		{
			float[] array3;
			if (!flip)
			{
				float[] array2 = new float[4];
				array2[1] = 0.5f;
				array3 = array2;
				array2[2] = 0.5f;
			}
			else
			{
				RuntimeHelpers.InitializeArray(array3 = new float[4], fieldof(_003CPrivateImplementationDetails_003E._0024_0024field-96).FieldHandle);
			}
			uvY = array3;
		}
	}

	private void AddQuarter(ref BlockKind blockKind, ref int i, ref int j, ref Vector3 vec, ref bool flip)
	{
		if (blockKind == BlockKind.Quarter)
		{
			if (!flip)
			{
				if ((i < 4 && (j == 1 || j == 2)) || i == 4)
				{
					vec -= new Vector3(0f, 0f, 0.75f);
				}
			}
			else if ((i < 4 && (j == 0 || j == 3)) || i == 5)
			{
				vec += new Vector3(0f, 0f, 0.75f);
			}
		}
		if (blockKind == BlockKind.QuarterOnWallSouth)
		{
			if (i == 0 && j > 1)
			{
				vec += new Vector3(0f, 0.75f, 0f);
			}
			if (i == 1 && j < 2)
			{
				vec += new Vector3(0f, 0.75f, 0f);
			}
			if ((i == 4 && j == 3) || (i == 4 && j == 0))
			{
				vec += new Vector3(0f, 0.75f, 0f);
			}
			if (i == 2)
			{
				vec += new Vector3(0f, 0.75f, 0f);
			}
			if ((i == 5 && j == 3) || (i == 5 && j == 0))
			{
				vec += new Vector3(0f, 0.75f, 0f);
			}
		}
		if (blockKind == BlockKind.QuarterOnWallNorth)
		{
			if (i == 0 && j < 2)
			{
				vec -= new Vector3(0f, 0.75f, 0f);
			}
			if (i == 3)
			{
				vec -= new Vector3(0f, 0.75f, 0f);
			}
			if (i == 5 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.75f, 0f);
			}
			if (i == 4 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.75f, 0f);
			}
			if (i == 1 && j > 1)
			{
				vec -= new Vector3(0f, 0.75f, 0f);
			}
		}
		if (blockKind == BlockKind.QuarterOnWallEast)
		{
			if (i == 2 && j < 2)
			{
				vec += new Vector3(0.75f, 0f, 0f);
			}
			if (i == 0)
			{
				vec += new Vector3(0.75f, 0f, 0f);
			}
			if (i == 3 && j > 1)
			{
				vec += new Vector3(0.75f, 0f, 0f);
			}
			if (i == 5 && j > 1)
			{
				vec += new Vector3(0.75f, 0f, 0f);
			}
			if (i == 4 && j < 2)
			{
				vec += new Vector3(0.75f, 0f, 0f);
			}
		}
		if (blockKind == BlockKind.QuarterOnWallWest)
		{
			if (i == 2 && j > 1)
			{
				vec -= new Vector3(0.75f, 0f, 0f);
			}
			if (i == 1)
			{
				vec -= new Vector3(0.75f, 0f, 0f);
			}
			if (i == 3 && j < 2)
			{
				vec -= new Vector3(0.75f, 0f, 0f);
			}
			if (i == 5 && j < 2)
			{
				vec -= new Vector3(0.75f, 0f, 0f);
			}
			if (i == 4 && j > 1)
			{
				vec -= new Vector3(0.75f, 0f, 0f);
			}
		}
	}

	private void AddQuarterUV(ref BlockKind curKind, ref BlockFace face, ref int i, ref float[] uvX, ref float[] uvY, ref bool flip)
	{
		if (curKind == BlockKind.QuarterOnWallSouth || curKind == BlockKind.QuarterOnWallNorth)
		{
			if (face == BlockFace.Top || face == BlockFace.Bottom)
			{
				float[] array = new float[4];
				array[1] = 0.25f;
				array[2] = 0.25f;
				uvY = array;
			}
			if (face == BlockFace.Side && i < 2)
			{
				uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.25f
				};
				uvX = new float[]
				{
					0f,
					0f,
					0.25f,
					1f
				};
			}
		}
		if (curKind == BlockKind.QuarterOnWallWest || curKind == BlockKind.QuarterOnWallEast)
		{
			if (face == BlockFace.Top || face == BlockFace.Bottom)
			{
				uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.25f
				};
				uvX = new float[]
				{
					0f,
					0f,
					0.25f,
					1f
				};
			}
			if (face == BlockFace.Side && i > 1 && i < 4)
			{
				uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.25f
				};
				uvX = new float[]
				{
					0f,
					0f,
					0.25f,
					1f
				};
			}
		}
		if (curKind == BlockKind.Quarter && face == BlockFace.Side)
		{
			float[] array3;
			if (!flip)
			{
				float[] array2 = new float[4];
				array2[1] = 0.25f;
				array3 = array2;
				array2[2] = 0.25f;
			}
			else
			{
				float[] array4 = new float[4];
				array4[0] = 0.25f;
				array3 = array4;
				array4[3] = 0.25f;
			}
			uvY = array3;
		}
	}

	private void AddThrid(ref BlockKind blockKind, ref int i, ref int j, ref Vector3 vec, ref bool flip)
	{
		if (blockKind == BlockKind.Third)
		{
			if (!flip)
			{
				if ((i < 4 && (j == 1 || j == 2)) || i == 4)
				{
					vec -= new Vector3(0f, 0f, 0.25f);
				}
			}
			else if ((i < 4 && (j == 0 || j == 3)) || i == 5)
			{
				vec += new Vector3(0f, 0f, 0.25f);
			}
		}
		if (blockKind == BlockKind.ThirdOnWallSouth)
		{
			if (i == 0 && j > 1)
			{
				vec += new Vector3(0f, 0.25f, 0f);
			}
			if (i == 1 && j < 2)
			{
				vec += new Vector3(0f, 0.25f, 0f);
			}
			if ((i == 4 && j == 3) || (i == 4 && j == 0))
			{
				vec += new Vector3(0f, 0.25f, 0f);
			}
			if (i == 2)
			{
				vec += new Vector3(0f, 0.25f, 0f);
			}
			if ((i == 5 && j == 3) || (i == 5 && j == 0))
			{
				vec += new Vector3(0f, 0.25f, 0f);
			}
		}
		if (blockKind == BlockKind.ThirdOnWallNorth)
		{
			if (i == 0 && j < 2)
			{
				vec -= new Vector3(0f, 0.25f, 0f);
			}
			if (i == 3)
			{
				vec -= new Vector3(0f, 0.25f, 0f);
			}
			if (i == 5 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.25f, 0f);
			}
			if (i == 4 && j > 0 && j < 3)
			{
				vec -= new Vector3(0f, 0.25f, 0f);
			}
			if (i == 1 && j > 1)
			{
				vec -= new Vector3(0f, 0.25f, 0f);
			}
		}
		if (blockKind == BlockKind.ThirdOnWallEast)
		{
			if (i == 2 && j < 2)
			{
				vec += new Vector3(0.25f, 0f, 0f);
			}
			if (i == 0)
			{
				vec += new Vector3(0.25f, 0f, 0f);
			}
			if (i == 3 && j > 1)
			{
				vec += new Vector3(0.25f, 0f, 0f);
			}
			if (i == 5 && j > 1)
			{
				vec += new Vector3(0.25f, 0f, 0f);
			}
			if (i == 4 && j < 2)
			{
				vec += new Vector3(0.25f, 0f, 0f);
			}
		}
		if (blockKind == BlockKind.ThirdOnWallWest)
		{
			if (i == 2 && j > 1)
			{
				vec -= new Vector3(0.25f, 0f, 0f);
			}
			if (i == 1)
			{
				vec -= new Vector3(0.25f, 0f, 0f);
			}
			if (i == 3 && j < 2)
			{
				vec -= new Vector3(0.25f, 0f, 0f);
			}
			if (i == 5 && j < 2)
			{
				vec -= new Vector3(0.25f, 0f, 0f);
			}
			if (i == 4 && j > 1)
			{
				vec -= new Vector3(0.25f, 0f, 0f);
			}
		}
	}

	private void AddThridUV(ref BlockKind curKind, ref BlockFace face, ref int i, ref float[] uvX, ref float[] uvY, ref bool flip)
	{
		if (curKind == BlockKind.ThirdOnWallSouth || curKind == BlockKind.ThirdOnWallNorth)
		{
			if (face == BlockFace.Top || face == BlockFace.Bottom)
			{
				float[] array = new float[4];
				array[1] = 0.75f;
				array[2] = 0.75f;
				uvY = array;
			}
			if (face == BlockFace.Side && i < 2)
			{
				uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.75f
				};
				uvX = new float[]
				{
					0f,
					0f,
					0.75f,
					1f
				};
			}
		}
		if (curKind == BlockKind.ThirdOnWallWest || curKind == BlockKind.ThirdOnWallEast)
		{
			if (face == BlockFace.Top || face == BlockFace.Bottom)
			{
				uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.75f
				};
				uvX = new float[]
				{
					0f,
					0f,
					0.75f,
					1f
				};
			}
			if (face == BlockFace.Side && i > 1 && i < 4)
			{
				uvY = new float[]
				{
					1f,
					0f,
					0f,
					0.75f
				};
				uvX = new float[]
				{
					0f,
					0f,
					0.75f,
					1f
				};
			}
		}
		if (curKind == BlockKind.Third && face == BlockFace.Side)
		{
			float[] array3;
			if (!flip)
			{
				float[] array2 = new float[4];
				array2[1] = 0.75f;
				array3 = array2;
				array2[2] = 0.75f;
			}
			else
			{
				float[] array4 = new float[4];
				array4[0] = 0.75f;
				array3 = array4;
				array4[3] = 0.75f;
			}
			uvY = array3;
		}
	}

	private void AddDiagonal(ref BlockKind blockKind, ref int i, ref int j, ref Vector3 vec, ref bool flip)
	{
		DiagonalAssociation association = DiagonalAssociation.GetAssociation(blockKind);
		if (blockKind == BlockKind.DiagonalWest || blockKind == BlockKind.DiagonalOnWallEastBottom || blockKind == BlockKind.DiagonalOnWallEastTop)
		{
			if (!flip && blockKind != BlockKind.DiagonalOnWallEastBottom)
			{
				if (i == association.I[0] && (j == association.J[0] || j == association.J[1]))
				{
					vec += Vector3.left;
				}
				else if (i == association.I[1] && j == association.J[1])
				{
					vec += Vector3.left;
				}
				else if (i == association.I[2] && j == association.J[0])
				{
					vec += Vector3.left;
				}
				else if (i == 4 && (j == association.J[2] || j == association.J[3]))
				{
					vec += Vector3.left;
				}
			}
			else if (i == association.I[0] && (j == association.FlipJ[0] || j == association.FlipJ[1]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
			else if (i == association.I[1] && j == association.FlipJ[1])
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
			else if (i == association.I[2] && j == association.FlipJ[0])
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
			else if (i == 5 && (j == association.FlipJ[2] || j == association.FlipJ[3]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
		}
		if (blockKind == BlockKind.DiagonalEast || blockKind == BlockKind.DiagonalOnWallWestBottom || blockKind == BlockKind.DiagonalOnWallWestTop)
		{
			if (!flip && blockKind != BlockKind.DiagonalOnWallWestBottom)
			{
				if (i == association.I[0] && (j == association.J[0] || j == association.J[1]))
				{
					vec += Vector3.right;
				}
				else if (i == association.I[1] && j == association.J[0])
				{
					vec += Vector3.right;
				}
				else if (i == association.I[2] && j == association.J[1])
				{
					vec += Vector3.right;
				}
				else if (i == 4 && (j == association.J[2] || j == association.J[3]))
				{
					vec += Vector3.right;
				}
			}
			else if (i == association.I[0] && (j == association.FlipJ[0] || j == association.FlipJ[1]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
			else if (i == association.I[1] && j == association.FlipJ[1])
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (i == association.I[2] && j == association.FlipJ[0])
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (i == 5 && (j == association.FlipJ[2] || j == association.FlipJ[3]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
		}
		if (blockKind == BlockKind.DiagonalSouth || blockKind == BlockKind.DiagonalOnWallSouthBottom || blockKind == BlockKind.DiagonalOnWallSouthTop)
		{
			if (!flip && blockKind != BlockKind.DiagonalOnWallSouthBottom)
			{
				if (i == association.I[0] && (j == association.J[0] || j == association.J[1]))
				{
					vec += Vector3.up;
				}
				else if (i == association.I[1] && j == association.J[0])
				{
					vec += Vector3.up;
				}
				else if (i == association.I[2] && j == association.J[1])
				{
					vec += Vector3.up;
				}
				else if (i == 4 && (j == association.J[2] || j == association.J[3]))
				{
					vec += Vector3.up;
				}
			}
			else if (i == association.I[0] && (j == association.FlipJ[0] || j == association.FlipJ[1]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
			else if (i == association.I[1] && j == association.FlipJ[1])
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == association.I[2] && j == association.FlipJ[0])
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == 5 && (j == association.FlipJ[2] || j == association.FlipJ[3]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
		}
		if (blockKind == BlockKind.DiagonalNorth || blockKind == BlockKind.DiagonalOnWallNorthBottom || blockKind == BlockKind.DiagonalOnWallNorthTop)
		{
			if (!flip && blockKind != BlockKind.DiagonalOnWallNorthBottom)
			{
				if (i == association.I[0] && (j == association.J[0] || j == association.J[1]))
				{
					vec += Vector3.down;
				}
				else if (i == association.I[1] && j == association.J[0])
				{
					vec += Vector3.down;
				}
				else if (i == association.I[2] && j == association.J[1])
				{
					vec += Vector3.down;
				}
				else if (i == 4 && (j == association.J[2] || j == association.J[3]))
				{
					vec += Vector3.down;
				}
			}
			else if (i == association.I[0] && (j == association.FlipJ[0] || j == association.FlipJ[1]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
			else if (i == association.I[1] && j == association.FlipJ[1])
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (i == association.I[2] && j == association.FlipJ[0])
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (i == 5 && (j == association.FlipJ[2] || j == association.FlipJ[3]))
			{
				vec += new Vector3(0f, 0f, 1f);
			}
		}
		if (blockKind == BlockKind.DiagonalOnWallEastLeft)
		{
			if (i == association.I[0] && (j == association.J[0] || j == association.J[1]))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == association.I[1] && j == 3)
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == association.I[2] && j == association.J[0])
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == 2 && (j == association.J[2] || j == association.J[3]))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
		}
		if (blockKind == BlockKind.DiagonalOnWallWestLeft)
		{
			if (i == association.I[0] && (j == association.J[0] || j == association.J[1]))
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (i == association.I[1] && j == association.J[0])
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (i == association.I[2] && j == 2)
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (i == 3 && (j == association.J[2] || j == association.J[3]))
			{
				vec += new Vector3(0f, -1f, 0f);
			}
		}
		if (blockKind == BlockKind.DiagonalOnWallSouthLeft)
		{
			if (i == 2 && (j == 0 || j == 1))
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (i == 4 && j == 0)
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (i == 5 && j == 3)
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (i == 0 && (j == 2 || j == 3))
			{
				vec += new Vector3(1f, 0f, 0f);
			}
		}
		if (blockKind == BlockKind.DiagonalOnWallNorthLeft)
		{
			if (i == 3 && (j == 0 || j == 1))
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
			else if (i == 4 && j == 2)
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
			else if (i == 5 && j == 1)
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
			else if (i == 1 && (j == 2 || j == 3))
			{
				vec += new Vector3(-1f, 0f, 0f);
			}
		}
		if (blockKind == BlockKind.DiagonalOnWallEastRight)
		{
			if (i == association.I[0] && (j == association.J[0] || j == association.J[1]))
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (i == association.I[1] && j == association.J[0])
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (i == association.I[2] && j == 1)
			{
				vec += new Vector3(0f, -1f, 0f);
			}
			else if (i == 3 && (j == association.J[2] || j == association.J[3]))
			{
				vec += new Vector3(0f, -1f, 0f);
			}
		}
		if (blockKind == BlockKind.DiagonalOnWallWestRight)
		{
			if (i == association.I[0] && (j == association.J[0] || j == association.J[1]))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == association.I[1] && j == 0)
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == association.I[2] && j == association.J[1])
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == 2 && (j == association.J[2] || j == association.J[3]))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
		}
		if (blockKind == BlockKind.DiagonalOnWallSouthRight)
		{
			if (i == 2 && (j == 2 || j == 3))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == 4 && j == 3)
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == 5 && j == 0)
			{
				vec += new Vector3(0f, 1f, 0f);
			}
			else if (i == 1 && (j == 0 || j == 1))
			{
				vec += new Vector3(0f, 1f, 0f);
			}
		}
		if (blockKind == BlockKind.DiagonalOnWallNorthRight)
		{
			if (i == 3 && (j == 2 || j == 3))
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (i == 4 && j == 1)
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (i == 5 && j == 2)
			{
				vec += new Vector3(1f, 0f, 0f);
			}
			else if (i == 0 && (j == 0 || j == 1))
			{
				vec += new Vector3(1f, 0f, 0f);
			}
		}
	}

	private void AddDiagonalUV(ref BlockKind curKind, ref BlockFace face, ref int i, ref float[] uvX, ref float[] uvY, ref bool flip)
	{
		if (curKind.IsDiagonal() && face == BlockFace.Side)
		{
			int[] uvnumber = DiagonalAssociation.GetUVNumber(curKind);
			if ((curKind == BlockKind.DiagonalEast || curKind == BlockKind.DiagonalNorth || curKind == BlockKind.DiagonalSouth || curKind == BlockKind.DiagonalWest || curKind == BlockKind.DiagonalOnWallSouthTop || curKind == BlockKind.DiagonalOnWallNorthTop || curKind == BlockKind.DiagonalOnWallEastTop || curKind == BlockKind.DiagonalOnWallWestTop || curKind == BlockKind.DiagonalOnWallSouthBottom || curKind == BlockKind.DiagonalOnWallNorthBottom || curKind == BlockKind.DiagonalOnWallEastBottom || curKind == BlockKind.DiagonalOnWallWestBottom) && !flip && curKind != BlockKind.DiagonalOnWallSouthBottom && curKind != BlockKind.DiagonalOnWallNorthBottom && curKind != BlockKind.DiagonalOnWallEastBottom && curKind != BlockKind.DiagonalOnWallWestBottom && i == uvnumber[0])
			{
				float[] array = new float[4];
				array[1] = 1f;
				array[2] = 1f;
				uvY = array;
			}
		}
		else if ((face == BlockFace.Top || face == BlockFace.Bottom) && (curKind == BlockKind.DiagonalOnWallWestRight || curKind == BlockKind.DiagonalOnWallWestLeft || curKind == BlockKind.DiagonalOnWallEastLeft || curKind == BlockKind.DiagonalOnWallEastRight || curKind == BlockKind.DiagonalOnWallSouthRight))
		{
			if (curKind == BlockKind.DiagonalOnWallWestRight && face == BlockFace.Top)
			{
				uvY = new float[]
				{
					0f,
					0f,
					0f,
					1f
				};
			}
			if (curKind == BlockKind.DiagonalOnWallWestLeft && face == BlockFace.Bottom)
			{
				float[] array2 = new float[4];
				array2[1] = 1f;
				uvY = array2;
			}
			if (curKind == BlockKind.DiagonalOnWallEastLeft && face == BlockFace.Top)
			{
				float[] array3 = new float[4];
				array3[0] = 1f;
				uvY = array3;
			}
			if (curKind == BlockKind.DiagonalOnWallEastLeft && face == BlockFace.Bottom)
			{
				uvY = new float[]
				{
					0f,
					0f,
					0f,
					1f
				};
			}
			if (curKind == BlockKind.DiagonalOnWallEastRight && face == BlockFace.Top)
			{
				float[] array4 = new float[4];
				array4[1] = 1f;
				uvY = array4;
			}
			if (curKind == BlockKind.DiagonalOnWallSouthRight && face == BlockFace.Bottom)
			{
				uvY = new float[]
				{
					0f,
					0f,
					0f,
					1f
				};
			}
		}
	}

	private void AddBlockSide(Vector3[] vert, Chunk.MeshData data, int index, BlockType blockType, BlockFace blockFace, byte blockLight, bool fullUV, float[] uvY = null, float[] uvX = null, bool flip = false, BlockKind blockKind = BlockKind.Default, int sideNum = 0)
	{
		if (uvX == null)
		{
			uvX = new float[]
			{
				0f,
				0f,
				1f,
				1f
			};
		}
		if (uvY == null)
		{
			uvY = new float[]
			{
				0f,
				1f,
				1f,
				0f
			};
		}
		float num = (float)World.Instance.Lighting.LightingSteps;
		float r = (float)(blockLight >> 4) / num;
		float g = (float)(blockLight & 15) / num;
		Color item = new Color(r, g, 0f, 0f);
		foreach (Vector3 vector in vert)
		{
			data.m_Vertices.Add(new Vector3(vector.x, vector.z, vector.y));
			data.m_Colors.Add(item);
		}
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
			Rect rect = this.worldData.BlockUvCoordinates[blockType].BlockFaceUvCoordinates[(int)blockFace];
			if (flip && blockFace != BlockFace.Side)
			{
				int num2 = (blockFace != BlockFace.Bottom) ? 2 : 0;
				rect = this.worldData.BlockUvCoordinates[blockType].BlockFaceUvCoordinates[num2];
			}
			if ((blockKind == BlockKind.HalfWallSouth && sideNum < 2) || (blockKind == BlockKind.HalfWallNorth && sideNum < 2) || (blockKind == BlockKind.HalfWallEast && ((sideNum > 1 && sideNum < 4) || blockFace != BlockFace.Side)) || (blockKind == BlockKind.HalfWallWest && ((sideNum > 1 && sideNum < 4) || blockFace != BlockFace.Side)) || ((blockKind == BlockKind.QuarterOnWallSouth && sideNum < 2) || (blockKind == BlockKind.QuarterOnWallNorth && sideNum < 2) || (blockKind == BlockKind.QuarterOnWallEast && ((sideNum > 1 && sideNum < 4) || blockFace != BlockFace.Side))) || (blockKind == BlockKind.QuarterOnWallWest && ((sideNum > 1 && sideNum < 4) || blockFace != BlockFace.Side)) || ((blockKind == BlockKind.ThirdOnWallSouth && sideNum < 2) || (blockKind == BlockKind.ThirdOnWallNorth && sideNum < 2) || (blockKind == BlockKind.ThirdOnWallEast && ((sideNum > 1 && sideNum < 4) || blockFace != BlockFace.Side))) || (blockKind == BlockKind.ThirdOnWallWest && ((sideNum > 1 && sideNum < 4) || blockFace != BlockFace.Side)))
			{
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvY[0], rect.y + rect.height * uvX[0]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[1], rect.y + rect.height * uvY[1]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvY[2], rect.y + rect.height * uvX[2]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[3], rect.y + rect.height * uvY[3]));
			}
			else if (blockKind == BlockKind.FenceOnWallEastWest && sideNum > 1)
			{
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvY[1], rect.y + rect.height * uvX[0]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvY[0], rect.y + rect.height * uvX[1]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvY[2], rect.y + rect.height * uvX[2]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvY[3], rect.y + rect.height * uvX[3]));
			}
			else if (blockKind == BlockKind.FenceOnWallSouthNorth && sideNum < 2)
			{
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[1], rect.y + rect.height * uvY[0]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[0], rect.y + rect.height * uvY[1]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[2], rect.y + rect.height * uvY[2]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[3], rect.y + rect.height * uvY[3]));
			}
			else if (blockKind == BlockKind.FenceOnWallSouthNorth && sideNum > 3)
			{
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[0], rect.y + rect.height * uvY[0]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[2], rect.y + rect.height * uvY[1]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[1], rect.y + rect.height * uvY[2]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[3], rect.y + rect.height * uvY[3]));
			}
			else
			{
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[0], rect.y + rect.height * uvY[0]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[1], rect.y + rect.height * uvY[1]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[2], rect.y + rect.height * uvY[2]));
				data.m_Uvs.Add(new Vector2(rect.x + rect.width * uvX[3], rect.y + rect.height * uvY[3]));
			}
		}
	}

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

	private readonly NewMeshGenerator.GenerateCondition[] generateTypeAssociate = new NewMeshGenerator.GenerateCondition[]
	{
		new NewMeshGenerator.GenerateCondition(BlockComparer.IsLand),
		new NewMeshGenerator.GenerateCondition(BlockComparer.IsWater),
		new NewMeshGenerator.GenerateCondition(BlockComparer.IsGlass)
	};

	private enum GenerateType
	{
		Land,
		Water,
		Glass
	}

	private delegate bool GenerateCondition(BlockType block);
}
