using System;
using System.Collections.Generic;

public class WorldDecorator : IWorldDecorator
{
	public WorldDecorator(WorldData worldData, IBatchProcessor<Chunk> batchProcessor, List<IDecoration> decorations)
	{
		this.m_BatchProcessor = batchProcessor;
		this.m_Decorations = decorations;
	}

	public void GenerateWorldDecorations(PChunkList chunks)
	{
		this.m_BatchProcessor.Process(chunks, new Action<Chunk>(this.GenerateDecorationsForChunk), true);
	}

	public void GenerateDecorationsForChunk(Chunk chunk)
	{
		RandomAdv random = new RandomAdv();
		foreach (IntVect intVect in chunk.TopSoilBlocks)
		{
			foreach (IDecoration decoration in this.m_Decorations)
			{
				decoration.Decorate(intVect.X, intVect.Y, intVect.Z, random);
			}
		}
	}

	private readonly IBatchProcessor<Chunk> m_BatchProcessor;

	private readonly List<IDecoration> m_Decorations;
}
