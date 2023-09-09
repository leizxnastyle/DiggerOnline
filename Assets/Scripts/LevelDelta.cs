using System;
using System.Collections.Generic;
using System.IO;

public class LevelDelta
{
	public LevelDelta()
	{
		this._MS = new MemoryStream();
		this._BW = new BinaryWriter(this._MS);
	}

	public void Add(BlockType block, int x, int y, int z, bool checkAvulsion = false, BlockKind kind = BlockKind.Default)
	{
		this._BW.Write((byte)block);
		this._BW.Write((byte)kind);
		this._BW.Write(x);
		this._BW.Write(y);
		this._BW.Write(z);
		this._BW.Write(checkAvulsion);
	}

	public void Add(int pos, byte[] bytes)
	{
		this._BW.Seek(pos, SeekOrigin.Begin);
		this._BW.Write(bytes);
	}

	public void MoveToLevel()
	{
		this._MS.Seek(0L, SeekOrigin.Begin);
		BinaryReader binaryReader = new BinaryReader(this._MS);
		int num = (int)this._MS.Length / 15;
		for (int i = 0; i < num; i++)
		{
			BlockType blockType = (BlockType)binaryReader.ReadByte();
			BlockKind kind = (BlockKind)binaryReader.ReadByte();
			int x = binaryReader.ReadInt32();
			int y = binaryReader.ReadInt32();
			int z = binaryReader.ReadInt32();
			bool flag = binaryReader.ReadBoolean();
			if (WorldData.Instance.IsValidBlock(x, y, z))
			{
				WorldData.Instance.SetBlockType(x, y, z, blockType);
				WorldData.Instance.SetBlockKind(x, y, z, kind);
				if (flag)
				{
					BlocksAvulsion.VerifyAvulsionAroundBlocks(x, y, z, 0, true);
				}
			}
		}
		this._MS.Seek(0L, SeekOrigin.End);
	}

	public List<byte[]> GetCurrentLevelDelta()
	{
		this._MS.Seek(0L, SeekOrigin.Begin);
		BinaryReader binaryReader = new BinaryReader(this._MS);
		int num = this._MS.ToArray().Length;
		List<byte[]> list = new List<byte[]>();
		for (int i = 0; i < num; i += 1500)
		{
			int num2 = 1500;
			if (i + num2 > num)
			{
				num2 = num - i;
			}
			list.Add(binaryReader.ReadBytes(num2));
		}
		this._MS.Seek(0L, SeekOrigin.End);
		return list;
	}

	public const int BlockInfoSize = 15;

	public const int SendPortionSize = 1500;

	private MemoryStream _MS;

	private BinaryWriter _BW;
}
