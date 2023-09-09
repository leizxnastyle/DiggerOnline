using System;

public class TerrainGenerator : ITerrainGenerator
{
	public TerrainGenerator(WorldData worldData, IBatchProcessor<Chunk> batchProcessor, ITerrainGenerationMethod terrainGenerationMethod)
	{
		this.m_WorldData = worldData;
		this.m_BatchProcessor = batchProcessor;
		this.m_TerrainGenerationMethod = terrainGenerationMethod;
	}

	public void GenerateTerrain(Chunk chunk)
	{
		if (chunk.Z == 0)
		{
			this.m_TerrainGenerationMethod.GenerateTerrain(this.m_WorldData, chunk, (int)this.m_WorldData.NoiseBlockXOffset);
		}
	}

	public void GenerateChunkTerrain(PChunkList chunks)
	{
		PChunkList pchunkList = new PChunkList();
		foreach (Chunk chunk in chunks)
		{
			if (chunk.Z == 0)
			{
				pchunkList.Add(chunk);
			}
		}
		this.m_BatchProcessor.Process(pchunkList, new Action<Chunk>(this.GenerateTerrain), true);
	}

	private readonly WorldData m_WorldData;

	private readonly IBatchProcessor<Chunk> m_BatchProcessor;

	private readonly ITerrainGenerationMethod m_TerrainGenerationMethod;
}
