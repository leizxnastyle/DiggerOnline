using System;
using UnityEngine;

public class TuttorialTrigger : MonoBehaviour
{
	public void Set(int tId, Vector3 pos, Vector3 rot)
	{
		this.nextId = tId;
		base.transform.position = pos;
		base.transform.eulerAngles = rot;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name.Contains("MAIN_PLAYER"))
		{
			TutorialManager.Inst.ToNext();
		}
	}

	private int nextId;
}
