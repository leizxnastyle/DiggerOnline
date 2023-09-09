using System;

public interface IMeshGenerator
{
	void GenerateLandMeshes(PChunkList chunks);

	void GenerateLandMesh(Chunk chunk);

	void GenerateWaterMeshes(PChunkList chunks);

	void GenerateWaterMesh(Chunk chunk);

	void GenerateGlassMeshes(PChunkList chunks);

	void GenerateGlassMesh(Chunk chunk);

	Chunk.MeshData[] GenerateMeshData(int x1, int x2, int y1, int y2, int z1, int z2, BlockGetter blockGetter, BlockKindGetter kindGetter = null);
}
