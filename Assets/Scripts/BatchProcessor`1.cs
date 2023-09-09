using System;
using System.Collections.Generic;
using System.Threading;

public class BatchProcessor<T> : IBatchProcessor<T> where T : class
{
	public void Process(IList<T> itemsToProcess, Action<T> action, bool waitUntilAllThreadsFinish)
	{
		int numberOfThreads = Environment.ProcessorCount - 1;
		this.Process(numberOfThreads, itemsToProcess, action, waitUntilAllThreadsFinish);
	}

	public void Process(int numberOfThreads, IList<T> itemsToProcess_, Action<T> action, bool waitUntilAllThreadsFinish)
	{
		List<T> itemsToProcess = new List<T>(itemsToProcess_);
		List<Thread> list = new List<Thread>();
		if (numberOfThreads < 1)
		{
			numberOfThreads = 1;
		}
		int count = itemsToProcess.Count;
		if (count == 0)
		{
			return;
		}
		if (numberOfThreads > count)
		{
			numberOfThreads = count;
		}
		int itemsPerThread = count / numberOfThreads;
		int process2 = itemsPerThread;
		for (int i = 0; i < numberOfThreads; i++)
		{
			if (i == numberOfThreads - 1)
			{
				process2 = count - i * itemsPerThread;
			}
			int number = i;
			int process = process2;
			Thread thread = new Thread(delegate()
			{
				BatchProcessor<T>.ActionAgainstMultiple(itemsToProcess, number * itemsPerThread, process, action);
			});
			list.Add(thread);
			thread.Start();
		}
		if (waitUntilAllThreadsFinish)
		{
			foreach (Thread thread2 in list)
			{
				thread2.Join();
			}
		}
	}

	private static void ActionAgainstMultiple(IList<T> items, int startIndex, int length, Action<T> action)
	{
		for (int i = startIndex; i < startIndex + length; i++)
		{
			T t = items[i];
			if (t != null)
			{
				action(t);
			}
			Thread.Sleep(1);
		}
	}
}
