using System;

public interface IWorldDecorator
{
	void GenerateWorldDecorations(PChunkList chunks);

	void GenerateDecorationsForChunk(Chunk chunk);
}
