using System;
using System.Collections.Generic;

public interface IBatchProcessor<T>
{
	void Process(IList<T> itemsToProcess, Action<T> action, bool waitUntilAllThreadsFinishh);

	void Process(int numberOfThreads, IList<T> itemsToProcess, Action<T> action, bool waitUntilAllThreadsFinish);
}
