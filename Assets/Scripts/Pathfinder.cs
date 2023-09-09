using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
	private static void GenerateNodeOffsets()
	{
		Pathfinder._NodeOffsets = new List<int[]>();
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				for (int k = -1; k <= 1; k++)
				{
					if (k < 0 || i != 0 || j != 0)
					{
						Pathfinder._NodeOffsets.Add(new int[]
						{
							i,
							j,
							k
						});
					}
				}
			}
		}
		List<int[]> nodeOffsets = Pathfinder._NodeOffsets;
		int[] array = new int[3];
		array[0] = 2;
		nodeOffsets.Add(array);
		List<int[]> nodeOffsets2 = Pathfinder._NodeOffsets;
		int[] array2 = new int[3];
		array2[0] = -2;
		nodeOffsets2.Add(array2);
		List<int[]> nodeOffsets3 = Pathfinder._NodeOffsets;
		int[] array3 = new int[3];
		array3[1] = 2;
		nodeOffsets3.Add(array3);
		List<int[]> nodeOffsets4 = Pathfinder._NodeOffsets;
		int[] array4 = new int[3];
		array4[1] = -2;
		nodeOffsets4.Add(array4);
	}

	public static List<Vector3> Find(Vector3 startPosition, Vector3 targetPosition)
	{
		IntVect left = new IntVect((int)startPosition.x, (int)startPosition.z, (int)startPosition.y);
		IntVect right = new IntVect((int)targetPosition.x, (int)targetPosition.z, (int)targetPosition.y);
		if (left == right)
		{
			return new List<Vector3>
			{
				targetPosition
			};
		}
		Pathfinder.PathNode pathNode = new Pathfinder.PathNode(left.X, left.Y, left.Z, null, targetPosition);
		int widthInBlocks = WorldData.Instance.WidthInBlocks;
		int heightInBlocks = WorldData.Instance.HeightInBlocks;
		int depthInBlocks = WorldData.Instance.DepthInBlocks;
		if (Pathfinder._PathBlockProcessed == null)
		{
			Pathfinder._PathBlockProcessed = new int[widthInBlocks, heightInBlocks, depthInBlocks];
		}
		Pathfinder._PathBlockProcessedIndex++;
		Pathfinder._PathBlockProcessed[pathNode.BlockX, pathNode.BlockY, pathNode.BlockZ] = Pathfinder._PathBlockProcessedIndex;
		if (Pathfinder._NodeOffsets == null)
		{
			Pathfinder.GenerateNodeOffsets();
		}
		Pathfinder.PathNode pathNode2 = null;
		for (int i = 100; i > 0; i--)
		{
			if (pathNode2 == null)
			{
				pathNode2 = pathNode.GetNextToProcess();
			}
			if (pathNode2 == null)
			{
				return null;
			}
			for (int j = 0; j < Pathfinder._NodeOffsets.Count; j++)
			{
				int num = pathNode2.BlockX + Pathfinder._NodeOffsets[j][0];
				int num2 = pathNode2.BlockY + Pathfinder._NodeOffsets[j][1];
				int num3 = pathNode2.BlockZ + Pathfinder._NodeOffsets[j][2];
				if (num >= 0 && num < widthInBlocks && num2 >= 0 && num2 < heightInBlocks && num3 > 0 && num3 < depthInBlocks)
				{
					if (Pathfinder._PathBlockProcessed[num, num2, num3] != Pathfinder._PathBlockProcessedIndex)
					{
						if (j >= Pathfinder._NodeOffsets.Count - 4)
						{
							int x = pathNode2.BlockX + Pathfinder._NodeOffsets[j][0] / 2;
							int y = pathNode2.BlockY + Pathfinder._NodeOffsets[j][1] / 2;
							if (WorldData.Instance.GetBlockType(x, y, num3) != BlockType.Air || WorldData.Instance.GetBlockType(x, y, num3 - 1) != BlockType.Air)
							{
								goto IL_310;
							}
						}
						Pathfinder._PathBlockProcessed[num, num2, num3] = Pathfinder._PathBlockProcessedIndex;
						if (num == right.X && num2 == right.Y && num3 == right.Z)
						{
							List<Vector3> list = new List<Vector3>();
							list.Add(targetPosition);
							while (pathNode2.Parent != null)
							{
								list.Add(pathNode2.GetPosition());
								pathNode2 = pathNode2.Parent;
							}
							list.Reverse();
							return list;
						}
						BlockType blockType = WorldData.Instance.GetBlockType(num, num2, num3);
						if (blockType == BlockType.Air)
						{
							BlockType blockType2 = WorldData.Instance.GetBlockType(num, num2, num3 - 1);
							if (blockType2 != BlockType.Air && blockType2 != BlockType.Water && blockType2 != BlockType.Lava)
							{
								pathNode2.AddChild(num, num2, num3, targetPosition);
							}
						}
					}
				}
				IL_310:;
			}
			pathNode2 = pathNode2.GetNextToProcess();
		}
		return null;
	}

	public static List<Vector3> FindRandom(Vector3 startPosition, Vector3 center, float distance)
	{
		IntVect intVect = new IntVect((int)center.x, (int)center.z, (int)center.y);
		int num = Mathf.CeilToInt(distance);
		int min = Mathf.Max(intVect.X - num, 0);
		int max = Mathf.Min(intVect.X + num, WorldData.Instance.WidthInBlocks - 1);
		int min2 = Mathf.Max(intVect.Y - num, 0);
		int max2 = Mathf.Min(intVect.Y + num, WorldData.Instance.HeightInBlocks - 1);
		IntVect intVect2 = default(IntVect);
		for (int i = 0; i < 5; i++)
		{
			int x = UnityEngine.Random.Range(min, max);
			int y = UnityEngine.Random.Range(min2, max2);
			bool flag = false;
			for (int j = 0; j < num * 2; j++)
			{
				int num2 = intVect.Z + ((j <= num) ? j : (-(j - num)));
				if (num2 >= 0 && num2 < WorldData.Instance.DepthInBlocks)
				{
					if (WorldData.Instance.GetBlockType(x, y, num2) == BlockType.Air)
					{
						BlockType blockType = WorldData.Instance.GetBlockType(x, y, num2 - 1);
						if (blockType != BlockType.Air && blockType != BlockType.Water && blockType != BlockType.Lava)
						{
							intVect2 = new IntVect(x, y, num2);
							j = num * 2;
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				Vector3 targetPosition = new Vector3((float)intVect2.X, (float)intVect2.Z, (float)intVect2.Y);
				targetPosition.x += 0.5f;
				targetPosition.z += 0.5f;
				List<Vector3> list = Pathfinder.Find(startPosition, targetPosition);
				if (list != null)
				{
					return list;
				}
			}
		}
		return null;
	}

	private static int[,,] _PathBlockProcessed;

	private static int _PathBlockProcessedIndex;

	private static List<int[]> _NodeOffsets;

	private class PathNode
	{
		public PathNode(int x, int y, int z, Pathfinder.PathNode parent, Vector3 targetPosition)
		{
			this.BlockX = x;
			this.BlockY = y;
			this.BlockZ = z;
			this.Child = new List<Pathfinder.PathNode>();
			this.Parent = parent;
			this._Processed = false;
			this._DistanceToTarget = Vector3.Distance(this.GetPosition(), targetPosition);
		}

		public void AddChild(int x, int y, int z, Vector3 targetPosition)
		{
			this.Child.Add(new Pathfinder.PathNode(x, y, z, this, targetPosition));
		}

		public Pathfinder.PathNode GetNextToProcess()
		{
			Pathfinder.PathNode pathNode = null;
			float num = 0f;
			this.GetNextToProcessInternal(ref pathNode, ref num);
			if (pathNode != null)
			{
				pathNode._Processed = true;
			}
			return pathNode;
		}

		private void GetNextToProcessInternal(ref Pathfinder.PathNode bestNode, ref float bestDistance)
		{
			if (!this._Processed && (bestNode == null || this._DistanceToTarget < bestDistance))
			{
				bestNode = this;
				bestDistance = this._DistanceToTarget;
			}
			foreach (Pathfinder.PathNode pathNode in this.Child)
			{
				pathNode.GetNextToProcessInternal(ref bestNode, ref bestDistance);
			}
		}

		public Vector3 GetPosition()
		{
			return new Vector3((float)this.BlockX + 0.5f, (float)this.BlockZ, (float)this.BlockY + 0.5f);
		}

		public int BlockX;

		public int BlockY;

		public int BlockZ;

		public List<Pathfinder.PathNode> Child;

		public Pathfinder.PathNode Parent;

		private bool _Processed;

		private float _DistanceToTarget;
	}
}
