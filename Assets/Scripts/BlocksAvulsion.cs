using System;
using System.Collections;
using UnityEngine;

public class BlocksAvulsion : MonoBehaviour
{
	public static void VerifyAvulsionAroundBlocks(int x, int y, int z, int radius, bool silent)
	{
		if (App.Instance.Settings.gameType == GameINI.GameType.BUILDING)
		{
			return;
		}
		if (BlocksAvulsion._ProcessedBlocks == null || BlocksAvulsion._BlocksWidth != WorldData.Instance.WidthInBlocks || BlocksAvulsion._BlocksHeight != WorldData.Instance.HeightInBlocks || BlocksAvulsion._BlocksDepth != WorldData.Instance.DepthInBlocks)
		{
			BlocksAvulsion._BlocksWidth = WorldData.Instance.WidthInBlocks;
			BlocksAvulsion._BlocksHeight = WorldData.Instance.HeightInBlocks;
			BlocksAvulsion._BlocksDepth = WorldData.Instance.DepthInBlocks;
			BlocksAvulsion._ProcessedBlocks = new bool[BlocksAvulsion._BlocksWidth, BlocksAvulsion._BlocksHeight, BlocksAvulsion._BlocksDepth];
		}
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				for (int k = -radius; k <= radius; k++)
				{
					if (Mathf.Abs(i) == radius || Mathf.Abs(j) == radius || Mathf.Abs(k) == radius)
					{
						BlocksAvulsion.CheckBlocksAvulsion(x + i, y + j, z + k - 1, silent);
						BlocksAvulsion.CheckBlocksAvulsion(x + i, y + j, z + k + 1, silent);
						BlocksAvulsion.CheckBlocksAvulsion(x + i + 1, y + j, z + k, silent);
						BlocksAvulsion.CheckBlocksAvulsion(x + i - 1, y + j, z + k, silent);
						BlocksAvulsion.CheckBlocksAvulsion(x + i, y + j + 1, z + k, silent);
						BlocksAvulsion.CheckBlocksAvulsion(x + i, y + j - 1, z + k, silent);
					}
				}
			}
		}
		BlocksAvulsion.RevertProcessedBlocks();
	}

	private static void CheckBlocksAvulsion(int x, int y, int z, bool silent)
	{
		if (WorldData.Instance.GetBlockType(x, y, z) == BlockType.Air)
		{
			return;
		}
		if (BlocksAvulsion._ProcessedBlocks[x, y, z])
		{
			return;
		}
		BlocksAvulsion.RevertProcessedBlocks();
		int num = -1;
		int num2 = 0;
		BlocksAvulsion._ProcessedBlocksLimits = new int[]
		{
			x,
			x,
			y,
			y,
			z,
			z
		};
		BlocksAvulsion.CheckDepthRecursive(x, y, z, 0, ref num, ref num2);
		if (num >= 0 && num < 300)
		{
			if (!silent)
			{
				int x2 = Mathf.Max(BlocksAvulsion._ProcessedBlocksLimits[0] - 1, 0);
				int x3 = Mathf.Min(BlocksAvulsion._ProcessedBlocksLimits[1] + 1, BlocksAvulsion._BlocksWidth - 1);
				int y2 = Mathf.Max(BlocksAvulsion._ProcessedBlocksLimits[2] - 1, 0);
				int y3 = Mathf.Min(BlocksAvulsion._ProcessedBlocksLimits[3] + 1, BlocksAvulsion._BlocksHeight - 1);
				int z2 = Mathf.Max(BlocksAvulsion._ProcessedBlocksLimits[4] - 1, 0);
				int z3 = Mathf.Min(BlocksAvulsion._ProcessedBlocksLimits[5] + 1, BlocksAvulsion._BlocksDepth - 1);
				BlockGetter blockGetter = delegate(int xx, int yy, int zz)
				{
					if (xx < 0 || yy < 0 || zz < 0 || xx >= BlocksAvulsion._BlocksWidth || yy >= BlocksAvulsion._BlocksHeight || zz >= BlocksAvulsion._BlocksDepth)
					{
						return BlockType.Air;
					}
					return (!BlocksAvulsion._ProcessedBlocks[xx, yy, zz]) ? BlockType.Air : WorldData.Instance.GetBlockType(xx, yy, zz);
				};
				BlockKindGetter kindGetter = delegate(int xx, int yy, int zz)
				{
					if (xx < 0 || yy < 0 || zz < 0 || xx >= BlocksAvulsion._BlocksWidth || yy >= BlocksAvulsion._BlocksHeight || zz >= BlocksAvulsion._BlocksDepth)
					{
						return BlockKind.Default;
					}
					return (!BlocksAvulsion._ProcessedBlocks[xx, yy, zz]) ? BlockKind.Default : WorldData.Instance.GetBlockKind(xx, yy, zz);
				};
				Chunk.MeshData[] array = World.Instance.MeshGen.GenerateMeshData(x2, x3, y2, y3, z2, z3, blockGetter, kindGetter);
				Vector3 chunkPos = default(Vector3);
				ChunkGameObject chunkGameObject = null;
				for (int i = 0; i < 3; i++)
				{
					if (!array[i].IsEmpty)
					{
						ChunkGameObject chunkGameObject2 = WorldGameObjectX.Instance.CreateChunkGameObject(i, "Avulsion_Chunk", chunkPos, array[i]);
						chunkGameObject2.gameObject.layer = LayerMask.NameToLayer("Avulsion");
						chunkGameObject2.GetComponent<MeshCollider>().convex = true;
						if (chunkGameObject == null)
						{
							chunkGameObject = chunkGameObject2;
						}
						else
						{
							chunkGameObject2.transform.parent = chunkGameObject.transform;
						}
					}
				}
				try
				{
					chunkGameObject.StartCoroutine(BlocksAvulsion.ChunkAvulsionProcess(chunkGameObject, (float)num2));
				}
				catch
				{
					UnityEngine.Debug.Log("Object reference not set to an instance of an object");
				}
			}
			BlocksAvulsion.DestroyBlocks(true);
			BlocksAvulsion.DestroyBlocks(false);
			if (!silent)
			{
				int x4 = Mathf.Max(BlocksAvulsion._ProcessedBlocksLimits[0] - World.Instance.Lighting.LightingSteps, 0);
				int x5 = Mathf.Min(BlocksAvulsion._ProcessedBlocksLimits[1] + World.Instance.Lighting.LightingSteps, BlocksAvulsion._BlocksWidth - 1);
				int y4 = Mathf.Max(BlocksAvulsion._ProcessedBlocksLimits[2] - World.Instance.Lighting.LightingSteps, 0);
				int y5 = Mathf.Min(BlocksAvulsion._ProcessedBlocksLimits[3] + World.Instance.Lighting.LightingSteps, BlocksAvulsion._BlocksHeight - 1);
				int z4 = Mathf.Max(BlocksAvulsion._ProcessedBlocksLimits[4] - World.Instance.Lighting.LightingSteps, 0);
				int z5 = Mathf.Min(BlocksAvulsion._ProcessedBlocksLimits[5] + World.Instance.Lighting.LightingSteps, BlocksAvulsion._BlocksDepth - 1);
				World.Instance.Lighting.RecalculateLighting(x4, x5, y4, y5, z4, z5);
			}
			BlocksAvulsion.RevertProcessedBlocks();
		}
	}

	private static IEnumerator ChunkAvulsionProcess(ChunkGameObject chunkGameObject, float mass)
	{
		yield return new WaitForSeconds(0.1f);
		chunkGameObject.gameObject.AddComponent<Rigidbody>();
		chunkGameObject.GetComponent<Rigidbody>().mass = mass;
		yield return new WaitForSeconds(10f);
		chunkGameObject.GetComponent<Rigidbody>().detectCollisions = false;
		yield return new WaitForSeconds(5f);
		UnityEngine.Object.Destroy(chunkGameObject.gameObject);
		yield break;
	}

	private static void CheckDepthRecursive(int x, int y, int z, int curDepth, ref int maxDepth, ref int blocks)
	{
		if (x < 0 || y < 0 || z < 0 || x >= BlocksAvulsion._BlocksWidth || y >= BlocksAvulsion._BlocksHeight || z >= BlocksAvulsion._BlocksDepth)
		{
			maxDepth = 300;
			return;
		}
		if (BlocksAvulsion._ProcessedBlocks[x, y, z])
		{
			return;
		}
		if (WorldData.Instance.GetBlockType(x, y, z) == BlockType.Air)
		{
			return;
		}
		BlocksAvulsion._ProcessedBlocks[x, y, z] = true;
		if (x < BlocksAvulsion._ProcessedBlocksLimits[0])
		{
			BlocksAvulsion._ProcessedBlocksLimits[0] = x;
		}
		if (x > BlocksAvulsion._ProcessedBlocksLimits[1])
		{
			BlocksAvulsion._ProcessedBlocksLimits[1] = x;
		}
		if (y < BlocksAvulsion._ProcessedBlocksLimits[2])
		{
			BlocksAvulsion._ProcessedBlocksLimits[2] = y;
		}
		if (y > BlocksAvulsion._ProcessedBlocksLimits[3])
		{
			BlocksAvulsion._ProcessedBlocksLimits[3] = y;
		}
		if (z < BlocksAvulsion._ProcessedBlocksLimits[4])
		{
			BlocksAvulsion._ProcessedBlocksLimits[4] = z;
		}
		if (z > BlocksAvulsion._ProcessedBlocksLimits[5])
		{
			BlocksAvulsion._ProcessedBlocksLimits[5] = z;
		}
		if (curDepth > maxDepth)
		{
			maxDepth = curDepth;
		}
		blocks++;
		if (curDepth > 300)
		{
			return;
		}
		BlocksAvulsion.CheckDepthRecursive(x, y, z - 1, curDepth + 1, ref maxDepth, ref blocks);
		BlocksAvulsion.CheckDepthRecursive(x, y, z + 1, curDepth + 1, ref maxDepth, ref blocks);
		BlocksAvulsion.CheckDepthRecursive(x + 1, y, z, curDepth + 1, ref maxDepth, ref blocks);
		BlocksAvulsion.CheckDepthRecursive(x - 1, y, z, curDepth + 1, ref maxDepth, ref blocks);
		BlocksAvulsion.CheckDepthRecursive(x, y + 1, z, curDepth + 1, ref maxDepth, ref blocks);
		BlocksAvulsion.CheckDepthRecursive(x, y - 1, z, curDepth + 1, ref maxDepth, ref blocks);
	}

	private static void DestroyBlocks(bool onlyWater)
	{
		for (int i = BlocksAvulsion._ProcessedBlocksLimits[5]; i >= BlocksAvulsion._ProcessedBlocksLimits[4]; i--)
		{
			for (int j = BlocksAvulsion._ProcessedBlocksLimits[0]; j <= BlocksAvulsion._ProcessedBlocksLimits[1]; j++)
			{
				for (int k = BlocksAvulsion._ProcessedBlocksLimits[2]; k <= BlocksAvulsion._ProcessedBlocksLimits[3]; k++)
				{
					if (BlocksAvulsion._ProcessedBlocks[j, k, i])
					{
						BlockType blockType = WorldData.Instance.GetBlockType(j, k, i);
						if (blockType != BlockType.Air && (!onlyWater || blockType == BlockType.Water))
						{
							World.Instance.RemoveBlockAt(new IntVect(j, k, i), true, false);
						}
					}
				}
			}
		}
	}

	private static void RevertProcessedBlocks()
	{
		if (BlocksAvulsion._ProcessedBlocksLimits == null)
		{
			return;
		}
		for (int i = BlocksAvulsion._ProcessedBlocksLimits[5]; i >= BlocksAvulsion._ProcessedBlocksLimits[4]; i--)
		{
			for (int j = BlocksAvulsion._ProcessedBlocksLimits[0]; j <= BlocksAvulsion._ProcessedBlocksLimits[1]; j++)
			{
				for (int k = BlocksAvulsion._ProcessedBlocksLimits[2]; k <= BlocksAvulsion._ProcessedBlocksLimits[3]; k++)
				{
					BlocksAvulsion._ProcessedBlocks[j, k, i] = false;
				}
			}
		}
		BlocksAvulsion._ProcessedBlocksLimits = null;
	}

	public const int AvulsionDepth = 300;

	public const float AvulsionDestroyTime = 10f;

	private static bool[,,] _ProcessedBlocks;

	private static int _BlocksWidth;

	private static int _BlocksHeight;

	private static int _BlocksDepth;

	private static int[] _ProcessedBlocksLimits;
}
