using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class TQueue<T> : ICollection, IEnumerable
{
	public TQueue()
	{
		this.m_Queue = new Queue<T>();
	}

	public TQueue(int capacity)
	{
		this.m_Queue = new Queue<T>(capacity);
	}

	public TQueue(IEnumerable<T> collection)
	{
		this.m_Queue = new Queue<T>(collection);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		this.LockQ.EnterReadLock();
		Queue<T> localQ;
		try
		{
			localQ = new Queue<T>(this.m_Queue);
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
		foreach (T item in localQ)
		{
			yield return item;
		}
		yield break;
	}

	public IEnumerator<T> GetEnumerator()
	{
		this.LockQ.EnterReadLock();
		Queue<T> localQ;
		try
		{
			localQ = new Queue<T>(this.m_Queue);
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
		foreach (T item in localQ)
		{
			yield return item;
		}
		yield break;
	}

	public void CopyTo(Array array, int index)
	{
		this.LockQ.EnterReadLock();
		try
		{
			this.m_Queue.ToArray().CopyTo(array, index);
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
	}

	public void CopyTo(T[] array, int index)
	{
		this.LockQ.EnterReadLock();
		try
		{
			this.m_Queue.CopyTo(array, index);
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
	}

	public int Count
	{
		get
		{
			this.LockQ.EnterReadLock();
			int count;
			try
			{
				count = this.m_Queue.Count;
			}
			finally
			{
				this.LockQ.ExitReadLock();
			}
			return count;
		}
	}

	public bool IsSynchronized
	{
		get
		{
			return true;
		}
	}

	public object SyncRoot
	{
		get
		{
			return this.objSyncRoot;
		}
	}

	public void Enqueue(T item)
	{
		this.LockQ.EnterWriteLock();
		try
		{
			this.m_Queue.Enqueue(item);
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
	}

	public T Dequeue()
	{
		this.LockQ.EnterWriteLock();
		T result;
		try
		{
			result = this.m_Queue.Dequeue();
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
		return result;
	}

	public void EnqueueAll(IEnumerable<T> ItemsToQueue)
	{
		this.LockQ.EnterWriteLock();
		try
		{
			foreach (T item in ItemsToQueue)
			{
				this.m_Queue.Enqueue(item);
			}
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
	}

	public void EnqueueAll(TList<T> ItemsToQueue)
	{
		this.LockQ.EnterWriteLock();
		try
		{
			foreach (object obj in ItemsToQueue)
			{
				T item = (T)((object)obj);
				this.m_Queue.Enqueue(item);
			}
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
	}

	public TList<T> DequeueAll()
	{
		this.LockQ.EnterWriteLock();
		TList<T> result;
		try
		{
			TList<T> tlist = new TList<T>();
			while (this.m_Queue.Count > 0)
			{
				tlist.Add(this.m_Queue.Dequeue());
			}
			result = tlist;
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
		return result;
	}

	public void Clear()
	{
		this.LockQ.EnterWriteLock();
		try
		{
			this.m_Queue.Clear();
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
	}

	public bool Contains(T item)
	{
		this.LockQ.EnterReadLock();
		bool result;
		try
		{
			result = this.m_Queue.Contains(item);
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
		return result;
	}

	public T Peek()
	{
		this.LockQ.EnterReadLock();
		T result;
		try
		{
			result = this.m_Queue.Peek();
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
		return result;
	}

	public T[] ToArray()
	{
		this.LockQ.EnterReadLock();
		T[] result;
		try
		{
			result = this.m_Queue.ToArray();
		}
		finally
		{
			this.LockQ.ExitReadLock();
		}
		return result;
	}

	public void TrimExcess()
	{
		this.LockQ.EnterWriteLock();
		try
		{
			this.m_Queue.TrimExcess();
		}
		finally
		{
			this.LockQ.ExitWriteLock();
		}
	}

	private readonly Queue<T> m_Queue;

	private readonly ReaderWriterLockSlim LockQ = new ReaderWriterLockSlim();

	private readonly object objSyncRoot = new object();
}
