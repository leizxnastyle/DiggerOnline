using System;

internal class DualLayerTerrainWithMediumValleysForRivers : ITerrainGenerationMethod
{
	public void GenerateTerrain(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		int num = (int)((float)worldData.DepthInBlocks * 0.5f);
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			int num2 = bottomChunk.X * worldData.ChunkBlockWidth + i + noiseBlockOffset;
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				int num3 = bottomChunk.Y * worldData.ChunkBlockHeight + j;
				float lowerGroundHeight = DualLayerTerrainWithMediumValleysForRivers.GetLowerGroundHeight(num2, num3, worldData.DepthInBlocks);
				int upperGroundHeight = DualLayerTerrainWithMediumValleysForRivers.GetUpperGroundHeight(worldData, num2, num3, lowerGroundHeight);
				bool flag = true;
				for (int k = worldData.DepthInBlocks - 1; k >= 0; k--)
				{
					BlockType blockType;
					if (k > upperGroundHeight)
					{
						blockType = BlockType.Air;
					}
					else if ((float)k > lowerGroundHeight)
					{
						float num4 = PerlinSimplexNoise.noise((float)num2 * 0.01f, (float)num3 * 0.01f, (float)k * 0.01f) * (0.015f * (float)k) + 0.1f;
						num4 += PerlinSimplexNoise.noise((float)num2 * 0.01f, (float)num3 * 0.01f, (float)k * 0.1f) * 0.06f + 0.1f;
						num4 += PerlinSimplexNoise.noise((float)num2 * 0.2f, (float)num3 * 0.2f, (float)k * 0.2f) * 0.03f + 0.01f;
						if (num4 > 0.2f)
						{
							blockType = BlockType.Air;
						}
						else if (flag)
						{
							blockType = BlockType.TopSoil;
							flag = false;
						}
						else
						{
							blockType = BlockType.Dirt;
						}
					}
					else if (flag)
					{
						blockType = BlockType.TopSoil;
						flag = false;
					}
					else
					{
						blockType = BlockType.Dirt;
					}
					if (blockType == BlockType.Air && k <= num)
					{
						blockType = BlockType.Lava;
					}
					WorldData.Instance.SetBlockType(num2, num3, k, blockType);
				}
			}
		}
	}

	private static int GetUpperGroundHeight(WorldData worldData, int blockX, int blockY, float lowerGroundHeight)
	{
		float num = PerlinSimplexNoise.noise((float)(blockX + 100) * 0.001f, (float)blockY * 0.001f) * 0.5f;
		float num2 = PerlinSimplexNoise.noise((float)(blockX + 100) * 0.002f, (float)blockY * 0.002f) * 0.25f;
		float num3 = PerlinSimplexNoise.noise((float)(blockX + 100) * 0.01f, (float)blockY * 0.01f) * 0.25f;
		float num4 = num + num2 + num3;
		return (int)(num4 * ((float)worldData.DepthInBlocks / 2f)) + (int)lowerGroundHeight;
	}

	private static float GetLowerGroundHeight(int blockX, int blockY, int worldDepthInBlocks)
	{
		int num = worldDepthInBlocks / 4;
		int num2 = (int)((float)worldDepthInBlocks * 0.5f);
		float num3 = PerlinSimplexNoise.noise((float)blockX * 0.0001f, (float)blockY * 0.0001f) * 0.5f;
		float num4 = PerlinSimplexNoise.noise((float)blockX * 0.0005f, (float)blockY * 0.0005f) * 0.35f;
		float num5 = PerlinSimplexNoise.noise((float)blockX * 0.02f, (float)blockY * 0.02f) * 0.15f;
		float num6 = num3 + num4 + num5;
		num6 = num6 * (float)num2 + (float)num;
		for (int i = (int)num6; i >= 0; i--)
		{
			WorldData.Instance.SetBlockType(blockX, blockY, i, BlockType.Dirt);
		}
		return num6;
	}
}
