using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

public class TList<T> : IEnumerable, ICollection<T>, IEnumerable<T>
{
	public TList()
	{
		this.m_TList = new List<T>();
	}

	public TList(int capacity)
	{
		this.m_TList = new List<T>(capacity);
	}

	public TList(IEnumerable<T> collection)
	{
		this.m_TList = new List<T>(collection);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		this.LockList.EnterReadLock();
		List<T> localList;
		try
		{
			localList = new List<T>(this.m_TList);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		foreach (T item in localList)
		{
			yield return item;
		}
		yield break;
	}

	private bool Disposed
	{
		get
		{
			return Thread.VolatileRead(ref this.m_Disposed) == 1;
		}
		set
		{
			Thread.VolatileWrite(ref this.m_Disposed, (!value) ? 0 : 1);
		}
	}

	public IEnumerator GetEnumerator()
	{
		this.LockList.EnterReadLock();
		List<T> localList;
		try
		{
			localList = new List<T>(this.m_TList);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		foreach (T item in localList)
		{
			yield return item;
		}
		yield break;
	}

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (this.Disposed)
		{
			return;
		}
		if (disposing)
		{
		}
		this.Disposed = true;
	}

	~TList()
	{
		this.Dispose(false);
	}

	public void Add(T item)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Add(item);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public void AddRange(IEnumerable<T> collection)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.AddRange(collection);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public bool AddIfNotExist(T item)
	{
		this.LockList.EnterWriteLock();
		bool result;
		try
		{
			if (this.m_TList.Contains(item))
			{
				result = false;
			}
			else
			{
				this.m_TList.Add(item);
				result = true;
			}
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
		return result;
	}

	public ReadOnlyCollection<T> AsReadOnly()
	{
		this.LockList.EnterReadLock();
		ReadOnlyCollection<T> result;
		try
		{
			result = this.m_TList.AsReadOnly();
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int BinarySearch(T item)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.BinarySearch(item);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int BinarySearch(T item, IComparer<T> comparer)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.BinarySearch(item, comparer);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.BinarySearch(index, count, item, comparer);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int Capacity
	{
		get
		{
			this.LockList.EnterReadLock();
			int capacity;
			try
			{
				capacity = this.m_TList.Capacity;
			}
			finally
			{
				this.LockList.ExitReadLock();
			}
			return capacity;
		}
		set
		{
			this.LockList.EnterWriteLock();
			try
			{
				this.m_TList.Capacity = value;
			}
			finally
			{
				this.LockList.ExitWriteLock();
			}
		}
	}

	public void Clear()
	{
		this.LockList.EnterReadLock();
		try
		{
			this.m_TList.Clear();
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
	}

	public bool Contains(T item)
	{
		this.LockList.EnterReadLock();
		bool result;
		try
		{
			result = this.m_TList.Contains(item);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
	{
		this.LockList.EnterReadLock();
		List<TOutput> result;
		try
		{
			result = this.m_TList.ConvertAll<TOutput>(converter);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		this.LockList.EnterReadLock();
		try
		{
			this.m_TList.CopyTo(array, arrayIndex);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
	}

	public int Count
	{
		get
		{
			this.LockList.EnterReadLock();
			int count;
			try
			{
				count = this.m_TList.Count;
			}
			finally
			{
				this.LockList.ExitReadLock();
			}
			return count;
		}
	}

	public bool Exists(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		bool result;
		try
		{
			result = this.m_TList.Exists(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public T Find(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		T result;
		try
		{
			result = this.m_TList.Find(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public List<T> FindAll(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		List<T> result;
		try
		{
			result = this.m_TList.FindAll(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int FindIndex(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindIndex(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int FindIndex(int startIndex, Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindIndex(startIndex, match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int FindIndex(int startIndex, int count, Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindIndex(startIndex, count, match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public T FindLast(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		T result;
		try
		{
			result = this.m_TList.FindLast(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int FindLastIndex(Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindLastIndex(match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int FindLastIndex(int startIndex, Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindLastIndex(startIndex, match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int FindLastIndex(int startIndex, int count, Predicate<T> match)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.FindLastIndex(startIndex, count, match);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public void ForEach(Action<T> action)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.ForEach(action);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public List<T> GetRange(int index, int count)
	{
		this.LockList.EnterReadLock();
		List<T> range;
		try
		{
			range = this.m_TList.GetRange(index, count);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return range;
	}

	public int IndexOf(T item)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.IndexOf(item);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int IndexOf(T item, int index)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.IndexOf(item, index);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int IndexOf(T item, int index, int count)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.IndexOf(item, index, count);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public void Insert(int index, T item)
	{
		this.LockList.ExitWriteLock();
		try
		{
			this.m_TList.Insert(index, item);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public void InsertRange(int index, IEnumerable<T> range)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.InsertRange(index, range);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}

	public int LastIndexOf(T item)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.LastIndexOf(item);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int LastIndexOf(T item, int index)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.LastIndexOf(item, index);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public int LastIndexOf(T item, int index, int count)
	{
		this.LockList.EnterReadLock();
		int result;
		try
		{
			result = this.m_TList.LastIndexOf(item, index, count);
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public bool Remove(T item)
	{
		this.LockList.EnterWriteLock();
		bool result;
		try
		{
			result = this.m_TList.Remove(item);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
		return result;
	}

	public int RemoveAll(Predicate<T> match)
	{
		this.LockList.EnterWriteLock();
		int result;
		try
		{
			result = this.m_TList.RemoveAll(match);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
		return result;
	}

	public void RemoveAt(int index)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.RemoveAt(index);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public void RemoveRange(int index, int count)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.RemoveRange(index, count);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public void Reverse()
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Reverse();
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public void Reverse(int index, int count)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Reverse(index, count);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public void Sort()
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Sort();
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public void Sort(Comparison<T> comparison)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Sort(comparison);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public void Sort(IComparer<T> comparer)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Sort(comparer);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public void Sort(int index, int count, IComparer<T> comparer)
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.Sort(index, count, comparer);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public T[] ToArray()
	{
		this.LockList.EnterReadLock();
		T[] result;
		try
		{
			result = this.m_TList.ToArray();
		}
		finally
		{
			this.LockList.ExitReadLock();
		}
		return result;
	}

	public void TrimExcess()
	{
		this.LockList.EnterWriteLock();
		try
		{
			this.m_TList.TrimExcess();
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
	}

	public bool TrueForAll(Predicate<T> match)
	{
		this.LockList.EnterWriteLock();
		bool result;
		try
		{
			result = this.m_TList.TrueForAll(match);
		}
		finally
		{
			this.LockList.ExitWriteLock();
		}
		return result;
	}

	private readonly List<T> m_TList;

	private readonly ReaderWriterLockSlim LockList = new ReaderWriterLockSlim();

	private int m_Disposed;
}
