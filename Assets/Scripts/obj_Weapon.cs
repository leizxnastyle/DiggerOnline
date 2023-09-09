using System;
using UnityEngine;

public class obj_Weapon : MonoBehaviour
{
	private void Start()
	{
		base.transform.localPosition = this.set_pos;
		base.transform.localEulerAngles = this.set_rot;
		base.transform.localScale = this.set_scale;
	}

	public Vector3 set_pos;

	public Vector3 set_rot;

	public Vector3 set_scale;
}
