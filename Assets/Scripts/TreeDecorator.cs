using System;
using UnityEngine;

public class TreeDecorator : IDecoration
{
	public TreeDecorator(WorldData worldData, GameObject[] list)
	{
		this.m_WorldData = worldData;
		this.model_list = list;
	}

	public void Decorate(int blockX, int blockY, int blockZ, RandomAdv random)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.model_list[0], new Vector3((float)blockX + 0.5f, (float)(blockZ + 1), (float)blockY + 0.5f), this.model_list[0].transform.rotation);
		gameObject.layer = 8;
		if (random.RandomRange(1, 100) < 99)
		{
			return;
		}
		int num = UnityEngine.Random.Range(1, this.model_list.Length);
		gameObject = (GameObject)UnityEngine.Object.Instantiate(this.model_list[num], new Vector3((float)blockX + 0.5f, (float)(blockZ + 1), (float)blockY + 0.5f), this.model_list[num].transform.rotation);
		gameObject.layer = 12;
	}

	private bool IsAValidLocationforDecoration(int blockX, int blockY, int blockZ, RandomAdv random)
	{
		return random.RandomRange(1, 100) >= 99 && blockZ < this.m_WorldData.DepthInBlocks - 20 && this.SpaceAboveIsEmpty(blockX, blockY, blockZ, 8, 2, 2);
	}

	private void CreateDecorationAt(int blockX, int blockY, int blockZ, RandomAdv random)
	{
		int num = random.RandomRange(6, 10);
		for (int i = blockZ + 1; i <= blockZ + num; i++)
		{
			this.CreateTrunkAt(blockX, blockY, i);
		}
		this.CreateSphereAt(blockX, blockY, blockZ + num, random.RandomRange(3, 4));
	}

	private void CreateSphereAt(int blockX, int blockY, int blockZ, int radius)
	{
		for (int i = blockX - radius; i <= blockX + radius; i++)
		{
			for (int j = blockY - radius; j <= blockY + radius; j++)
			{
				for (int k = blockZ - radius; k <= blockZ + radius; k++)
				{
					if (Vector3.Distance(new Vector3((float)blockX, (float)blockY, (float)blockZ), new Vector3((float)i, (float)j, (float)k)) <= (float)radius)
					{
						this.m_WorldData.SetBlockType(i, j, k, BlockType.Leaves);
					}
				}
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

	private GameObject[] model_list;
}
