using System;
using UnityEngine;

public class pnl_Scaller : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		this.ScaleTo();
	}

	private void ScaleTo()
	{
		if (base.transform.localScale != this.scaleTo)
		{
			base.transform.localScale = this.scaleTo;
		}
	}

	public Vector3 scaleTo;
}
