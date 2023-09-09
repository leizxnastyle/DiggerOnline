using System;
using System.Collections.Generic;

public class HDPool<T> where T : class
{
	public int AcquiredCount { get; private set; }

	public int ReleasedCount { get; private set; }

	public int PoolCount
	{
		get
		{
			return this.AcquiredCount - this.ReleasedCount;
		}
	}

	public int CachedCount
	{
		get
		{
			return this.stack.Count;
		}
	}

	public T Acquire()
	{
		this.AcquiredCount++;
		return (this.stack.Count != 0) ? this.stack.Pop() : ((T)((object)null));
	}

	public void Release(T obj)
	{
		this.ReleasedCount++;
		this.stack.Push(obj);
	}

	private readonly Stack<T> stack = new Stack<T>();
}
