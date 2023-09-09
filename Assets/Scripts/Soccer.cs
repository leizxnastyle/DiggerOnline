using System;
using UnityEngine;

public class Soccer : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if ((base.transform.position - this.plChar.transform.position).magnitude < (float)this.threshold)
		{
			base.GetComponent<Rigidbody>().AddForce((base.transform.position - this.plChar.transform.position).normalized * (float)this.power);
		}
	}

	public Transform plChar;

	public int threshold;

	public int power;
}
