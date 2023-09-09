using System;

public interface ITerrainGenerator
{
	void GenerateChunkTerrain(PChunkList chunks);

	void GenerateTerrain(Chunk chunk);
}
