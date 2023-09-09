using System;

public class VoiceChatCircularBuffer<T>
{
	public VoiceChatCircularBuffer(int maxCapacity)
	{
		this.capacity = maxCapacity;
		this.buffer = new T[this.capacity];
	}

	public bool HasItems
	{
		get
		{
			return this.count > 0;
		}
	}

	public int TailIndex
	{
		get
		{
			return this.tail;
		}
	}

	public T[] Data
	{
		get
		{
			return this.buffer;
		}
	}

	public int Count
	{
		get
		{
			return this.count;
		}
	}

	public int Capacity
	{
		get
		{
			return this.capacity;
		}
	}

	public void Add(T item)
	{
		if (this.count == this.capacity && this.head == this.tail && ++this.tail == this.capacity)
		{
			this.tail = 0;
		}
		this.buffer[this.head] = item;
		if (++this.head == this.capacity)
		{
			this.head = 0;
		}
		this.count = Math.Min(this.capacity, this.count + 1);
	}

	public T Remove()
	{
		if (this.count == 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		T result = this.buffer[this.tail];
		if (++this.tail == this.capacity)
		{
			this.tail = 0;
		}
		this.count--;
		return result;
	}

	public bool NextIndex(ref int value)
	{
		if (++value == this.capacity)
		{
			value = 0;
		}
		return value != this.head;
	}

	private int capacity;

	private int count;

	private int head;

	private int tail;

	private T[] buffer;
}
