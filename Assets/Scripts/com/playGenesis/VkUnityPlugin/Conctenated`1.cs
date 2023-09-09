using System;

namespace com.playGenesis.VkUnityPlugin
{
	public class Conctenated<T>
	{
		public Conctenated(T element)
		{
			this.Element = element;
		}

		public T Element { get; set; }

		public Conctenated<T> NextElement { get; set; }
	}
}
