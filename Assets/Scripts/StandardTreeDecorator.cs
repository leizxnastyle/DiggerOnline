using System;

public class StandardTreeDecorator : IDecoration
{
	public StandardTreeDecorator(WorldData worldData)
	{
		this.m_WorldData = worldData;
	}

	public void Decorate(int blockX, int blockY, int blockZ, RandomAdv random)
	{
		if (this.IsAValidLocationforDecoration(blockX, blockY, blockZ, random))
		{
			this.CreateDecorationAt(blockX, blockY, blockZ, random);
		}
	}

	private bool IsAValidLocationforDecoration(int blockX, int blockY, int blockZ, RandomAdv random)
	{
		return random.RandomRange(1, 100) >= 99 && blockZ < this.m_WorldData.DepthInBlocks - 20 && this.SpaceAboveIsEmpty(blockX, blockY, blockZ, 8, 2, 2);
	}

	private void CreateDecorationAt(int blockX, int blockY, int blockZ, RandomAdv random)
	{
		int num = random.RandomRange(3, 5);
		for (int i = blockZ + 1; i <= blockZ + num; i++)
		{
			this.CreateTrunkAt(blockX, blockY, i);
		}
		int num2 = random.RandomRange(2, 3);
		this.CreatePlain(blockX, blockY, blockZ + num, num2);
		this.CreatePlain(blockX, blockY, blockZ + num + 1, num2);
		this.CreatePlain(blockX, blockY, blockZ + num + 2, num2);
		this.CreatePlain(blockX, blockY, blockZ + num + 3, num2 - 1);
		this.CreatePlain(blockX, blockY, blockZ + num + 4, num2 - 2);
	}

	private void CreatePlain(int blockX, int blockY, int blockZ, int radius)
	{
		for (int i = blockX - radius; i <= blockX + radius; i++)
		{
			for (int j = blockY - radius; j <= blockY + radius; j++)
			{
				this.m_WorldData.SetBlockType(i, j, blockZ, BlockType.Leaves);
			}
		}
	}

	private void CreateTrunkAt(int blockX, int blockY, int z)
	{
		this.m_WorldData.SetBlockType(blockX, blockY, z, BlockType.Wood);
	}

	private bool SpaceAboveIsEmpty(int blockX, int blockY, int blockZ, int depthAbove, int widthAround, int heightAround)
	{
		for (int i = blockZ + 1; i <= blockZ + depthAbove; i++)
		{
			for (int j = blockX - widthAround; j <= blockX + widthAround; j++)
			{
				for (int k = blockY - heightAround; k < blockY + heightAround; k++)
				{
					if (this.m_WorldData.GetBlockType(j, k, i) != BlockType.Air)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public override string ToString()
	{
		return "Standard Tree";
	}

	private readonly WorldData m_WorldData;
}
