using System;
using System.Collections.Generic;

public abstract class VoiceChatPool<T> where T : class
{
	public T Get()
	{
		if (this.queue.Count > 0)
		{
			return this.queue.Dequeue();
		}
		return this.Create();
	}

	public void Return(T obj)
	{
		if (obj != null)
		{
			this.queue.Enqueue(obj);
		}
	}

	protected abstract T Create();

	private Queue<T> queue = new Queue<T>();
}
