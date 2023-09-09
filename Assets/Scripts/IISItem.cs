using System;

public interface IISItem
{
	T GetItemsFromType<T>(T obj);
}
