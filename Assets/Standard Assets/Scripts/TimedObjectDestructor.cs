using System;
using UnityEngine;

[Serializable]
public class TimedObjectDestructor : MonoBehaviour
{
	public TimedObjectDestructor()
	{
		this.timeOut = 1f;
	}

	public virtual void Awake()
	{
		this.Invoke("DestroyNow", this.timeOut);
	}

	public virtual void DestroyNow()
	{
		if (this.detachChildren)
		{
			this.transform.DetachChildren();
		}
		UnityEngine.Object.DestroyObject(this.gameObject);
	}

	public virtual void Main()
	{
	}

	public float timeOut;

	public bool detachChildren;
}
