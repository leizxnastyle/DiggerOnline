using System;
using System.Collections;
using System.Collections.Generic;

public class PChunkList : HDPoolableObject, IEnumerable, IEnumerable<Chunk>, IList<Chunk>, ICollection<Chunk>
{
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	public static PChunkList Acquire()
	{
		return PChunkList.pool.Acquire() ?? new PChunkList();
	}

	public override void Release()
	{
		this.list.Clear();
		PChunkList.pool.Release(this);
	}

	public IEnumerator<Chunk> GetEnumerator()
	{
		return this.list.GetEnumerator();
	}

	public void Add(Chunk item)
	{
		this.list.Add(item);
	}

	public void Clear()
	{
		this.list.Clear();
	}

	public bool Contains(Chunk item)
	{
		return this.list.Contains(item);
	}

	public void CopyTo(Chunk[] array, int arrayIndex)
	{
		this.list.CopyTo(array, arrayIndex);
	}

	public bool Remove(Chunk item)
	{
		return this.list.Remove(item);
	}

	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}

	public int IndexOf(Chunk item)
	{
		return this.list.IndexOf(item);
	}

	public void Insert(int index, Chunk item)
	{
		this.list.Insert(index, item);
	}

	public void RemoveAt(int index)
	{
		this.list.RemoveAt(index);
	}

	public Chunk this[int index]
	{
		get
		{
			return this.list[index];
		}
		set
		{
			this.list[index] = value;
		}
	}

	public void Sort(Comparison<Chunk> comparer)
	{
		this.list.Sort(comparer);
	}

	public void Sort(IComparer<Chunk> comparer)
	{
		this.list.Sort(comparer);
	}

	public void Sort()
	{
		this.list.Sort();
	}

	public static readonly HDPool<PChunkList> pool = new HDPool<PChunkList>();

	private readonly List<Chunk> list = new List<Chunk>();
}
