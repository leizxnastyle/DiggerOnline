using System;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
	private void Update()
	{
		Vector3 b = Camera.main.transform.position - base.transform.position;
		b.x = (b.z = 0f);
		base.transform.LookAt(Camera.main.transform.position - b);
	}
}
