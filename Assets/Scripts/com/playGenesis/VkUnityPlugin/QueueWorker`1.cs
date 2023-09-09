using System;
using UnityEngine;

namespace com.playGenesis.VkUnityPlugin
{
	public class QueueWorker<T> : MonoBehaviour
	{
		protected void AddNextElement(Conctenated<T> target, Conctenated<T> member)
		{
			if (target.NextElement == null)
			{
				target.NextElement = member;
			}
			else
			{
				this.AddNextElement(target.NextElement, member);
			}
		}

		public void Add(T element)
		{
			if (this._current == null)
			{
				this._current = new Conctenated<T>(element);
				this.StartProcessing();
			}
			else
			{
				this.AddNextElement(this._current, new Conctenated<T>(element));
			}
		}

		protected virtual void StartProcessing()
		{
		}

		protected void ProccessNext()
		{
			this._current = this._current.NextElement;
			if (this._current == null)
			{
				return;
			}
			this.StartProcessing();
		}

		protected Conctenated<T> _current;
	}
}
