using System;

internal class AutumnLand : ITerrainGenerationMethod
{
	public void GenerateTerrain(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		this.lowerGround = new float[worldData.ChunkBlockWidth, worldData.ChunkBlockHeight];
		this.upperGround = new float[worldData.ChunkBlockWidth, worldData.ChunkBlockHeight];
		this.LandPass(worldData, bottomChunk, noiseBlockOffset);
	}

	private void CoalIronResource(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		int num = bottomChunk.X * worldData.ChunkBlockWidth;
		int num2 = bottomChunk.Y * worldData.ChunkBlockHeight;
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			int num3 = num + i;
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				int num4 = num2 + j;
				for (int k = worldData.DepthInBlocks - 1; k >= 0; k--)
				{
					int num5 = num3 + noiseBlockOffset;
					float num6 = PerlinSimplexNoise.noise((float)num5 * 0.08f, (float)num4 * 0.08f, (float)k * 0.08f) * 0.25f;
					if ((float)k < this.lowerGround[i, j] - 5f || (float)k >= this.upperGround[i, j])
					{
						if ((float)k < this.lowerGround[i, j] - 15f && num6 < -0.234f && WorldData.Instance.GetBlockType(num3, num4, k) == BlockType.Stone)
						{
							BlockType blockType = BlockType.Gold;
							WorldData.Instance.SetBlockType(num3, num4, k, blockType);
						}
					}
				}
			}
		}
	}

	private void CaveSystem(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		int num = bottomChunk.X * worldData.ChunkBlockWidth;
		int num2 = bottomChunk.Y * worldData.ChunkBlockHeight;
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			int num3 = num + i;
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				int num4 = num2 + j;
				for (int k = worldData.DepthInBlocks - 1; k >= 0; k--)
				{
					if ((float)k <= this.lowerGround[i, j])
					{
						int num5 = num3 + noiseBlockOffset;
						float num6 = PerlinSimplexNoise.noise((float)num5 * 0.04f, (float)num4 * 0.04f, (float)k * 0.04f) * 0.7f;
						num6 += PerlinSimplexNoise.noise((float)num5 * 0.02f, (float)num4 * 0.02f, (float)k * 0.02f) * 0.25f;
						if (num6 < -0.5f)
						{
							BlockType blockType = BlockType.Air;
							WorldData.Instance.SetBlockType(num3, num4, k, blockType);
						}
					}
				}
			}
		}
	}

	private void LandPass(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		int num = bottomChunk.X * worldData.ChunkBlockWidth;
		int num2 = bottomChunk.Y * worldData.ChunkBlockHeight;
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			int num3 = num + i;
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				int num4 = num2 + j;
				float lowerGroundHeight = AutumnLand.GetLowerGroundHeight(num3, num4, i, j, worldData.DepthInBlocks, noiseBlockOffset);
				int upperGroundHeight = AutumnLand.GetUpperGroundHeight(worldData, num3, num4, lowerGroundHeight, noiseBlockOffset);
				this.lowerGround[i, j] = lowerGroundHeight;
				this.upperGround[i, j] = (float)upperGroundHeight;
				bool flag = true;
				for (int k = worldData.DepthInBlocks - 1; k >= 0; k--)
				{
					BlockType blockType;
					if (k > upperGroundHeight)
					{
						blockType = BlockType.Air;
					}
					else if ((float)k > lowerGroundHeight - 5f)
					{
						int num5 = num3 + noiseBlockOffset;
						float num6 = PerlinSimplexNoise.noise((float)num5 * 0.009f, (float)num4 * 0.009f, (float)k * 0.0009f) * 0.25f;
						num6 += PerlinSimplexNoise.noise((float)num5 * 0.04f, (float)num4 * 0.04f, (float)k * 0.01f) * 0.05f;
						num6 += PerlinSimplexNoise.noise((float)num5 * 0.01f, (float)num4 * 0.01f, (float)k * 0.01f) * 0.15f;
						if (num6 < 0.12f)
						{
							blockType = BlockType.Air;
						}
						else if (flag)
						{
							blockType = BlockType.Autumn1;
							bottomChunk.TopSoilBlocks.Add(new IntVect(num3, num4, k));
							flag = false;
						}
						else
						{
							blockType = BlockType.Stone;
						}
					}
					else
					{
						if (flag)
						{
							blockType = BlockType.Autumn1;
							bottomChunk.TopSoilBlocks.Add(new IntVect(num3, num4, k));
							flag = false;
						}
						else
						{
							blockType = BlockType.Dirt;
						}
						if ((float)k < lowerGroundHeight - 15f)
						{
							blockType = BlockType.Stone;
						}
					}
					WorldData.Instance.SetBlockType(num3, num4, k, blockType);
				}
			}
		}
	}

	private static int GetUpperGroundHeight(WorldData worldData, int blockX, int blockY, float lowerGroundHeight, int noiseBlockOffset)
	{
		int num = blockX + noiseBlockOffset;
		float num2 = PerlinSimplexNoise.noise((float)num * 0.001f, (float)blockY * 0.001f) * 0.5f;
		float num3 = PerlinSimplexNoise.noise((float)(num + 100) * 0.002f, (float)blockY * 0.002f) * 0.25f;
		float num4 = PerlinSimplexNoise.noise((float)(num + 100) * 0.01f, (float)blockY * 0.01f) * 0.25f;
		float num5 = num2 + num3 + num4;
		return (int)(num5 * ((float)worldData.DepthInBlocks / 3f)) + (int)lowerGroundHeight;
	}

	private static float GetLowerGroundHeight(int blockX, int blockY, int blockXInChunk, int blockYInChunk, int worldDepthInBlocks, int noiseBlockOffset)
	{
		int num = worldDepthInBlocks / 4;
		int num2 = (int)((float)worldDepthInBlocks * 0.75f);
		float num3 = PerlinSimplexNoise.noise((float)blockX * 5E-05f, (float)blockY * 5E-05f) * 0.5f;
		float num4 = PerlinSimplexNoise.noise((float)blockX * 0.0005f, (float)blockY * 0.0005f) * 0.25f;
		float num5 = PerlinSimplexNoise.noise((float)blockX * 0.005f, (float)blockY * 0.005f) * 0.12f;
		float num6 = PerlinSimplexNoise.noise((float)blockX * 0.01f, (float)blockY * 0.01f) * 0.12f;
		float num7 = PerlinSimplexNoise.noise((float)blockX * 0.03f, (float)blockY * 0.03f) * num6;
		float num8 = num3 + num4 + num5 + num6 + num7;
		num8 = num8 * (float)num2 + (float)num;
		for (int i = (int)num8; i >= 0; i--)
		{
			WorldData.Instance.SetBlockType(blockX, blockY, i, BlockType.Dirt);
		}
		return num8 - 20f;
	}

	private float[,] lowerGround;

	private float[,] upperGround;

	private float[,] stoneGround;
}
