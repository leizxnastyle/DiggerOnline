using System;
using UnityEngine;

[AddComponentMenu("Camera-Control/Smooth Look At")]
[Serializable]
public class SmoothLookAt : MonoBehaviour
{
	public SmoothLookAt()
	{
		this.damping = 6f;
		this.smooth = true;
	}

	public virtual void LateUpdate()
	{
		if (this.target)
		{
			if (this.smooth)
			{
				Quaternion b = Quaternion.LookRotation(this.target.position - this.transform.position);
				this.transform.rotation = Quaternion.Slerp(this.transform.rotation, b, Time.deltaTime * this.damping);
			}
			else
			{
				this.transform.LookAt(this.target);
			}
		}
	}

	public virtual void Start()
	{
		if (this.GetComponent<Rigidbody>())
		{
			this.GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	public virtual void Main()
	{
	}

	public Transform target;

	public float damping;

	public bool smooth;
}
