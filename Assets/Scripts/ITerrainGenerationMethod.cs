using System;

public interface ITerrainGenerationMethod
{
	void GenerateTerrain(WorldData worldData, Chunk bottomChunk, int noiseBlockOffset);
}
