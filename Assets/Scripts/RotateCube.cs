using System;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(Vector3.up, (float)this.Degree_per_second * Time.deltaTime);
	}

	public int Degree_per_second;
}
