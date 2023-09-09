using System;

internal class FlatLand : ITerrainGenerationMethod
{
	public void GenerateTerrain(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		this.LandPass(worldData, bottomChunk, noiseBlockOffset);
	}

	private void LandPass(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset)
	{
		int num = bottomChunk.X * worldData.ChunkBlockWidth;
		int num2 = bottomChunk.Y * worldData.ChunkBlockHeight;
		for (int i = 0; i < worldData.ChunkBlockWidth; i++)
		{
			for (int j = 0; j < worldData.ChunkBlockHeight; j++)
			{
				for (int k = worldData.DepthInBlocks - 1; k >= 0; k--)
				{
					BlockType blockType;
					if (k > 25)
					{
						blockType = BlockType.Air;
					}
					else if (k < 25 && k >= 20)
					{
						blockType = BlockType.Dirt;
					}
					else if (k < 20)
					{
						blockType = BlockType.Stone;
					}
					else
					{
						blockType = BlockType.TopSoil;
					}
					WorldData.Instance.SetBlockType(num + i, num2 + j, k, blockType);
				}
			}
		}
	}
}
