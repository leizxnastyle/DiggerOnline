using System;
using UnityEngine;

public class DeleteAfterTime : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("DeleteInst", this.life_time);
	}

	private void Update()
	{
	}

	private void DeleteInst()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public float life_time = 1f;
}
