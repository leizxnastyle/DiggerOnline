using System;

internal class SandLand : ITerrainGenerationMethod
{
	public void GenerateTerrain(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		this.lowerGround = new float[worldData.ChunkBlockWidth, worldData.ChunkBlockHeight];
		this.LandPass(worldData, bottomChunk, noiseBlockOffset);
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
				float lowerGroundHeight = SandLand.GetLowerGroundHeight(num3, num4, worldData.DepthInBlocks, noiseBlockOffset);
				this.lowerGround[i, j] = lowerGroundHeight;
				for (int k = worldData.DepthInBlocks - 1; k >= 0; k--)
				{
					BlockType blockType;
					if ((float)k > lowerGroundHeight)
					{
						blockType = BlockType.Air;
					}
					else
					{
						blockType = BlockType.Sand;
					}
					WorldData.Instance.SetBlockType(num3, num4, k, blockType);
				}
			}
		}
	}

	private static float GetLowerGroundHeight(int blockX, int blockY, int worldDepthInBlocks, int noiseBlockOffset)
	{
		int num = worldDepthInBlocks / 4;
		int num2 = (int)((float)worldDepthInBlocks * 0.5f);
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
}
