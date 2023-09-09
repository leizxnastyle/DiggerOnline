using System;
using System.Collections.Generic;
using UnityEngine;

public class LightProcessor : ILightProcessor
{
	public LightProcessor(WorldData worldData)
	{
		this._WorldData = worldData;
	}

	public int LightingSteps
	{
		get
		{
			return 9;
		}
	}

	public void InitializeLighting()
	{
		this._BlocksLight = new byte[this._WorldData.WidthInBlocks, this._WorldData.HeightInBlocks, this._WorldData.DepthInBlocks];
		for (int i = 0; i < this._WorldData.WidthInBlocks; i++)
		{
			for (int j = 0; j < this._WorldData.HeightInBlocks; j++)
			{
				for (int k = 0; k < this._WorldData.DepthInBlocks; k++)
				{
					if (WorldData.Instance.GetBlockType(i, j, k) == BlockType.Lava)
					{
						this.SetBlockLight(i, j, k, (byte)this.LightingSteps);
					}
				}
			}
		}
	}

	public void InitializeLightingChunk(Chunk chunk)
	{
		if (chunk.Z != 0)
		{
			return;
		}
		int x = chunk.X * this._WorldData.ChunkBlockWidth;
		int x2 = chunk.X * this._WorldData.ChunkBlockWidth + this._WorldData.ChunkBlockWidth - 1;
		int y = chunk.Y * this._WorldData.ChunkBlockHeight;
		int y2 = chunk.Y * this._WorldData.ChunkBlockHeight + this._WorldData.ChunkBlockHeight - 1;
		int z = 0;
		int z2 = this._WorldData.DepthInBlocks - 1;
		this.RecalculateLighting(x, x2, y, y2, z, z2);
	}

	public void AddLight(int x, int y, int z, bool recalculateLighting)
	{
		this.SetBlockLight(x, y, z, (byte)this.LightingSteps);
		if (recalculateLighting)
		{
			this.RecalculateLightingAroundBlock(x, y, z, 0, true);
		}
	}

	public void RemoveLight(int x, int y, int z, bool recalculateLighting)
	{
		this.SetBlockLight(x, y, z, 0);
		if (recalculateLighting)
		{
			this.RecalculateLightingAroundBlock(x, y, z, 0, true);
		}
	}

	public bool IsLight(int x, int y, int z)
	{
		return (int)(this.GetBlockLight(x, y, z) & 15) == this.LightingSteps;
	}

	public void RecalculateLightingAroundBlock(int x, int y, int z, int radius, bool lightChanged)
	{
		int num = Mathf.Max(x - radius - this.LightingSteps, 0);
		int num2 = Mathf.Min(x + radius + this.LightingSteps, this._WorldData.WidthInBlocks - 1);
		int num3 = Mathf.Max(y - radius - this.LightingSteps, 0);
		int num4 = Mathf.Min(y + radius + this.LightingSteps, this._WorldData.HeightInBlocks - 1);
		int num5 = Mathf.Max(z - radius - this.LightingSteps, 0);
		int num6 = Mathf.Min(z + radius + this.LightingSteps, this._WorldData.DepthInBlocks - 1);
		int num7;
		int x2;
		int num8;
		int num9;
		int num10;
		int num11;
		if (lightChanged)
		{
			num7 = num;
			x2 = num2;
			num8 = num3;
			num9 = num4;
			num10 = num5;
			num11 = num6;
		}
		else
		{
			num7 = Mathf.Max(x - radius, 0);
			x2 = Mathf.Min(x + radius, this._WorldData.WidthInBlocks - 1);
			num8 = Mathf.Max(y - radius, 0);
			num9 = Mathf.Min(y + radius, this._WorldData.HeightInBlocks - 1);
			num10 = Mathf.Max(z - 1, num5);
			num11 = Mathf.Min(z + 1, num6);
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					for (int k = num5; k <= num6; k++)
					{
						if ((int)(this.GetBlockLight(i, j, k) & 15) == this.LightingSteps)
						{
							num7 = Mathf.Min(num7, i);
							x2 = Mathf.Max(num7, i);
							num8 = Mathf.Min(num8, j);
							num9 = Mathf.Max(num9, j);
							num10 = Mathf.Min(num10, k);
							num11 = Mathf.Max(num11, k);
						}
					}
				}
			}
		}
		this.RecalculateLighting(num7, x2, num8, num9, num10, num11);
	}

	public void RecalculateLighting(int x1, int x2, int y1, int y2, int z1, int z2)
	{
		List<IntVect> list = new List<IntVect>();
		int chunkBlockWidth = this._WorldData.ChunkBlockWidth;
		int chunkBlockHeight = this._WorldData.ChunkBlockHeight;
		int chunkBlockDepth = this._WorldData.ChunkBlockDepth;
		for (int i = x1; i <= x2; i++)
		{
			for (int j = y1; j <= y2; j++)
			{
				int k = this._WorldData.DepthInBlocks - 1;
				Chunk chunk = WorldData.Instance.GetChunk(i, j, k);
				bool flag = false;
				bool flag2 = false;
				while (k >= 0)
				{
					BlockType blockType = chunk.GetBlockType(i % chunkBlockWidth, j % chunkBlockHeight, k % chunkBlockDepth);
					BlockKind block = chunk.GetBlockKind(i % chunkBlockWidth, j % chunkBlockHeight, k % chunkBlockDepth);
					if (block.IsFlip())
					{
						block = block.DoFlip();
					}
					if (!flag && blockType.IsLand() && !block.IsFence())
					{
						flag = true;
					}
					bool flag3 = flag;
					if (flag3 && !flag2)
					{
						flag2 = true;
						if (!block.IsDefault())
						{
							flag3 = false;
						}
					}
					int num = flag3 ? (this.LightingSteps / 2) : this.LightingSteps;
					if (k >= z1 && k <= z2)
					{
						if ((int)(this.GetBlockLight(i, j, k) & 15) == this.LightingSteps)
						{
							list.Add(new IntVect(i, j, k));
						}
						this.SetBlockLight(i, j, k, (byte)(num << 4));
						chunk.NeedsRegeneration = true;
					}
					else if (this.GetBlockLight(i, j, k) >> 4 != num)
					{
						this.SetBlockLight(i, j, k, (byte)(num << 4 | (int)(this.GetBlockLight(i, j, k) & 15)));
						chunk.NeedsRegeneration = true;
					}
					k--;
					if (k % chunkBlockDepth == chunkBlockDepth - 1)
					{
						chunk = WorldData.Instance.GetChunk(i, j, k);
					}
				}
			}
		}
		foreach (IntVect intVect in list)
		{
			this.SetLightingAroundBlockRecursive(intVect.X, intVect.Y, intVect.Z, intVect.X, intVect.Y, intVect.Z, this.LightingSteps, true);
		}
		x1 = Mathf.Max(x1 - 1, 0);
		x2 = Mathf.Min(x2 + 1, this._WorldData.WidthInBlocks - 1);
		y1 = Mathf.Max(y1 - 1, 0);
		y2 = Mathf.Min(y2 + 1, this._WorldData.HeightInBlocks - 1);
		z1 = Mathf.Max(z1 - 1, 0);
		z2 = Mathf.Min(z2 + 1, this._WorldData.DepthInBlocks - 1);
		for (int l = x1; l <= x2; l++)
		{
			for (int m = y1; m <= y2; m++)
			{
				this.ProcessPerimeterCube(l, m, z1);
			}
		}
		for (int n = x1; n <= x2; n++)
		{
			for (int num2 = y1; num2 <= y2; num2++)
			{
				this.ProcessPerimeterCube(n, num2, z2);
			}
		}
		for (int num3 = x1; num3 <= x2; num3++)
		{
			for (int num4 = z1; num4 <= z2; num4++)
			{
				this.ProcessPerimeterCube(num3, y1, num4);
			}
		}
		for (int num5 = x1; num5 <= x2; num5++)
		{
			for (int num6 = z1; num6 <= z2; num6++)
			{
				this.ProcessPerimeterCube(num5, y2, num6);
			}
		}
		for (int num7 = z1; num7 <= z2; num7++)
		{
			for (int num8 = y1; num8 <= y2; num8++)
			{
				this.ProcessPerimeterCube(x1, num8, num7);
			}
		}
		for (int num9 = z1; num9 <= z2; num9++)
		{
			for (int num10 = y1; num10 <= y2; num10++)
			{
				this.ProcessPerimeterCube(x2, num10, num9);
			}
		}
	}

	private void ProcessPerimeterCube(int x, int y, int z)
	{
		int num = (int)(this.GetBlockLight(x, y, z) & 15);
		if (num > 1)
		{
			num--;
			this.SetLightingAroundBlockRecursive(x - 1, y, z, x, y, z, num, false);
			this.SetLightingAroundBlockRecursive(x + 1, y, z, x, y, z, num, false);
			this.SetLightingAroundBlockRecursive(x, y - 1, z, x, y, z, num, false);
			this.SetLightingAroundBlockRecursive(x, y + 1, z, x, y, z, num, false);
			this.SetLightingAroundBlockRecursive(x, y, z - 1, x, y, z, num, false);
			this.SetLightingAroundBlockRecursive(x, y, z + 1, x, y, z, num, false);
		}
	}

	private void SetLightingAroundBlockRecursive(int x, int y, int z, int fromX, int fromY, int fromZ, int lightAmount, bool isMain = false)
	{
		if (x < 0 || y < 0 || x >= this._WorldData.WidthInBlocks || y >= this._WorldData.HeightInBlocks || z >= this._WorldData.DepthInBlocks || z < 0)
		{
			return;
		}
		int num = x / this._WorldData.ChunkBlockWidth;
		int num2 = y / this._WorldData.ChunkBlockHeight;
		int num3 = z / this._WorldData.ChunkBlockDepth;
		int x2 = x % this._WorldData.ChunkBlockWidth;
		int y2 = y % this._WorldData.ChunkBlockHeight;
		int z2 = z % this._WorldData.ChunkBlockDepth;
		Chunk chunk = this._WorldData.Chunks[num, num2, num3];
		BlockType blockType = chunk.GetBlockType(x2, y2, z2);
		if (!isMain && blockType != BlockType.Air && !WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass && (lightAmount != this.LightingSteps || blockType != BlockType.Lava))
		{
			return;
		}
		if ((int)(this.GetBlockLight(x, y, z) & 15) >= lightAmount)
		{
			return;
		}
		this.SetBlockLight(x, y, z, (byte)((int)(this.GetBlockLight(x, y, z) & 240) | lightAmount));
		chunk.NeedsRegeneration = true;
		if (lightAmount <= 0)
		{
			return;
		}
		lightAmount--;
		if (x - 1 != fromX || y != fromY || z != fromZ)
		{
			this.SetLightingAroundBlockRecursive(x - 1, y, z, x, y, z, lightAmount, false);
		}
		if (x + 1 != fromX || y != fromY || z != fromZ)
		{
			this.SetLightingAroundBlockRecursive(x + 1, y, z, x, y, z, lightAmount, false);
		}
		if (x != fromX || y - 1 != fromY || z != fromZ)
		{
			this.SetLightingAroundBlockRecursive(x, y - 1, z, x, y, z, lightAmount, false);
		}
		if (x != fromX || y + 1 != fromY || z != fromZ)
		{
			this.SetLightingAroundBlockRecursive(x, y + 1, z, x, y, z, lightAmount, false);
		}
		if (x != fromX || y != fromY || z - 1 != fromZ)
		{
			this.SetLightingAroundBlockRecursive(x, y, z - 1, x, y, z, lightAmount, false);
		}
		if (x != fromX || y != fromY || z + 1 != fromZ)
		{
			this.SetLightingAroundBlockRecursive(x, y, z + 1, x, y, z, lightAmount, false);
		}
	}

	public byte GetBlockLight(int x, int y, int z)
	{
		if (x >= this._WorldData.WidthInBlocks)
		{
			return 0;
		}
		if (y >= this._WorldData.HeightInBlocks)
		{
			return 0;
		}
		if (z >= this._WorldData.DepthInBlocks)
		{
			return 0;
		}
		if (x < 0)
		{
			return 0;
		}
		if (y < 0)
		{
			return 0;
		}
		if (z < 0)
		{
			return 0;
		}
		BlockKind blockKind = this._WorldData.GetBlockKind(x, y, z);
		if (!blockKind.IsDefault())
		{
			return this._BlocksLight[x, y, z];
		}
		BlockType blockType = this._WorldData.GetBlockType(x, y, z);
		if (blockType == BlockType.Lava)
		{
			return this._BlocksLight[x, y, z];
		}
		if (blockType != BlockType.Air && blockType != BlockType.Water && !WorldGameObjectX.Instance.BlockParametrs[(int)blockType].is_glass)
		{
			return 0;
		}
		return this._BlocksLight[x, y, z];
	}

	private void SetBlockLight(int x, int y, int z, byte light)
	{
		this._BlocksLight[x, y, z] = light;
	}

	private readonly WorldData _WorldData;

	private byte[,,] _BlocksLight;
}
