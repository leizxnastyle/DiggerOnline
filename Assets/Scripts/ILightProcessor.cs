using System;

public interface ILightProcessor
{
	int LightingSteps { get; }

	void InitializeLighting();

	void InitializeLightingChunk(Chunk chunk);

	void AddLight(int x, int y, int z, bool recalculateLighting);

	void RemoveLight(int x, int y, int z, bool recalculateLighting);

	bool IsLight(int x, int y, int z);

	void RecalculateLightingAroundBlock(int x, int y, int z, int radius, bool lightChanged);

	void RecalculateLighting(int x1, int x2, int y1, int y2, int z1, int z2);

	byte GetBlockLight(int x, int y, int z);
}
