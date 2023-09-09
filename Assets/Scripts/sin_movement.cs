using System;
using UnityEngine;

public class sin_movement : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		base.transform.position += this.amplitude * (Mathf.Sin(6.28318548f * this.frequency * Time.time) - Mathf.Sin(6.28318548f * this.frequency * (Time.time - Time.deltaTime))) * this.dir;
	}

	public float amplitude;

	public float frequency;

	private Vector3 dir = new Vector3(0f, 1f, 0f);
}
