using System;
using UnityEngine;

public class fxdestroyer : MonoBehaviour
{
	private void Start()
	{
		UnityEngine.Object.Destroy(base.gameObject, this.time);
	}

	public float time = 1f;
}
